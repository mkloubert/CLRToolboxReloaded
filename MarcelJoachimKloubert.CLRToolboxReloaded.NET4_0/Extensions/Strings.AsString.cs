// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. http://blog.marcel-kloubert.de

#if !(PORTABLE || PORTABLE40)
#define KNOWS_DBNULL
#endif

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Converts or casts an object to its string representation / content.
        /// </summary>
        /// <param name="obj">The object to convert.</param>
        /// <param name="handleDbNullAsNull">
        /// Handle DBNull object as <see langword="null" /> reference or not.
        /// </param>
        /// <returns>The converted object.</returns>
        public static string AsString(this object obj, bool handleDbNullAsNull = true)
        {
            if (obj is string)
            {
                return (string)obj;
            }

            if (obj == null)
            {
                return null;
            }

#if KNOWS_DBNULL

            if (handleDbNullAsNull &&
                global::System.DBNull.Value.Equals(obj))
            {
                return null;
            }

#endif

            return obj.ToString();
        }

        #endregion Methods (1)
    }
}