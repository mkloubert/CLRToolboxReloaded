// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

        #region Methods (9)

        /// <summary>
        /// Returns all (default) encodings that should be used by that test fixture.
        /// </summary>
        /// <returns>The list of encodings.</returns>
        protected virtual IEnumerable<Encoding> GetEncodings()
        {
            return Encoding.GetEncodings()
                           .Select(ei => ei.GetEncoding());
        }

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

        /// <summary>
        /// Logic for <see cref="TestFixtureSetUpAttribute" />.
        /// </summary>
        [TestFixtureSetUp]
        public void SetupFixture()
        {
            this.OnSetupFixture();
        }

        /// <summary>
        /// Logic for <see cref="SetUpAttribute" />.
        /// </summary>
        [SetUp]
        public void SetupTest()
        {
            this.OnSetupTest();
        }

        /// <summary>
        /// Logic for <see cref="TestFixtureTearDownAttribute" />.
        /// </summary>
        [TestFixtureTearDown]
        public void TearDownFixture()
        {
            this.OnTearDownFixture();
        }

        /// <summary>
        /// Logic for <see cref="TearDownAttribute" />.
        /// </summary>
        [TearDown]
        public void TearDownTest()
        {
            this.OnTearDownTest();
        }

        #endregion Methods (8)
    }
}