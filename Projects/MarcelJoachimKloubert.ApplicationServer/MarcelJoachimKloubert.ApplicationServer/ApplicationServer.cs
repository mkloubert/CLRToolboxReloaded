// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Services;
using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Configuration;
using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.ApplicationServer
{
    /// <summary>
    /// Implementation of a dedicated application server.
    /// </summary>
    public sealed partial class ApplicationServer : NotifiableBase, IApplicationServer
    {
        #region Constructors (1)

        /// <summary>
        /// The destructor / finalizer.
        /// </summary>
        ~ApplicationServer()
        {
            this.Dispose(false);
        }

        #endregion Constructors (1)

        #region Events and delegates (3)

        /// <inheriteddoc />
        public event EventHandler Disposed;

        /// <inheriteddoc />
        public event EventHandler Disposing;

        /// <inheriteddoc />
        public event EventHandler Initialized;

        #endregion Events and delegates (3)

        #region Properties (9)

        /// <inheriteddoc />
        public bool CanRestart
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public bool CanStart
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public bool CanStop
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public IConfigRepository Config
        {
            get { return this.Get<IConfigRepository>(); }

            private set { this.Set(value); }
        }

        /// <inheriteddoc />
        public IApplicationServerContext Context
        {
            get { return this.Get<IApplicationServerContext>(); }

            private set { this.Set(value); }
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

        #endregion Properties (9)

        #region Methods (15)

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

                this.OnDispose(disposing);

                if (disposing)
                {
                    this.IsDisposed = true;
                    this.RaiseEventHandler(this.Disposed);
                }
            }
        }

        /// <inheriteddoc />
        public IEnumerable<IServiceModule> GetServiceModules()
        {
            IEnumerable<IServiceModule> result;

            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                result = (this.ServiceModules ?? Enumerable.Empty<IServiceModule>());
            }

            return result;
        }

        void IInitializable.Initialize()
        {
            throw new InvalidOperationException("Need context for initializing process!");
        }

        /// <inheriteddoc />
        public void Initialize(IApplicationServerContext context)
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

        private void OnDispose(bool disposing)
        {
            this.OnStop(false);

            this.ServiceModules = null;
        }

        private void OnInitialize(IApplicationServerContext context)
        {
            this.AllLoggers = new ILogger[0];
            this.ServiceModules = new IServiceModule[0];
        }

        private void OnStart(bool restarting)
        {
            if (this.IsRunning)
            {
                return;
            }

            this.ReloadConfig();
            this.ReloadLoggers();

            this.LoadAndInitializeServices();

            this.Logger.Log(categories: LogCategories.Information,
                            tag: "START_SERVER",
                            msg: "Server is running now.");

            this.IsRunning = true;
        }

        private void OnStop(bool restarting)
        {
            if (this.IsRunning == false)
            {
                return;
            }

            this.UnloadAndDisposeServices();

            this.Logger.Log(categories: LogCategories.Information,
                            tag: "STOP_SERVER",
                            msg: "Server has been stopped.");

            this.IsRunning = false;
        }

        private void ReloadConfig()
        {
            IConfigRepository config = null;

            var dir = new DirectoryInfo(this.Context.RootDirectory);
            if (dir.Exists)
            {
                var file = new FileInfo(Path.Combine(dir.FullName, "config.json"));
                if (file.Exists)
                {
                    config = new JsonFileConfigRepository(file: file,
                                                          isReadOnly: false);
                }
            }

            this.Config = config ?? new KeyValuePairConfigRepository();
        }

        /// <inheriteddoc />
        public void Restart()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                this.OnStop(true);
                this.OnStart(true);
            }
        }

        /// <inheriteddoc />
        public void Start()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                this.OnStart(false);
            }
        }

        /// <inheriteddoc />
        public void Stop()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                this.OnStop(false);
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(objectName: this.GetType().FullName,
                                                  message: string.Format("Instance {0} has already been disposed!",
                                                                         this.GetType().GetHashCode()));
            }
        }

        private void ThrowIfNotInitialized()
        {
            if (this.IsInitialized == false)
            {
                throw new InvalidOperationException(message: string.Format("Instance {0} of class '{1}' has not been initialized yet!",
                                                                           this.GetType().GetHashCode(),
                                                                           this.GetType().FullName));
            }
        }

        #endregion Methods (15)
    }
}