// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using System;

namespace MarcelJoachimKloubert.ApplicationServer.Services
{
    /// <summary>
    /// A basic service module.
    /// </summary>
    public abstract partial class ServiceModuleBase : NotifiableBase, IServiceModule
    {
        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceModuleBase" /> class.
        /// </summary>
        /// <param name="id">
        /// The value for the <see cref="ServiceModuleBase.Id" /> property.
        /// </param>
        protected ServiceModuleBase(Guid id)
        {
            this.Id = id;
        }

        /// <summary>
        /// The finalizer.
        /// </summary>
        ~ServiceModuleBase()
        {
            this.Dispose(false);
        }

        #endregion Constructors (2)

        #region Events and delegates (3)

        /// <inheriteddoc />
        public event EventHandler Disposed;

        /// <inheriteddoc />
        public event EventHandler Disposing;

        /// <inheriteddoc />
        public event EventHandler Initialized;

        #endregion Events and delegates (3)

        #region Properties (8)

        /// <inheriteddoc />
        public virtual bool CanRestart
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public virtual bool CanStart
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public virtual bool CanStop
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public IServiceModuleContext Context
        {
            get { return this.Get<IServiceModuleContext>(); }

            private set { this.Set(value); }
        }

        /// <inheriteddoc />
        public Guid Id
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public bool IsDisposed
        {
            get { return this.Get<bool>(); }

            private set { this.Set(value); }
        }

        /// <inheriteddoc />
        [ReceiveNotificationFrom("Context")]
        public bool IsInitialized
        {
            get { return this.Context != null; }
        }

        /// <inheriteddoc />
        public bool IsRunning
        {
            get { return this.Get<bool>(); }

            private set { this.Set(value); }
        }

        #endregion Properties (8)

        #region Methods (20)

        /// <summary>
        /// Is invoked BEFORE <see cref="ServiceModuleBase.OnDispose(bool, ref bool)" /> is called.
        /// </summary>
        /// <param name="disposing">
        /// <see cref="ServiceModuleBase.Dispose()" /> method is called (<see langword="true" />); otherwise
        /// the finalizer is called.
        /// </param>
        protected virtual void BeforeDispose(bool disposing)
        {
            if (this.CanStop)
            {
                this.OnStop(disposing ? StartStopContext.Dispose : StartStopContext.Finalizer);
            }
        }

        /// <inheriteddoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            lock (this._SYNC)
            {
                if (disposing && this.IsDisposed)
                {
                    return;
                }

                if (disposing)
                {
                    this.RaiseEventHandler(this.Disposing);
                }

                this.BeforeDispose(disposing);

                var isDisposed = disposing ? true : this.IsDisposed;
                this.OnDispose(disposing, ref isDisposed);

                this.IsDisposed = isDisposed;

                if (this.IsDisposed)
                {
                    this.RaiseEventHandler(this.Disposed);
                }
            }
        }

        /// <inheriteddoc />
        public bool Equals(Guid other)
        {
            return this.Id == other;
        }

        /// <inheriteddoc />
        public bool Equals(IIdentifiable other)
        {
            return other != null ? this.Equals(other.Id) : false;
        }

        /// <inheriteddoc />
        public override bool Equals(object other)
        {
            if (other is Guid)
            {
                return this.Equals((Guid)other);
            }

            if (other is IIdentifiable)
            {
                return this.Equals((IIdentifiable)other);
            }

            return base.Equals(other);
        }

        /// <inheriteddoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        void IInitializable.Initialize()
        {
            throw new InvalidOperationException("Need context for initializing process!");
        }

        /// <inheriteddoc />
        public void Initialize(IServiceModuleContext context)
        {
            lock (this._SYNC)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }

                this.ThrowIfDisposed();

                if (this.IsInitialized)
                {
                    throw new InvalidOperationException(message: string.Format("Instance {0} of class '{1}' has already been initialized!",
                                                                               this.GetType().GetHashCode(),
                                                                               this.GetType().FullName));
                }

                this.OnInitialize(context);

