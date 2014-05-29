// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox._Tests
{
    [TestFixture]
    public sealed class Strings
    {
        #region Methods (7)

        [Test]
        public void CharSequence_Conversion_CharArray()
        {
            string str1 = null;
            string str2 = "MK+TM";
            string str3 = string.Empty;

            CharSequence chars1 = str1;
            CharSequence chars2 = str2;
            CharSequence chars3 = string.Empty;
            CharSequence chars4 = string.Empty;
            CharSequence chars5 = null;

            // convert TO
            char[] ca1 = chars1;
            char[] ca2 = chars2;
            char[] ca3 = chars3;
            char[] ca4 = chars4;
            char[] ca5 = chars5;

            // convert BACK
            CharSequence chars1Back = ca1;
            CharSequence chars2Back = ca2;
            CharSequence chars3Back = ca3;
            CharSequence chars4Back = ca4;
            CharSequence chars5Back = ca5;

            // check if conversion and back conversion
            // are correct
            Assert.AreEqual(chars1, chars1Back);
            Assert.AreEqual(chars2, chars2Back);
            Assert.AreEqual(chars3, chars3Back);
            Assert.AreEqual(chars4, chars4Back);
            Assert.AreEqual(chars5, chars5Back);

            // check for (null): chars*
            Assert.IsNull(chars1);
            Assert.IsNotNull(chars2);
            Assert.IsNotNull(chars3);
            Assert.IsNotNull(chars4);
            Assert.IsNull(chars5);

            // check for (null): s*
            Assert.IsNull(ca1);
            Assert.IsNotNull(ca2);
            Assert.IsNotNull(ca3);
            Assert.IsNotNull(ca4);
            Assert.IsNull(ca5);
        }

        [Test]
        public void CharSequence_Conversion_String()
        {
            string str1 = null;
            string str2 = "MK+TM";
            string str3 = string.Empty;

            CharSequence chars1 = str1;
            CharSequence chars2 = str2;
            CharSequence chars3 = string.Empty;
            CharSequence chars4 = string.Empty;
            CharSequence chars5 = null;

            // convert TO
            string s1 = chars1;
            string s2 = chars2;
            string s3 = chars3;
            string s4 = chars4;
            string s5 = chars5;

            // convert BACK
            CharSequence chars1Back = s1;
            CharSequence chars2Back = s2;
            CharSequence chars3Back = s3;
            CharSequence chars4Back = s4;
            CharSequence chars5Back = s5;

            // check if conversion and back conversion
            // are correct
            Assert.AreEqual(chars1, chars1Back);
            Assert.AreEqual(chars2, chars2Back);
            Assert.AreEqual(chars3, chars3Back);
            Assert.AreEqual(chars4, chars4Back);
            Assert.AreEqual(chars5, chars5Back);

            // check for (null): chars*
            Assert.IsNull(chars1);
            Assert.IsNotNull(chars2);
            Assert.IsNotNull(chars3);
            Assert.IsNotNull(chars4);
            Assert.IsNull(chars5);

            // check for (null): s*
            Assert.IsNull(s1);
            Assert.IsNotNull(s2);
            Assert.IsNotNull(s3);
            Assert.IsNotNull(s4);
            Assert.IsNull(s5);
        }

        [Test]
        public void CharSequence_Conversion_StringBuilder()
        {
            string str1 = null;
            string str2 = "MK+TM";
            string str3 = string.Empty;

            CharSequence chars1 = str1;
            CharSequence chars2 = str2;
            CharSequence chars3 = string.Empty;
            CharSequence chars4 = string.Empty;
            CharSequence chars5 = null;

            // convert TO
            StringBuilder sb1 = chars1;
            StringBuilder sb2 = chars2;
            StringBuilder sb3 = chars3;
            StringBuilder sb4 = chars4;
            StringBuilder sb5 = chars5;

            // convert BACK
            CharSequence chars1Back = sb1;
            CharSequence chars2Back = sb2;
            CharSequence chars3Back = sb3;
            CharSequence chars4Back = sb4;
            CharSequence chars5Back = sb5;

            // check if conversion and back conversion
            // are correct
            Assert.AreEqual(chars1, chars1Back);
            Assert.AreEqual(chars2, chars2Back);
            Assert.AreEqual(chars3, chars3Back);
            Assert.AreEqual(chars4, chars4Back);
            Assert.AreEqual(chars5, chars5Back);
        }

        [Test]
        public void CharSequence_Equals_CharArray()
        {
            string str1 = null;
            string str2 = "MK+TM";
            string str3 = string.Empty;

            CharSequence chars1 = str1;
            CharSequence chars2 = str2;
            CharSequence chars3 = string.Empty;
            CharSequence chars4 = string.Empty;
            CharSequence chars5 = null;

            char[] ca1 = chars1;
            char[] ca2 = chars2;
            char[] ca3 = chars3;
            char[] ca4 = chars4;
            char[] ca5 = chars5;

            Assert.AreEqual(chars1, ca1);
            Assert.AreEqual(chars2, ca2);
            Assert.AreEqual(chars3, ca3);
            Assert.AreEqual(chars4, ca4);
            Assert.AreEqual(chars5, ca5);
        }

        [Test]
        public void CharSequence_Equals_String()
        {
            string str1 = null;
            string str2 = "MK+TM";
            string str3 = string.Empty;

            CharSequence chars1 = str1;
            CharSequence chars2 = str2;
            CharSequence chars3 = string.Empty;
            CharSequence chars4 = string.Empty;
            CharSequence chars5 = null;

            string s1 = chars1;
            string s2 = chars2;
            string s3 = chars3;
            string s4 = chars4;
            string s5 = chars5;

            Assert.AreEqual(chars1, s1);
            Assert.AreEqual(chars2, s2);
            Assert.AreEqual(chars3, s3);
            Assert.AreEqual(chars4, s4);
            Assert.AreEqual(chars5, s5);
        }

        [Test]
        public void CharSequence_Equals_StringBuilder()
        {
            string str1 = null;
            string str2 = "MK+TM";
            string str3 = string.Empty;

            CharSequence chars1 = str1;
            CharSequence chars2 = str2;
            CharSequence chars3 = string.Empty;
            CharSequence chars4 = string.Empty;
            CharSequence chars5 = null;

            StringBuilder sb1 = chars1;
            StringBuilder sb2 = chars2;
            StringBuilder sb3 = chars3;
            StringBuilder sb4 = chars4;
            StringBuilder sb5 = chars5;

            Assert.AreEqual(chars1, sb1);
            Assert.AreEqual(chars2, sb2);
            Assert.AreEqual(chars3, sb3);
            Assert.AreEqual(chars4, sb4);
            Assert.AreEqual(chars5, sb5);
        }

        
        [Test]
        public void CharSquence_Compare_String()
        {
            var strList = new string[] { "1", null, "TM", "MK" };
            var strListSorted = strList.OrderBy(x => x);

            var charList1 = new List<CharSequence> { "1", null, "MK", "TM" };
            var charList1Sorted = charList1.OrderBy(x => x);

            var charList2 = new CharSequence[] { "1", string.Empty, "MK", "TM" };
            var charList2Sorted = charList2.OrderBy(x => x);

            var charList3 = new List<CharSequence> { "1", null, "MK" };
            var charList3Sorted = charList3.OrderBy(x => x);

            // strListSorted == charList1Sorted
            Assert.IsTrue(charList1Sorted.OrderBy(x => x)
                                         .Select(x => (string)x)
                                         .SequenceEqual(strListSorted));

            // strListSorted != charList2Sorted
            Assert.IsFalse(charList2Sorted.OrderBy(x => x)
                                          .Select(x => (string)x)
                                          .SequenceEqual(strListSorted));

            // strListSorted != charList3Sorted
            Assert.IsFalse(charList3Sorted.OrderBy(x => x)
                                          .Select(x => (string)x)
                                          .SequenceEqual(strListSorted));
        }


        [Test]
        public void CharSequenceTest()
        {
            string str1 = null;
            string str2 = "MK+TM";
            string str3 = string.Empty;

            CharSequence chars1 = str1;
            CharSequence chars2 = str2;
            CharSequence chars3 = string.Empty;
            CharSequence chars4 = string.Empty;
            CharSequence chars5 = null;

            // check if original char sequences
            // have the save data as back converted strings
            // (explicit operator)
            Assert.AreEqual(chars1, (string)chars1);
            Assert.AreEqual(chars2, (string)chars2);
            Assert.AreEqual(chars3, (string)chars3);
            Assert.AreEqual(chars4, (string)chars4);
            Assert.AreEqual(chars5, (string)chars5);

            // check equal operators (CharSequence <==> CharSequence)
            {
                // chars1 (==)
                Assert.IsFalse(chars1 == chars2);
                Assert.IsFalse(chars2 == chars1);
                Assert.IsFalse(chars1 == chars3);
                Assert.IsFalse(chars3 == chars1);
                Assert.IsFalse(chars1 == chars4);
                Assert.IsFalse(chars4 == chars1);
                Assert.IsTrue(chars1 == chars5);
                Assert.IsTrue(chars5 == chars1);
                
                // chars1 (!=)
                Assert.IsTrue(chars1 != chars2);
                Assert.IsTrue(chars2 != chars1);
                Assert.IsTrue(chars1 != chars3);
                Assert.IsTrue(chars3 != chars1);
                Assert.IsTrue(chars1 != chars4);
                Assert.IsTrue(chars4 != chars1);
                Assert.IsFalse(chars1 != chars5);
                Assert.IsFalse(chars5 != chars1);

                // chars2 (==)
                Assert.IsFalse(chars2 == chars3);
                Assert.IsFalse(chars3 == chars2);
                Assert.IsFalse(chars2 == chars4);
                Assert.IsFalse(chars4 == chars2);
                Assert.IsFalse(chars2 == chars5);
                Assert.IsFalse(chars5 == chars2);
                
                // chars2 (!=)
                Assert.IsTrue(chars2 != chars3);
                Assert.IsTrue(chars3 != chars2);
                Assert.IsTrue(chars2 != chars4);
                Assert.IsTrue(chars4 != chars2);
                Assert.IsTrue(chars2 != chars5);
                Assert.IsTrue(chars5 != chars2);

                // chars3 (==)
                Assert.IsTrue(chars3 == chars4);
                Assert.IsTrue(chars4 == chars3);
                Assert.IsFalse(chars3 == chars5);
                Assert.IsFalse(chars5 == chars3);
                
                // chars3 (!=)
                Assert.IsFalse(chars3 != chars4);
                Assert.IsFalse(chars4 != chars3);
                Assert.IsTrue(chars3 != chars5);
                Assert.IsTrue(chars5 != chars3);

                // chars4 (==)
                Assert.IsFalse(chars4 == chars5);
                Assert.IsFalse(chars5 == chars4);

                // chars4 (!=)
                Assert.IsTrue(chars4 != chars5);
                Assert.IsTrue(chars5 != chars4);
            }

            // check equal operators (CharSequence <==> string)
            {
                // chars1 (==)
                Assert.IsTrue(chars1 == str1);
                Assert.IsTrue(str1 == chars1);
                Assert.IsFalse(chars1 == str2);
                Assert.IsFalse(str2 == chars1);
                Assert.IsFalse(chars1 == str3);
                Assert.IsFalse(str3 == chars1);
                
                // chars1 (!=)
                Assert.IsFalse(chars1 != str1);
                Assert.IsFalse(str1 != chars1);
                Assert.IsTrue(chars1 != str2);
                Assert.IsTrue(str2 != chars1);
                Assert.IsTrue(chars1 != str3);
                Assert.IsTrue(str3 != chars1);

                // chars2 (==)
                Assert.IsFalse(chars2 == str1);
                Assert.IsFalse(str1 == chars2);
                Assert.IsTrue(chars2 == str2);
                Assert.IsTrue(str2 == chars2);
                Assert.IsFalse(chars2 == str3);
                Assert.IsFalse(str3 == chars2);
                
                // chars2 (!=)
                Assert.IsTrue(chars2 != str1);
                Assert.IsTrue(str1 != chars2);
                Assert.IsFalse(chars2 != str2);
                Assert.IsFalse(str2 != chars2);
                Assert.IsTrue(chars2 != str3);
                Assert.IsTrue(str3 != chars2);

                // chars3 (==)
                Assert.IsFalse(chars3 == str1);
                Assert.IsFalse(str1 == chars3);
                Assert.IsFalse(chars3 == str2);
                Assert.IsFalse(str2 == chars3);
                Assert.IsTrue(chars3 == str3);
                Assert.IsTrue(str3 == chars3);

                // chars3 (!=)
                Assert.IsTrue(chars3 != str1);
                Assert.IsTrue(str1 != chars3);
                Assert.IsTrue(chars3 != str2);
                Assert.IsTrue(str2 != chars3);
                Assert.IsFalse(chars3 != str3);
                Assert.IsFalse(str3 != chars3);
            }

            // check instances
            {
                Assert.AreSame((CharSequence)str1, chars1);
                Assert.AreNotSame((CharSequence)str2, chars2);
                Assert.AreNotSame((CharSequence)str3, chars3);
            }

            // check sequence feature (foreach)
            {
                // chars2
                {
                    string test = null;
                    foreach (var c in chars2)
                    {
                        test += c;
                    }

                    Assert.IsNotNull(test);  // at least one item was handled in foreach()
                    Assert.AreEqual(test, chars2);
                    Assert.AreEqual(test, str2);
                }

                // chars3
                {
                    string test = null;
                    foreach (var c in chars3)
                    {
                        test += c;
                    }

                    Assert.IsNull(test);  // no item was handled in foreach()
                    Assert.AreNotEqual(test, chars3);
                    Assert.AreNotEqual(test, str3);
                }
            }
        }

        #endregion Methods (7)
    }
}