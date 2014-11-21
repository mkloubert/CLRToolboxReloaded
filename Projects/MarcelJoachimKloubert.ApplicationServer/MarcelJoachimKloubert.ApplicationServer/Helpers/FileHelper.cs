// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System.IO;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.ApplicationServer.Helpers
{
    internal static class FileHelper
    {
        #region Methods (2)

        internal static void ShredderAndDeleteFile(FileStream fs)
        {
            if (fs == null)
            {
                return;
            }

            Task.Factory.StartNew(action: ShredderFileStreamTaskAction,
                                  state: fs);
        }

        private static void ShredderFileStreamTaskAction(object state)
        {
            var fs = (FileStream)state;

            try
            {
                fs.Position = 0;
                fs.Shredder();
            }
            catch
            {
            }
            finally
            {
                try
                {
                    fs.Dispose();
                    File.Delete(fs.Name);
                }
                catch
                {
                }
            }
        }

        #endregion Methods (2)
    }
}