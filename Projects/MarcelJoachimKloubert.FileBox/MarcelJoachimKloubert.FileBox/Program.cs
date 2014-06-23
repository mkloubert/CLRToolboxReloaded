// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox
{
    internal static class Program
    {
        #region Methods (9)

        private static void Box(Func<RSACryptoServiceProvider, int, int?, IEnumerable<FileItem>> func,
                                ExecutionContext ctx)
        {
            var allFiles = func(ctx.RSA, 0, null);

            int maxLength = allFiles.Select(f => f.RealName != null ? f.RealName.Length : 0)
                                    .Max();

            foreach (var file in func(ctx.RSA, 0, null).OrderBy(f => f.Name))
            {
                Console.WriteLine("[{0}] {1}",
                                  (file.RealName ?? string.Empty).PadLeft(maxLength, ' '),
                                  file.IsCorrupted ? "?????" : file.Name);
            }
        }

        private static void Inbox(ExecutionContext ctx)
        {
            var conn = ctx.CreateConnection();

            Box(conn.GetInbox, ctx);
        }
       
        private static void Invoke(Action action,
                                   ConsoleColor? foreColor = null, ConsoleColor? bgColor = null)
        {
            Invoke<Action>(action: (a) => a(),
                           actionState: action,
                           foreColor: foreColor, bgColor: bgColor);
        }

        private static void Invoke<S>(Action<S> action, S actionState,
                                      ConsoleColor? foreColor = null, ConsoleColor? bgColor = null)
        {
            var oldForeColor = Console.ForegroundColor;
            var oldBgColor = Console.BackgroundColor;

            try
            {
                if (foreColor.HasValue)
                {
                    Console.ForegroundColor = foreColor.Value;
                }

                if (bgColor.HasValue)
                {
                    Console.BackgroundColor = bgColor.Value;
                }

                action(actionState);
            }
            finally
            {
                Console.BackgroundColor = oldBgColor;
                Console.ForegroundColor = oldForeColor;
            }
        }

        private static int Main(string[] args)
        {
            int result;

            try
            {
                IEnumerable<string> normalizedArgs = args.Where(a => string.IsNullOrWhiteSpace(a) == false)
                                                         .Select(a => a.TrimStart());

                Action<ExecutionContext> actionToInvoke = null;
                IEnumerable<string> actionArgs = normalizedArgs;

                if (normalizedArgs.Count() > 0)
                {
                    switch (normalizedArgs.First().ToLower().Trim())
                    {
                        case "inbox":
                            actionArgs = actionArgs.Skip(1);
                            actionToInvoke = Inbox;
                            break;

                        case "outbox":
                            actionArgs = actionArgs.Skip(1);
                            actionToInvoke = Outbox;
                            break;

                        case "send":
                            actionArgs = actionArgs.Skip(1);
                            actionToInvoke = Send;
                            break;
                    }
                }

                if (actionToInvoke != null)
                {
                    var ctx = new ExecutionContext();
                    ctx.Host = "localhost";
                    ctx.Port = 5979;
                    ctx.IsSecure = true;

                    var keyFile = new FileInfo(Path.Combine(Environment.CurrentDirectory, "key.xml"));
                    if (keyFile.Exists)
                    {
                        var rsa = new RSACryptoServiceProvider();
                        using (var stream = keyFile.OpenRead())
                        {
                            var xmlDoc = XDocument.Load(stream);

                            rsa.FromXmlString(xmlDoc.Root.ToString());
                        }

                        ctx.RSA = rsa;
                    }

                    foreach (var a in normalizedArgs.Skip(1))
                    {
                        if (a.StartsWith("--user="))
                        {
                            ctx.Username = a.Substring(7).ToLower().Trim();
                        }
                        else if (a.StartsWith("--password="))
                        {
                            ctx.Password = a.Substring(11);
                        }
                        else if (a.StartsWith("--host="))
                        {
                            ctx.Host = a.Substring(7).Trim();
                        }
                        else if (a.StartsWith("--port="))
                        {
                            ctx.Port = int.Parse(a.Substring(7).Trim());
                        }
                        else if ((a.ToLower().Trim() == "--help") ||
                                 (a.ToLower().Trim() == "/?"))
                        {
                            actionToInvoke = ShowLongHelp;
                        }
                        else if (a.ToLower().Trim() == "--http")
                        {
                            ctx.IsSecure = false;
                        }
                        else if (a.ToLower().Trim() == "--https")
                        {
                            ctx.IsSecure = true;
                        }
                    }

                    ctx.Arguments = normalizedArgs.Skip(1)
                                                  .ToList();

                    actionToInvoke(ctx);
                }
                else
                {
                    ShowShortHelp();
                }

                result = 0;
            }
            catch (Exception ex)
            {
                result = 1;

                var innerEx = ex.GetBaseException() ?? ex;

                Invoke((e) =>
                       {
                           Console.WriteLine();
                           Console.WriteLine();

                           Console.WriteLine(e);
                       }, innerEx
                        , foreColor: ConsoleColor.Yellow, bgColor: ConsoleColor.Red);
            }

#if DEBUG
            global::System.Console.WriteLine();
            global::System.Console.WriteLine();
            global::System.Console.WriteLine("===== ENTER =====");

            global::System.Console.ReadLine();
#endif

            return result;
        }

        private static void Outbox(ExecutionContext ctx)
        {
            var conn = ctx.CreateConnection();

            Box(conn.GetOutbox, ctx);
        }

        private static void Send(ExecutionContext ctx)
        {
            var files = new HashSet<string>();
            var recipients = new HashSet<string>();

            foreach (var a in ctx.Arguments)
            {
                if (a.StartsWith("--to="))
                {
                    foreach (var r in a.Substring(5)
                                       .Split(';')
                                       .Select(s => s.ToLower().Trim())
                                       .Where(s => s != string.Empty))
                    {
                        recipients.Add(r);
                    }
                }
                else if (a.StartsWith("--") == false)
                {
                    var file = a.Trim();

                    if (file != string.Empty)
                    {
                        if (Path.IsPathRooted(file) == false)
                        {
                            file = Path.GetFullPath(file);
                        }

                        file = Path.GetFullPath(file);
                        files.Add(file);
                    }
                }
            }

            var conn = ctx.CreateConnection();

            foreach (var f in files)
            {
                Invoke(() => Console.Write("Sending file '"),
                       foreColor: ConsoleColor.Gray);
                Invoke((p) => Console.Write(Path.GetFileName(p)), f,
                       foreColor: ConsoleColor.White);
                Invoke(() => Console.Write("'... "),
                       foreColor: ConsoleColor.Gray);
                try
                {
                    var fi = new FileInfo(f);
                    if (fi.Exists)
                    {
                        conn.Send(fi.FullName, recipients);

                        Invoke(() => Console.WriteLine("[OK]"),
                               foreColor: ConsoleColor.Green);
                    }
                    else
                    {
                        Invoke(() => Console.WriteLine("[NOT FOUND!]"),
                               foreColor: ConsoleColor.Yellow);
                    }
                }
                catch (Exception ex)
                {
                    var innerEx = ex.GetBaseException() ?? ex;

                    Invoke((e) => Console.WriteLine("[ERROR: {0}]",
                                                    e.Message), innerEx,
                           foreColor: ConsoleColor.Red);
                }
            }
        }

        private static void ShowLongHelp(ExecutionContext ctx)
        {
            ShowShortHelp();
        }

        private static void ShowShortHelp()
        {
        }

        #endregion Methods (8)
    }
}