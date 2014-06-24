// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http
{
    /// <summary>
    /// Simple implementation of <see cref="IHttpRequestContext" /> interface.
    /// </summary>
    public sealed class HttpRequestContext : ObjectBase, IHttpRequestContext
    {
        #region Properties (2)

        /// <inheriteddoc />
        public IHttpRequest Request
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public IHttpResponse Response
        {
            get;
            set;
        }

        #endregion Properties
    }
}