// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using System;

namespace MarcelJoachimKloubert.FileBox.Server.Security
{
    internal sealed class ServerIdentity : IdentifiableBase, IServerIdentity
    {
        #region Fields (1)

        private Guid _id;

        #endregion Fields (1)

        #region Constructors (2)

        internal ServerIdentity()
            : this(id: Guid.NewGuid())
        {
        }

        internal ServerIdentity(Guid id)
        {
            this._id = id;
        }

        #endregion Constructors (2)

        #region Properties (4)

        public string AuthenticationType
        {
            get;
            internal set;
        }

        public override Guid Id
        {
            get { return this._id; }
        }

        public bool IsAuthenticated
        {
            get;
            internal set;
        }

        public string Name
        {
            get;
            internal set;
        }

        #endregion Properties (4)
    }
}