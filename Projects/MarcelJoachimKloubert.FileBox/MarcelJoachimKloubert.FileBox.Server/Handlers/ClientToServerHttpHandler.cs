// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Json;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    internal sealed partial class ClientToServerHttpHandler : HttpHandlerBase
    {
        #region Constructors (1)

        internal ClientToServerHttpHandler(FileBoxHost host, IHttpServer server)
            : base(host: host,
                   server: server)
        {
        }

        #endregion Constructors (1)

        #region Methods (3)

        protected override void HandleRequest(object sender, HttpRequestEventArgs e)
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
                    case "list-inbox":
                        actionToInvoke = this.ListInbox;
                        break;

                    case "list-outbox":
                        actionToInvoke = this.ListOutbox;
                        break;

                    case "receive-file-inbox":
                        actionToInvoke = this.ReceiveInboxFile;
                        break;

                    case "receive-file-outbox":
                        actionToInvoke = this.ReceiveOutboxFile;
                        break;

                    case "send-file":
                        actionToInvoke = this.SendFile;
                        break;

                    case "server-info":
                        actionToInvoke = this.ServerInfo;
                        break;

                    case "update-key":
                        actionToInvoke = this.UpdateKey;
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

        private void ListBox(HttpRequestEventArgs e, string boxPath)
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

                var files = new List<object>();
                var sender = (IServerPrincipal)e.Request.User;
                var rsa = sender.TryGetRsaCrypter();

                var boxDir = new DirectoryInfo(boxPath);
                if (boxDir.Exists)
                {
                    boxDir.GetFiles("*" + GlobalConstants.FileExtensions.META_FILE)
                          .ForAll(throwExceptions: false,
                                  action: ctx =>
                                      {
                                          var metaFile = ctx.Item;

                                          ulong index;
                                          if (ulong.TryParse(Path.GetFileNameWithoutExtension(metaFile.Name), out index) == false)
                                          {
                                              // must be a valid number
                                              return;
                                          }

                                          var metaPwdFile = new FileInfo(Path.Combine(metaFile.DirectoryName, index.ToString() + GlobalConstants.FileExtensions.META_PASSWORD_FILE));
                                          if (metaPwdFile.Exists == false)
                                          {
                                              // no password file for meta data found
                                              return;
                                          }

                                          var dataFile = new FileInfo(Path.Combine(metaFile.DirectoryName, index.ToString() + GlobalConstants.FileExtensions.DATA_FILE));
                                          if (dataFile.Exists == false)
                                          {
                                              // no data file found
                                              return;
                                          }

                                          ctx.State.FileList.Add(new
                                              {
                                                  name = index.ToString(),

                                                  meta = new
                                                  {
                                                      dat = Convert.ToBase64String(File.ReadAllBytes(metaFile.FullName)),
                                                      sec = Convert.ToBase64String(File.ReadAllBytes(metaPwdFile.FullName)),
                                                  },
                                              });
                                      },
                                  actionState: new
                                      {
                                          MetaFileEncoding = new UTF8Encoding(),
                                          FileList = files,
                                      });
                }

                result.data = new
                    {
                        files = files.ToArray(),
                        key = rsa != null ? rsa.ToXmlString(includePrivateParameters: false) : null,
                    };
            }
            catch (Exception ex)
            {
                SetupJsonResultByException(result, ex);
            }

            e.Response.WriteJson(result);
        }

        private void ReceiveBoxFile(HttpRequestEventArgs e, string boxPath)
        {
            if (e.Request.TryGetKnownMethod() != HttpMethod.GET)
            {
                e.Response.StatusCode = HttpStatusCode.MethodNotAllowed;
                return;
            }

            var fileFound = false;

            var fileName = (e.Request.Headers["X-FileBox-File"] ?? string.Empty).Trim();

            ulong index;
            if (ulong.TryParse(fileName, out index))
            {
                var boxDir = new DirectoryInfo(boxPath);
                if (boxDir.Exists)
                {
                    var dataFile = new FileInfo(Path.Combine(boxDir.FullName,
                                                             index.ToString() + GlobalConstants.FileExtensions.DATA_FILE));
                    var metaFile = new FileInfo(Path.Combine(boxDir.FullName,
                                                             index.ToString() + GlobalConstants.FileExtensions.META_FILE));
                    var metaPwdFile = new FileInfo(Path.Combine(boxDir.FullName,
                                                                index.ToString() + GlobalConstants.FileExtensions.META_PASSWORD_FILE));

                    if (dataFile.Exists &&
                        metaFile.Exists &&
                        metaPwdFile.Exists)
                    {
                        fileFound = true;

                        using (var stream = dataFile.OpenRead())
                        {
                            stream.CopyTo(e.Response.Stream);
                        }
                    }
                }
            }

            if (fileFound == false)
            {
                e.Response.DocumentNotFound = true;
            }
        }

        #endregion Methods (3)
    }
}