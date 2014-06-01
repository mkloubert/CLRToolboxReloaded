// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Extensions
{
    [TestFixture]
    public class Loggers
    {
        #region Methods (5)

        [Test]
        public void AggregateLoggerTest()
        {
            var childLogger1 = new DummyLogger();

            int number = 0;
            string str = "MK+";

            var childLogger2 = DelegateLogger.Create((msg) => ++number,
                                                     (msg) =>
                                                     {
                                                         str += msg.Message;

                                                         if (number > 0)
                                                         {
                                                             throw new Exception();
                                                         }

                                                         ++number;
                                                     });

            var childLogger3 = DelegateLogger.Create((msg) => number++);

            var logger = AggregateLogger.Create(childLogger1,
                                                childLogger2,
                                                childLogger3);

            logger.Log(msg: "TM");

            Assert.AreEqual(str, "MK+TM");
            Assert.AreEqual(number, 2);

            // 3 loggers
            Assert.AreEqual(logger.GetLoggers().Count(), 3);

            // same instances?
            Assert.IsTrue(logger.GetLoggers()
                                .SequenceEqual(new ILogger[] { childLogger1,
                                                               childLogger2,
                                                               childLogger3 }));
        }

        [Test]
        public void AsyncLoggerTest()
        {
            int? threadId = null;
            string str = null;
            var childLogger = DelegateLogger.Create((msg) =>
                {
                    threadId = Thread.CurrentThread.ManagedThreadId;

                    str = msg.LogTag +
                          msg.GetMessage<string>() +
                          "tm";
                });

            var logger = new AsyncLogger(childLogger);
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
        public void DelegateLoggerTest()
        {
            string str = null;
            var ids = new HashSet<Guid>();
            var logMsgs = new HashSet<ILogMessage>();
            var logMsgsRefs = new HashSet<ILogMessage>(new DelegateEqualityComparer<ILogMessage>((x, y) => object.ReferenceEquals(x, y)));

            var logger1 = DelegateLogger.Create((msg) =>
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

            var logger2 = DelegateLogger.Create((msg) => { throw new Exception("1"); },
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

            // 3 different instances, but are equal because of their IDs
            Assert.AreEqual(logMsgs.Count, 1);
        }

        [Test]
        public void EventLoggerTest()
        {
            string str = null;

            var logger = new EventLogger();
            logger.MessageReceived += (sender, e) => str += e.Message.LogTag;
            logger.MessageReceived += (sender, e) => str += e.Message.GetMessage<string>();

            logger.Log(msg: "MK",
                       tag: "tm+");

            Assert.IsTrue(str == "TM+MK");
        }

        [Test]
        public void LogCategoriesTest()
        {
            ILogMessage lastMsg = null;
            var logger = DelegateLogger.Create((msg) => lastMsg = msg);

            logger.Log(msg: "test",
                       categories: LogCategories.Debug | LogCategories.TODO);

            Assert.IsNotNull(lastMsg);

            var categories = new List<LogCategories>();
            categories.AddRange(lastMsg.GetCategoryFlags());

            Assert.AreEqual(categories.Count, 2);

            Assert.IsTrue(lastMsg.HasAllCategories());

            // both are set
            Assert.IsTrue(lastMsg.HasAllCategories(LogCategories.TODO));
            Assert.IsTrue(lastMsg.HasAllCategories(LogCategories.Debug));

            // these are not set
            Assert.IsFalse(lastMsg.HasAllCategories(LogCategories.Warnings));
            Assert.IsFalse(lastMsg.HasAllCategories(LogCategories.Errors));

            // both are set
            Assert.IsTrue(lastMsg.HasAllCategories(LogCategories.TODO,
                                                   LogCategories.Debug));

            // 'Warnings' is not part of 'lastMsg.Categories'
            Assert.IsFalse(lastMsg.HasAllCategories(LogCategories.TODO,
                                                    LogCategories.Debug,
                                                    LogCategories.Warnings));
        }

        #endregion Methods (5)
    }
}