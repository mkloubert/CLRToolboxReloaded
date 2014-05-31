// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    partial class NullIndexDictionary<TValue>
    {
        #region CLASS: DictionaryEnumerator

        private struct DictionaryEnumerator : IEnumerator<KeyValuePair<int, TValue>>, IDictionaryEnumerator
        {
            #region Fields (2)

            private readonly IEnumerator<KeyValuePair<int, TValue>> _ENUMERATOR;
            private readonly byte _RETURN_TYPE;

            #endregion Fields (2)

            #region Constrcutors (1)

            internal DictionaryEnumerator(IEnumerable<KeyValuePair<int, TValue>> enumerator, byte returnType)
            {
                this._ENUMERATOR = enumerator.GetEnumerator();
                this._RETURN_TYPE = returnType;
            }

            #endregion Constrcutors (1)

            #region Methods (3)

            public void Dispose()
            {
                this._ENUMERATOR
                    .Dispose();
            }

            public bool MoveNext()
            {
                return this._ENUMERATOR
                           .MoveNext();
            }

            public void Reset()
            {
                this._ENUMERATOR.Reset();
            }

            #endregion Methods (3)

            #region Properties (7)

            public KeyValuePair<int, TValue> Current
            {
                get { return this._ENUMERATOR.Current; }
            }

            object IEnumerator.Current
            {
                get
                {
                    switch (this._RETURN_TYPE)
                    {
                        case 1:
                            return this.Entry;
                    }

                    return this.Current;
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    return new DictionaryEntry(key: this.Key,
                                               value: this.Value);
                }
            }

            public int Key
            {
                get { return this.Current.Key; }
            }

            object IDictionaryEnumerator.Key
            {
                get { return this.Key; }
            }

            public TValue Value
            {
                get { return this.Current.Value; }
            }

            object IDictionaryEnumerator.Value
            {
                get { return this.Value; }
            }

            #endregion Properties (7)
        }

        #endregion CLASS: DictionaryEnumerator
    }
}