// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.IO;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Execution.Jobs;
using MarcelJoachimKloubert.FileBox.Server.Helpers;
using MarcelJoachimKloubert.FileBox.Server.Json;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox.Server
{
    partial class FileBoxHost
    {
        #region Methods (1)

        private void SendFile(HttpRequestEventArgs e)
        {
            if (e.Request.TryGetKnownMethod() != HttpMethod.POST)
            {
                e.Response.StatusCode = HttpStatusCode.MethodNotAllowed;
                return;
            }

            var result = new JsonResult();

            try
            {
                result.code = 0;

                var syncRoot = new object();
                var sender = (IServerPrincipal)e.Request.User;

                var targetDir = new DirectoryInfo(sender.Inbox);
                if (targetDir.Exists == false)
                {
                    targetDir.Create();
                    targetDir.Refresh();
                }

                var fileName = (e.Request.Headers["X-FileBox-Filename"] ?? string.Empty).Trim();
                if (fileName != string.Empty)
                {
                    var recipients = (e.Request.Headers["X-FileBox-To"] ?? string.Empty).Split(';')
                                                                                        .Select(r => r.ToLower().Trim())
                                                                                        .Where(r => r != string.Empty)
                                                                                        .Distinct();

                    var rand = new CryptoRandom();

                    // copy to temporary file
                    using (var stream = new EventStream(e.Request.GetBody()))
                    {
                        // define unique temp file
                        FileInfo tempFile;
                        do
                        {
                            var fBlob = new byte[4];
                            rand.NextBytes(fBlob);

                            tempFile = new FileInfo(Path.Combine(sender.Temp,
                                                                 fBlob.AsHexString() + GlobalConstants.FileExtensions.TEMP_FILE));
                        }
                        while (tempFile.Exists);

                        try
                        {
                            var fileId = Guid.NewGuid();
                            var fileDate = e.Request.Time.ToUniversalTime();

                            var fileLastWriteTime = fileDate;
                            var fileCreationTime = fileDate;

                            long fileSize = 0;
                            stream.DataTransfered += (s, e2) =>
                                {
                                    switch (e2.Context)
                                    {
                                        case EventStreamDataTransferedContext.Read:
                                            fileSize += e2.Buffer.Length;
                                            break;
                                    }
                                };

                            // generate password for the temp file
                            var pwd = new byte[48];
                            rand.NextBytes(pwd);

                            // generate salt for the temp file
                            var salt = new byte[16];
                            rand.NextBytes(salt);

                            var meta = new XElement("file");
                            meta.SetAttributeValue("name", fileName);
                            meta.SetAttributeValue("id", fileId.ToString("N"));

                            // file dates
                            meta.Add(new XElement("date",
                                                  fileDate.ToString("u", CultureInfo.InvariantCulture)));
                            meta.Add(new XElement("lastWriteTime",
                                                  fileLastWriteTime.ToString("u", CultureInfo.InvariantCulture)));
                            meta.Add(new XElement("creationTime",
                                                  fileCreationTime.ToString("u", CultureInfo.InvariantCulture)));

                            // crypt data
                            using (var tempStream = new FileStream(tempFile.FullName,
                                                                   FileMode.CreateNew, FileAccess.ReadWrite))
                            {
                                var cryptStream = new CryptoStream(tempStream,
                                                                   CryptoHelper.CreateRijndael(pwd: pwd,
                                                                                               salt: salt).CreateEncryptor(),
                                                                   CryptoStreamMode.Write);
                                stream.CopyTo(cryptStream);

                                cryptStream.Flush();
                                cryptStream.Close();
                            }

                            // file size
                            meta.Add(new XElement("size",
                                                  fileSize));

                            recipients.ForAll((ctx) =>
                                {
                                    ctx.State
                                       .Host
                                       .EnqueueJob(new SendJob(sync: syncRoot,
                                                               host: ctx.State.Host,
                                                               tempFile: tempFile.FullName,
                                                               pwd: pwd, salt: salt,
                                                               sender: sender, recipient: ctx.Item,
                                                               meta: new XElement(meta)));
                                }, new
                                {
                                    Host = this,
                                });
                        }
                        catch
                        {
                            // delete temp file before rethrow exception
                            this.TryDeleteFile(tempFile);

                            throw;
                        }
                    }
                }
                else
                {
                    result.code = -2;
                    result.msg = "Invalid filename";
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