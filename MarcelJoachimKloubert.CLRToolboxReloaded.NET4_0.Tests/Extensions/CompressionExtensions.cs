// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Extensions
{
    /// <summary>
    /// Test compression extensions like GZip and GUnzip
    /// </summary>
    public class CompressionExtensions : TestFixtureBase
    {
        #region Fields (2)

        private const string _WORD_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly List<string> _WORDS = new List<string>();

        #endregion Fields (2)

        #region Methods (4)

        [Test]
        public void GZip_ByteArray()
        {
            var uncompressedData = Encoding.UTF8.GetBytes(string.Join(" ", this._WORDS));
            var compressedData = uncompressedData.GZip();
            var uncompressedData2 = compressedData.GUnzip();

            Assert.IsTrue(uncompressedData.SequenceEqual(uncompressedData2));
            Assert.IsFalse(uncompressedData.SequenceEqual(compressedData));
            Assert.IsFalse(uncompressedData2.SequenceEqual(compressedData));
        }

        [Test]
        public void GZip_Stream()
        {
            using (var uncompressedData = new MemoryStream(Encoding.UTF8.GetBytes(string.Join(" ", this._WORDS))))
            {
                using (var compressedData = new MemoryStream())
                {
                    uncompressedData.GZip(compressedData);

                    compressedData.Position = 0;
                    using (var uncompressedData2 = new MemoryStream())
                    {
                        compressedData.GUnzip(uncompressedData2);

                        Assert.IsTrue(uncompressedData.ToArray()
                                                      .SequenceEqual(uncompressedData2.ToArray()));
                        Assert.IsFalse(uncompressedData.ToArray()
                                                       .SequenceEqual(compressedData.ToArray()));
                        Assert.IsFalse(uncompressedData2.ToArray()
                                                        .SequenceEqual(compressedData.ToArray()));
                    }
                }
            }
        }

        protected override void OnTearDownTest()
        {
            this._WORDS.Clear();
        }

        protected override void OnSetupTest()
        {
            this._WORDS.Clear();

            for (var i = 0; i < 100; i++)
            {
                foreach (var c in _WORD_CHARS)
                {
                    var wordSize = this._RANDOM.Next(0, 1024);

                    var newWord = new StringBuilder();
                    for (var ii = 0; ii <= wordSize; ii++)
                    {
                        newWord.Append(c);
                    }

                    this._WORDS.Add(newWord.ToString());
                }
            }

            this._WORDS.Shuffle(this._RANDOM);
        }

        #endregion Methods (3)
    }
}