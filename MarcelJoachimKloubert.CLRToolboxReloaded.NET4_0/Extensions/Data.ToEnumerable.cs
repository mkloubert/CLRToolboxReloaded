// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Data
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Converts a data reader to a lazy sequence of data records.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The sequence of data records.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<IDataRecord> ToEnumerable(this IDataReader reader)
        {
            return ToEnumerable<IDataReader, IDataRecord>(reader,
                                                          r => r);
        }

        /// <summary>
        /// Converts a data reader to a lazy sequence of data records.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="selector">The selector returns the current data record for the given reader of <paramref name="reader" />.</param>
        /// <returns>The sequence of data records.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> and/or <paramref name="selector" /> are <see langword="null" />.
        /// </exception>
        public static IEnumerable<TRec> ToEnumerable<TReader, TRec>(this TReader reader, Func<TReader, TRec> selector)
            where TRec : global::System.Data.IDataRecord
            where TReader : global::System.Data.IDataReader
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            while (reader.Read())
            {
                yield return selector(reader);
            }
        }

        private static IEnumerable<TReader> ToEnumerableCommon<TReader>(TReader reader)
            where TReader : global::System.Data.IDataReader
        {
            return ToEnumerable<TReader, TReader>(reader,
                                                  r => r);
        }

        #endregion Methods
    }
}