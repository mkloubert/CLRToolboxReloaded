// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Compression
{
    /// <summary>
    /// A compressor that simply copies data from source to destionation (no compression).
    /// </summary>
    public sealed class DummyCompressor : CompressorBase
    {
        #region Constructors (4)

        /// <inheriteddoc />
        public DummyCompressor(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public DummyCompressor(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public DummyCompressor(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public DummyCompressor()
            : base()
        {
        }

        #endregion Constructors (4)

        #region Methods (2)

        /// <inheriteddoc />
        protected override void OnCompress(Stream src, Stream dest, int? bufferSize)
        {
            this.CopyData(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override void OnUncompress(Stream src, Stream dest, int? bufferSize)
        {
            this.OnCompress(src, dest, bufferSize);
        }

        #endregion Methods (2)
    }
}