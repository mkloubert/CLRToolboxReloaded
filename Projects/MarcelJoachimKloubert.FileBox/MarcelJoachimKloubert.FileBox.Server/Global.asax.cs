// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Configuration;
using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using MarcelJoachimKloubert.FileBox.Server.Execution.Jobs;
using MarcelJoachimKloubert.FileBox.Server.Handlers;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Web.Routing;

namespace MarcelJoachimKloubert.FileBox.Server
{
    /// <summary>
    /// The global application class.
    /// </summary>
    public class Global : global::System.Web.HttpApplication
    {
        #region Fields (3)

        private const string _CONFIG_CATEGORY_DIRS = "directories";
        private JobQueue _job_queue;
        private JobScheduler _scheduler;
        private SendHttpHandler _sendHttpHandler;

        #endregion Fields (3)

        #region Properties (8)

        /// <summary>
        /// Gets the root of the bin files.
        /// </summary>
        public string BinDirectory
        {
            get;
            private set;
        }

        public IConfigRepository Config
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the global MEF catalog.
        /// </summary>
        public AggregateCatalog GlobalCompositionCatalog
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the global MEF composition container.
        /// </summary>
        public CompositionContainer GlobalCompositionContainer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the global service locator.
        /// </summary>
        public DelegateServiceLocator GlobalServiceLocator
        {
            get;
            private set;
        }

        public IJobQueue JobQueue
        {
            get { return this._job_queue; }
        }

        /// <summary>
        /// Gets the root of the temp files.
        /// </summary>
        public string TempDirectory
        {
            get
            {
                string dir;
                this.Config.TryGetValue<string>(category: _CONFIG_CATEGORY_DIRS, name: "temp",
                                                value: out dir);

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
                                                value: out dir);

                return this.GetFullPath(dir);
            }
        }

        #endregion Properties (8)

        #region Methods (10)

        /// <inheriteddoc />
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        /// <inheriteddoc />
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        /// <inheriteddoc />
        protected void Application_Error(object sender, EventArgs e)
        {
        }

        /// <inheriteddoc />
        protected void Application_End(object sender, EventArgs e)
        {
            using (this._scheduler)
            {
                this._scheduler = null;
            }
        }

        /// <inheriteddoc />
        protected void Application_Start(object sender, EventArgs e)
        {
            this._job_queue = new JobQueue();

            this.BinDirectory = this.Server.MapPath("~/bin");
            var binDir = new DirectoryInfo(this.BinDirectory);

            var configFile = new FileInfo(Path.Combine(binDir.FullName, "config.json"));
            this.Config = new JsonFileConfigRepository(file: configFile,
                                                       isReadOnly: true);

            // service locator
            {
                this.GlobalCompositionCatalog = new AggregateCatalog();
                this.GlobalCompositionCatalog.Catalogs.Add(new AssemblyCatalog(typeof(__IDummyObject).Assembly));

                this.GlobalCompositionContainer = new CompositionContainer(this.GlobalCompositionCatalog,
                                                                           isThreadSafe: true);
                this.GlobalCompositionContainer.ComposeExportedValue(this);

                this.GlobalServiceLocator = new DelegateServiceLocator(baseLocator: new ExportProviderServiceLocator(this.GlobalCompositionContainer));

                // IJobQueue
                this.GlobalServiceLocator
                    .RegisterSingleProvider<global::MarcelJoachimKloubert.FileBox.Server.Execution.Jobs.IJobQueue>(this.GetJobQueue);

                ServiceLocator.SetLocator(this.GlobalServiceLocator);
            }

            // list inbox
            RouteTable.Routes.Add(new Route
                       (
                           "inbox",
                           new InboxHttpHandler(handler: this.CheckLogin)
                       ));

            // (server) info
            RouteTable.Routes.Add(new Route
                       (
                           "info",
                           new ServerInfoHttpHandler(handler: this.CheckLogin)
                       ));
            
            // list outbox
            RouteTable.Routes.Add(new Route
                       (
                           "outbox",
                           new OutboxHttpHandler(handler: this.CheckLogin)
                       ));
            
            // receive file (inbox)
            RouteTable.Routes.Add(new Route
                       (
                           "receiveinbox",
                           new ReceiveInboxHttpHandler(handler: this.CheckLogin)
                       ));

            // receive file (outbox)
            RouteTable.Routes.Add(new Route
                       (
                           "receiveoutbox",
                           new ReceiveOutboxHttpHandler(handler: this.CheckLogin)
                       ));

            // send file
            this._sendHttpHandler = new SendHttpHandler(handler: this.CheckLogin);
            RouteTable.Routes.Add(new Route
                       (
                           "send",
                           this._sendHttpHandler
                       ));

            // update RSA key
            RouteTable.Routes.Add(new Route
                       (
                           "updatekey",
                           new UpdateKeyHttpHandler(handler: this.CheckLogin)
                       ));

            this._scheduler = new JobScheduler(this.GetNextJobs);
            this._scheduler.Initialize();
            this._scheduler.Start();
        }

        private void CheckLogin(string username, string pwd,
                                ref IServerPrincipal user)
        {
            var userFilesPath = this.UserFileDirectory;
            var usrFilesDir = new DirectoryInfo(userFilesPath);

            foreach (var dir in usrFilesDir.GetDirectories())
            {
                if (dir.Name.ToLower().Trim() == username)
                {
                    user = ServerPrincipal.FromUsername(username);

                    break;
                }
            }
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
                    result = Path.GetFullPath(Path.Combine(this.BinDirectory, path));
                }
            }

            return result ?? this.BinDirectory;
        }

        private IJobQueue GetJobQueue(DelegateServiceLocator locator, object key)
        {
            return this.JobQueue;
        }

        private IEnumerable<IJob> GetNextJobs(IJobScheduler scheduler)
        {
            var queue = this._job_queue;
            if (queue == null)
            {
                yield break;
            }

            IJob job;
            while (queue.JOBS.TryDequeue(out job))
            {
                yield return job;
            }
        }

        /// <inheriteddoc />
        protected void Session_End(object sender, EventArgs e)
        {
        }

        /// <inheriteddoc />
        protected void Session_Start(object sender, EventArgs e)
        {
        }

        #endregion Methods (10)
    }
}