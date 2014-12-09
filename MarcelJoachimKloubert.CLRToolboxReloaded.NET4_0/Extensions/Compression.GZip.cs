// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (5)

        /// <summary>
        /// Compresses binary data via GZIP.
        /// </summary>
        /// <param name="data">The data to compress.</param>
        /// <returns>The compressed data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <see langword="null" />.
        /// </exception>
        public static byte[] GZip(this IEnumerable<byte> data)
        {
            // (null) check is done by constructor of MemoryStream class

            using (var src = new MemoryStream(AsArray(data), false))
            {
                return GZip(src);
            }
        }

        /// <summary>
        /// Compresses the data of a source stream via GZIP.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="bufferSize">The buffer size for read operation to use.</param>
        /// <returns>The compressed data.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        public static byte[] GZip(this Stream src, int? bufferSize = null)
        {
            using (var dest = new MemoryStream())
            {
                GZipInner(src, dest, bufferSize);

                return dest.ToArray();
            }
        }

        /// <summary>
        /// Compresses binary data to a destination stream via GZIP.
        /// </summary>
        /// <param name="data">The data to compress.</param>
        /// <param name="dest">The destination stream.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        public static void GZip(this IEnumerable<byte> data, Stream dest)
        {
            // (null) check is done by constructor of MemoryStream class

            using (var src = new MemoryStream(AsArray(data), false))
            {
                GZip(src, dest);
            }
        }

        /// <summary>
        /// Compresses the data of a source stream to a destination stream via GZIP.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">The buffer size for read operation to use.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="src" /> cannot be read and/or <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        public static void GZip(this Stream src, Stream dest, int? bufferSize = null)
        {
            GZipInner(src, dest, bufferSize);
        }

        private static void GZipInner(Stream src, Stream dest, int? bufferSize)
        {
            using (var gzip = new GZipStream(dest, CompressionMode.Compress, true))
            {
                if (bufferSize.HasValue)
                {
                    src.CopyTo(gzip, bufferSize.Value);
                }
                else
                {
                    src.CopyTo(gzip);
                }

                gzip.Flush();
            }
        }

        #endregion Methods
    }
}