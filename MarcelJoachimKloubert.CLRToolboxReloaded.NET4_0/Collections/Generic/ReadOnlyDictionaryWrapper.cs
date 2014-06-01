// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    /// <summary>
    /// An <see cref="IReadOnlyDictionary{TKey, TValue}"/> wrapper for an <see cref="IDictionary{TKey, TValue}" /> object.
    /// </summary>
    /// <typeparam name="TKey">Type of the keys.</typeparam>
    /// <typeparam name="TValue">Type of the values.</typeparam>
    public sealed class ReadOnlyDictionaryWrapper<TKey, TValue> : ObjectBase, IReadOnlyDictionary<TKey, TValue>
    {
        #region Fields (1)

        private readonly IDictionary<TKey, TValue> _DICTIONARY;

        #endregion Fields (1)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionaryWrapper{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="dict">The value for the <see cref="ReadOnlyDictionaryWrapper{TKey, TValue}.InnerDictionary" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> is <see langword="null" />.
        /// </exception>
        public ReadOnlyDictionaryWrapper(IDictionary<TKey, TValue> dict)
            : base(isSynchronized: false)
        {
            if (dict == null)
            {
                throw new ArgumentNullException("dict");
            }

            this._DICTIONARY = dict;
        }

        #endregion Constructors (1)

        #region Properties (5)

        /// <inheriteddoc />
        public int Count
        {
            get { return this._DICTIONARY.Count; }
        }

        /// <summary>
        /// Gets the inner dictionary.
        /// </summary>
        public IDictionary<TKey, TValue> InnerDictionary
        {
            get { return this._DICTIONARY; }
        }

        /// <inheriteddoc />
        public IEnumerable<TKey> Keys
        {
            get { return this._DICTIONARY.Keys; }
        }

        /// <inheriteddoc />
        public IEnumerable<TValue> Values
        {
            get { return this._DICTIONARY.Values; }
        }

        /// <inheriteddoc />
        public TValue this[TKey key]
        {
            get { return this._DICTIONARY[key]; }
        }

        #endregion Properties (5)

        #region Methods (4)

        /// <inheriteddoc />
        public bool ContainsKey(TKey key)
        {
            return this._DICTIONARY
                       .ContainsKey(key);
        }

        /// <inheriteddoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            return this._DICTIONARY
                       .TryGetValue(key, out value);
        }

        /// <inheriteddoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this._DICTIONARY
                       .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion Methods (4)
    }
}