// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Creates a list of tasks for handling the items of a sequence while each item is handled in a separate task.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="seq">The sequence.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>
        /// The list of tasks.
        /// </returns>
        public static IEnumerable<Task> CreateTasksForAll<T>(this IEnumerable<T> seq,
                                                             Action<IForAllItemContext<T>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return CreateTasksForAll<T, Action<IForAllItemContext<T>>>(seq,
                                                                       action: (ctx) => ctx.State(ctx),
                                                                       actionState: action);
        }

        /// <summary>
        /// Creates a list of tasks for handling the items of a sequence while each item is handled in a separate task.
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
        /// The list of tasks.
        /// </returns>
        public static IEnumerable<Task> CreateTasksForAll<T, TState>(this IEnumerable<T> seq,
                                                                     Action<IForAllItemContext<T, TState>> action,
                                                                     TState actionState)
        {
            return CreateTasksForAll<T, TState>(seq,
                                                action: action,
                                                actionStateProvider: (item, index) => actionState);
        }

        /// <summary>
        /// Creates a list of tasks for handling the items of a sequence while each item is handled in a separate task.
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
        /// The list of tasks.
        /// </returns>
        public static IEnumerable<Task> CreateTasksForAll<T, TState>(this IEnumerable<T> seq,
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

            var errors = new List<Exception>();
            var sync = new object();

            using (var e = seq.GetEnumerator())
            {
                long index = -1;

                while (e.MoveNext())
                {
                    yield return new Task(action: (state) =>
                        {
                            var tuple = (ForAllAsyncTuple<T, TState>)state;

                            tuple.Invoke();
                        }, state: new ForAllAsyncTuple<T, TState>(action: action,
                                                                  actionStateProvider: actionStateProvider,
                                                                  index: ++index,
                                                                  item: e.Current,
                                                                  errors: errors,
                                                                  sync: sync));
                }
            }
        }

        #endregion Methods (3)
    }
}