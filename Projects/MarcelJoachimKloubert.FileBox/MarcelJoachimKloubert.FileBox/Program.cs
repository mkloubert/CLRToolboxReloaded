// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.FileBox
{
    internal static class Program
    {
        #region Methods (6)

        private static void Inbox(ExecutionContext ctx)
        {
        }

        private static void Main(string[] args)
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
        }

        private static void Outbox(ExecutionContext ctx)
        {
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
                var fi = new FileInfo(f);
                if (fi.Exists)
                {
                    conn.Send(fi.FullName, recipients);
                }
                else
                {
                    //TODO report file not found
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

        #endregion Methods (6)
    }
}