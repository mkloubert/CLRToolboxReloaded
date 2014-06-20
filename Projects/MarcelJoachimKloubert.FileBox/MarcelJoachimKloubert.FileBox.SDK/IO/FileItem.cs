// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox.IO
{
    /// <summary>
    /// Stores the data of a file (item).
    /// </summary>
    public sealed class FileItem
    {
        #region Properties (3)

        /// <summary>
        /// Gets the (machine) name of the server.
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the underlying server.
        /// </summary>
        public FileBoxConnection Server
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        public long Size
        {
            get;
            internal set;
        }

        #endregion Properties (3)
    }
}