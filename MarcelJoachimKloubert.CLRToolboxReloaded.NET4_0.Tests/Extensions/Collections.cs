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
    [TestFixture]
    public class Collections
    {
        #region Methods (6)

        [Test]
        public void AsArray()
        {
            IEnumerable a = new object[] { 1, 2, 3 };
            IEnumerable<object> b = Enumerable.Empty<object>()
                                              .Concat(new object[] { 1, 2, 3 });
            IEnumerable c = null;

            Assert.AreSame(a, a.AsArray());
            Assert.AreNotSame(b, b.AsArray());
            Assert.IsNull(c.AsArray());
        }

        [Test]
        public void CreateTasksForAll()
        {
            var a = new int[] { 1, 2, 3, 4 };

            int result = 0;
            var tasks = a.CreateTasksForAll(ctx =>
                {
                    result += ctx.Item;
                }).ToArray();

            Assert.IsTrue(tasks.Count() == 4);
            Assert.IsTrue(result == 0);

            foreach (var t in tasks)
            {
                t.Start();
            }
            Task.WaitAll(tasks);

            Assert.IsTrue(result == 10);
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
            var a = new int[] { 1, 2, 3, 4 };

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

            Assert.IsTrue(result1 == 7);
            Assert.IsTrue(result2 == 10);
            Assert.IsTrue(result3 == 8);
            Assert.IsNull(ex1);
            Assert.IsNull(ex2);
            Assert.IsNotNull(ex3);
        }

        [Test]
        public void ForAllAsync()
        {
            var a = new int[] { 1, 2, 3, 4 };

            int result1 = 0;
            AggregateException ex1 = null;
            Assert.Throws(typeof(global::System.AggregateException),
                          () =>
                          {
                              ex1 = a.ForAllAsync(ctx =>
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
                                    ex2 = a.ForAllAsync(ctx =>
                                                        {
                                                            result2 += ctx.Item;
                                                        });
                                });

            int result3 = 0;
            AggregateException ex3 = null;
            Assert.DoesNotThrow(() =>
                                {
                                    ex3 = a.ForAllAsync(action: ctx =>
                                                        {
                                                            if (ctx.Index == 1)
                                                            {
                                                                throw new Exception();
                                                            }

                                                            result3 += ctx.Item;
                                                        }, throwExceptions: false);
                                });

            Assert.IsTrue(result1 == 7);
            Assert.IsTrue(result2 == 10);
            Assert.IsTrue(result3 == 8);
            Assert.IsNull(ex1);
            Assert.IsNull(ex2);
            Assert.IsNotNull(ex3);
        }

        [Test]
        public void ForEach()
        {
            var a = new int[] { 1, 2, 3 };

            int result1 = 0;
            try
            {
                a.ForEach(ctx =>
                    {
                        if (ctx.Index == 2)
                        {
                            throw new Exception();
                        }

                        result1 += ctx.Item;
                    });
            }
            catch
            {
            }

            int result2 = 0;
            a.ForEach(ctx =>
                {
                    result2 += ctx.Item;
                });

            int result3 = 0;
            a.ForEach(ctx =>
                {
                    if (ctx.Index == 1)
                    {
                        ctx.Cancel = true;
                        return;
                    }

                    result3 += ctx.Item;
                });

            Assert.IsTrue(result1 == 3);
            Assert.IsTrue(result2 == 6);
            Assert.IsTrue(result3 == 1);
        }

        #endregion Methods (6)
    }
}