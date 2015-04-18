// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Method (2)

        /// <summary>
        /// Checks if a sequence is <see langword="null" /> or empty.
        /// </summary>
        /// <param name="seq">The sequence to check.</param>
        /// <returns>Is <see langword="null" /> or empty; or not.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable seq)
        {
            if (seq == null)
            {
                return true;
            }

            return IsEmpty(seq);
        }

        /// <summary>
        /// Checks if a sequence is <see langword="null" /> or empty.
        /// </summary>
        /// <typeparam name="T">Type of the items of the sequence.</typeparam>
        /// <param name="seq">The sequence to check.</param>
        /// <returns>Is <see langword="null" /> or empty; or not.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> seq)
        {
            if (seq == null)
            {
                return true;
            }

            return IsEmpty<T>(seq);
        }

        #endregion Method (2)
    }
}