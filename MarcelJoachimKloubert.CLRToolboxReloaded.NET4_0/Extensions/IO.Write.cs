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
        /// Writes all data of a buffer to a stream.
        /// </summary>
        /// <param name="stream">The stream where to write the data to.</param>
        /// <param name="buffer">The buffer that contains the data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream" /> and/or <paramref name="buffer" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="stream" /> is not writable.
        /// </exception>
        public static void Write(this Stream stream, byte[] buffer)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (stream.CanWrite == false)
            {
                throw new IOException();
            }

            stream.Write(buffer, 0, buffer.Length);
        }

        #endregion Methods (1)
    }
}