// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (4)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptSecureString(Stream, int?)" />
        public static SecureString DecryptSecureString(this Stream src, int? bufferSize = null)
        {
            return GlobalCrypter.Current.DecryptSecureString(src, bufferSize);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptSecureString(Stream, Encoding, int?)" />
        public static SecureString DecryptSecureString(this Stream src, Encoding enc, int? bufferSize = null)
        {
            return GlobalCrypter.Current.DecryptSecureString(src, enc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptSecureString(IEnumerable{byte})" />
        public static SecureString DecryptSecureString(this IEnumerable<byte> src)
        {
            return GlobalCrypter.Current.DecryptSecureString(src);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptSecureString(IEnumerable{byte}, Encoding)" />
        public static SecureString DecryptSecureString(this IEnumerable<byte> src, Encoding enc)
        {
            return GlobalCrypter.Current.DecryptSecureString(src, enc);
        }

        #endregion Methods (4)
    }
}