// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web
{
    internal class WebExecutionContext : ObjectBase, IWebExecutionContext
    {
        #region Properties (2)

        public IHttpRequest Request
        {
            get;
            internal set;
        }

        public IHttpResponse Response
        {
            get;
            internal set;
        }

        #endregion Properties (2)
    }
}