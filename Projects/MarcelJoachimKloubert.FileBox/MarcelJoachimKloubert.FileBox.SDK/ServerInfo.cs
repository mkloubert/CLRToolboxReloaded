// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Stores information about the current server.
    /// </summary>
    public sealed class ServerInfo
    {
        #region Properties (2)

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

        #endregion Properties (1)
    }
}