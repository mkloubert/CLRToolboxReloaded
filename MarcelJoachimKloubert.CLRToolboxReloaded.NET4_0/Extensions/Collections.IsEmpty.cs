// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Checks if a sequence is empty.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="seq">The sequence to check.</param>
        /// <returns>Sequence is empty or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public static bool IsEmpty<T>(this IEnumerable<T> seq)
        {
            return seq.Count<T>() < 1;
        }

        /// <summary>
        /// Checks if a sequence is empty.
        /// </summary>
        /// <param name="seq">The sequence to check.</param>
        /// <returns>Sequence is empty or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public static bool IsEmpty(this IEnumerable seq)
        {
            if (seq == null)
            {
                throw new ArgumentNullException("seq");
            }

            var coll = seq as ICollection;
            if (coll != null)
            {
                return coll.Count < 1;
            }

            return IsEmpty<object>(seq.Cast<object>());
        }

        #endregion Methods (2)
    }
}