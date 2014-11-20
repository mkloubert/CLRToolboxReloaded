﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using System.Collections.Generic;
using System.Reflection;

namespace MarcelJoachimKloubert.ApplicationServer.Services
{
    /// <summary>
    /// The context of an <see cref="IServiceModule" />.
    /// </summary>
    public interface IServiceModuleContext : IServiceLocator
    {
        #region Properties (4)

        /// <summary>
        /// Gets the underlying assembly.
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// Gets the hash of <see cref="IServiceModuleContext.Assembly" />.
        /// </summary>
        byte[] AssemblyHash { get; }

        /// <summary>
        /// Gets the full path of the assembly if available.
        /// </summary>
        string AssemblyLocation { get; }

        /// <summary>
        /// Gets the underlying module.
        /// </summary>
        IServiceModule Module { get; }

        #endregion Properties (4)

        #region Methods (1)

        /// <summary>
        /// Returns all other modules that are also part of <see cref="IServiceModuleContext.Assembly" />.
        /// </summary>
        /// <returns>The other modules.</returns>
        IEnumerable<IServiceModule> GetOtherModules();

        #endregion Methods (1)
    }
}