// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    /// <summary>
    /// A thread safe list.
    /// </summary>
    /// <typeparam name="T">Type of the items.</typeparam>
    public class SynchronizedList<T> : SynchronizedCollection<T>,
                                       IList<T>, IReadOnlyList<T>,
                                       IList
    {
        #region Constructors (3)

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedList{T}" /> class.
        /// </summary>
        public SynchronizedList()
            : this(items: new List<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedList{T}" /> class.
        /// </summary>
        /// <param name="items">
        /// The initial items / list.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> is <see langword="null" />.
        /// </exception>
        public SynchronizedList(IEnumerable<T> items)
            : this(items: items,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedList{T}" /> class.
        /// </summary>
        /// <param name="items">
        /// The initial items / list.
        /// </param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public SynchronizedList(IEnumerable<T> items, object sync)
            : base(items: items,
                   sync: sync)
        {
        }

        #endregion Constructors (3)

        #region Properties (4)

        /// <summary>
        /// Gets the base list.
        /// </summary>
        public IList<T> BaseList
        {
            get { return (IList<T>)this.BaseCollection; }
        }

        /// <inheriteddoc />
        public bool IsFixedSize
        {
            get
            {
                return this.IfList((l) => l.IsFixedSize,
                                   (l) => l.IsReadOnly);
            }
        }

        /// <inheriteddoc />
        public T this[int index]
        {
            get
            {
                return this.InvokeForBaseList((l, s) => l[s.Index],
                                              new
                                              {
                                                  Index = index,
                                              });
            }

            set
            {
                this.InvokeForBaseList((l, s) =>
                {
                    l[s.Index] = s.NewValue;
                },
                                       new
                                       {
                                           Index = index,
                                           NewValue = value,
                                       });
            }
        }

        /// <inheriteddoc />
        object IList.this[int index]
        {
            get
            {
                return this.IfList((l, s) => l[s.Index],
                                   (l, s) => l[s.Index],
                                   new
                                   {
                                       Index = index,
                                   });
            }

            set
            {
                this.IfList((l, s) =>
                {
                    l[s.Index] = s.NewValue;
                },
                            (l, s) =>
                            {
                                l[s.Index] = s.List
                                              .ChangeObject<T>(s.NewValue);
                            },
                            new
                            {
                                Index = index,
                                List = this,
                                NewValue = value,
                            });
            }
        }

        #endregion Properties (4)

        #region Methods (16)

        /// <inheriteddoc />
        int IList.Add(object value)
        {
            return this.IfList((l, s) => l.Add(s.Value),
                               (l, s) =>
                               {
                                   l.Add(s.List
                                          .ChangeObject<T>(s.Value));

                                   return l.Count - 1;
                               },
                               new
                               {
                                   List = this,
                                   Value = value,
                               });
        }

        /// <inheriteddoc />
        bool IList.Contains(object value)
        {
            return this.IfList((l, s) => l.Contains(s.Value),
                               (l, s) => l.Contains(s.List
                                                     .ChangeObject<T>(s.Value)),
                               new
                               {
                                   List = this,
                                   Value = value,
                               });
        }

        private void IfList(Action<IList> isTrueAction,
                            Action<IList<T>> isFalseAction)
        {
            this.IfList(isTrueAction: (l, f) => f.IsTrue(l),
                        isFalseAction: (l, f) => f.IsFalse(l),
                        actionState: new
                            {
                                IsFalse = isFalseAction,
                                IsTrue = isTrueAction,
                            });
        }

        private void IfList<TState>(Action<IList, TState> isTrueAction,
                                    Action<IList<T>, TState> isFalseAction,
                                    TState actionState)
        {
            this.IfList(
                isTrueFunc: (l, s) =>
                    {
                        s.IsTrue(l,
                                 s.ActionState);

                        return (object)null;
                    },
                isFalseFunc: (l, s) =>
                    {
                        s.IsFalse(l,
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

        private TResult IfList<TResult>(Func<IList, TResult> isTrueFunc,
                                        Func<IList<T>, TResult> isFalseFunc)
        {
            return this.IfList(isTrueFunc: (l, f) => f.IsTrue(l),
                               isFalseFunc: (l, f) => f.IsFalse(l),
                               funcState: new
                                   {
                                       IsFalse = isFalseFunc,
                                       IsTrue = isTrueFunc,
                                   });
        }

        private TResult IfList<TState, TResult>(Func<IList, TState, TResult> isTrueFunc,
                                                Func<IList<T>, TState, TResult> isFalseFunc,
                                                TState funcState)
        {
            return this.InvokeForBaseList(
                func: (list, state) =>
                    {
                        var l = list as IList;

                        return (l != null) ? state.IsTrue(l, state.FuncState)
                                           : state.IsFalse(list, state.FuncState);
                    },
                funcState: new
                    {
                        FuncState = funcState,
                        IsFalse = isFalseFunc,
                        IsTrue = isTrueFunc,
                    });
        }

        /// <inheriteddoc />
        public int IndexOf(T item)
        {
            return this.InvokeForBaseList((l, s) => l.IndexOf(s.Item),
                                          new
                                          {
                                              Item = item,
                                          });
        }

        /// <inheriteddoc />
        int IList.IndexOf(object value)
        {
            return this.IfList((l, s) => l.IndexOf(s.Value),
                               (l, s) => l.IndexOf(s.List
                                                    .ChangeObject<T>(s.Value)),
                               new
                               {
                                   List = this,
                                   Value = value,
                               });
        }

        /// <inheriteddoc />
        public void Insert(int index, T item)
        {
            this.InvokeForBaseList((l, s) => l.Insert(s.Index, s.Item),
                                   new
                                   {
                                       Index = index,
                                       Item = item,
                                   });
        }

        /// <inheriteddoc />
        void IList.Insert(int index, object value)
        {
            this.IfList((l, s) => l.Insert(s.Index, s.Value),
                        (l, s) => l.Insert(s.Index, s.List
                                                     .ChangeObject<T>(s.Value)),
                        new
                        {
                            Index = index,
                            List = this,
                            Value = value,
                        });
        }

        private void InvokeForBaseList(Action<IList<T>> action)
        {
            this.InvokeForBaseList(action: (l, a) => a(l),
                                   actionState: action);
        }

        private void InvokeForBaseList<TState>(Action<IList<T>, TState> action,
                                               TState actionState)
        {
            this.InvokeForBaseList(func: (list, state) =>
                                   {
                                       state.Action(list,
                                                    state.ActionState);

                                       return (object)null;
                                   }, funcState: new
                                   {
                                       Action = action,
                                       ActionState = actionState,
                                   });
        }

        private TResult InvokeForBaseList<TResult>(Func<IList<T>, TResult> func)
        {
            return this.InvokeForBaseList(func: (l, f) => f(l),
                                          funcState: func);
        }

        private TResult InvokeForBaseList<TState, TResult>(Func<IList<T>, TState, TResult> func,
                                                           TState funcState)
        {
            TResult result;

            lock (this._SYNC)
            {
                result = func(this.BaseList,
                              funcState);
            }

            return result;
        }

        /// <inheriteddoc />
        protected override sealed IEnumerable<T> PrepareInitialItems(IEnumerable<T> items)
        {
            var result = items as IList<T>;
            if (result == null)
            {
                result = new List<T>();

                result.AddRange(items);
            }

            return result;
        }

        /// <inheriteddoc />
        void IList.Remove(object value)
        {
            this.IfList((l, s) => l.Remove(s.Value),
                        (l, s) => l.Remove(s.List
                                            .ChangeObject<T>(s.Value)),
                        new
                        {
                            List = this,
                            Value = value,
                        });
        }

        /// <inheriteddoc />
        public void RemoveAt(int index)
        {
            this.InvokeForBaseList((l, s) => l.RemoveAt(s.Index),
                                   new
                                   {
                                       Index = index,
                                   });
        }

        #endregion Methods (16)
    }
}