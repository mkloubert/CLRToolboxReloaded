// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;

namespace MarcelJoachimKloubert.ApplicationServer
{
    /// <summary>
    /// The context of an <see cref="IApplicationServer" />.
    /// </summary>
    public interface IApplicationServerContext : IServiceLocator
    {
        #region Properties (3)

        /// <summary>
        /// Gets the underlying logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the full path of the root directory.
        /// </summary>
        string RootDirectory { get; }

        /// <summary>
        /// Gets the underlying server.
        /// </summary>
        IApplicationServer Server { get; }

        #endregion Properties (3)
    }
}