// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// A crypter that simply copies data from source to destination (no encryption).
    /// </summary>
    public sealed class DummyCrypter : CrypterBase
    {
        #region Constructors (4)

        /// <inheriteddoc />
        public DummyCrypter(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public DummyCrypter(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public DummyCrypter(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public DummyCrypter()
            : base()
        {
        }

        #endregion Constructors (4)

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

        #region Methods (4)

        /// <inheriteddoc />
        protected override void OnDecrypt(Stream src, Stream dest, int? bufferSize)
        {
            this.OnEncrypt(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override void OnEncrypt(Stream src, Stream dest, int? bufferSize)
        {
            this.CopyData(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override void SaltString(StringBuilder str, Encoding enc)
        {
            // dummy
        }

        /// <inheriteddoc />
        protected override void UnsaltString(StringBuilder str, Encoding enc)
        {
            // dummy
        }

        #endregion Methods (4)
    }
}