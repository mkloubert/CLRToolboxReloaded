// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Data
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Returns an <see cref="IDataReader" /> as a sequence of dictionaries.
        /// </summary>
        /// <param name="reader">The reader from where to get the data from.</param>
        /// <returns>The reader as dictionaries.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<Dictionary<string, object>> ToDictionaryEnumerable(this IDataReader reader)
        {
            return ToDictionaryEnumerable<Dictionary<string, object>>(reader: reader);
        }

        /// <summary>
        /// Returns an <see cref="IDataReader" /> as a sequence of dictionaries.
        /// </summary>
        /// <typeparam name="TDict">Type of the result dictionaries.</typeparam>
        /// <param name="reader">The reader from where to get the data from.</param>
        /// <returns>The reader as dictionaries.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<TDict> ToDictionaryEnumerable<TDict>(this IDataReader reader)
            where TDict : global::System.Collections.Generic.IDictionary<string, object>, new()
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            while (reader.Read())
            {
                yield return ToDictionary<TDict>(rec: reader);
            }
        }

        #endregion Methods (2)
    }
}