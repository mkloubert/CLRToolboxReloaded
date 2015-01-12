// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Compression
{
    /// <summary>
    /// A compressor that does nothing.
    /// </summary>
    public sealed class DummyCompressor : CompressorBase
    {
        #region Constrcutors (4)

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

        #endregion Constrcutors (4)

        #region Methods (2)

        /// <inheriteddoc />
        protected override void OnCompress(Stream src, Stream dest, int? bufferSize)
        {
            // dummy
        }

        /// <inheriteddoc />
        protected override void OnUncompress(Stream src, Stream dest, int? bufferSize)
        {
            // dummy
        }

        #endregion Methods (2)
    }
}