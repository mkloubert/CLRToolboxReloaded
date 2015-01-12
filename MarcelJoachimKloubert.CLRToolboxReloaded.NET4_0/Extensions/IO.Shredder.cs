// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Shredders a stream.
        /// </summary>
        /// <param name="stream">The stream to shredder.</param>
        /// <param name="count">The number of write operations.</param>
        /// <param name="blockSize">The block size in bytes.</param>
        /// <param name="restorePos">Restore position after shredder operation.</param>
        /// <param name="fromBeginning">
        /// Start from the beginning of the stream (<see langword="true" />)
        /// or from the current position (<see langword="false" />).
        /// </param>
        /// <param name="flushAfterWrite">Call <see cref="Stream.Flush()" /> method after each write operation.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="stream" /> is NOT seakable and/or writable.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count" /> and/or <paramref name="blockSize" /> are invalid.
        /// </exception>
        public static void Shredder(this Stream stream,
                                    int count = 1,
                                    int blockSize = 8192,
                                    bool restorePos = true,
                                    bool fromBeginning = true,
                                    bool flushAfterWrite = true)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (stream.CanSeek == false)
            {
                throw new IOException("stream.CanSeek");
            }

            if (stream.CanWrite == false)
            {
                throw new IOException("stream.CanWrite");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (blockSize < 1)
            {
                throw new ArgumentOutOfRangeException("blockSize");
            }

            long? lastKnownPos = null;
            if (restorePos)
            {
                lastKnownPos = stream.Position;
            }

            try
            {
                var startPos = fromBeginning ? stream.Seek(0, SeekOrigin.Begin)
                                             : (lastKnownPos ?? stream.Position);

                var len = stream.Length;

                for (var i = 0; i < count; i++)
                {
                    stream.Position = startPos;

                    byte byteToWrite = 0;
                    switch (i % 3)
                    {
                        case 0:
                            byteToWrite = 255;
                            break;

                        case 2:
                            byteToWrite = 151;
                            break;
                    }

                    var block = Enumerable.Repeat(byteToWrite, blockSize)
                                          .ToArray();

                    var blockCount = (long)Math.Floor((double)len / (double)block.Length);
                    for (long ii = 0; ii < blockCount; ii++)
                    {
                        Write(stream, block);

                        if (flushAfterWrite)
                        {
                            stream.Flush();
                        }
                    }

                    var lastBlockSize = (int)(len % (long)blockSize);
                    if (lastBlockSize > 0)
                    {
                        stream.Write(block, 0, lastBlockSize);

                        if (flushAfterWrite)
                        {
                            stream.Flush();
                        }
                    }
                }
            }
            finally
            {
                if (lastKnownPos.HasValue)
                {
                    stream.Position = lastKnownPos.Value;
                }
            }
        }

        #endregion Methods (1)
    }
}