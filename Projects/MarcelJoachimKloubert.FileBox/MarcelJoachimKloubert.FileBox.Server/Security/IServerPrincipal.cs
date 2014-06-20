// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using System.Security.Principal;

namespace MarcelJoachimKloubert.FileBox.Server.Security
{
    /// <summary>
    /// Describes a server principal.
    /// </summary>
    public interface IServerPrincipal : IObject, IPrincipal
    {
        #region Properties (1)

        /// <inheriteddoc />
        new IServerIdentity Identity { get; }

        #endregion Properties
    }
}