// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region CLASS: SynchronizedCollection<T>

    /// <summary>
    /// A thread safe collection.
    /// </summary>
    /// <typeparam name="T">Type of the items.</typeparam>
    public class SynchronizedCollection<T> : ObjectBase,
                                             ICollection<T>, IReadOnlyCollection<T>,
                                             ICollection
    {
        #region Fields (1)

        private readonly ICollection<T> _BASE_COLL;

        #endregion Fields (1)

        #region Constructors (3)

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedCollection{T}" /> class.
        /// </summary>
        public SynchronizedCollection()
            : this(items: new List<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedCollection{T}" /> class.
        /// </summary>
        /// <param name="items">
        /// The initial items / collection.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is <see langword="null" />.
        /// </exception>
        public SynchronizedCollection(IEnumerable<T> items)
            : this(items: items,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedCollection{T}" /> class.
        /// </summary>
        /// <param name="items">
        /// The initial items / collection.
        /// </param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public SynchronizedCollection(IEnumerable<T> items, object sync)
            : base(isSynchronized: true,
                   sync: sync)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this._BASE_COLL = (ICollection<T>)this.PrepareInitialItems(items);
        }

        #endregion Constructors (3)

        #region Properties (4)

        /// <summary>
        /// Gets the base collection.
        /// </summary>
        public ICollection<T> BaseCollection
        {
            get { return this._BASE_COLL; }
        }

        /// <summary>
        /// Gets the converter to use.
        /// </summary>
        protected virtual IConverter Converter
        {
            get { return GlobalServices.Converter; }
        }

        /// <inheriteddoc />
        public int Count
        {
            get { return this.InvokeForBaseCollection(coll => coll.Count); }
        }

        /// <inheriteddoc />
        public bool IsReadOnly
        {
            get { return this.InvokeForBaseCollection(coll => coll.IsReadOnly); }
        }

        #endregion Properties (4)

        #region Methods (18)

        /// <inheriteddoc />
        public void Add(T item)
        {
            this.InvokeForBaseCollection((coll, state) => coll.Add(state.Item),
                                         new
                                         {
                                             Item = item,
                                         });
        }

        /// <summary>
        /// Changes the type of an object.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="input">The input object.</param>
        /// <returns>The casted / converted object.</returns>
        protected TResult ChangeObject<TResult>(object input)
        {
            var converter = this.Converter;
            if (converter != null)
            {
                return converter.ChangeType<TResult>(value: input);
            }

            return (TResult)input;
        }

        /// <inheriteddoc />
        public void Clear()
        {
            this.InvokeForBaseCollection((coll) => coll.Clear());
        }

        /// <inheriteddoc />
        public bool Contains(T item)
        {
            return this.InvokeForBaseCollection((coll, state) => coll.Contains(state.Item),
                                                new
                                                {
                                                    Item = item,
                                                });
        }

        /// <inheriteddoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.InvokeForBaseCollection((coll, state) => coll.CopyTo(state.TargetArray,
                                                                      state.StartIndex),
                                         new
                                         {
                                             StartIndex = arrayIndex,
                                             TargetArray = array,
                                         });
        }

        /// <inheriteddoc />
        void ICollection.CopyTo(Array array, int index)
        {
            this.IfCollection((c, s) => c.CopyTo(s.TargetArray, s.StartIndex),
                              (c, s) =>
                              {
                                  var srcArray = c.AsArray();
                                  try
                                  {
                                      Array.Copy(srcArray, 0,
                                                 s.TargetArray, s.StartIndex,
                                                 srcArray.Length);
                                  }
                                  finally
                                  {
                                      srcArray = null;
                                  }
                              },
                              new
                              {
                                  StartIndex = index,
                                  TargetArray = array,
                              });
        }

        /// <inheriteddoc />
        public IEnumerator<T> GetEnumerator()
        {
            return this.InvokeForBaseCollection((coll, state) => SynchronizedEnumerator.Create(seq: coll,
                                                                                               sync: state.Dictionary._SYNC),
                                                new
                                                {
                                                    Dictionary = this,
                                                });
        }

        /// <inheriteddoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void IfCollection(Action<ICollection> isTrueAction,
                                  Action<ICollection<T>> isFalseAction)
        {
            this.IfCollection(isTrueAction: (c, f) => f.IsTrue(c),
                              isFalseAction: (c, f) => f.IsFalse(c),
                              actionState: new
                                  {
                                      IsFalse = isFalseAction,
                                      IsTrue = isTrueAction,
                                  });
        }

        private void IfCollection<TState>(Action<ICollection, TState> isTrueAction,
                                          Action<ICollection<T>, TState> isFalseAction,
                                          TState actionState)
        {
            this.IfCollection(
                isTrueFunc: (c, s) =>
                    {
                        s.IsTrue(c,
                                 s.ActionState);

                        return (object)null;
                    },
                isFalseFunc: (c, s) =>
                    {
                        s.IsFalse(c,
                                  s.ActionState);

                        return (object)null;
                    },
                funcState: new
                    {
                        ActionState = actionState,
                        IsFalse = isFalseAction,
                        IsTrue = isTrueAction,
                    });
        }

        private TResult IfCollection<TResult>(Func<ICollection, TResult> isTrueFunc,
                                              Func<ICollection<T>, TResult> isFalseFunc)
        {
            return this.IfCollection(isTrueFunc: (c, f) => f.IsTrue(c),
                                     isFalseFunc: (c, f) => f.IsFalse(c),
                                     funcState: new
                                         {
                                             IsFalse = isFalseFunc,
                                             IsTrue = isTrueFunc,
                                         });
        }

        private TResult IfCollection<TState, TResult>(Func<ICollection, TState, TResult> isTrueFunc,
                                                      Func<ICollection<T>, TState, TResult> isFalseFunc,
                                                      TState funcState)
        {
            return this.InvokeForBaseCollection(
                func: (coll, state) =>
                    {
                        var c = coll as ICollection;

                        return (c != null) ? state.IsTrue(c, state.FuncState)
                                           : state.IsFalse(coll, state.FuncState);
                    },
                funcState: new
                    {
                        FuncState = funcState,
                        IsFalse = isFalseFunc,
                        IsTrue = isTrueFunc,
                    });
        }

        private void InvokeForBaseCollection(Action<ICollection<T>> action)
        {
            this.InvokeForBaseCollection(action: (c, a) => a(c),
                                         actionState: action);
        }

        private void InvokeForBaseCollection<TState>(Action<ICollection<T>, TState> action,
                                                     TState actionState)
        {
            this.InvokeForBaseCollection(func: (coll, state) =>
                                         {
                                             state.Action(coll,
                                                          state.ActionState);

                                             return (object)null;
                                         }, funcState: new
                                         {
                                             Action = action,
                                             ActionState = actionState,
                                         });
        }

        private TResult InvokeForBaseCollection<TResult>(Func<ICollection<T>, TResult> func)
        {
            return this.InvokeForBaseCollection(func: (c, f) => f(c),
                                                funcState: func);
        }

        private TResult InvokeForBaseCollection<TState, TResult>(Func<ICollection<T>, TState, TResult> func,
                                                                 TState funcState)
        {
            TResult result;

            lock (this._SYNC)
            {
                result = func(this._BASE_COLL,
                              funcState);
            }

            return result;
        }

        /// <summary>
        /// Prepares the initial items that were submitted with the constructor.
        /// </summary>
        /// <param name="items">The initial items.</param>
        /// <returns>The prepared collection.</returns>
        protected virtual IEnumerable<T> PrepareInitialItems(IEnumerable<T> items)
        {
            var result = items as ICollection<T>;
            if (result == null)
            {
                result = new List<T>();

                result.AddRange(items);
            }

            return result;
        }

        /// <inheriteddoc />
        public bool Remove(T item)
        {
            return this.InvokeForBaseCollection((coll, state) => coll.Remove(state.Item),
                                                new
                                                {
                                                    Item = item,
                                                });
        }

        #endregion Methods (18)
    }

    #endregion CLASS: SynchronizedCollection<T>

    #region CLASS: SynchronizedCollection

    /// <summary>
    /// Factory class for <see cref="SynchronizedCollection{T}" />.
    /// </summary>
    public static class SynchronizedCollection
    {
        #region Methods (3)

        /// <summary>
        /// Creates a new instance of the <see cref="SynchronizedCollection{T}" /> class.
        /// </summary>
        /// <returns>The new instance.</returns>
        public static SynchronizedCollection<T> Create<T>()
        {
            return new SynchronizedCollection<T>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SynchronizedCollection{T}" /> class.
        /// </summary>
        /// <param name="items">
        /// The initial items / collection.
        /// </param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is <see langword="null" />.
        /// </exception>
        public static SynchronizedCollection<T> Create<T>(IEnumerable<T> items)
        {
            return new SynchronizedCollection<T>(items: items);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SynchronizedCollection{T}" /> class.
        /// </summary>
        /// <param name="items">
        /// The initial items / collection.
        /// </param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static SynchronizedCollection<T> Create<T>(IEnumerable<T> items, object sync)
        {
            return new SynchronizedCollection<T>(items: items,
                                                 sync: sync);
        }

        #endregion Methods (3)
    }

    #endregion CLASS: SynchronizedCollection
}