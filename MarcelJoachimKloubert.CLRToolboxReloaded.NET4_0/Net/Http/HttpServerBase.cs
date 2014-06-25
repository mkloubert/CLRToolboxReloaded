// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Security;
using System;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http
{
    /// <summary>
    /// A basic HTTP server.
    /// </summary>
    public abstract partial class HttpServerBase : DisposableObjectBase, IHttpServer
    {
        #region Fields (2)

        /// <summary>
        /// The default port for HTTP requests.
        /// </summary>
        public const int DEFAULT_PORT_HTTP = 80;

        /// <summary>
        /// The default port for secure HTTP requests.
        /// </summary>
        public const int DEFAULT_PORT_SECURE_HTTP = 443;

        #endregion Fields

        #region Constructors (2)

        /// <inheriteddoc />
        protected HttpServerBase(object sync)
            : base(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected HttpServerBase()
            : this(sync: new object())
        {
        }

        #endregion Constructors

        #region Properties (13)

        /// <inheriteddoc />
        public virtual bool CanRestart
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public virtual bool CanStart
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public virtual bool CanStop
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public UsernamePasswordValidator CredentialValidator
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool IsRunning
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public int? Port
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public HttpPrincipalProvider PrincipalFinder
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public HttpRequestValidator RequestValidator
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public virtual bool SupportsSecureHttp
        {
            get { return false; }
        }

        /// <inheriteddoc />
        public HttpTransferMode TransferMode
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool UseSecureHttp
        {
            get;
            set;
        }

        #endregion Properties

        #region Delegates and Events (8)

        /// <inheriteddoc />
        public event EventHandler<HttpRequestEventArgs> HandleBadRequest;

        /// <inheriteddoc />
        public event EventHandler<HttpRequestEventArgs> HandleClientError;

        /// <inheriteddoc />
        public event EventHandler<HttpRequestEventArgs> HandleDocumentNotFound;

        /// <inheriteddoc />
        public event EventHandler<HttpRequestErrorEventArgs> HandleError;

        /// <inheriteddoc />
        public event EventHandler<HttpRequestEventArgs> HandleForbidden;

        /// <inheriteddoc />
        public event EventHandler<HttpRequestEventArgs> HandleNotImplemented;

        /// <inheriteddoc />
        public event EventHandler<HttpRequestEventArgs> HandleRequest;

        /// <inheriteddoc />
        public event EventHandler<HttpRequestEventArgs> HandleServerError;

        /// <inheriteddoc />
        public event EventHandler<HttpRequestEventArgs> HandleUnauthorized;

        #endregion Delegates and Events

        #region Methods (22)

        /// <summary>
        /// The logic that disposes that server.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void DisposeServer(DisposeContext context)
        {
            // dummy
        }

        /// <inheriteddoc />
        protected override sealed void OnDispose(DisposeContext context)
        {
            this.StopInner(StartStopContext.Dispose);

            this.DisposeServer(context);
        }

        private bool OnHandle(EventHandler<HttpRequestEventArgs> handler, IHttpRequest req, IHttpResponse resp)
        {
            if (req == null)
            {
                throw new ArgumentNullException("req");
            }

            if (resp == null)
            {
                throw new ArgumentNullException("resp");
            }

            var e = new HttpRequestEventArgs(req, resp);
            e.Handled = false;

            if (handler != null)
            {
                e.Handled = true;
                handler(this, e);
            }

            return e.Handled;
        }

        /// <summary>
        /// Raises the <see cref="HttpServerBase.HandleBadRequest" /> event.
        /// </summary>
        /// <param name="req">The request context.</param>
        /// <param name="resp">The response context.</param>
        /// <returns>Event handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" /> and/or <paramref name="resp" /> are <see langword="null" />.
        /// </exception>
        protected bool OnHandleBadRequest(IHttpRequest req, IHttpResponse resp)
        {
            return this.OnHandle(this.HandleBadRequest,
                                 req, resp);
        }

        /// <summary>
        /// Raises the <see cref="HttpServerBase.HandleClientError" /> event.
        /// </summary>
        /// <param name="req">The request context.</param>
        /// <param name="resp">The response context.</param>
        /// <returns>Event handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" /> and/or <paramref name="resp" /> are <see langword="null" />.
        /// </exception>
        protected bool OnHandleClientError(IHttpRequest req, IHttpResponse resp)
        {
            return this.OnHandle(this.HandleClientError,
                                 req, resp);
        }

        /// <summary>
        /// Raises the <see cref="HttpServerBase.HandleDocumentNotFound" /> event.
        /// </summary>
        /// <param name="req">The request context.</param>
        /// <param name="resp">The response context.</param>
        /// <returns>Event handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" /> and/or <paramref name="resp" /> are <see langword="null" />.
        /// </exception>
        protected bool OnHandleDocumentNotFound(IHttpRequest req, IHttpResponse resp)
        {
            return this.OnHandle(this.HandleDocumentNotFound,
                                 req, resp);
        }

        /// <summary>
        /// Raises the <see cref="HttpServerBase.HandleError" /> event.
        /// </summary>
        /// <param name="req">The request context.</param>
        /// <param name="resp">The response context.</param>
        /// <param name="ex">The thrown exception.</param>
        /// <returns>Event handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" />, <paramref name="resp" /> and/or <paramref name="ex" /> are <see langword="null" />.
        /// </exception>
        protected bool OnHandleError(IHttpRequest req, IHttpResponse resp, Exception ex)
        {
            if (req == null)
            {
                throw new ArgumentNullException("req");
            }

            if (resp == null)
            {
                throw new ArgumentNullException("resp");
            }

            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            var e = new HttpRequestErrorEventArgs(req, resp, ex);
            e.Handled = false;

            var handler = this.HandleError;
            if (handler != null)
            {
                e.Handled = true;
                handler(this, new HttpRequestErrorEventArgs(req, resp, ex));
            }

            return e.Handled;
        }

        /// <summary>
        /// Raises the <see cref="HttpServerBase.HandleForbidden" /> event.
        /// </summary>
        /// <param name="req">The request context.</param>
        /// <param name="resp">The response context.</param>
        /// <returns>Event handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" /> and/or <paramref name="resp" /> are <see langword="null" />.
        /// </exception>
        protected bool OnHandleForbidden(IHttpRequest req, IHttpResponse resp)
        {
            return this.OnHandle(this.HandleForbidden,
                                 req, resp);
        }

        /// <summary>
        /// Raises the <see cref="HttpServerBase.HandleNotImplemented" /> event.
        /// </summary>
        /// <param name="req">The request context.</param>
        /// <param name="resp">The response context.</param>
        /// <returns>Event handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" /> and/or <paramref name="resp" /> are <see langword="null" />.
        /// </exception>
        protected bool OnHandleNotImplemented(IHttpRequest req, IHttpResponse resp)
        {
            return this.OnHandle(this.HandleNotImplemented,
                                 req, resp);
        }

        /// <summary>
        /// Raises the <see cref="HttpServerBase.HandleRequest" /> event.
        /// </summary>
        /// <param name="req">The request context.</param>
        /// <param name="resp">The response context.</param>
        /// <returns>Event handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" /> and/or <paramref name="resp" /> are <see langword="null" />.
        /// </exception>
        protected bool OnHandleRequest(IHttpRequest req, IHttpResponse resp)
        {
            return this.OnHandle(this.HandleRequest,
                                 req, resp);
        }

        /// <summary>
        /// Raises the <see cref="HttpServerBase.HandleServerError" /> event.
        /// </summary>
        /// <param name="req">The request context.</param>
        /// <param name="resp">The response context.</param>
        /// <returns>Event handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" /> and/or <paramref name="resp" /> are <see langword="null" />.
        /// </exception>
        protected bool OnHandleServerError(IHttpRequest req, IHttpResponse resp)
        {
            return this.OnHandle(this.HandleServerError,
                                 req, resp);
        }

        /// <summary>
        /// Raises the <see cref="HttpServerBase.HandleUnauthorized" /> event.
        /// </summary>
        /// <param name="req">The request context.</param>
        /// <param name="resp">The response context.</param>
        /// <returns>Event handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="req" /> and/or <paramref name="resp" /> are <see langword="null" />.
        /// </exception>
        protected bool OnHandleUnauthorized(IHttpRequest req, IHttpResponse resp)
        {
            return this.OnHandle(this.HandleUnauthorized,
                                 req, resp);
        }

        /// <summary>
        /// The logic for <see cref="HttpServerBase.SetSslCertificateByThumbprint(string)" /> method.
        /// </summary>
        /// <param name="thumbprint">The thumbprint to search for.</param>
        protected virtual void OnSetSslCertificateByThumbprint(string thumbprint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The logic for <see cref="HttpServerBase.Start()" /> and
        /// the <see cref="HttpServerBase.Restart()" /> method.
        /// </summary>
        /// <param name="context">The invokation context.</param>
        /// <param name="isRunning">
        /// The new value for <see cref="HttpServerBase.IsRunning" /> property.
        /// Is <see langword="true" /> by default.
        /// </param>
        protected abstract void OnStart(StartStopContext context,
                                        ref bool isRunning);

        /// <summary>
        /// The logic for <see cref="HttpServerBase.Stop()" /> and
        /// the <see cref="HttpServerBase.Restart()" /> method.
        /// </summary>
        /// <param name="context">The invokation context.</param>
        /// <param name="isRunning">
        /// The new value for <see cref="HttpServerBase.IsRunning" /> property.
        /// Is <see langword="false" /> by default.
        /// </param>
        protected abstract void OnStop(StartStopContext context,
                                       ref bool isRunning);

        /// <inheriteddoc />
        public void Restart()
        {
            lock (this._SYNC)
            {
                if (!this.CanRestart)
                {
                    throw new InvalidOperationException();
                }

                this.StopInner(StartStopContext.Restart);
                this.StartInner(StartStopContext.Restart);
            }
        }

        /// <inheriteddoc />
        public HttpServerBase SetSslCertificateByThumbprint(string thumbprint)
        {
            lock (this._SYNC)
            {
                if (this.SupportsSecureHttp == false)
                {
                    // does not support HTTPs
                    throw new NotSupportedException();
                }

                this.OnSetSslCertificateByThumbprint(thumbprint);
                return this;
            }
        }

        IHttpServer IHttpServer.SetSslCertificateByThumbprint(string thumbprint)
        {
            return this.SetSslCertificateByThumbprint(thumbprint: thumbprint);
        }

        /// <inheriteddoc />
        public void Start()
        {
            lock (this._SYNC)
            {
                if (this.CanStart == false)
                {
                    throw new InvalidOperationException();
                }

                if ((this.SupportsSecureHttp == false) &&
                    this.UseSecureHttp)
                {
                    // does not support HTTPs
                    throw new NotSupportedException();
                }

                this.StartInner(StartStopContext.Start);
            }
        }

        private void StartInner(StartStopContext context)
        {
            if (this.IsRunning)
            {
                return;
            }

            var isRunning = true;
            try
            {
                this.OnStart(context, ref isRunning);
            }
            finally
            {
                this.IsRunning = isRunning;
            }
        }

        /// <inheriteddoc />
        public void Stop()
        {
            lock (this._SYNC)
            {
                if (this.CanStop == false)
                {
                    throw new InvalidOperationException();
                }

                this.StopInner(StartStopContext.Stop);
            }
        }

        private void StopInner(StartStopContext context)
        {
            if (this.IsRunning == false)
            {
                return;
            }

            var isRunning = false;
            try
            {
                this.OnStop(context,
                            ref isRunning);
            }
            finally
            {
                this.IsRunning = isRunning;
            }
        }

        #endregion Methods
    }
}