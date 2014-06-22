// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.IO;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    partial class SendHttpHandler
    {
        #region Nested classes (1)

        private abstract class SendJobBase : JobBase
        {
            #region Fields (6)

            protected XElement _meta;
            protected byte[] _pwd;
            protected string _recipient;
            protected byte[] _salt;
            protected IServerPrincipal _sender;
            protected FileInfo _tempFile;

            #endregion Fields (6)

            #region Constructors (2)

            protected internal SendJobBase(object sync,
                                           string tempFile,
                                           byte[] pwd, byte[] salt,
                                           IServerPrincipal sender, string recipient,
                                           XElement meta)
                : base(Guid.NewGuid(),
                       isSynchronized: true, sync: sync)
            {
                this._tempFile = new FileInfo(Path.GetFullPath(tempFile));

                this._pwd = pwd;
                this._salt = salt;

                this._sender = sender;
                this._recipient = recipient;

                this._meta = meta;
            }

            ~SendJobBase()
            {
                this._meta = null;
                this._pwd = null;
                this._recipient = null;
                this._salt = null;
                this._sender = null;
                this._tempFile = null;
            }

            #endregion Constructors (2)

            #region Methods (3)

            protected sealed override void OnCanExecute(DateTimeOffset time, ref bool canExecuteJob)
            {
                canExecuteJob = true;
            }

            protected internal void FindUniqueDataAndMetaFileNames(DirectoryInfo targetDir,
                                                                   out FileInfo dataFile, out FileInfo metaFile)
            {
                dataFile = null;
                metaFile = null;

                ulong i = 0;
                do
                {
                    dataFile = new FileInfo(Path.Combine(targetDir.FullName, i + ".bin"));
                    metaFile = new FileInfo(Path.Combine(targetDir.FullName, i + ".dat"));

                    if (dataFile.Exists || metaFile.Exists)
                    {
                        ++i;
                    }
                    else
                    {
                        break;
                    }
                }
                while (true);
            }

            protected internal static void TryDeleteFile(FileInfo file)
            {
                if (file == null)
                {
                    return;
                }

                try
                {
                    var path = file.FullName;
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                catch
                {
                    // ignore here
                }
            }

            #endregion Methods (3)
        }

        #endregion Nested classes (1)
    }
}