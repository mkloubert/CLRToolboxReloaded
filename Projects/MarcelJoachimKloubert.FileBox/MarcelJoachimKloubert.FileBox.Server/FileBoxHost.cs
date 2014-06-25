// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Configuration;
using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Execution.Jobs;
using MarcelJoachimKloubert.FileBox.Server.Json;
using MarcelJoachimKloubert.FileBox.Server.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;

namespace MarcelJoachimKloubert.FileBox.Server
{
    /// <summary>
    /// A FileBox host.
    /// </summary>
    public sealed partial class FileBoxHost : DisposableObjectBase, IRunnable, IInitializable
    {
        #region Fields (6)

        private const string _CONFIG_CATEGORY_DIRS = "directories";
        private ConcurrentQueue<IJob> _jobs;
        private FileBoxHttpServer _server;
        private JobScheduler _scheduler;

        /// <summary>
        /// .NET format for a long time string.
        /// </summary>
        public const string LONG_TIME_FORMAT = "u";

        /// <summary>
        /// String format for GUIDs.
        /// </summary>
        public const string GUID_FORMAT = "N";

        #endregion Fields (6)

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
        {
            this.Config = config ?? new KeyValuePairConfigRepository();
            this.Port = 5979;
            this.UseSecureConnections = true;
        }

        #endregion Constructors (2)

        #region Events (1)

        /// <inheriteddoc />
        public event EventHandler Initialized;

        #endregion Events (1)

        #region Properties (11)

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

