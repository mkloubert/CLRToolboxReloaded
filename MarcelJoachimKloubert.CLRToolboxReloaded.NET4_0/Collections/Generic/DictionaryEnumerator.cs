// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    /// <summary>
    /// A wrapper for <see cref="IDictionary{TKey, TValue}" /> based enumerators that also can be used as
    /// <see cref="IDictionaryEnumerator" /> instances.
    /// </summary>
    /// <typeparam name="TKey">Type of the keys.</typeparam>
    /// <typeparam name="TValue">Type of the values.</typeparam>
    public partial struct DictionaryEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
    {
        #region Fields (2)

        private readonly IEnumerator<KeyValuePair<TKey, TValue>> _ENUMERATOR;
        private readonly EnumeratorMode _MODE;

        #endregion Fields (2)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEnumerator{TKey, TValue}" /> struct.
        /// </summary>
        /// <param name="seq">The sequence from where to get the inner enumerator from.</param>
        /// <param name="mode">The enumerator mode.</param>
        /// <exception cref="NullReferenceException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public DictionaryEnumerator(IEnumerable<KeyValuePair<TKey, TValue>> seq,
                                    EnumeratorMode mode)
            : this(seq.GetEnumerator(),
                   mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryEnumerator{TKey, TValue}" /> struct.
        /// </summary>
        /// <param name="enumerator">The inner enumerator to use.</param>
        /// <param name="mode">The enumerator mode.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enumerator" /> is <see langword="null" />.
        /// </exception>
        public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator,
                                    EnumeratorMode mode)
        {
            if (enumerator == null)
            {
                throw new ArgumentNullException("enumerator");
            }

            this._ENUMERATOR = enumerator;
            this._MODE = mode;
        }

        #endregion Constructors (2)

        #region Methods (3)

        /// <inheriteddoc />
        public void Dispose()
        {
            this._ENUMERATOR
                .Dispose();
        }

        /// <inheriteddoc />
        public bool MoveNext()
        {
            return this._ENUMERATOR
                       .MoveNext();
        }

        /// <inheriteddoc />
        public void Reset()
        {
            this._ENUMERATOR
                .Reset();
        }

        #endregion Methods (3)

        #region Properties (8)

        /// <inheriteddoc />
        public KeyValuePair<TKey, TValue> Current
        {
            get { return this._ENUMERATOR.Current; }
        }

        object IEnumerator.Current
        {
            get
            {
                switch (this._MODE)
                {
                    case EnumeratorMode.GenericDictionary:
                        return this.Current;

                    case EnumeratorMode.IDictionary:
                        return this.Entry;
                }

                throw new NotImplementedException();
            }
        }

        /// <inheriteddoc />
        public DictionaryEntry Entry
        {
            get
            {
                return new DictionaryEntry(key: this.Key,
                                           value: this.Value);
            }
        }
        
        /// <inheriteddoc />
        public TKey Key
        {
            get { return this.Current.Key; }
        }

        object IDictionaryEnumerator.Key
        {
            get { return this.Key; }
        }

        /// <summary>
        /// Gets the mode of this enumerator.
        /// </summary>
        public EnumeratorMode Mode
        {
            get { return this._MODE; }
        }

        /// <inheriteddoc />
        public TValue Value
        {
            get { return this.Current.Value; }
        }

        object IDictionaryEnumerator.Value
        {
            get { return this.Value; }
        }

        #endregion Properties (7)
    }
}