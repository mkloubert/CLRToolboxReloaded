// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Wcf
{
    partial class WcfHttpServer
    {
        #region Nested classes (1)

        internal sealed class HttpResponse : HttpResponseBase, IDisposable
        {
            #region Fields (3)

            private readonly IDictionary<string, object> _FRONTEND_VARS;
            private readonly IDictionary<string, string> _HEADERS;
            private readonly WcfHttpServer _SERVER;

            #endregion Fields (3)

            #region Constructors (2)

            internal HttpResponse(HttpResponseMessageProperty property,
                                  WcfHttpServer srv)
                : base(stream: srv.CreateResponseStream())
            {
                this._SERVER = srv;
                this.Property = property;

                this._FRONTEND_VARS = new Dictionary<string, object>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                           emptyIsNull: true));

                this._HEADERS = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                     emptyIsNull: true));
            }

            ~HttpResponse()
            {
                this.Dispose(false);
            }

            #endregion Constructors (2)

            #region Properties (3)

            public override IDictionary<string, object> FrontendVars
            {
                get { return this._FRONTEND_VARS; }
            }

            public override IDictionary<string, string> Headers
            {
                get { return this._HEADERS; }
            }

            internal HttpResponseMessageProperty Property
            {
                get;
                private set;
            }

            #endregion Properties (3)

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
                        .CloseResponseStream(this.Stream);
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