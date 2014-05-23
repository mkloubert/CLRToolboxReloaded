// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    /// <summary>
    /// An <see cref="IReadOnlyCollection{T}"/> wrapper for an <see cref="ICollection{T}" /> object.
    /// </summary>
    /// <typeparam name="T">Type of the items.</typeparam>
    public sealed class ReadOnlyCollectionWrapper<T> : ObjectBase, IReadOnlyCollection<T>
    {
        #region Fields (1)

        private readonly ICollection<T> _COLLECTION;

        #endregion Fields (1)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyCollectionWrapper{T}" /> class.
        /// </summary>
        /// <param name="collection">The value for the <see cref="ReadOnlyCollectionWrapper{T}.InnerCollection" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> is <see langword="null" />.
        /// </exception>
        public ReadOnlyCollectionWrapper(ICollection<T> collection)
            : base(synchronized: false)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            this._COLLECTION = collection;
        }

        #endregion Constructors (1)

        #region Properties (2)

        /// <inheriteddoc />
        public int Count
        {
            get { return this._COLLECTION.Count; }
        }

        /// <summary>
        /// Gets the inner collection.
        /// </summary>
        public ICollection<T> InnerCollection
        {
            get { return this._COLLECTION; }
        }

        #endregion Properties (2)

        #region Methods (2)

        /// <inheriteddoc />
        public IEnumerator<T> GetEnumerator()
        {
            return this._COLLECTION.GetEnumerator();
        }

        /// <inheriteddoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion Methods (2)
    }
}