// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web
{
    /// <summary>
    /// Describes a web interface request context.
    /// </summary>
    public interface IWebExecutionContext : IObject
    {
        #region Properties (2)

        /// <summary>
        /// Gets the underlying HTTP request context.
        /// </summary>
        IHttpRequest Request { get; }

        /// <summary>
        /// Gets the underlying HTTP response context.
        /// </summary>
        IHttpResponse Response { get; }

        #endregion Properties (2)
    }
}