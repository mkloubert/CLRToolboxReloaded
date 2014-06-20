// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using Antlr.Runtime.Misc;
using MarcelJoachimKloubert.CLRToolbox;
using System.Security.Principal;

namespace MarcelJoachimKloubert.FileBox.Server.Security
{
    internal sealed class ServerPrincipal : ObjectBase, IServerPrincipal
    {
        #region Properties (3)

        public IServerIdentity Identity
        {
            get;
            internal set;
        }

        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; }
        }

        internal Func<string, bool> IsInRolePredicate
        {
            get;
            set;
        }

        #endregion Properties (3)

        #region Methods (1)

        public bool IsInRole(string role)
        {
            return this.IsInRolePredicate(role);
        }

        #endregion Methods (1)
    }
}