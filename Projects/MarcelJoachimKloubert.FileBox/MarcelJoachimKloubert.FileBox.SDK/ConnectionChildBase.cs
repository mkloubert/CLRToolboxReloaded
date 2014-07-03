// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// A basic child of a server connection.
    /// </summary>
    public abstract class ConnectionChildBase : ObjectBase, IConnectionChild
    {
        #region Methods (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionChildBase" /> class.
        /// </summary>
        protected ConnectionChildBase()
        {
        }

        #endregion Methods (1)

        #region Properties (2)

        /// <summary>
        /// Gets the underlying server.
        /// </summary>
        public FileBoxConnection Server
        {
            get;
            internal set;
        }

        IConnection IConnectionChild.Server
        {
            get { return this.Server; }
        }

        #endregion Properties (2)
    }
}