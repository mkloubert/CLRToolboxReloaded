// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Compression
{
    /// <summary>
    /// A basic object that compressed data.
    /// </summary>
    public abstract class CompressorBase : DataTransformerBase, ICompressor
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected CompressorBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected CompressorBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected CompressorBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected CompressorBase()
            : base()
        {
        }

        #endregion Constrcutors (4)

        #region Properties (2)

        /// <inheriteddoc />
        public override sealed bool CanRestoreData
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public override sealed bool CanTransformData
        {
            get { return true; }
        }

        #endregion Properties (2)

        #region Methods (12)

        /// <inheriteddoc />
        public byte[] Compress(IEnumerable<byte> src)
        {
            return this.TransformData(src);
        }

        /// <inheriteddoc />
        public byte[] Compress(Stream src, int? bufferSize = null)
        {
            return this.TransformData(src, bufferSize);
        }

        /// <inheriteddoc />
        public void Compress(Stream src, Stream dest, int? bufferSize = null)
        {
            this.TransformData(src, dest, bufferSize);
        }

        /// <summary>
        /// Stores the logic for the <see cref="CompressorBase.Compress(Stream, Stream, int?)" /> method.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        protected abstract void OnCompress(Stream src, Stream dest, int? bufferSize);

        /// <inheriteddoc />
        protected override sealed void OnRestoreData(Stream src, Stream dest, int? bufferSize)
        {
            this.OnUncompress(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override sealed void OnTransformData(Stream src, Stream dest, int? bufferSize)
        {
            this.OnCompress(src, dest, bufferSize);
        }

        /// <summary>
        /// Stores the logic for the <see cref="CompressorBase.Uncompress(Stream, Stream, int?)" /> method.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        protected abstract void OnUncompress(Stream src, Stream dest, int? bufferSize);

        /// <inheriteddoc />
        protected override sealed void SaltString(StringBuilder str, Encoding enc)
        {
            // uncompressed data needs no salt
        }

        /// <inheriteddoc />
        public byte[] Uncompress(IEnumerable<byte> src)
        {
            return this.RestoreData(src);
        }

        /// <inheriteddoc />
        public byte[] Uncompress(Stream src, int? bufferSize = null)
        {
            return this.RestoreData(src, bufferSize);
        }

        /// <inheriteddoc />
        public void Uncompress(Stream src, Stream dest, int? bufferSize = null)
        {
            this.RestoreData(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override sealed void UnsaltString(StringBuilder str, Encoding enc)
        {
            // compressed data needs no unsalt
        }

        #endregion Methods (12)
    }
}