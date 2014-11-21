// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using System.IO;

namespace MarcelJoachimKloubert.ApplicationServer
{
    partial class ApplicationServer
    {
        #region Methods (1)

        private void CleanupTempDirectory()
        {
            this.Logger.Log(categories: LogCategories.Information,
                            tag: "CLEANUP_TEMP",
                            msg: "Cleaning up temp directory...");

            CleanupDirectory(new DirectoryInfo(this.Context
                                                   .TempDirectory));
        }

        #endregion Methods (1)
    }
}