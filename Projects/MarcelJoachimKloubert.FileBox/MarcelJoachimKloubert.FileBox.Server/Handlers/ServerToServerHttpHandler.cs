// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using System;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    internal sealed partial class ServerToServerHttpHandler : HttpHandlerBase
    {
        #region Constructors (1)

        internal ServerToServerHttpHandler(FileBoxHost host, IHttpServer server)
            : base(host: host,
                   server: server)
        {
        }

        #endregion Constructors (1)

        #region Methods (1)

        protected override void HandleRequest(object sender, CLRToolbox.Net.Http.HttpRequestEventArgs e)
        {
            Action<HttpRequestEventArgs> actionToInvoke = null;

            var addr = e.Request.Address;
            if (addr != null)
            {
                var normalizedUrl = (addr.AbsolutePath ?? string.Empty).ToLower().Trim();
                if (normalizedUrl != string.Empty)
                {
                    while (normalizedUrl.StartsWith("/"))
                    {
                        normalizedUrl = normalizedUrl.Substring(1).Trim();
                    }
                }

                switch (normalizedUrl)
                {
                    case "send-file-remote":
                        actionToInvoke = this.SendFileRemote;
                        break;
                }
            }

            if (actionToInvoke != null)
            {
                actionToInvoke(e);
            }
            else
            {
                e.Response.DocumentNotFound = true;
            }
        }

        #endregion Methods (1)
    }
}