// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Security.Principal;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Listener
{
    partial class HttpListenerServer
    {
        #region Nested classes (1)

        private sealed class DummyIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get;
                internal set;
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
        }

        #endregion Nested classes (1)
    }
}