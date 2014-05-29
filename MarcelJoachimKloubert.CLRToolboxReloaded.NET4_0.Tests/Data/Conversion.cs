// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using NUnit.Framework;
using System;
using System.Globalization;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Data
{
    [TestFixture]
    public class Conversion
    {
        #region CLASS: TestClass

        private sealed class TestClass
        {
            #region Properties (1)

            public string Test { get; set; }

            #endregion Properties (1)

            #region Methods (2)

            [ConvertTo(typeof(long))]
            public long ToInt64(IConvertToArgs args)
            {
                return long.Parse(this.Test);
            }

            public override string ToString()
            {
                return this.Test;
            }

            #endregion Methods (2)
        }

        #endregion CLASS: TestClass

        #region Methods (1)

        [Test]
        public void CommonConverterTest()
        {
            var r = new Random();
            var gerCulture = CultureInfo.GetCultureInfo("de-DE");
            var converter = new CommonConverter();
            const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

            var input1 = "MK+TM";
            var input2 = r.Next();
            var input3 = new TestClass() { Test = r.Next().ToString() };
            var input4 = r.Next().ToString();
            var input5 = DateTime.Now.ToString(DATE_FORMAT);
            var input6 = r.NextDouble().ToString(gerCulture);
            var input7 = r.Next();
            var input8 = r.NextDouble();

            var output1 = converter.ChangeType<string>(input1.ToCharArray());
            var output2 = converter.ChangeType<string>(input2);
            var output3 = converter.ChangeType<string>(input3);
            var output4 = converter.ChangeType<int>(input4);
            var output5 = converter.ChangeType<DateTime>(input5);
            var output6 = converter.ChangeType<double>(input6, gerCulture);
            var output7 = converter.ChangeType<long>(input7);
            var output8 = converter.ChangeType<decimal>(input8);
            var output9 = converter.ChangeType<long>(input3);

            Assert.AreEqual(input1, output1);
            Assert.AreEqual(input2.ToString(), output2);
            Assert.AreEqual(input3.Test, output3);
            Assert.AreEqual(input4, output4.ToString());
            Assert.AreEqual(input5, output5.ToString(DATE_FORMAT));
            Assert.AreEqual(input6, output6.ToString(gerCulture));
            Assert.AreEqual((long)input7, output7);
            Assert.AreEqual((decimal)input8, output8);
            Assert.AreEqual(input3.Test, output9.ToString());
        }

        #endregion Methods (1)
    }
}