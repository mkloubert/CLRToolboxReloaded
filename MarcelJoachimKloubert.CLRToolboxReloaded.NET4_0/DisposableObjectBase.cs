// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// A basic implementation of <see cref="IDisposableObject" />.
    /// </summary>
    public abstract partial class DisposableObjectBase : ObjectBase, IDisposableObject
    {
        #region Fields (1)

        private bool _isDisposed;

        #endregion Fields (1)

        #region Constrcutors (5)

        /// <inheriteddoc />
        protected DisposableObjectBase(bool synchronized, object sync)
            : base(synchronized: synchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected DisposableObjectBase(bool synchronized)
            : base(synchronized: synchronized)
        {
        }

        /// <inheriteddoc />
        protected DisposableObjectBase(object sync)
            : base(synchronized: true,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected DisposableObjectBase()
            : base(synchronized: true)
        {
        }

        /// <inheriteddoc />
        ~DisposableObjectBase()
        {
            this.Dispose(DisposeContext.Finalizer);
        }

        #endregion Constrcutors (5)

        #region Properties (1)

        /// <inheriteddoc />
        public bool IsDisposed
        {
            get { return this._isDisposed; }
        }

        #endregion Properties (1)

        #region Properties (2)

        /// <inheriteddoc />
        public event EventHandler Disposed;

        /// <inheriteddoc />
        public event EventHandler Disposing;

        #endregion Properties (2)

        #region Methods (6)

        /// <inheriteddoc />
        public void Dispose()
        {
            this.Dispose(DisposeContext.DisposeMethod);
            GC.SuppressFinalize(this);
        }

        private void Dispose(DisposeContext ctx)
        {
            Action<DisposeContext> disposeAction;
            if (this._SYNCHRONIZED == false)
            {
                disposeAction = this.Dispose_NonThreadSafe;
            }
            else
            {
                disposeAction = this.Dispose_ThreadSafe;
            }

            disposeAction(ctx);
        }

        private void Dispose_NonThreadSafe(DisposeContext ctx)
        {
            if ((ctx == DisposeContext.DisposeMethod) &&
                this._isDisposed)
            {
                return;
            }

            if (ctx == DisposeContext.DisposeMethod)
            {
                this.RaiseEventHandler(this.Disposing);
            }

            this.OnDispose(ctx);

            if (ctx == DisposeContext.DisposeMethod)
            {
                this._isDisposed = true;
                this.RaiseEventHandler(this.Disposed);
            }
        }

        private void Dispose_ThreadSafe(DisposeContext ctx)
        {
            lock (this._SYNC)
            {
                this.Dispose_NonThreadSafe(ctx);
            }
        }

        /// <summary>
        /// The logic for the <see cref="DisposableObjectBase.Dispose()" /> method and the finalizer.
        /// </summary>
        /// <param name="ctx">The underlying context.</param>
        protected virtual void OnDispose(DisposeContext ctx)
        {
            // dummy
        }

        /// <summary>
        /// Throws an exception if that object has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Object has been disposed (<see cref="DisposableObjectBase.IsDisposed" /> is <see langword="true" />).
        /// </exception>
        protected void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw new ObjectDisposedException(objectName: this.GetType().FullName);
            }
        }

        #endregion Methods (6)
    }
}