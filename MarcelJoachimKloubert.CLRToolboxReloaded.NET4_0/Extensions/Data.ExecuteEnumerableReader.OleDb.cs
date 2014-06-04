// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxDataExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Executes a command and returns it as sequence of underlying data record.
        /// </summary>
        /// <param name="cmd">The command to execute.</param>
        /// <returns>The (lazy) sequence of command.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cmd" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<OleDbDataReader> ExecuteEnumerableReader(this OleDbCommand cmd)
        {
            return ExecuteEnumerableReaderInner<OleDbDataReader>(cmd);
        }

        #endregion Methods
    }
}