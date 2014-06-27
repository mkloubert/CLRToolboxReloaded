// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.FileBox.Server.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox.Server.Security
{
    internal sealed class ServerPrincipal : ObjectBase, IServerPrincipal
    {
        #region Properties (10)

        public bool CanWriteMessages
        {
            get { return this.TryGetRsaCrypter() != null; }
        }

        public string Files
        {
            get;
            internal set;
        }

        internal FileBoxHost Host
        {
            get;
            set;
        }

        public IServerIdentity Identity
        {
            get;
            internal set;
        }

        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; }
        }

        public string Inbox
        {
            get { return Path.GetFullPath(Path.Combine(this.Files, "i")); }
        }

        internal Func<string, bool> IsInRolePredicate
        {
            get;
            set;
        }

        public string Outbox
        {
            get { return Path.GetFullPath(Path.Combine(this.Files, "o")); }
        }

        public string Messages
        {
            get { return Path.GetFullPath(Path.Combine(this.Files, "m")); }
        }

        public string Temp
        {
            get { return Path.GetFullPath(Path.Combine(this.Files, "t")); }
        }

        #endregion Properties (10)

        #region Methods (4)

        internal static ServerPrincipal FromUsername(FileBoxHost host, string username)
        {
            var dir = new DirectoryInfo(Path.Combine(host.UserFileDirectory,
                                                     username.ToLower().Trim()));

            Guid id;
            using (var md5 = new MD5CryptoServiceProvider())
            {
                id = new Guid(md5.ComputeHash(new UTF8Encoding().GetBytes(username)));
            }

            return new ServerPrincipal()
                {
                    Files = Path.GetFullPath(dir.FullName),

                    Host = host,

                    Identity = new ServerIdentity(id: id)
                        {
                            AuthenticationType = "HttpBasicAuth",
                            IsAuthenticated = true,
                            Name = username.ToLower().Trim(),
                        },

                    IsInRolePredicate = (role) => false,
                };
        }

        public bool IsInRole(string role)
        {
            return this.IsInRolePredicate(role);
        }

        public RSACryptoServiceProvider TryGetRsaCrypter()
        {
            RSACryptoServiceProvider result = null;

            try
            {
                var keyFile = new FileInfo(Path.Combine(this.Files, "key.xml"));
                if (keyFile.Exists)
                {
                    result = new RSACryptoServiceProvider();
                    result.FromXmlString(File.ReadAllText(keyFile.FullName,
                                                          Encoding.UTF8));
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public void WriteMessage(string subject, string content)
        {
            var rsa = this.TryGetRsaCrypter();
            if (rsa == null)
            {
                throw new ArgumentNullException("rsa");
            }

            var rand = new CryptoRandom();

            var msgDir = new DirectoryInfo(this.Messages);
            if (msgDir.Exists == false)
            {
                msgDir.Create();
                msgDir.Refresh();
            }

            FileInfo dataFile;
            FileInfo metaFile;
            FileInfo dataPwdFile;
            FileHelper.CreateUniqueFilesForCryptedData(dir: msgDir,
                                                       dataFile: out dataFile,
                                                       metaFile: out metaFile,
                                                       metaPwdFile: out dataPwdFile,
                                                       createMetaFile: false);

            var msgId = Guid.NewGuid();
            var msgDate = AppTime.Now;

            var msg = new XElement("message");
            try
            {
                // meta data
                {
                    var metaPwdAndSalt = new byte[123];
                    rand.NextBytes(metaPwdAndSalt);

                    msg.SetAttributeValue("id", msgId.ToString("N"));

                    msg.Add(new XElement("date", msgDate.ToString("u")));

                    if (subject != null)
                    {
                        msg.Add(new XElement("subject", subject));
                    }

                    // content
                    {
                        var contentElement = new XElement("content");
                        contentElement.SetAttributeValue("type", "text/x-markdown");

                        if (content != null)
                        {
                            contentElement.Value = content;
                        }

                        msg.Add(new XElement("content", contentElement));
                    }

                    // write password of data file
                    using (var metaPwdStream = new FileStream(path: dataPwdFile.FullName,
                                                              mode: FileMode.Create,
                                                              access: FileAccess.ReadWrite))
                    {
                        var cryptedMetaPwd = rsa.Encrypt(metaPwdAndSalt, false);

                        metaPwdStream.Write(cryptedMetaPwd, 0, cryptedMetaPwd.Length);
                    }

                    // write crypted message data
                    using (var msgStream = new FileStream(path: dataFile.FullName,
                                                          mode: FileMode.Create,
                                                          access: FileAccess.ReadWrite))
                    {
                        var cryptStream = new CryptoStream(msgStream,
                                                           CryptoHelper.CreateRijndael(metaPwdAndSalt.Skip(6).Take(48).ToArray(),
                                                                                       metaPwdAndSalt.Skip(6 + 48).Take(16).ToArray())
                                                                       .CreateEncryptor(),
                                                           CryptoStreamMode.Write);

                        msg.Save(cryptStream);

                        cryptStream.Flush();
                        cryptStream.Close();
                    }
                }
            }
            catch
            {
                this.Host
                    .TryDeleteFile(dataFile);
                this.Host
                    .TryDeleteFile(dataPwdFile);

                throw;
            }
        }

        #endregion Methods (4)
    }
}