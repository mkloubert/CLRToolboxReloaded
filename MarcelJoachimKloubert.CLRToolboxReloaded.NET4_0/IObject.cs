// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// The mother of all objects.
    /// </summary>
    public interface IObject
    {
        #region Data members (3)

        /// <summary>
        /// Gets if that object works thread safe or not.
        /// </summary>
        bool Synchronized { get; }

        /// <summary>
        /// Gets the object for thread safe operations.
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// Gets or sets an object or value that should be linked with that instance.
        /// </summary>
        object Tag { get; set; }

        #endregion Data members (3)

        #region Events (1)

        /// <summary>
        /// Is invoked after one or more errors have been received.
        /// </summary>
        event EventHandler<ErrorEventArgs> ErrorsReceived;

        #endregion Events (1)
    }
}