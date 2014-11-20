// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;

namespace MarcelJoachimKloubert.ApplicationServer.Services
{
    /// <summary>
    /// Describes a service module.
    /// </summary>
    public interface IServiceModule : IDisposableObject, IIdentifiable, IInitializable<IServiceModuleContext>, INotifiable, IRunnable
    {
        #region Properties (1)

        /// <summary>
        /// Gets the underlying context.
        /// </summary>
        IServiceModuleContext Context { get; }

        #endregion Properties (1)
    }
}