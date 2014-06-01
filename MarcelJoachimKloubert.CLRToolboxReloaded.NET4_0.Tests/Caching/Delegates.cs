﻿// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Caching;
using NUnit.Framework;
using System;
using System.Threading;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Caching
{
    [TestFixture]
    public class Delegates
    {
        #region Fields (1)

        private DelegateCache _cache;
        private int _tests;

        #endregion Fields (1)

        #region Methods (2)

        [TestFixtureSetUp]
        public void Setup_Global_Fixture()
        {
            this._cache = new DelegateCache();
        }

        [TestFixtureTearDown]
        public void Setup_Global_TearDown()
        {
            this._cache.Dispose();
        }

        [SetUp]
        public void Setup_Test_Fixture()
        {
            this._cache.Clear();
        }

        [TearDown]
        public void Setup_Test_TearDown()
        {
            ++this._tests;
        }

        [Test]
        // [Ignore]
        public void DelgateCache_Action()
        {
            var cache = this._cache;

            var val = -1;
            DelegateCache.CachedAction action = () => ++val;

            cache.SaveAction(action, TimeSpan.FromSeconds(3));

            for (var i = 0; i < 4; i++)
            {
                cache.InvokeAction(action);

                if (i < 3)
                {
                    Assert.AreEqual(val, 0);
                }
                else
                {
                    Assert.AreEqual(val, 1);
                }

                Thread.Sleep(1100);
            }
        }

        [Test]
        // [Ignore]
        public void DelgateCache_Func()
        {
            var cache = this._cache;

            var val = 0;
            DelegateCache.CachedFunc<int> func = () => val++;

            cache.SaveFunc(func, TimeSpan.FromSeconds(5));

            for (var i = 0; i < 6; i++)
            {
                var v = cache.InvokeFunc(func);

                if (i < 5)
                {
                    Assert.AreEqual(v, 0);
                }
                else
                {
                    Assert.AreEqual(v, 1);
                }

                Thread.Sleep(1100);
            }
        }

        #endregion Methods (2)
    }
}