// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox
{
    internal static class Program
    {
        #region Methods (12)

        private static void Help(ExecutionContext ctx)
        {
            Action<ExecutionContext> actionToInvoke = null;

            if (ctx.Arguments.Count > 0)
            {
                var command = ctx.Arguments[0].ToLower().Trim();

                switch (command)
                {
                    case "list":
                        actionToInvoke = Help_List;
                        break;

                    case "send":
                        actionToInvoke = Help_Send;
                        break;
                }
            }

            if (actionToInvoke != null)
            {
                actionToInvoke(ctx);
            }
            else
            {
                ShowShortHelp();
            }
        }

        private static void Help_List(ExecutionContext ctx)
        {
            Console.WriteLine("Lists the files of a folder.");
            Console.WriteLine();
            Console.WriteLine("Usage:  filebox list [FOLDER]");
            Console.WriteLine();

            Console.WriteLine("The following values are available for [FOLDER]:");
            Console.WriteLine();
            Console.WriteLine("  inbox         List RECEIVED files.");
            Console.WriteLine("  outbox        List SEND files.");
            Console.WriteLine();
            Console.WriteLine(@"Example:  filebox inbox --host=""fb.kremlin.ru"" --user=""ejsnowden""");
        }

        private static void Help_Send(ExecutionContext ctx)
        {
            Console.WriteLine("Sends one or more files to one or more recipients.");
            Console.WriteLine();
            Console.WriteLine("Usage:  filebox send --to=[RECIPIENTS] [FILE]*");
            Console.WriteLine();
            Console.WriteLine("  [FILE]        Paths to one or more files to send.");
            Console.WriteLine();
            Console.WriteLine("The following options are available:");
            Console.WriteLine();
            Console.WriteLine("  --to=[RECIPIENTS]        A ';' separated list of recipients.");
            Console.WriteLine();
            Console.WriteLine(@"Example:  filebox send --to=""ejsnowden@kremlin.ru;wwputin@kremlin.ru""");
            Console.WriteLine(@"          ""C:\file1.pdf"" ""C:\file2.pdf""");
            Console.WriteLine(@"          --host=""fb.kremlin.ru"" --user=""tm""");
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

        private static void List(ExecutionContext ctx)
        {
            Action<ExecutionContext> actionToInvoke = Help_List;

            if (ctx.Arguments.Count > 0)
            {
                switch (ctx.Arguments[0].ToLower())
                {
                    case "inbox":
                        actionToInvoke = List_Inbox;
                        break;

                    case "outbox":
                        actionToInvoke = List_Outbox;
                        break;
                }
            }

            actionToInvoke(ctx);
        }

        private static void List_Box(Func<RSACryptoServiceProvider, int, int?, IEnumerable<FileItem>> func,
                                     ExecutionContext ctx)
        {
            var allFiles = func(ctx.RSA, 0, null);

            int? maxLength = allFiles.Select(f => f.RealName != null ? f.RealName.Length : 0)
                                     .Cast<int?>()
                                     .Max();

            foreach (var file in func(ctx.RSA, 0, null).OrderBy(f => f.Name))
            {
                var prefix = string.Format("[{0}] ",
                                           (file.RealName ?? string.Empty).PadLeft(maxLength.Value, ' '));
                var prefix2 = "".PadLeft(prefix.Length, ' ');

                // file name and
                // prefix if file item is marked as "corrupted"
                Invoke(() => Console.Write(prefix));
                if (file.IsCorrupted)
                {
                    Invoke(() => Console.Write("[CORRUPT] "),
                           foreColor: ConsoleColor.Yellow);
                }
                Invoke(() => Console.Write(file.IsCorrupted ? "???" : file.Name),
                       foreColor: ConsoleColor.White);
                Invoke(() => Console.WriteLine());

                // send date
                Invoke(() => Console.Write(prefix2));
                Invoke(() => Console.Write("Date: {0}",
                                           file.IsCorrupted ? "???" : file.SendTime.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")));
                Invoke(() => Console.WriteLine());

                // size
                Invoke(() => Console.Write(prefix2));
                Invoke(() => Console.Write("Size: {0}",
                                           file.IsCorrupted ? "???" : file.Size.ToString()));
                Invoke(() => Console.WriteLine());

                Invoke(() => Console.WriteLine());
            }
        }

        private static void List_Inbox(ExecutionContext ctx)
        {
            var conn = ctx.CreateConnection();

            List_Box(conn.GetInbox, ctx);
        }

        private static void List_Outbox(ExecutionContext ctx)
        {
            var conn = ctx.CreateConnection();

            List_Box(conn.GetOutbox, ctx);
        }

        private static int Main(string[] args)
        {
            PrintHeader();

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
                        case "help":
                            actionArgs = actionArgs.Skip(1);
                            actionToInvoke = Help;
                            break;

                        case "list":
                            actionArgs = actionArgs.Skip(1);
                            actionToInvoke = List;
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

        private static void PrintHeader()
        {
            var asm = Assembly.GetExecutingAssembly();
            var asmName = asm.GetName();

            Console.WriteLine("FileBox  {0}, for .NET 4 and Mono", asmName.Version);
            Console.WriteLine("Created by Marcel Joachim Kloubert <marcel.kloubert@gmx.net>");
            Console.WriteLine();
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

        private static void ShowShortHelp()
        {
            Console.WriteLine("Usage:  filebox [COMMAND] [OPTIONS]*");
            Console.WriteLine();

            Console.WriteLine("The following commands are available:");
            Console.WriteLine();
            Console.WriteLine("  help [COMMAND]        Shows details of a command");
            Console.WriteLine("  list");
            Console.WriteLine("  send");

            Console.WriteLine();

            Console.WriteLine("These options can be used to define the connection parameters:");
            Console.WriteLine();
            Console.WriteLine("  --host=[HOST]                 The IP or host address of the server.");
            Console.WriteLine("                                Default: 'localhost'");
            Console.WriteLine();
            Console.WriteLine("  --http                        Use UNSECURE http connection.");
            Console.WriteLine("                                Default: NOT set");
            Console.WriteLine();
            Console.WriteLine("  --https                       Use SECURE https connection.");
            Console.WriteLine("                                Default: set");
            Console.WriteLine();
            Console.WriteLine("  --password=[PASSWORD]         The password for the connection.");
            Console.WriteLine();
            Console.WriteLine("  --port=[PORT]                 The TCP port where the server listens on.");
            Console.WriteLine("                                Default: 5979");
        }

        #endregion Methods (12)
    }
}