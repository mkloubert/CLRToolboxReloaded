// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// A crypter based on XOR.
    /// </summary>
    public sealed class XorCrypter : CrypterBase
    {
        #region Fields (1)

        private readonly KeyProvider _KEY_PROVIDER;

        #endregion Fields (1)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="XorCrypter" /> class.
        /// </summary>
        /// <param name="provider">
        /// The function / delegate that provides the key to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public XorCrypter(KeyProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._KEY_PROVIDER = provider;
        }

        #endregion Constructors (1)

        #region Events and delegates (2)

        /// <summary>
        /// A function or method that provides the key to use.
        /// </summary>
        /// <param name="crypter">The underlying crypter instance.</param>
        /// <param name="key">The stream where to write the key to.</param>
        public delegate void KeyProvider(XorCrypter crypter, Stream key);

        #endregion Events and delegates (2)

        #region Properties (2)

        /// <inheriteddoc />
        public override bool CanDecrypt
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public override bool CanEncrypt
        {
            get { return true; }
        }

        #endregion Properties (2)

        #region Methods (7)

        /// <summary>
        /// Creates a new instance of the <see cref="XorCrypter" /> class
        /// and generates a random key.
        /// </summary>
        /// <param name="key">The variable where to write the generated key to.</param>
        /// <returns>The new instance.</returns>
        public static XorCrypter Create(out byte[] key)
        {
            key = Guid.NewGuid().ToByteArray();

            return Create(key);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XorCrypter" /> class.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <returns>The new instance.</returns>
        public static XorCrypter Create(Guid key)
        {
            return Create(key.ToByteArray());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XorCrypter" /> class.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <returns>The new instance.</returns>
        public static XorCrypter Create(IEnumerable<byte> key)
        {
            return new XorCrypter((c, k) =>
                {
                    if (key != null)
                    {
                        k.Write(key.AsArray());
                    }
                });
        }

        private void DeOrEncrypt(Stream src, Stream dest)
        {
            var key = this.GetKey();

            byte? lastByte;
            int index = -1;
            while ((lastByte = src.ReadSingleByte()).IsNotNull())
            {
                var byteToWrite = lastByte.Value;
                if (key.Length > 0)
                {
                    byteToWrite = (byte)(byteToWrite ^ key[++index % key.Length]);
                }

                dest.WriteByte(byteToWrite);
            }
        }

        private byte[] GetKey()
        {
            using (var key = new MemoryStream())
            {
                this._KEY_PROVIDER(this, key);

                return key.ToArray();
            }
        }

        /// <inheriteddoc />
        protected override void OnDecrypt(Stream src, Stream dest, int? bufferSize)
        {
            this.DeOrEncrypt(src, dest);
        }

        /// <inheriteddoc />
        protected override void OnEncrypt(Stream src, Stream dest, int? bufferSize)
        {
            this.DeOrEncrypt(src, dest);
        }

        #endregion Methods (7)
    }
}