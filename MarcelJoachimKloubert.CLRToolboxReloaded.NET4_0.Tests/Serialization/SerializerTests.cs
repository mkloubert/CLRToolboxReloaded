// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Serialization;
using MarcelJoachimKloubert.CLRToolbox.Serialization.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Serialization
{
    /// <summary>
    /// Tests the serializers.
    /// </summary>
    public sealed class SerializerTests : TestFixtureBase
    {
        [Serializable]
        private sealed class TestClass : MarshalByRefObject, IEquatable<TestClass>
        {
            public int A;
            public DateTime B;
            
            public bool Equals(TestClass other)
            {
                if (other == null)
                {
                    return false;
                }

                return (this.A == other.A) &&
                       (this.B == other.B);
            }

            public override bool Equals(object other)
            {
                if (other is TestClass)
                {
                    return this.Equals((TestClass)other);
                }

                return base.Equals(other);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        #region Methods (2)
        
        [Test]
        public void BinaryFormatterSerializer_Test()
        {
            var serializer = new BinaryFormatterSerializer();

            var obj1a = new TestClass()
            {
                A = 23979,
                B = new DateTime(1979, 9, 5),
            };
            var bin1 = serializer.Serialize(obj1a);
            var obj1b = serializer.Deserialize<TestClass>(bin1);

            Assert.AreEqual(obj1a, obj1b);
        }

        [Test]
        public void JsonNetSerializer_Test()
        {
            var serializer = JsonNetSerializer.Create();

            var obj1a = new TestClass()
            {
                A = 5979,
                B = new DateTime(1979, 9, 23),
            };
            var json1 = serializer.Serialize(obj1a);
            var obj1b = serializer.Deserialize<TestClass>(json1);

            Assert.AreEqual(obj1a, obj1b);
        }

        #endregion
    }
}
