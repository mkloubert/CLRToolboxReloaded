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

        private List<FileItem> GetBox(Location loc, RSACryptoServiceProvider rsa, int startAt, int? maxItems)
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

            List<FileItem> result;

            try
            {
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

                var request = this.CreateWebRequest(path);
                request.Method = "GET";

                request.Headers["X-FileBox-StartAt"] = startAt.ToString();

                if (maxItems.HasValue)
                {
                    request.Headers["X-FileBox-MaxItems"] = maxItems.ToString();
                }

                var jsonResult = this.GetJsonObject(request.GetResponse());
                result = new List<FileItem>();

                switch (jsonResult.code)
                {
                    case 0:
                        if (jsonResult.data != null)
                        {
                            var files = jsonResult.data.files;
                            foreach (dynamic item in files)
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

                                        byte[] metaPwdAndSalt = rsa.Decrypt(Convert.FromBase64String(meta.sec.Trim()),
                                                                            false);

                                        using (var cryptedMetaStream = new MemoryStream(buffer: Convert.FromBase64String(meta.dat.Trim()),
                                                                                        writable: false))
                                        {
                                            using (var decryptedMetaStream = new MemoryStream())
                                            {
                                                var cryptoMetaStream = new CryptoStream(cryptedMetaStream,
                                                                                        CreateRijndael(pwd: metaPwdAndSalt.Take(48).ToArray(),
                                                                                                       salt: metaPwdAndSalt.Skip(48).ToArray()).CreateDecryptor(),
                                                                                        CryptoStreamMode.Read);

                                                cryptoMetaStream.CopyTo(decryptedMetaStream);

                                                decryptedMetaStream.Position = 0;
                                                var xml = XDocument.Load(decryptedMetaStream).Root;
                                                try
                                                {
                                                    newItem.Id = Guid.Parse(xml.Attribute("id").Value.Trim());
                                                    newItem.Name = xml.Attribute("name").Value.Trim();
                                                    newItem.Size = long.Parse(xml.Elements("size").Single().Value.Trim(),
                                                                              CultureInfo.InvariantCulture);

                                                    newItem.CreationDate = DateTimeOffset.ParseExact(xml.Elements("creationTime").Single().Value.Trim(),
                                                                                                     "u",
                                                                                                     CultureInfo.InvariantCulture).ToLocalTime();
                                                    newItem.LastWriteTime = DateTimeOffset.ParseExact(xml.Elements("lastWriteTime").Single().Value.Trim(),
                                                                                                      "u",
                                                                                                      CultureInfo.InvariantCulture).ToLocalTime();
                                                    newItem.SendTime = DateTimeOffset.ParseExact(xml.Elements("date").Single().Value.Trim(),
                                                                                                 "u",
                                                                                                 CultureInfo.InvariantCulture).ToLocalTime();

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
                                    result.Add(newItem);
                                }
                            }
                        }
                        break;

                    default:
                        throw new FileBoxException(result: jsonResult);
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

                if (string.IsNullOrEmpty(user))
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
        /// <remarks>UTF-8 encoding is used.</remarks>
        public JsonResult GetJsonObject(WebResponse response, bool closeResponse = true)
        {
            return this.GetJsonObject(response: response,
                                      enc: this.GetEncoding(),
                                      closeResponse: closeResponse);
        }

        /// <summary>
        /// Reads a JSON object from a response.
        /// </summary>
        /// <param name="response">The HTTP response context.</param>
        /// <param name="closeResponse">Close <paramref name="response" /> after reading or not.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <returns>The JSON object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="response" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        public JsonResult GetJsonObject(WebResponse response, Encoding enc, bool closeResponse = true)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (enc == null)
            {
                throw new ArgumentNullException("enc");
            }

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
        public List<FileItem> GetInbox(RSACryptoServiceProvider rsa, int startAt = 0, int? maxItems = null)
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
        public List<FileItem> GetOutbox(RSACryptoServiceProvider rsa, int startAt = 0, int? maxItems = null)
        {
            return this.GetBox(loc: Location.Outbox,
                               rsa: rsa,
                               startAt: startAt, maxItems: maxItems);
        }

        /// <summary>
        /// Gets information about the server.
        /// </summary>
        /// <returns>The server information.</returns>
        public ServerInfo GetServerInfo()
        {
            ServerInfo result = null;

            var request = this.CreateWebRequest("server-info");
            request.Method = "GET";

            var response = (HttpWebResponse)request.GetResponse();

            var jsonResult = this.GetJsonObject(response);
            if (jsonResult != null)
            {
                var info = jsonResult.data;

                result = new ServerInfo();
                result.Server = this;

                result.Name = info.name;

                result.Key = (info.user.key ?? string.Empty).Trim();
                if (result.Key == string.Empty)
                {
                    result.Key = null;
                }
            }

            return result;
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="recipients" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// File in <paramref name="path" /> does not exist.
        /// </exception>
        public void Send(string path, params string[] recipients)
        {
            this.Send(path, (IEnumerable<string>)recipients);
        }

        /// <summary>
        /// Send a file.
        /// </summary>
        /// <param name="path">The path of the file to send.</param>
        /// <param name="recipients">The list of recipients.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="recipients" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// File in <paramref name="path" /> does not exist.
        /// </exception>
        public void Send(string path, IEnumerable<string> recipients)
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

            try
            {
                var request = this.CreateWebRequest("send-file");
                request.Method = "PUT";

                request.Headers["X-FileBox-Filename"] = file.Name.Trim();
                request.Headers["X-FileBox-To"] = string.Join(";", recipients.Where(r => string.IsNullOrWhiteSpace(r) == false)
                                                                             .Select(r => r.ToLower().Trim())
                                                                             .Distinct());
                request.ContentType = "application/octet-stream";
                request.ContentLength = file.Length;

                using (var stream = file.OpenRead())
                {
                    using (var reqStream = request.GetRequestStream())
                    {
                        stream.CopyTo(reqStream);

                        reqStream.Flush();
                        reqStream.Close();
                    }
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

        /// <summary>
        /// Updates the public RSA key on the server.
        /// </summary>
        /// <param name="xml">The RSA key as XML data.</param>
        /// <remarks>UTF-8 encoding is used.</remarks>
        public void UpdateKey(string xml)
        {
            this.UpdateKey(xml, this.GetEncoding());
        }

        /// <summary>
        /// Updates the public RSA key on the server.
        /// </summary>
        /// <param name="xml">The RSA key as XML data.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enc" /> is <see langword="null" />.
        /// </exception>
        public void UpdateKey(string xml, Encoding enc)
        {
            if (enc == null)
            {
                throw new ArgumentNullException("enc");
            }

            try
            {
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