// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Net.Web;
using MarcelJoachimKloubert.ApplicationServer.Services;
using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Configuration;
using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        #region Properties (8)

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

        #endregion Properties (8)

        #region Methods (19)

        private static void CleanupDirectory(DirectoryInfo dir,
                                             bool deleteDirectory = false)
        {
            if (dir.Exists == false)
            {
                return;
            }

            try
            {
                // sub directories
                dir.EnumerateDirectories()
                   .ForAllAsync(action: ctx => CleanupDirectory(ctx.Item,
                                                                deleteDirectory: true),
                                throwExceptions: false);

                // files
                {
                    var tasks = dir.EnumerateFiles()
                               .Select(f => CreateShredderFileTask(f))
                               .ToArray();

                    tasks.ForAll(action: ctx => ctx.Item.Start(),
                                 throwExceptions: false);

                    Task.WaitAll(tasks);
                }

                if (deleteDirectory)
                {
                    dir.Refresh();
                    if (dir.Exists)
                    {
                        dir.Delete();
                    }
                }
            }
            catch
            {
            }
        }

        private static Task CreateShredderFileTask(FileInfo file)
        {
            return new Task(action: ShredderAndDeleteFile,
                            state: file);
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

                this.OnDispose(disposing);

                if (disposing)
                {
                    this.IsDisposed = true;
                    this.RaiseEventHandler(this.Disposed);
                }
            }
        }

        /// <summary>
        /// Fills a <see cref="CompositionContainer" /> instance with common export values.
        /// </summary>
        /// <param name="container">The instance to fill.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="container" /> is <see langword="null" />.
        /// </exception>
        public void FillCompositionContainerWithCommonExportValues(CompositionContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            container.ComposeExportedValue<global::MarcelJoachimKloubert.ApplicationServer.ApplicationServer>(this);
            container.ComposeExportedValue<global::MarcelJoachimKloubert.ApplicationServer.IApplicationServer>(this);

            container.ComposeExportedValue<global::MarcelJoachimKloubert.ApplicationServer.IApplicationServerContext>(this.Context);
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
            this._web_url_handler = new WebUrlHandler(this);

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

            this.CleanupTempDirectory();

            this.LoadAndInitializeServiceModules();

            this.StartWebInterface();

            this.IsRunning = true;

            this.Logger.Log(categories: LogCategories.Information,
                            tag: "START_SERVER",
                            msg: "Server is running now.");
        }

        private void OnStop(bool restarting)
        {
            if (this.IsRunning == false)
            {
                return;
            }

            this.StopWebInterface();

            this.UnloadAndDisposeServiceModules();

            this.IsRunning = false;

            this.Logger.Log(categories: LogCategories.Information,
                            tag: "STOP_SERVER",
                            msg: "Server has been stopped.");
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

        private static void ShredderAndDeleteFile(object state)
        {
            var file = (FileInfo)state;

            try
            {
                using (var fs = file.Open(FileMode.Open, FileAccess.ReadWrite))
                {
                    fs.Shredder();
                }
            }
            catch
            {
            }
            finally
            {
                try
                {
                    file.Refresh();
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                catch
                {
                }
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

        #endregion Methods (19)
    }
}