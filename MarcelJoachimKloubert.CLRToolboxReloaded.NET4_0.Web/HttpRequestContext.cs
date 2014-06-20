// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;
using System.Security.Principal;
using System.Web;

namespace MarcelJoachimKloubert.CLRToolbox.Web
{
    /// <summary>
    /// Simple implementation of the <see cref="IHttpRequestContext" /> interface.
    /// </summary>
    public sealed class HttpRequestContext : ObjectBase, IHttpRequestContext
    {
        #region Properties (3)

        /// <inheriteddoc />
        public HttpContext Http
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public DateTimeOffset Time
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IPrincipal User
        {
            get;
            set;
        }

        #endregion Properties (3)

        #region Methods (1)
        
        /// <inheriteddoc />
        public P GetUser<P>() where P : global::System.Security.Principal.IPrincipal
        {
            return GlobalConverter.Current
                                  .ChangeType<P>(value: this.User);
        }

        #endregion Methods (1)
    }
}