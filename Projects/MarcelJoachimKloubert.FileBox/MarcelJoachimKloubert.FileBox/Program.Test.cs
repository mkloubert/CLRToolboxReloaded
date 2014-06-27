// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox
{
    static partial class Program
    {
        #region Methods (1)

        private static void Test(ExecutionContext ctx)
        {
#if DEBUG
            var conn = new FileBoxConnection();
            conn.Host = "localhost";
            conn.Port = 23979;
            conn.IsSecure = false;

            conn.User = "mkloubert";

            conn.Send(@"E:\Dev\fb\send\TortoiseGit-1.8.9.0-64bit.msi", new string[] { "tm" });
#endif
        }

        #endregion Methods (1)
    }
}