// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Security.Cryptography;
using System.Collections.Generic;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (4)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.Decrypt(Stream, int?)" />
        public static byte[] Decrypt(this Stream src, int? bufferSize = null)
        {
            return GlobalCrypter.Current.Decrypt(src);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.Decrypt(Stream, Stream, int?)" />
        public static void Decrypt(this Stream src, Stream dest, int? bufferSize = null)
        {
            GlobalCrypter.Current.Decrypt(src, dest, bufferSize);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.Decrypt(IEnumerable{byte})" />
        public static byte[] Decrypt(this IEnumerable<byte> src)
        {
            return GlobalCrypter.Current.Decrypt(src);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.Decrypt(IEnumerable{byte}, Stream)" />
        public static void Decrypt(this IEnumerable<byte> src, Stream dest)
        {
            GlobalCrypter.Current.Decrypt(src, dest);
        }

        #endregion Methods (4)
    }
}