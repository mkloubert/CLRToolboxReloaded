// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Transforms all items of a list.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="list">The list to transform.</param>
        /// <param name="transformFunc">
        /// The function to use to transform an item of <paramref name="list" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list" /> and/or <paramref name="transformFunc" /> are <see langword="null" />.
        /// </exception>
        public static void Transform<T>(this IList<T> list,
                                        Func<IForEachItemContext<T>, T> transformFunc)
        {
            if (transformFunc == null)
            {
                throw new ArgumentNullException("transformFunc");
            }

            Transform<T, Func<IForEachItemContext<T>, T>>(list: list,
                                                          transformFunc: (ctx) => ctx.State(ctx),
                                                          funcState: transformFunc);
        }

        /// <summary>
        /// Transforms all items of a list.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <typeparam name="TState">Type of the state item for <paramref name="transformFunc" />.</typeparam>
        /// <param name="list">The list to transform.</param>
        /// <param name="transformFunc">
        /// The function to use to transform an item of <paramref name="list" />.
        /// </param>
        /// <param name="funcState">The state object for <paramref name="transformFunc" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list" /> and/or <paramref name="transformFunc" /> are <see langword="null" />.
        /// </exception>
        public static void Transform<T, TState>(this IList<T> list,
                                                Func<IForEachItemContext<T, TState>, T> transformFunc,
                                                TState funcState)
        {
            Transform<T, TState>(list: list,
                                 transformFunc: transformFunc,
                                 funcStateProvider: (i, idx) => funcState);
        }

        /// <summary>
        /// Transforms all items of a list.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <typeparam name="TState">Type of the state item for <paramref name="transformFunc" />.</typeparam>
        /// <param name="list">The list to transform.</param>
        /// <param name="transformFunc">
        /// The function to use to transform an item of <paramref name="list" />.
        /// </param>
        /// <param name="funcStateProvider">The function that provides the state object for <paramref name="transformFunc" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list" />, <paramref name="transformFunc" /> and/or <paramref name="funcStateProvider" /> are <see langword="null" />.
        /// </exception>
        public static void Transform<T, TState>(this IList<T> list,
                                                Func<IForEachItemContext<T, TState>, T> transformFunc,
                                                Func<T, int, TState> funcStateProvider)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (transformFunc == null)
            {
                throw new ArgumentNullException("transformFunc");
            }

            if (funcStateProvider == null)
            {
                throw new ArgumentNullException("funcStateProvider");
            }

            for (var i = 0; i < list.Count; i++)
            {
                var ctx = new ForEachItemContext<T, TState>(isSynchronized: false);
                ctx.Cancel = false;
                ctx.Index = i;
                ctx.Item = list[i];
                ctx.State = funcStateProvider(ctx.Item, i);
                
                var newItem = transformFunc(ctx);
                
                if (ctx.Cancel)
                {
                    break;
                }

                list[i] = newItem;
            }
        }

        #endregion Methods (3)
    }
}