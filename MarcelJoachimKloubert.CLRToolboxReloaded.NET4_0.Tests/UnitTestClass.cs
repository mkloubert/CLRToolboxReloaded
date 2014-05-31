using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.CLRToolbox._Tests
{
    [TestFixture]
    public class UnitTestClass
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine("Setup");
        }

        [Test]
        public void Test1()
        {
            Console.WriteLine("Test1");
        }

        [Test]
        public void Test2()
        {
            Console.WriteLine("Test2");
        }

        [TearDown]
        public void Teardown()
        {
            Console.WriteLine("Teardown");
        }
    }
}
