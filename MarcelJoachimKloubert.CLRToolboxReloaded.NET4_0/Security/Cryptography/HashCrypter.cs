// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    #region CLASS: HashCrypter<TAlgo>

    /// <summary>
    /// A crypter that is based on a hash algorithm.
    /// </summary>
    public class HashCrypter<TAlgo> : CrypterBase
        where TAlgo : global::System.Security.Cryptography.HashAlgorithm, new()
    {
        #region Fields (1)

        /// <summary>
        /// Stores the function / method that provides the salt to use.
        /// </summary>
        protected readonly SaltProvider _SALT_PROVIDER;

        #endregion Fields (1)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="HashCrypter{TAlgo}" /> class.
        /// </summary>
        public HashCrypter()
            : this(provider: GetNoSalt)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashCrypter{TAlgo}" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the salt to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public HashCrypter(SaltProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._SALT_PROVIDER = provider;
        }

        #endregion Constructors (2)

        #region Properties (2)

        /// <inheriteddoc />
        public override bool CanDecrypt
        {
            get { return false; }
        }

        /// <inheriteddoc />
        public override bool CanEncrypt
        {
            get { return true; }
        }

        #endregion Properties (2)

        #region Delegates and events (1)

        /// <summary>
        /// The method / function that provides the salt to use.
        /// </summary>
        /// <param name="hasher">The underlying hasher.</param>
        /// <param name="salt">The stream where to write the salt data to.</param>
        public delegate void SaltProvider(HashCrypter<TAlgo> hasher, Stream salt);

        #endregion Delegates and events (1)

        #region Methods (5)

        private static void GetNoSalt(HashCrypter<TAlgo> algo, Stream salt)
        {
            // dummy
        }

        /// <summary>
        /// Returns the salt data from salt provider.
        /// </summary>
        /// <returns>The salt to use.</returns>
        protected virtual byte[] GetSalt()
        {
            using (var salt = new MemoryStream())
            {
                try
                {
                    this._SALT_PROVIDER(this, salt);

                    return salt.ToArray();
                }
                finally
                {
                    this.DestroyTempStream(salt);
                }
            }
        }

        /// <inheriteddoc />
        protected override void OnEncrypt(Stream src, Stream dest, int? bufferSize)
        {
            using (var hasher = new TAlgo())
            {
                hasher.ComputeHash(src, dest);
            }
        }

        /// <inheriteddoc />
        protected override void SaltString(StringBuilder str, Encoding enc)
        {
            var salt = this.GetSalt();
            try
            {
                if (salt.IsNotEmpty())
                {
                    str.Append(enc.GetString(salt));
                }
            }
            finally
            {
                this.DestroyTempByteArray(salt);
            }
        }

        /// <inheriteddoc />
        protected override void UnsaltString(StringBuilder str, Encoding enc)
        {
            // HASHER CANNOT UNSALT STRINGS!
        }

        #endregion Methods (5)
    }

    #endregion CLASS: HashCrypter<TAlgo>

    #region CLASS: HashCrypter

    /// <summary>
    /// Factory class for <see cref="HashCrypter{TAlgo}" />.
    /// </summary>
    public static class HashCrypter
    {
        #region Methods (4)

        /// <summary>
        /// Creates a new instance of the <see cref="HashCrypter{TAlgo}" /> class.
        /// </summary>
        /// <typeparam name="TAlgo">The algorithm to use.</typeparam>
        public static HashCrypter<TAlgo> Create<TAlgo>()
            where TAlgo : global::System.Security.Cryptography.HashAlgorithm, new()
        {
            return new HashCrypter<TAlgo>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HashCrypter{TAlgo}" /> class.
        /// </summary>
        /// <typeparam name="TAlgo">The algorithm to use.</typeparam>
        /// <param name="salt">The salt to use.</param>
        /// <returns>The new instance.</returns>
        public static HashCrypter<TAlgo> Create<TAlgo>(IEnumerable<byte> salt)
            where TAlgo : global::System.Security.Cryptography.HashAlgorithm, new()
        {
            return new HashCrypter<TAlgo>(provider: (h, s) =>
                {
                    if (salt == null)
                    {
                        return;
                    }

                    s.Write(salt.AsArray());
                });
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HashCrypter{TAlgo}" /> class.
        /// </summary>
        /// <typeparam name="TAlgo">The algorithm to use.</typeparam>
        /// <param name="saltStr">The UTF-8 encoded salt to use.</param>
        /// <returns>The new instance.</returns>
        public static HashCrypter<TAlgo> Create<TAlgo>(string saltStr)
            where TAlgo : global::System.Security.Cryptography.HashAlgorithm, new()
        {
            return Create<TAlgo>(saltStr: saltStr,
                                 enc: Encoding.UTF8);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HashCrypter{TAlgo}" /> class.
        /// </summary>
        /// <typeparam name="TAlgo">The algorithm to use.</typeparam>
        /// <param name="saltStr">The salt to use.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enc" /> is <see langword="null" />.
        /// </exception>
        public static HashCrypter<TAlgo> Create<TAlgo>(string saltStr, Encoding enc)
            where TAlgo : global::System.Security.Cryptography.HashAlgorithm, new()
        {
            if (enc == null)
            {
                throw new ArgumentNullException("enc");
            }

            return Create<TAlgo>(salt: enc.GetBytes(saltStr ?? string.Empty));
        }

        #endregion Methods (4)
    }

    #endregion CLASS: HashCrypter
}