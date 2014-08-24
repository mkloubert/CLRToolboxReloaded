using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MarcelJoachimKloubert.ClrToolboxReloaded.Examples.GZip
{
    internal static class Program
    {
        #region Methods (1)

        private static void Main(string[] args)
        {
            var rand = new CryptoRandom();

            // create random content to zip
            var content = new StringBuilder();
            byte[] blob;
            {
                // create random content to zip
                for (ushort i = 'a'; i <= 'z'; i++)
                {
                    var count = rand.Next(0, 1024);

                    for (var ii = 0; ii < count; ii++)
                    {
                        content.Append((char)i);
                    }
                }

                blob = Encoding.UTF8.GetBytes(content.ToString());
            }

            // what is the size of the created content?
            Console.WriteLine("Source size: {0}", blob.Length);

            Console.WriteLine();

            // compress and uncompress content
            var compressedBlob = blob.GZip();
            var uncompressedBlob = compressedBlob.GUnzip();

            // output sizes
            Console.WriteLine("Compressed size  : {0}", compressedBlob.Length);
            Console.WriteLine("Uncompressed size: {0}", uncompressedBlob.Length);

            Console.WriteLine();

            // check if source and uncompressed content are the same.
            using (var md5_1 = new MD5CryptoServiceProvider())
            {
                using (var md5_2 = new MD5CryptoServiceProvider())
                {
                    var hash1 = md5_1.ComputeHash(blob);
                    var hash2 = md5_2.ComputeHash(uncompressedBlob);

                    Console.WriteLine("Source and umcompressed are same: {0}", hash1.SequenceEqual(hash2) ? "yes" : "no");
                }
            }

            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("===== ENTER =====");

            Console.ReadLine();
        }

        #endregion Methods (1)
    }
}