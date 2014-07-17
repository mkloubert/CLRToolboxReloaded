// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data;
using System;
using System.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Data
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (6)

        /// <summary>
        /// Begins a transaction based on a <see cref="IDbConnection" /> and invokes an action inside that transaction.
        /// </summary>
        /// <param name="conn">The connection to use.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The result of <paramref name="action" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static void InvokeForTransaction(this IDbConnection conn, Action<IDbTransactionContext> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            InvokeForTransaction<Action<IDbTransactionContext>>(conn: conn,
                                                                action: (ctx) => ctx.State(ctx),
                                                                actionState: action);
        }

        /// <summary>
        /// Begins a transaction based on a <see cref="IDbConnection" /> and invokes an action inside that transaction.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the execution context of <paramref name="action" />.</typeparam>
        /// <param name="conn">The connection to use.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">
        /// The state object for the execution context of <paramref name="action" />.
        /// </param>
        /// <returns>The result of <paramref name="action" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static void InvokeForTransaction<TState>(this IDbConnection conn,
                                                        Action<IDbTransactionContext<TState>> action, TState actionState)
        {
            InvokeForTransaction<TState>(conn: conn,
                                         action: action,
                                         actionStateProvider: (trans) => actionState);
        }

        /// <summary>
        /// Begins a transaction based on a <see cref="IDbConnection" /> and invokes an action inside that transaction.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the execution context of <paramref name="action" />.</typeparam>
        /// <param name="conn">The connection to use.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">
        /// The function that provides the state object for the execution context of <paramref name="action" />.
        /// </param>
        /// <returns>The result of <paramref name="action" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" />, <paramref name="action" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        public static void InvokeForTransaction<TState>(this IDbConnection conn,
                                                        Action<IDbTransactionContext<TState>> action, Func<IDbTransaction, TState> actionStateProvider)
        {
            InvokeForTransaction<TState, object>(conn: conn,
                                                 func: (ctx) =>
                                                     {
                                                         action(ctx);
                                                         return (object)null;
                                                     },
                                                 funcStateProvider: actionStateProvider);
        }

        /// <summary>
        /// Begins a transaction based on a <see cref="IDbConnection" /> and invokes a function inside that transaction.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="conn">The connection to use.</param>
        /// <param name="func">The function to invoke.</param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static TResult InvokeForTransaction<TResult>(this IDbConnection conn,
                                                            Func<IDbTransactionContext, TResult> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return InvokeForTransaction<Func<IDbTransactionContext, TResult>, TResult>(conn: conn,
                                                                                       func: (ctx) => ctx.State(ctx),
                                                                                       funcState: func);
        }

        /// <summary>
        /// Begins a transaction based on a <see cref="IDbConnection" /> and invokes a function inside that transaction.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the execution context of <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="conn">The connection to use.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcState">
        /// The state object for the execution context of <paramref name="func" />.
        /// </param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> and/or <paramref name="func" /> are <see langword="null" />.
        /// </exception>
        public static TResult InvokeForTransaction<TState, TResult>(this IDbConnection conn,
                                                                    Func<IDbTransactionContext<TState>, TResult> func, TState funcState)
        {
            return InvokeForTransaction<TState, TResult>(conn: conn,
                                                         func: func,
                                                         funcStateProvider: (trans) => funcState);
        }

        /// <summary>
        /// Begins a transaction based on a <see cref="IDbConnection" /> and invokes a function inside that transaction.
        /// </summary>
        /// <typeparam name="TState">Type of the state object for the execution context of <paramref name="func" />.</typeparam>
        /// <typeparam name="TResult">Type of the result of <paramref name="func" />.</typeparam>
        /// <param name="conn">The connection to use.</param>
        /// <param name="func">The function to invoke.</param>
        /// <param name="funcStateProvider">
        /// The function that provides the state object for the execution context of <paramref name="func" />.
        /// </param>
        /// <returns>The result of <paramref name="func" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" />, <paramref name="func" /> and/or <paramref name="funcStateProvider" /> are <see langword="null" />.
        /// </exception>
        public static TResult InvokeForTransaction<TState, TResult>(this IDbConnection conn,
                                                                    Func<IDbTransactionContext<TState>, TResult> func, Func<IDbTransaction, TState> funcStateProvider)
        {
            if (conn == null)
            {
                throw new ArgumentNullException("conn");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            if (funcStateProvider == null)
            {
                throw new ArgumentNullException("funcStateProvider");
            }

            var trans = conn.BeginTransaction();

            var ctx = new DbTransactionContext<TState>();
            ctx.Commit = true;
            ctx.RollbackOnFailure = true;
            ctx.Transaction = trans;

            TResult result;

            try
            {
                ctx.State = funcStateProvider(trans);

                result = func(ctx);

                if (ctx.Rollback)
                {
                    trans.Rollback();
                }
                else
                {
                    if (ctx.Commit)
                    {
                        trans.Commit();
                    }
                }
            }
            catch
            {
                if (ctx.RollbackOnFailure)
                {
                    // rollback before rethrow exception
                    trans.Rollback();
                }

                throw;
            }

            return result;
        }

        #endregion Methods (6)
    }
}