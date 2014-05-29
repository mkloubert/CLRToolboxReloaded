﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.ComponentModel
{
    [TestFixture]
    public sealed class Notification
    {
        #region CLASS: TestNotifiable

        public sealed class TestNotifiable : NotifiableBase
        {
            public int ReceivedValueFrom_Test1_Count = 0;
            public readonly List<string> ReceivedValueFrom_Test1_Methods = new List<string>();

            public int Test1
            {
                get { return this.Get<int>("Test1"); }

                set { this.Set(value, "Test1"); }
            }

            [ReceiveNotificationFrom("Test1")]
            public int Test2
            {
                get { return this.Test1; }
            }

            public int Test3
            {
                get { return this.Test1; }
            }

            [ReceiveValueFrom("Test1")]
            private void ReceiveFrom_Test1_1(IReceiveValueFromArgs args)
            {
                this.ReceivedValueFrom_Test1_Count++;
                this.ReceivedValueFrom_Test1_Methods.Add("ReceiveFrom_Test1_1");
            }

            [ReceiveValueFrom("Test1")]
            private void ReceiveFrom_Test1_2()
            {
                this.ReceivedValueFrom_Test1_Count++;
                this.ReceivedValueFrom_Test1_Methods.Add("ReceiveFrom_Test1_2");
            }

            private void ReceiveFrom_Test1_3(IReceiveValueFromArgs args)
            {
            }
        }

        #endregion CLASS: TestNotifiable

        #region Methods (1)

        [Test]
        public void NotifiableTest1()
        {
            var r = new Random();

            var changedProperties = new List<string>();
            var changingProperties = new List<string>();

            var obj = new TestNotifiable();
            obj.PropertyChanged += (sender, e) =>
                {
                    changedProperties.Add(e.PropertyName);
                };
            obj.PropertyChanging += (sender, e) =>
                {
                    changingProperties.Add(e.PropertyName);
                };

            // generate a test value
            var test1Val = r.Next(minValue: 1,
                             maxValue: int.MaxValue);
            obj.Test1 = test1Val;

            Assert.AreEqual(obj.Test1, test1Val);

            // PropertyChanged
            Assert.IsTrue(changedProperties.Contains("Test1"));
            Assert.IsTrue(changedProperties.Contains("Test2"));
            Assert.IsFalse(changedProperties.Contains("Test3"));

            // PropertyChanging
            Assert.IsTrue(changingProperties.Contains("Test1"));
            Assert.IsTrue(changingProperties.Contains("Test2"));
            Assert.IsFalse(changingProperties.Contains("Test3"));

            // ReceivedValueFromAttribute
            Assert.IsTrue(obj.ReceivedValueFrom_Test1_Methods.Contains("ReceiveFrom_Test1_1"));
            Assert.IsTrue(obj.ReceivedValueFrom_Test1_Methods.Contains("ReceiveFrom_Test1_2"));
            Assert.IsFalse(obj.ReceivedValueFrom_Test1_Methods.Contains("ReceiveFrom_Test1_3"));
            Assert.IsFalse(obj.ReceivedValueFrom_Test1_Methods.Contains("ReceiveFrom_Test1_4"));

            Assert.AreEqual(obj.ReceivedValueFrom_Test1_Count, 2);
        }

        #endregion Methods (1)
    }
}