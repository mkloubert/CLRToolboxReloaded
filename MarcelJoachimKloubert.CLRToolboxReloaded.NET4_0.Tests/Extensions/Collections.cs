// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Extensions
{
    public class Collections : TestFixtureBase
    {
        #region Methods (7)

        [Test]
        public void AsArray()
        {
            IEnumerable a = new object[] { 1, 2, 3 };
            IEnumerable<object> b = Enumerable.Empty<object>()
                                              .Concat(new object[] { 1, 2, 3 });
            IEnumerable c = null;

            var d = new List<long>();
            d.AddRange(Enumerable.Empty<long>()
                                 .Concat(new long[] { 1, 2, 3, 4, 5, 6 }));

            // should have the same instance
            Assert.AreSame(a, a.AsArray());

            // should create a new array instance
            // because input is no array
            Assert.AreNotSame(b, b.AsArray());

            // (null) returns (null)
            Assert.IsNull(c.AsArray());

            // should create a new array instance
            // because input is no array
            Assert.AreNotSame(d, d.AsArray());
        }

        [Test]
        public void CreateTasksForAll()
        {
            var a = new int[] { 0, 1, 2, 4, 8, 16 };

            int result = 0;
            var tasks = a.CreateTasksForAll(ctx =>
                {
                    lock (ctx.SyncRoot)
                    {
                        result += ctx.Item;
                    }
                }).ToArray();

            // one task for each item
            Assert.IsTrue(tasks.Count() == 6);
            // nothing executed yet
            Assert.IsTrue(result == 0);

            foreach (var t in tasks)
            {
                t.Start();
            }
            Task.WaitAll(tasks);

            Assert.IsTrue(result == 31);
        }

        [Test]
        public void AsSequence()
        {
            IEnumerable a = new object[] { 1, 2, 3 };
            IEnumerable<object> b = Enumerable.Empty<object>()
                                              .Concat(new object[] { 1, 2, 3 });
            IEnumerable c = null;

            Assert.AreSame(a, a.AsSequence<object>());
            Assert.AreNotSame(b, b.AsSequence<int>());
            Assert.IsNull(c.AsSequence<object>());
        }

        [Test]
        public void ForAll()
        {
            var a = new int[] { 1, 2, 4, 8 };

            int result1 = 0;
            AggregateException ex1 = null;
            Assert.Throws(typeof(global::System.AggregateException),
                          () =>
                          {
                              ex1 = a.ForAll(ctx =>
                                             {
                                                 if (ctx.Index == 2)
                                                 {
                                                     throw new Exception();
                                                 }

                                                 result1 += ctx.Item;
                                             });
                          });

            int result2 = 0;
            AggregateException ex2 = new AggregateException();
            Assert.DoesNotThrow(() =>
                                {
                                    ex2 = a.ForAll(ctx =>
                                                   {
                                                       result2 += ctx.Item;
                                                   });
                                });

            int result3 = 0;
            AggregateException ex3 = null;
            Assert.DoesNotThrow(() =>
                                {
                                    ex3 = a.ForAll(action: ctx =>
                                                   {
                                                       if (ctx.Index == 1)
                                                       {
                                                           throw new Exception();
                                                       }

                                                       result3 += ctx.Item;
                                                   }, throwExceptions: false);
                                });

            Assert.IsTrue(result1 == 11);
            Assert.IsTrue(result2 == 15);
            Assert.IsTrue(result3 == 13);

            // exception was thrown
            Assert.IsNull(ex1);

            // no error occured
            Assert.IsNull(ex2);

            // exception returned
            Assert.IsNotNull(ex3);
        }

        [Test]
        public void ForAllAsync()
        {
            var a = new int[] { 0, 1, 2, 4 };

            int result1 = 0;
            AggregateException ex1 = null;
            Assert.Throws(typeof(global::System.AggregateException),
                          () =>
                          {
                              ex1 = a.ForAllAsync(ctx =>
                                                  {
                                                      lock (ctx.SyncRoot)
                                                      {
                                                          if (ctx.Index == 2)
                                                          {
                                                              throw new Exception();
                                                          }

                                                          result1 += ctx.Item;
                                                      }
                                                  });
                          });

            int result2 = 0;
            AggregateException ex2 = new AggregateException();
            Assert.DoesNotThrow(() =>
                                {
                                    ex2 = a.ForAllAsync(ctx =>
                                                        {
                                                            lock (ctx.SyncRoot)
                                                            {
                                                                result2 += ctx.Item;
                                                            }
                                                        });
                                });

            int result3 = 0;
            AggregateException ex3 = null;
            Assert.DoesNotThrow(() =>
                                {
                                    ex3 = a.ForAllAsync(action: ctx =>
                                                        {
                                                            lock (ctx.SyncRoot)
                                                            {
                                                                if (ctx.Index == 1)
                                                                {
                                                                    throw new Exception();
                                                                }

                                                                result3 += ctx.Item;
                                                            }
                                                        }, throwExceptions: false);
                                });

            Assert.IsTrue(result1 == 5);
            Assert.IsTrue(result2 == 7);
            Assert.IsTrue(result3 == 6);
            Assert.IsNull(ex1);
            Assert.IsNull(ex2);
            Assert.IsNotNull(ex3);
        }

        [Test]
        public void ForEach()
        {
            var a = new int[] { 1, 2, 3 };

            int result1 = 0;
            int executionCount1 = 0;
            Assert.Throws(typeof(global::System.Exception),
                          () =>
                          {
                              a.ForEach(ctx =>
                                        {
                                            if (ctx.Index == 2)
                                            {
                                                throw new Exception();
                                            }

                                            result1 += ctx.Item;

                                            ++executionCount1;
                                        });
                          });

            int result2 = 0;
            int executionCount2 = 0;
            a.ForEach(ctx =>
                {
                    result2 += ctx.Item;

                    ++executionCount2;
                });

            int result3 = 0;
            int executionCount3 = 0;
            bool canceled3 = false;
            a.ForEach(ctx =>
                {
                    if (ctx.Index == 1)
                    {
                        ctx.Cancel = true;
                        canceled3 = true;

                        return;
                    }

                    result3 += ctx.Item;

                    ++executionCount3;
                });

            Assert.IsTrue(result1 == 3);
            Assert.AreEqual(executionCount1, 2);  // last executed failed

            Assert.IsTrue(result2 == 6);
            Assert.AreEqual(executionCount2, a.Length);  // no error and no cancellation

            Assert.IsTrue(result3 == 1);
            Assert.IsTrue(canceled3);  // canceled
            Assert.AreEqual(executionCount3, 1);  // second execution was canceled
        }

        [Test]
        public void TryGetValue()
        {
            var dict = new Dictionary<string, object>();
            dict["a"] = 1;

            long val1;
            var res1 = dict.TryGetValue<long>("a", out val1);

            long val2;
            var res2 = dict.TryGetValue<long>("A", out val2, 2);

            string val3;
            var res3 = dict.TryGetValue<string>("b", out val3, (k, t) => k == "B" ? "3" : "4");

            Assert.IsTrue(res1);
            Assert.AreEqual(val1, 1L);

            Assert.IsFalse(res2);
            Assert.AreEqual(val2, 2L);

            Assert.IsFalse(res3);
            Assert.AreEqual(val3, "4");
        }

        #endregion Methods (7)
    }
}