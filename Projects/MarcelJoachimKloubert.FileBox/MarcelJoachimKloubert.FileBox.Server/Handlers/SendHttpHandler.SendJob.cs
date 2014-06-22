// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using MarcelJoachimKloubert.CLRToolbox.ServiceLocation;
using MarcelJoachimKloubert.FileBox.Server.Execution.Jobs;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    partial class SendHttpHandler
    {
        #region Nested classes (1)

        private sealed class SendJob : SendJobBase
        {
            #region Constructors (1)

            internal SendJob(object sync,
                             string tempFile,
                             byte[] pwd, byte[] salt,
                             IServerPrincipal sender, string recipient,
                             XElement meta)
                : base(sync: sync,
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
                TryDeleteFile(this._tempFile);
            }

            protected override void OnExecute(IJobExecutionContext ctx)
            {
                var rand = new CryptoRandom();

                IServerPrincipal recipient = ServerPrincipal.FromUsername(this._recipient);

                var targetDir = new DirectoryInfo(recipient.Inbox);
                if (targetDir.Exists)
                {
                    var rsa = recipient.TryGetRsaCrypter();
                    if (rsa != null)
                    {
                        // find filenames that are NOT in use yet
                        FileInfo targetDataFile;
                        FileInfo targetMetaFile;
                        FindUniqueDataAndMetaFileNames(targetDir,
                                                        dataFile: out targetDataFile,
                                                        metaFile: out targetMetaFile);

                        using (var tempStream = new FileStream(this._tempFile.FullName,
                                                               FileMode.Open, FileAccess.Read))
                        {
                            try
                            {
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
                                    {
                                        var metaBlob = new UTF8Encoding().GetBytes(meta.ToString());
                                        var cryptedMetaBlob = rsa.Encrypt(metaBlob, false);

                                        metaStream.Write(cryptedMetaBlob, 0, cryptedMetaBlob.Length);
                                    }

                                    using (var dataStream = new FileStream(targetDataFile.FullName,
                                                                           FileMode.CreateNew, FileAccess.ReadWrite))
                                    {
                                        var cryptedDataStream = new CryptoStream(dataStream,
                                                                                 CreateRijndael(pwd: filePwd,
                                                                                                salt: fileSalt).CreateEncryptor(),
                                                                                 CryptoStreamMode.Write);
                                        {
                                            // copy from temp file to target file
                                            var cryptoTempStream = new CryptoStream(tempStream,
                                                                                    CreateRijndael(pwd: this._pwd,
                                                                                                    salt: this._salt).CreateDecryptor(),
                                                                                    CryptoStreamMode.Read);
                                            cryptoTempStream.CopyTo(cryptedDataStream);

                                            cryptedDataStream.Flush();
                                            cryptedDataStream.Close();
                                        }
                                    }

                                    metaStream.Flush();
                                    metaStream.Close();
                                }

                                // write to outbox of sender
                                {
                                    var queue = ServiceLocator.Current.GetInstance<IJobQueue>();

                                    queue.Enqueue(new CopyToOutboxJob(sync: this._SYNC,
                                                                      tempFile: this._tempFile.FullName,
                                                                      pwd: this._pwd, salt: this._salt,
                                                                      sender: this._sender, recipient: this._recipient,
                                                                      meta: new XElement(this._meta)));
                                }
                            }
                            catch
                            {
                                // delete files before rethrow exception
                                TryDeleteFile(targetDataFile);
                                TryDeleteFile(targetMetaFile);

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

        #endregion Nested classes (1)
    }
}