// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.IO.Console;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.ApplicationServer.TestHost
{
    internal static class Program
    {
        #region Methods (3)

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            GlobalConsole.SetConsole(new SystemConsole(isSynchronized: true));

            var rootDir = Path.GetFullPath(Environment.CurrentDirectory);

            using (var server = new ApplicationServer())
            {
                var logger = AsyncLogger.Create(logger: new AggregateLogger(provider: server.GetLoggers,
                                                                            isSynchronized: true));

                // service locator
                CompositionContainer container;
                DelegateServiceLocator serviceLocator;
                {
                    var catalog = new AggregateCatalog();
                    catalog.Catalogs.Add(new AssemblyCatalog(server.GetType().Assembly));

                    container = new CompositionContainer(catalog: catalog,
                                                         isThreadSafe: true);

                    // additional external services
                    {
                        var serviceDir = new DirectoryInfo(Path.Combine(rootDir, "services"));
                        if (serviceDir.Exists)
                        {
                            var ex = serviceDir.EnumerateFiles("*.dll")
                                               .ForAll(action: faCtx =>
                                                   {
                                                       var asm = Assembly.LoadFrom(faCtx.Item.FullName);

                                                       faCtx.State.Catalogs.Add(new AssemblyCatalog(asm));
                                                   }, actionState: new
                                                   {
                                                       Catalogs = catalog.Catalogs,
                                                   }, throwExceptions: false);

                            if (ex != null)
                            {
                                logger.Log(categories: LogCategories.Errors,
                                           tag: "LOAD_EXTERNAL_SERVICES",
                                           msg: string.Format("Error while loading external services: {0}",
                                                              ex));
                            }
                        }
                    }

                    var mefServiceLocator = new ExportProviderServiceLocator(provider: container);

                    serviceLocator = new DelegateServiceLocator(baseLocator: mefServiceLocator);

                    container.ComposeExportedValue<global::MarcelJoachimKloubert.CLRToolbox.ServiceLocation.IServiceLocator>(serviceLocator);

                    ServiceLocator.SetLocator(serviceLocator);
                }

                // context
                var ctx = new ApplicationServerContext()
                {
                    Logger = logger,
                    RootDirectory = rootDir,
                    Server = server,
                    ServiceLocator = serviceLocator,
                };

                server.Initialize(ctx);
                server.FillCompositionContainerWithCommonExportValues(container);

                server.Start();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("===== ENTER =====");
                Console.ReadLine();
            }
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
        }

        #endregion Methods (3)
    }
}