// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// A basic object that encrypts and decrypts data.
    /// </summary>
    public abstract class CrypterBase : ObjectBase, ICrypter
    {
        #region Fields (2)

        private readonly Action<Stream, Stream, int?> _DECRYPT_ACTION;
        private readonly Action<Stream, Stream, int?> _ENCRYPT_ACTION;

        #endregion Fields (2)

        #region Constrcutors (4)

        /// <inheriteddoc />
        protected CrypterBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._DECRYPT_ACTION = this.OnDecrypt_ThreadSafe;
                this._ENCRYPT_ACTION = this.OnEncrypt_ThreadSafe;
            }
            else
            {
                this._DECRYPT_ACTION = this.OnDecrypt;
                this._ENCRYPT_ACTION = this.OnEncrypt;
            }
        }

        /// <inheriteddoc />
        protected CrypterBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected CrypterBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected CrypterBase()
            : this(isSynchronized: false)
        {
        }

        #endregion Constrcutors (4)

        #region Properties (2)

        /// <inheriteddoc />
        public abstract bool CanDecrypt
        {
            get;
        }

        /// <inheriteddoc />
        public abstract bool CanEncrypt
        {
            get;
        }

        #endregion Properties (2)

        #region Methods (12)

        /// <inheriteddoc />
        public byte[] Decrypt(IEnumerable<byte> src)
        {
            using (var dest = new MemoryStream())
            {
                this.Decrypt(src, dest);

                return dest.ToArray();
            }
        }

        /// <inheriteddoc />
        public byte[] Decrypt(Stream src, int? bufferSize = null)
        {
            using (var dest = new MemoryStream())
            {
                this.Decrypt(src, dest);

                return dest.ToArray();
            }
        }

        /// <inheriteddoc />
        public void Decrypt(IEnumerable<byte> src, Stream dest)
        {
            using (var srcStream = new MemoryStream(src.AsArray(), false))
            {
                this.Decrypt(srcStream, dest);
            }
        }

        /// <inheriteddoc />
        public void Decrypt(Stream src, Stream dest, int? bufferSize = null)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            if (src.CanRead == false)
            {
                throw new IOException("src");
            }

            if (dest.CanWrite == false)
            {
                throw new IOException("dest");
            }

            if (this.CanDecrypt == false)
            {
                throw new InvalidOperationException();
            }

            if (bufferSize < 1)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            this._DECRYPT_ACTION(src, dest,
                                 bufferSize);
        }

        /// <inheriteddoc />
        public byte[] Encrypt(IEnumerable<byte> src)
        {
            using (var dest = new MemoryStream())
            {
                this.Encrypt(src, dest);

                return dest.ToArray();
            }
        }

        /// <inheriteddoc />
        public byte[] Encrypt(Stream src, int? bufferSize = null)
        {
            using (var dest = new MemoryStream())
            {
                this.Encrypt(src, dest);

                return dest.ToArray();
            }
        }

        /// <inheriteddoc />
        public void Encrypt(IEnumerable<byte> src, Stream dest)
        {
            using (var srcStream = new MemoryStream(src.AsArray(), false))
            {
                this.Encrypt(srcStream, dest);
            }
        }

        /// <inheriteddoc />
        public void Encrypt(Stream src, Stream dest, int? bufferSize = null)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            if (src.CanRead == false)
            {
                throw new IOException("src");
            }

            if (dest.CanWrite == false)
            {
                throw new IOException("dest");
            }

            if (this.CanEncrypt == false)
            {
                throw new InvalidOperationException();
            }

            if (bufferSize < 1)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            this._ENCRYPT_ACTION(src, dest,
                                 bufferSize);
        }

        /// <summary>
        /// Stores the logic for the <see cref="CrypterBase.Decrypt(Stream, Stream, int?)" /> method.
        /// </summary>
        /// <param name="src">The stream with the crypted data.</param>
        /// <param name="dest">The stream where to write the decrypted data to.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        protected virtual void OnDecrypt(Stream src, Stream dest, int? bufferSize)
        {
            throw new NotImplementedException();
        }

        private void OnDecrypt_ThreadSafe(Stream src, Stream dest, int? bufferSize)
        {
            lock (this._SYNC)
            {
                this.OnDecrypt(src, dest, bufferSize);
            }
        }

        /// <summary>
        /// Stores the logic for the <see cref="CrypterBase.Encrypt(Stream, Stream, int?)" /> method.
        /// </summary>
        /// <param name="src">The stream with the uncrypted data.</param>
        /// <param name="dest">The stream where to write the encrypted data to.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        protected virtual void OnEncrypt(Stream src, Stream dest, int? bufferSize)
        {
            throw new NotImplementedException();
        }

        private void OnEncrypt_ThreadSafe(Stream src, Stream dest, int? bufferSize)
        {
            lock (this._SYNC)
            {
                this.OnEncrypt(src, dest, bufferSize);
            }
        }

        #endregion Methods (12)
    }
}