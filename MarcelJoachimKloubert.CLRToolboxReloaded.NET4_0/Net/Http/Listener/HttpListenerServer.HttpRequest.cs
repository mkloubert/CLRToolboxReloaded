﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Listener
{
    partial class HttpListenerServer
    {
        #region Nested classes (1)

        private sealed class HttpRequest : HttpRequestBase
        {
            #region Fields (9)

            private readonly HttpListenerContext _CONTEXT;
            private readonly IReadOnlyDictionary<string, string> _GET_VARS;
            private readonly IReadOnlyDictionary<string, string> _HEADERS;
            private readonly string _METHOD;
            private readonly IReadOnlyDictionary<string, string> _REQUEST_VARS;
            private readonly IReadOnlyDictionary<string, string> _POST_VARS;
            private readonly ITcpAddress _REMOTE_ADDR;
            private readonly DateTimeOffset _TIME;
            private readonly IPrincipal _USER;

            #endregion Fields (8)

            #region Constrcutors (1)

            internal HttpRequest(HttpListenerContext ctx,
                                 IPrincipal user)
            {
                this._TIME = AppTime.Now;

                this._CONTEXT = ctx;
                this._USER = user;

                // remote address
                var remoteIP = ctx.Request.RemoteEndPoint;
                if (remoteIP != null)
                {
                    this._REMOTE_ADDR = new TcpAddress()
                        {
                            Address = remoteIP.Address.AsString(),
                            Port = remoteIP.Port,
                        };
                }

                this._METHOD = (ctx.Request.HttpMethod ?? string.Empty).ToUpper().Trim();
                if (this._METHOD == string.Empty)
                {
                    this._METHOD = null;
                }

                // GET
                var getVars = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                   emptyIsNull: true));
                try
                {
                    ctx.Request.QueryString.Keys.Cast<string>().ForAll(
                        throwExceptions: false,
                        action: faCtx =>
                            {
                                var key = faCtx.Item;

                                SetVar(vars: faCtx.State.Vars,
                                       key: key,
                                       value: faCtx.State.Request.QueryString[key]);
                            },
                        actionState: new
                            {
                                Request = ctx.Request,
                                Vars = getVars,
                            });
                }
                finally
                {
                    this._GET_VARS = new ReadOnlyDictionaryWrapper<string, string>(dict: getVars);
                }

                // POST
                var postVars = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                    emptyIsNull: true));
                try
                {
                    if (this._METHOD == "POST")
                    {
                        using (var temp = new MemoryStream())
                        {
                            ctx.Request.InputStream.CopyTo(temp);

                            temp.Position = 0;
                            using (var reader = new StreamReader(temp, Encoding.ASCII))
                            {
                                var extractedVars = HttpUtility.ParseQueryString(reader.ReadToEnd());

                                extractedVars.AllKeys.Cast<string>().ForAll(
                                    throwExceptions: false,
                                    action: faCtx =>
                                        {
                                            var key = faCtx.Item;

                                            SetVar(vars: faCtx.State.Vars,
                                                   key: key,
                                                   value: faCtx.State.POST[key]);
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
                    this._POST_VARS = new ReadOnlyDictionaryWrapper<string, string>(dict: getVars);
                }

                // REQUEST
                var requestVars = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                       emptyIsNull: true));
                try
                {
                    // then GET
                    getVars.ForAll(
                        throwExceptions: false,
                        action: faCtx =>
                            {
                                var key = faCtx.Item.Key;

                                SetVar(vars: faCtx.State.Vars,
                                       key: key,
                                       value: faCtx.Item.Value);
                            },
                        actionState: new
                            {
                                Vars = requestVars,
                            });

                    // then POST
                    postVars.ForAll(
                        throwExceptions: false,
                        action: faCtx =>
                            {
                                var key = faCtx.Item.Key;

                                SetVar(vars: faCtx.State.Vars,
                                       key: key,
                                       value: faCtx.Item.Value);
                            },
                        actionState: new
                            {
                                Vars = requestVars,
                            });
                }
                finally
                {
                    this._REQUEST_VARS = new ReadOnlyDictionaryWrapper<string, string>(dict: getVars);
                }

                // headers
                var headers = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                   emptyIsNull: true));
                try
                {
                    ctx.Request.Headers.AllKeys.ForAll(
                        throwExceptions: false,
                        action: faCtx =>
                            {
                                var key = faCtx.Item;

                                SetVar(vars: faCtx.State.Vars,
                                       key: key,
                                       value: faCtx.State.Request.Headers[key]);
                            },
                        actionState: new
                            {
                                Request = ctx.Request,
                                Vars = headers,
                            });
                }
                catch
                {
                    // ignore errors
                }
                finally
                {
                    this._HEADERS = new ReadOnlyDictionaryWrapper<string, string>(dict: headers);
                }
            }

            #endregion Constrcutors (1)

            #region Properties (10)

            public override Uri Address
            {
                get { return this._CONTEXT.Request.Url; }
            }

            public override string ContentType
            {
                get { return this._CONTEXT.Request.ContentType; }
            }

            public override IReadOnlyDictionary<string, string> GET
            {
                get { return this._GET_VARS; }
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
                get { return this._POST_VARS; }
            }

            public override ITcpAddress RemoteAddress
            {
                get { return this._REMOTE_ADDR; }
            }

            public override IReadOnlyDictionary<string, string> REQUEST
            {
                get { return this._REQUEST_VARS; }
            }

            public override DateTimeOffset Time
            {
                get { return this._TIME; }
            }

            public override IPrincipal User
            {
                get { return this._USER; }
            }

            #endregion Properties (10)

            #region Methods (2)

            protected override Stream OnGetBody()
            {
                return this._CONTEXT.Request.InputStream;
            }

            private static void SetVar(IDictionary<string, string> vars, string key, string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = null;
                }

                vars[key ?? string.Empty] = value;
            }

            #endregion Methods (2)
        }

        #endregion Nested classes (1)
    }
}