// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Json;
using System;
using System.Net;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    partial class ServerToServerHttpHandler
    {
        #region Methods (1)

        private void SendFileRemote(HttpRequestEventArgs e)
        {
            if (e.Request.TryGetKnownMethod() != HttpMethod.PUT)
            {
                e.Response.StatusCode = HttpStatusCode.MethodNotAllowed;

                return;
            }

            var result = new JsonResult();

            try
            {
                result.code = 0;

                var from = (e.Request.Headers["X-FileBox-From"] ?? string.Empty).ToLower().Trim();
                if (from != string.Empty)
                {
                    var to = (e.Request.Headers["X-FileBox-To"] ?? string.Empty).ToLower().Trim();
                    if (to != string.Empty)
                    {
                    }
                    else
                    {
                        // no recipients defined

                        result.code = -3;
                    }
                }
                else
                {
                    // no sender defined

                    result.code = -2;
                }
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