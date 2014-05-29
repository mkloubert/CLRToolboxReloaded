﻿// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using NUnit.Framework;
using System;

namespace MarcelJoachimKloubert.CLRToolbox._Tests
{
    [TestFixture]
    public class RandomValues
    {
        #region Methods (2)

        [Test]
        // [Ignore]
        public void CryptoRandom_NoSeed()
        {
            for (var i = 0; i < 7; i++)
            {
                var r = new CryptoRandom();

                for (var ii = 0; ii < 1000000; ii++)
                {
                    // 0 <= v1 < 1000
                    var v1 = r.Next(0, 1000);
                    Assert.IsTrue(v1 >= 0 && v1 < 1000);

                    // -1000 <= v2 < 100000
                    var v2 = r.Next(-1000, 100000);
                    Assert.IsTrue(v2 >= -1000 && v1 < 100000);

                    // v3 == 0
                    var v3 = r.Next(0, 0);
                    Assert.IsFalse(v3 != 0);

                    // -1 <= v4 < 1
                    var v4 = r.Next(-1, 1);
                    Assert.IsTrue(v4 >= -1 && v4 < 1);
                }
            }
        }

        [Test]
        // [Ignore]
        public void CryptoRandom_WithSeed()
        {
            for (var i = 0; i < 13; i++)
            {
                var r = CryptoRandom.Create();

                for (var ii = 0; ii < 1000000; ii++)
                {
                    // 0 <= v1 < 10000
                    var v1 = r.Next(0, 10000);
                    Assert.IsTrue(v1 >= 0 && v1 < 10000);

                    // -2100 <= v2 < 107000
                    var v2 = r.Next(-2100, 107000);
                    Assert.IsTrue(v2 >= -2100 && v1 < 107000);

                    // v3 == 1
                    var v3 = r.Next(1, 1);
                    Assert.IsFalse(v3 != 1);

                    // -1 <= v4 < 1
                    var v4 = r.Next(-1, 1);
                    Assert.IsTrue(v4 >= -1 && v4 < 1);
                }
            }
        }

        #endregion Methods (2)
    }
}