// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Compression
{
    /// <summary>
    /// A compressor that uses delegates for operations.
    /// </summary>
    public sealed partial class DelegateCompressor : CompressorBase
    {
        #region Fields (2)

        private readonly CompressorAction _COMPRESS_ACTION;
        private readonly CompressorAction _UNCOMPRESS_ACTION;

        #endregion Fields (2)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCompressor" /> class.
        /// </summary>
        /// <param name="compressAction">The compress action.</param>
        /// <param name="uncompressAction">The uncompress action.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="compressAction" /> and/or <paramref name="uncompressAction" /> are <see langword="null" />.
        /// </exception>
        public DelegateCompressor(CompressorAction compressAction,
                                  CompressorAction uncompressAction)
        {
            if (compressAction == null)
            {
                throw new ArgumentNullException("compressAction");
            }

            if (uncompressAction == null)
            {
                throw new ArgumentNullException("uncompressAction");
            }

            this._COMPRESS_ACTION = compressAction;
            this._UNCOMPRESS_ACTION = uncompressAction;
        }

        #endregion Constructors (1)

        #region Events and delegates (1)

        /// <summary>
        /// Describes a compressor action.
        /// </summary>
        /// <param name="compressor">The underlying compressor instance.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer size in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        public delegate void CompressorAction(DelegateCompressor compressor, CompressionMode mode,
                                              Stream src, Stream dest, int? bufferSize);

        #endregion Events and delegates (1)

        #region Methods (2)

        /// <inheriteddoc />
        protected override void OnCompress(Stream src, Stream dest, int? bufferSize)
        {
            this._COMPRESS_ACTION(this, CompressionMode.Compress,
                                  src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override void OnUncompress(Stream src, Stream dest, int? bufferSize)
        {
            this._UNCOMPRESS_ACTION(this, CompressionMode.Uncompress,
                                    src, dest, bufferSize);
        }

        #endregion Methods (2)
    }
}