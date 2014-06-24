// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace MarcelJoachimKloubert.FileBox.Server.Execution.Jobs
{
    internal abstract class SendJobBase : JobBase
    {
        #region Fields (7)

        protected FileBoxHost _host;
        protected XElement _meta;
        protected byte[] _pwd;
        protected string _recipient;
        protected byte[] _salt;
        protected IServerPrincipal _sender;
        protected FileInfo _tempFile;

        #endregion Fields (7)

        #region Constructors (2)

        protected internal SendJobBase(Guid id,
                                       FileBoxHost host,
                                       object sync,
                                       string tempFile,
                                       byte[] pwd, byte[] salt,
                                       IServerPrincipal sender, string recipient,
                                       XElement meta)
            : base(id: id,
                   isSynchronized: true, sync: sync)
        {
            this._host = host;

            this._tempFile = new FileInfo(Path.GetFullPath(tempFile));

            this._pwd = pwd;
            this._salt = salt;

            this._sender = sender;
            this._recipient = recipient;

            this._meta = meta;
        }

        ~SendJobBase()
        {
            this._host = null;
            this._meta = null;
            this._pwd = null;
            this._recipient = null;
            this._salt = null;
            this._sender = null;
            this._tempFile = null;
        }

        #endregion Constructors (2)

        #region Methods (4)

        protected internal static Encoding CreateXmlEncoder()
        {
            return new UTF8Encoding();
        }

        protected sealed override void OnCanExecute(DateTimeOffset time, ref bool canExecuteJob)
        {
            canExecuteJob = true;
        }

        protected internal void FindUniqueDataAndMetaFileNames(DirectoryInfo targetDir,
                                                               out FileInfo dataFile, out FileInfo metaFile, out FileInfo metaPwdFile)
        {
            dataFile = null;
            metaFile = null;
            metaPwdFile = null;

            ulong i = 0;
            do
            {
                dataFile = new FileInfo(Path.Combine(targetDir.FullName,
                                                     i + GlobalConstants.FileExtensions.DATA_FILE));
                metaFile = new FileInfo(Path.Combine(targetDir.FullName,
                                                     i + GlobalConstants.FileExtensions.META_FILE));
                metaPwdFile = new FileInfo(Path.Combine(targetDir.FullName,
                                                        i + GlobalConstants.FileExtensions.META_PASSWORD_FILE));

                if (dataFile.Exists || metaFile.Exists || metaPwdFile.Exists)
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

        protected internal void TryDeleteFile(FileInfo file)
        {
            this._host
                .TryDeleteFile(file: file);
        }

        #endregion Methods (4)
    }
}