// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Returns an <see cref="IDataRecord" /> as a dictionary.
        /// </summary>
        /// <param name="rec">The record from where to get the data from.</param>
        /// <returns>The record as dictionary.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rec" /> is <see langword="null" />.
        /// </exception>
        public static Dictionary<string, object> ToDictionary(this IDataRecord rec)
        {
            return ToDictionary<Dictionary<string, object>>(rec: rec);
        }

        /// <summary>
        /// Returns an <see cref="IDataRecord" /> as a dictionary.
        /// </summary>
        /// <typeparam name="TDict">Type of the result dictionary.</typeparam>
        /// <param name="rec">The record from where to get the data from.</param>
        /// <returns>The record as dictionary.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rec" /> is <see langword="null" />.
        /// </exception>
        public static TDict ToDictionary<TDict>(this IDataRecord rec)
            where TDict : global::System.Collections.Generic.IDictionary<string, object>, new()
        {
            if (rec == null)
            {
                throw new ArgumentNullException("rec");
            }

            var dict = new TDict();

            for (var i = 0; i < rec.FieldCount; i++)
            {
                dict.Add(key: rec.GetName(i) ?? string.Empty,
                         value: rec.IsDBNull(i) ? null : rec.GetValue(i));
            }

            return dict;
        }

        #endregion Methods (2)
    }
}