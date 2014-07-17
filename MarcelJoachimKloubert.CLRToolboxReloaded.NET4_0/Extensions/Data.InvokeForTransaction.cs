// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data;
using System;
using System.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Begins a transaction based on a <see cref="IDbConnection" />.
        /// </summary>
        /// <typeparam name="TConn">Type of the connection.</typeparam>
        /// <param name="conn">The connection to use.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>
        /// The value of <paramref name="conn" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static TConn InvokeForTransaction<TConn>(this TConn conn, Action<IDbTransactionContext> action)
            where TConn : global::System.Data.IDbConnection
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return InvokeForTransaction<TConn, Action<IDbTransactionContext>>(conn: conn,
                                                                              action: (ctx) => ctx.State(ctx),
                                                                              actionState: action);
        }

        /// <summary>
        /// Begins a transaction based on a <see cref="IDbConnection" />.
        /// </summary>
        /// <typeparam name="TConn">Type of the connection.</typeparam>
        /// <typeparam name="TState">Type of the state object for the execution context.</typeparam>
        /// <param name="conn">The connection to use.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">The state object for the execution context.</param>
        /// <returns>
        /// The value of <paramref name="conn" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> and/or <paramref name="action" /> are <see langword="null" />.
        /// </exception>
        public static TConn InvokeForTransaction<TConn, TState>(this TConn conn,
                                                                Action<IDbTransactionContext<TState>> action, TState actionState)
            where TConn : global::System.Data.IDbConnection
        {
            return InvokeForTransaction<TConn, TState>(conn: conn,
                                                       action: action,
                                                       actionStateProvider: (trans) => actionState);
        }

        /// <summary>
        /// Begins a transaction based on a <see cref="IDbConnection" />.
        /// </summary>
        /// <typeparam name="TConn">Type of the connection.</typeparam>
        /// <typeparam name="TState">Type of the state object for the execution context.</typeparam>
        /// <param name="conn">The connection to use.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">
        /// The function that provides the state object for the execution context.
        /// </param>
        /// <returns>
        /// The value of <paramref name="conn" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" />, <paramref name="action" /> and/or <paramref name="actionStateProvider" /> are <see langword="null" />.
        /// </exception>
        public static TConn InvokeForTransaction<TConn, TState>(this TConn conn,
                                                                Action<IDbTransactionContext<TState>> action, Func<IDbTransaction, TState> actionStateProvider)
            where TConn : global::System.Data.IDbConnection
        {
            if (conn == null)
            {
                throw new ArgumentNullException("conn");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            var trans = conn.BeginTransaction();

            var ctx = new DbTransactionContext<TState>();
            ctx.CommitOnSuccess = true;
            ctx.RollbackOnFailure = true;
            ctx.Transaction = trans;

            try
            {
                ctx.State = actionStateProvider(trans);

                action(ctx);

                if (ctx.CommitOnSuccess)
                {
                    trans.Commit();
                }
            }
            catch
            {
                if (ctx.RollbackOnFailure)
                {
                    trans.Rollback();
                }

                throw;
            }

            return conn;
        }

        #endregion Methods (3)
    }
}