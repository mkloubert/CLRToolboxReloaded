// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (6)

        /// <summary>
        /// Invokes an action for all items of a sequence even if one or more invokations fail.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>
        /// Should return <see langword="null" />; otherwise an <see cref="AggregateException" /> is thrown.
        /// </returns>
        /// <exception cref="AggregateException">At least one error occured.</exception>
        public static AggregateException ForAll<T>(this IEnumerable<T> seq,
                                                   Action<IForAllItemContext<T>> action)
        {
            return ForAll<T>(seq,
                             throwExceptions: true,
                             action: action);
        }

        /// <summary>
        /// Invokes an action for all items of a sequence even if one or more invokations fail.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="throwExceptions">
        /// If at least one exception was thrown while invokation throw (<see langword="true" />)
        /// or return (<see langword="false" />).
        /// </param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>
        /// The occured errors or <see langword="null" /> if no exception was thrown while invokation.
        /// </returns>
        /// <exception cref="AggregateException">At least one error occured.</exception>
        public static AggregateException ForAll<T>(this IEnumerable<T> seq,
                                                   bool throwExceptions,
                                                   Action<IForAllItemContext<T>> action)
        {
            return ForAll<T, Action<IForAllItemContext<T>>>(seq,
                                                            throwExceptions: throwExceptions,
                                                            action: (ctx) => ctx.State(ctx),
                                                            actionState: action);
        }

        /// <summary>
        /// Invokes an action for all items of a sequence even if one or more invokations fail.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <typeparam name="TState">
        /// Type of the state item for <paramref name="action" />.
        /// </typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">
        /// The state object for for <paramref name="action" />.
        /// </param>
        /// <returns>
        /// Should return <see langword="null" />; otherwise an <see cref="AggregateException" /> is thrown.
        /// </returns>
        /// <exception cref="AggregateException">At least one error occured.</exception>
        public static AggregateException ForAll<T, TState>(this IEnumerable<T> seq,
                                                           Action<IForAllItemContext<T, TState>> action,
                                                           TState actionState)
        {
            return ForAll<T, TState>(seq,
                                     action: action,
                                     actionState: actionState,
                                     throwExceptions: true);
        }

        /// <summary>
        /// Invokes an action for all items of a sequence even if one or more invokations fail.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <typeparam name="TState">
        /// Type of the state item for <paramref name="action" />.
        /// </typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="throwExceptions">
        /// If at least one exception was thrown while invokation throw (<see langword="true" />)
        /// or return (<see langword="false" />).
        /// </param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionState">
        /// The state object for for <paramref name="action" />.
        /// </param>
        /// <returns>
        /// The occured errors or <see langword="null" /> if no exception was thrown while invokation.
        /// </returns>
        /// <exception cref="AggregateException">At least one error occured.</exception>
        public static AggregateException ForAll<T, TState>(this IEnumerable<T> seq,
                                                           bool throwExceptions,
                                                           Action<IForAllItemContext<T, TState>> action,
                                                           TState actionState)
        {
            return ForAll<T, TState>(seq,
                                     action: action,
                                     actionStateProvider: (item, index) => actionState,
                                     throwExceptions: throwExceptions);
        }

        /// <summary>
        /// Invokes an action for all items of a sequence even if one or more invokations fail.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <typeparam name="TState">
        /// Type of the state item for <paramref name="action" />.
        /// </typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">
        /// The provider that creates / returns the state object for for <paramref name="action" />.
        /// </param>
        /// <returns>
        /// Should return <see langword="null" />; otherwise an <see cref="AggregateException" /> is thrown.
        /// </returns>
        /// <exception cref="AggregateException">At least one error occured.</exception>
        public static AggregateException ForAll<T, TState>(this IEnumerable<T> seq,
                                                           Action<IForAllItemContext<T, TState>> action,
                                                           Func<T, long, TState> actionStateProvider)
        {
            return ForAll<T, TState>(seq,
                                     throwExceptions: true,
                                     action: action,
                                     actionStateProvider: actionStateProvider);
        }

        /// <summary>
        /// Invokes an action for all items of a sequence even if one or more invokations fail.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <typeparam name="TState">
        /// Type of the state item for <paramref name="action" />.
        /// </typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="throwExceptions">
        /// If at least one exception was thrown while invokation throw (<see langword="true" />)
        /// or return (<see langword="false" />).
        /// </param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="actionStateProvider">
        /// The provider that creates / returns the state object for for <paramref name="action" />.
        /// </param>
        /// <returns>
        /// The occured errors or <see langword="null" /> if no exception was thrown while invokation.
        /// </returns>
        /// <exception cref="AggregateException">At least one error occured.</exception>
        public static AggregateException ForAll<T, TState>(this IEnumerable<T> seq,
                                                           bool throwExceptions,
                                                           Action<IForAllItemContext<T, TState>> action,
                                                           Func<T, long, TState> actionStateProvider)
        {
            if (seq == null)
            {
                throw new ArgumentNullException("seq");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (actionStateProvider == null)
            {
                throw new ArgumentNullException("actionStateProvider");
            }

            List<Exception> errors = new List<Exception>();

            try
            {
                using (var e = seq.GetEnumerator())
                {
                    long index = -1;

                    while (e.MoveNext())
                    {
                        try
                        {
                            var ctx = new ForAllItemContext<T, TState>(isSynchronized: false)
                            {
                                Index = ++index,
                                Item = e.Current,
                            };
                            ctx.State = actionStateProvider(ctx.Item, ctx.Index);

                            action(ctx);
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }

            AggregateException result = null;

            if (errors.Count > 0)
            {
                result = new AggregateException(errors);
            }

            if (throwExceptions &&
                (result != null))
            {
                throw result;
            }

            return result;
        }

        #endregion Methods (6)
    }
}