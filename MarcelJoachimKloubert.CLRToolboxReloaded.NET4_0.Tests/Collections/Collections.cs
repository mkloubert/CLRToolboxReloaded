// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Collections
{
    [TestFixture]
    public class Collections
    {
        #region Methods (3)

        [Test]
        public void AddRange()
        {
            IList<int> coll1 = new Collection<int>();
            coll1.AddRange(Enumerable.Range(0, 5979));

            ICollection<int> coll2 = new List<int>();
            coll2.AddRange(Enumerable.Range(0, 23979));

            Assert.IsTrue(coll1.SequenceEqual(Enumerable.Range(0, 5979)));
            Assert.IsTrue(coll2.SequenceEqual(Enumerable.Range(0, 23979)));
        }

        [Test]
        public void NullIndexDictionary_NullIndex()
        {
            var dict1 = new NullIndexDictionary<long>();
            dict1[null] = 5979;
            dict1[null] = 5980;
            dict1[null] = 5981;

            Assert.IsTrue(dict1.Keys
                               .SequenceEqual(Enumerable.Range(0, 3)));
            Assert.IsTrue(dict1.Values
                               .SequenceEqual(Enumerable.Range(5979, 3)
                                                        .Select(v => (long)v)));
        }

        [Test]
        public void NullIndexDictionary_AddRange()
        {
            var dict1 = new NullIndexDictionary<long>();
            dict1.AddRange(new long[] { 23979, 23980, 23981 });

            Assert.IsTrue(dict1.Keys
                               .SequenceEqual(Enumerable.Range(0, 3)));
            Assert.IsTrue(dict1.Values
                               .SequenceEqual(Enumerable.Range(23979, 3)
                                                        .Select(v => (long)v)));
        }

        [Test]
        public void NullIndexDictionary_AddRangeExtension()
        {
            var dict1 = new NullIndexDictionary<long>();

            IList<long> dict2 = dict1;
            dict2.AddRange(new long[] { 23979, 23980, 23981 });

            Assert.IsTrue(dict1.Keys
                               .SequenceEqual(Enumerable.Range(0, 3)));
            Assert.IsTrue(dict1.Values
                               .SequenceEqual(Enumerable.Range(23979, 3)
                                                        .Select(v => (long)v)));
        }

        #endregion Methods (3)
    }
}