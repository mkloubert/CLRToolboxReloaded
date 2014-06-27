// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox.Server.Net
{
    /// <summary>
    /// Stores the data of a TCP connection.
    /// </summary>
    public sealed class TcpHostConnection
    {
        #region Properties (3)

        /// <summary>
        /// Gets or sets if the connection should be open / active or not.
        /// </summary>
        public bool IsActive
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets if the connection should be secure or not.
        /// </summary
        public bool IsSecure
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the TCP port for the connection.
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        #endregion Properties (3)
    }
}