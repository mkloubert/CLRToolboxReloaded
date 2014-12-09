// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Tries to peek the most upper value of a <see cref="Stack{T}" />.
        /// </summary>
        /// <typeparam name="T">Type of the items of the stack.</typeparam>
        /// <param name="stack">The stack.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The most upper value or the default value from <paramref name="defaultValue" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stack" /> is <see langword="null" />.
        /// </exception>
        public static T PeekOrDefault<T>(this Stack<T> stack, T defaultValue = default(T))
        {
            return PeekOrDefault<T>(stack,
                                    (s) => defaultValue);
        }

        /// <summary>
        /// Tries to peek the most upper value of a <see cref="Stack{T}" />.
        /// </summary>
        /// <typeparam name="T">Type of the items of the stack.</typeparam>
        /// <param name="stack">The stack.</param>
        /// <param name="defaultValueProvider">The function / method that provides the default value.</param>
        /// <returns>The most upper value or the default value from <paramref name="defaultValueProvider" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stack" /> and/or <paramref name="defaultValueProvider" /> are <see langword="null" />.
        /// </exception>
        public static T PeekOrDefault<T>(this Stack<T> stack, Func<Stack<T>, T> defaultValueProvider)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack");
            }

            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            return stack.Count > 0 ? stack.Peek()
                                   : defaultValueProvider(stack);
        }

        #endregion Methods (1)
    }
}