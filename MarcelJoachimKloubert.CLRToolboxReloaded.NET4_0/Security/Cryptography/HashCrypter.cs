// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// A crypter that is based on a hash algorithm.
    /// </summary>
    public sealed class HashCrypter<TAlgo> : CrypterBase
        where TAlgo : global::System.Security.Cryptography.HashAlgorithm, new()
    {
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

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnEncrypt(Stream src, Stream dest, int? bufferSize)
        {
            using (var hasher = new TAlgo())
            {
                hasher.ComputeHash(src, dest);
            }
        }

        #endregion Methods (1)
    }
}