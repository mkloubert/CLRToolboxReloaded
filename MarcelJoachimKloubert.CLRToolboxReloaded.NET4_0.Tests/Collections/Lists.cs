// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Collections
{
    [TestFixture]
    public class Lists
    {
        #region Methods (1)

        [Test]
        public void Transform()
        {
            var list1 = new List<object>(Enumerable.Range(0, 5979).Cast<object>());
            Assert.IsTrue(list1.All(i => i is int));

            var list2 = Enumerable.Range(0, 23979).Cast<object>().ToArray();

            // Int32 => Int64
            list1.Transform((ctx) => (long)((int)ctx.Item));
            Assert.IsTrue(list1.All(i => i is long));

            // convert first 5979 items from Int32 => String
            list2.Transform((ctx) =>
            {
                var result = ctx.Item;

                if (ctx.Index < 5979)
                {
                    result = result.ToString();
                }

                return result;
            });
            Assert.IsTrue(list2.Where(i => i is string)
                               .Count() == 5979);
        }

        #endregion Methods (1)
    }
}