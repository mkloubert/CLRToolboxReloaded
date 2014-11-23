// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

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

        #region Properties (4)

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

        /// <inheriteddoc />
        public override bool SupportsSecureHttp
        {
            get { return true; }
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

        #region Methods (16)

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
            var user = ctx.User;
            if (user != null)
            {
                var finder = this.PrincipalFinder;
                if (finder != null)
                {
                    user = finder(user.Identity);
                }
            }

            req = new HttpRequest(server: this,
                                  ctx: ctx,
                                  user: user);

            resp = new HttpResponse(server: this,
                                    ctx: ctx);
        }

        /// <summary>
        /// Closes a request stream.
        /// </summary>
        /// <param name="ctx">The underlying HTTP context.</param>
        /// <param name="stream">The stream to close.</param>
        protected virtual void CloseRequestStream(HttpListenerContext ctx, Stream stream)
        {
            stream.Dispose();
        }

        /// <summary>
        /// Closes a response stream.
        /// </summary>
        /// <param name="ctx">The underlying HTTP context.</param>
        /// <param name="stream">The stream to close.</param>
        protected virtual void CloseResponseStream(HttpListenerContext ctx, Stream stream)
        {
            stream.Dispose();
        }

        private Task CreateHandleContextTask(HttpListenerContext ctx)
        {
            return new Task(action: this.HandleContext,
                            state: ctx);
        }

        /// <summary>
        /// Creates an stream for the body of a request context.
        /// </summary>
        /// <param name="ctx">The underlying HTTP context.</param>
        /// <returns>The initial stream.</returns>
        protected virtual Stream CreateRequestStream(HttpListenerContext ctx)
        {
            return new MemoryStream();
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

        private static void DisposeListener(ref HttpListener listener)
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
            DisposeListener(ref this._listener);
        }

        private void HandleContext(object state)
        {
            var ctx = (HttpListenerContext)state;

            HttpRequest req;
            HttpResponse resp;
            this.CreateRequestAndResponse(ctx: ctx,
                                          req: out req, resp: out resp);

            using (req)
            using (resp)
            {
                try
                {
                    resp.StatusCode = HttpStatusCode.OK;
                    resp.StatusDescription = null;
                    resp.Compress = false;

                    var isRequestValid = true;

                    var reqValidator = this.RequestValidator;
                    if (reqValidator != null)
                    {
                        isRequestValid = reqValidator(req);
                    }

                    if (isRequestValid)
                    {
                        var isAuthorized = true;

                        var credValidator = this.CredentialValidator;
                        if (credValidator != null)
                        {
                            var id = (HttpListenerBasicIdentity)ctx.User.Identity;

                            isAuthorized = credValidator(id.Name,
                                                         id.Password);
                        }

                        if (isAuthorized)
                        {
                            if (this.OnHandleRequest(req, resp) == false)
                            {
                                // 501 - NotImplemented

                                resp.Compress = false;
                                resp.StatusCode = HttpStatusCode.NotImplemented;
                                resp.StatusDescription = null;

                                this.OnHandleNotImplemented(req, resp);
                            }
                        }
                        else
                        {
                            // 401 - Unauthorized

                            resp.Compress = false;
                            resp.StatusCode = HttpStatusCode.Unauthorized;
                            resp.StatusDescription = null;

                            this.OnHandleUnauthorized(req, resp);
                        }

                        if (resp.IsForbidden == false)
                        {
                            if (resp.DocumentNotFound)
                            {
                                // 404 - NotFound

                                resp.Compress = false;
                                resp.StatusCode = HttpStatusCode.NotFound;
                                resp.StatusDescription = null;

                                this.OnHandleDocumentNotFound(req, resp);
                            }
                        }
                        else
                        {
                            // 403 - Forbidden

                            resp.Compress = false;
                            resp.StatusCode = HttpStatusCode.Forbidden;
                            resp.StatusDescription = null;

                            this.OnHandleForbidden(req, resp);
                        }
                    }
                    else
                    {
                        // 400 - BadRequest

                        resp.Compress = false;
                        resp.StatusCode = HttpStatusCode.BadRequest;
                        resp.StatusDescription = null;

                        this.OnHandleBadRequest(req, resp);
                    }
                }
                catch (Exception ex)
                {
                    // 500 - InternalServerError

                    resp.Compress = false;
                    resp.StatusCode = HttpStatusCode.InternalServerError;
                    resp.StatusDescription = (ex.GetBaseException() ?? ex).Message;

                    this.OnHandleError(req, resp, ex);
                }
                finally
                {
                    try
                    {
                        if (((int)resp.StatusCode >= 400) && ((int)resp.StatusCode < 500))
                        {
                            // client error

                            this.OnHandleClientError(req, resp);
                        }
                        else if (((int)resp.StatusCode >= 500) && ((int)resp.StatusCode < 600))
                        {
                            // server error

                            this.OnHandleServerError(req, resp);
                        }

                        var compress = resp.Compress ?? false;
                        if (compress)
                        {
                            var closeOldStream = false;

                            var oldStream = resp.Stream;
                            var newStream = this.CreateResponseStream(ctx);
                            try
                            {
                                resp.SetStream(newStream);
                                closeOldStream = true;
                                    
                                oldStream.Position = 0;
                                oldStream.GZip(newStream);

                                ctx.Response.Headers[HttpResponseHeader.ContentEncoding] = "gzip";
                            }
                            catch
                            {
                                this.CloseResponseStream(ctx, newStream);

                                throw;
                            }
                            finally
                            {
                                if (closeOldStream)
                                {
                                    this.CloseResponseStream(ctx, oldStream);
                                }
                            }
                        }

                        SendResponse(ctx: ctx,
                                     req: req, resp: resp);
                    }
                    catch (Exception ex)
                    {
                        this.OnErrorsReceived(ex);
                    }
                }
            }
        }

        private void HttpListener_BeginGetContext(IAsyncResult ar)
        {
            HttpListener listener = null;
            var tryStartListening = true;

            try
            {
                listener = (HttpListener)ar.AsyncState;

                var ctx = listener.EndGetContext(ar);
                tryStartListening = false;

                // listen for next request
                // before handle current context
                this.StartListening(listener: listener,
                                    throwException: false);

                this.HandleContext(ctx);
            }
            catch (Exception ex)
            {
                this.OnErrorsReceived(ex);
            }
            finally
            {
                if (tryStartListening)
                {
                    this.StartListening(listener: listener,
                                        throwException: false);
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
                var useHttps = this.UseSecureHttp;

                newListener = new HttpListener();

                if (this.CredentialValidator != null)
                {
                    newListener.AuthenticationSchemes = AuthenticationSchemes.Basic;
                    newListener.Realm = this.RealmName;
                }

                this.AddPrefixes(listener: newListener,
                                 useHttps: useHttps);

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

        private void StartListening(HttpListener listener, bool throwException)
        {
            if (listener == null)
            {
                return;
            }

            try
            {
                if (listener.IsListening)
                {
                    listener.BeginGetContext(callback: this.HttpListener_BeginGetContext,
                                             state: listener);
                }
            }
            catch (Exception ex)
            {
                if (throwException)
                {
                    throw;
                }
                else
                {
                    this.OnErrorsReceived(ex);
                }
            }
        }

        private static void SendResponse(HttpListenerContext ctx, HttpRequest req, HttpResponse resp)
        {
            var response = ctx.Response;
            try
            {
                // headers
                resp.Headers.ForAll(faCtx =>
                    {
                        faCtx.State
                             .Response.Headers[faCtx.Item.Key] = faCtx.Item.Value;
                    }, new
                    {
                        Response = resp,
                    });

                if (string.IsNullOrWhiteSpace(resp.ContentType) == false)
                {
                    ctx.Response.ContentType = resp.ContentType;
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