// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Data
{
    /// <summary>
    /// A basic database connection.
    /// </summary>
    public abstract class DatabaseBase : DisposableObjectBase, IDatabase
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected DatabaseBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected DatabaseBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected DatabaseBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected DatabaseBase()
            : base()
        {
        }

        #endregion Constructors

        #region Properties (1)

        /// <inheriteddoc />
        public abstract bool CanUpdate
        {
            get;
        }

        #endregion Properties

        #region Methods (8)

        /// <summary>
        /// Invokes an action for that database.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        protected void InvokeDatabaseAction(Action<DatabaseBase> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.InvokeDatabaseAction<Action<DatabaseBase>>((db, a) => a(db),
                                                            actionState: action);
        }

        /// <summary>
        /// Invokes an action for that database.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for <paramref name="action" />.</typeparam>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">The state object for <paramref name="action" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action" /> is <see langword="null" />.
        /// </exception>
        protected void InvokeDatabaseAction<TState>(Action<DatabaseBase, TState> action,
                                                    TState actionState)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.InvokeDatabaseFunc((db, state) =>
            {
                state.Action(db,
                             state.ActionState);

                return (object)null;
            }, funcState: new
            {
                Action = action,
                ActionState = actionState,
            });
        }

        /// <summary>
        /// Invokes a function for that database.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> is <see langword="null" />.
        /// </exception>
        protected TResult InvokeDatabaseFunc<TResult>(Func<DatabaseBase, TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return this.InvokeDatabaseFunc<Func<DatabaseBase, TResult>, TResult>((db, f) => f(db),
                                                                                 funcState: func);
        }

        /// <summary>
        /// Invokes a function for that database.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">The state object for <paramref name="func" />.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func" /> is <see langword="null" />.
        /// </exception>
        protected TResult InvokeDatabaseFunc<TState, TResult>(Func<DatabaseBase, TState, TResult> func,
                                                              TState funcState)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            Func<Func<DatabaseBase, TState, TResult>, TState, TResult> funcToInvoke;
            if (this._IS_SYNCHRONIZED)
            {
                funcToInvoke = this.InvokeDatabaseFunc_ThreadSafe;
            }
            else
            {
                funcToInvoke = this.InvokeDatabaseFunc_NonThreadSafe;
            }

            return funcToInvoke(func, funcState);
        }

        private TResult InvokeDatabaseFunc_NonThreadSafe<TState, TResult>(Func<DatabaseBase, TState, TResult> func,
                                                                          TState funcState)
        {
            return func(this, funcState);
        }

        private TResult InvokeDatabaseFunc_ThreadSafe<TState, TResult>(Func<DatabaseBase, TState, TResult> func,
                                                                       TState funcState)
        {
            TResult result;

            lock (this._SYNC)
            {
                result = this.InvokeDatabaseFunc_NonThreadSafe(func, funcState);
            }

            return result;
        }

        /// <inheriteddoc />
        protected virtual void OnUpdate()
        {
            throw new NotImplementedException();
        }

        /// <inheriteddoc />
        public void Update()
        {
            this.InvokeDatabaseAction((db) =>
                {
                    db.ThrowIfDisposed();

                    if (db.CanUpdate == false)
                    {
                        throw new InvalidOperationException();
                    }

                    db.OnUpdate();
                });
        }

        #endregion Methods
    }
}