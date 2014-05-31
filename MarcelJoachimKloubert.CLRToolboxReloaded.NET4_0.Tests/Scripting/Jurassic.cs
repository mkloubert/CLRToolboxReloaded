// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Scripting;
using NUnit.Framework;
using System;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Extensions
{
    [TestFixture]
    public class Jurassic
    {
        #region CLASS: TestClass

        private sealed class TestClass
        {
            public int Test
            {
                get;
                set;
            }
        }

        #endregion

        #region Methods (1)

        [Test]
        public void Functions()
        {
            var executor = new JurassicScriptExecutor();

            var test1 = false;
            var action1 = new Action(() => test1 = true);
            
            var action2 = new Func<int, int>((input) => input * 2);

            var test3 = new TestClass();
            executor.SetVariable("test3", test3);

            executor.SetFunction("testAction1", action1);
            executor.SetFunction("testAction2", action2);

            var ctx = executor.Execute(@"
testAction1();
test3.Test = 1 + testAction2(4) * 2;
");

            Assert.IsTrue(test1);
            Assert.AreEqual(test3.Test, 17);
        }

        #endregion Methods (1)
    }
}