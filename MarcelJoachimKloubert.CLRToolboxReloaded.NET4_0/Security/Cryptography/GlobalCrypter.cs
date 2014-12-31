// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// Access to a global <see cref="ICrypter" /> object.
    /// </summary>
    public static partial class GlobalCrypter
    {
        #region Fields (1)

        private static CrypterProvider _provider;

        #endregion Fields

        #region Properties (1)

        /// <summary>
        /// Gets the global crypter instance.
        /// </summary>
        public static ICrypter Current
        {
            get { return _provider(); }
        }

        #endregion Properties

        #region Delegates and Events (1)

        /// <summary>
        /// Describes the logic that returns the global crypter.
        /// </summary>
        /// <returns>The provided <see cref="ICrypter" /> object.</returns>
        public delegate ICrypter CrypterProvider();

        #endregion Delegates and Events

        #region Methods (2)

        /// <summary>
        /// Sets the value for <see cref="GlobalCrypter.Current" />.
        /// </summary>
        /// <param name="newCrypter">The new converter.</param>
        public static void SetConverter(ICrypter newCrypter)
        {
            SetConverterProvider(newCrypter != null ? new CrypterProvider(() => newCrypter) : null);
        }

        /// <summary>
		/// Sets the logic that returns the value for <see cref="GlobalCrypter.Current" />.
        /// </summary>
        /// <param name="newProvider">The new provider delegate.</param>
        public static void SetConverterProvider(CrypterProvider newProvider)
        {
            _provider = newProvider;
        }

        #endregion Methods
    }
}