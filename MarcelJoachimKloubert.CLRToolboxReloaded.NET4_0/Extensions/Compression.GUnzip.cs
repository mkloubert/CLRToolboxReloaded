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
        /// UNcompresses binary data via GZIP.
        /// </summary>
        /// <param name="data">The data to decompress.</param>
        /// <returns>The decompressed data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <see langword="null" />.
        /// </exception>
        public static byte[] GUnzip(this IEnumerable<byte> data)
        {
            // (null) check is done by constructor of MemoryStream class

            using (var src = new MemoryStream(AsArray(data), false))
            {
                return GUnzip(src);
            }
        }

        /// <summary>
        /// UNcompresses the data of a source stream via GZIP.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="bufferSize">The buffer size for read operation to use.</param>
        /// <returns>The decompressed data.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        public static byte[] GUnzip(this Stream src, int? bufferSize = null)
        {
            using (var dest = new MemoryStream())
            {
                GUnzipInner(src, dest, bufferSize);

                return dest.ToArray();
            }
        }

        /// <summary>
        /// UNcompresses binary data to a destination stream via GZIP.
        /// </summary>
        /// <param name="data">The data to decompress.</param>
        /// <param name="dest">The destination stream.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        public static void GUnzip(this IEnumerable<byte> data, Stream dest)
        {
            // (null) check is done by constructor of MemoryStream class

            using (var src = new MemoryStream(AsArray(data), false))
            {
                GUnzip(src, dest);
            }
        }

        /// <summary>
        /// UNcompresses the data of a source stream to a destination stream via GZIP.
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
        public static void GUnzip(this Stream src, Stream dest, int? bufferSize = null)
        {
            GUnzipInner(src, dest, bufferSize);
        }

        private static void GUnzipInner(Stream src, Stream dest, int? bufferSize)
        {
            using (var gunzip = new GZipStream(src, CompressionMode.Decompress, true))
            {
                if (bufferSize.HasValue)
                {
                    gunzip.CopyTo(dest, bufferSize.Value);
                }
                else
                {
                    gunzip.CopyTo(dest);
                }

                gunzip.Flush();
            }
        }

        #endregion Methods
    }
}