                this.Context = context;
                this.RaiseEventHandler(this.Initialized);
            }
        }

        /// <summary>
        /// Stores the logic for the <see cref="ServiceModuleBase.Dispose()" /> method and the finalizer.
        /// </summary>
        /// <param name="disposing">
        /// <see cref="ServiceModuleBase.Dispose()" /> method is called (<see langword="true" />); otherwise
        /// the finalizer is called.
        /// </param>
        protected virtual void OnDispose(bool disposing, ref bool isDisposed)
        {
            // dummy
        }

        /// <summary>
        /// Stores the logic for the <see cref="ServiceModuleBase.Initialize()" /> method.
        /// </summary>
        /// <param name="context">The underlying context.</param>
        protected virtual void OnInitialize(IServiceModuleContext context)
        {
            // dummy
        }

        private void OnStart(StartStopContext context)
        {
            if (this.CanStart == false)
            {
                throw new InvalidOperationException(message: string.Format("Instance {0} of class '{1}' cannot be started!",
                                                                           this.GetType().GetHashCode(),
                                                                           this.GetType().FullName));
            }

            if (this.IsRunning)
            {
                return;
            }

            var isRunning = true;
            this.OnStart(context, ref isRunning);

            this.IsRunning = isRunning;
        }

        /// <summary>
        /// Stores the logic for the <see cref="ServiceModuleBase.Restart()" /> and
        /// <see cref="ServiceModuleBase.Start()" /> methods.
        /// </summary>
        /// <param name="restarting">
        /// <see cref="ServiceModuleBase.Restart()" /> method is called (<see langword="true" />); otherwise
        /// the <see cref="ServiceModuleBase.Start()" /> method is called.
        /// </param>
        /// <param name="isRunning">
        /// The new value for the <see cref="ServiceModuleBase.IsRunning" /> property.
        /// That value is <see langword="true" /> by default.
        /// </param>
        protected virtual void OnStart(StartStopContext context,
                                       ref bool isRunning)
        {
            // dummy
        }

        private void OnStop(StartStopContext context)
        {
            if (this.CanStop == false)
            {
                throw new InvalidOperationException(message: string.Format("Instance {0} of class '{1}' cannot be stopped!",
                                                                           this.GetType().GetHashCode(),
                                                                           this.GetType().FullName));
            }

            if (this.IsRunning == false)
            {
                return;
            }

            var isRunning = false;
            this.OnStop(context, ref isRunning);

            this.IsRunning = isRunning;
        }

        /// <summary>
        /// Stores the logic for the <see cref="ServiceModuleBase.Restart()" /> and
        /// <see cref="ServiceModuleBase.Stop()" /> methods.
        /// </summary>
        /// <param name="restarting">
        /// <see cref="ServiceModuleBase.Restart()" /> method is called (<see langword="true" />); otherwise
        /// the <see cref="ServiceModuleBase.Stop()" /> method is called.
        /// </param>
        /// <param name="isRunning">
        /// The new value for the <see cref="ServiceModuleBase.IsRunning" /> property.
        /// That value is <see langword="false" /> by default.
        /// </param>
        protected virtual void OnStop(StartStopContext context,
                                      ref bool isRunning)
        {
            // dummy
        }

        /// <inheriteddoc />
        public void Restart()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                this.OnStop(StartStopContext.Restart);
                this.OnStart(StartStopContext.Restart);
            }
        }

        /// <inheriteddoc />
        public void Start()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                this.OnStart(StartStopContext.Start);
            }
        }

        /// <inheriteddoc />
        public void Stop()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                this.OnStop(StartStopContext.Stop);
            }
        }

        /// <summary>
        /// Throws an exception if that object has already been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Instance has already been disposed.
        /// </exception>
        protected void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(objectName: this.GetType().FullName,
                                                  message: string.Format("Instance {0} has already been disposed!",
                                                                         this.GetType().GetHashCode()));
            }
        }

        /// <summary>
        /// Throws an exception if that object has NOT been initialized yet.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Instance has not been initialized yet.
        /// </exception>
        protected void ThrowIfNotInitialized()
        {
            if (this.IsInitialized == false)
            {
                throw new InvalidOperationException(message: string.Format("Instance {0} of class '{1}' has not been initialized yet!",
                                                                           this.GetType().GetHashCode(),
                                                                           this.GetType().FullName));
            }
        }

        #endregion Methods (20)
    }
}