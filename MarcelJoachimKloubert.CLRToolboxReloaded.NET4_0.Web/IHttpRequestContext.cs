// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Security.Principal;
using System.Web;

namespace MarcelJoachimKloubert.CLRToolbox.Web
{
    /// <summary>
    /// Describes a context of a HTTP request.
    /// </summary>
    public interface IHttpRequestContext : IObject
    {
        #region Properties (3)

        /// <summary>
        /// Gets the underlying ASP.NET HTTP context.
        /// </summary>
        HttpContext Http { get; }

        /// <summary>
        /// Gets the time of the request.
        /// </summary>
        DateTimeOffset Time { get; }

        /// <summary>
        /// Gets the logged user.
        /// </summary>
        IPrincipal User { get; }

        #endregion Properties (3)

        #region Methods (1)

        /// <summary>
        /// Returns the value of <see cref="IHttpRequestContext.User" /> property strong typed.
        /// </summary>
        /// <typeparam name="P">The target type.</typeparam>
        /// <returns>The converted / casted instance of <see cref="IHttpRequestContext.User" />.</returns>
        P GetUser<P>() where P : global::System.Security.Principal.IPrincipal;

        #endregion Methods (1)
    }
}