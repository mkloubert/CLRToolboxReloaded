// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region CLASS: DictionaryWrapper<TKey, TValue>

    /// <summary>
    /// An <see cref="IDictionary{TKey, TValue}" /> wrapper for an <see cref="IList" /> IDictionary.
    /// </summary>
    /// <typeparam name="TKey">Type of the keys.</typeparam>
    /// <typeparam name="TValue">Type of the values.</typeparam>
    public class DictionaryWrapper<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionary
    {
        #region Fields (2)

        private readonly ConverterProvider _CONVERTER_PROVIDER;
        private readonly IDictionary _INNER_DICT;

        #endregion Fields (2)

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryWrapper{TKey, TValue}" /> class.
        /// </summary>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public DictionaryWrapper()
            : this(convProvider: GetGlobalConverter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryWrapper{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="convProvider">The function or method that provides the converter to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="convProvider" /> is <see langword="null" />.
        /// </exception>
        public DictionaryWrapper(ConverterProvider convProvider)
            : this(dict: new Dictionary<object, object>(),
                   convProvider: convProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryWrapper{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="dict">The value for the <see cref="DictionaryWrapper{TKey, TValue}.InnerDictionary" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public DictionaryWrapper(IDictionary dict)
            : this(dict: dict,
                   convProvider: GetGlobalConverter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryWrapper{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="dict">The value for the <see cref="DictionaryWrapper{TKey, TValue}.InnerDictionary" /> property.</param>
        /// <param name="convProvider">The function or method that provides the converter to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> and/or <paramref name="convProvider" /> are <see langword="null" />.
        /// </exception>
        public DictionaryWrapper(IDictionary dict, ConverterProvider convProvider)
        {
            if (dict == null)
            {
                throw new ArgumentNullException("dict");
            }

            if (convProvider == null)
            {
                throw new ArgumentNullException("convProvider");
            }

            this._CONVERTER_PROVIDER = convProvider;
            this._INNER_DICT = dict;
        }

        #endregion Constructors (4)

        #region Delegates and events (1)

        /// <summary>
        /// Provider for an <see cref="IConverter" /> that should be used by an instance of that class.
        /// </summary>
        /// <param name="wrapper">The underlying wrapper instance.</param>
        /// <returns>The converter to use.</returns>
        public delegate IConverter ConverterProvider(DictionaryWrapper<TKey, TValue> wrapper);

        #endregion Delegates and events (1)

        #region Properties (14)

        /// <inheriteddoc />
        public int Count
        {
            get { return this._INNER_DICT.Count; }
        }

        /// <inheriteddoc />
        public IDictionary InnerDictionary
        {
            get { return this._INNER_DICT; }
        }

        /// <inheriteddoc />
        public bool IsFixedSize
        {
            get { return this._INNER_DICT.IsFixedSize; }
        }

        /// <inheriteddoc />
        public bool IsReadOnly
        {
            get { return this._INNER_DICT.IsReadOnly; }
        }

        /// <inheriteddoc />
        public bool IsSynchronized
        {
            get { return this._INNER_DICT.IsSynchronized; }
        }

        /// <inheriteddoc />
        public ICollection<TKey> Keys
        {
            get
            {
                var coll = this._INNER_DICT.Keys;
                if (coll == null)
                {
                    return null;
                }

                var result = coll as ICollection<TKey>;
                if (result == null)
                {
                    // needs to be converted

                    result = coll.Cast<object>()
                                 .Select(o => this.ConvertTo<TKey>(o))
                                 .ToArray();
                }

                return result;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return this.Keys; }
        }

        ICollection IDictionary.Keys
        {
            get { return this._INNER_DICT.Keys; }
        }

        /// <inheriteddoc />
        public object SyncRoot
        {
            get { return this._INNER_DICT.SyncRoot; }
        }

        /// <inheriteddoc />
        public ICollection<TValue> Values
        {
            get
            {
                var coll = this._INNER_DICT.Values;
                if (coll == null)
                {
                    return null;
                }

                var result = coll as ICollection<TValue>;
                if (result == null)
                {
                    // needs to be converted

                    result = coll.Cast<object>()
                                 .Select(o => this.ConvertTo<TValue>(o))
                                 .ToArray();
                }

                return result;
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return this.Values; }
        }

        ICollection IDictionary.Values
        {
            get { return this._INNER_DICT.Values; }
        }

        /// <inheriteddoc />
        public TValue this[TKey key]
        {
            get { return this.ConvertTo<TValue>(this._INNER_DICT[key]); }

            set { this._INNER_DICT[key] = value; }
        }

        object IDictionary.this[object key]
        {
            get { return this._INNER_DICT[key]; }

            set { this._INNER_DICT[key] = value; }
        }

        #endregion Properties (14)

        #region Methods (18)

        /// <inheriteddoc />
        public void Add(TKey key, TValue value)
        {
            this._INNER_DICT
                .Add(key, value);
        }

        void IDictionary.Add(object key, object value)
        {
            this._INNER_DICT
                .Add(key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key,
                     item.Value);
        }

        /// <inheriteddoc />
        public void Clear()
        {
            this._INNER_DICT
                .Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if (this.TryGetValue(item.Key, out value) == false)
            {
                return false;
            }

            return object.Equals(item.Value, value);
        }

        bool IDictionary.Contains(object key)
        {
            return this._INNER_DICT
                       .Contains(key);
        }

        /// <inheriteddoc />
        public bool ContainsKey(TKey key)
        {
            return this._INNER_DICT
                       .Contains(key);
        }

        private T ConvertTo<T>(object input)
        {
            var converter = this._CONVERTER_PROVIDER(this);

            return converter != null ? converter.ChangeType<T>(value: input)
                                     : (T)input;
        }

        /// <inheriteddoc />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if ((index < 0) || (index > array.Length))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            var array2 = this.ToArray();
            var count = array2.Length;

            if ((array.Length - index) < count)
            {
                throw new ArgumentException("array+index");
            }

            for (var i = 0; i < count; i++)
            {
                array[index++] = array2[i];
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this._INNER_DICT
                .CopyTo(array, index);
        }

        /// <inheriteddoc />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var seq = this._INNER_DICT as IEnumerable<KeyValuePair<TKey, TValue>>;
            if (seq == null)
            {
                seq = this._INNER_DICT
                          .SelectEntries()
                          .Select(e => new KeyValuePair<TKey, TValue>(key: this.ConvertTo<TKey>(e.Key),
                                                                      value: this.ConvertTo<TValue>(e.Value)));
            }

            return seq.GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return this._INNER_DICT
                       .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private static IConverter GetGlobalConverter(DictionaryWrapper<TKey, TValue> wrapper)
        {
            return GlobalConverter.Current;
        }

        /// <inheriteddoc />
        public bool Remove(TKey key)
        {
            var dict = this._INNER_DICT as IDictionary<TKey, TValue>;
            if (dict != null)
            {
                return dict.Remove(key);
            }

            var result = this._INNER_DICT.Contains(key);
            if (result)
            {
                this._INNER_DICT
                    .Remove(key);
            }

            return result;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            var key = item.Key;

            TValue value;
            if (this.TryGetValue(key, out value) == false)
            {
                return false;
            }

            if (object.Equals(item.Value, value) == false)
            {
                return false;
            }

            return this.Remove(key);
        }

        void IDictionary.Remove(object key)
        {
            this._INNER_DICT
                .Remove(key);
        }

        /// <inheriteddoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (this._INNER_DICT.Contains(key))
            {
                value = this.ConvertTo<TValue>(this._INNER_DICT[key]);
                return true;
            }

            value = default(TValue);
            return false;
        }

        #endregion Methods (18)
    }

    #endregion CLASS: DictionaryWrapper<TKey, TValue>

    #region CLASS: DictionaryWrapper<TKey>

    /// <summary>
    /// A more simle version of <see cref="DictionaryWrapper{TKey, TValue}" /> class with objects as values.
    /// </summary>
    /// <typeparam name="TKey">Type of the keys.</typeparam>
    public class DictionaryWrapper<TKey> : DictionaryWrapper<TKey, object>
    {
        #region Constructors (4)

        /// <inheriteddoc />
        public DictionaryWrapper()
            : base()
        {
        }

        /// <inheriteddoc />
        public DictionaryWrapper(ConverterProvider convProvider)
            : base(convProvider)
        {
        }

        /// <inheriteddoc />
        public DictionaryWrapper(IDictionary dict)
            : base(dict)
        {
        }

        /// <inheriteddoc />
        public DictionaryWrapper(IDictionary dict, ConverterProvider convProvider)
            : base(dict, convProvider)
        {
        }

        #endregion Constructors (4)
    }

    #endregion CLASS: DictionaryWrapper<TKey>

    #region CLASS: DictionaryWrapper

    /// <summary>
    /// A more simle version of <see cref="DictionaryWrapper{TKey}" /> class with objects as keys.
    /// </summary>
    public sealed class DictionaryWrapper : DictionaryWrapper<object>
    {
        #region Constructors (4)

        /// <inheriteddoc />
        public DictionaryWrapper()
            : base()
        {
        }

        /// <inheriteddoc />
        public DictionaryWrapper(ConverterProvider convProvider)
            : base(convProvider)
        {
        }

        /// <inheriteddoc />
        public DictionaryWrapper(IDictionary dict)
            : base(dict)
        {
        }

        /// <inheriteddoc />
        public DictionaryWrapper(IDictionary dict, ConverterProvider convProvider)
            : base(dict, convProvider)
        {
        }

        #endregion Constructors (4)

        #region Methods (12)

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper{TKey, TValue}" /> class.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys.</typeparam>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public static DictionaryWrapper<TKey, TValue> Create<TKey, TValue>()
        {
            return new DictionaryWrapper<TKey, TValue>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper{TKey, TValue}" /> class.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys.</typeparam>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <param name="dict">The inner dictionary to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public static DictionaryWrapper<TKey, TValue> Create<TKey, TValue>(IDictionary dict)
        {
            return new DictionaryWrapper<TKey, TValue>(dict);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper{TKey, TValue}" /> class.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys.</typeparam>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <param name="converter">The converter to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter" /> is <see langword="null" />.
        /// </exception>
        public static DictionaryWrapper<TKey, TValue> Create<TKey, TValue>(IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return new DictionaryWrapper<TKey, TValue>((w) => converter);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper{TKey, TValue}" /> class.
        /// </summary>
        /// <typeparam name="TKey">Type of the keys.</typeparam>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <param name="dict">The inner dictionary to use.</param>
        /// <param name="converter">The converter to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> and/or <paramref name="converter" /> are <see langword="null" />.
        /// </exception>
        public static DictionaryWrapper<TKey, TValue> Create<TKey, TValue>(IDictionary dict, IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return new DictionaryWrapper<TKey, TValue>(dict,
                                                       (w) => converter);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper{TValue}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public static DictionaryWrapper<TValue> Create<TValue>()
        {
            return new DictionaryWrapper<TValue>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper{TValue}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <param name="dict">The inner dictionary to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public static DictionaryWrapper<TValue> Create<TValue>(IDictionary dict)
        {
            return new DictionaryWrapper<TValue>(dict);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper{TValue}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <param name="converter">The converter to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter" /> is <see langword="null" />.
        /// </exception>
        public static DictionaryWrapper<TValue> Create<TValue>(IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return new DictionaryWrapper<TValue>((w) => converter);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper{TValue}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the values.</typeparam>
        /// <param name="dict">The inner dictionary to use.</param>
        /// <param name="converter">The converter to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> and/or <paramref name="converter" /> are <see langword="null" />.
        /// </exception>
        public static DictionaryWrapper<TValue> Create<TValue>(IDictionary dict, IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return new DictionaryWrapper<TValue>(dict,
                                                 (w) => converter);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper" /> class.
        /// </summary>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public static DictionaryWrapper Create()
        {
            return new DictionaryWrapper();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper" /> class.
        /// </summary>
        /// <param name="dict">The inner dictionary to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public static DictionaryWrapper Create(IDictionary dict)
        {
            return new DictionaryWrapper(dict);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper" /> class.
        /// </summary>
        /// <param name="converter">The converter to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter" /> is <see langword="null" />.
        /// </exception>
        public static DictionaryWrapper Create(IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return new DictionaryWrapper((w) => converter);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DictionaryWrapper" /> class.
        /// </summary>
        /// <param name="dict">The inner dictionary to use.</param>
        /// <param name="converter">The converter to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> and/or <paramref name="converter" /> are <see langword="null" />.
        /// </exception>
        public static DictionaryWrapper Create(IDictionary dict, IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return new DictionaryWrapper(dict,
                                         (w) => converter);
        }

        #endregion Methods (12)
    }

    #endregion CLASS: DictionaryWrapper
}