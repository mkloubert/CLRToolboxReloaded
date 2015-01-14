// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Collections
{
    /// <summary>
    /// A common object that builds collections.
    /// </summary>
    public class CollectionBuilder : ObjectBase, ICollectionBuilder
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        public CollectionBuilder(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public CollectionBuilder(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        public CollectionBuilder(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        public CollectionBuilder()
            : base()
        {
        }

        #endregion Constrcutors (4)
    }
}