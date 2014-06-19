// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution;
using MarcelJoachimKloubert.CLRToolbox.Threading;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Execution
{
    public sealed class Mediators : TestFixtureBase
    {
        #region Methods (1)

        [Test]
        public void TestMediator()
        {
            var mediator = new Mediator();

            var sum1 = 0;
            var sum2 = 0;
            var threadIds = new HashSet<int>();

            // same thread
            var action1 = new MediatorAction<int>((i) =>
                {
                    sum1 += i;
                    threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                });
            var action2 = new MediatorAction<int>((i) =>
                {
                    sum1 += i;
                    threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                });

            // background thread
            var action3 = new MediatorAction<int>((i) =>
                {
                    sum2 += i;
                    threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                });
            var action4 = new MediatorAction<int>((i) =>
                {
                    sum2 += i;
                    threadIds.Add(Thread.CurrentThread.ManagedThreadId);
                });

            mediator.Subscribe(action1)
                    .Subscribe(action2)
                    .Subscribe(action3, ThreadOption.Background)
                    .Subscribe(action4, ThreadOption.Background)
                    .Subscribe(action4, ThreadOption.Background);

            mediator.Publish(1);

            Assert.AreEqual(sum1, 2);
            Assert.AreEqual(sum2, 3);
            Assert.IsTrue(threadIds.Count > 1 &&
                          threadIds.Count < 5);

            mediator.Unsubscribe(action4);

            sum1 = 1;
            sum2 = 2;
            threadIds.Clear();
            mediator.Publish(2);

            Assert.AreEqual(sum1, 5);
            Assert.AreEqual(sum2, 4);
            Assert.IsTrue(threadIds.Count == 2);
        }

        #endregion Methods (1)
    }
}