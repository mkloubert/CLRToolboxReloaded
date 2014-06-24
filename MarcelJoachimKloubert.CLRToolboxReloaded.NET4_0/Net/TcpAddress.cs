// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Net
{
    /// <summary>
    /// Simple implementation of <see cref="ITcpAddress" /> interface.
    /// </summary>
    public sealed class TcpAddress : ObjectBase, ITcpAddress
    {
        #region Properties (2)

        /// <inheriteddoc />
        public string Address
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public int Port
        {
            get;
            set;
        }

        #endregion Properties (2)
    }
}