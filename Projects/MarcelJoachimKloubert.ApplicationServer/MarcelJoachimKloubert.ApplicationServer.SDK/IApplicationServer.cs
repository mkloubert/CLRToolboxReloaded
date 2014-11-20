// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Services;
using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Configuration;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.ApplicationServer
{
    /// <summary>
    /// Describes an application server.
    /// </summary>
    public interface IApplicationServer : IDisposableObject, IInitializable<IApplicationServerContext>, INotifiable, IRunnable
    {
        #region Properties (2)

        /// <summary>
        /// Gets the current configuration of / for that server.
        /// </summary>
        IConfigRepository Config { get; }

        /// <summary>
        /// Gets the underlying context.
        /// </summary>
        IApplicationServerContext Context { get; }

        #endregion Properties (2)

        #region Methods (1)

        /// <summary>
        /// Enumerates the loaded service modules.
        /// </summary>
        /// <returns>The loaded services modules.</returns>
        IEnumerable<IServiceModule> GetServiceModules();

        #endregion Methods (1)
    }
}