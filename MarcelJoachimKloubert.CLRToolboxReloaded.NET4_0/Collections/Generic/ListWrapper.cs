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
    #region CLASS: ListWrapper<T>

    /// <summary>
    /// An <see cref="IList{T}" /> wrapper for an <see cref="IList" /> object.
    /// </summary>
    /// <typeparam name="T">Type of the objects.</typeparam>
    public class ListWrapper<T> : IList<T>, IReadOnlyList<T>, IList
    {
        #region Fields (2)

        private readonly ConverterProvider _CONVERTER_PROVIDER;
        private readonly IList _INNER_LIST;

        #endregion Fields (2)

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="ListWrapper{T}" /> class.
        /// </summary>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public ListWrapper()
            : this(convProvider: GetGlobalConverter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListWrapper{T}" /> class.
        /// </summary>
        /// <param name="convProvider">The function or method that provides the converter to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="convProvider" /> is <see langword="null" />.
        /// </exception>
        public ListWrapper(ConverterProvider convProvider)
            : this(list: new List<object>(),
                   convProvider: convProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListWrapper{T}" /> class.
        /// </summary>
        /// <param name="list">The value for the <see cref="ListWrapper{T}.InnerList" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public ListWrapper(IList list)
            : this(list: list,
                   convProvider: GetGlobalConverter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListWrapper{T}" /> class.
        /// </summary>
        /// <param name="list">The value for the <see cref="ListWrapper{T}.InnerList" /> property.</param>
        /// <param name="convProvider">The function or method that provides the converter to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list" /> and/or <paramref name="convProvider" /> are <see langword="null" />.
        /// </exception>
        public ListWrapper(IList list, ConverterProvider convProvider)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (convProvider == null)
            {
                throw new ArgumentNullException("convProvider");
            }

            this._CONVERTER_PROVIDER = convProvider;
            this._INNER_LIST = list;
        }

        #endregion Constructors (4)

        #region Delegates and events (1)

        /// <summary>
        /// Provider for an <see cref="IConverter" /> that should be used by an instance of that class.
        /// </summary>
        /// <param name="wrapper">The underlying wrapper instance.</param>
        /// <returns>The converter to use.</returns>
        public delegate IConverter ConverterProvider(ListWrapper<T> wrapper);

        #endregion Delegates and events (1)

        #region Properties (8)

        /// <inheriteddoc />
        public int Count
        {
            get { return this._INNER_LIST.Count; }
        }

        /// <summary>
        /// Gets the inner list.
        /// </summary>
        public IList InnerList
        {
            get { return this._INNER_LIST; }
        }

        /// <inheriteddoc />
        public bool IsFixedSize
        {
            get { return this._INNER_LIST.IsFixedSize; }
        }

        /// <inheriteddoc />
        public bool IsReadOnly
        {
            get { return this._INNER_LIST.IsReadOnly; }
        }

        /// <inheriteddoc />
        public bool IsSynchronized
        {
            get { return this._INNER_LIST.IsSynchronized; }
        }

        /// <summary>
        /// Gets the provider function / method that provides the converter to use.
        /// </summary>
        public ConverterProvider ProviderOfConverter
        {
            get { return this._CONVERTER_PROVIDER; }
        }

        /// <inheriteddoc />
        public object SyncRoot
        {
            get { return this._INNER_LIST.SyncRoot; }
        }

        /// <inheriteddoc />
        public T this[int index]
        {
            get { return this.ConvertTo(this._INNER_LIST[index]); }

            set { this._INNER_LIST[index] = value; }
        }

        object IList.this[int index]
        {
            get { return this._INNER_LIST[index]; }

            set { this._INNER_LIST[index] = value; }
        }

        #endregion Properties (7)

        #region Methods (18)

        /// <inheriteddoc />
        public void Add(T item)
        {
            this._INNER_LIST
                .Add(item);
        }

        int IList.Add(object value)
        {
            return this._INNER_LIST
                       .Add(value);
        }

        /// <inheriteddoc />
        public void Clear()
        {
            this._INNER_LIST
                .Clear();
        }

        /// <inheriteddoc />
        public bool Contains(T item)
        {
            return this._INNER_LIST
                       .Contains(item);
        }

        bool IList.Contains(object value)
        {
            return this._INNER_LIST
                       .Contains(value);
        }

        private T ConvertTo(object input)
        {
            var converter = this._CONVERTER_PROVIDER(this);

            return converter != null ? converter.ChangeType<T>(value: input)
                                     : (T)input;
        }

        /// <inheriteddoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(this._INNER_LIST.AsArray(), 0,
                       array, arrayIndex,
                       this._INNER_LIST.Count);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this._INNER_LIST
                .CopyTo(array, index);
        }

        /// <inheriteddoc />
        public IEnumerator<T> GetEnumerator()
        {
            var seq = this._INNER_LIST as IEnumerable<T>;
            if (seq == null)
            {
                seq = this._INNER_LIST
                          .Cast<object>()
                          .Select(o => this.ConvertTo(o));
            }

            return seq.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private static IConverter GetGlobalConverter(ListWrapper<T> wrapper)
        {
            return GlobalConverter.Current;
        }

        /// <inheriteddoc />
        public int IndexOf(T item)
        {
            return this._INNER_LIST
                       .IndexOf(item);
        }

        int IList.IndexOf(object value)
        {
            return this._INNER_LIST
                       .IndexOf(value);
        }

        /// <inheriteddoc />
        public void Insert(int index, T item)
        {
            this._INNER_LIST
                .Insert(index, item);
        }

        void IList.Insert(int index, object value)
        {
            this._INNER_LIST
                .Insert(index, value);
        }

        /// <inheriteddoc />
        public bool Remove(T item)
        {
            var coll = this._INNER_LIST as ICollection<T>;
            if (coll != null)
            {
                return coll.Remove(item);
            }

            var result = this._INNER_LIST.Contains(item);
            if (result)
            {
                this._INNER_LIST
                    .Remove(item);
            }

            return result;
        }

        void IList.Remove(object value)
        {
            this._INNER_LIST
                .Remove(value);
        }

        /// <inheriteddoc />
        public void RemoveAt(int index)
        {
            this._INNER_LIST
                .RemoveAt(index);
        }

        #endregion Methods (18)
    }

    #endregion CLASS: ListWrapper<T>

    #region CLASS: ListWrapper

    /// <summary>
    /// Simple implementation of <see cref="ListWrapper{T}" /> class.
    /// </summary>
    public sealed class ListWrapper : ListWrapper<object>
    {
        #region Constructors (4)

        /// <inheriteddoc />
        public ListWrapper()
            : base()
        {
        }

        /// <inheriteddoc />
        public ListWrapper(ConverterProvider convProvider)
            : base(convProvider)
        {
        }

        /// <inheriteddoc />
        public ListWrapper(IList list)
            : base(list)
        {
        }

        /// <inheriteddoc />
        public ListWrapper(IList list, ConverterProvider convProvider)
            : base(list, convProvider)
        {
        }

        #endregion Constructors (4)

        #region Methods (8)

        /// <summary>
        /// Creates a new instance of the <see cref="ListWrapper{T}" /> class.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public static ListWrapper<T> Create<T>()
        {
            return new ListWrapper<T>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListWrapper{T}" /> class.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="list">The inner list to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public static ListWrapper<T> Create<T>(IList list)
        {
            return new ListWrapper<T>(list);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListWrapper{T}" /> class.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="converter">The converter to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter" /> is <see langword="null" />.
        /// </exception>
        public static ListWrapper<T> Create<T>(IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return new ListWrapper<T>((w) => converter);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListWrapper{T}" /> class.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="list">The inner list to use.</param>
        /// <param name="converter">The converter to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list" /> and/or <paramref name="converter" /> are <see langword="null" />.
        /// </exception>
        public static ListWrapper<T> Create<T>(IList list, IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return new ListWrapper<T>(list,
                                      (w) => converter);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListWrapper" /> class.
        /// </summary>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public static ListWrapper Create()
        {
            return new ListWrapper();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListWrapper" /> class.
        /// </summary>
        /// <param name="list">The inner list to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// Converter from <see cref="GlobalConverter.Current" /> is used.
        /// </remarks>
        public static ListWrapper Create(IList list)
        {
            return new ListWrapper(list);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListWrapper" /> class.
        /// </summary>
        /// <param name="converter">The converter to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="converter" /> is <see langword="null" />.
        /// </exception>
        public static ListWrapper Create(IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return new ListWrapper((w) => converter);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ListWrapper" /> class.
        /// </summary>
        /// <param name="list">The inner list to use.</param>
        /// <param name="converter">The converter to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list" /> and/or <paramref name="converter" /> are <see langword="null" />.
        /// </exception>
        public static ListWrapper Create(IList list, IConverter converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            return new ListWrapper(list,
                                   (w) => converter);
        }

        #endregion Methods (8)
    }

    #endregion CLASS: ListWrapper
}