// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// A crypter that uses multi <see cref="ICrypter" /> instances.
    /// </summary>
    public class AggregateCrypter : CrypterBase
    {
        #region Fields (1)

        private readonly CrypterProvider _CRYPTER_PROVIDER;

        #endregion Fields (1)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateCrypter" /> class.
        /// </summary>
        /// <param name="provider">
        /// The function / delegate that provides crypters to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AggregateCrypter(CrypterProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._CRYPTER_PROVIDER = provider;
        }

        #endregion Constructors (1)

        #region Events and delegates (2)

        /// <summary>
        /// A function or method that provides the crypters to use.
        /// </summary>
        /// <param name="crypter">The underlying crypter instance.</param>
        public delegate IEnumerable<ICrypter> CrypterProvider(AggregateCrypter crypter);

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

        #region Methods (9)

        /// <summary>
        /// Closes an old temporary stream that was used for encrypt / decrypt operations.
        /// </summary>
        /// <param name="stream">The stream to close.</param>
        protected virtual void CloseTempStream(Stream stream)
        {
            stream.Dispose();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AggregateCrypter" /> class.
        /// </summary>
        /// <param name="crypters">The crypters to use.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="crypters" /> is <see langword="null" />.
        /// </exception>
        public static AggregateCrypter Create(params ICrypter[] crypters)
        {
            return Create((IEnumerable<ICrypter>)crypters);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AggregateCrypter" /> class.
        /// </summary>
        /// <param name="crypters">The crypters to use.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="crypters" /> is <see langword="null" />.
        /// </exception>
        public static AggregateCrypter Create(IEnumerable<ICrypter> crypters)
        {
            if (crypters == null)
            {
                throw new ArgumentNullException("crypters");
            }

            return new AggregateCrypter((c) => crypters);
        }

        /// <summary>
        /// Creates a temporary stream for encrypt / decrypt operations.
        /// </summary>
        /// <returns>The created stream.</returns>
        protected virtual Stream CreateTempStream()
        {
            return new MemoryStream();
        }

        private void DeOrEncrypt(Stream src, Stream dest, int? bufferSize,
                                 IEnumerable<ICrypter> crypters,
                                 Action<ICrypter, Stream, Stream, int?> crypterAction)
        {
            Stream currentDest = null;

            using (var e = crypters.GetEnumerator())
            {
                var index = -1L;
                Stream currentSrc = null;

                int? currentBufSize = null;
                while (e.MoveNext())
                {
                    ++index;

                    if (index == 0)
                    {
                        // first operation

                        currentSrc = src;
                        currentBufSize = bufferSize;
                    }
                    else
                    {
                        // last destionation is new source
                        currentSrc = currentDest;
                        currentSrc.Position = 0;

                        currentBufSize = this.GetBufferSizeForTempStream(currentSrc);
                    }

                    var currentCrypter = e.Current;

                    currentDest = this.CreateTempStream();
                    try
                    {
                        crypterAction(currentCrypter,
                                      currentSrc, currentDest, currentBufSize);
                    }
                    catch
                    {
                        // close before rethrow exception
                        this.CloseTempStream(currentDest);

                        throw;
                    }
                    finally
                    {
                        if (index > 0)
                        {
                            // at that point it is a temp stream
                            this.CloseTempStream(currentSrc);
                        }
                    }
                }
            }

            if (currentDest != null)
            {
                currentDest.Position = 0;
                currentDest.CopyTo(dest);
            }
        }

        /// <summary>
        /// Returns the buffer size in bytes that should be used to read a temp
        /// stream that is used as source.
        /// </summary>
        /// <param name="stream">The temporary stream.</param>
        /// <returns>
        /// The buffer size.
        /// <see langword="null" /> indicates to use the default value.
        /// </returns>
        protected virtual int? GetBufferSizeForTempStream(Stream stream)
        {
            return null;
        }

        /// <summary>
        /// Returns the crypters to use.
        /// </summary>
        /// <returns>The crypters to use.</returns>
        /// <remarks>
        /// The normal order is for enrypt operation.
        /// The reverse order is for decrypt operation.
        /// </remarks>
        public IEnumerable<ICrypter> GetCrypters()
        {
            return (this._CRYPTER_PROVIDER(this) ?? Enumerable.Empty<ICrypter>()).Where(c => c != null);
        }

        /// <inheriteddoc />
        protected override void OnDecrypt(Stream src, Stream dest, int? bufferSize)
        {
            this.DeOrEncrypt(src, dest, bufferSize,
                             this.GetCrypters().Reverse(),
                             (c, s, d, bs) => c.Decrypt(s, d, bs));
        }

        /// <inheriteddoc />
        protected override void OnEncrypt(Stream src, Stream dest, int? bufferSize)
        {
            this.DeOrEncrypt(src, dest, bufferSize,
                             this.GetCrypters(),
                             (c, s, d, bs) => c.Encrypt(s, d, bs));
        }

        #endregion Methods (9)
    }
}