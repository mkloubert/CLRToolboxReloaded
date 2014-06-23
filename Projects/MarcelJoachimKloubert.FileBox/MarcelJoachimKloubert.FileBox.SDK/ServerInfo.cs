// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Security.Cryptography;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Stores information about the current server.
    /// </summary>
    public sealed class ServerInfo : ServerObjectBase
    {
        #region Properties (3)

        /// <summary>
        /// Gets if the server has stored the public key for the underlying user or not.
        /// </summary>
        public bool HasKey
        {
            get { return this.Key != null; }
        }

        /// <summary>
        /// Gets the public key of the connected user if available.
        /// </summary>
        public string Key
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

        #endregion Properties (3)

        #region Methods (1)

        /// <summary>
        /// Tries to return a RSA crypter based on the value of <see cref="ServerInfo.Key" />.
        /// </summary>
        /// <returns>The RSA crypter or <see langword="null" /> if no valid public key is available.</returns>
        public RSACryptoServiceProvider TryGetRsaCrypter()
        {
            RSACryptoServiceProvider result = null;

            try
            {
                if (this.HasKey)
                {
                    result = new RSACryptoServiceProvider();
                    result.FromXmlString(this.Key);
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

        #endregion Methods (1)
    }
}