// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.ObjectModel
{
    /// <summary>
    /// A thread safe version of <see cref="ObservableCollection{T}" />.
    /// </summary>
    /// <typeparam name="T">Type of the items.</typeparam>
    public class SynchronizedObservableCollection<T> : ObservableCollection<T>
    {
        #region Fields (2)

        private bool _isEditing;

        /// <summary>
        /// An uniue object for sync operations.
        /// </summary>
        protected readonly object _SYNC;

        #endregion Fields

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedObservableCollection{T}" /> class.
        /// </summary>
        /// <param name="syncRoot">The value for <see cref="SynchronizedObservableCollection{T}._SYNC" /> field..</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="syncRoot" /> is <see langword="null" />.
        /// </exception>
        public SynchronizedObservableCollection(object syncRoot)
        {
            if (syncRoot == null)
            {
                throw new ArgumentNullException("syncRoot");
            }

            this._SYNC = syncRoot;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedObservableCollection{T}" /> class.
        /// </summary>
        public SynchronizedObservableCollection()
            : this(new object())
        {
        }

        #endregion Constructors

        #region Properties (1)

        /// <summary>
        /// Gets if that collection is in edit mode or not.
        /// </summary>
        public bool IsEditing
        {
            get { return this._isEditing; }

            private set
            {
                if (this._isEditing == value)
                {
                    return;
                }

                this._isEditing = value;
                base.OnPropertyChanged(new PropertyChangedEventArgs("IsEditing"));
            }
        }

        #endregion

        #region Methods (18)

        /// <summary>
        /// Adds a list of items.
        /// </summary>
        /// <param name="itemsToAdd">The items to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="itemsToAdd" /> is <see langword="null" />.
        /// </exception>
        public void AddRange(IEnumerable<T> itemsToAdd)
        {
            if (itemsToAdd == null)
            {
                throw new ArgumentNullException("itemsToAdd");
            }

            itemsToAdd.ForEach(ctx =>
                               {
                                   ctx.State
                                      .Collection.Add(ctx.Item);
                               },
                               new
                               {
                                   Collection = this,
                               });
        }

        /// <summary>
        /// Sets that collection in 'edit mode', what means that raising <see cref="ObservableCollection{T}.CollectionChanged" /> and
        /// <see cref="ObservableCollection{T}.PropertyChanged" /> events are disabled until
        /// <see cref="SynchronizedObservableCollection{T}.EndEdit(bool)" /> method is called.
        /// </summary>
        public void BeginEdit()
        {
            this.InvokeForCollection((coll) => coll.IsEditing = true);
        }

        /// <inheriteddoc />
        protected override void ClearItems()
        {
            this.InvokeForCollection((coll) => base.ClearItems());
        }

        /// <summary>
        /// Creates an action wrapper for that collection that is invoked thread safe.
        /// </summary>
        /// <typeparam name="TState">Type of the second parameter for <paramref name="action" />.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">
        /// The function that returns the state object (2nd parameter) for <paramref name="action" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        protected Action CreateSyncAction<TState>(Action<SynchronizedObservableCollection<T>, TState> action,
                                                  Func<SynchronizedObservableCollection<T>, TState> actionStateProvider)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            return () =>
                {
                    lock (this._SYNC)
                    {
                        action(this,
                               actionStateProvider(this));
                    }
                };
        }

        /// <summary>
        /// Invokes an action for that collection.
        /// While the action is running, that collection is switched to edit mode.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="raiseEvents">Raise collection events after invokation or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        public void Edit(Action<SynchronizedObservableCollection<T>> action,
                         bool raiseEvents = true)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.Edit<Action<SynchronizedObservableCollection<T>>>(action: (c, a) => a(c),
                                                                   actionState: action,
                                                                   raiseEvents: raiseEvents);
        }

        /// <summary>
        /// Invokes an action for that collection.
        /// While the action is running, that collection is switched to edit mode.
        /// </summary>
        /// <typeparam name="TState">Type of the second parameter for <paramref name="action" />.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">
        /// The state object (2nd parameter) for <paramref name="action" />.
        /// </param>
        /// <param name="raiseEvents">Raise collection events after invokation or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        public void Edit<TState>(Action<SynchronizedObservableCollection<T>, TState> action,
                                 TState actionState,
                                 bool raiseEvents = true)
        {
            this.Edit<TState>(action: action,
                              actionStateProvider: (coll) => actionState,
                              raiseEvents: raiseEvents);
        }

        /// <summary>
        /// Invokes an action for that collection.
        /// While the action is running, that collection is switched to edit mode.
        /// </summary>
        /// <typeparam name="TState">Type of the second parameter for <paramref name="action" />.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">
        /// The function that returns the state object (2nd parameter) for <paramref name="action" />.
        /// </param>
        /// <param name="raiseEvents">Raise collection events after invokation or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        public void Edit<TState>(Action<SynchronizedObservableCollection<T>, TState> action,
                                 Func<SynchronizedObservableCollection<T>, TState> actionStateProvider,
                                 bool raiseEvents = true)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            this.InvokeForCollection(
                action:
                    (coll, state) =>
                    {
                        try
                        {
                            coll.IsEditing = true;

                            state.Action(coll,
                                         state.StateProvider(coll));
                        }
                        finally
                        {
                            coll.IsEditing = false;
                        }
                    },
                actionState: new
                    {
                        Action = action,
                        StateProvider = actionStateProvider,
                    });
        }

        /// <summary>
        /// Unsets that collection from 'edit mode'.
        /// </summary>
        /// <param name="raiseEvents">Raise events or not.</param>
        public void EndEdit(bool raiseEvents = true)
        {
            this.InvokeForCollection(
                action: (coll, state) =>
                    {
                        coll.IsEditing = false;

                        if (state.RaiseEvents)
                        {
                            coll.RaiseCollectionEvents();
                        }
                    },
                actionState: new
                    {
                        RaiseEvents = raiseEvents,
                    });
        }

        /// <inheriteddoc />
        protected override void InsertItem(int index, T item)
        {
            this.InvokeForCollection((coll, state) => base.InsertItem(state.Index, state.Item),
                                     new
                                     {
                                         Index = index,
                                         Item = item,
                                     });
        }

        /// <summary>
        /// Invokes an action for that collection.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        protected void InvokeForCollection(Action<SynchronizedObservableCollection<T>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.InvokeForCollection<Action<SynchronizedObservableCollection<T>>>(action: (coll, a) => a(coll),
                                                                                  actionState: action);
        }

        /// <summary>
        /// Invokes an action for that collection.
        /// </summary>
        /// <typeparam name="TState">Type of the second parameter for <paramref name="action" />.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">
        /// The state object (2nd parameter) for <paramref name="action" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        protected void InvokeForCollection<TState>(Action<SynchronizedObservableCollection<T>, TState> action,
                                                   TState actionState)
        {
            this.InvokeForCollection<TState>(action: action,
                                             actionStateProvider: (coll) => actionState);
        }

        /// <summary>
        /// Invokes an action for that collection.
        /// </summary>
        /// <typeparam name="TState">Type of the second parameter for <paramref name="action" />.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">
        /// The function that returns the state object (2nd parameter) for <paramref name="action" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        protected virtual void InvokeForCollection<TState>(Action<SynchronizedObservableCollection<T>, TState> action,
                                                           Func<SynchronizedObservableCollection<T>, TState> actionStateProvider)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            var syncAction = this.CreateSyncAction<TState>(action: action,
                                                           actionStateProvider: actionStateProvider);

            syncAction();
        }

        /// <inheriteddoc />
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            this.InvokeForCollection((coll, state) => base.MoveItem(state.OldIndex, state.NewIndex),
                                     new
                                     {
                                         OldIndex = oldIndex,
                                         NewIndex = newIndex,
                                     });
        }

        /// <inheriteddoc />
        protected override sealed void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.InvokeForCollection((coll, state) =>
                {
                    if (coll.IsEditing)
                    {
                        return;
                    }

                    base.OnCollectionChanged(state.Arguments);
                }, new
                {
                    Arguments = e,
                });
        }

        /// <inheriteddoc />
        protected override sealed void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.InvokeForCollection((coll, state) =>
                {
                    if (coll.IsEditing)
                    {
                        return;
                    }

                    base.OnPropertyChanged(state.Arguments);
                }, new
                {
                    Arguments = e,
                });
        }

        private void RaiseCollectionEvents()
        {
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            base.OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            base.OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }

        /// <inheriteddoc />
        protected override void RemoveItem(int index)
        {
            this.InvokeForCollection((coll, state) => base.RemoveItem(state.Index),
                                     new
                                     {
                                         Index = index,
                                     });
        }

        /// <inheriteddoc />
        protected override void SetItem(int index, T item)
        {
            this.InvokeForCollection((coll, state) => base.SetItem(state.Index, state.Item),
                                     new
                                     {
                                         Index = index,
                                         Item = item,
                                     });
        }

        #endregion Methods
    }
}