        /// <inheriteddoc />
        public int Port
        {
            get;
            set;
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

        /// <inheriteddoc />
        public bool UseSecureConnections
        {
            get;
            set;
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

        #endregion Properties (11)

        #region Methods (25)

        private static object AssemblyToJson(Assembly asm)
        {
            if (asm == null)
            {
                return null;
            }

            return new
                {
                    name = asm.FullName,
                };
        }

        private void DisposeOldScheduler()
        {
            try
            {
                using (var s = this._scheduler)
                {
                    this._scheduler = null;
                }
            }
            catch
            {
                // ignore here
            }
        }

        private void DisposeOldServer()
        {
            try
            {
                using (var srv = this._server)
                {
                    if (srv != null)
                    {
                        srv.RequestValidator = (r) => false;
                        srv.CredentialValidator = (u, p) => false;
                        srv.PrincipalFinder = (i) => null;

                        srv.Stop();

                        srv.HandleRequest -= this.HttpListenerServer_HandleRequest;
                    }

                    this._server = null;
                }
            }
            catch (Exception ex)
            {
                this.OnErrorsReceived(ex);
            }
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

            this._jobs
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

        private IEnumerable<IJob> GetNextJobs(IJobScheduler scheduler)
        {
            IJob result;
            while (this._jobs.TryDequeue(out result))
            {
                yield return result;
            }
        }

        private void HttpListenerServer_HandleRequest(object sender, HttpRequestEventArgs e)
        {
            Action<HttpRequestEventArgs> actionToInvoke = null;

            var addr = e.Request.Address;
            if (addr != null)
            {
                var url = addr.AbsolutePath.ToLower().Trim();
                if (url != null)
                {
                    url = url.ToLower().Trim();

                    while (url.StartsWith("/"))
                    {
                        url = url.Substring(1).Trim();
                    }
                }

                switch (url)
                {
                    case "list-inbox":
                        actionToInvoke = this.ListInbox;
                        break;

                    case "list-outbox":
                        actionToInvoke = this.ListOutbox;
                        break;

                    case "receive-file-inbox":
                        actionToInvoke = this.ReceiveInboxFile;
                        break;

                    case "receive-file-outbox":
                        actionToInvoke = this.ReceiveOutboxFile;
                        break;

                    case "send-file":
                        actionToInvoke = this.SendFile;
                        break;

                    case "server-info":
                        actionToInvoke = this.ServerInfo;
                        break;

                    case "update-key":
                        actionToInvoke = this.UpdateKey;
                        break;
                }
            }

            if (actionToInvoke != null)
            {
                actionToInvoke(e);
            }
            else
            {
                e.Response.DocumentNotFound = true;
            }
        }

        private IPrincipal HttpListenerServer_PrincipalFinder(IIdentity id)
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

        private bool HttpListenerServer_RequestValidator(IHttpRequest request)
        {
            return true;
        }

        private bool HttpListenerServer_UsernamePasswordValidator(string username, string password)
        {
            var id = new ServerIdentity()
                {
                    Name = username,
                };

            var princ = (ServerPrincipal)this.HttpListenerServer_PrincipalFinder(id);
            if (princ != null)
            {
                //TODO: check password
                return true;
            }

            return false;
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

        private void ListBox(HttpRequestEventArgs e, string boxPath)
        {
            if (e.Request.TryGetKnownMethod() != HttpMethod.GET)
            {
                e.Response.StatusCode = HttpStatusCode.MethodNotAllowed;
                return;
            }

            var result = new JsonResult();

            try
            {
                result.code = 0;

                var files = new List<object>();
                var sender = (IServerPrincipal)e.Request.User;
                var rsa = sender.TryGetRsaCrypter();

                var boxDir = new DirectoryInfo(boxPath);
                if (boxDir.Exists)
                {
                    boxDir.GetFiles("*" + GlobalConstants.FileExtensions.META_FILE)
                          .ForAll(throwExceptions: false,
                                  action: ctx =>
                                      {
                                          var metaFile = ctx.Item;

                                          ulong index;
                                          if (ulong.TryParse(Path.GetFileNameWithoutExtension(metaFile.Name), out index) == false)
                                          {
                                              // must be a valid number
                                              return;
                                          }

                                          var metaPwdFile = new FileInfo(Path.Combine(metaFile.DirectoryName, index.ToString() + GlobalConstants.FileExtensions.META_PASSWORD_FILE));
                                          if (metaPwdFile.Exists == false)
                                          {
                                              // no password file for meta data found
                                              return;
                                          }

                                          var dataFile = new FileInfo(Path.Combine(metaFile.DirectoryName, index.ToString() + GlobalConstants.FileExtensions.DATA_FILE));
                                          if (dataFile.Exists == false)
                                          {
                                              // no data file found
                                              return;
                                          }

                                          ctx.State.FileList.Add(new
                                              {
                                                  name = index.ToString(),

                                                  meta = new
                                                      {
                                                          dat = Convert.ToBase64String(File.ReadAllBytes(metaFile.FullName)),
                                                          sec = Convert.ToBase64String(File.ReadAllBytes(metaPwdFile.FullName)),
                                                      },
                                              });
                                      },
                                  actionState: new
                                      {
                                          MetaFileEncoding = new UTF8Encoding(),
                                          FileList = files,
                                      });
                }

                result.data = new
                    {
                        files = files.ToArray(),
                        key = rsa != null ? rsa.ToXmlString(includePrivateParameters: false) : null,
                    };
            }
            catch (Exception ex)
            {
                SetupJsonResultByException(result, ex);
            }

            e.Response.WriteJson(result);
        }

        private static object MethodToJson(MethodBase method)
        {
            if (method == null)
            {
                return null;
            }

            return new
                {
                    name = method.Name,
                    type = TypeToJson(method.DeclaringType),
                };
        }

        private void ReceiveBoxFile(HttpRequestEventArgs e, string boxPath)
        {
            if (e.Request.TryGetKnownMethod() != HttpMethod.GET)
            {
                e.Response.StatusCode = HttpStatusCode.MethodNotAllowed;
                return;
            }

            var fileFound = false;

            var fileName = (e.Request.Headers["X-FileBox-File"] ?? string.Empty).Trim();

            ulong index;
            if (ulong.TryParse(fileName, out index))
            {
                var boxDir = new DirectoryInfo(boxPath);
                if (boxDir.Exists)
                {
                    var dataFile = new FileInfo(Path.Combine(boxDir.FullName, index.ToString() + GlobalConstants.FileExtensions.DATA_FILE));
                    var metaFile = new FileInfo(Path.Combine(boxDir.FullName, index.ToString() + GlobalConstants.FileExtensions.META_FILE));
                    var metaPwdFile = new FileInfo(Path.Combine(boxDir.FullName, index.ToString() + GlobalConstants.FileExtensions.META_PASSWORD_FILE));

                    if (dataFile.Exists &&
                        metaFile.Exists &&
                        metaPwdFile.Exists)
                    {
                        fileFound = true;

                        using (var stream = dataFile.OpenRead())
                        {
                            stream.CopyTo(e.Response.Stream);
                        }
                    }
                }
            }

            if (fileFound == false)
            {
                e.Response.DocumentNotFound = true;
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

        private static void SetupJsonResultByException(JsonResult result, Exception ex)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            result.code = -1;
            result.msg = null;
            result.data = null;

            if (ex == null)
            {
                return;
            }

            var innerEx = ex.GetBaseException() ?? ex;

            object stacktrace;
            try
            {
                StackTrace st;
#if DEBUG
                st = new StackTrace(e: ex, fNeedFileInfo: true);
#else
                st = new StackTrace(e: ex);
#endif

                stacktrace = st.GetFrames()
                               .Select(f =>
                               {
                                   return new
                                   {
                                       column = f.GetFileColumnNumber(),
                                       file = f.GetFileName(),
                                       line = f.GetFileLineNumber(),
                                       method = MethodToJson(f.GetMethod()),
                                   };
                               }).ToArray();
            }
            catch
            {
                stacktrace = innerEx.StackTrace;
            }

            result.msg = innerEx.Message;
            result.data = new
            {
                fullMsg = ex.ToString(),
                stackTrace = stacktrace,
                type = TypeToJson(innerEx.GetType()),
            };
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

            this.DisposeOldServer();

            var oldJobQueue = this._jobs;
            try
            {
                this._jobs = new ConcurrentQueue<IJob>();

                var newScheduler = this._scheduler = new JobScheduler(this.GetNextJobs);
                if (newScheduler.IsInitialized == false)
                {
                    newScheduler.Initialize();
                }

                var newServer = this._server = new FileBoxHttpServer(host: this);
                newServer.PrincipalFinder = this.HttpListenerServer_PrincipalFinder;
                newServer.CredentialValidator = this.HttpListenerServer_UsernamePasswordValidator;
                newServer.RequestValidator = this.HttpListenerServer_RequestValidator;
                newServer.Port = this.Port;
                newServer.UseSecureHttp = this.UseSecureConnections;
                newServer.HandleRequest += this.HttpListenerServer_HandleRequest;

                newScheduler.Start();
                newServer.Start();
            }
            catch
            {
                this.DisposeOldScheduler();
                this.DisposeOldServer();

                this._jobs = oldJobQueue;

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

            this.DisposeOldServer();
            this.IsRunning = false;
        }

        private void ThrowIfNotInitialized()
        {
            if (this.IsInitialized == false)
            {
                throw new InvalidOperationException("Host is NOT initialized yet!");
            }
        }

        internal void TryDeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            this.EnqueueJob(new DeleteFileJob(filePath: path));
        }

        internal void TryDeleteFile(FileInfo file)
        {
            if (file == null)
            {
                return;
            }

            this.TryDeleteFile(path: file.FullName);
        }

        private static DateTimeOffset? TryParseTime(string str)
        {
            DateTimeOffset? result = null;

            try
            {
                if (string.IsNullOrWhiteSpace(str) == false)
                {
                    DateTimeOffset temp;
                    if (DateTimeOffset.TryParseExact(str.Trim(), LONG_TIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out temp))
                    {
                        result = temp;
                    }
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        private static object TypeToJson(Type type)
        {
            if (type == null)
            {
                return null;
            }

            return new
                {
                    assembly = AssemblyToJson(type.Assembly),
                    name = type.FullName,
                };
        }

        #endregion Methods (25)
    }
}