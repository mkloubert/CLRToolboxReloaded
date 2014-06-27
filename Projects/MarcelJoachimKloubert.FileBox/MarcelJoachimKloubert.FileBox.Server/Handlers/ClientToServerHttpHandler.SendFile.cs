// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
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

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    partial class ClientToServerHttpHandler
    {
        #region Methods (1)

        private void SendFile(HttpRequestEventArgs e)
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

                var syncRoot = new object();

                var sender = (IServerPrincipal)e.Request.User;
                var senderRsa = sender.TryGetRsaCrypter();

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
                        var tempFile = FileHelper.CreateTempFile(tempDir: sender.Temp);

                        try
                        {
                            var fileId = Guid.NewGuid();
                            var fileDate = e.Request.Time;

                            var fileLastWriteTime = TryParseTime(e.Request.Headers["X-FileBox-LastWriteTime"]) ?? fileDate;
                            var fileCreationTime = TryParseTime(e.Request.Headers["X-FileBox-CreationTime"]) ?? fileDate;

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
                            meta.SetAttributeValue("id", fileId.ToString(GUID_FORMAT));

                            // sender
                            {
                                var fromElement = new XElement("from",
                                                               sender.Identity.Name);
                                fromElement.SetAttributeValue("id",
                                                              sender.Identity.Id.ToString(GUID_FORMAT));

                                if (senderRsa != null)
                                {
                                    fromElement.SetAttributeValue("publicKey",
                                                                  Convert.ToBase64String(senderRsa.ExportCspBlob(includePrivateParameters: false)));
                                }

                                meta.Add(fromElement);
                            }

                            // file dates
                            meta.Add(new XElement("date",
                                                  fileDate.ToString(LONG_TIME_FORMAT, CultureInfo.InvariantCulture)));
                            meta.Add(new XElement("lastWriteTime",
                                                  fileLastWriteTime.ToString(LONG_TIME_FORMAT, CultureInfo.InvariantCulture)));
                            meta.Add(new XElement("creationTime",
                                                  fileCreationTime.ToString(LONG_TIME_FORMAT, CultureInfo.InvariantCulture)));

                            // crypt data
                            using (var tempStream = new FileStream(tempFile.FullName,
                                                                   FileMode.Create, FileAccess.ReadWrite))
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
                                    var r = ctx.Item;
                                    string remoteHost = null;

                                    var adIndex = r.IndexOf('@');
                                    if (adIndex > -1)
                                    {
                                        remoteHost = r.Substring(adIndex + 1).ToLower().Trim();
                                        r = r.Substring(0, adIndex).Trim();
                                    }

                                    if (string.IsNullOrWhiteSpace(r))
                                    {
                                        // no recipient defined
                                        return;
                                    }

                                    // check if host is local
                                    if (string.IsNullOrWhiteSpace(remoteHost) == false)
                                    {
                                        remoteHost = remoteHost.ToLower().Trim();

                                        if ((remoteHost == "127.0.0.1") ||
                                            (remoteHost == "localhost"))
                                        {
                                            // is local
                                            remoteHost = null;
                                        }
                                        else if (this._HOST.GetHostNames().Contains(remoteHost))
                                        {
                                            // is local
                                            remoteHost = null;
                                        }
                                    }

                                    string recipientAddr;
                                    if (string.IsNullOrWhiteSpace(remoteHost))
                                    {
                                        recipientAddr = r;
                                    }
                                    else
                                    {
                                        recipientAddr = r + "@" + remoteHost;
                                    }

                                    var metaCopy = new XElement(ctx.State.Meta);
                                    metaCopy.Add(new XElement("to", recipientAddr));

                                    IJob newJob;
                                    if (string.IsNullOrWhiteSpace(remoteHost))
                                    {
                                        // local recipient

                                        newJob = new SendJob(sync: ctx.State.Sync,
                                                             host: ctx.State.Host,
                                                             tempFile: tempFile.FullName,
                                                             pwd: ctx.State.Password, salt: ctx.State.Salt,
                                                             sender: ctx.State.Sender, recipient: r,
                                                             meta: metaCopy);
                                    }
                                    else
                                    {
                                        // send to remote server

                                        newJob = new SendRemoteJob(sync: ctx.State.Sync,
                                                                   host: ctx.State.Host,
                                                                   tempFile: tempFile.FullName,
                                                                   pwd: ctx.State.Password, salt: ctx.State.Salt,
                                                                   sender: ctx.State.Sender,
                                                                   recipient: r, remoteHost: remoteHost,
                                                                   meta: metaCopy);
                                    }

                                    ctx.State
                                       .Host
                                       .EnqueueJob(newJob);
                                }, new
                                {
                                    Host = this._HOST,
                                    Meta = meta,
                                    Password = pwd,
                                    Salt = salt,
                                    Sender = sender,
                                    Sync = syncRoot,
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