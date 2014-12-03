// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using NUnit.Framework;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Extensions
{
    [TestFixture]
    public class Reflection
    {
        #region Methods (2)

        [Test]
        public void TryGetDelegateTypeFromMethod()
        {
            var actionType = this.GetType().GetMethod("Method_Action", BindingFlags.NonPublic | BindingFlags.Instance)
                                           .TryGetDelegateTypeFromMethod();
            var action1TypeInt = this.GetType().GetMethod("Method_Action1_int", BindingFlags.NonPublic | BindingFlags.Instance)
                                               .TryGetDelegateTypeFromMethod();
            var action1TypeLong = this.GetType().GetMethod("Method_Action1_long", BindingFlags.NonPublic | BindingFlags.Instance)
                                                .TryGetDelegateTypeFromMethod();

            var funcType = this.GetType().GetMethod("Method_Func", BindingFlags.NonPublic | BindingFlags.Instance)
                                         .TryGetDelegateTypeFromMethod();
            var func1TypeInt = this.GetType().GetMethod("Method_Func1_int", BindingFlags.NonPublic | BindingFlags.Instance)
                                             .TryGetDelegateTypeFromMethod();
            var func1TypeLong = this.GetType().GetMethod("Method_Func1_long", BindingFlags.NonPublic | BindingFlags.Instance)
                                              .TryGetDelegateTypeFromMethod();

            Assert.AreEqual(actionType, typeof(global::System.Action));

            Assert.AreEqual(action1TypeInt, typeof(global::System.Action<int>));
            Assert.AreNotEqual(action1TypeInt, typeof(global::System.Action<long>));

            Assert.AreEqual(action1TypeLong, typeof(global::System.Action<long>));
            Assert.AreNotEqual(action1TypeLong, typeof(global::System.Action<int>));

            Assert.AreEqual(funcType, typeof(global::System.Func<object>));
            Assert.AreNotEqual(funcType, typeof(global::System.Func<int>));

            Assert.AreEqual(func1TypeInt, typeof(global::System.Func<int, string>));
            Assert.AreNotEqual(func1TypeInt, typeof(global::System.Func<long, string>));
            Assert.AreNotEqual(func1TypeInt, typeof(global::System.Func<long, double>));

            Assert.AreEqual(func1TypeLong, typeof(global::System.Func<long, double>));
            Assert.AreNotEqual(func1TypeLong, typeof(global::System.Func<int, double>));
            Assert.AreNotEqual(func1TypeLong, typeof(global::System.Func<int, string>));
        }

        private void Method_Action()
        {
        }

        private void Method_Action1_int(int a)
        {
        }

        private void Method_Action1_long(long a)
        {
        }

        private object Method_Func()
        {
            return 1;
        }

        private string Method_Func1_int(int a)
        {
            return a.ToString();
        }

        private double Method_Func1_long(long a)
        {
            return a;
        }

        #endregion Methods (2)
    }
}