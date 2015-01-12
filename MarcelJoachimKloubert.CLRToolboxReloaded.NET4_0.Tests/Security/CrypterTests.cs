// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Security.Cryptography;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        #region Methods (6)

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

        private string CreateRandomText(int maxSize = 1024 * 1024)
        {
            var result = new StringBuilder();

            var textSize = this._RANDOM.Next(0, maxSize);
            for (var i = 0; i < textSize; i++)
            {
                result.Append(_WORD_CHARS[this._RANDOM
                                              .Next(0, _WORD_CHARS.Length)]);
            }

            return result.ToString();
        }

        [Test]
        public void Hasher_Test()
        {
            var crypterGenericType = typeof(HashCrypter<>);

            // find HashCrypter.Create<TAlgo>(string, Encoding)
            var createMethod = typeof(HashCrypter).GetMethods()
                                                  .Single(m =>
                                                  {
                                                      if (m.Name != "Create")
                                                      {
                                                          return false;
                                                      }

                                                      var genArgs = m.GetGenericArguments();
                                                      if (genArgs.Length != 1)
                                                      {
                                                          return false;
                                                      }

                                                      var @params = m.GetParameters();
                                                      return (@params.Length == 2) &&
                                                             (@params[0].ParameterType.Equals(typeof(string))) &&
                                                             (@params[1].ParameterType.Equals(typeof(Encoding)));
                                                  });

            var hashAlgorithms = new HashSet<Type>
            {
                typeof(MD5Cng),
                typeof(MD5CryptoServiceProvider),
                typeof(RIPEMD160Managed),
                typeof(SHA1Cng),
                typeof(SHA1CryptoServiceProvider),
                typeof(SHA1Managed),
                typeof(SHA256Cng),
                typeof(SHA256CryptoServiceProvider),
                typeof(SHA256Managed),
                typeof(SHA384Cng),
                typeof(SHA384CryptoServiceProvider),
                typeof(SHA384Managed),
                typeof(SHA512Cng),
                typeof(SHA512CryptoServiceProvider),
                typeof(SHA512Managed),
            };

            var src = this.CreateRandomText();

            var durations = new List<long>();

            foreach (var enc in this.GetEncodings())
            {
                var salt = this.CreateRandomText(128);

                foreach (var algo in hashAlgorithms)
                {
                    // call HashCrypter.Create<TAlgo>(string, Encoding)
                    var crypter = (ICrypter)createMethod.MakeGenericMethod(algo)
                                                        .Invoke(null,
                                                                new object[] { salt, enc });

                    var hashes = new List<byte[]>();
                    for (var i = 0; i < 2; i++)
                    {
                        hashes.Add(crypter.EncryptString(src, enc));
                    }

                    var firstHash = hashes.First();
                    Assert.IsTrue(hashes.Skip(1)
                                        .All(h => h.SequenceEqual(firstHash)));
                }
            }
        }

        private void TestCrypter(ICrypter crypter)
        {
            var src = this.CreateRandomText();

            foreach (var enc in this.GetEncodings())
            {
                var dest = crypter.EncryptString(src, enc);
                var src2 = crypter.DecryptString(dest, enc);

                Assert.AreEqual(src, src2);
            }
        }

        [Test]
        public void XOR_Test()
        {
            byte[] pwd;
            var crypter = XorCrypter.Create(out pwd);

            this.TestCrypter(crypter);
        }

        #endregion Methods (6)
    }
}