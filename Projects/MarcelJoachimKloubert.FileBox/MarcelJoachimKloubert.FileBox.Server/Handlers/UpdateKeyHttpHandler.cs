// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using MarcelJoachimKloubert.CLRToolbox.Web;
using MarcelJoachimKloubert.FileBox.Server.Extensions;
using MarcelJoachimKloubert.FileBox.Server.IO;
using MarcelJoachimKloubert.FileBox.Server.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    /// <summary>
    /// The HTTP handler that lists a directory.
    /// </summary>
    public sealed class UpdateKeyHttpHandler : FileBoxHttpHandlerBase
    {
        #region Constrcutors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateKeyHttpHandler" /> class.
        /// </summary>
        /// <param name="handler">
        /// The handler for the <see cref="FileBoxHttpHandlerBase.CheckLogin(string, SecureString, ref bool, ref IPrincipal)" /> method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler" /> is <see langword="null" />.
        /// </exception>
        public UpdateKeyHttpHandler(CheckLoginHandler handler)
            : base(handler: handler)
        {
        }

        #endregion Constrcutors (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessRequest_Authorized(IHttpRequestContext context)
        {
            var result = new JsonResult();

            try
            {
                if ("POST" == context.Http.Request.HttpMethod.ToUpper().Trim())
                {
                    result.code = 0;
                    
                    var dirs = ServiceLocator.Current.GetInstance<IDirectories>();

                    var filesDir = new DirectoryInfo(dirs.Files);
                    if (filesDir.Exists == false)
                    {
                        filesDir.Create();
                        filesDir.Refresh();
                    }

                    var userDir = new DirectoryInfo(Path.Combine(filesDir.FullName, context.User.Identity.Name));
                    if (userDir.Exists == false)
                    {
                        userDir.Create();
                        userDir.Refresh();
                    }

                    string xml;
                    using (var temp = new MemoryStream())
                    {
                        using (var stream = context.Http.Request.GetBufferlessInputStream())
                        {
                            stream.CopyTo(temp);

                            stream.Close();
                        }

                        xml = Encoding.UTF8.GetString(temp.ToArray());
                    }

                    var rsa = new RSACryptoServiceProvider();
                    rsa.FromXmlString(xml);

                    var keyFile = new FileInfo(Path.Combine(userDir.FullName, "key.xml"));
                    if (keyFile.Exists)
                    {
                        keyFile.Delete();
                        keyFile.Refresh();
                    }

                    File.WriteAllText(path: keyFile.FullName,
                                      contents: rsa.ToXmlString(includePrivateParameters: false),
                                      encoding: Encoding.UTF8);
                }
                else
                {
                    result.code = -2;
                    result.msg = "Method not allowed";
                }
            }
            catch (Exception ex)
            {
                SetupJsonResultByException(result, ex);
            }

            context.Http.Response.WriteJson(result);
        }

        #endregion Methods (1)
    }
}