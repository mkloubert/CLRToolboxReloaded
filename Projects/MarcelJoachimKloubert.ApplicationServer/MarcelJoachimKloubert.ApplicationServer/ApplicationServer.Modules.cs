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

        private DirectoryInfo GetServiceModuleDirectory()
        {
            return new DirectoryInfo(Path.Combine(this.Context.RootDirectory,
                                                  "modules"));
        }

        private void LoadAndInitializeServiceModules()
        {
            const string LOG_CATEGORY = "LOAD_SERVICE_MODULES";

            var dir = this.GetServiceModuleDirectory();
            if (dir.Exists == false)
            {
                this.Logger.Log(categories: LogCategories.Warnings,
                                tag: LOG_CATEGORY,
                                msg: "Module directory does NOT exist!");

                return;
            }

            IList<object> modulesConf;
            {
                this.Config.TryGetValue(category: "services", name: "modules",
                                        value: out modulesConf);

                modulesConf = modulesConf ?? new List<object>();
            }

            var modules = new List<IServiceModule>();

            var ex = dir.EnumerateFiles("*.dll")
                        .ForAll(action: (ctx) =>
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

                                ctx.State.Server
                                         .FillCompositionContainerWithCommonExportValues(container);

                                var mefServiceLocator = new ExportProviderServiceLocator(provider: container);

                                serviceLocator = new DelegateServiceLocator(baseLocator: mefServiceLocator);
                            }

                            var ex2 = serviceLocator.GetAllInstances<IServiceModule>()
                                                    .ForAll(action: ctx2 =>
                                                    {
                                                        var module = ctx2.Item;

                                                        var modCtx = new ServiceModuleContext()
                                                        {
                                                            Assembly = ctx2.State.Assembly,
                                                            AssemblyHash = ctx2.State.AssemblyHash,
                                                            AssemblyLocation = ctx2.State.AssemblyLocation,
                                                            GetOtherModulesFunc = CreateGetOtherModulesFunc(module, ctx2.State.AllModules),
                                                            Module = module,
                                                            ServiceLocator = ctx2.State.ServiceLocator,
                                                        };

                                                        if (module.IsInitialized == false)
                                                        {
                                                            module.Initialize(modCtx);
                                                        }

                                                        if (module.IsInitialized)
                                                        {
                                                            ctx2.State.AllModules.Add(module);
                                                        }
                                                        else
                                                        {
                                                            ctx2.State.Server.Logger.Log(categories: LogCategories.Warnings,
                                                                                         tag: LOG_CATEGORY,
                                                                                         msg: string.Format("Module '{0}' from '{1}' could NOT be initialized!",
                                                                                                            module.GetType().FullName,
                                                                                                            ctx2.State.AssemblyLocation));
                                                        }
                                                    }, actionState: new
                                                    {
                                                        AllModules = ctx.State.Modules,
                                                        Assembly = asm,
                                                        AssemblyHash = asmHash,
                                                        AssemblyLocation = f.FullName,
                                                        ModuleConfig = ctx.State.ModuleConfig,
                                                        Server = ctx.State.Server,
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

        private void UnloadAndDisposeServiceModules()
        {
            const string LOG_CATEGORY = "UNLOAD_SERVICE_MODULES";

            var modules = this.ServiceModules;
            if (modules == null)
            {
                return;
            }

            var ex = modules.ForAll(action: (ctx) =>
                        {
                            var m = ctx.Item;

                            if (m.IsDisposed == false)
                            {
                                m.Dispose();
                            }
                        }, throwExceptions: false);

            if (ex != null)
            {
                this.Logger.Log(categories: LogCategories.Errors,
                                tag: LOG_CATEGORY,
                                msg: string.Format("Error while unloading modules: {0}",
                                                   ex));
            }
            else
            {
                this.Logger.Log(categories: LogCategories.Information,
                                tag: LOG_CATEGORY,
                                msg: string.Format("{0} service modules were unloaded.",
                                                   modules.Length));
            }
        }

        #endregion Methods (4)
    }
}