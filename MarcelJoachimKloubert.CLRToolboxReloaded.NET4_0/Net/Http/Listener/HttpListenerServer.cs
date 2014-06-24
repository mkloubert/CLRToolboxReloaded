// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(NET40)
#define KNOWS_ASYNC_PATTERN
#endif

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Listener
{
    /// <summary>
    /// A server based on a <see cref="HttpListener" /> instance.
    /// </summary>
    public partial class HttpListenerServer : HttpServerBase
    {
        #region Fields (1)

        private HttpListener _listener;

        #endregion Fields (1)

        #region Constructors (2)

        /// <inheriteddoc />
        public HttpListenerServer(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public HttpListenerServer()
            : this(sync: new object())
        {
        }

        #endregion Constructors

        #region Properties (3)

        /// <summary>
        /// Gets the name of the realm for basic auth.
        /// </summary>
        public virtual string RealmName
        {
            get { return "CLR Toolbox Reloaded"; }
        }

        /// <summary>
        /// Gets or sets the store location of the SSL certificate.
        /// <see langword="null" /> indicates to use the default one.
        /// </summary>
        public StoreLocation? SslStoreLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the store name of the SSL certificate.
        /// <see langword="null" /> indicates to use the default one.
        /// </summary>
        public StoreName? SslStoreName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the thumbprint of the SSL certificate.
        /// <see langword="null" /> indicates to use the default one.
        /// </summary>
        public string Thumbprint
        {
            get;
            set;
        }

        #endregion

        #region Methods (13)

        /// <summary>
        /// Is used to add prefix entries to a <see cref="HttpListener" /> based on the current data of that object.
        /// </summary>
        /// <param name="listener">The listener where the prefixes should be added to.</param>
        /// <param name="useHttps">User secure HTTP (https) or not.</param>
        protected virtual void AddPrefixes(HttpListener listener, bool useHttps)
        {
            listener.Prefixes.Add(string.Format("http{0}://+:{1}/",
                                                useHttps ? "s" : string.Empty,
                                                this.Port));

            listener.Prefixes.Add(string.Format("http{0}://*:{1}/",
                                                useHttps ? "s" : string.Empty,
                                                this.Port));
        }

        private void CreateRequestAndResponse(HttpListenerContext ctx, out HttpRequest req, out HttpResponse resp)
        {
            // find user
            IPrincipal user = null;
            {
                var finder = this.PrincipalFinder;
                if (finder != null)
                {
                    string username = null;
                    string password = null;
                    try
                    {
                        ExtractUsernameAndPassword(req: ctx.Request,
                                                   username: out username, password: out password);

                        var id = new DummyIdentity()
                            {
                                AuthenticationType = "HttpBasicAuth",
                                Name = username,
                            };

                        user = finder(id);
                    }
                    finally
                    {
                        username = null;
                        password = null;
                    }
                }
            }

            req = new HttpRequest(ctx,
                                  user: user);
            resp = new HttpResponse(ctx: ctx,
                                    stream: this.CreateResponseStream(ctx));
        }

        /// <summary>
        /// Creates an initial stream for a response context.
        /// </summary>
        /// <param name="ctx">The underlying HTTP context.</param>
        /// <returns>The initial stream.</returns>
        protected virtual Stream CreateResponseStream(HttpListenerContext ctx)
        {
            return new MemoryStream();
        }

        private static void DisposeListener(HttpListener listener)
        {
            try
            {
                using (var l = listener)
                {
                    listener = null;
                }
            }
            catch
            {
                // ignore here
            }
        }

        private void DisposeOldListener()
        {
            DisposeListener(this._listener);
        }

        private static void ExtractUsernameAndPassword(HttpListenerRequest req,
                                                       out string username, out string password)
        {
            username = null;
            password = null;

            try
            {
                var data = (req.Headers["Authorization"] ?? string.Empty).Trim();
                if (data != string.Empty)
                {
                    if (data.ToLower().StartsWith("basic "))
                    {
                        var base64EncodedData = data.Substring(data.IndexOf(" ")).Trim();
                        var blobData = Convert.FromBase64String(base64EncodedData);

                        var strData = new UTF8Encoding().GetString(blobData);

                        var semicolon = strData.IndexOf(":");
                        if (semicolon > -1)
                        {
                            username = strData.Substring(0, semicolon).ToLower().Trim();
                            if (username == string.Empty)
                            {
                                username = null;
                            }

                            password = strData.Substring(semicolon + 1);
                        }
                        else
                        {
                            username = strData.Trim();
                        }

                        if (string.IsNullOrWhiteSpace(username))
                        {
                            username = null;
                        }

                        if (string.IsNullOrEmpty(password))
                        {
                            password = null;
                        }
                    }
                }
            }
            catch
            {
                username = null;
                password = null;
            }
        }

        private void HandleContext(HttpListenerContext ctx)
        {
            HttpRequest req;
            HttpResponse resp;
            this.CreateRequestAndResponse(ctx: ctx,
                                          req: out req, resp: out resp);

            try
            {
                resp.StatusCode = HttpStatusCode.OK;
                resp.StatusDescription = null;

                var isRequestValid = true;

                var reqValidator = this.RequestValidator;
                if (reqValidator != null)
                {
                    isRequestValid = reqValidator(req);
                }

                if (isRequestValid)
                {
                    var isAuthroized = true;

                    var credValidator = this.CredentialValidator;
                    if (credValidator != null)
                    {
                        string username = null;
                        string password = null;
                        try
                        {
                            ExtractUsernameAndPassword(req: ctx.Request,
                                                       username: out username, password: out password);

                            isAuthroized = credValidator(username, password);
                        }
                        finally
                        {
                            username = null;
                            password = null;
                        }
                    }

                    if (isAuthroized)
                    {
                        if (this.OnHandleRequest(req, resp) == false)
                        {
                            // 501 - NotImplemented

                            resp.StatusCode = HttpStatusCode.NotImplemented;
                            resp.StatusDescription = null;

                            this.OnHandleNotImplemented(req, resp);
                        }
                    }
                    else
                    {
                        // 401 - Unauthorized

                        resp.StatusCode = HttpStatusCode.Unauthorized;
                        resp.StatusDescription = null;

                        if (credValidator != null)
                        {
                            /*
                            ctx.Response.Headers[HttpResponseHeader.WwwAuthenticate] =
                                string.Format("Basic realm=\"{0}\"",
                                              this.RealmName);*/
                        }

                        this.OnHandleUnauthorized(req, resp);
                    }

                    if (resp.IsForbidden == false)
                    {
                        if (resp.DocumentNotFound)
                        {
                            // 404 - NotFound

                            resp.StatusCode = HttpStatusCode.NotFound;
                            resp.StatusDescription = null;

                            this.OnHandleDocumentNotFound(req, resp);
                        }
                    }
                    else
                    {
                        // 403 - Forbidden

                        resp.StatusCode = HttpStatusCode.Forbidden;
                        resp.StatusDescription = null;

                        this.OnHandleForbidden(req, resp);
                    }
                }
                else
                {
                    // 400 - BadRequest

                    resp.StatusCode = HttpStatusCode.BadRequest;
                    resp.StatusDescription = null;

                    this.OnHandleBadRequest(req, resp);
                }
            }
            catch (Exception ex)
            {
                // 500 - InternalServerError

                resp.StatusCode = HttpStatusCode.InternalServerError;
                resp.StatusDescription = (ex.GetBaseException() ?? ex).Message;

                this.OnHandleError(req, resp, ex);
            }
            finally
            {
                try
                {
                    SendResponse(ctx: ctx,
                                 req: req, resp: resp);
                }
                catch
                {
                    // ignore here
                }
            }
        }

        private void HttpListener_BeginGetContext(IAsyncResult ar)
        {
            HttpListener listener = null;

            try
            {
                listener = (HttpListener)ar.AsyncState;

                HandleContext(ctx: listener.EndGetContext(ar));
            }
            catch (Exception ex)
            {
                this.OnErrorsReceived(ex);
            }
            finally
            {
                if (listener != null)
                {
                    lock (this._SYNC)
                    {
                        if (this.IsRunning)
                        {
                            this.StartListening(listener: listener,
                                                throwException: false);
                        }
                    }
                }
            }
        }

        /// <inheriteddoc />
        protected override void OnSetSslCertificateByThumbprint(string thumbprint)
        {
            if (string.IsNullOrWhiteSpace(thumbprint))
            {
                thumbprint = null;
            }

            this.Thumbprint = thumbprint != null ? thumbprint.Trim() : null;
        }

        /// <inheriteddoc />
        protected override void OnStart(HttpServerBase.StartStopContext context,
                                        ref bool isRunning)
        {
            this.DisposeOldListener();

            HttpListener newListener = null;
            try
            {
                newListener = new HttpListener();

                var useHttps = this.UseSecureHttp;
                var storeLoc = this.SslStoreLocation ?? StoreLocation.CurrentUser;
                var storeName = this.SslStoreName ?? StoreName.My;
                var thumb = this.Thumbprint;

                this.AddPrefixes(listener: newListener,
                                 useHttps: useHttps);

                if (useHttps)
                {
                    X509Certificate2 cert = null;

                    if (string.IsNullOrWhiteSpace(thumb) == false)
                    {
                        var store = new X509Store(storeName, storeLoc);
                        try
                        {
                            store.Open(OpenFlags.ReadOnly);

                            var foundCerts = store.Certificates.Find(findType: X509FindType.FindByThumbprint,
                                                                     findValue: thumb,
                                                                     validOnly: false);

                            cert = foundCerts.Cast<X509Certificate2>()
                                             .SingleOrDefault();
                        }
                        finally
                        {
                            store.Close();
                        }
                    }

                    if (cert != null)
                    {
                        //TODO
                    }
                }

                newListener.Start();
                this.StartListening(newListener, true);

                this._listener = newListener;
            }
            catch
            {
                this.DisposeOldListener();

                throw;
            }
        }

        /// <inheriteddoc />
        protected override void OnStop(HttpServerBase.StartStopContext context,
                                       ref bool isRunning)
        {
            this.DisposeOldListener();
        }

#if (KNOWS_ASYNC_PATTERN)
        private async void StartListening(HttpListener listener, bool throwException)
#else
        private void StartListening(HttpListener listener, bool throwException)
#endif
        {
            if (listener == null)
            {
                return;
            }

#if (KNOWS_ASYNC_PATTERN)

            while (this.IsRunning)
            {
                try
                {
                    var ctx = await listener.GetContextAsync();

                    HandleContext(ctx: ctx);
                }
                catch (global::System.Exception ex)
                {
                    this.OnErrorsReceived(ex);

                    DisposeListener(listener: listener);
                }
            }

#else

            try
            {
                listener.BeginGetContext(callback: this.HttpListener_BeginGetContext,
                                         state: listener);
            }
            catch
            {
                if (throwException)
                {
                    throw;
                }
            }

#endif
        }

        private static void SendResponse(HttpListenerContext ctx, HttpRequest req, HttpResponse resp)
        {
            var response = ctx.Response;
            try
            {
                // headers
                foreach (var item in resp.Headers)
                {
                    response.Headers[item.Key] = item.Value;
                }

                var outputStream = resp.Stream;
                if (outputStream != null)
                {
                    var responseStream = response.OutputStream;
                    if (object.ReferenceEquals(outputStream, responseStream) == false)
                    {
                        if (outputStream.CanSeek)
                        {
                            outputStream.Position = 0;
                        }

                        outputStream.CopyTo(responseStream);
                    }
                }
            }
            finally
            {
                response.Close();
            }
        }

        #endregion
    }
}