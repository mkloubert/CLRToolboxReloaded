// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    partial class NullIndexDictionary<TValue>
    {
        #region Methods (21)

        void ICollection<TValue>.Add(TValue item)
        {
            this.Add(item: item);
        }

        int IList.Add(object value)
        {
            return this.Add(item: this.ConvertTo<TValue>(value));
        }

        void IDictionary<int?, TValue>.Add(int? key, TValue value)
        {
            this.Add(key: key,
                     value: value);
        }

        void IDictionary.Add(object key, object value)
        {
            this.Add(key: this.ConvertTo<int?>(key),
                     value: this.ConvertTo<TValue>(value));
        }

        void ICollection<KeyValuePair<int?, TValue>>.Add(KeyValuePair<int?, TValue> item)
        {
            this.Add(key: item.Key,
                     value: item.Value);
        }

        bool ICollection<KeyValuePair<int?, TValue>>.Contains(KeyValuePair<int?, TValue> item)
        {
            TValue value;
            if (this.TryGetValue(item.Key, out value))
            {
                // found => now check if equal
                return object.Equals(item.Value, value);
            }

            return false;
        }

        bool IList.Contains(object value)
        {
            return ((ICollection<TValue>)this).Contains(item: this.ConvertTo<TValue>(value));
        }

        bool IDictionary.Contains(object key)
        {
            return this.ContainsKey(key: this.ConvertTo<int?>(key));
        }

        void ICollection<KeyValuePair<int?, TValue>>.CopyTo(KeyValuePair<int?, TValue>[] array, int arrayIndex)
        {
            this.CopyToInner(array: array,
                             arrayIndex: arrayIndex,
                             arrayValueProvider: (i, v) => new KeyValuePair<int?, TValue>(key: i,
                                                                                          value: v));
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyToInner(array: array,
                             arrayIndex: index,
                             arrayValueProvider: (i, v) => v);
        }

        IEnumerator<KeyValuePair<int?, TValue>> IEnumerable<KeyValuePair<int?, TValue>>.GetEnumerator()
        {
            IEnumerable<KeyValuePair<int, TValue>> seq = this;

            return seq.Select(i => new KeyValuePair<int?, TValue>(key: i.Key,
                                                                  value: i.Value))
                      .GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this._INNER_DICT
                       .Values
                       .GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new DictionaryEnumerator<int, TValue>(this,
                                                         DictionaryEnumerator<int, TValue>.EnumeratorMode.IDictionary);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        int IList.IndexOf(object value)
        {
            return ((IList<TValue>)this).IndexOf(item: this.ConvertTo<TValue>(value));
        }

        void IList.Insert(int index, object value)
        {
            ((IList<TValue>)this).Insert(index: index,
                                         item: this.ConvertTo<TValue>(value));
        }

        bool IDictionary<int?, TValue>.Remove(int? key)
        {
            return this.Remove(key: key)
                       .HasValue;
        }

        bool ICollection<KeyValuePair<int?, TValue>>.Remove(KeyValuePair<int?, TValue> item)
        {
            TValue value;
            if (this.TryGetValue(item.Key, out value))
            {
                if (object.Equals(item.Value, value))
                {
                    // found => now check if equal
                    return this.Remove(key: item.Key)
                               .HasValue;
                }
            }

            return false;
        }

        bool ICollection<TValue>.Remove(TValue item)
        {
            using (var e = this._INNER_DICT.Keys.ToArray().AsEnumerable().GetEnumerator())
            {
                while (e.MoveNext())
                {
                    var key = e.Current;
                    var value = this._INNER_DICT[key];

                    if (object.Equals(value, item))
                    {
                        return this.Remove(key)
                                   .HasValue;
                    }
                }
            }

            return false;
        }

        void IList.Remove(object value)
        {
            ((ICollection<TValue>)this).Remove(item: this.ConvertTo<TValue>(value));
        }

        void IDictionary.Remove(object key)
        {
            this.Remove(key: this.ConvertTo<int?>(key));
        }

        #endregion Methods (21)

        #region Properties (9)

        ICollection<int?> IDictionary<int?, TValue>.Keys
        {
            get
            {
                return this.Keys
                           .Cast<int?>()
                           .ToArray();
            }
        }

        IEnumerable<int?> IReadOnlyDictionary<int?, TValue>.Keys
        {
            get { return this.Keys.Cast<int?>(); }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                var result = this.Keys as ICollection;
                if (result == null)
                {
                    result = this.Keys.ToArray();
                }

                return result;
            }
        }

        TValue IList<TValue>.this[int index]
        {
            get
            {
                this.ThrowIfOutOfRange(index);

                TValue result;
                this._INNER_DICT.TryGetValue(key: index, value: out result);

                return result;
            }

            set
            {
                this.ThrowIfOutOfRange(index);

                this._INNER_DICT[index] = value;
            }
        }

        TValue IReadOnlyList<TValue>.this[int index]
        {
            get { return ((IList<TValue>)this)[index]; }
        }

        object IList.this[int index]
        {
            get { return ((IList<TValue>)this)[index]; }

            set
            {
                ((IList<TValue>)this)[index] = this.ConvertTo<TValue>(value);
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this[key: this.ConvertTo<int?>(key)];
            }

            set
            {
                this[key: this.ConvertTo<int?>(key)] = this.ConvertTo<TValue>(value);
            }
        }

        ICollection<TValue> IDictionary<int?, TValue>.Values
        {
            get
            {
                return this.Values
                           .ToArray();
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                var result = this.Values as ICollection;
                if (result == null)
                {
                    result = this.Values.ToArray();
                }

                return result;
            }
        }

        #endregion Properties (9)
    }
}