// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http
{
    /// <summary>
    /// Extension of <see cref="HttpRequestEventArgs" /> for handling error data.
    /// </summary>
    public class HttpRequestErrorEventArgs : HttpRequestEventArgs
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestErrorEventArgs"/> class.
        /// </summary>
        /// <param name="req">The value for the <see cref="HttpRequestEventArgs.Request" /> property.</param>
        /// <param name="resp">The value for the <see cref="HttpRequestEventArgs.Response" /> property.</param>
        /// <param name="ex">The value for the <see cref="HttpRequestErrorEventArgs.Exception" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" />, <paramref name="resp" /> and/or <paramref name="ex" /> are <see langword="null" />.
        /// </exception>
        public HttpRequestErrorEventArgs(IHttpRequest req, IHttpResponse resp, Exception ex)
            : base(req, resp)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            this.Exception = ex;
        }

        #endregion Constructors

        #region Properties (1)

        /// <summary>
        /// Gets the occured exception.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }

        #endregion Properties
    }
}