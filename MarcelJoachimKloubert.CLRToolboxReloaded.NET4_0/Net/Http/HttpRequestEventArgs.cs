// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http
{
    /// <summary>
    /// Arguments for an event that works with HTTP requests and response data.
    /// </summary>
    public class HttpRequestEventArgs : EventArgs
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestEventArgs"/> class.
        /// </summary>
        /// <param name="req">The value for the <see cref="HttpRequestEventArgs.Request" /> property.</param>
        /// <param name="resp">The value for the <see cref="HttpRequestEventArgs.Response" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" /> and/or <paramref name="resp" /> are <see langword="null" />.
        /// </exception>
        public HttpRequestEventArgs(IHttpRequest req, IHttpResponse resp)
        {
            if (req == null)
            {
                throw new ArgumentNullException("req");
            }

            if (resp == null)
            {
                throw new ArgumentNullException("resp");
            }

            this.Request = req;
            this.Response = resp;
        }

        #endregion Constructors

        #region Properties (3)

        /// <summary>
        /// Gets or sets if the request has been handled or not.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the underlying request context.
        /// </summary>
        public IHttpRequest Request
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the underlying response context.
        /// </summary>
        public IHttpResponse Response
        {
            get;
            private set;
        }

        #endregion Properties
    }
}