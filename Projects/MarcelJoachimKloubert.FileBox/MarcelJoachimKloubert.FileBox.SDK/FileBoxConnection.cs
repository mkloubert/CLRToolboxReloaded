// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.FileBox.IO;
using MarcelJoachimKloubert.FileBox.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Handles a FileBox (server) connection.
    /// </summary>
    public sealed class FileBoxConnection
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBoxConnection" /> class.
        /// </summary>
        public FileBoxConnection()
        {
            this.IsSecure = true;
        }

        #endregion Constructors (1)

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

        #region Methods (13)

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
        /// Gets information about the server.
        /// </summary>
        /// <returns>The server information.</returns>
        public ServerInfo GetServerInfo()
        {
            ServerInfo result = null;

            var request = this.CreateWebRequest("info");
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

        private string GetBaseUrl()
        {
            return string.Format("http{0}://{1}:{2}/",
                                 this.IsSecure ? "s" : string.Empty,
                                 string.IsNullOrWhiteSpace(this.Host) ? "localhost" : this.Host.Trim(),
                                 this.Port);
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
        /// Reads a JSON object from a response.
        /// </summary>
        /// <param name="response">The HTTP response context.</param>
        /// <returns>The JSON object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="response" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>UTF-8 encoding is used.</remarks>
        public JsonResult GetJsonObject(HttpWebResponse response)
        {
            return this.GetJsonObject(response, this.GetEncoding());
        }

        /// <summary>
        /// Reads a JSON object from a response.
        /// </summary>
        /// <param name="response">The HTTP response context.</param>
        /// <returns>The JSON object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="response" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        public JsonResult GetJsonObject(HttpWebResponse response, Encoding enc)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (enc == null)
            {
                throw new ArgumentNullException("enc");
            }

            dynamic result = null;

            using (var respStream = response.GetResponseStream())
            {
                using (var txtReader = new StreamReader(respStream, enc))
                {
                    using (var reader = new JsonTextReader(txtReader))
                    {
                        var serializer = new JsonSerializer();
                        result = serializer.Deserialize<ExpandoObject>(reader);
                    }
                }
            }

            return result != null ? new JsonResult()
            {
                code = result.code != null ? Convert.ToInt32(result.code) : null,
                data = result.data,
                msg = Convert.ChangeType(result.msg, typeof(string)),
            } : null;
        }

        /// <summary>
        /// Returns the files from the INBOX folder.
        /// </summary>
        /// <param name="startAt"></param>
        /// <param name="maxItems"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startAt" /> and/or <paramref name="maxItems" /> are invalid.
        /// </exception>
        public List<FileItem> GetInbox(int startAt = 0, int? maxItems = null)
        {
            if (startAt < 0)
            {
                throw new ArgumentOutOfRangeException("startAt");
            }

            if (maxItems < 0)
            {
                throw new ArgumentOutOfRangeException("maxItems");
            }

            var result = new List<FileItem>();

            var request = this.CreateWebRequest("inbox");
            request.Method = "GET";

            request.Headers["X-FileBox-StartAt"] = startAt.ToString();

            if (maxItems.HasValue)
            {
                request.Headers["X-FileBox-MaxItems"] = maxItems.ToString();
            }

            var response = (HttpWebResponse)request.GetResponse();

            var jsonResult = this.GetJsonObject(response);
            if (jsonResult != null &&
                jsonResult.data != null)
            {
                foreach (dynamic item in jsonResult.data)
                {
                    var newItem = new FileItem();
                    newItem.Name = item.name;
                    newItem.Server = this;
                    newItem.Size = Convert.ToInt64(item.size);

                    result.Add(newItem);
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
            SecureString newValue = null;
            if (pwd != null)
            {
                newValue = new SecureString();
                foreach (var c in pwd)
                {
                    newValue.AppendChar(c);
                }
            }

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

            var request = this.CreateWebRequest("send");
            request.Method = "POST";

            request.Headers["X-FileBox-Filename"] = file.Name.Trim();
            request.Headers["X-FileBox-To"] = string.Join(";", recipients.Where(r => string.IsNullOrWhiteSpace(r) == false)
                                                                         .Select(r => r.ToLower().Trim())
                                                                         .Distinct());
            request.ContentType = "application/octet-stream";

            using (var stream = file.OpenRead())
            {
                using (var reqStream = request.GetRequestStream())
                {
                    stream.CopyTo(reqStream);

                    reqStream.Flush();
                    reqStream.Close();
                }
            }

            var response = request.GetResponse();
            response.Close();
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

            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xml);

            var request = this.CreateWebRequest("updatekey");
            request.Method = "POST";

            request.ContentType = "text/xml; charset=" + enc.WebName;
            using (var stream = request.GetRequestStream())
            {
                var blob = enc.GetBytes(rsa.ToXmlString(includePrivateParameters: false));

                stream.Write(blob, 0, blob.Length);

                stream.Flush();
                stream.Close();
            }

            var response = request.GetResponse();
            response.Close();
        }

        #endregion Methods (13)
    }
}