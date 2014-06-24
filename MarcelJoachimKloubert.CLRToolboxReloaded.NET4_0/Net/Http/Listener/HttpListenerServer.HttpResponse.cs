// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http.Listener
{
    partial class HttpListenerServer
    {
        #region Nested classes (1)

        private sealed class HttpResponse : HttpResponseBase
        {
            #region Fields (3)

            private readonly HttpListenerContext _CONTEXT;
            private readonly IDictionary<string, object> _FRONTEND_VARS;
            private readonly IDictionary<string, string> _HEADERS;

            #endregion Fields (3)

            #region Constrcutors (1)

            internal HttpResponse(HttpListenerContext ctx,
                                  Stream stream)
                : base(stream: stream)
            {
                this._CONTEXT = ctx;

                this._FRONTEND_VARS = new Dictionary<string, object>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                           emptyIsNull: true));

                this._HEADERS = new Dictionary<string, string>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                     emptyIsNull: true));
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
        }

        #endregion Nested classes (1)
    }
}