// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox
{
    internal static partial class Program
    {
        #region Methods (5)

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
            PrintHeader();

            int result;

            try
            {
                result = 0;

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

                        case "test":
                            actionArgs = actionArgs.Skip(1);
                            actionToInvoke = Test;
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

                    var inputPassword = false;

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
                        else if (a == "--password")
                        {
                            inputPassword = true;
                        }
                    }

                    var cancelled = false;
                    if (inputPassword)
                    {
                        Console.Write("Password: ");
                        ctx.Password = ReadPassword();

                        if (ctx.Password == string.Empty)
                        {
                            cancelled = true;
                        }
                    }

                    if (cancelled == false)
                    {
                        ctx.Arguments = normalizedArgs.Skip(1)
                                                  .ToList();

                        actionToInvoke(ctx);
                    }
                }
                else
                {
                    ShowShortHelp();
                }
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
            Console.WriteLine();
        }

        private static string ReadPassword()
        {
            var result = new StringBuilder();

            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (keyInfo.Key != ConsoleKey.Backspace)
                {
                    // append char

                    result.Append(keyInfo.KeyChar);
                    // Console.Write("*");
                }
                else
                {
                    // remove last char

                    if (result.Length > 0)
                    {
                        result.Remove(result.Length - 1, 1);
                        // Console.Write("\b \b");
                    }
                }
            }

            return result.ToString();
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
            Console.WriteLine("                                Default: not set");
            Console.WriteLine();
            Console.WriteLine("  --https                       Use SECURE https connection.");
            Console.WriteLine("                                Default: set");
            Console.WriteLine();
            Console.WriteLine("  --password                    The user has to enter the password");
            Console.WriteLine("                                for the connection.");
            Console.WriteLine();
            Console.WriteLine("  --password=[PASSWORD]         The password for the connection.");
            Console.WriteLine("                                Default: none");
            Console.WriteLine();
            Console.WriteLine("  --port=[PORT]                 The TCP port where the server listens on.");
            Console.WriteLine("                                Default: 5979");

            Console.WriteLine();

            Console.WriteLine(@"Example:  filebox list inbox --host=""fb.doe.com"" --password --user=""john""");
        }

        #endregion Methods (5)
    }
}