// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using MarcelJoachimKloubert.FileBox.Server.Helpers;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox.Server.Execution.Jobs
{
    internal sealed class SendJob : SendJobBase
    {
        #region Constructors (1)

        internal SendJob(FileBoxHost host,
                         object sync,
                         string tempFile,
                         byte[] pwd, byte[] salt,
                         IServerPrincipal sender, string recipient,
                         XElement meta)
            : base(id: new Guid("{11AC2E44-0122-4BBB-A883-CF6BD8678C7D}"),
                   host: host,
                   sync: sync,
                   tempFile: tempFile,
                   pwd: pwd, salt: salt,
                   sender: sender, recipient: recipient,
                   meta: meta)
        {
        }

        #endregion Constructors (1)

        #region Methods (2)

        protected override void OnError(IJobExecutionContext ctx, Exception ex)
        {
            this.TryDeleteFile(this._tempFile);
        }

        protected override void OnExecute(IJobExecutionContext ctx)
        {
            var rand = new CryptoRandom();

            IServerPrincipal recipient = ServerPrincipal.FromUsername(this._host,
                                                                      this._recipient);

            var targetDir = new DirectoryInfo(recipient.Inbox);
            if (targetDir.Exists)
            {
                var rsa = recipient.TryGetRsaCrypter();
                if (rsa != null)
                {
                    // find filenames that are NOT in use yet
                    FileInfo targetDataFile;
                    FileInfo targetMetaFile;
                    FileInfo targetMetaPwdFile;
                    FindUniqueDataAndMetaFileNames(targetDir,
                                                   dataFile: out targetDataFile,
                                                   metaFile: out targetMetaFile,
                                                   metaPwdFile: out targetMetaPwdFile);

                    using (var tempStream = new FileStream(this._tempFile.FullName,
                                                           FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            // meta password
                            Rijndael metaCrypter;
                            using (var metaPwdStream = new FileStream(targetMetaPwdFile.FullName,
                                                                      FileMode.CreateNew, FileAccess.ReadWrite))
                            {
                                var metaPwdAndSalt = new byte[120];
                                rand.NextBytes(metaPwdAndSalt);

                                metaCrypter = CryptoHelper.CreateRijndael(pwd: metaPwdAndSalt.Skip(7).Take(48).ToArray(),
                                                                          salt: metaPwdAndSalt.Skip(7 + 48).Take(16).ToArray());

                                // save crypted meta data
                                {
                                    var cryptedMetaBlob = rsa.Encrypt(metaPwdAndSalt, false);

                                    metaPwdStream.Write(cryptedMetaBlob, 0, cryptedMetaBlob.Length);
                                }
                            }

                            using (var metaStream = new FileStream(targetMetaFile.FullName,
                                                                   FileMode.CreateNew, FileAccess.ReadWrite))
                            {
                                // generate password for target file
                                var filePwd = new byte[48];
                                rand.NextBytes(filePwd);

                                // generate salt for target file
                                var fileSalt = new byte[16];
                                rand.NextBytes(fileSalt);

                                // save salt and password to meta XML
                                var meta = new XElement(this._meta);
                                meta.Add(new XElement("password",
                                                      Convert.ToBase64String(filePwd)));
                                meta.Add(new XElement("salt",
                                                      Convert.ToBase64String(fileSalt)));

                                // save crypted meta data
                                using (var metaDataStream = new MemoryStream(buffer: CreateXmlEncoder().GetBytes(meta.ToString()),
                                                                             writable: false))
                                {
                                    var cryptedMetaStream = new CryptoStream(metaStream,
                                                                             metaCrypter.CreateEncryptor(),
                                                                             CryptoStreamMode.Write);

                                    metaDataStream.CopyTo(cryptedMetaStream);

                                    cryptedMetaStream.Flush();
                                    cryptedMetaStream.Close();
                                }

                                using (var dataStream = new FileStream(targetDataFile.FullName,
                                                                       FileMode.CreateNew, FileAccess.ReadWrite))
                                {
                                    var cryptedDataStream = new CryptoStream(dataStream,
                                                                             CryptoHelper.CreateRijndael(pwd: filePwd,
                                                                                                         salt: fileSalt).CreateEncryptor(),
                                                                             CryptoStreamMode.Write);
                                    {
                                        // copy from temp file to target file
                                        var cryptoTempStream = new CryptoStream(tempStream,
                                                                                CryptoHelper.CreateRijndael(pwd: this._pwd,
                                                                                                            salt: this._salt).CreateDecryptor(),
                                                                                CryptoStreamMode.Read);

                                        cryptoTempStream.CopyTo(cryptedDataStream);

                                        cryptedDataStream.Flush();
                                        cryptedDataStream.Close();
                                    }
                                }
                            }

                            // write to outbox of sender
                            {
                                this._host
                                    .EnqueueJob(new CopyToOutboxJob(sync: this._SYNC,
                                                                    host: this._host,
                                                                    tempFile: this._tempFile.FullName,
                                                                    pwd: this._pwd, salt: this._salt,
                                                                    sender: this._sender, recipient: this._recipient,
                                                                    meta: new XElement(this._meta)));
                            }
                        }
                        catch
                        {
                            // delete files before rethrow exception

                            this.TryDeleteFile(targetDataFile);
                            this.TryDeleteFile(targetMetaFile);
                            this.TryDeleteFile(targetMetaPwdFile);

                            throw;
                        }
                    }
                }
                else
                {
                    //TODO
                }
            }
            else
            {
                //TODO
            }
        }

        #endregion Methods (2)
    }
}