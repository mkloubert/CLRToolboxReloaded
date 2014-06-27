// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarcelJoachimKloubert.FileBox.Server.Helpers
{
    internal static class FileHelper
    {
        #region Methods (7)

        internal static void CreateEmptyFile(FileInfo file,
                                             FileAccess initialAccess = FileAccess.ReadWrite)
        {
            using (var stream = new FileStream(path: file.FullName,
                                               mode: FileMode.CreateNew,
                                               access: initialAccess))
            { }
        }

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

        internal static void CreateUniqueFilesForCryptedData(DirectoryInfo dir,
                                                             out FileInfo dataFile, out FileInfo metaFile, out FileInfo metaPwdFile,
                                                             bool createDataFile = true,
                                                             bool createMetaFile = true,
                                                             bool createMetaPwdFile = true)
        {
            dataFile = null;
            metaFile = null;
            metaPwdFile = null;

            ulong i = 0;
            do
            {
                dataFile = new FileInfo(Path.Combine(dir.FullName,
                                                     i + GlobalConstants.FileExtensions.DATA_FILE));
                metaFile = new FileInfo(Path.Combine(dir.FullName,
                                                     i + GlobalConstants.FileExtensions.META_FILE));
                metaPwdFile = new FileInfo(Path.Combine(dir.FullName,
                                                        i + GlobalConstants.FileExtensions.META_PASSWORD_FILE));

                if ((dataFile.Exists == false) &&
                    (metaFile.Exists == false) &&
                    (metaPwdFile.Exists == false))
                {
                    var filesToDelete = new List<FileInfo>();

                    try
                    {
                        if (createDataFile)
                        {
                            // create empty data file

                            filesToDelete.Add(dataFile);
                            CreateEmptyFile(dataFile);
                        }

                        if (createMetaFile)
                        {
                            // create empty meta file

                            filesToDelete.Add(metaFile);
                            CreateEmptyFile(metaFile);
                        }

                        if (createMetaPwdFile)
                        {
                            // create empty meta password file

                            filesToDelete.Add(metaPwdFile);
                            CreateEmptyFile(metaPwdFile);
                        }

                        break;
                    }
                    catch
                    {
                        // ignore
                    }
                    finally
                    {
                        // delete files before continue

                        filesToDelete.ForAll(
                            throwExceptions: false,
                            action: ctx =>
                            {
                                var path = ctx.Item.FullName;

                                if (File.Exists(path))
                                {
                                    File.Delete(path);
                                }
                            });
                    }
                }

                ++i;
            }
            while (true);
        }

        #endregion Methods (7)
    }
}