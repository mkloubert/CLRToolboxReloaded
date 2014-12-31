// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Pops all items from a <see cref="Stack{T}" />.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="stack">The stack from where to pop the items from.</param>
        /// <returns>The poped items.</returns>
        public static IEnumerable<T> PopAll<T>(this Stack<T> stack)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack");
            }

            while (stack.Count > 0)
            {
                yield return stack.Pop();
            }
        }

        #endregion Methods (1)
    }
}