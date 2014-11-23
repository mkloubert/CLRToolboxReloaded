// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System.IO;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.ApplicationServer.Helpers
{
    /// <summary>
    /// Helper class for file operations.
    /// </summary>
    public static class FileHelper
    {
        #region Methods (2)

        /// <summary>
        /// Shredders and deletes a file in background.
        /// </summary>
        /// <param name="fs">The stream of the underlying file.</param>
        /// <returns>
        /// The underlying task or <see langword="null" /> if no task is running.
        /// </returns>
        public static Task ShredderAndDeleteFile(FileStream fs)
        {
            if (fs == null)
            {
                return null;
            }

            return Task.Factory.StartNew(action: ShredderFileStreamTaskAction,
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
                // ignore errors here
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
                    // ignore errors here
                }
            }
        }

        #endregion Methods (2)
    }
}