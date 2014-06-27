// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.FileBox
{
    static partial class Program
    {
        #region Methods (2)

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

        #endregion Methods (2)
    }
}