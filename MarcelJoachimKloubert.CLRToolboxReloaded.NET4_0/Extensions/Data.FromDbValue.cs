// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;
using System.Collections.Generic;
using System.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Data
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Reads a value from a data record.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="rec">The record from where to read the value from.</param>
        /// <param name="name">The name of the field inside the data record where the value is stored.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rec" /> is <see langword="null" />.
        /// </exception>
        public static T FromDbValue<T>(this IDataRecord rec, string name)
        {
            if (rec == null)
            {
                throw new ArgumentNullException("rec");
            }

            return FromDbValue<T>(rec,
                                  rec.GetOrdinal(name));
        }

        /// <summary>
        /// Reads a value from a data record.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="rec">The record from where to read the value from.</param>
        /// <param name="ordinal">The ordinal of the field inside the data record where the value is stored.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rec" /> is <see langword="null" />.
        /// </exception>
        public static T FromDbValue<T>(this IDataRecord rec, int ordinal)
        {
            if (rec == null)
            {
                throw new ArgumentNullException("rec");
            }

            return GlobalConverter.Current
                                  .ChangeType<T>(rec.IsDBNull(ordinal) ? null : rec.GetValue(ordinal));
        }

        #endregion Methods
    }
}