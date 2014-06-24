// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using System.Security.Principal;

namespace MarcelJoachimKloubert.FileBox.Server.Security
{
    /// <summary>
    /// Describes a server identity.
    /// </summary>
    public interface IServerIdentity : IIdentity, IIdentifiable
    {
    }
}