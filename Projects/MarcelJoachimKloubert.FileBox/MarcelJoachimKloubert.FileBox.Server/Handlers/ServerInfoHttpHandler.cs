// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Web;
using MarcelJoachimKloubert.FileBox.Server.Extensions;
using MarcelJoachimKloubert.FileBox.Server.Json;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    /// <summary>
    /// The HTTP handler that lists a directory.
    /// </summary>
    public sealed class ServerInfoHttpHandler : FileBoxHttpHandlerBase
    {
        #region Constrcutors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerInfoHttpHandler" /> class.
        /// </summary>
        /// <param name="handler">
        /// The handler for the <see cref="FileBoxHttpHandlerBase.CheckLogin(string, SecureString, ref bool, ref IPrincipal)" /> method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler" /> is <see langword="null" />.
        /// </exception>
        public ServerInfoHttpHandler(CheckLoginHandler handler)
            : base(handler: handler)
        {
        }

        #endregion Constrcutors (1)

        #region Methods (2)

        /// <inheriteddoc />
        protected override void OnProcessRequest_Authorized(IHttpRequestContext context)
        {
            var result = new JsonResult();

            try
            {
                result.code = 0;
                
                var user = context.GetUser<IServerPrincipal>();
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

            context.Http.Response.WriteJson(result);
        }

        #endregion Methods (2)
    }
}