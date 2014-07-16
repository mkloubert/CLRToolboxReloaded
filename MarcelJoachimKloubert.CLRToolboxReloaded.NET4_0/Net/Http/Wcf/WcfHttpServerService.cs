// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;
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
            using (var uncompressedResponse = new MemoryStream())
            {
                using (var requestStream = new MemoryStream())
                {
                    this._WEB_ENCODER.WriteMessage(message, requestStream);
                    requestStream.Position = 0;

                    using (var request = new WcfHttpRequest(msg: message,
                                                            stream: requestStream,
                                                            user: this.TryFindUser()))
                    {
                        using (var response = new WcfHttpResponse(property: new HttpResponseMessageProperty(),
                                                                  outputStream: uncompressedResponse))
                        {
                            using (var responseStream = new MemoryStream())
                            {
                                uncompressedResponse.Position = 0;

                                var responseMessage = new BinaryMessage(responseStream.ToArray());
                                responseMessage.Properties[HttpResponseMessageProperty.Name] = response.Property;

                                return responseMessage;
                            }
                        }
                    }
                }
            }
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