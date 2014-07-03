// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Describes an object that is a child of a (server) connection.
    /// </summary>
    public interface IConnectionChild : IObject
    {
        #region Properties (1)

        /// <summary>
        /// Gets the underlying server connection.
        /// </summary>
        IConnection Server { get; }

        #endregion Properties (1)
    }
}