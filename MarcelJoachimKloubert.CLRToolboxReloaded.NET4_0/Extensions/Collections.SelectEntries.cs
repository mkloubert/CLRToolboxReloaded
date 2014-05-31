// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        // Public Methods (1) 

        /// <summary>
        /// Selects the <see cref="DictionaryEntry" /> of an <see cref="IDictionary" /> object.
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        /// <returns>The selected entries.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> is <see langword="null" />.
        /// </exception>
        public static IEnumerable<DictionaryEntry> SelectEntries(this IDictionary dict)
        {
            if (dict == null)
            {
                throw new ArgumentNullException("dict");
            }

            var e = dict.GetEnumerator();
            try
            {
                while (e.MoveNext())
                {
                    yield return e.Entry;
                }
            }
            finally
            {
                DisposeEx(e as IDisposable);
            }
        }

        #endregion Methods
    }
}