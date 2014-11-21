// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.ComponentModel;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web
{
    /// <summary>
    /// Describes a web interface module.
    /// </summary>
    public interface IWebInterfaceModule : INotifiable, IIdentifiable
    {
        #region Properties (1)

        /// <summary>
        /// Gets the name of that module.
        /// </summary>
        string Name { get; }

        #endregion

        #region Methods (1)

        /// <summary>
        /// Handles a request.
        /// </summary>
        /// <param name="context">The underlying context.</param>
        void Handle(IWebExecutionContext context);

        #endregion
    }
}