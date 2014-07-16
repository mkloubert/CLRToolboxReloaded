// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfHttpRequest = MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf.WcfHttpServer.HttpRequest;
using WcfHttpResponse = MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf.WcfHttpServer.HttpResponse;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Prefix,
                     ConcurrencyMode = ConcurrencyMode.Multiple,
                     InstanceContextMode = InstanceContextMode.Single)]
    internal sealed partial class WcfHttpServerService : IWcfHttpServerService
    {
        #region Fields (2)

        private readonly WcfHttpServer _SERVER;
        private readonly MessageEncoder _WEB_ENCODER = CreateWebMessageBindingEncoder().CreateMessageEncoderFactory().Encoder;

        #endregion Fields (2)

        #region Constructors (1)

        internal WcfHttpServerService(WcfHttpServer server)
        {
            this._SERVER = server;
        }

        #endregion Constructors (1)

        #region Methods (3)

        internal static WebMessageEncodingBindingElement CreateWebMessageBindingEncoder()
        {
            var encoding = new WebMessageEncodingBindingElement();
            encoding.MaxReadPoolSize = int.MaxValue;
            encoding.ContentTypeMapper = new RawContentTypeMapper();
            encoding.ReaderQuotas.MaxArrayLength = int.MaxValue;

            return encoding;
        }

        public Message Request(Message message)
        {
            BinaryMessage result;

            using (var req = new WcfHttpRequest(msg: message,
                                                srv: this._SERVER,
                                                user: this.TryFindUser()))
            using (var resp = new WcfHttpResponse(property: new HttpResponseMessageProperty(),
                                                  srv: this._SERVER))
            {
                using (var outputStream = new MemoryStream())
                {
                    try
                    {
                        resp.StatusCode = HttpStatusCode.OK;
                        resp.StatusDescription = null;
                        resp.Compress = false;

                        var isRequestValid = true;

                        var reqValidator = this._SERVER.RequestValidator;
                        if (reqValidator != null)
                        {
                            isRequestValid = reqValidator(req);
                        }

                        if (isRequestValid)
                        {
                            if (this._SERVER.OnHandleRequestInner(req, resp) == false)
                            {
                                // 501 - NotImplemented

                                resp.Compress = false;
                                resp.StatusCode = HttpStatusCode.NotImplemented;
                                resp.StatusDescription = null;

                                this._SERVER.OnHandleNotImplementedInner(req, resp);
                            }
                        }
                        else
                        {
                            // 400 - BadRequest

                            resp.Compress = false;
                            resp.StatusCode = HttpStatusCode.BadRequest;
                            resp.StatusDescription = null;

                            this._SERVER.OnHandleBadRequestInner(req, resp);
                        }

                        if (resp.IsForbidden == false)
                        {
                            if (resp.DocumentNotFound)
                            {
                                // 404 - NotFound

                                resp.Compress = false;
                                resp.StatusCode = HttpStatusCode.NotFound;
                                resp.StatusDescription = null;

                                this._SERVER.OnHandleDocumentNotFoundInner(req, resp);
                            }
                        }
                        else
                        {
                            // 403 - Forbidden

                            resp.Compress = false;
                            resp.StatusCode = HttpStatusCode.Forbidden;
                            resp.StatusDescription = null;

                            this._SERVER.OnHandleForbiddenInner(req, resp);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 500 - InternalServerError

                        resp.Compress = false;
                        resp.StatusCode = HttpStatusCode.InternalServerError;
                        resp.StatusDescription = (ex.GetBaseException() ?? ex).Message;

                        this._SERVER.OnHandleErrorInner(req, resp, ex);
                    }
                    finally
                    {
                        if (resp.Stream.CanSeek)
                        {
                            resp.Stream.Position = 0;
                        }

                        resp.Stream.CopyTo(outputStream);
                        outputStream.Position = 0;

                        var responseMessage = new BinaryMessage(outputStream.ToArray());
                        responseMessage.Properties[HttpResponseMessageProperty.Name] = resp.Property;

                        result = responseMessage;
                    }
                }
            }

            return result;
        }

        private IPrincipal TryFindUser()
        {
            IPrincipal result = null;

            try
            {
                var finder = this._SERVER.PrincipalFinder;
                if (finder != null)
                {
                    var secCtx = ServiceSecurityContext.Current;
                    if (secCtx != null)
                    {
                        var id = secCtx.PrimaryIdentity;
                        if (id != null)
                        {
                            result = finder(id);
                        }
                    }
                }
            }
            catch
            {
                // ignore errors here
            }

            return result;
        }

        #endregion Methods (3)
    }
}