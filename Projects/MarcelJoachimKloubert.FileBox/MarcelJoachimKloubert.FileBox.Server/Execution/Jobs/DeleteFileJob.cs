// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using System;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.FileBox.Server.Execution.Jobs
{
    internal sealed class DeleteFileJob : JobBase
    {
        #region Fields (2)

        private const int _BLOCK_SIZE = 8192;
        private readonly string _FILE_PATH;

        #endregion Fields (2)

        #region Constructors (1)

        internal DeleteFileJob(string filePath)
            : base(id: new Guid("{095A5FF8-C986-4678-BF8A-376298EE5E2A}"))
        {
            this._FILE_PATH = filePath;
        }

        #endregion Constructors (1)

        #region Methods (3)

        protected override void OnCompleted(IJobExecutionContext ctx)
        {
            if (File.Exists(this._FILE_PATH))
            {
                File.Delete(this._FILE_PATH);
            }
        }

        protected override void OnCanExecute(DateTimeOffset time, ref bool canExecuteJob)
        {
            canExecuteJob = File.Exists(this._FILE_PATH);
        }

        protected override void OnExecute(IJobExecutionContext ctx)
        {
            var rand = new CryptoRandom();

            using (var stream = new FileStream(path: this._FILE_PATH,
                                               mode: FileMode.Open, access: FileAccess.ReadWrite))
            {
                var fileSize = stream.Length;

                for (var i = 0; i < 3; i++)
                {
                    if (ctx.IsCancelling)
                    {
                        break;
                    }

                    Action<int> writeBlock = (sizeOfBlock) =>
                    {
                        if (sizeOfBlock < 1)
                        {
                            return;
                        }

                        byte[] buffer;

                        if (i != 1)
                        {
                            buffer = new byte[sizeOfBlock];
                            rand.NextBytes(buffer);
                        }
                        else
                        {
                            buffer = Enumerable.Repeat((byte)0, sizeOfBlock)
                                               .ToArray();
                        }

                        stream.Write(buffer, 0, buffer.Length);
                    };

                    stream.Position = 0;

                    // write blocks
                    var numBlocks = (long)Math.Floor((double)fileSize / (double)_BLOCK_SIZE);
                    for (long ii = 0; ii < numBlocks; ii++)
                    {
                        if (ctx.IsCancelling)
                        {
                            break;
                        }

                        writeBlock(_BLOCK_SIZE);
                    }

                    if (ctx.IsCancelling)
                    {
                        break;
                    }

                    // fill last / rest block
                    writeBlock((int)(fileSize % (long)_BLOCK_SIZE));
                }
            }
        }

        #endregion Methods (3)
    }
}