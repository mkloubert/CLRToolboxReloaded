// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Xml.Linq;
using LocationEnum = MarcelJoachimKloubert.FileBox.Location;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Stores the data of a file (item).
    /// </summary>
    public sealed class FileItem : ServerObjectBase
    {
        #region Constructors (1)

        /// <summary>
        ///
        /// </summary>
        ~FileItem()
        {
            using (var pwd = this.CryptedMetaXml)
            {
                this.CryptedMetaXml = null;
            }
        }

        #endregion Constructors (1)

        #region Properties (11)

        /// <summary>
        /// Gets the creation date.
        /// </summary>
        public DateTimeOffset CreationDate
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the crypted meta data of the item.
        /// </summary>
        public byte[] CryptedMeta
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the crypted meta data of the item as XML string.
        /// </summary>
        public SecureString CryptedMetaXml
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the ID of the file.
        /// </summary>
        public Guid Id
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets if the data of that data is corrupted or not.
        /// </summary>
        public bool IsCorrupted
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the last write time.
        /// </summary>
        public DateTimeOffset LastWriteTime
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the location of the file.
        /// </summary>
        public LocationEnum Location
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the (machine) name of the server.
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the (internal) name of that item on the server.
        /// </summary>
        public string RealName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets time the file was send.
        /// </summary>
        public DateTimeOffset SendTime
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        public long Size
        {
            get;
            internal set;
        }

        #endregion Properties (11)

        #region Methods (1)

        /// <summary>
        /// Opens that file for reading.
        /// </summary>
        /// <returns>The stream for reading the file.</returns>
        /// <exception cref="InvalidOperationException">
        /// File item contains corrupted data.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The operation is (currently) not supported.
        /// </exception>
        public Stream Open()
        {
            if (this.IsCorrupted)
            {
                throw new InvalidOperationException();
            }

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

        #endregion Methods (1)
    }
}