// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region CLASS: SynchronizedDictionary<TKey, TValue>

    /// <summary>
    /// A thread safe generic dictionary.
    /// </summary>
    /// <typeparam name="TKey">Type of the keys.</typeparam>
    /// <typeparam name="TValue">Type of the values.</typeparam>
    public class SynchronizedDictionary<TKey, TValue> : SynchronizedCollection<KeyValuePair<TKey, TValue>>,
                                                        IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>,
                                                        IDictionary
    {
        #region Constructors (5)

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.
        /// </summary>
        public SynchronizedDictionary()
            : this(items: new Dictionary<TKey, TValue>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="keyComparer">
        /// The key comparer to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyComparer" /> is <see langword="null" />.
        /// </exception>
        public SynchronizedDictionary(IEqualityComparer<TKey> keyComparer)
            : this(keyComparer: keyComparer,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="keyComparer">
        /// The key comparer to use.
        /// </param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyComparer" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public SynchronizedDictionary(IEqualityComparer<TKey> keyComparer, object sync)
            : this(items: new Dictionary<TKey, TValue>(keyComparer),
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="items">
        /// The initial items / dictionary.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is <see langword="null" />.
        /// </exception>
        public SynchronizedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items)
            : this(items: items,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="items">
        /// The initial items / dictionary.
        /// </param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public SynchronizedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> items, object sync)
            : base(items: items,
                   sync: sync)
        {
        }

        #endregion Constructors (5)

        #region Properties (12)

        /// <summary>
        /// Gets the base dictionary.
        /// </summary>
        public IDictionary<TKey, TValue> BaseDictionary
        {
            get { return (IDictionary<TKey, TValue>)this.BaseCollection; }
        }

        /// <inheriteddoc />
        public bool IsFixedSize
        {
            get
            {
                return this.IfDictionary((d) => d.IsFixedSize,
                                         (d) => d.IsReadOnly);
            }
        }

        /// <inheriteddoc />
        public SynchronizedCollection<TKey> Keys
        {
            get
            {
                return this.InvokeForBaseDictionary((dict, state) => new SynchronizedCollection<TKey>(dict.Keys,
                                                                                                      state.Dictionary),
                                                    new
                                                    {
                                                        Dictionary = this,
                                                    });
            }
        }

        /// <inheriteddoc />
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get { return this.Keys; }
        }

        /// <inheriteddoc />
        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return this.Keys; }
        }

        /// <inheriteddoc />
        ICollection IDictionary.Keys
        {
            get { return this.Keys; }
        }

        /// <inheriteddoc />
        public SynchronizedCollection<TValue> Values
        {
            get
            {
                return this.InvokeForBaseDictionary((dict, state) => new SynchronizedCollection<TValue>(dict.Values,
                                                                                                        state.Dictionary),
                                                    new
                                                    {
                                                        Dictionary = this,
                                                    });
            }
        }

        /// <inheriteddoc />
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get { return this.Values; }
        }

        /// <inheriteddoc />
        ICollection IDictionary.Values
        {
            get { return this.Values; }
        }

        /// <inheriteddoc />
        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return this.Values; }
        }

        /// <inheriteddoc />
        public TValue this[TKey key]
        {
            get
            {
                return this.InvokeForBaseDictionary((dict, state) => dict[state.Key],
                                                    new
                                                    {
                                                        Key = key,
                                                    });
            }

            set
            {
                this.InvokeForBaseDictionary((dict, state) =>
                                             {
                                                 dict[state.Key] = state.NewValue;
                                             },
                                             new
                                             {
                                                 Key = key,
                                                 NewValue = value,
                                             });
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this.IfDictionary((d, s) => d[s.Key],
                                         (d, s) =>
                                         {
                                             var sd = s.Dictionary;

                                             return (object)sd[sd.ChangeObject<TKey>(s.Key)];
                                         },
                                         new
                                         {
                                             Dictionary = this,
                                             Key = key,
                                         });
            }

            set
            {
                this.IfDictionary((d, s) =>
                                  {
                                      d[s.Key] = s.NewValue;
                                  },
                                  (d, s) =>
                                  {
                                      var sd = s.Dictionary;

                                      sd[sd.ChangeObject<TKey>(s.Key)] = sd.ChangeObject<TValue>(s.NewValue);
                                  },
                                  new
                                  {
                                      Dictionary = this,
                                      Key = key,
                                      NewValue = value,
                                  });
            }
        }

        #endregion Properties (12)

        #region Methods (12)

        /// <inheriteddoc />
        public void Add(TKey key, TValue value)
        {
            this.InvokeForBaseDictionary((dict, state) => dict.Add(state.Key,
                                                                   state.Value),
                                         new
                                         {
                                             Key = key,
                                             Value = value,
                                         });
        }

        /// <inheriteddoc />
        void IDictionary.Add(object key, object value)
        {
            this.IfDictionary((d, s) => d.Add(s.Key, s.Value),
                              (d, s) =>
                              {
                                  var sd = s.Dictionary;

                                  sd.Add(key: sd.ChangeObject<TKey>(s.Key),
                                         value: sd.ChangeObject<TValue>(s.Value));
                              },
                              new
                              {
                                  Dictionary = this,
                                  Key = key,
                                  Value = value,
                              });
        }

        /// <inheriteddoc />
        bool IDictionary.Contains(object key)
        {
            return this.IfDictionary((d, s) => d.Contains(s.Key),
                                     (d, s) =>
                                     {
                                         var sd = s.Dictionary;

                                         return sd.ContainsKey(key: sd.ChangeObject<TKey>(s.Key));
                                     },
                                     new
                                     {
                                         Dictionary = this,
                                         Key = key,
                                     });
        }

        /// <inheriteddoc />
        public bool ContainsKey(TKey key)
        {
            return this.InvokeForBaseDictionary((dict, state) => dict.ContainsKey(state.Key),
                                                new
                                                {
                                                    Key = key,
                                                });
        }

        /// <inheriteddoc />
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator<TKey, TValue>(this.GetEnumerator(),
                                                          DictionaryEnumerator<TKey, TValue>.EnumeratorMode.IDictionary);
        }

        private void IfDictionary(Action<IDictionary> isTrueAction,
                                  Action<IDictionary<TKey, TValue>> isFalseAction)
        {
            this.IfDictionary(isTrueAction: (d, f) => f.IsTrue(d),
                              isFalseAction: (d, f) => f.IsFalse(d),
                              actionState: new
                                  {
                                      IsFalse = isFalseAction,
                                      IsTrue = isTrueAction,
                                  });
        }

        private void IfDictionary<TState>(Action<IDictionary, TState> isTrueAction,
                                          Action<IDictionary<TKey, TValue>, TState> isFalseAction,
                                          TState actionState)
        {
            this.IfDictionary(
                isTrueFunc: (d, s) =>
                    {
                        s.IsTrue(d,
                                 s.ActionState);

                        return (object)null;
                    },
                isFalseFunc: (d, s) =>
                    {
                        s.IsFalse(d,
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

        private TResult IfDictionary<TResult>(Func<IDictionary, TResult> isTrueFunc,
                                              Func<IDictionary<TKey, TValue>, TResult> isFalseFunc)
        {
            return this.IfDictionary(isTrueFunc: (d, f) => f.IsTrue(d),
                                     isFalseFunc: (d, f) => f.IsFalse(d),
                                     funcState: new
                                         {
                                             IsFalse = isFalseFunc,
                                             IsTrue = isTrueFunc,
                                         });
        }

        private TResult IfDictionary<TState, TResult>(Func<IDictionary, TState, TResult> isTrueFunc,
                                                      Func<IDictionary<TKey, TValue>, TState, TResult> isFalseFunc,
                                                      TState funcState)
        {
            return this.InvokeForBaseDictionary(
                func: (dict, state) =>
                    {
                        var d = dict as IDictionary;

                        return (d != null) ? state.IsTrue(d, state.FuncState)
                                           : state.IsFalse(dict, state.FuncState);
                    },
                funcState: new
                    {
                        FuncState = funcState,
                        IsFalse = isFalseFunc,
                        IsTrue = isTrueFunc,
                    });
        }

        private object InvokeForBaseDictionary(object func, object funcState)
        {
            throw new NotImplementedException();
        }

        private void InvokeForBaseDictionary(Action<IDictionary<TKey, TValue>> action)
        {
            this.InvokeForBaseDictionary(action: (d, a) => a(d),
                                         actionState: action);
        }

        private void InvokeForBaseDictionary<TState>(Action<IDictionary<TKey, TValue>, TState> action,
                                                     TState actionState)
        {
            this.InvokeForBaseDictionary(func: (dict, state) =>
                                         {
                                             state.Action(dict,
                                                          state.ActionState);

                                             return (object)null;
                                         }, funcState: new
                                         {
                                             Action = action,
                                             ActionState = actionState,
                                         });
        }

        private TResult InvokeForBaseDictionary<TResult>(Func<IDictionary<TKey, TValue>, TResult> func)
        {
            return this.InvokeForBaseDictionary(func: (d, f) => f(d),
                                                funcState: func);
        }

        private TResult InvokeForBaseDictionary<TState, TResult>(Func<IDictionary<TKey, TValue>, TState, TResult> func,
                                                                 TState funcState)
        {
            TResult result;

            lock (this._SYNC)
            {
                result = func(this.BaseDictionary,
                              funcState);
            }

            return result;
        }

        /// <inheriteddoc />
        protected override sealed IEnumerable<KeyValuePair<TKey, TValue>> PrepareInitialItems(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            var result = items as IDictionary<TKey, TValue>;
            if (result == null)
            {
                result = new Dictionary<TKey, TValue>();

                items.ForEach(ctx => ctx.State.BaseDictionary
                                              .Add(ctx.Item.Key, ctx.Item.Value),
                    new
                    {
                        BaseDictionary = result,
                    });
            }

            return result;
        }

        /// <inheriteddoc />
        public bool Remove(TKey key)
        {
            return this.InvokeForBaseDictionary((dict, state) => dict.Remove(state.Key),
                                                new
                                                {
                                                    Key = key,
                                                });
        }

        void IDictionary.Remove(object key)
        {
            this.IfDictionary((d, s) => d.Remove(s.Key),
                              (d, s) =>
                              {
                                  var sd = s.Dictionary;

                                  sd.Remove(key: sd.ChangeObject<TKey>(key));
                              },
                              new
                              {
                                  Dictionary = this,
                                  Key = key,
                              });
        }

        /// <inheriteddoc />
        public bool TryGetValue(TKey key, out TValue value)
        {
            bool result = false;

            value = this.InvokeForBaseDictionary((dict, state) =>
                                                 {
                                                     TValue v;
                                                     result = dict.TryGetValue(state.Key, out v);

                                                     return v;
                                                 },
                                                 new
                                                 {
                                                     Key = key,
                                                 });
            return result;
        }

        #endregion Methods (12)
    }

    #endregion CLASS: SynchronizedDictionary<TKey, TValue>

    #region CLASS: SynchronizedDictionary

    /// <summary>
    /// Factory class for <see cref="SynchronizedDictionary{TKey, TValue}" />.
    /// </summary>
    public static class SynchronizedDictionary
    {
        #region Methods (5)

        /// <summary>
        /// Creates a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <returns>The new instance.</returns>
        public static SynchronizedDictionary<TKey, TValue> Create<TKey, TValue>()
        {
            return new SynchronizedDictionary<TKey, TValue>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="keyComparer">
        /// The key comparer to use.
        /// </param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyComparer" /> is <see langword="null" />.
        /// </exception>
        public static SynchronizedDictionary<TKey, TValue> Create<TKey, TValue>(IEqualityComparer<TKey> keyComparer)
        {
            return new SynchronizedDictionary<TKey, TValue>(keyComparer: keyComparer);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="keyComparer">
        /// The key comparer to use.
        /// </param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keyComparer" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static SynchronizedDictionary<TKey, TValue> Create<TKey, TValue>(IEqualityComparer<TKey> keyComparer, object sync)
        {
            return new SynchronizedDictionary<TKey, TValue>(keyComparer: keyComparer,
                                                            sync: sync);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="items">
        /// The initial items / dictionary.
        /// </param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is <see langword="null" />.
        /// </exception>
        public static SynchronizedDictionary<TKey, TValue> Create<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            return new SynchronizedDictionary<TKey, TValue>(items: items);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SynchronizedDictionary{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="items">
        /// The initial items / dictionary.
        /// </param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static SynchronizedDictionary<TKey, TValue> Create<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items, object sync)
        {
            return new SynchronizedDictionary<TKey, TValue>(items: items,
                                                            sync: sync);
        }

        #endregion Methods (5)
    }

    #endregion CLASS: SynchronizedDictionary
}