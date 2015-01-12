// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40 || PORTABLE45)
#define KNOWS_ARRAY_LONGLENGTH
#define KNOWS_SECURE_STRING
#endif

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// A basic object that encrypts and decrypts data.
    /// </summary>
    public abstract class CrypterBase : DataTransformerBase, ICrypter
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected CrypterBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected CrypterBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected CrypterBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected CrypterBase()
            : base()
        {
        }

        #endregion Constrcutors (4)

        #region Properties (4)

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

        /// <inheriteddoc />
        public override sealed bool CanRestoreData
        {
            get { return this.CanDecrypt; }
        }

        /// <inheriteddoc />
        public override sealed bool CanTransformData
        {
            get { return this.CanEncrypt; }
        }

        #endregion Properties (4)

        #region Methods (31)

        /// <inheriteddoc />
        protected override void DestroyTempByteArray(byte[] array)
        {
            if (array == null)
            {
                return;
            }

#if KNOWS_ARRAY_LONGLENGTH
            for (long i = 0; i < array.LongLength; i++)
#else
            for (int i = 0; i < array.Length; i++)
#endif
            {
                array[i] = 0;
            }
        }

        /// <inheriteddoc />
        protected override void DestroyTempStream(Stream stream)
        {
            if (stream == null)
            {
                return;
            }

            if (stream.CanWrite == false)
            {
                return;
            }

            var finalFlush = true;
            try
            {
                try
                {
                    stream.Shredder(count: 3,
                                    restorePos: false,
                                    fromBeginning: true,
                                    flushAfterWrite: false);
                }
                finally
                {
                    // flush stream ...
                    {
                        stream.Flush();
                        finalFlush = false;
                    }

                    // ... before make empty
                    {
                        // ... before make empty

                        stream.SetLength(0);
                        finalFlush = true;
                    }
                }
            }
            finally
            {
                if (finalFlush)
                {
                    // now flush all data
                    stream.Flush();
                }
            }
        }

        /// <inheriteddoc />
        protected override void DestroyTempStringBuilder(StringBuilder builder)
        {
            if (builder == null)
            {
                return;
            }

            try
            {
                for (var i = 0; i < builder.Length; i++)
                {
                    builder[i] = '\0';
                }
            }
            finally
            {
                builder.Clear();
            }
        }

        /// <inheriteddoc />
        public byte[] Decrypt(IEnumerable<byte> src)
        {
            return this.RestoreData(src);
        }

        /// <inheriteddoc />
        public byte[] Decrypt(Stream src, int? bufferSize = null)
        {
            return this.RestoreData(src, bufferSize);
        }

        /// <inheriteddoc />
        public void Decrypt(IEnumerable<byte> src, Stream dest)
        {
            this.RestoreData(src, dest);
        }

        /// <inheriteddoc />
        public void Decrypt(Stream src, Stream dest, int? bufferSize = null)
        {
            this.RestoreData(src, dest, bufferSize);
        }

#if KNOWS_SECURE_STRING

        /// <inheriteddoc />
        public global::System.Security.SecureString DecryptSecureString(global::System.IO.Stream src, int? bufferSize = null)
        {
            return this.RestoreSecureString(src, bufferSize);
        }

        /// <inheriteddoc />
        public global::System.Security.SecureString DecryptSecureString(global::System.IO.Stream src, global::System.Text.Encoding enc, int? bufferSize = null)
        {
            return this.RestoreSecureString(src, enc, bufferSize);
        }

        /// <inheriteddoc />
        public global::System.Security.SecureString DecryptSecureString(global::System.Collections.Generic.IEnumerable<byte> src)
        {
            return this.RestoreSecureString(src);
        }

        /// <inheriteddoc />
        public global::System.Security.SecureString DecryptSecureString(global::System.Collections.Generic.IEnumerable<byte> src, global::System.Text.Encoding enc)
        {
            return this.RestoreSecureString(src, enc);
        }

#endif

        /// <inheriteddoc />
        public void DecryptString(Stream src, StringBuilder builder, int? bufferSize = null)
        {
            this.RestoreString(src, builder, bufferSize);
        }

        /// <inheriteddoc />
        public void DecryptString(Stream src, StringBuilder builder, Encoding enc, int? bufferSize = null)
        {
            this.RestoreString(src, builder, enc, bufferSize);
        }

        /// <inheriteddoc />
        public string DecryptString(Stream src, int? bufferSize = null)
        {
            return this.RestoreString(src, bufferSize);
        }

        /// <inheriteddoc />
        public string DecryptString(Stream src, Encoding enc, int? bufferSize = null)
        {
            return this.RestoreString(src, enc, bufferSize);
        }

        /// <inheriteddoc />
        public void DecryptString(IEnumerable<byte> src, StringBuilder builder)
        {
            this.RestoreString(src, builder);
        }

        /// <inheriteddoc />
        public void DecryptString(IEnumerable<byte> src, StringBuilder builder, Encoding enc)
        {
            this.RestoreString(src, builder, enc);
        }

        /// <inheriteddoc />
        public string DecryptString(IEnumerable<byte> src)
        {
            return this.RestoreString(src);
        }

        /// <inheriteddoc />
        public string DecryptString(IEnumerable<byte> src, Encoding enc)
        {
            return this.RestoreString(src, enc);
        }

        /// <inheriteddoc />
        public byte[] Encrypt(IEnumerable<byte> src)
        {
            return this.TransformData(src);
        }

        /// <inheriteddoc />
        public byte[] Encrypt(Stream src, int? bufferSize = null)
        {
            return this.TransformData(src, bufferSize);
        }

        /// <inheriteddoc />
        public void Encrypt(IEnumerable<byte> src, Stream dest)
        {
            this.TransformData(src, dest);
        }

        /// <inheriteddoc />
        public void Encrypt(Stream src, Stream dest, int? bufferSize = null)
        {
            this.TransformData(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        public void EncryptString(string str, Stream dest)
        {
            this.TransformString(str, dest);
        }

        /// <inheriteddoc />
        public void EncryptString(string str, Stream dest, Encoding enc)
        {
            this.TransformString(str, enc, dest);
        }

        /// <inheriteddoc />
        public byte[] EncryptString(string str)
        {
            return this.TransformString(str);
        }

        /// <inheriteddoc />
        public byte[] EncryptString(string str, Encoding enc)
        {
            return this.TransformString(str, enc);
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

        /// <inheriteddoc />
        protected override sealed void OnRestoreData(Stream src, Stream dest, int? bufferSize)
        {
            this.OnDecrypt(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override sealed void OnTransformData(Stream src, Stream dest, int? bufferSize)
        {
            this.OnEncrypt(src, dest, bufferSize);
        }

        #endregion Methods (31)
    }
}