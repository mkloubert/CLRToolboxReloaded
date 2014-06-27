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
        #region Fields (2)

        private bool _isDisposed;
        private readonly Action<DisposeContext> _DISPOSE_ACTION;

        #endregion Fields (2)

        #region Constrcutors (5)

        /// <inheriteddoc />
        protected DisposableObjectBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._DISPOSE_ACTION = this.Dispose_ThreadSafe;
            }
            else
            {
                this._DISPOSE_ACTION = this.Dispose_NonThreadSafe;
            }
        }

        /// <inheriteddoc />
        protected DisposableObjectBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected DisposableObjectBase(object sync)
            : this(isSynchronized: true,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected DisposableObjectBase()
            : this(isSynchronized: true)
        {
        }

        /// <inheriteddoc />
        ~DisposableObjectBase()
        {
            this._DISPOSE_ACTION(DisposeContext.Finalizer);
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

        #region Methods (5)

        /// <inheriteddoc />
        public void Dispose()
        {
            this._DISPOSE_ACTION(DisposeContext.DisposeMethod);
            GC.SuppressFinalize(this);
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

        #endregion Methods (5)
    }
}