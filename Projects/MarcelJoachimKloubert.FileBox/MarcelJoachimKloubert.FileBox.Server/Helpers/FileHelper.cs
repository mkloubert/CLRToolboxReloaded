// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.IO;

namespace MarcelJoachimKloubert.FileBox.Server.Helpers
{
    internal static class FileHelper
    {
        #region Methods (4)

        internal static FileInfo CreateTempFile(string tempDir)
        {
            return CreateTempFile(dir: new DirectoryInfo(tempDir));
        }

        internal static FileInfo CreateTempFile(string tempDir, Random rand)
        {
            return CreateTempFile(dir: new DirectoryInfo(tempDir),
                                  rand: rand);
        }

        internal static FileInfo CreateTempFile(DirectoryInfo dir)
        {
            return CreateTempFile(dir: dir,
                                  rand: new CryptoRandom());
        }

        internal static FileInfo CreateTempFile(DirectoryInfo dir, Random rand)
        {
            FileInfo result;
            do
            {
                var fBlob = new byte[4];
                rand.NextBytes(fBlob);

                result = new FileInfo(Path.Combine(dir.FullName,
                                                   fBlob.AsHexString() + GlobalConstants.FileExtensions.TEMP_FILE));
            }
            while (result.Exists);

            File.WriteAllBytes(result.FullName, new byte[0]);
            return result;
        }

        #endregion Methods (4)
    }
}