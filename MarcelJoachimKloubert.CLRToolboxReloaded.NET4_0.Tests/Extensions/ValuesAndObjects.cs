// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using NUnit.Framework;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Extensions
{
    [TestFixture]
    public class ValuesAndObjects
    {
        #region Methods (6)
        
        [Test]
        public void IsNotFalse()
        {
            bool? a = true;
            bool? b = false;
            bool? c = null;

            Assert.IsTrue(a.IsNotFalse());
            Assert.IsFalse(b.IsNotFalse());
            Assert.IsTrue(c.IsNotFalse());
        }

        [Test]
        public void IsNotTrue()
        {
            bool? a = true;
            bool? b = false;
            bool? c = null;

            Assert.IsFalse(a.IsNotTrue());
            Assert.IsTrue(b.IsNotTrue());
            Assert.IsTrue(c.IsNotTrue());
        }

        [Test]
        public void IsNotNull()
        {
            object a = null;
            object b = 0;
            object c = DBNull.Value;
            bool? d = null;
            DateTime? e = new DateTime(1979, 9, 5, 23, 9, 19, 79);
            int? f = 5979;

            Assert.IsFalse(a.IsNotNull());
            Assert.IsTrue(b.IsNotNull());
            Assert.IsFalse(c.IsNotNull());
            Assert.IsFalse(d.IsNotNull());
            Assert.IsTrue(e.IsNotNull());
            Assert.IsTrue(f.IsNotNull());
        }

        [Test]
        public void IsNull()
        {
            object a = null;
            object b = 0;
            object c = DBNull.Value;
            bool? d = null;
            DateTime? e = new DateTime(1979, 9, 5, 23, 9, 19, 79);
            int? f = 5979;

            Assert.IsTrue(a.IsNull());
            Assert.IsFalse(b.IsNull());
            Assert.IsTrue(c.IsNull());
            Assert.IsTrue(d.IsNull());
            Assert.IsFalse(e.IsNull());
            Assert.IsFalse(f.IsNull());
        }

        [Test]
        public void IsFalse()
        {
            bool? a = true;
            bool? b = false;
            bool? c = null;

            Assert.IsFalse(a.IsFalse());
            Assert.IsTrue(b.IsFalse());
            Assert.IsFalse(c.IsFalse());
        }

        [Test]
        public void IsTrue()
        {
            bool? a = true;
            bool? b = false;
            bool? c = null;

            Assert.IsTrue(a.IsTrue());
            Assert.IsFalse(b.IsTrue());
            Assert.IsFalse(c.IsTrue());
        }

        #endregion Methods (1)
    }
}