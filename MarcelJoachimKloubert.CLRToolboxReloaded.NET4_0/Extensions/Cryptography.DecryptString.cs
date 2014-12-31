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
        /// <see cref="ICrypter.DecryptString(Stream, int?)" />
        public static string DecryptString(this Stream src, int? bufferSize = null)
        {
            return GlobalCrypter.Current.DecryptString(src);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptString(Stream, Encoding, int?)" />
        public static string DecryptString(this Stream src, Encoding enc, int? bufferSize = null)
        {
            return GlobalCrypter.Current.DecryptString(src, enc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptString(Stream, StringBuilder, int?)" />
        public static void DecryptString(this Stream src, StringBuilder builder, int? bufferSize = null)
        {
            GlobalCrypter.Current.DecryptString(src, builder);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptString(Stream, StringBuilder, Encoding, int?)" />
        public static void DecryptString(this Stream src, StringBuilder builder, Encoding enc, int? bufferSize = null)
        {
            GlobalCrypter.Current.DecryptString(src, builder, enc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptString(IEnumerable{byte})" />
        public static string DecryptString(this IEnumerable<byte> src)
        {
            return GlobalCrypter.Current.DecryptString(src);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptString(IEnumerable{byte}, Encoding)" />
        public static string DecryptString(this IEnumerable<byte> src, Encoding enc)
        {
            return GlobalCrypter.Current.DecryptString(src, enc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptString(IEnumerable{byte}, StringBuilder)" />
        public static void DecryptString(this IEnumerable<byte> src, StringBuilder builder)
        {
            GlobalCrypter.Current.DecryptString(src, builder);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ICrypter.DecryptString(IEnumerable{byte}, StringBuilder, Encoding)" />
        public static void DecryptString(this IEnumerable<byte> src, StringBuilder builder, Encoding enc)
        {
            GlobalCrypter.Current.DecryptString(src, builder, enc);
        }

        #endregion Methods (8)
    }
}