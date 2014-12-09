// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// A crypter based on <see cref="Rijndael" />.
    /// </summary>
    public class AesCrypter : CrypterBase
    {
        #region Fields (1)

        /// <summary>
        /// Stores the delegate that sets up the crypter.
        /// </summary>
        protected readonly CrypterSetup _CRYPTER_SETUP;

        #endregion Fields (1)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="AesCrypter" /> class.
        /// </summary>
        /// <param name="setup">
        /// The function / delegate that sets up the decryption and encryption processes.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="setup" /> is <see langword="null" />.
        /// </exception>
        public AesCrypter(CrypterSetup setup)
        {
            if (setup == null)
            {
                throw new ArgumentNullException("setup");
            }

            this._CRYPTER_SETUP = setup;
        }

        #endregion Constructors (1)

        #region Delegates and Events (1)

        /// <summary>
        /// A function or method that setups all data for encryption/decryption process.
        /// </summary>
        /// <param name="crypter">The underlying crypter instance.</param>
        /// <param name="pwd">The stream where to write the password to.</param>
        /// <param name="salt">The stream where to write the salt to.</param>
        /// <param name="iterations">The variable where to write the custom iteration value to.</param>
        /// <param name="keySize">The variable where to write the custom size of the key to.</param>
        /// <param name="ivSize">The variable where to write the custom size of the IV to.</param>
        public delegate void CrypterSetup(AesCrypter crypter,
                                          Stream pwd, Stream salt,
                                          ref int? iterations,
                                          ref int? keySize, ref int? ivSize);

        #endregion Delegates and Events (1)

        #region Properties (5)

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

        /// <summary>
        /// Gets the default iteration value.
        /// </summary>
        protected virtual int DefaultIterations
        {
            get { return 1000; }
        }

        /// <summary>
        /// Gets the default iteration value.
        /// </summary>
        protected virtual int DefaultIVSize
        {
            get { return 16; }
        }

        /// <summary>
        /// Gets the default iteration value.
        /// </summary>
        protected virtual int DefaultKeySize
        {
            get { return 32; }
        }

        #endregion Properties (5)

        #region Methods (11)

        /// <summary>
        /// Creates a new instance of the <see cref="AesCrypter" /> class
        /// and generates a random password and salt.
        /// </summary>
        /// <param name="salt">The variable where to write the generated salt to.</param>
        /// <param name="pwd">The variable where to write the generated password to.</param>
        /// <param name="iterations">The custom iterations.</param>
        /// <param name="keySize">The custom key size.</param>
        /// <param name="ivSize">The custom IV size.</param>
        /// <returns>The created instance.</returns>
        public static AesCrypter Create(out byte[] pwd, out byte[] salt,
                                        int? iterations = null,
                                        int? keySize = null, int? ivSize = null)
        {
            salt = Guid.NewGuid().ToByteArray();

            return Create(salt, out pwd,
                          iterations,
                          keySize, ivSize);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AesCrypter" /> class
        /// and generates a random password.
        /// </summary>
        /// <param name="salt">The salt to use.</param>
        /// <param name="pwd">The variable where to write the generated password to.</param>
        /// <param name="iterations">The custom iterations.</param>
        /// <param name="keySize">The custom key size.</param>
        /// <param name="ivSize">The custom IV size.</param>
        /// <returns>The created instance.</returns>
        public static AesCrypter Create(Guid salt, out byte[] pwd,
                                        int? iterations = null,
                                        int? keySize = null, int? ivSize = null)
        {
            return Create(salt.ToByteArray(), out pwd,
                          iterations,
                          keySize, ivSize);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AesCrypter" /> class
        /// and generates a random password.
        /// </summary>
        /// <param name="salt">The optional salt to use.</param>
        /// <param name="pwd">The variable where to write the generated password to.</param>
        /// <param name="iterations">The custom iterations.</param>
        /// <param name="keySize">The custom key size.</param>
        /// <param name="ivSize">The custom IV size.</param>
        /// <returns>The created instance.</returns>
        public static AesCrypter Create(IEnumerable<byte> salt, out byte[] pwd,
                                        int? iterations = null,
                                        int? keySize = null, int? ivSize = null)
        {
            pwd = Guid.NewGuid().ToByteArray();

            return Create(pwd, salt,
                          iterations,
                          keySize, ivSize);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AesCrypter" /> class.
        /// </summary>
        /// <param name="pwd">The password to use.</param>
        /// <param name="salt">The salt to use.</param>
        /// <param name="iterations">The custom iterations.</param>
        /// <param name="keySize">The custom key size.</param>
        /// <param name="ivSize">The custom IV size.</param>
        /// <returns>The created instance.</returns>
        public static AesCrypter Create(IEnumerable<byte> pwd, Guid salt,
                                        int? iterations = null,
                                        int? keySize = null, int? ivSize = null)
        {
            return Create(pwd, salt.ToByteArray(),
                          iterations,
                          keySize, ivSize);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AesCrypter" /> class.
        /// </summary>
        /// <param name="pwd">The password to use.</param>
        /// <param name="salt">The optional salt to use.</param>
        /// <param name="iterations">The custom iterations.</param>
        /// <param name="keySize">The custom key size.</param>
        /// <param name="ivSize">The custom IV size.</param>
        /// <returns>The created instance.</returns>
        public static AesCrypter Create(Guid pwd, IEnumerable<byte> salt = null,
                                        int? iterations = null,
                                        int? keySize = null, int? ivSize = null)
        {
            return Create(pwd.ToByteArray(), salt,
                          iterations,
                          keySize, ivSize);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AesCrypter" /> class.
        /// </summary>
        /// <param name="pwd">The password to use.</param>
        /// <param name="salt">The optional salt to use.</param>
        /// <param name="iterations">The custom iterations.</param>
        /// <param name="keySize">The custom key size.</param>
        /// <param name="ivSize">The custom IV size.</param>
        /// <returns>The created instance.</returns>
        public static AesCrypter Create(IEnumerable<byte> pwd, IEnumerable<byte> salt = null,
                                        int? iterations = null,
                                        int? keySize = null, int? ivSize = null)
        {
            return new AesCrypter(setup: delegate(AesCrypter crypter, 
                                                  Stream pwdStream, Stream saltStream,
                                                  ref int? it,
                                                  ref int? ks, ref int? ivs)
                {
                    if (pwd != null)
                    {
                        pwdStream.Write(pwd.AsArray());
                    }

                    if (salt != null)
                    {
                        saltStream.Write(salt.AsArray());
                    }

                    it = iterations;
                    ks = keySize;
                    ivs = ivSize;
                });
        }

        /// <summary>
        /// Creates a new algorithm instance.
        /// </summary>
        /// <returns>The created instance.</returns>
        protected virtual Rijndael CreateAlgorithm()
        {
            return Rijndael.Create();
        }

        /// <summary>
        /// Creates a new crypto stream.
        /// </summary>
        /// <param name="stream">The underlying base stream.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>The created instance.</returns>
        protected virtual CryptoStream CreateCryptoStream(Stream stream, CryptoStreamMode mode)
        {
            int? ivSize = null;
            int? keySize = null;
            DeriveBytes db;
            using (var pwd = new MemoryStream())
            {
                using (var salt = new MemoryStream())
                {
                    int? interations = null;
                    this._CRYPTER_SETUP(this,
                                        pwd, salt,
                                        ref interations,
                                        ref keySize, ref ivSize);

                    db = this.CreateDeriveBytes(pwd.ToArray(),
                                                salt.ToArray(),
                                                interations ?? this.DefaultIterations);
                }
            }

            var alg = this.CreateAlgorithm();
            alg.Key = db.GetBytes(keySize ?? this.DefaultKeySize);
            alg.IV = db.GetBytes(ivSize ?? this.DefaultIVSize);

            ICryptoTransform transform;
            switch (mode)
            {
                case CryptoStreamMode.Read:
                    transform = alg.CreateDecryptor();
                    break;

                case CryptoStreamMode.Write:
                    transform = alg.CreateEncryptor();
                    break;

                default:
                    throw new NotImplementedException();
            }

            return new CryptoStream(stream,
                                    transform,
                                    mode);
        }

        /// <summary>
        /// Returns the new <see cref="DeriveBytes" /> instance that is used to get key and IV for algorithm instance.
        /// </summary>
        /// <param name="pwd">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns>The created instance.</returns>
        protected virtual DeriveBytes CreateDeriveBytes(byte[] pwd, byte[] salt, int iterations)
        {
            return new Rfc2898DeriveBytes(pwd, salt,
                                          iterations);
        }

        /// <inheriteddoc />
        protected override void OnDecrypt(Stream src, Stream dest, int? bufferSize)
        {
            var cs = this.CreateCryptoStream(src, CryptoStreamMode.Read);

            if (bufferSize.HasValue)
            {
                cs.CopyTo(dest, bufferSize.Value);
            }
            else
            {
                cs.CopyTo(dest);
            }
        }

        /// <inheriteddoc />
        protected override void OnEncrypt(Stream src, Stream dest, int? bufferSize)
        {
            var cs = this.CreateCryptoStream(dest, CryptoStreamMode.Write);

            if (bufferSize.HasValue)
            {
                src.CopyTo(cs, bufferSize.Value);
            }
            else
            {
                src.CopyTo(cs);
            }

            cs.FlushFinalBlock();
        }

        #endregion Methods (11)
    }
}