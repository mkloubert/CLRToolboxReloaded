// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Describes an object.
    /// </summary>
    public interface IObject
    {
        #region Properties (2)

        /// <summary>
        /// Gets an object that can be used for thread safe operations.
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// Gets or sets an object that should be linked with that instance.
        /// </summary>
        object Tag { get; set; }

        #endregion Properties (2)
    }
}