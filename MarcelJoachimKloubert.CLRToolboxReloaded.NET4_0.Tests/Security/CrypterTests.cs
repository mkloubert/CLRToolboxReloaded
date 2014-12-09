// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Security.Cryptography;
using NUnit.Framework;
using System.Linq;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Security
{
    /// <summary>
    /// Tests for <see cref="AesCrypter" /> class.
    /// </summary>
    public sealed class CrypterTests : TestFixtureBase
    {
        #region Fields (1)

        private const string _WORD_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        #endregion Fields (1)

        #region Methods (7)

        [Test]
        public void AES_Test()
        {
            byte[] pwd;
            byte[] salt;
            var crypter = AesCrypter.Create(out pwd, out salt);

            this.TestCrypter(crypter);
        }

        [Test]
        public void Aggregate_Test()
        {
            byte[] pwd;
            byte[] salt;
            var crypter1 = AesCrypter.Create(out pwd, out salt);

            byte[] key;
            var crypter2 = XorCrypter.Create(out key);

            var crypter = AggregateCrypter.Create(crypter1, crypter2);

            this.TestCrypter(crypter);
        }

        private string CreateRandomText()
        {
            var result = new StringBuilder();

            var textSize = this._RANDOM.Next(0, 1024 * 1024);
            for (var i = 0; i < textSize; i++)
            {
                result.Append(_WORD_CHARS[this._RANDOM
                                              .Next(0, _WORD_CHARS.Length)]);
            }

            return result.ToString();
        }

        private void TestCrypter(ICrypter crypter)
        {
            for (var i = 0; i < 100; i++)
            {
                var str = this.CreateRandomText();

                var src = Encoding.UTF8.GetBytes(str);
                var dest = crypter.Encrypt(src);
                var src2 = crypter.Decrypt(dest);

                Assert.IsTrue(src.SequenceEqual(src2));
                Assert.IsFalse(src.SequenceEqual(dest));
                Assert.IsFalse(dest.SequenceEqual(src2));
            }
        }

        [Test]
        public void XOR_Test()
        {
            byte[] pwd;
            var crypter = XorCrypter.Create(out pwd);

            this.TestCrypter(crypter);
        }

        #endregion Methods (7)
    }
}