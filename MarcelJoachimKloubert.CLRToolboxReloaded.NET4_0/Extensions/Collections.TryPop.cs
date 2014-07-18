// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Tries to pop an item from a stack.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="stack">The stack.</param>
        /// <param name="value">The variable where to write the value to.</param>
        /// <param name="defaultValue">The value for <paramref name="value" /> if <paramref name="stack" /> is empty.</param>
        /// <returns>Pop operation was successfull or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stack" /> is <see langword="null" />.
        /// </exception>
        public static bool TryPop<T>(this Stack<T> stack, out T value, T defaultValue = default(T))
        {
            return TryPop<T>(stack: stack,
                             value: out value,
                             defaultValueProvider: (s) => defaultValue);
        }

        /// <summary>
        /// Tries to pop an item from a stack.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="stack">The stack.</param>
        /// <param name="value">The variable where to write the value to.</param>
        /// <param name="defaultValueProvider">
        /// The function that provides the value for <paramref name="value" /> if <paramref name="stack" />
        /// is empty.
        /// </param>
        /// <returns>Pop operation was successfull or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stack" /> and/or <paramref name="defaultValueProvider" /> are <see langword="null" />.
        /// </exception>
        public static bool TryPop<T>(this Stack<T> stack, out T value, Func<Stack<T>, T> defaultValueProvider)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack");
            }

            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            if (stack.Count > 0)
            {
                value = stack.Pop();
                return true;
            }

            value = defaultValueProvider(stack);
            return false;
        }

        #endregion Methods (2)
    }
}