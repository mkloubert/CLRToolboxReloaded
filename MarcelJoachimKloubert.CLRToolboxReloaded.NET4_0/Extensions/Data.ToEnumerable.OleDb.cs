// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Data
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Converts an OleDB data reader to a lazy sequence of OleDB data records.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The sequence of data records.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<OleDbDataReader> ToEnumerable(this OleDbDataReader reader)
        {
            return ToEnumerableCommon(reader);
        }

        #endregion Methods
    }
}