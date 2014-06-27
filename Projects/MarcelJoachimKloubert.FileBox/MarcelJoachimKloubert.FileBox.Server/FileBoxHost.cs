// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Configuration;
using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Execution.Jobs;
using MarcelJoachimKloubert.FileBox.Server.Handlers;
using MarcelJoachimKloubert.FileBox.Server.Net;
using MarcelJoachimKloubert.FileBox.Server.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;

namespace MarcelJoachimKloubert.FileBox.Server
{
    /// <summary>
    /// A FileBox host.
    /// </summary>
    public partial class FileBoxHost : DisposableObjectBase, IRunnable, IInitializable
    {
        #region Fields (9)

        private readonly ConcurrentQueue<IJob> _ASYNC_JOBS = new ConcurrentQueue<IJob>();
        private JobScheduler _asyncJobScheduler;
        private HttpHandlerBase _clientToServerHandler;
        private const string _CONFIG_CATEGORY_DIRS = "directories";
        private const string _CONFIG_CATEGORY_HOST = "host";
        private const string _LOOPBACK = "127.0.0.1";
        private JobScheduler _jobScheduler;
        private readonly ConcurrentQueue<IJob> _JOBS = new ConcurrentQueue<IJob>();
        private HttpHandlerBase _serverToServerHandler;

        #endregion Fields (9)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBoxHost" /> class.
        /// </summary>
        public FileBoxHost()
            : this(config: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBoxHost" /> class.
        /// </summary>
        /// <param name="config">The configuration to use.</param>
        public FileBoxHost(IConfigRepository config)
            : base(isSynchronized: true)
        {
            this.Config = config ?? new KeyValuePairConfigRepository();

            this.ClientToServer = new TcpHostConnection()
                {
                    IsActive = true,
                    IsSecure = true,
                    Port = 5979,
                };

            this.ServerToServer = new TcpHostConnection()
                {
                    IsActive = true,
                    IsSecure = true,
                    Port = 5980,
                };
        }

        #endregion Constructors (2)

        #region Events (1)

        /// <inheriteddoc />
        public event EventHandler Initialized;

        #endregion Events (1)

        #region Properties (12)

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

        /// <summary>
        /// Gets the connection data for client to server connections.
        /// </summary>
        public TcpHostConnection ClientToServer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the underlying configuration.
        /// </summary>
        public IConfigRepository Config
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public bool IsRunning
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the primary host name of that host.
        /// </summary>
        public string PrimaryHostName
        {
            get
            {
                string name;
                this.Config.TryGetValue<string>(category: _CONFIG_CATEGORY_HOST, name: "name",
                                                value: out name,
                                                defaultVal: null);

                if (string.IsNullOrWhiteSpace(name))
                {
                    name = this.GetHostNames().FirstOrDefault();
                }

                return string.IsNullOrWhiteSpace(name) ? _LOOPBACK
                                                       : name.ToLower().Trim();
            }
        }

        /// <summary>
        /// Gets the connection data for server to server connections.
        /// </summary>
        public TcpHostConnection ServerToServer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the root of the temporary files.
        /// </summary>
        public string TempDirectory
        {
            get
            {
                string dir;
                this.Config.TryGetValue<string>(category: _CONFIG_CATEGORY_DIRS, name: "temp",
                                                value: out dir,
                                                defaultVal: null);

                if (string.IsNullOrWhiteSpace(dir))
                {
                    dir = Path.GetTempPath();
                }

                return this.GetFullPath(dir);
            }
        }

        /// <summary>
        /// Gets the root of the user files.
        /// </summary>
        public string UserFileDirectory
        {
            get
            {
                string dir;
                this.Config.TryGetValue<string>(category: _CONFIG_CATEGORY_DIRS, name: "userFiles",
                                                value: out dir,
                                                defaultVal: "./files");

                return this.GetFullPath(dir);
            }
        }

        /// <summary>
        /// Gets the working directory.
        /// </summary>
        public string WorkingDirectory
        {
            get
            {
                string dir;
                this.Config.TryGetValue<string>(category: _CONFIG_CATEGORY_DIRS, name: "work",
                                                value: out dir,
                                                defaultVal: null);

                if (string.IsNullOrWhiteSpace(dir))
                {
                    dir = Environment.CurrentDirectory;
                }
                else
                {
                    if (Path.IsPathRooted(dir) == false)
                    {
                        dir = Path.Combine(Environment.CurrentDirectory, dir);
                    }
                }

                return Path.GetFullPath(dir);
            }
        }

        #endregion Properties (12)

        #region Methods (24)

        private static void ClearQueue<T>(ConcurrentQueue<T> queue)
        {
            T item;
            while (queue.TryDequeue(out item))
            { }
        }

        private void Cleanup()
        {
            this.ClearJobQueues();
            this.DisposeAllHandlersAndSchedulers();
        }

        private void ClearJobQueues()
        {
            ClearQueue(this._ASYNC_JOBS);
            ClearQueue(this._JOBS);
        }

        private ServerPrincipal ClientToServerHandler_FindPrincipal(IIdentity id)
        {
            if (id == null)
            {
                return null;
            }

            var userFilesPath = this.UserFileDirectory;
            var usrFilesDir = new DirectoryInfo(userFilesPath);

            foreach (var dir in usrFilesDir.GetDirectories())
            {
                string name = (id.Name ?? string.Empty).ToLower().Trim();

                if (dir.Name.ToLower().Trim() == name)
                {
                    return ServerPrincipal.FromUsername(this, name);
                }
            }

            return null;
        }

        private bool ClientToServerHandler_Validate_Credentials(string username, string password)
        {
            var id = new ServerIdentity()
            {
                Name = username,
            };

            var princ = this.ClientToServerHandler_FindPrincipal(id);
            if (princ != null)
            {
                //TODO: check password
                return true;
            }

            return false;
        }

        private bool ClientToServerHandler_Validate_Request(IHttpRequest req)
        {
            return true;
        }

        private void DisposeAllHandlersAndSchedulers()
        {
            this.DisposeHttpHandler(ref this._serverToServerHandler);
            this.DisposeHttpHandler(ref this._clientToServerHandler);

            this.DisposeJobScheduler(ref this._jobScheduler);
            this.DisposeJobScheduler(ref this._asyncJobScheduler);
        }

        private void DisposeHttpHandler(ref HttpHandlerBase handler)
        {
            try
            {
                using (var h = handler)
                {
                    handler = null;
                }
            }
            catch (Exception ex)
            {
                this.OnErrorsReceived(ex);
            }
        }

        private void DisposeJobScheduler(ref JobScheduler scheduler)
        {
            try
            {
                using (var s = scheduler)
                {
                    scheduler = null;
                }
            }
            catch (Exception ex)
            {
                this.OnErrorsReceived(ex);
            }
        }

        /// <summary>
        /// Enqueues a new job for running async.
        /// </summary>
        /// <param name="job">The job to enqueue.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="job" /> is <see langword="null" />.
        /// </exception>
        public void EnqueueAsyncJob(IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job");
            }

            this._ASYNC_JOBS
                .Enqueue(job);
        }

