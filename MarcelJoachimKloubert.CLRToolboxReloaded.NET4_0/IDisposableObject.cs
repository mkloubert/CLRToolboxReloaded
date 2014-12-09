// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// Describes an extension of <see cref="IDisposable" /> interface.
    /// </summary>
    public interface IDisposableObject : IObject, IDisposable
    {
        #region Data members (1)

        /// <summary>
        /// Gets if that object works thread safe or not.
        /// </summary>
        bool IsDisposed { get; }

        #endregion Data members (1)

        #region Events (2)

        /// <summary>
        /// Is invoked AFTER that object has been disposed.
        /// </summary>
        event EventHandler Disposed;
        
        /// <summary>
        /// Is invoked BEFORE that object starts disposing.
        /// </summary>
        event EventHandler Disposing;

        #endregion Events (2)
    }
}