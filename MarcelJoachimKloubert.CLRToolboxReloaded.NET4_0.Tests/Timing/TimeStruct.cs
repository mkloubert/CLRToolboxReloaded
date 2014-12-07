// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Timing;
using NUnit.Framework;
using System;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Timing
{
    /// <summary>
    /// Tests for <see cref="Time" /> struct.
    /// </summary>
    public sealed class TimeStruct : TestFixtureBase
    {
        #region Fields (1)

        private readonly Random _RAND = new Random();

        #endregion Fields (1)

        #region Methods (11)

        [Test]
        public void Time_Add()
        {
            var time = (Time)"12:30:44.5678901";

            Assert.AreEqual(time.Add(TimeSpan.FromTicks(7)), (Time)"12:30:44.5678908");
            Assert.AreEqual(time.Add(TimeSpan.FromMilliseconds(13)), (Time)"12:30:44.5808901");
            Assert.AreEqual(time.Add(TimeSpan.FromSeconds(5)), (Time)"12:30:49.5678901");
            Assert.AreEqual(time.Add(TimeSpan.FromMinutes(12)), (Time)"12:42:44.5678901");
            Assert.AreEqual(time.Add(TimeSpan.FromHours(1)), (Time)"13:30:44.5678901");

            Assert.AreEqual(time.AddTicks(-4), (Time)"12:30:44.5678897");
            Assert.AreEqual(time.AddMilliseconds(-10), (Time)"12:30:44.5578901");
            Assert.AreEqual(time.AddSeconds(5), (Time)"12:30:49.5678901");
            Assert.AreEqual(time.AddMinutes(12), (Time)"12:42:44.5678901");
            Assert.AreEqual(time.AddHours(10), (Time)"22:30:44.5678901");

            var time2 = Time.MaxValue;
            Assert.IsFalse(time2.CanAddTicks(1));
            Assert.IsTrue(time2.CanAddTicks(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    var t = time2.AddTicks(1);
                });

            var time3 = Time.MinValue;
            Assert.IsFalse(time3.CanAddTicks(-1));
            Assert.IsTrue(time3.CanAddTicks(1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    var t = time3.AddTicks(-1);
                });
        }
        
        [Test]
        public void Time_Apply()
        {
            // DateTimeOffset
            {
                var dto1 = new DateTimeOffset(1979, 9, 23, 1, 7, 19,
                                              TimeSpan.FromHours(2)).AddMilliseconds(810);
                var dto2 = new DateTimeOffset(1979, 9, 23, 5, 9, 19,
                                              TimeSpan.FromHours(2)).AddMilliseconds(790);
                var dto3 = dto1 <= Time.Parse("05:09:19.79");

                Assert.AreEqual(dto2, dto3);
                Assert.AreNotEqual(dto1, dto3);
            }

            // DateTime
            {
                var dt1 = new DateTime(1979, 9, 5, 0, 12, 19).AddMilliseconds(790);
                var dt2 = new DateTime(1979, 9, 5, 23, 9, 19).AddMilliseconds(790);
                var dt3 = dt1 <= (Time)"23:09:19.790";

                Assert.AreEqual(dt2, dt3);
                Assert.AreNotEqual(dt1, dt3);
            }
        }

        [Test]
        public void Time_Compare()
        {
            var time = (Time)"12:30";

            for (var i = 0; i < 1000000; i++)
            {
                var secs = (double)this._RAND.Next(1, 3600);

                var t1 = time.AddMinutes(secs / 60.0);
                var t2 = t1.SubtractMinutes(secs / 60.0 * 2.0);
                var t3 = t2.AddSeconds(secs);

                // time < t1
                Assert.IsTrue(time < t1);
                Assert.AreEqual(time.CompareTo(t1), -1);
                Assert.AreEqual(Time.Compare(time, t1), -1);

                // time > t2
                Assert.IsTrue(time > t2);
                Assert.AreEqual(time.CompareTo(t2), 1);
                Assert.AreEqual(Time.Compare(time, t2), 1);

                // time == t3
                Assert.IsTrue(time == t3);
                Assert.AreEqual(time.CompareTo(t3), 0);
                Assert.AreEqual(Time.Compare(time, t3), 0);
            }
        }

        [Test]
        public void Time_Conversion()
        {
            Assert.AreEqual(new Time(hours: 12, minutes: 30, secs: 44),
                            (Time)"12:30:44");

            for (var i = 0; i < 1000000; i++)
            {
                var t1 = new Time(hours: this._RAND.Next(0, 24),
                                  minutes: this._RAND.Next(0, 60),
                                  secs: this._RAND.Next(0, 60),
                                  msecs: this._RAND.Next(0, 1000),
                                  ticks: this._RAND.Next(0, 10000));
                var str = (string)t1;
                var t2 = (Time)str;

                TimeSpan ts1 = t1;
                TimeSpan ts2 = t2;

                Assert.AreEqual(t1, t2);

                Assert.AreEqual(ts1, ts2);

                Assert.AreEqual(ts1.Ticks, t1.Ticks);
                Assert.AreEqual(ts1.Ticks, t2.Ticks);

                Assert.AreEqual(ts2.Ticks, t1.Ticks);
                Assert.AreEqual(ts2.Ticks, t2.Ticks);

                Assert.AreEqual(t1.ToString(), str);
                Assert.AreEqual(t2.ToString(), str);
            }
        }

        [Test]
        public void Time_Difference()
        {
            var t1 = (Time)"05:09:19.79";
            var t2 = (Time)"23:09:19.79";

            Assert.AreEqual(t1 - t2, TimeSpan.FromHours(-18));
            Assert.AreEqual(t2 - t1, TimeSpan.FromHours(18));
        }

        [Test]
        public void Time_ForEach()
        {
            // test 1
            {
                var start = (Time)"01:00";

                var seq = start * TimeSpan.FromHours(1);
                Assert.AreEqual(seq.Count(), 23);
                Assert.AreEqual(seq.SkipWhile(t => t.Hours < 3)
                                   .TakeWhile(t => t.Hours < 22)
                                   .Count(), 19);
            }

            // test 2
            {
                var start = (Time)"02:00";

                var seq = TimeSpan.FromHours(0.5).Ticks * start;
                Assert.AreEqual(seq.Count(), 44);
                Assert.AreEqual(seq.SkipWhile(t => t.Hours < 4)
                                   .TakeWhile(t => t.Hours < 20)
                                   .Count(), 32);
            }
        }

        [Test]
        public void Time_IncrementDecrement()
        {
            var oldIncrementBy = Time.IncrementBy;
            var oldDecrementBy = Time.DecrementBy;
            try
            {
                Time.IncrementByMinutes = 12;
                Time.DecrementByHours = 7;
                {
                    var t = (Time)"21:50";

                    Assert.AreEqual(++t, (Time)"22:02:00");
                    Assert.AreEqual(t--, (Time)"22:02:00");
                    Assert.AreEqual(t, (Time)"15:02:00");
                    Assert.AreEqual(--t, (Time)"08:02:00");
                    Assert.AreEqual(t++, (Time)"08:02:00");
                    Assert.AreEqual(t, (Time)"08:14:00");

                    Assert.IsTrue(t.CanDecrement());
                    --t;
                    t++;
                    Assert.IsFalse(t.CanDecrement());

                    Assert.AreEqual(t.Hours, 1);
                    Assert.AreEqual(t.Minutes, 26);
                    Assert.AreEqual(t.Seconds, 0);
                    Assert.AreEqual(t.Milliseconds, 0);
                }
            }
            finally
            {
                Time.IncrementBy = oldIncrementBy;
                Time.DecrementBy = oldDecrementBy;
            }
        }

        [Test]
        public void Time_Normalize()
        {
            var time = (Time)"12:30:44.56789";

            Assert.AreEqual(time.NormalizeMilliseconds(), (Time)"12:30:44.56700");
            Assert.AreEqual(time.NormalizeSeconds(), (Time)"12:30:44.00000");
            Assert.AreEqual(time.NormalizeMinutes(), (Time)"12:30:00.00000");
            Assert.AreEqual(time.NormalizeHours(), (Time)"12:00:00.00000");
        }

        [Test]
        public void Time_Parse()
        {
            // valid
            Assert.DoesNotThrow(() =>
                {
                    var t = Time.Parse("23:09:19.79");
                });
            
            // out of range
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                {
                    var t = Time.Parse("1.00:12:19.79");
                });

            // OK
            Time t1;
            Assert.IsTrue(Time.TryParse("05:09:19.79", out t1));

            // invalid format
            Time t2;
            Assert.IsFalse(Time.TryParse("24:12:19.79", out t2));

            // (null) / invalid format
            Time t3;
            Assert.IsFalse(Time.TryParse(null, out t3));

            // empty / invalid format
            Time t4;
            Assert.IsFalse(Time.TryParse(string.Empty, out t4));

            // whitespace / invalid format
            Time t5;
            Assert.IsFalse(Time.TryParse("        ", out t5));

            // out of range
            Time t6;
            Assert.IsFalse(Time.TryParse("1.00:12:19.79", out t6));
        }

        [Test]
        public void Time_Subtract()
        {
            var time = (Time)"12:30:44.5678901";

            Assert.AreEqual(time.Subtract(TimeSpan.FromTicks(-7)), (Time)"12:30:44.5678908");
            Assert.AreEqual(time.Subtract(TimeSpan.FromMilliseconds(-13)), (Time)"12:30:44.5808901");
            Assert.AreEqual(time.Subtract(TimeSpan.FromSeconds(-5)), (Time)"12:30:49.5678901");
            Assert.AreEqual(time.Subtract(TimeSpan.FromMinutes(-12)), (Time)"12:42:44.5678901");
            Assert.AreEqual(time.Subtract(TimeSpan.FromHours(-1)), (Time)"13:30:44.5678901");

            Assert.AreEqual(time.SubtractTicks(4), (Time)"12:30:44.5678897");
            Assert.AreEqual(time.SubtractMilliseconds(10), (Time)"12:30:44.5578901");
            Assert.AreEqual(time.SubtractSeconds(-5), (Time)"12:30:49.5678901");
            Assert.AreEqual(time.SubtractMinutes(-12), (Time)"12:42:44.5678901");
            Assert.AreEqual(time.SubtractHours(-10), (Time)"22:30:44.5678901");
        }

        [Test]
        public void Time_ToEndOf()
        {
            var time = (Time)"12:30:44.5678901";

            // msec
            Assert.AreEqual(time.ToEndOfMillisecond(), (Time)"12:30:44.5679999");
            Assert.AreEqual(time.ToEndOfMillisecond()
                                .AddTicks(1), (Time)"12:30:44.568");
            Assert.AreEqual(time.ToEndOfMillisecond()
                                .AddTicks(-1), (Time)"12:30:44.5679998");

            // sec
            Assert.AreEqual(time.ToEndOfSecond(), (Time)"12:30:44.9999999");
            Assert.AreEqual(time.ToEndOfSecond()
                                .AddTicks(1), (Time)"12:30:45");
            Assert.AreEqual(time.ToEndOfSecond()
                                .AddTicks(-1), (Time)"12:30:44.9999998");

            // min
            Assert.AreEqual(time.ToEndOfMinute(), (Time)"12:30:59.9999999");
            Assert.AreEqual(time.ToEndOfMinute()
                                .AddTicks(1), (Time)"12:31");
            Assert.AreEqual(time.ToEndOfMinute()
                                .AddTicks(-1), (Time)"12:30:59.9999998");

            // hour
            Assert.AreEqual(time.ToEndOfHour(), (Time)"12:59:59.9999999");
            Assert.AreEqual(time.ToEndOfHour()
                                .AddTicks(1), (Time)"13:00");
            Assert.AreEqual(time.ToEndOfHour()
                                .AddTicks(-1), (Time)"12:59:59.9999998");
        }

        #endregion Methods (8)
    }
}