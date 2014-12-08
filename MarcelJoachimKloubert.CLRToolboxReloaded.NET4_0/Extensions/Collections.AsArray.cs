// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Converts or casts a sequence to an array.
        /// </summary>
        /// <param name="seq">The sequence to cast / convert.</param>
        /// <returns>
        /// The converted / casted array or <see langword="null" /> if <see langword="seq" />
        /// is also <see langword="null" />.
        /// </returns>
        /// <remarks>
        /// If <see langword="seq" /> is already an array, it is simply casted.
        /// </remarks>
        public static object[] AsArray(this IEnumerable seq)
        {
            if (seq == null)
            {
                return null;
            }

            var genList = seq as IGeneralList;
            if (genList != null)
            {
                return genList.ToArray();
            }

            return AsArray<object>(seq.Cast<object>());
        }

        /// <summary>
        /// Converts or casts a sequence to an array.
        /// </summary>
        /// <typeparam name="T">Type of the items.</typeparam>
        /// <param name="seq">The sequence to cast / convert.</param>
        /// <returns>
        /// The converted / casted array or <see langword="null" /> if <see langword="seq" />
        /// is also <see langword="null" />.
        /// </returns>
        /// <remarks>
        /// If <see langword="seq" /> is already an array, it is simply casted.
        /// </remarks>
        public static T[] AsArray<T>(this IEnumerable<T> seq)
        {
            if (seq == null)
            {
                return null;
            }

            var result = seq as T[];
            if (result == null)
            {
                result = seq.ToArray();
            }

            return result;
        }

        #endregion Methods (2)
    }
}