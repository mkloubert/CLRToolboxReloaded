// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Reads all data from a stream to the beginning of a buffer.
        /// </summary>
        /// <param name="stream">The stream where to read the data from.</param>
        /// <param name="buffer">The buffer that should contain the data.</param>
        /// <returns>The number of read data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream" /> and/or <paramref name="buffer" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="stream" /> is not readable.
        /// </exception>
        /// <remarks>That operation is done in one read operation.</remarks>
        public static int Read(this Stream stream, byte[] buffer)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (stream.CanRead == false)
            {
                throw new IOException();
            }

            return stream.Read(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Tries to read a number of bytes from a stream.
        /// </summary>
        /// <param name="stream">The stream from where to read the data from.</param>
        /// <param name="count">The (maximum) number of data to read.</param>
        /// <returns>The read data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count" /> is smaller than 0.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="stream" /> is not readable.
        /// </exception>
        public static byte[] Read(this Stream stream, int count)
        {
            // do not check 'stream' for (null), this is done
            // by Read(this Stream, byte[]) method

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            var buffer = new byte[count];
            var bytesRead = Read(stream, buffer);

            return bytesRead == buffer.Length ? buffer
                                              : buffer.Take(bytesRead).ToArray();
        }

        #endregion Methods (2)
    }
}