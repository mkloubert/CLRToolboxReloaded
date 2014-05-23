// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Invokes an action for each item of a sequence.
        /// The operation will NOT continue if at least one invokation fails.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>
        /// Operation was canceled (<see langword="true" />) or completely done (<see langword="true" />).
        /// </returns>
        public static bool ForEach<T>(this IEnumerable<T> seq,
                                      Action<IForEachItemContext<T>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return ForEach<T, Action<IForEachItemContext<T>>>(seq,
                                                              action: (ctx) => ctx.State(ctx),
                                                              actionState: action);
        }

        /// <summary>
        /// Invokes an action for each item of a sequence.
        /// The operation will NOT continue if at least one invokation fails.
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
        /// Operation was canceled (<see langword="true" />) or completely done (<see langword="true" />).
        /// </returns>
        public static bool ForEach<T, TState>(this IEnumerable<T> seq,
                                              Action<IForEachItemContext<T, TState>> action,
                                              TState actionState)
        {
            return ForEach<T, TState>(seq,
                                      action: action,
                                      actionStateProvider: (item, index) => actionState);
        }

        /// <summary>
        /// Invokes an action for each item of a sequence.
        /// The operation will NOT continue if at least one invokation fails.
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
        /// Operation was canceled (<see langword="true" />) or completely done (<see langword="true" />).
        /// </returns>
        public static bool ForEach<T, TState>(this IEnumerable<T> seq,
                                              Action<IForEachItemContext<T, TState>> action,
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

            var result = false;

            using (var e = seq.GetEnumerator())
            {
                long index = -1;

                while (e.MoveNext())
                {
                    var ctx = new ForEachItemContext<T, TState>()
                    {
                        Cancel = false,
                        Index = ++index,
                        Item = e.Current,
                    };
                    ctx.State = actionStateProvider(ctx.Item, ctx.Index);

                    action(ctx);

                    if (ctx.Cancel)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        #endregion Methods (3)
    }
}