// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox
{
    partial class DisposableObjectBase
    {
        #region ENUM: DisposeContext

        /// <summary>
        /// List of possible contextes for <see cref="DisposableObjectBase.OnDispose(DisposeContext)" /> method.
        /// </summary>
        protected enum DisposeContext
        {
            /// <summary>
            /// <see cref="IDisposable.Dispose()" /> method.
            /// </summary>
            DisposeMethod,

            /// <summary>
            /// Destructor / finalizer
            /// </summary>
            Finalizer,
        }

        #endregion ENUM: DisposeContext
    }
}