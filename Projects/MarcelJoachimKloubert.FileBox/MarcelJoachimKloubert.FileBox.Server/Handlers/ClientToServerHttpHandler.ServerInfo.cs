// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Json;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.Net;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    partial class ClientToServerHttpHandler
    {
        #region Methods (1)

        private void ServerInfo(HttpRequestEventArgs e)
        {
            if (e.Request.TryGetKnownMethod() != HttpMethod.GET)
            {
                e.Response.StatusCode = HttpStatusCode.MethodNotAllowed;
                return;
            }

            var result = new JsonResult();

            try
            {
                result.code = 0;

                var user = (IServerPrincipal)e.Request.User;
                var rsa = user.TryGetRsaCrypter();

                result.data = new
                {
                    name = Environment.MachineName,

                    user = new
                        {
                            key = rsa != null ? rsa.ToXmlString(includePrivateParameters: false) : null,
                            name = user.Identity.Name,
                        },
                };
            }
            catch (Exception ex)
            {
                SetupJsonResultByException(result, ex);
            }

            e.Response.WriteJson(result);
        }

        #endregion Methods (1)
    }
}