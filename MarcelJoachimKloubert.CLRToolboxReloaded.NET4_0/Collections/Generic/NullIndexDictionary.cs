// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region CLASS: NullIndexDictionary<TValue>

    /// <summary>
    /// A dictionary that uses a nullable integer for its indexes.
    /// <see langword="null" /> indexes can be used to access the last or next index.
    /// </summary>
    /// <typeparam name="TValue">Type of the values.</typeparam>
    public class NullIndexDictionary<TValue> : IDictionary<int?, TValue>,
                                               IReadOnlyDictionary<int?, TValue>,
                                               IEnumerable<KeyValuePair<int, TValue>>
    {
        #region Fields (1)

        private readonly IDictionary<int, TValue> _INNER_DICT;

        #endregion Fields (1)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="NullIndexDictionary{TValue}" /> class.
        /// </summary>
        public NullIndexDictionary()
            : this(innerDict: new Dictionary<int, TValue>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullIndexDictionary{TValue}" /> class.
        /// </summary>
        /// <param name="innerDict">The inner dictionary to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerDict" /> is <see langword="null" />.
        /// </exception>
        public NullIndexDictionary(IDictionary<int, TValue> innerDict)
        {
            if (innerDict == null)
            {
                throw new ArgumentNullException("innerDict");
            }

            this._INNER_DICT = innerDict;
        }

        #endregion Constructors (2)

        #region Methods (15)

        /// <inheriteddoc />
        public void Add(int? key, TValue value)
        {
            if (key.HasValue == false)
            {
                key = (this.TryGetLastIndex() + 1) ?? 0;
            }

            this._INNER_DICT
                .Add(key: key.Value,
                     value: value);
        }

        void ICollection<KeyValuePair<int?, TValue>>.Add(KeyValuePair<int?, TValue> item)
        {
            this.Add(key: item.Key,
                     value: item.Value);
        }

        /// <inheriteddoc />
        public void Clear()
        {
            this._INNER_DICT.Clear();
        }

        bool ICollection<KeyValuePair<int?, TValue>>.Contains(KeyValuePair<int?, TValue> item)
        {
            return this.ContainsKey(item.Key);
        }

        /// <inheriteddoc />
        public bool ContainsKey(int? key)
        {
            if (key.HasValue == false)
            {
                return this.Count > 0;
            }

            return this._INNER_DICT
                       .ContainsKey(key.Value);
        }

        /// <inheriteddoc />
        public void CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex)
        {
            this.CopyToInner(array: array,
                             arrayIndex: arrayIndex,
                             arrayValueProvider: (i, v) => new KeyValuePair<int, TValue>(key: i,
                                                                                         value: v));
        }

        void ICollection<KeyValuePair<int?, TValue>>.CopyTo(KeyValuePair<int?, TValue>[] array, int arrayIndex)
        {
            this.CopyToInner(array: array,
                             arrayIndex: arrayIndex,
                             arrayValueProvider: (i, v) => new KeyValuePair<int?, TValue>(key: i,
                                                                                          value: v));
        }

        private void CopyToInner(Array array, int arrayIndex,
                                 Func<int, TValue, object> arrayValueProvider)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if ((array.Length - arrayIndex) < this.Count)
            {
                throw new ArgumentException("arrayIndex");
            }

            var num = this._INNER_DICT.Count;
            for (var i = 0; i < num; i++)
            {
                array.SetValue(value: arrayValueProvider(i, this[i]),
                               indices: new int[] { arrayIndex + i });
            }
        }

        /// <inheriteddoc />
        public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
        {
            return this._INNER_DICT
                       .GetEnumerator();
        }

        IEnumerator<KeyValuePair<int?, TValue>> IEnumerable<KeyValuePair<int?, TValue>>.GetEnumerator()
        {
            return this._INNER_DICT
                       .Select(i => new KeyValuePair<int?, TValue>(key: i.Key,
                                                                   value: i.Value))
                       .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheriteddoc />
        public bool Remove(int? key)
        {
            if (key.HasValue == false)
            {
                key = this.TryGetLastIndex();
            }

            if (key.HasValue == false)
            {
                return false;
            }

            return this._INNER_DICT
                       .Remove(key.Value);
        }

        bool ICollection<KeyValuePair<int?, TValue>>.Remove(KeyValuePair<int?, TValue> item)
        {
            return this.Remove(key: item.Key);
        }

        private int? TryGetLastIndex()
        {
            return this.Keys
                       .Cast<int?>()
                       .OrderByDescending(k => k)
                       .FirstOrDefault();
        }

        /// <inheriteddoc />
        public bool TryGetValue(int? key, out TValue value)
        {
            value = default(TValue);

            if (key.HasValue == false)
            {
                key = this.TryGetLastIndex();
            }

            if (key.HasValue == false)
            {
                return false;
            }

            return this._INNER_DICT
                       .TryGetValue(key: key.Value,
                                    value: out value);
        }

        #endregion Methods (15)

        #region Properties (8)

        /// <inheriteddoc />
        public int Count
        {
            get { return this._INNER_DICT.Count; }
        }

        /// <inheriteddoc />
        public bool IsReadOnly
        {
            get { return this._INNER_DICT.IsReadOnly; }
        }

        /// <inheriteddoc />
        public ICollection<int> Keys
        {
            get { return this._INNER_DICT.Keys; }
        }

        IEnumerable<int?> IReadOnlyDictionary<int?, TValue>.Keys
        {
            get { return this.Keys.Cast<int?>(); }
        }

        ICollection<int?> IDictionary<int?, TValue>.Keys
        {
            get { return this.Keys.Cast<int?>().ToArray(); }
        }

        /// <inheriteddoc />
        public TValue this[int? key]
        {
            get
            {
                if (key.HasValue == false)
                {
                    key = this.TryGetLastIndex();
                }

                if (key.HasValue == false)
                {
                    throw new ArgumentOutOfRangeException("key");
                }

                return this._INNER_DICT[key.Value];
            }

            set
            {
                if (key.HasValue)
                {
                    this._INNER_DICT[key.Value] = value;
                }
                else
                {
                    this.Add(key: key,
                             value: value);
                }
            }
        }

        /// <inheriteddoc />
        public ICollection<TValue> Values
        {
            get { return this._INNER_DICT.Values; }
        }

        IEnumerable<TValue> IReadOnlyDictionary<int?, TValue>.Values
        {
            get { return this.Values; }
        }

        #endregion Properties (8)
    }

    #endregion CLASS: NullIndexDictionary<TValue>

    #region CLASS: NullIndexDictionary

    /// <summary>
    /// Simple implementation of <see cref="NullIndexDictionary{TValue}" /> class.
    /// </summary>
    public sealed class NullIndexDictionary : NullIndexDictionary<object>
    {
        #region Constructors (2)

        /// <inheriteddoc />
        public NullIndexDictionary()
            : base()
        {
        }

        /// <inheriteddoc />
        public NullIndexDictionary(IDictionary<int, object> innerDict)
            : base(innerDict: innerDict)
        {
        }

        #endregion Constructors (2)
    }

    #endregion CLASS: NullIndexDictionary
}