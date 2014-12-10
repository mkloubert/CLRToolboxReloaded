// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// Crypter that is based on delegates.
    /// </summary>
    public sealed partial class DelegateCrypter : CrypterBase
    {
        #region Fields (4)

        private readonly bool _CAN_DECRYPT = true;
        private readonly bool _CAN_ENCRYPT = true;
        private readonly CrypterAction _DECRYPT_ACTION;
        private readonly CrypterAction _ENCRYPT_ACTION;

        #endregion Fields (4)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCrypter" /> class.
        /// </summary>
        /// <param name="cryptAction">The encrypt AND decrypt action.</param>
        /// <param name="canEncrypt">The value for the <see cref="DelegateCrypter.CanEncrypt" /> property.</param>
        /// <param name="canDecrypt">The value for the <see cref="DelegateCrypter.CanDecrypt" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cryptAction" /> is <see langword="null" />.
        /// </exception>
        public DelegateCrypter(CrypterAction cryptAction,
                               bool canEncrypt = true, bool canDecrypt = true)
            : this(encryptAction: cryptAction,
                   decryptAction: cryptAction)
        {
            this._CAN_DECRYPT = canDecrypt;
            this._CAN_ENCRYPT = canEncrypt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCrypter" /> class.
        /// </summary>
        /// <param name="encryptAction">The encryption action.</param>
        /// <param name="decryptAction">The decryption action.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="encryptAction" /> and/or <paramref name="decryptAction" /> are <see langword="null" />.
        /// </exception>
        public DelegateCrypter(CrypterAction encryptAction,
                               CrypterAction decryptAction)
        {
            if (encryptAction == null)
            {
                throw new ArgumentNullException("encryptAction");
            }

            if (decryptAction == null)
            {
                throw new ArgumentNullException("decryptAction");
            }

            this._DECRYPT_ACTION = decryptAction;
            this._ENCRYPT_ACTION = encryptAction;
        }

        #endregion Constructors (2)

        #region Events and delegates (1)

        /// <summary>
        /// Describes a crypter action.
        /// </summary>
        /// <param name="crypter">The underlying crypter instance.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        public delegate void CrypterAction(DelegateCrypter crypter, CryptMode mode,
                                           Stream src, Stream dest, int? bufferSize);

        #endregion Events and delegates (1)

        #region Properties (2)

        /// <inheriteddoc />
        public override bool CanDecrypt
        {
            get { return this._CAN_DECRYPT; }
        }

        /// <inheriteddoc />
        public override bool CanEncrypt
        {
            get { return this._CAN_ENCRYPT; }
        }

        #endregion Properties (2)

        #region Methods (2)

        /// <inheriteddoc />
        protected override void OnDecrypt(Stream src, Stream dest, int? bufferSize)
        {
            this._DECRYPT_ACTION(this, CryptMode.Decrypt,
                                 src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override void OnEncrypt(Stream src, Stream dest, int? bufferSize)
        {
            this._ENCRYPT_ACTION(this, CryptMode.Encrypt,
                                 src, dest, bufferSize);
        }

        #endregion Methods (2)
    }
}