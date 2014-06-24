// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Net.Http.Listener;
using MarcelJoachimKloubert.FileBox.Server.Execution.Jobs;
using MarcelJoachimKloubert.FileBox.Server.Helpers;
using System.IO;
using System.Net;

namespace MarcelJoachimKloubert.FileBox.Server.Net.Http
{
    internal sealed class FileBoxHttpServer : HttpListenerServer
    {
        #region Fields (1)

        private readonly FileBoxHost _HOST;

        #endregion Fields (1)

        #region Constructors (1)

        internal FileBoxHttpServer(FileBoxHost host)
        {
            this._HOST = host;
        }

        #endregion Constructors (1)

        #region Methods (6)

        private void CloseFileAndDeleteStream(Stream stream)
        {
            var fs = stream as FileStream;
            if (fs == null)
            {
                return;
            }

            var file = new FileInfo(fs.Name);
            fs.Dispose();

            this.TryDeleteFile(file);
        }

        protected override void CloseRequestStream(HttpListenerContext ctx, Stream stream)
        {
            this.CloseFileAndDeleteStream(stream: stream);
        }

        protected override void CloseResponseStream(HttpListenerContext ctx, Stream stream)
        {
            this.CloseFileAndDeleteStream(stream: stream);
        }

        protected override Stream CreateRequestStream(HttpListenerContext ctx)
        {
            var tempDir = new DirectoryInfo(this._HOST.TempDirectory);

            var tmpFile = FileHelper.CreateTempFile(tempDir);
            try
            {
                FileStream result = null;
                try
                {
                    result = new FileStream(tmpFile.FullName,
                                            FileMode.Open,
                                            FileAccess.ReadWrite);

                    ctx.Request.InputStream.CopyTo(result);
                    result.Position = 0;
                }
                catch
                {
                    if (result != null)
                    {
                        result.Dispose();
                    }

                    throw;
                }

                return result;
            }
            catch
            {
                this.TryDeleteFile(tmpFile);

                throw;
            }
        }

        protected override Stream CreateResponseStream(HttpListenerContext ctx)
        {
            var tempDir = new DirectoryInfo(this._HOST.TempDirectory);

            var tmpFile = FileHelper.CreateTempFile(tempDir);
            try
            {
                FileStream result = null;
                try
                {
                    return new FileStream(tmpFile.FullName,
                                          FileMode.Open,
                                          FileAccess.ReadWrite);
                }
                catch
                {
                    if (result != null)
                    {
                        result.Dispose();
                    }

                    throw;
                }
            }
            catch
            {
                this.TryDeleteFile(tmpFile);

                throw;
            }
        }

        private void TryDeleteFile(FileInfo file)
        {
            if (file == null)
            {
                return;
            }

            this._HOST
                .EnqueueJob(new DeleteFileJob(filePath: file.FullName));
        }

        #endregion Methods (6)
    }
}