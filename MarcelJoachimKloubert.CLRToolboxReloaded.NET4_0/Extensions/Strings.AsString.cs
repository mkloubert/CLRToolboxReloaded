// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define KNOWS_DBNULL
#endif

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

            if (obj is IEnumerable<char>)
            {
                return new string(AsArray(obj as IEnumerable<char>));
            }

            if (obj is TextReader)
            {
                return ((TextReader)obj).ReadToEnd();
            }

            if (obj is IEnumerable<byte>)
            {
                return AsHexString(obj as IEnumerable<byte>);
            }

            if ((obj is IEnumerable<string>) ||
                (obj is IEnumerable<IEnumerable<char>>))
            {
                return string.Concat(((IEnumerable)obj).Cast<object>()
                                                       .Select(o => AsString(o)));
            }

            return obj.ToString();
        }

        #endregion Methods (1)
    }
}