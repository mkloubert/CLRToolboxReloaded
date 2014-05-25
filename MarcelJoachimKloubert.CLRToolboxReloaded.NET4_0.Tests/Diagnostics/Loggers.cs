// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMAggregateLogger = MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.AggregateLogger;
using TMAsyncLogger = MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.AsyncLogger;
using TMDelegateLogger = MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.DelegateLogger;
using TMLogCategories = MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.LogCategories;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Extensions
{
    [TestFixture]
    public class Loggers
    {
        #region Methods (4)

        [Test]
        public void AggregateLogger()
        {
            var childLogger1 = new DummyLogger();

            int number = 0;
            string str = "MK+";

            var childLogger2 = new DelegateLogger();
            childLogger2.Add((msg) => ++number);
            childLogger2.Add((msg) =>
            {
                str += msg.Message;

                if (number > 0)
                {
                    throw new Exception();
                }

                ++number;
            });

            var childLogger3 = new DelegateLogger();
            childLogger3.Add((msg) => number++);

            var logger = TMAggregateLogger.Create(childLogger1,
                                                  childLogger2,
                                                  childLogger3);

            logger.Log(msg: "TM");

            Assert.AreEqual(str, "MK+TM");
            Assert.AreEqual(number, 2);

            // 3 loggers
            Assert.AreEqual(logger.GetLoggers().Count, 3);

            // same instances?
            Assert.IsTrue(logger.GetLoggers()
                                .SequenceEqual(new ILogger[] { childLogger1,
                                                               childLogger2,
                                                               childLogger3 }));
        }

        [Test]
        public void AsyncLogger()
        {
            int? threadId = null;
            string str = null;
            var childLogger = TMDelegateLogger.Create((msg) =>
                {
                    threadId = Thread.CurrentThread.ManagedThreadId;

                    str = msg.LogTag +
                          msg.GetMessage<string>() +
                          "tm";
                });

            var logger = new TMAsyncLogger(childLogger);
            logger.Log("mk+",
                       "l://");

            var timedOut = true;
            var start = DateTimeOffset.Now;
            TimeSpan duration;
            do
            {
                duration = DateTimeOffset.Now - start;

                if (str != null)
                {
                    timedOut = false;
                    break;
                }
            }
            while (duration <= TimeSpan.FromSeconds(30));

            // the job should be done in 30 seconds
            Assert.IsFalse(timedOut, message: "Timed out!");

            // a non-null value means: executed
            Assert.AreEqual(str, "L://mk+tm");

            // should not work in the same thread
            Assert.AreNotEqual(threadId,
                               Thread.CurrentThread.ManagedThreadId);
        }

        [Test]
        public void DelegateLogger()
        {
            string str = null;
            var ids = new HashSet<Guid>();
            var logMsgs = new HashSet<ILogMessage>();
            var logMsgsRefs = new HashSet<ILogMessage>(new DelegateEqualityComparer<ILogMessage>((x, y) => object.ReferenceEquals(x, y)));

            var logger1 = TMDelegateLogger.Create((msg) =>
                                                  {
                                                      ids.Add(msg.Id);
                                                      logMsgs.Add(msg);
                                                      logMsgsRefs.Add(msg);

                                                      if (string.IsNullOrEmpty(str))
                                                      {
                                                          str += msg.GetMessage<string>();
                                                      }
                                                  },
                                                  (msg) =>
                                                  {
                                                      ids.Add(msg.Id);
                                                      logMsgs.Add(msg);
                                                      logMsgsRefs.Add(msg);

                                                      if (str != string.Empty)
                                                      {
                                                          throw new Exception();
                                                      }

                                                      // that should not happen
                                                      str += "+YS";
                                                  },
                                                  (msg) =>
                                                  {
                                                      ids.Add(msg.Id);
                                                      logMsgs.Add(msg);
                                                      logMsgsRefs.Add(msg);

                                                      if (str != string.Empty)
                                                      {
                                                          str += "+TM";
                                                      }
                                                      else
                                                      {
                                                          str += "+YS";
                                                      }
                                                  });

            var logger2 = TMDelegateLogger.Create((msg) => { throw new Exception("1"); },
                                                  (msg) => { throw new Exception("2"); },
                                                  (msg) => { throw new Exception("3"); });

            var result1 = logger1.Log(msg: new char[] { 'M', 'K' });
            var result2 = logger2.Log(msg: new DateTime(1979, 9, 5, 23, 9, 19, 79));

            // not all failed => true
            Assert.IsTrue(result1);
            // all failed => false
            Assert.IsFalse(result2);

            Assert.IsTrue(str == "MK+TM");

            // all message objects have the same ID
            Assert.AreEqual(ids.Count, 1);

            // 3 different instances
            Assert.AreEqual(logMsgsRefs.Count, 3);

            // 3 different instances, but are equal because of theit IDs
            Assert.AreEqual(logMsgs.Count, 1);
        }

        [Test]
        public void LogCategories()
        {
            ILogMessage lastMsg = null;
            var logger = TMDelegateLogger.Create((msg) => lastMsg = msg);

            logger.Log(msg: "test",
                       categories: TMLogCategories.Debug | TMLogCategories.TODO);

            Assert.IsNotNull(lastMsg);

            var categories = new List<TMLogCategories>();
            categories.AddRange(lastMsg.GetCategoryFlags());

            Assert.AreEqual(categories.Count, 2);
            
            Assert.IsTrue(lastMsg.HasAllCategories());

            // both are set
            Assert.IsTrue(lastMsg.HasAllCategories(TMLogCategories.TODO));
            Assert.IsTrue(lastMsg.HasAllCategories(TMLogCategories.Debug));
            
            // these are not set
            Assert.IsFalse(lastMsg.HasAllCategories(TMLogCategories.Warnings));
            Assert.IsFalse(lastMsg.HasAllCategories(TMLogCategories.Errors));
            
            // both are set
            Assert.IsTrue(lastMsg.HasAllCategories(TMLogCategories.TODO,
                                                   TMLogCategories.Debug));

            // 'Warnings' is not part of 'lastMsg.Categories'
            Assert.IsFalse(lastMsg.HasAllCategories(TMLogCategories.TODO,
                                                    TMLogCategories.Debug,
                                                    TMLogCategories.Warnings));
        }

        #endregion Methods (4)
    }
}