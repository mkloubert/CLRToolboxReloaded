// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Collections
{
    [TestFixture]
    public class NullIndexDictionaryTests
    {
        #region Methods (5)

        [Test]
        public void NullIndexDictionary_IDictionary()
        {
            IDictionary dict = new NullIndexDictionary<int>();

            // IDictionary<TKey, TValue>.Add(TKey, TValue)
            // IDictionary<TKey, TValue>.ContainsKey(TKey)
            {
                Assert.DoesNotThrow(() => dict.Add(null, 1));
                Assert.IsTrue(dict.Count == 1);
                Assert.IsTrue(dict.Contains(0));

                Assert.DoesNotThrow(() => dict.Add(null, 3));
                Assert.IsTrue(dict.Count == 2);
                Assert.IsTrue(dict.Contains("1"));

                Assert.DoesNotThrow(() => dict.Add(null, 2));
                Assert.IsTrue(dict.Count == 3);
                Assert.IsTrue(dict.Contains(2));

                Assert.DoesNotThrow(() => dict.Add(4, 666));
                Assert.IsTrue(dict.Count == 5);
                Assert.IsTrue(dict.Contains(3));
                Assert.IsTrue(dict.Contains(4.0f));
                Assert.IsFalse(dict.Contains(5));
            }

            // sequence
            {
                var count = 0;
                foreach (DictionaryEntry entry in dict)
                {
                    ++count;
                }

                Assert.AreEqual(count, dict.Count);
            }

            // check keys

            var test = dict.SelectEntries()
                           .Select(i => i.Key).ToArray();

            Assert.IsTrue(dict.SelectEntries()
                              .Select(i => i.Key)
                              .Cast<int?>()
                              .SequenceEqual(Enumerable.Range(0, 5).Cast<int?>()));

            // check values
            Assert.IsTrue(dict.Cast<KeyValuePair<int, int>>()
                              .Select(i => i.Value)
                              .Cast<int>()
                              .SequenceEqual(new int[] { 1, 3, 2, 0, 666 }));

            // IDictionary.Remove(object)
            {
                dict.Remove((long)1);

                Assert.IsTrue(dict.SelectEntries()
                                  .Select(i => i.Value)
                                  .Cast<int>()
                                  .Reverse()
                                  .SequenceEqual(new int[] { 666, 0, 2, 1 }));
            }

            // IDictionary.this[object]
            {
                // getter
                {
                    object value1 = default(int);
                    Assert.DoesNotThrow(() => value1 = dict[null]);
                    Assert.Catch(() => value1 = dict[5]);

                    Assert.AreEqual(value1, 666);
                }

                // setter
                {
                    Assert.DoesNotThrow(() => dict[null] = 667);
                    Assert.DoesNotThrow(() => dict[5] = 123);
                    Assert.DoesNotThrow(() => dict[7] = 668);

                    // check keys
                    Assert.IsTrue(dict.Cast<KeyValuePair<int, int>>()
                                      .Select(e => e.Key)
                                      .Cast<int?>()
                                      .SequenceEqual(Enumerable.Range(0, 8).Cast<int?>()));

                    // check values
                    Assert.IsTrue(dict.SelectEntries()
                                      .Select(e => e.Value)
                                      .Reverse()
                                      .Cast<int>()
                                      .SequenceEqual(new int[] { 668, 0, 123, 667, 666, 0, 2, 1 }));
                }
            }
        }

        [Test]
        public void NullIndexDictionary_IList()
        {
            var dict = new NullIndexDictionary<int>();
            IList list = dict;

            // Add
            {
                list.Add("23979");
                list.Add("5979");
                list.Add("1");

                Assert.Catch(() => list.Add("MK+TM"));

                Assert.AreEqual(list.Count, 3);
            }

            // IList<T>.IndexOf(T)
            {
                Assert.AreEqual(list.IndexOf("1"), 2);
                Assert.AreNotEqual(list.IndexOf("5979"), -1);
            }

            // IList<T>.Insert(int, T)
            {
                list.Insert(1, "22");

                Assert.AreEqual(list.IndexOf("22"), 1);
                Assert.AreEqual(list.IndexOf("1"), 3);

                Assert.AreEqual(list.Count, 4);

                Assert.IsTrue(list.Cast<int>()
                                  .SequenceEqual(new int[] { 23979, 22, 5979, 1 }));
            }

            // IList<T>.RemoveAt(int)
            {
                list.RemoveAt(1);

                Assert.AreEqual(list.Count, 3);

                Assert.IsTrue(list.Contains("1"));
                Assert.IsFalse(list.Contains("22"));
                Assert.IsTrue(list.Contains("5979"));
                Assert.IsTrue(list.Contains("23979"));

                Assert.AreEqual(list.IndexOf("1"), 2);
                Assert.AreEqual(list.IndexOf("22"), -1);
                Assert.AreEqual(list.IndexOf("5979"), 1);
                Assert.AreEqual(list.IndexOf("23979"), 0);

                Assert.IsTrue(list.Cast<int>()
                                  .SequenceEqual(new int[] { 23979, 5979, 1 }));
            }

            // IList<T>.this[int]
            {
                // getter
                {
                    object val = null;
                    Assert.DoesNotThrow(() => val = list[1]);

                    Assert.Throws(typeof(global::System.ArgumentOutOfRangeException),
                                  () => val = list[-1]);
                    Assert.Throws(typeof(global::System.ArgumentOutOfRangeException),
                                  () => val = list[list.Count]);

                    Assert.AreEqual(val, 5979);
                }

                // setter
                {
                    Assert.DoesNotThrow(() => list[1] = 666);
                    Assert.Throws(typeof(global::System.ArgumentOutOfRangeException),
                                  () => list[-1] = 666);
                    Assert.Throws(typeof(global::System.ArgumentOutOfRangeException),
                                  () => list[list.Count] = 666);

                    Assert.AreEqual(list[1], 666);
                }

                // breaks
                {
                    // special 1
                    dict.Add(999, 999999);
                    Assert.IsTrue(list.Count == 1000);

                    dict[123] = 321123;  // special 2
                    Assert.IsTrue(list.Count == 1000);

                    for (var i = 3; i < list.Count; i++)
                    {
                        var expectedValue = default(int);
                        switch (i)
                        {
                            case 123:
                                // special 2
                                expectedValue = 321123;
                                break;

                            case 999:
                                // special 1
                                expectedValue = 999999;
                                break;
                        }

                        Assert.AreEqual(list[i], expectedValue);
                    }
                }
            }
        }

        [Test]
        public void NullIndexDictionary_Test1_Generic_IEnumerable()
        {
            var dict = new NullIndexDictionary<long>();
            dict.Add(1);
            dict.Add(23979);
            dict.Add(5979);

            IEnumerable<long> seq1 = dict;
            IEnumerable seq2 = seq1;

            // check IEnumerable<T>
            var items1 = new List<long>();
            using (var e1 = seq1.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    items1.Add(e1.Current);
                }
            }

            // check IEnumerable
            var items2 = new List<object>();
            foreach (var item in seq2)
            {
                Assert.IsInstanceOf(typeof(global::System.Collections.Generic.KeyValuePair<int, long>),
                                    item);

                dynamic dynItem = item;
                items2.Add(dynItem.Value);
            }

            Assert.IsTrue(items1.SequenceEqual(new long[] { 1, 23979, 5979 }));
            Assert.IsTrue(items2.SequenceEqual(new object[] { (long)1, (long)23979, (long)5979 }));
        }

        [Test]
        public void NullIndexDictionary_Test1_Generic_ICollection_TValue()
        {
            ICollection<object> coll = new NullIndexDictionary();

            // ICollection<T>.Add(T)
            {
                coll.Add(23979);
                coll.Add(5979);
                coll.Add(1);
            }

            // ICollection<T>.Count
            {
                Assert.AreEqual(coll.Count, 3);
            }

            // ICollection<T>.Contains(T)
            // ICollection<T>.Remove(T)
            {
                coll.Remove(5979);

                Assert.AreEqual(coll.Count, 2);

                // ICollection<T>.Contains(T)
                Assert.IsTrue(coll.Contains(1));
                Assert.IsFalse(coll.Contains(5979));
                Assert.IsTrue(coll.Contains(23979));

                Assert.IsTrue(coll.SequenceEqual(new object[] { 23979, 1 }));
            }
        }

        [Test]
        public void NullIndexDictionary_Test1_Generic_IDictionary_TKey_TValue()
        {
            IDictionary<int?, int> dict = new NullIndexDictionary<int>();

            // IDictionary<TKey, TValue>.Add(TKey, TValue)
            // IDictionary<TKey, TValue>.ContainsKey(TKey)
            {
                Assert.DoesNotThrow(() => dict.Add(null, 1));
                Assert.IsTrue(dict.Count == 1);
                Assert.IsTrue(dict.ContainsKey(0));

                Assert.DoesNotThrow(() => dict.Add(null, 3));
                Assert.IsTrue(dict.Count == 2);
                Assert.IsTrue(dict.ContainsKey(1));

                Assert.DoesNotThrow(() => dict.Add(null, 2));
                Assert.IsTrue(dict.Count == 3);
                Assert.IsTrue(dict.ContainsKey(2));

                Assert.DoesNotThrow(() => dict.Add(4, 666));
                Assert.IsTrue(dict.Count == 5);
                Assert.IsTrue(dict.ContainsKey(3));
                Assert.IsTrue(dict.ContainsKey(4));
                Assert.IsFalse(dict.ContainsKey(5));
            }

            // sequence
            {
                var count = 0;
                foreach (KeyValuePair<int?, int> item in dict)
                {
                    ++count;
                }

                Assert.AreEqual(count, dict.Count);
            }

            // check keys
            Assert.IsTrue(dict.Select(i => i.Key)
                              .SequenceEqual(Enumerable.Range(0, 5).Cast<int?>()));

            // check values
            Assert.IsTrue(dict.Select(i => i.Value)
                              .SequenceEqual(new int[] { 1, 3, 2, 0, 666 }));

            // IDictionary<TKey, TValue>.Remove(TKey)
            {
                dict.Remove(1);

                Assert.IsTrue(dict.Select(i => i.Value)
                                  .Reverse()
                                  .SequenceEqual(new int[] { 666, 0, 2, 1 }));
            }

            // IDictionary<TKey, TValue>.this[TKey]
            {
                // getter
                {
                    int value1 = default(int);
                    Assert.DoesNotThrow(() => value1 = dict[null]);
                    Assert.Catch(() => value1 = dict[5]);

                    Assert.AreEqual(value1, 666);
                }

                // setter
                {
                    Assert.DoesNotThrow(() => dict[null] = 667);
                    Assert.DoesNotThrow(() => dict[5] = 123);
                    Assert.DoesNotThrow(() => dict[7] = 668);

                    // check keys
                    Assert.IsTrue(dict.Select(i => i.Key)
                                      .SequenceEqual(Enumerable.Range(0, 8).Cast<int?>()));

                    // check values
                    Assert.IsTrue(dict.Select(i => i.Value)
                                      .Reverse()
                                      .SequenceEqual(new int[] { 668, 0, 123, 667, 666, 0, 2, 1 }));
                }
            }

            // IDictionary<TKey, TValue>.TryGetValue(TKey, out value)
            {
                int value = -1;

                Assert.Catch(() => dict.TryGetValue(-1, out value));
                Assert.AreEqual(value, -1);

                Assert.IsTrue(dict.TryGetValue(1, out value));
                Assert.AreEqual(value, 2);

                Assert.IsFalse(dict.TryGetValue(dict.Count, out value));
                Assert.AreEqual(value, 0);
            }
        }

        [Test]
        public void NullIndexDictionary_Test1_Generic_IList_TValue()
        {
            var dict = new NullIndexDictionary<string>();
            IList<string> list = dict;

            // Add
            {
                list.Add("23979");
                list.Add("5979");
                list.Add("1");

                Assert.AreEqual(list.Count, 3);
            }

            // IList<T>.IndexOf(T)
            {
                Assert.AreEqual(list.IndexOf("1"), 2);
                Assert.AreNotEqual(list.IndexOf("5979"), -1);
            }

            // IList<T>.Insert(int, T)
            {
                list.Insert(1, "22");

                Assert.AreEqual(list.IndexOf("22"), 1);
                Assert.AreEqual(list.IndexOf("1"), 3);

                Assert.AreEqual(list.Count, 4);

                Assert.IsTrue(list.SequenceEqual(new string[] { "23979", "22", "5979", "1" }));
            }

            // IList<T>.RemoveAt(int)
            {
                list.RemoveAt(1);

                Assert.AreEqual(list.Count, 3);

                Assert.IsTrue(list.Contains("1"));
                Assert.IsFalse(list.Contains("22"));
                Assert.IsTrue(list.Contains("5979"));
                Assert.IsTrue(list.Contains("23979"));

                Assert.AreEqual(list.IndexOf("1"), 2);
                Assert.AreEqual(list.IndexOf("22"), -1);
                Assert.AreEqual(list.IndexOf("5979"), 1);
                Assert.AreEqual(list.IndexOf("23979"), 0);

                Assert.IsTrue(list.SequenceEqual(new string[] { "23979", "5979", "1" }));
            }

            // IList<T>.this[int]
            {
                // getter
                {
                    string val = null;
                    Assert.DoesNotThrow(() => val = list[1]);

                    Assert.Throws(typeof(global::System.ArgumentOutOfRangeException),
                                  () => val = list[-1]);
                    Assert.Throws(typeof(global::System.ArgumentOutOfRangeException),
                                  () => val = list[list.Count]);

                    Assert.AreEqual(val, "5979");
                }

                // setter
                {
                    Assert.DoesNotThrow(() => list[1] = "666");
                    Assert.Throws(typeof(global::System.ArgumentOutOfRangeException),
                                  () => list[-1] = "666");
                    Assert.Throws(typeof(global::System.ArgumentOutOfRangeException),
                                  () => list[list.Count] = "666");

                    Assert.AreEqual(list[1], "666");
                }

                // breaks
                {
                    // special 1
                    dict.Add(999, "999999");
                    Assert.IsTrue(list.Count == 1000);

                    dict[123] = "321123";  // special 2
                    Assert.IsTrue(list.Count == 1000);

                    for (var i = 3; i < list.Count; i++)
                    {
                        var expectedValue = default(string);
                        switch (i)
                        {
                            case 123:
                                // special 2
                                expectedValue = "321123";
                                break;

                            case 999:
                                // special 1
                                expectedValue = "999999";
                                break;
                        }

                        Assert.AreEqual(list[i], expectedValue);
                    }
                }
            }
        }

        #endregion Methods (5)
    }
}