// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox.Server.IO
{
    /// <summary>
    /// List of directories.
    /// </summary>
    public interface IDirectories
    {
        #region Properties (1)

        /// <summary>
        /// Gets the root directory if the files.
        /// </summary>
        string Files { get; }

        #endregion Properties (1)
    }
}