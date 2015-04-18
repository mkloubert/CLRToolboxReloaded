// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    partial class WcfHttpServer
    {
        #region Nested classes (1)

        internal sealed class HttpRequest : HttpRequestBase, IDisposable
        {
            #region Fields (11)

            private readonly Uri _ADDRESS;
            private readonly IReadOnlyDictionary<string, string> _GET;
            private readonly IReadOnlyDictionary<string, string> _HEADERS;
            private readonly string _METHOD;
            private readonly IReadOnlyDictionary<string, string> _POST;
            private readonly ITcpAddress _REMOTE_ADDR;
            private readonly IReadOnlyDictionary<string, string> _REQUEST;
            private readonly WcfHttpServer _SERVER;
            private readonly Stream _STREAM;
            private readonly DateTimeOffset _TIME;
            private readonly IPrincipal _USER;

            #endregion Fields (11)

            #region Constructors (2)

            internal HttpRequest(Message msg,
                                 MessageEncoder enc,
                                 WcfHttpServer srv,
                                 IPrincipal user)
            {
                this._TIME = GlobalServices.Now;

                this.Property = (HttpRequestMessageProperty)msg.Properties[HttpRequestMessageProperty.Name];
                this._SERVER = srv;
                this._STREAM = this._SERVER.CreateRequestStream();
                this._USER = user;

                // write body to stream
                enc.WriteMessage(msg, this._STREAM);
                if (this._STREAM.CanSeek)
                {
                    this._STREAM.Position = 0;
                }

                // remote IP
                try
                {
                    var messageProperties = OperationContext.Current.IncomingMessageProperties;
                    if (messageProperties != null)
                    {
                        var endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                        if (endpointProperty != null)
                        {
                            this._REMOTE_ADDR = new TcpAddress()
                                {
                                    Address = endpointProperty.Address,
                                    Port = endpointProperty.Port,
                                };
                        }
                    }
                }
                catch
                {
                    // ignore errors here
                }

                this._ADDRESS = msg.Headers.To;
                this._METHOD = (this.Property.Method ?? string.Empty).ToUpper().Trim();

                // headers
                {
                    var headers = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateHttpKeyComparer());

                    this.Property.Headers.AllKeys.ForAll(
                        action: (ctx) =>
                            {
                                var key = ctx.Item;

                                ctx.State.Headers[key ?? string.Empty] = ctx.State.Property.Headers[key];
                            },
                        actionState: new
                            {
                                Headers = headers,
                                Property = this.Property,
                            },
                        throwExceptions: false);

                    this._HEADERS = new ReadOnlyDictionaryWrapper<string, string>(dict: headers);
                }

                // GET
                {
                    var getVars = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateHttpKeyComparer());

                    var queryStr = this.Property.QueryString;
                    if (string.IsNullOrWhiteSpace(queryStr) == false)
                    {
                        var coll = HttpUtility.ParseQueryString(queryStr);
                        coll.AllKeys.ForAll(
                            action: (ctx) =>
                                {
                                    var key = ctx.Item;

                                    ctx.State.Vars[key ?? string.Empty] = ctx.State.Collection[key];
                                },
                            actionState: new
                                {
                                    Collection = coll,
                                    Vars = getVars,
                                },
                            throwExceptions: false);
                    }

                    this._GET = new ReadOnlyDictionaryWrapper<string, string>(dict: getVars);
                }

                // POST
                var postVars = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateHttpKeyComparer());
                try
                {
                    if (this._METHOD == "POST")
                    {
                        using (var temp = new MemoryStream())
                        {
                            this._STREAM.CopyTo(temp);

                            temp.Position = 0;
                            using (var reader = new StreamReader(temp, Encoding.ASCII))
                            {
                                var extractedVars = HttpUtility.ParseQueryString(reader.ReadToEnd());

                                extractedVars.AllKeys.ForAll(
                                    throwExceptions: false,
                                    action: ctx =>
                                        {
                                            var key = ctx.Item;

                                            SetVar(vars: ctx.State.Vars,
                                                   key: key,
                                                   value: ctx.State.POST[key]);
                                        },
                                    actionState: new
                                        {
                                            POST = extractedVars,
                                            Vars = postVars,
                                        });
                            }
                        }
                    }
                }
                finally
                {
                    this._POST = new ReadOnlyDictionaryWrapper<string, string>(dict: postVars);
                }

                // REQUEST
                this._REQUEST = this.CreateRequestVarsDictionary();
            }

            ~HttpRequest()
            {
                this.Dispose(false);
            }

            #endregion Constructors (2)

            #region Properties (12)

            public override Uri Address
            {
                get { return this._ADDRESS; }
            }

            public override string ContentType
            {
                get
                {
                    string result;
                    this._HEADERS.TryGetValue("Content-type", out result);

                    return string.IsNullOrWhiteSpace(result) ? null : result.ToLower().Trim();
                }
            }

            public override IReadOnlyDictionary<string, string> GET
            {
                get { return this._GET; }
            }

            public override IReadOnlyDictionary<string, string> Headers
            {
                get { return this._HEADERS; }
            }

            public override string Method
            {
                get { return this._METHOD; }
            }

            public override IReadOnlyDictionary<string, string> POST
            {
                get { return this._POST; }
            }

            internal HttpRequestMessageProperty Property
            {
                get;
                private set;
            }

            public override ITcpAddress RemoteAddress
            {
                get { return this._REMOTE_ADDR; }
            }

            public override IReadOnlyDictionary<string, string> REQUEST
            {
                get { return this._REQUEST; }
            }

            internal Stream Stream
            {
                get { return this._STREAM; }
            }

            public override DateTimeOffset Time
            {
                get { return this._TIME; }
            }

            public override IPrincipal User
            {
                get { return this._USER; }
            }

            #endregion Properties (12)

            #region Methods (3)

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                try
                {
                    this._SERVER
                        .CloseRequestStream(this._STREAM);
                }
                catch
                {
                    if (disposing)
                    {
                        throw;
                    }
                }
            }

            protected override Stream OnGetBody()
            {
                return new NonDisposableStream(baseStream: this._STREAM,
                                               callBehaviour: NonDisposableStream.CallBehaviour.Nothing);
            }

            #endregion Methods (3)
        }

        #endregion Nested classes (1)
    }
}