// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region CLASS: NullIndexDictionary<TValue>

    /// <summary>
    /// A dictionary that uses a nullable integer for its indexes.
    /// <see langword="null" /> indexes can be used to access the last or next index.
    /// </summary>
    /// <typeparam name="TValue">Type of the values.</typeparam>
    public partial class NullIndexDictionary<TValue> : IDictionary<int?, TValue>,
                                                       IReadOnlyDictionary<int?, TValue>,
                                                       IEnumerable<KeyValuePair<int, TValue>>,
                                                       IList<TValue>, IReadOnlyList<TValue>,
                                                       IList, IDictionary
    {
        #region Fields (2)

        /// <summary>
        /// Stores the inner dictionary.
        /// </summary>
        protected readonly IDictionary<int, TValue> _INNER_DICT;

        /// <summary>
        /// An unique object for sync operations.
        /// </summary>
        protected readonly object _SYNC = new object();

        #endregion Fields (2)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="NullIndexDictionary{TValue}" /> class.
        /// </summary>
        public NullIndexDictionary()
            : this(innerDict: new Dictionary<int, TValue>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullIndexDictionary{TValue}" /> class.
        /// </summary>
        /// <param name="innerDict">The inner dictionary to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerDict" /> is <see langword="null" />.
        /// </exception>
        public NullIndexDictionary(IDictionary<int, TValue> innerDict)
        {
            if (innerDict == null)
            {
                throw new ArgumentNullException("innerDict");
            }

            this._INNER_DICT = innerDict;
        }

        #endregion Constructors (2)

        #region Methods (20)

        /// <summary>
        /// Adds an items add the end of the list.
        /// </summary>
        /// <param name="item">The value to add.</param>
        /// <returns>The new index.</returns>
        public int Add(TValue item)
        {
            return this.Add(key: null,
                            value: item);
        }

        /// <inheriteddoc />
        public int Add(int? key, TValue value)
        {
            this.CheckIndex(key);

            if (key.HasValue == false)
            {
                key = (this.TryGetLastIndex() + 1) ?? 0;
            }

            this._INNER_DICT
                .Add(key: key.Value,
                     value: value);

            return key.Value;
        }

        /// <summary>
        /// Adds/appends a list of items.
        /// </summary>
        /// <param name="seq">The itrms to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public void AddRange(IEnumerable<TValue> seq)
        {
            seq.ForEach(action: ctx => ctx.State
                                          .Dictionary.Add(item: ctx.Item),
                        actionState: new
                        {
                            Dictionary = this,
                        });
        }

        private void CheckIndex(int? key)
        {
            if (key < 0)
            {
                throw new ArgumentOutOfRangeException("key");
            }
        }

        /// <inheriteddoc />
        public void Clear()
        {
            this._INNER_DICT
                .Clear();
        }

        /// <inheriteddoc />
        public bool Contains(TValue item)
        {
            return this._INNER_DICT
                       .Values
                       .Contains(item);
        }

        /// <inheriteddoc />
        public bool ContainsKey(int? key)
        {
            if (key.HasValue == false)
            {
                return this.Count > 0;
            }

            return (key >= 0) &&
                   (key < this.Count);
        }

        /// <inheriteddoc />
        public void CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex)
        {
            this.CopyToInner(array: array,
                             arrayIndex: arrayIndex,
                             arrayValueProvider: (i, v) => new KeyValuePair<int, TValue>(key: i,
                                                                                         value: v));
        }

        /// <inheriteddoc />
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            this._INNER_DICT
                .Values
                .CopyTo(array, arrayIndex);
        }

        private void CopyToInner(Array array, int arrayIndex,
                                 Func<int, TValue, object> arrayValueProvider)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if ((arrayIndex < 0) || (arrayIndex > array.Length))
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if ((array.Length - arrayIndex) < this.Count)
            {
                throw new ArgumentException("arrayIndex");
            }

            var num = this._INNER_DICT.Count;
            for (var i = 0; i < num; i++)
            {
                array.SetValue(value: arrayValueProvider(i, this[i]),
                               indices: new int[] { arrayIndex + i });
            }
        }

        /// <inheriteddoc />
        public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
        {
            return new DictionaryEnumerator(this.Keys
                                                .Select(k =>
                                                    {
                                                        TValue value;
                                                        this._INNER_DICT.TryGetValue(k, out value);

                                                        return new KeyValuePair<int, TValue>(key: k,
                                                                                             value: value);
                                                    }), 2);
        }

        /// <inheriteddoc />
        public int IndexOf(TValue item)
        {
            var result = -1;
            this._INNER_DICT
                .ForEach(ctx =>
                {
                    if (object.Equals(ctx.Item.Value,  // current item
                                      ctx.State.Item))  // item from method parameter
                    {
                        result = ctx.Item.Key;
                        ctx.Cancel = true;
                    }
                }, actionState: new
                {
                    Item = item,
                });

            return result;
        }

        /// <inheriteddoc />
        public void Insert(int index, TValue item)
        {
            this.CheckIndex(index);

            // move other items "down"
            this._INNER_DICT
                .Keys
                .Where(k => k >= index)
                .OrderByDescending(k => k)
                .ToArray()
                .ForEach(ctx =>
                {
                    var dict = ctx.State.Dictionary;
                    var k = ctx.Item;

                    dict[k + 1] = dict[k];
                }, actionState: new
                {
                    Dictionary = this._INNER_DICT,
                    Item = item,
                });

            // set inserted item
            this._INNER_DICT[index] = item;
        }

        /// <summary>
        /// Removes the last item.
        /// </summary>
        /// <returns>The key that was removed.</returns>
        public int? Remove()
        {
            return this.Remove(key: null);
        }

        /// <inheriteddoc />
        public int? Remove(int? key)
        {
            this.CheckIndex(key);

            if (key.HasValue == false)
            {
                key = this.TryGetLastIndex();
            }

            if (key.HasValue)
            {
                if (this._INNER_DICT.Remove(key.Value))
                {
                    // move other items "up"
                    this._INNER_DICT
                        .Keys
                        .Where(k => k > key)
                        .OrderBy(k => k)
                        .ToArray()
                        .ForEach(ctx =>
                        {
                            var dict = ctx.State.Dictionary;
                            var k = ctx.Item;

                            dict[k - 1] = dict[k];
                            dict.Remove(k);
                        }, actionState: new
                        {
                            Dictionary = this._INNER_DICT,
                        });

                    return key.Value;
                }
            }

            return null;
        }

        /// <inheriteddoc />
        public void RemoveAt(int index)
        {
            this.CheckIndex(index);

            if (index >= this.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            this.Remove(key: index);
        }

        private void ThrowIfOutOfRange(int index)
        {
            if ((index < 0) || (index >= this.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }
        }

        /// <summary>
        /// Converts a <see cref="NullIndexDictionary{TValue}" /> to a well known
        /// <see cref="Dictionary{TKey, TValue}" /> object.
        /// </summary>
        /// <param name="dict">The input value.</param>
        /// <returns>The output value.</returns>
        protected internal static Dictionary<int, TValue> ToDictionary(NullIndexDictionary<TValue> dict)
        {
            Dictionary<int, TValue> result = null;

            if (dict != null)
            {
                result = dict._INNER_DICT as Dictionary<int, TValue>;
                if (result == null)
                {
                    result = new Dictionary<int, TValue>(dict._INNER_DICT);
                }
            }

            return result;
        }

        /// <summary>
        /// Converts a <see cref="NullIndexDictionary{TValue}" /> to a well known
        /// <see cref="List{T}" /> object.
        /// </summary>
        /// <param name="niDict">The input value.</param>
        /// <returns>The output value.</returns>
        /// <exception cref="InvalidCastException">
        /// The smallest key of <paramref name="niDict" /> is less than 0.
        /// </exception>
        /// <remarks>
        /// All breaks in the index list of <paramref name="niDict" /> are filled wirh default values.
        /// </remarks>
        protected internal static List<TValue> ToList(NullIndexDictionary<TValue> niDict)
        {
            List<TValue> result = null;

            if (niDict != null)
            {
                result = new List<TValue>();
                if (niDict.Count > 0)
                {
                    var minIndex = niDict.Keys.Min();
                    if (minIndex < 0)
                    {
                        // must be at least 0
                        throw new InvalidCastException();
                    }

                    // fill with default values before
                    // insertion starts
                    Enumerable.Range(start: 0,
                                     count: minIndex)
                              .ForEach(ctx =>
                              {
                                  ctx.State
                                     .List.Add(ctx.State.DefaultValue);
                              }, actionState: new
                              {
                                  DefaultValue = default(TValue),
                                  List = result,
                              });

                    // set items and fill breaks with default values
                    Enumerable.Range(start: minIndex,
                                     count: niDict.Keys.Max() - minIndex + 1)
                              .ForEach(ctx =>
                              {
                                  var dict = ctx.State.Dictionary;
                                  var key = ctx.Item;

                                  ctx.State
                                     .List.Add(dict.ContainsKey(key) ? dict[key]
                                                                     : ctx.State.DefaultValue);
                              }, actionState: new
                              {
                                  DefaultValue = default(TValue),
                                  Dictionary = niDict._INNER_DICT,
                                  List = result,
                              });
                }
            }

            return result;
        }

        private int? TryGetLastIndex()
        {
            return this._INNER_DICT
                       .Keys
                       .Cast<int?>()
                       .Max();
        }

        /// <inheriteddoc />
        public bool TryGetValue(int? key, out TValue value)
        {
            this.CheckIndex(key);

            value = default(TValue);

            if (key >= this.Count)
            {
                return false;
            }

            if (key.HasValue == false)
            {
                key = this.TryGetLastIndex();
            }

            if (key.HasValue == false)
            {
                return false;
            }

            return this._INNER_DICT
                       .TryGetValue(key: key.Value,
                                    value: out value);
        }

        #endregion Methods (20)

        #region Properties (8)

        /// <inheriteddoc />
        public int Count
        {
            get { return this._INNER_DICT.Keys.Max() + 1; }
        }

        /// <inheriteddoc />
        public bool IsFixedSize
        {
            get { return this.IsReadOnly; }
        }

        /// <inheriteddoc />
        public bool IsReadOnly
        {
            get { return this._INNER_DICT.IsReadOnly; }
        }

        /// <inheriteddoc />
        public bool IsSynchronized
        {
            get
            {
                var coll = this._INNER_DICT as ICollection;
                if (coll != null)
                {
                    return coll.IsSynchronized;
                }

                var obj = this._INNER_DICT as IObject;
                if (obj != null)
                {
                    return obj.Synchronized;
                }

                return false;
            }
        }

        /// <inheriteddoc />
        public IEnumerable<int> Keys
        {
            get
            {
                return Enumerable.Range(0, this._INNER_DICT.Count < 1 ? 0
                                                                      : this.Count);
            }
        }

        /// <inheriteddoc />
        public object SyncRoot
        {
            get
            {
                var coll = this._INNER_DICT as ICollection;
                if (coll != null)
                {
                    return coll.SyncRoot;
                }

                var obj = this._INNER_DICT as IObject;
                if (obj != null)
                {
                    return obj.SyncRoot;
                }

                return this._SYNC;
            }
        }

        /// <inheriteddoc />
        public TValue this[int? key]
        {
            get
            {
                if (key.HasValue == false)
                {
                    key = this.TryGetLastIndex();
                }

                if (key.HasValue == false)
                {
                    throw new ArgumentOutOfRangeException("key");
                }

                if (key.Value >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("key");
                }

                TValue result;
                this._INNER_DICT.TryGetValue(key.Value, out result);

                return result;
            }

            set
            {
                if (key.HasValue)
                {
                    // set / overwrite
                    this._INNER_DICT[key.Value] = value;
                }
                else
                {
                    // append item
                    this.Add(item: value);
                }
            }
        }

        /// <inheriteddoc />
        public IEnumerable<TValue> Values
        {
            get
            {
                return Enumerable.Range(0, this.Count)
                                 .Select(i =>
                                     {
                                         TValue result;
                                         this._INNER_DICT.TryGetValue(i, out result);

                                         return result;
                                     });
            }
        }

        #endregion Properties (8)

        #region Operators (2)

        /// <summary>
        /// Converts a <see cref="NullIndexDictionary{TValue}" /> to a well known
        /// <see cref="Dictionary{TKey, TValue}" /> object.
        /// </summary>
        /// <param name="dict">The input value.</param>
        /// <returns>The output value.</returns>
        public static implicit operator Dictionary<int, TValue>(NullIndexDictionary<TValue> dict)
        {
            return ToDictionary(dict);
        }

        /// <summary>
        /// Converts a <see cref="NullIndexDictionary{TValue}" /> to a well known
        /// <see cref="List{T}" /> object.
        /// </summary>
        /// <param name="dict">The input value.</param>
        /// <returns>The output value.</returns>
        /// <remarks>
        /// All breaks in the index list of <paramref name="dict" /> are filled wirh default values.
        /// </remarks>
        /// <exception cref="InvalidCastException">
        /// The smallest key of <paramref name="dict" /> is less than 0.
        /// </exception>
        public static explicit operator List<TValue>(NullIndexDictionary<TValue> dict)
        {
            return ToList(dict);
        }

        #endregion Operators (2)
    }

    #endregion CLASS: NullIndexDictionary<TValue>

    #region CLASS: NullIndexDictionary

    /// <summary>
    /// Simple implementation of <see cref="NullIndexDictionary{TValue}" /> class.
    /// </summary>
    public sealed class NullIndexDictionary : NullIndexDictionary<object>
    {
        #region Constructors (2)

        /// <inheriteddoc />
        public NullIndexDictionary()
            : base()
        {
        }

        /// <inheriteddoc />
        public NullIndexDictionary(IDictionary<int, object> innerDict)
            : base(innerDict: innerDict)
        {
        }

        #endregion Constructors (2)

        #region Operators (2)

        /// <summary>
        /// Converts a <see cref="NullIndexDictionary" /> to a well known
        /// <see cref="Dictionary{TKey, TValue}" /> object.
        /// </summary>
        /// <param name="dict">The input value.</param>
        /// <returns>The output value.</returns>
        public static implicit operator Dictionary<int, object>(NullIndexDictionary dict)
        {
            return ToDictionary(dict);
        }

        /// <summary>
        /// Converts a <see cref="NullIndexDictionary{TValue}" /> to a well known
        /// <see cref="List{T}" /> object.
        /// </summary>
        /// <param name="dict">The input value.</param>
        /// <returns>The output value.</returns>
        /// <remarks>
        /// All breaks in the index list of <paramref name="dict" /> are filled wirh default values.
        /// </remarks>
        /// <exception cref="InvalidCastException">
        /// The smallest key of <paramref name="dict" /> is less than 0.
        /// </exception>
        public static explicit operator List<object>(NullIndexDictionary dict)
        {
            return ToList(dict);
        }

        #endregion Operators (2)
    }

    #endregion CLASS: NullIndexDictionary
}