// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    partial struct DictionaryEnumerator<TKey, TValue>
    {
        /// <summary>
        /// List of modes for <see cref="DictionaryEnumerator{TKey, TValue}" /> struct.
        /// </summary>
        public enum EnumeratorMode : byte
        {
            /// <summary>
            /// Use for generic dictionary instance.
            /// </summary>
            GenericDictionary,

            /// <summary>
            /// Use for general dictionary instance.
            /// </summary>
            IDictionary,
        }
    }
}