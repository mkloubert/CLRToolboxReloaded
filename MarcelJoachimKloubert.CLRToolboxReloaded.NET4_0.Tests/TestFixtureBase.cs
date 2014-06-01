// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using NUnit.Framework;
using System;

namespace MarcelJoachimKloubert.CLRToolbox._Tests
{
    /// <summary>
    /// A basic test fixture.
    /// </summary>
    [TestFixture]
    public abstract class TestFixtureBase
    {
        #region Fields (1)

        /// <summary>
        /// The global random generator.
        /// </summary>
        protected readonly Random _RANDOM = new Random();

        #endregion Fields (1)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFixtureBase" /> class.
        /// </summary>
        protected TestFixtureBase()
        {
        }

        #endregion Constructors (1)

        #region Methods (8)

        /// <summary>
        /// The logic for the <see cref="TestFixtureBase.SetupFixture" /> method.
        /// </summary>
        protected virtual void OnSetupFixture()
        {
            // dummy
        }

        /// <summary>
        /// The logic for the <see cref="TestFixtureBase.SetupTest" /> method.
        /// </summary>
        protected virtual void OnSetupTest()
        {
            // dummy
        }

        /// <summary>
        /// The logic for the <see cref="TestFixtureBase.TearDownFixture" /> method.
        /// </summary>
        protected virtual void OnTearDownFixture()
        {
            // dummy
        }

        /// <summary>
        /// The logic for the <see cref="TestFixtureBase.TearDownTest" /> method.
        /// </summary>
        protected virtual void OnTearDownTest()
        {
            // dummy
        }

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            this.OnSetupFixture();
        }

        [SetUp]
        public void SetupTest()
        {
            this.OnSetupTest();
        }

        [TestFixtureTearDown]
        public void TearDownFixture()
        {
            this.OnTearDownFixture();
        }

        [TearDown]
        public void TearDownTest()
        {
            this.OnTearDownTest();
        }

        #endregion Methods (8)
    }
}