// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// A crypter that uses multi <see cref="ICrypter" /> instances.
    /// </summary>
    public class AggregateCrypter : CrypterBase, IEnumerable<ICrypter>
    {
        #region Fields (2)

        private readonly CrypterProvider _CRYPTER_PROVIDER;
        private readonly AggregateDataTransformer _TRANSFORMER;

        #endregion Fields (2)

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
            this._TRANSFORMER = new AggregateDataTransformer(provider: this.GetTransformers);
        }

        #endregion Constructors (1)

        #region Events and delegates (1)

        /// <summary>
        /// A function or method that provides the crypters to use.
        /// </summary>
        /// <param name="crypter">The underlying crypter instance.</param>
        /// <returns>The crypters to use.</returns>
        public delegate IEnumerable<ICrypter> CrypterProvider(AggregateCrypter crypter);

        #endregion Events and delegates (1)

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

        #region Methods (8)

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
        /// Returns the crypters to use.
        /// </summary>
        /// <returns>The crypters to use.</returns>
        /// <remarks>
        /// The normal order is for encrypt operation.
        /// The reverse order is for decrypt operation.
        /// </remarks>
        public IEnumerable<ICrypter> GetCrypters()
        {
            return (this._CRYPTER_PROVIDER(this) ?? Enumerable.Empty<ICrypter>()).Where(c => c != null);
        }

        /// <inheriteddoc />
        public IEnumerator<ICrypter> GetEnumerator()
        {
            return this.GetCrypters()
                       .GetEnumerator();
        }

        /// <inheriteddoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<IDataTransformer> GetTransformers(AggregateDataTransformer transformer)
        {
            var result = this.GetCrypters();

#if MONO_PORTABLE
            return global::System.Linq.Enumerable.Cast<IDataTransformer>(result);
#else
            return result;
#endif
        }

        /// <inheriteddoc />
        protected override void OnDecrypt(Stream src, Stream dest, int? bufferSize)
        {
            this._TRANSFORMER
                .RestoreData(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override void OnEncrypt(Stream src, Stream dest, int? bufferSize)
        {
            this._TRANSFORMER
                .TransformData(src, dest, bufferSize);
        }

        #endregion Methods (8)
    }
}