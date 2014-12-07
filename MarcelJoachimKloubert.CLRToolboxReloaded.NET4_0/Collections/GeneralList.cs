// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Collections
{
    /// <summary>
    /// Simple implementation of <see cref="IGeneralList" /> interface.
    /// </summary>
    [DebuggerDisplay("GeneralList.Count = {Count}")]
    public class GeneralList : List<object>, IGeneralList
    {
        #region Fields (2)

        /// <summary>
        /// Stores if that list is thread safe or not.
        /// </summary>
        protected readonly bool _IS_SYNCHRONIZED;

        /// <summary>
        /// Stores the object for thread safe operations.
        /// </summary>
        protected readonly object _SYNC;

        #endregion Fields (2)

        #region Constructors (8)

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralList" /> class.
        /// </summary>
        /// <param name="collection">The initial items.</param>
        /// <param name="isSynchronized">The value for the <see cref="GeneralList.IsSynchronized" /> property.</param>
        /// <param name="sync">The value for the <see cref="GeneralList.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public GeneralList(IEnumerable<object> collection, bool isSynchronized, object sync)
            : base(collection: collection)
        {
            if (sync == null)
            {
                throw new ArgumentNullException("sync");
            }

            this._IS_SYNCHRONIZED = isSynchronized;
            this._SYNC = sync;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralList" /> class.
        /// </summary>
        /// <param name="collection">The initial items.</param>
        /// <param name="sync">The value for the <see cref="GeneralList.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public GeneralList(IEnumerable<object> collection, object sync)
            : this(collection: collection,
                   isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralList" /> class.
        /// </summary>
        /// <param name="collection">The initial items.</param>
        /// <param name="isSynchronized">The value for the <see cref="GeneralList.IsSynchronized" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> is <see langword="null" />.
        /// </exception>
        public GeneralList(IEnumerable<object> collection, bool isSynchronized)
            : this(collection: collection,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralList" /> class.
        /// </summary>
        /// <param name="collection">The initial items.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> is <see langword="null" />.
        /// </exception>
        public GeneralList(IEnumerable<object> collection)
            : this(collection: collection,
                   isSynchronized: false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralList" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="GeneralList.IsSynchronized" /> property.</param>
        /// <param name="sync">The value for the <see cref="GeneralList.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public GeneralList(bool isSynchronized, object sync)
            : this(collection: Enumerable.Empty<object>(),
                   isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralList" /> class.
        /// </summary>
        /// <param name="sync">The value for the <see cref="GeneralList.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public GeneralList(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralList" /> class.
        /// </summary>
        /// <param name="isSynchronized">The value for the <see cref="GeneralList.IsSynchronized" /> property.</param>
        public GeneralList(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralList" /> class.
        /// </summary>
        public GeneralList()
            : this(isSynchronized: false)
        {
        }

        #endregion Constructors (8)

        #region Events and delegates (1)

        /// <inheriteddoc />
        public event EventHandler<ErrorEventArgs> ErrorsReceived;

        #endregion Events and delegates (1)

        #region Properties (5)

        /// <inheriteddoc />
        public bool IsEmpty
        {
            get { return this.Count < 1; }
        }

        /// <inheriteddoc />
        public bool IsNotEmpty
        {
            get { return this.IsEmpty == false; }
        }

        /// <inheriteddoc />
        public bool IsSynchronized
        {
            get { return this._IS_SYNCHRONIZED; }
        }

        /// <inheriteddoc />
        public object SyncRoot
        {
            get { return this._SYNC; }
        }

        /// <inheriteddoc />
        public virtual object Tag
        {
            get;
            set;
        }

        #endregion Properties (5)

        #region Methods (14)

        /// <inheriteddoc />
        public void AddRange(IEnumerable seq)
        {
            this.AddRange(collection: seq.Cast<object>());
        }

        /// <inheriteddoc />
        public void AddRangeOf<T>(IEnumerable seq)
        {
            this.AddRange(seq.OfType<T>());
        }

        /// <inheriteddoc />
        public IList<object> AsList()
        {
            return this;
        }

        /// <inheriteddoc />
        public IList<T> AsList<T>(bool ofType = false, IFormatProvider provider = null)
        {
            var list = this as IList<T>;
            if ((list != null) &&
                (ofType == false))
            {
                return list;
            }

            return this.ToList<T>(ofType: ofType,
                                  provider: provider);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IGeneralList.Clone(bool)" />
        public GeneralList Clone(bool copySyncRoot = false)
        {
            return new GeneralList(collection: this,
                                   isSynchronized: this._IS_SYNCHRONIZED,
                                   sync: copySyncRoot ? this._SYNC : new object());
        }

        IGeneralList IGeneralList.Clone(bool copySyncRoot)
        {
            return this.Clone(copySyncRoot: copySyncRoot);
        }

        object ICloneable.Clone()
        {
            return this.Clone(copySyncRoot: false);
        }

        /// <summary>
        /// Converts /casts an object with a default format provider.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="input">The input value.</param>
        /// <returns>The converted / casted object.</returns>
        protected virtual T ConvertTo<T>(object input)
        {
            return this.ConvertTo<T>(input: input,
                                     provider: null);
        }

        /// <summary>
        /// Converts / casts an object.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="input">The input value.</param>
        /// <param name="provider">The optional format provider to use.</param>
        /// <returns>The converted / casted object.</returns>
        protected virtual T ConvertTo<T>(object input,
                                         IFormatProvider provider)
        {
            return GlobalConverter.Current
                                  .ChangeType<T>(value: input,
                                                 provider: provider);
        }

        /// <summary>
        /// Raises the <see cref="GeneralList.ErrorsReceived" /> event.
        /// </summary>
        /// <param name="ex">The thrown exception(s).</param>
        /// <returns>Event was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ex" /> is <see langword="null" />.
        /// </exception>
        protected bool OnErrorsReceived(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            var handle = this.ErrorsReceived;
            if (handle != null)
            {
                handle(this, new ErrorEventArgs(ex));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Selects the items of that list for use in a new list by using a default format provider.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="ofType">
        /// Cast / convert all items (<see langword="false" />) or filter out (<see langword="true" />).
        /// </param>
        /// <returns>The selected items.</returns>
        protected virtual IEnumerable<T> SelectCollectionForNewList<T>(bool ofType = false)
        {
            return this.SelectCollectionForNewList<T>(ofType: ofType,
                                                      provider: null);
        }

        /// <summary>
        /// Selects the items of that list for use in a new list.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="ofType">
        /// Cast / convert all items (<see langword="false" />) or filter out (<see langword="true" />).
        /// </param>
        /// <param name="provider">
        /// The optional format provider that is used if items are casted / converted.
        /// </param>
        /// <returns>The selected items.</returns>
        protected IEnumerable<T> SelectCollectionForNewList<T>(bool ofType,
                                                               IFormatProvider provider)
        {
            if (ofType)
            {
                return Enumerable.OfType<T>(source: this);
            }

            return Enumerable.Select<object, T>(this,
                                                i => this.ConvertTo<T>(input: i,
                                                                       provider: provider));
        }

        /// <inheriteddoc />
        public virtual T[] ToArray<T>(bool ofType = false, IFormatProvider provider = null)
        {
            return this.SelectCollectionForNewList<T>(ofType: ofType,
                                                      provider: provider)
                       .ToArray();
        }

        /// <inheriteddoc />
        public virtual IList<object> ToList()
        {
            return this.Clone(copySyncRoot: false);
        }

        /// <inheriteddoc />
        public virtual IList<T> ToList<T>(bool ofType = false, IFormatProvider provider = null)
        {
            return new List<T>(collection: this.SelectCollectionForNewList<T>(ofType: ofType,
                                                                              provider: provider));
        }

        #endregion Methods (14)
    }
}