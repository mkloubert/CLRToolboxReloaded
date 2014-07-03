// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Xml.Linq;
using LocationEnum = MarcelJoachimKloubert.FileBox.Location;

namespace MarcelJoachimKloubert.FileBox.Impl
{
    /// <summary>
    /// Stores the data of a file (item).
    /// </summary>
    internal sealed class FileBoxFile : ConnectionChildBase, IFile
    {
        #region Constructors (1)

        ~FileBoxFile()
        {
            using (var pwd = this.CryptedMetaXml)
            {
                this.CryptedMetaXml = null;
            }
        }

        #endregion Constructors (1)

        #region Properties (11)

        public DateTimeOffset CreationDate
        {
            get;
            internal set;
        }

        public byte[] CryptedMeta
        {
            get;
            internal set;
        }

        public SecureString CryptedMetaXml
        {
            get;
            internal set;
        }

        public Guid Id
        {
            get;
            internal set;
        }

        public bool IsCorrupted
        {
            get;
            internal set;
        }

        public DateTimeOffset LastWriteTime
        {
            get;
            internal set;
        }

        public LocationEnum Location
        {
            get;
            internal set;
        }

        public string Name
        {
            get;
            internal set;
        }

        public string RealName
        {
            get;
            internal set;
        }

        public DateTimeOffset SendTime
        {
            get;
            internal set;
        }

        public long Size
        {
            get;
            internal set;
        }

        #endregion Properties (11)

        #region Methods (2)

        public Stream Open()
        {
            this.ThrowIfCorrupted();

            string path;
            switch (this.Location)
            {
                case LocationEnum.Inbox:
                    path = "receive-file-inbox";
                    break;

                case LocationEnum.Outbox:
                    path = "receive-file-outbox";
                    break;

                default:
                    throw new NotSupportedException();
            }

            byte[] pwd;
            byte[] salt;
            var xml = XDocument.Parse(ToUnsecureString(this.CryptedMetaXml)).Root;
            try
            {
                pwd = Convert.FromBase64String(xml.Elements("password").Single().Value.Trim());
                salt = Convert.FromBase64String(xml.Elements("salt").Single().Value.Trim());

                var request = this.Server.CreateWebRequest(path);
                request.Method = "GET";

                request.Headers["X-FileBox-File"] = this.RealName;

                var response = request.GetResponse();

                return new CryptoStream(response.GetResponseStream(),
                                        CreateRijndael(pwd: pwd,
                                                       salt: salt).CreateDecryptor(),
                                        CryptoStreamMode.Read);
            }
            finally
            {
                pwd = null;
                salt = null;
                xml = null;
            }
        }

        private void ThrowIfCorrupted()
        {
            if (this.IsCorrupted)
            {
                throw new InvalidOperationException();
            }
        }

        #endregion Methods (2)
    }
}