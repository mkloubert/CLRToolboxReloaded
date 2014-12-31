// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (8)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.Encrypt(Stream, int?)" />
        public static byte[] Encrypt(this Stream src, int? bufferSize = null)
        {
            return GlobalCrypter.Current.Encrypt(src);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.Encrypt(Stream, Stream, int?)" />
        public static void Encrypt(this Stream src, Stream dest, int? bufferSize = null)
        {
            GlobalCrypter.Current.Encrypt(src, dest, bufferSize);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.Encrypt(IEnumerable{byte})" />
        public static byte[] Encrypt(this IEnumerable<byte> src)
        {
            return GlobalCrypter.Current.Encrypt(src);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.Encrypt(IEnumerable{byte}, Stream)" />
        public static void Encrypt(this IEnumerable<byte> src, Stream dest)
        {
            GlobalCrypter.Current.Encrypt(src, dest);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.EncryptString(string)" />
        public static byte[] Encrypt(this string str)
        {
            return GlobalCrypter.Current.EncryptString(str);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.EncryptString(string, Encoding)" />
        public static byte[] Encrypt(this string str, Encoding enc)
        {
            return GlobalCrypter.Current.EncryptString(str, enc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.EncryptString(string, Stream)" />
        public static void Encrypt(this string str, Stream dest)
        {
            GlobalCrypter.Current.EncryptString(str, dest);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.EncryptString(string, Stream, Encoding)" />
        public static void Encrypt(this string str, Stream dest, Encoding enc)
        {
            GlobalCrypter.Current.EncryptString(str, dest, enc);
        }

        #endregion Methods (8)
    }
}