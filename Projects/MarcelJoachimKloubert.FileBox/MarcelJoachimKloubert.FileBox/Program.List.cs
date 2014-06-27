// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.FileBox
{
    static partial class Program
    {
        #region Methods (5)

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

        private static void List_Box(Func<RSACryptoServiceProvider, int, int?, IExecutionContext<List<FileItem>>> func,
                                     ExecutionContext ctx)
        {
            var execCtx = func(ctx.RSA, 0, null);
            execCtx.Start();

            var allFiles = execCtx.Result;

            int? maxLength = allFiles.Select(f => f.RealName != null ? f.RealName.Length : 0)
                                     .Cast<int?>()
                                     .Max();

            foreach (var file in allFiles.OrderBy(f => f.Name))
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

        #endregion Methods (5)
    }
}