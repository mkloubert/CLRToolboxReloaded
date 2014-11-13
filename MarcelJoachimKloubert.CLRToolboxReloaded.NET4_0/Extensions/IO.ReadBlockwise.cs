// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Reads a <see cref="Stream" /> blockwise.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="blockSize">The maximum size of a block.</param>
        /// <returns>The read blocks.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="blockSize" /> is invalid.</exception>
        /// <exception cref="IOException"><paramref name="stream" /> cannot be read.</exception>
        public static IEnumerable<byte[]> ReadBlockwise(this Stream stream, int blockSize = 81920)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (stream.CanRead == false)
            {
                throw new IOException();
            }

            if (blockSize < 1)
            {
                throw new ArgumentOutOfRangeException("blockSize");
            }

            var buffer = new byte[blockSize];

            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                var result = buffer;
                if (bytesRead != buffer.Length)
                {
                    result = buffer.Take(bytesRead).ToArray();
                }

                yield return result;
            }
        }

        #endregion Methods (1)
    }
}