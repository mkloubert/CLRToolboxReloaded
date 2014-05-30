// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Collections
{
    [TestFixture]
    public class StackTests
    {
        #region Methods (2)

        [Test]
        public void PeekOrDefaultTest()
        {
            var stack1 = new Stack<object>();
            stack1.Push(1);

            var stack2 = new Stack<object>();

            Assert.IsNotNull(stack1.PeekOrDefault());
            Assert.IsNull(stack2.PeekOrDefault());
        }

        [Test]
        public void PushRangeTest()
        {
            var items = Enumerable.Range(0, 23979);

            var stack = new Stack<int>();
            stack.PushRange(items);

            Assert.IsTrue(items.SequenceEqual(stack.Reverse()));
        }

        #endregion Methods (2)
    }
}