// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// A basic server object.
    /// </summary>
    public abstract class ServerObjectBase : FileBoxObjectBase
    {
        #region Methods (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerObjectBase" /> class.
        /// </summary>
        protected ServerObjectBase()
        {
        }

        #endregion Methods (1)

        #region Properties (1)

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