// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Compression
{
    /// <summary>
    /// Describes an object that compresses data.
    /// </summary>
    public interface ICompressor : IDataTransformer
    {
        #region Methods (6)

        /// <summary>
        /// Compresses data.
        /// </summary>
        /// <param name="src">The source data.</param>
        /// <returns>The uncompressed data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        byte[] Compress(IEnumerable<byte> src);

        /// <summary>
        /// Compresses data.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <returns>The uncompressed data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        byte[] Compress(Stream src, int? bufferSize = null);

        /// <summary>
        /// Compresses data.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read and/or <paramref name="dest" /> cannot be written.
        /// </exception>
        void Compress(Stream src, Stream dest, int? bufferSize = null);

        /// <summary>
        /// Uncompresses data.
        /// </summary>
        /// <param name="src">The source data.</param>
        /// <returns>The uncompressed data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        byte[] Uncompress(IEnumerable<byte> src);

        /// <summary>
        /// Uncompresses data.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <returns>The uncompressed data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        byte[] Uncompress(Stream src, int? bufferSize = null);

        /// <summary>
        /// Uncompresses data.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read and/or <paramref name="dest" /> cannot be written.
        /// </exception>
        void Uncompress(Stream src, Stream dest, int? bufferSize = null);

        #endregion Methods (6)
    }
}