        /// <summary>
        /// Enqueues a new job.
        /// </summary>
        /// <param name="job">The job to enqueue.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="job" /> is <see langword="null" />.
        /// </exception>
        public void EnqueueJob(IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job");
            }

            this._JOBS
                .Enqueue(job);
        }

        private string GetFullPath(string path)
        {
            string result = null;

            if (string.IsNullOrWhiteSpace(path) == false)
            {
                if (Path.IsPathRooted(path))
                {
                    result = Path.GetFullPath(path);
                }
                else
                {
                    result = Path.GetFullPath(Path.Combine(this.WorkingDirectory, path));
                }
            }

            return result ?? this.WorkingDirectory;
        }

        /// <summary>
        /// Returns the DNS names that host is using.
        /// </summary>
        /// <returns>The list of DNS names for/of that host.</returns>
        public List<string> GetHostNames()
        {
            var result = new HashSet<string>();

            // DNS host name
            string hostName = null;
            try
            {
                hostName = Dns.GetHostName().ToLower().Trim();

                result.Add(hostName);
            }
            catch
            {
                // ignore here
            }

            // IP addresses
            try
            {
                Dns.GetHostAddresses(string.IsNullOrWhiteSpace(hostName) ? _LOOPBACK : hostName)
                   .ForAll(ctx =>
                        {
                            ctx.State
                               .HostAddresses.Add(ctx.Item
                                                     .ToString().ToLower().Trim());
                        }, new
                        {
                            HostAddresses = result,
                        });
            }
            catch
            {
                // ignore here
            }

            // machine name from environment
            try
            {
                result.Add(Environment.MachineName.ToLower().Trim());
            }
            catch
            {
                // ignore here
            }

            // clenup and normalize before return items
            return result.Where(h => (string.IsNullOrWhiteSpace(h) == false) &&
                                     (h != _LOOPBACK) &&
                                     (h != "localhost"))
                         .ToList();
        }

