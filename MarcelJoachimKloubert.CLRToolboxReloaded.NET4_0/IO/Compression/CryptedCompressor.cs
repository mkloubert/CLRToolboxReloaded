// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Security.Cryptography;
using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Compression
{
    /// <summary>
    ///
    /// </summary>
    /// <see cref="CompressionCrypter" />
    public sealed class CryptedCompressor : CompressorBase
    {
        #region Fields (2)

        private readonly CompressorProvider _COMPRESSOR_PROVIDER;
        private readonly CrypterProvider _CRYPTER_PROVIDER;

        #endregion Fields (2)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptedCompressor" /> class.
        /// </summary>
        /// <param name="compressorProvider">The method / function that provides the compressor to use.</param>
        /// <param name="crypterProvider">The method / function that provides the crypter to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="compressorProvider" /> and/or <paramref name="crypterProvider" /> are <see langword="null" />.
        /// </exception>
        public CryptedCompressor(CompressorProvider compressorProvider, CrypterProvider crypterProvider)
        {
            if (compressorProvider == null)
            {
                throw new ArgumentNullException("compressorProvider");
            }

            if (crypterProvider == null)
            {
                throw new ArgumentNullException("crypterProvider");
            }

            this._COMPRESSOR_PROVIDER = compressorProvider;
            this._CRYPTER_PROVIDER = crypterProvider;
        }

        #endregion Constructors (1)

        #region Delegates and events (2)

        /// <summary>
        /// Describes the function / method that provides the compressor to use.
        /// </summary>
        /// <param name="compressor">The underlying compressor instance.</param>
        /// <returns>The compressor to use.</returns>
        public delegate ICompressor CompressorProvider(CryptedCompressor compressor);

        /// <summary>
        /// Describes the function / method that provides the crypter to use.
        /// </summary>
        /// <param name="compressor">The underlying compressor instance.</param>
        /// <returns>The crypter to use.</returns>
        public delegate ICrypter CrypterProvider(CryptedCompressor compressor);

        #endregion Delegates and events (2)

        #region Methods (4)

        /// <summary>
        /// Creates a new instance of the <see cref="CryptedCompressor" /> class.
        /// </summary>
        /// <param name="compressor">The compressor to use.</param>
        /// <param name="crypter">The crypter to use.</param>
        /// <returns>The new created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="compressor" /> and/or <paramref name="crypter" /> are <see langword="null" />.
        /// </exception>
        public static CryptedCompressor Create(ICompressor compressor, ICrypter crypter)
        {
            if (compressor == null)
            {
                throw new ArgumentNullException("compressor");
            }

            if (crypter == null)
            {
                throw new ArgumentNullException("crypter");
            }

            return new CryptedCompressor(compressorProvider: (c) => compressor,
                                         crypterProvider: (c) => crypter);
        }

        private void GetCrypterAndCompressor(out ICompressor compressor, out ICrypter crypter)
        {
            crypter = this._CRYPTER_PROVIDER(this) ?? new DummyCrypter();
            compressor = this._COMPRESSOR_PROVIDER(this) ?? new DummyCompressor();
        }

        /// <inheriteddoc />
        protected override void OnCompress(Stream src, Stream dest, int? bufferSize)
        {
            ICompressor compressor;
            ICrypter crypter;
            this.GetCrypterAndCompressor(out compressor, out crypter);

            using (var temp = new MemoryStream())
            {
                try
                {
                    compressor.Compress(src, temp, bufferSize);

                    temp.Position = 0;
                    crypter.Encrypt(temp, dest);
                }
                finally
                {
                    this.DestroyTempStream(temp);
                }
            }
        }

        /// <inheriteddoc />
        protected override void OnUncompress(Stream src, Stream dest, int? bufferSize)
        {
            ICompressor compressor;
            ICrypter crypter;
            this.GetCrypterAndCompressor(out compressor, out crypter);

            using (var temp = new MemoryStream())
            {
                try
                {
                    crypter.Decrypt(src, temp, bufferSize);

                    temp.Position = 0;
                    compressor.Uncompress(temp, dest);
                }
                finally
                {
                    this.DestroyTempStream(temp);
                }
            }
        }

        #endregion Methods (4)
    }
}