// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// A data transformer that simply copies data from source to destination.
    /// </summary>
    public sealed class DataCopier : DataTransformerBase
    {
        #region Constructors (4)

        /// <inheriteddoc />
        public DataCopier(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public DataCopier(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public DataCopier(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public DataCopier()
            : base()
        {
        }

        #endregion Constructors (4)

        #region Properties (2)

        /// <inheriteddoc />
        public override bool CanRestoreData
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public override bool CanTransformData
        {
            get { return true; }
        }

        #endregion Properties (2)

        #region Methods (2)

        /// <inheriteddoc />
        protected override void OnRestoreData(Stream src, Stream dest, int? bufferSize)
        {
            this.OnTransformData(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override void OnTransformData(Stream src, Stream dest, int? bufferSize)
        {
            this.CopyData(src, dest, bufferSize);
        }

        #endregion Methods (2)
    }
}