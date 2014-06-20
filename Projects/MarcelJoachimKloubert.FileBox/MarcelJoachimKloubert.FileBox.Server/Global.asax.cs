﻿// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Configuration;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using MarcelJoachimKloubert.FileBox.Server.Handlers;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace MarcelJoachimKloubert.FileBox.Server
{
    /// <summary>
    /// The global application class.
    /// </summary>
    public class Global : global::System.Web.HttpApplication
    {
        #region Fields (1)

        private const string _CONFIG_CATEGORY_DIRS = "directories";

        #endregion Fields (1)

        #region Properties (6)

        /// <summary>
        /// Gets the root of the bin files.
        /// </summary>
        public string BinDirectory
        {
            get
            {
                return HttpContext.Current.Server.MapPath("~/bin");
            }
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

        #endregion Properties (6)

        #region Methods (9)

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
        }

        /// <inheriteddoc />
        protected void Application_Start(object sender, EventArgs e)
        {
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
                    Guid id;
                    using (var md5 = new MD5CryptoServiceProvider())
                    {
                        id = new Guid(md5.ComputeHash(new UTF8Encoding().GetBytes(username)));
                    }

                    user = new ServerPrincipal()
                        {
                            Identity = new ServerIdentity(id: id)
                            {
                                AuthenticationType = "HttpBasicAuth",
                                IsAuthenticated = true,
                                Name = username,
                            },

                            IsInRolePredicate = (role) => false,
                        };

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

        /// <inheriteddoc />
        protected void Session_End(object sender, EventArgs e)
        {
        }

        /// <inheriteddoc />
        protected void Session_Start(object sender, EventArgs e)
        {
        }

        #endregion Methods (9)
    }
}