// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;
using System.IO.Compression;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Compression
{
    /// <summary>
    /// A compressor that uses GZIP to compress / uncompress data.
    /// </summary>
    public sealed class GZipCompressor : CompressorBase
    {
        #region Constructors (4)

        /// <inheriteddoc />
        public GZipCompressor(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public GZipCompressor(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public GZipCompressor(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public GZipCompressor()
            : base()
        {
        }

        #endregion Constructors (4)

        #region Methods (2)

        /// <inheriteddoc />
        protected override void OnCompress(Stream src, Stream dest, int? bufferSize)
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

        /// <inheriteddoc />
        protected override void OnUncompress(Stream src, Stream dest, int? bufferSize)
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

        #endregion Methods (2)
    }
}