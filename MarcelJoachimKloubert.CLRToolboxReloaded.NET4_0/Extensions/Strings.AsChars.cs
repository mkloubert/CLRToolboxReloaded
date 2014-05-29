// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define STRING_IS_CHAR_SEQUENCE
#endif

using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Converts a string to a char sequence.
        /// </summary>
        /// <param name="str">The input string.</param>
        /// <returns>
        /// <paramref name="str" /> as char sequence or <see langword="null" /> if
        /// <paramref name="str" /> is also <see langword="null" />.
        /// </returns>
        /// <remarks>
        /// Portable environments, e.g., do not handle strings as char sequences.
        /// </remarks>
        public static IEnumerable<char> AsChars(this string str)
        {
#if STRING_IS_CHAR_SEQUENCE
            return str;
#else
            return (global::MarcelJoachimKloubert.CLRToolbox.CharSequence)str;
#endif
        }

        #endregion Methods (1)
    }
}