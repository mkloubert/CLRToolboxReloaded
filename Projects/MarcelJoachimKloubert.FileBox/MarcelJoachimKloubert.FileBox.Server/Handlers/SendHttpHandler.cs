﻿// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using MarcelJoachimKloubert.CLRToolbox.Web;
using MarcelJoachimKloubert.FileBox.Server.Execution.Jobs;
using MarcelJoachimKloubert.FileBox.Server.Extensions;
using MarcelJoachimKloubert.FileBox.Server.IO;
using MarcelJoachimKloubert.FileBox.Server.Json;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    /// <summary>
    /// The HTTP handler that lists a directory.
    /// </summary>
    public sealed partial class SendHttpHandler : FileBoxHttpHandlerBase
    {
        #region Fields (1)

        private const int _ITERATIONS = 1000;

        #endregion Fields (1)

        #region Constrcutors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="SendHttpHandler" /> class.
        /// </summary>
        /// <param name="handler">
        /// The handler for the <see cref="FileBoxHttpHandlerBase.CheckLogin(string, SecureString, ref bool, ref IPrincipal)" /> method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler" /> is <see langword="null" />.
        /// </exception>
        public SendHttpHandler(CheckLoginHandler handler)
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
                if ("POST" == context.Http.Request.HttpMethod.ToUpper().Trim())
                {
                    result.code = 0;

                    var syncRoot = new object();
                    var sender = context.GetUser<IServerPrincipal>();

                    var targetDir = new DirectoryInfo(sender.Inbox);
                    if (targetDir.Exists == false)
                    {
                        targetDir.Create();
                        targetDir.Refresh();
                    }

                    var fileName = (context.Http.Request.Headers["X-FileBox-Filename"] ?? string.Empty).Trim();
                    if (fileName != string.Empty)
                    {
                        var dirs = ServiceLocator.Current.GetInstance<IDirectories>();

                        var recipients = (context.Http.Request.Headers["X-FileBox-To"] ?? string.Empty).Split(';')
                                                                                                       .Select(r => r.ToLower().Trim())
                                                                                                       .Where(r => r != string.Empty)
                                                                                                       .Distinct();

                        var rand = new CryptoRandom();

                        // copy to temporary file
                        using (var stream = context.Http.Request.GetBufferlessInputStream())
                        {
                            // define unique temp file
                            FileInfo tempFile;
                            do
                            {
                                var fBlob = new byte[4];
                                rand.NextBytes(fBlob);

                                tempFile = new FileInfo(Path.Combine(dirs.Temp,
                                                                     fBlob.AsHexString() + ".tmp"));
                            }
                            while (tempFile.Exists);

                            try
                            {
                                // generate password
                                var pwd = new byte[48];
                                rand.NextBytes(pwd);

                                // generate salt
                                var salt = new byte[16];
                                rand.NextBytes(salt);

                                var meta = new XElement("file");
                                meta.SetAttributeValue("name", fileName);

                                // crypt data
                                using (var tempStream = new FileStream(tempFile.FullName,
                                                                       FileMode.CreateNew, FileAccess.ReadWrite))
                                {
                                    var cryptStream = new CryptoStream(tempStream,
                                                                       CreateRijndael(pwd: pwd,
                                                                                      salt: salt).CreateEncryptor(),
                                                                       CryptoStreamMode.Write);
                                    stream.CopyTo(cryptStream);

                                    cryptStream.Flush();
                                    cryptStream.Close();
                                }

                                var queue = ServiceLocator.Current.GetInstance<IJobQueue>();

                                foreach (var r in recipients)
                                {
                                    queue.Enqueue(new SendJob(sync: syncRoot,
                                                              tempFile: tempFile.FullName,
                                                              pwd: pwd, salt: salt,
                                                              sender: sender, recipient: r,
                                                              meta: new XElement(meta)));
                                }
                            }
                            catch
                            {
                                // delete temp file before rethrow exception
                                try
                                {
                                    if (File.Exists(tempFile.FullName))
                                    {
                                        File.Delete(tempFile.FullName);
                                    }
                                }
                                catch
                                {
                                    // ignore here
                                }

                                throw;
                            }
                        }
                    }
                    else
                    {
                        result.code = -3;
                        result.msg = "Invalid filename";
                    }
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

        protected internal static Rijndael CreateRijndael(byte[] pwd, byte[] salt)
        {
            var pdb = new Rfc2898DeriveBytes(pwd, salt,
                                             1000);

            var result = Rijndael.Create();
            result.Key = pdb.GetBytes(32);
            result.IV = pdb.GetBytes(16);

            return result;
        }

        #endregion Methods (2)
    }
}