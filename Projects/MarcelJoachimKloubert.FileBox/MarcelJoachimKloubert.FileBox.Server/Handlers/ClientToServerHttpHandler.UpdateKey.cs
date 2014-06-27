// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Json;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    partial class ClientToServerHttpHandler
    {
        #region Methods (1)

        private void UpdateKey(HttpRequestEventArgs e)
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

                var sender = (IServerPrincipal)e.Request.User;

                var filesDir = new DirectoryInfo(sender.Files);
                if (filesDir.Exists == false)
                {
                    filesDir.Create();
                    filesDir.Refresh();
                }

                var userDir = new DirectoryInfo(Path.Combine(filesDir.FullName, sender.Identity.Name));
                if (userDir.Exists == false)
                {
                    userDir.Create();
                    userDir.Refresh();
                }

                string xml;
                using (var temp = new MemoryStream())
                {
                    using (var stream = e.Request.GetBody())
                    {
                        stream.CopyTo(temp);

                        stream.Close();
                    }

                    xml = new UTF8Encoding().GetString(temp.ToArray());
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
            catch (Exception ex)
            {
                SetupJsonResultByException(result, ex);
            }

            e.Response.WriteJson(result);
        }

        #endregion Methods (1)
    }
}