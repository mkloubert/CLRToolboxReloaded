// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Handles a FileBox (server) connection.
    /// </summary>
    public sealed class FileBoxConnection : FileBoxObjectBase
    {
        #region Fields (1)

        /// <summary>
        /// .NET format for a long time string.
        /// </summary>
        public const string LONG_TIME_FORMAT = "u";

        #endregion Fields (1)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBoxConnection" /> class.
        /// </summary>
        public FileBoxConnection()
        {
            this.IsSecure = true;
        }

        /// <summary>
        /// Frees the resources of that object.
        /// </summary>
        ~FileBoxConnection()
        {
            using (var pwd = this.Password)
            {
                this.Password = null;
            }
        }

        #endregion Constructors (2)

        #region Properties (6)

        /// <summary>
        /// Gets or sets the default data encoding.
        /// </summary>
        public Encoding DefaultEncoding
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the host address.
        /// </summary>
        public string Host
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the TCP port.
        /// </summary>
        public bool IsSecure
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public SecureString Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the TCP port.
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the username.
        /// </summary>
        public string User
        {
            get;
            set;
        }

        #endregion Properties (6)

        #region Methods (16)

        /// <summary>
        /// Creates a basic and setupped HTTP web request client.
        /// </summary>
        /// <param name="relativePath">The additional path for the request URI.</param>
        /// <returns>The created instance.</returns>
        public HttpWebRequest CreateWebRequest(string relativePath)
        {
            var result = (HttpWebRequest)HttpWebRequest.Create(this.GetBaseUrl() + relativePath.Trim());

            // basic auth
            NetworkCredential cred = this.GetCredentials();
            if (cred != null)
            {
                string authInfo;
                var ptr = IntPtr.Zero;
                try
                {
                    if (cred.SecurePassword != null)
                    {
                        ptr = Marshal.SecureStringToGlobalAllocUnicode(cred.SecurePassword);
                    }

                    authInfo = string.Format("{0}:{1}",
                                             this.User,
                                             Marshal.PtrToStringUni(ptr));

                    result.Headers["Authorization"] = string.Format("Basic {0}",
                                                                    Convert.ToBase64String(Encoding.ASCII
                                                                                                   .GetBytes(authInfo)));
                }
                finally
                {
                    authInfo = null;
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                }
            }

            return result;
        }

        private string GetBaseUrl()
        {
            return string.Format("http{0}://{1}:{2}/",
                                 this.IsSecure ? "s" : string.Empty,
                                 string.IsNullOrWhiteSpace(this.Host) ? "localhost" : this.Host.Trim(),
                                 this.Port);
        }

        private ExecutionContext<List<FileItem>> GetBox(Location loc, RSACryptoServiceProvider rsa, int startAt, int? maxItems)
        {
            if (rsa == null)
            {
                throw new ArgumentNullException("rsa");
            }

            if (startAt < 0)
            {
                throw new ArgumentOutOfRangeException("startAt");
            }

            if (maxItems < 0)
            {
                throw new ArgumentOutOfRangeException("maxItems");
            }

            string path;
            switch (loc)
            {
                case Location.Inbox:
                    path = "list-inbox";
                    break;

                case Location.Outbox:
                    path = "list-outbox";
                    break;

                default:
                    throw new NotSupportedException(loc.ToString());
            }

            var result = new ExecutionContext<List<FileItem>>();
            result.SetFunc((ctx, s) =>
                {
                    var fileList = new List<FileItem>();

                    s.Request.Method = "GET";

                    s.Request.Headers["X-FileBox-StartAt"] = s.StartAt.ToString();

                    if (s.MaximumItems.HasValue)
                    {
                        s.Request.Headers["X-FileBox-MaxItems"] = s.MaximumItems.ToString();
                    }

                    var jsonResult = s.Connection
                                      .GetJsonObject(s.Request.GetResponse());

                    switch (jsonResult.code)
                    {
                        case 0:
                            if (jsonResult.data != null)
                            {
                                foreach (dynamic item in jsonResult.data.files)
                                {
                                    var newItem = new FileItem();
                                    newItem.IsCorrupted = true;
                                    newItem.Location = loc;
                                    newItem.Server = this;
                                    newItem.Size = -1;

                                    try
                                    {
                                        newItem.RealName = item.name.Trim();
                                        if (newItem.RealName != string.Empty)
                                        {
                                            var meta = item.meta;

                                            var metaPwdAndSalt = rsa.Decrypt(Convert.FromBase64String(meta.sec.Trim()),
                                                                             false);

                                            using (var cryptedMetaStream = new MemoryStream(buffer: Convert.FromBase64String(meta.dat.Trim()),
                                                                                            writable: false))
                                            {
                                                using (var decryptedMetaStream = new MemoryStream())
                                                {
                                                    var cryptoMetaStream = new CryptoStream(cryptedMetaStream,
                                                                                            CreateRijndael(pwd: metaPwdAndSalt.Skip(7).Take(48).ToArray(),
                                                                                                           salt: metaPwdAndSalt.Skip(7 + 48).Take(16).ToArray()).CreateDecryptor(),
                                                                                            CryptoStreamMode.Read);

                                                    cryptoMetaStream.CopyTo(decryptedMetaStream);

                                                    decryptedMetaStream.Position = 0;
                                                    var xml = XDocument.Load(decryptedMetaStream).Root;
                                                    try
                                                    {
                                                        newItem.Id = Guid.Parse(xml.Attribute("id").Value.Trim());
                                                        newItem.Name = xml.Attribute("name").Value.Trim();

                                                        // size in bytes
                                                        newItem.Size = long.Parse(xml.Elements("size").Single().Value.Trim(),
                                                                                  s.DataCulture);

                                                        // creation date
                                                        newItem.CreationDate = DateTimeOffset.ParseExact(xml.Elements("creationTime").Single().Value.Trim(),
                                                                                                         LONG_TIME_FORMAT,
                                                                                                         s.DataCulture).ToLocalTime();

                                                        // last write time
                                                        newItem.LastWriteTime = DateTimeOffset.ParseExact(xml.Elements("lastWriteTime").Single().Value.Trim(),
                                                                                                          LONG_TIME_FORMAT,
                                                                                                          s.DataCulture).ToLocalTime();

                                                        // send time
                                                        newItem.SendTime = DateTimeOffset.ParseExact(xml.Elements("date").Single().Value.Trim(),
                                                                                                     LONG_TIME_FORMAT,
                                                                                                     s.DataCulture).ToLocalTime();

                                                        // save XML as secure string
                                                        newItem.CryptedMetaXml = new SecureString();
                                                        foreach (var c in xml.ToString())
                                                        {
                                                            newItem.CryptedMetaXml.AppendChar(c);
                                                        }
                                                        newItem.CryptedMetaXml.MakeReadOnly();
                                                    }
                                                    finally
                                                    {
                                                        xml = null;
                                                    }

                                                    newItem.CryptedMeta = decryptedMetaStream.ToArray();
                                                }
                                            }

                                            newItem.IsCorrupted = false;
                                        }
                                    }
                                    catch
                                    {
                                        newItem.CryptedMeta = null;
                                    }

                                    if (newItem.CryptedMeta == null)
                                    {
                                        newItem.CryptedMetaXml = null;
                                    }

                                    if (string.IsNullOrWhiteSpace(newItem.RealName) == false)
                                    {
                                        fileList.Add(newItem);
                                    }
                                }
                            }
                            break;
                    }

                    return fileList;
                }, new
                {
                    Connection = this,
                    DataCulture = CultureInfo.InvariantCulture,
                    MaximumItems = maxItems,
                    Request = this.CreateWebRequest(path),
                    StartAt = startAt,
                });

            return result;
        }

        private NetworkCredential GetCredentials()
        {
            NetworkCredential result = null;

            if (string.IsNullOrWhiteSpace(this.User) == false)
            {
                string domain = null;
                string user = null;

                var bs = this.User.IndexOf('\\');
                if (bs > 0)
                {
                    domain = this.User.Substring(0, bs).Trim();
                    if (domain == string.Empty)
                    {
                        domain = null;
                    }

                    user = this.User.Substring(bs + 1).Trim();
                }
                else
                {
                    user = this.User.Trim();
                }

                if (string.IsNullOrWhiteSpace(user))
                {
                    user = null;
                }

                result = new NetworkCredential(domain: domain,
                                               userName: user,
                                               password: this.Password);
            }

            return result;
        }

        /// <summary>
        /// Returns the encoder based on <see cref="FileBoxConnection.DefaultEncoding" />.
        /// </summary>
        /// <returns>
        /// The encoder. Returns an <see cref="UTF8Encoding" /> if <see cref="FileBoxConnection.DefaultEncoding" />
        /// is <see langword="null" />.
        /// </returns>
        public Encoding GetEncoding()
        {
            return this.DefaultEncoding ?? new UTF8Encoding();
        }

        /// <summary>
        /// Reads a JSON object from a response.
        /// </summary>
        /// <param name="response">The HTTP response context.</param>
        /// <param name="closeResponse">Close <paramref name="response" /> after reading or not.</param>
        /// <returns>The JSON object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="response" /> is <see langword="null" />.
        /// </exception>
        public JsonResult GetJsonObject(WebResponse response, bool closeResponse = true)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            var enc = this.GetEncoding();

            dynamic jsonResult;

            using (var respStream = response.GetResponseStream())
            {
                using (var txtReader = new StreamReader(respStream, enc))
                {
                    using (var reader = new JsonTextReader(txtReader))
                    {
                        var serializer = new JsonSerializer();
                        jsonResult = serializer.Deserialize<ExpandoObject>(reader);
                    }
                }
            }

            var result = jsonResult != null ? new JsonResult()
                {
                    code = jsonResult.code != null ? Convert.ToInt32(jsonResult.code) : null,
                    data = jsonResult.data,
                    msg = Convert.ChangeType(jsonResult.msg, typeof(string)),
                } : null;

            if (closeResponse)
            {
                response.Close();
            }

            return result;
        }

        /// <summary>
        /// Returns the files from the INBOX folder.
        /// </summary>
        /// <param name="rsa">The crypter for encrypting the meta data of the files.</param>
        /// <param name="startAt">The zero based index of the first item.</param>
        /// <param name="maxItems">The maximum number of items to return.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rsa" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startAt" /> and/or <paramref name="maxItems" /> are invalid.
        /// </exception>
        public IExecutionContext<List<FileItem>> GetInbox(RSACryptoServiceProvider rsa, int startAt = 0, int? maxItems = null)
        {
            return this.GetBox(loc: Location.Inbox,
                               rsa: rsa,
                               startAt: startAt, maxItems: maxItems);
        }

        /// <summary>
        /// Returns the files from the OUTBOX folder.
        /// </summary>
        /// <param name="rsa">The crypter for encrypting the meta data of the files.</param>
        /// <param name="startAt">The zero based index of the first item.</param>
        /// <param name="maxItems">The maximum number of items to return.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rsa" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startAt" /> and/or <paramref name="maxItems" /> are invalid.
        /// </exception>
        public IExecutionContext<List<FileItem>> GetOutbox(RSACryptoServiceProvider rsa, int startAt = 0, int? maxItems = null)
        {
            return this.GetBox(loc: Location.Outbox,
                               rsa: rsa,
                               startAt: startAt, maxItems: maxItems);
        }

        /// <summary>
        /// Gets information about the server.
        /// </summary>
        /// <param name="autoStart">Start operation directly or not.</param>
        /// <returns>The underlying execution result.</returns>
        public IExecutionContext<ServerInfo> GetServerInfo(bool autoStart = true)
        {
            var result = new ExecutionContext<ServerInfo>();

            result.SetFunc((ctx, s) =>
                {
                    s.Request.Method = "GET";

                    ServerInfo info = null;

                    var response = s.Request.GetResponse();

                    var jsonResult = s.Connection.GetJsonObject(response);
                    if (jsonResult != null)
                    {
                        var jsonInfo = jsonResult.data;

                        info = new ServerInfo();
                        info.Server = s.Connection;

                        info.Name = jsonInfo.name;

                        info.Key = (jsonInfo.user.key ?? string.Empty).Trim();
                        if (info.Key == string.Empty)
                        {
                            info.Key = null;
                        }
                    }

                    return info;
                }, new
                {
                    Connection = this,
                    Request = this.CreateWebRequest("server-info"),
                });

            if (autoStart)
            {
                result.Start();
            }

            return result;
        }

        private static void InvokeSafe(Action action)
        {
            try
            {
                action();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sets the value of <see cref="FileBoxConnection.Password" /> via a string.
        /// </summary>
        /// <param name="pwd">The new value.</param>
        public void SetPassword(string pwd)
        {
            var newValue = new SecureString();
            if (pwd != null)
            {
                foreach (var c in pwd)
                {
                    newValue.AppendChar(c);
                }
            }

            newValue.MakeReadOnly();
            this.Password = newValue;
        }

        /// <summary>
        /// Send a file.
        /// </summary>
        /// <param name="path">The path of the file to send.</param>
        /// <param name="recipients">The list of recipients.</param>
        /// <returns>The underlying execution context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="recipients" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// File in <paramref name="path" /> does not exist.
        /// </exception>
        /// <remarks>The operations directly starts.</remarks>
        public IExecutionContext Send(string path, params string[] recipients)
        {
            return this.Send(path: path,
                             autoStart: true,
                             recipients: recipients);
        }

        /// <summary>
        /// Send a file.
        /// </summary>
        /// <param name="path">The path of the file to send.</param>
        /// <param name="autoStart">Directly start operation or not.</param>
        /// <param name="recipients">The list of recipients.</param>
        /// <returns>The underlying execution context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="recipients" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// File in <paramref name="path" /> does not exist.
        /// </exception>
        public IExecutionContext Send(string path, bool autoStart, params string[] recipients)
        {
            return this.Send(path: path,
                             recipients: (IEnumerable<string>)recipients,
                             autoStart: autoStart);
        }

        /// <summary>
        /// Send a file.
        /// </summary>
        /// <param name="path">The path of the file to send.</param>
        /// <param name="recipients">The list of recipients.</param>
        /// <param name="autoStart">Directly start operation or not.</param>
        /// <returns>The underlying execution context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="recipients" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// File in <paramref name="path" /> does not exist.
        /// </exception>
        public IExecutionContext Send(string path,
                                      IEnumerable<string> recipients,
                                      bool autoStart = true)
        {
            if (recipients == null)
            {
                throw new ArgumentNullException("recipients");
            }

            var file = new FileInfo(path);
            if (file.Exists == false)
            {
                throw new FileNotFoundException();
            }

            var result = new ExecutionContext();
            result.SetAction((ctx, state) =>
                {
                    ctx.OverallPercentage = 0;
                    ctx.OverallProgressStatus = "Sending file operation";
                    ctx.OverallTaskId = 1;

                    ctx.CurrentStepPercentage = 0;
                    ctx.CurrentStepProgressStatus = "Sending file...";
                    ctx.CurrentStepTaskId = 1;

                    state.Request.Method = "PUT";
                    state.Request.ContentType = "application/octet-stream";

                    // file / content size
                    InvokeSafe(() => state.Request.ContentLength = state.File.Length);

                    // creation time
                    TrySetHeaderValue(state.Request.Headers,
                                      "X-FileBox-CreationTime",
                                      () => state.File.CreationTime.ToString(LONG_TIME_FORMAT));

                    // file name
                    state.Request.Headers["X-FileBox-Filename"] = state.File.Name.Trim();

                    // last write time
                    TrySetHeaderValue(state.Request.Headers,
                                      "X-FileBox-LastWriteTime",
                                      () => state.File.LastWriteTime.ToString(LONG_TIME_FORMAT));

                    // recipients
                    state.Request.Headers["X-FileBox-To"] = string.Join(";", recipients.Where(r => string.IsNullOrWhiteSpace(r) == false)
                                                                                       .Select(r => r.ToLower().Trim())
                                                                                       .Distinct());

                    // send file content
                    using (var stream = state.File.OpenRead())
                    {
                        using (var reqStream = state.Request.GetRequestStream())
                        {
                            // copy data
                            var dataSize = stream.Length;
                            if (dataSize > 0)
                            {
                                var buffer = new byte[81920];
                                int bytesRead;
                                long bytesWritten = 0;
                                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    reqStream.Write(buffer, 0, bytesRead);

                                    bytesWritten += bytesRead;
                                    ctx.CurrentStepPercentage = (double)bytesWritten / (double)dataSize * 100d;
                                }
                            }

                            reqStream.Flush();
                            reqStream.Close();
                        }
                    }

                    var json = this.GetJsonObject(state.Request.GetResponse());

                    ctx.OverallPercentage = 100;

                    switch (json.code)
                    {
                        case 0:
                            // OK
                            break;

                        default:
                            throw new FileBoxException(result: json);
                    }
                }, new
                {
                    File = file,
                    Request = this.CreateWebRequest("send-file"),
                });

            if (autoStart)
            {
                result.Start();
            }

            return result;
        }

        private static void TrySetHeaderValue(WebHeaderCollection coll, string name, Func<string> valueProvider)
        {
            try
            {
                coll[name] = valueProvider();
            }
            catch
            {
                // ignore
            }
        }

        /// <summary>
        /// Updates the public RSA key on the server.
        /// </summary>
        /// <param name="xml">The RSA key as XML data.</param>
        public void UpdateKey(string xml)
        {
            try
            {
                var enc = this.GetEncoding();

                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xml);

                var request = this.CreateWebRequest("update-key");
                request.Method = "PUT";

                var blob = enc.GetBytes(rsa.ToXmlString(includePrivateParameters: false));

                request.ContentType = "text/xml; charset=" + enc.WebName;
                request.ContentLength = blob.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(blob, 0, blob.Length);

                    stream.Flush();
                    stream.Close();
                }

                var json = this.GetJsonObject(request.GetResponse());
                switch (json.code)
                {
                    case 0:
                        // OK
                        break;

                    default:
                        throw new FileBoxException(result: json);
                }
            }
            catch (FileBoxException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FileBoxException(innerException: ex);
            }
        }

        #endregion Methods (16)
    }
}