        private IEnumerable<IJob> GetNextAsyncJobs(IJobScheduler scheduler)
        {
            IJob result;
            while (this._ASYNC_JOBS.TryDequeue(out result))
            {
                yield return result;
            }
        }

        private IEnumerable<IJob> GetNextJobs(IJobScheduler scheduler)
        {
            IJob result;
            while (this._JOBS.TryDequeue(out result))
            {
                yield return result;
            }
        }

        /// <inheriteddoc />
        public void Initialize()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();

                if (this.IsInitialized)
                {
                    throw new InvalidOperationException();
                }

                this.IsInitialized = true;
                this.RaiseEventHandler(this.Initialized);
            }
        }

        /// <inheriteddoc />
        public void Restart()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                this.StopInner();
                this.StartInner();
            }
        }

        /// <inheriteddoc />
        public void Start()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                this.StartInner();
            }
        }

        private void StartInner()
        {
            if (this.IsRunning)
            {
                return;
            }

            this.Cleanup();

            try
            {
                // job scheudlers
                {
                    // sync jobs
                    var newJobScheduler = this._jobScheduler = new JobScheduler(provider: this.GetNextJobs);
                    if (newJobScheduler.IsInitialized == false)
                    {
                        newJobScheduler.Initialize();
                    }
                    newJobScheduler.Start();

                    // ASYNC jobs
                    var newAsyncJobScheduler = this._asyncJobScheduler = new AsyncJobScheduler(provider: this.GetNextJobs);
                    if (newAsyncJobScheduler.IsInitialized == false)
                    {
                        newAsyncJobScheduler.Initialize();
                    }
                    newAsyncJobScheduler.Start();
                }

                // client => server handler
                if (this.ClientToServer.IsActive)
                {
                    // HTTP server
                    var newServer = new FileBoxHttpServer(host: this);
                    newServer.CredentialValidator = this.ClientToServerHandler_Validate_Credentials;
                    newServer.Port = this.ClientToServer.Port;
                    newServer.PrincipalFinder = this.ClientToServerHandler_FindPrincipal;
                    newServer.RequestValidator = this.ClientToServerHandler_Validate_Request;
                    newServer.UseSecureHttp = this.ClientToServer.IsSecure;

                    // handler
                    var newClientToServerHandler = this._clientToServerHandler = new ClientToServerHttpHandler(this, newServer);
                    newClientToServerHandler.Start();
                }

                // server => server handler
                if (this.ServerToServer.IsActive)
                {
                    // HTTP server
                    var newServer = new FileBoxHttpServer(host: this);
                    newServer.Port = this.ServerToServer.Port;
                    newServer.UseSecureHttp = this.ServerToServer.IsSecure;

                    // handler
                    var newServerToServerHandler = this._serverToServerHandler = new ServerToServerHttpHandler(this, newServer);
                    newServerToServerHandler.Start();
                }
            }
            catch
            {
                // cleanup before rethrow exception
                this.Cleanup();

                throw;
            }

            this.IsRunning = true;
        }

        /// <inheriteddoc />
        public void Stop()
        {
            lock (this._SYNC)
            {
                this.ThrowIfDisposed();
                this.ThrowIfNotInitialized();

                this.StopInner();
            }
        }

        private void StopInner()
        {
            if (this.IsRunning == false)
            {
                return;
            }

            this.DisposeAllHandlersAndSchedulers();

            this.IsRunning = false;
        }

        private void ThrowIfNotInitialized()
        {
            if (this.IsInitialized == false)
            {
                throw new InvalidOperationException("Host is NOT initialized yet!");
            }
        }

        internal void TryDeleteFile(FileInfo file)
        {
            if (file == null)
            {
                return;
            }

            this.TryDeleteFile(path: file.FullName);
        }

        internal void TryDeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            this.EnqueueAsyncJob(new DeleteFileJob(filePath: path));
        }

        #endregion Methods (24)
    }
}