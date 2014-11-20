// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using MarcelJoachimKloubert.CLRToolbox.IO.Console;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace MarcelJoachimKloubert.ApplicationServer.TestHost
{
    internal static class Program
    {
        #region Methods (1)

        private static void Main(string[] args)
        {
            GlobalConsole.SetConsole(new SystemConsole(isSynchronized: true));

            using (var server = new ApplicationServer())
            {
                // service locator
                CompositionContainer container;
                DelegateServiceLocator serviceLocator;
                {
                    var catalog = new AggregateCatalog();
                    catalog.Catalogs.Add(new AssemblyCatalog(server.GetType().Assembly));

                    container = new CompositionContainer(catalog: catalog,
                                                         isThreadSafe: true);

                    var mefServiceLocator = new ExportProviderServiceLocator(provider: container);

                    serviceLocator = new DelegateServiceLocator(baseLocator: mefServiceLocator);

                    container.ComposeExportedValue<global::MarcelJoachimKloubert.CLRToolbox.ServiceLocation.IServiceLocator>(serviceLocator);
                    container.ComposeExportedValue<global::MarcelJoachimKloubert.ApplicationServer.IApplicationServer>(server);

                    ServiceLocator.SetLocator(serviceLocator);
                }

                // context
                var ctx = new ApplicationServerContext()
                {
                    Logger = AsyncLogger.Create(logger: new AggregateLogger(provider: server.GetLoggers,
                                                                            isSynchronized: true)),
                    RootDirectory = Path.GetFullPath(Environment.CurrentDirectory),
                    Server = server,
                    ServiceLocator = serviceLocator,
                };

                container.ComposeExportedValue<global::MarcelJoachimKloubert.ApplicationServer.IApplicationServerContext>(ctx);

                server.Initialize(ctx);
                server.Start();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("===== ENTER =====");
                Console.ReadLine();
            }
        }

        #endregion Methods (1)
    }
}