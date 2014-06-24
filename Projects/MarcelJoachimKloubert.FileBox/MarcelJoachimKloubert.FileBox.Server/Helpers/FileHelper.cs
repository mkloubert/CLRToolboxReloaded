// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using MarcelJoachimKloubert.FileBox.Server.Execution.Jobs;
using System.IO;

namespace MarcelJoachimKloubert.FileBox.Server.Helpers
{
    internal static class FileHelper
    {
        #region Methods (2)

        internal static void TryDeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            ServiceLocator.Current
                          .GetInstance<IJobQueue>()
                          .Enqueue(new DeleteFileJob(filePath: path));
        }

        internal static void TryDeleteFile(FileInfo file)
        {
            if (file == null)
            {
                return;
            }

            TryDeleteFile(path: file.FullName);
        }

        #endregion Methods (2)
    }
}