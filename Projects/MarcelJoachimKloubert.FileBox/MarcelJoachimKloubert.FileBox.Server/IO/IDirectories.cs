// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox.Server.IO
{
    /// <summary>
    /// List of directories.
    /// </summary>
    public interface IDirectories
    {
        #region Properties (2)

        /// <summary>
        /// Gets the root directoryóf the user files.
        /// </summary>
        string Files { get; }

        /// <summary>
        /// Gets the root directory of the temp files.
        /// </summary>
        string Temp { get; }

        #endregion Properties (2)
    }
}