// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Services;
using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.ApplicationServer
{
    partial class ApplicationServer
    {
        #region Properties (1)

        /// <summary>
        /// Returns the current list of service modules.
        /// </summary>
        public IServiceModule[] ServiceModules
        {
            get { return this.Get<IServiceModule[]>(); }

            private set { this.Set(value); }
        }

        #endregion Properties (1)

        #region Methods (4)

        private static Func<IEnumerable<IServiceModule>> CreateGetOtherModulesFunc(IServiceModule module, IEnumerable<IServiceModule> others)
        {
            return new Func<IEnumerable<IServiceModule>>(() =>
                {
                    return others.Except(new IServiceModule[] { module });
                });
        }

        private DirectoryInfo GetServiceDirectory()
        {
            return new DirectoryInfo(Path.Combine(this.Context.RootDirectory,
                                                  "services"));
        }

        private void LoadAndInitializeServices()
        {
            const string LOG_CATEGORY = "LOAD_SERVICE_MODULES";

            var dir = this.GetServiceDirectory();
            if (dir.Exists == false)
            {
                //TODO log
                return;
            }

            IList<object> modulesConf;
            {
                this.Config.TryGetValue(category: "services", name: "modules",
                                        value: out modulesConf);

                modulesConf = modulesConf ?? new List<object>();
            }

            var modules = new SynchronizedCollection<IServiceModule>();

            var ex = dir.GetFiles("*.dll")
                        .ForAllAsync(action: (ctx) =>
                        {
                            var f = ctx.Item;
                            var asmBlob = File.ReadAllBytes(f.FullName);
                            var asm = Assembly.Load(asmBlob);

                            byte[] asmHash;
                            using (var md5 = new MD5CryptoServiceProvider())
                            {
                                asmHash = md5.ComputeHash(asmBlob);
                            }

                            // service locator
                            DelegateServiceLocator serviceLocator;
                            {
                                var catalog = new AggregateCatalog();
                                catalog.Catalogs.Add(new AssemblyCatalog(asm));

                                var container = new CompositionContainer(catalog: catalog,
                                                                         isThreadSafe: true);

                                var mefServiceLocator = new ExportProviderServiceLocator(provider: container);

                                serviceLocator = new DelegateServiceLocator(baseLocator: mefServiceLocator);
                            }

                            var ex2 = serviceLocator.GetAllInstances<IServiceModule>()
                                                    .ForAllAsync(action: ctx2 =>
                                                    {
                                                        var m = ctx2.Item;
                                                        var allMods = ctx2.State.AllModules;

                                                        //TODO

                                                        var modCtx = new ServiceModuleContext()
                                                        {
                                                            Assembly = ctx2.State.Assembly,
                                                            AssemblyHash = ctx2.State.AssemblyHash,
                                                            AssemblyLocation = ctx2.State.AssemblyLocation,
                                                            GetOtherModulesFunc = CreateGetOtherModulesFunc(m, allMods),
                                                            Module = m,
                                                            ServiceLocator = ctx2.State.ServiceLocator,
                                                        };

                                                        if (m.IsInitialized == false)
                                                        {
                                                            m.Initialize(modCtx);
                                                        }

                                                        if (m.IsInitialized)
                                                        {
                                                            allMods.Add(m);
                                                        }
                                                        else
                                                        {
                                                            //TODO: log
                                                        }
                                                    }, actionState: new
                                                    {
                                                        AllModules = ctx.State.Modules,
                                                        Assembly = asm,
                                                        AssemblyHash = asmHash,
                                                        AssemblyLocation = f.FullName,
                                                        ModuleConfig = ctx.State.ModuleConfig,
                                                        ServiceLocator = serviceLocator,
                                                    }, throwExceptions: false);

                            if (ex2 != null)
                            {
                                ctx.State.Server.Logger.Log(categories: LogCategories.Errors,
                                                            tag: LOG_CATEGORY,
                                                            msg: string.Format("Error while loading module from file '{0}': {1}",
                                                                               f.FullName,
                                                                               ex2));
                            }
                        }, actionState: new
                        {
                            ModuleConfig = modulesConf.Cast<IDictionary<string, object>>()
                                                      .Where(c => c != null)
                                                      .Select(c => (IDictionary<string, object>)new ConcurrentDictionary<string, object>(c))
                                                      .ToArray(),
                            Modules = modules,
                            Server = this,
                        }, throwExceptions: false);

            if (ex != null)
            {
                this.Logger.Log(categories: LogCategories.Errors,
                                tag: LOG_CATEGORY,
                                msg: string.Format("Error while loading modules: {0}",
                                                   ex));
            }

            this.ServiceModules = modules.ToArray();
            if (modules.Count > 0)
            {
                this.Logger.Log(categories: LogCategories.Information,
                                tag: LOG_CATEGORY,
                                msg: string.Format("Loaded {0} service modules.",
                                                   modules.Count));
            }
            else
            {
                this.Logger.Log(categories: LogCategories.Warnings,
                                tag: LOG_CATEGORY,
                                msg: "No modules loaded!");
            }
        }

        private void UnloadAndDisposeServices()
        {
            var modules = this.ServiceModules;
            if (modules == null)
            {
                return;
            }

            var ex = modules.ForAllAsync(action: (ctx) =>
                        {
                            var m = ctx.Item;

                            if (m.IsDisposed == false)
                            {
                                m.Dispose();
                            }
                        }, actionState: new
                        {
                        }, throwExceptions: false);

            if (ex != null)
            {
                // TODO log
            }
        }

        #endregion Methods (4)
    }
}