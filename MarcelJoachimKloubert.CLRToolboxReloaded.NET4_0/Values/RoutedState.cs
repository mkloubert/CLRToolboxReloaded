// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Values
{
    /// <summary>
    /// A routed state that uses the highest or lowest value of its parents.
    /// </summary>
    /// <typeparam name="T">Type of the underlying value.</typeparam>
    public partial class RoutedState<T> : StateBase<T>
        where T : global::System.IComparable
    {
        #region Fields (4)

        private readonly HashSet<RoutedState<T>> _CHILDREN = new HashSet<RoutedState<T>>();
        private readonly Stradegy _STRADEGY;
        private readonly HashSet<RoutedState<T>> _PARENTS = new HashSet<RoutedState<T>>();
        private T _value;

        #endregion Fields (3)

        #region Constructors (2)

        /// <inheriteddoc />
        protected RoutedState(object sync, Stradegy stradegy = Stradegy.Ascending)
            : base(sync: sync,
                   isSynchronized: true)
        {
            this._STRADEGY = stradegy;
        }

        /// <inheriteddoc />
        protected RoutedState(Stradegy stradegy = Stradegy.Ascending)
            : this(sync: new object(),
                   stradegy: stradegy)
        {
        }

        #endregion Constructors (2)

        #region Events and delegates (1)

        private void ParentState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var otherState = (RoutedState<T>)sender;

            switch (e.PropertyName)
            {
                case "Value":
                    var compareValue = CompareValues(this.Value, otherState.Value);
                    switch (this.ValueStradegy)
                    {
                        case RoutedState<T>.Stradegy.Descending:
                            if (compareValue > 0)
                            {
                                this.OnPropertyChanged(e.PropertyName);
                            }
                            break;

                        default:
                            if (compareValue < 0)
                            {
                                this.OnPropertyChanged(e.PropertyName);
                            }
                            break;
                    }
                    break;
            }
        }

        #endregion Events and delegates (1)

        #region Properties (2)

        /// <inheriteddoc />
        public override T Value
        {
            get
            {
                T result = this._value;
                this.InvokeForLists((parents, children) =>
                    {
                        parents.ForEach(ctx =>
                            {
                                var parentValue = ctx.Item.Value;

                                var updateResultValue = false;

                                var compareValue = CompareValues(result, parentValue);
                                switch (this.ValueStradegy)
                                {
                                    case RoutedState<T>.Stradegy.Descending:
                                        updateResultValue = compareValue > 0;
                                        break;

                                    default:
                                        updateResultValue = compareValue < 0;
                                        break;
                                }

                                if (updateResultValue)
                                {
                                    result = parentValue;
                                }
                            });
                    });

                return result;
            }

            set
            {
                this._value = value;

                if (object.Equals(this.Value, value) == false)
                {
                    this.RaiseValueChanged();
                }
            }
        }

        /// <summary>
        /// Gets the value stradegy.
        /// </summary>
        public Stradegy ValueStradegy
        {
            get { return this._STRADEGY; }
        }

        #endregion Properties (2)

        #region Methods (13)

        /// <summary>
        /// Adds a child.
        /// </summary>
        /// <param name="state">The child to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="state" /> cannot be added as parent because it is already a child or it is the same instance.
        /// </exception>
        public void AddChild(RoutedState<T> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            if (object.ReferenceEquals(this, state))
            {
                throw new InvalidOperationException("Cannot add this instance as child!");
            }

            this.InvokeForLists((parents, children, actionState) =>
                {
                    if (parents.Contains(actionState.Child))
                    {
                        throw new InvalidOperationException("Child is already a parent!");
                    }

                    if (children.Add(actionState.Child) == false)
                    {
                        return;
                    }

                    actionState.Child.AddParent(actionState.ThisObject);
                }, new
                {
                    Child = state,
                    ThisObject = this,
                });
        }

        /// <summary>
        /// Adds a parent.
        /// </summary>
        /// <param name="state">The parent to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="state" /> cannot be added as parent because it is already a child or it is the same instance.
        /// </exception>
        public void AddParent(RoutedState<T> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            if (object.ReferenceEquals(this, state))
            {
                throw new InvalidOperationException("Cannot add this instance as parent!");
            }

            this.InvokeForLists((parents, children, actionState) =>
                {
                    if (children.Contains(actionState.Parent))
                    {
                        throw new InvalidOperationException("Parent is already a child!");
                    }

                    if (parents.Add(actionState.Parent) == false)
                    {
                        return;
                    }

                    actionState.Parent.AddChild(actionState.ThisObject);

                    actionState.Parent.PropertyChanged += actionState.ThisObject.ParentState_PropertyChanged;

                    actionState.ThisObject.RaiseValueChanged();
                }, new
                {
                    Parent = state,
                    ThisObject = this,
                });
        }

        /// <summary>
        /// Removes all parents and children states.
        /// </summary>
        public void Clear()
        {
            this.ClearParents();
            this.ClearChildren();
        }

        /// <summary>
        /// Removes all children.
        /// </summary>
        public void ClearChildren()
        {
            RoutedState<T>[] childrenToRemove = null;
            this.InvokeForLists((parents, children) => children.ToArray());

            childrenToRemove.ForAll((ctx) => ctx.State.ThisObject.RemoveChild(ctx.Item),
                                    new
                                    {
                                        ThisObject = this,
                                    });
        }

        /// <summary>
        /// Removes all parents.
        /// </summary>
        public void ClearParents()
        {
            RoutedState<T>[] parentsToRemove = null;
            this.InvokeForLists((parents, children) => parents.ToArray());

            parentsToRemove.ForAll((ctx) => ctx.State.ThisObject.RemoveParent(ctx.Item),
                                   new
                                   {
                                       ThisObject = this,
                                   });
        }

        private static int CompareValues(T x, T y)
        {
            if (x == null)
            {
                if (y != null)
                {
                    return y.CompareTo(x) * -1;
                }

                // x and y are (null)
                return 0;
            }

            return x.CompareTo(y);
        }

        private void InvokeForLists(Action<HashSet<RoutedState<T>>, HashSet<RoutedState<T>>> action)
        {
            this.InvokeForLists((parents, children, state) =>
                {
                    state.Action(parents, children);
                }, new
                {
                    Action = action,
                });
        }

        private void InvokeForLists<S>(Action<HashSet<RoutedState<T>>, HashSet<RoutedState<T>>, S> action, S actionState)
        {
            lock (this._SYNC)
            {
                action(this._PARENTS, this._CHILDREN,
                       actionState);
            }
        }

        private bool RaiseValueChanged()
        {
            return this.OnPropertyChanged("Value");
        }

        /// <summary>
        /// Removes a child.
        /// </summary>
        /// <param name="state">The child to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state" /> is <see langword="null" />.
        /// </exception>
        public void RemoveChild(RoutedState<T> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            this.InvokeForLists((parents, children, actionState) =>
                {
                    children.Remove(actionState.Child);

                    actionState.Child.RemoveParent(actionState.ThisObject);
                }, new
                {
                    Child = state,
                    ThisObject = this,
                });
        }

        /// <summary>
        /// Removes a parent.
        /// </summary>
        /// <param name="state">The parent to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="state" /> is <see langword="null" />.
        /// </exception>
        public void RemoveParent(RoutedState<T> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            this.InvokeForLists((parents, children, actionState) =>
                {
                    actionState.Parent.PropertyChanged -= actionState.ThisObject.ParentState_PropertyChanged;

                    parents.Remove(actionState.Parent);
                    actionState.Parent.RemoveChild(actionState.ThisObject);
                }, new
                {
                    Parent = state,
                    ThisObject = this,
                });
        }

        #endregion Methods (13)
    }
}