// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Listener
{
    partial class HttpListenerServer
    {
        #region Nested classes (1)

        private sealed class HttpResponse : HttpResponseBase, IDisposable
        {
            #region Fields (5)

            private readonly HttpListenerContext _CONTEXT;
            private readonly IDictionary<string, object> _FRONTEND_VARS;
            private readonly IDictionary<string, string> _HEADERS;
            private readonly Stream _INITIAL_STREAM;
            private readonly HttpListenerServer _SERVER;

            #endregion Fields (4)

            #region Constrcutors (1)

            internal HttpResponse(HttpListenerServer server,
                                  HttpListenerContext ctx)
                : base(stream: server.CreateResponseStream(ctx))
            {
                this._CONTEXT = ctx;
                this._INITIAL_STREAM = this.Stream;
                this._SERVER = server;

                this._FRONTEND_VARS = new Dictionary<string, object>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                           emptyIsNull: true));

                this._HEADERS = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                     emptyIsNull: true));
            }

            ~HttpResponse()
            {
                this.Dispose(false);
            }

            #endregion Constrcutors (1)

            #region Properties (2)

            public override IDictionary<string, object> FrontendVars
            {
                get { return this._FRONTEND_VARS; }
            }

            public override IDictionary<string, string> Headers
            {
                get { return this._HEADERS; }
            }

            #endregion Properties (2)

            #region Methods (2)

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
                        .CloseResponseStream(this._CONTEXT,
                                             this._INITIAL_STREAM);
                }
                catch
                {
                    if (disposing)
                    {
                        throw;
                    }
                }
            }

            #endregion Methods (2)
        }

        #endregion Nested classes (1)
    }
}