// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Tries to read a single byte from a stream.
        /// </summary>
        /// <param name="stream">The stream from where to read the data from.</param>
        /// <returns>The read data or <see langword="null" /> if no data is available.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="stream" /> is not readable.
        /// </exception>
        public static byte? ReadSingleByte(this Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (stream.CanRead == false)
            {
                throw new IOException();
            }

            var result = stream.ReadByte();
            return result >= 0 ? (byte)result : (byte?)null;
        }

        #endregion Methods (1)
    }
}