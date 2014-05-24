// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        #endregion Methods (6)
    }
}