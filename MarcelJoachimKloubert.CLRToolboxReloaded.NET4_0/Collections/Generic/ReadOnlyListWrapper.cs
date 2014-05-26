// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    /// <summary>
    /// An <see cref="IReadOnlyCollection{T}"/> wrapper for an <see cref="IList{T}" /> object.
    /// </summary>
    /// <typeparam name="T">Type of the items.</typeparam>
    public sealed class ReadOnlyListWrapper<T> : ObjectBase, IReadOnlyList<T>
    {
        #region Fields (1)

        private readonly IList<T> _LIST;

        #endregion Fields (1)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyListWrapper{T}" /> class.
        /// </summary>
        /// <param name="list">The value for the <see cref="ReadOnlyListWrapper{T}.InnerList" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list" /> is <see langword="null" />.
        /// </exception>
        public ReadOnlyListWrapper(IList<T> list)
            : base(synchronized: false)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            this._LIST = list;
        }

        #endregion Constructors (1)

        #region Properties (3)

        /// <inheriteddoc />
        public int Count
        {
            get { return this._LIST.Count; }
        }

        /// <summary>
        /// Gets the inner list.
        /// </summary>
        public IList<T> InnerList
        {
            get { return this._LIST; }
        }

        /// <inheriteddoc />
        public T this[int index]
        {
            get { return this._LIST[index]; }
        }

        #endregion Properties (3)

        #region Methods (2)

        /// <inheriteddoc />
        public IEnumerator<T> GetEnumerator()
        {
            return this._LIST.GetEnumerator();
        }

        /// <inheriteddoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion Methods (2)
    }
}