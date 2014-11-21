// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Helpers;
using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Net.Http.Listener;
using System;
using System.IO;
using System.Net;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Http
{
    internal sealed class HttpServer : HttpListenerServer
    {
        #region Fields (2)

        private readonly ApplicationServer _APP_SERVER;
        private readonly Random _RANDOM;

        #endregion Fields (2)

        #region Constructors (1)

        internal HttpServer(ApplicationServer appServer)
        {
            this._APP_SERVER = appServer;
            this._RANDOM = new CryptoRandom();

            this.DefineTempDirectory();
        }

        #endregion Constructors (1)

        #region Properties (1)

        internal string TempDirectory
        {
            get;
            private set;
        }

        #endregion Properties (1)

        #region Methods (7)

        protected override void CloseRequestStream(HttpListenerContext ctx, Stream stream)
        {
            FileHelper.ShredderAndDeleteFile(stream as FileStream);
        }

        protected override void CloseResponseStream(HttpListenerContext ctx, Stream stream)
        {
            FileHelper.ShredderAndDeleteFile(stream as FileStream);
        }

        protected override Stream CreateRequestStream(HttpListenerContext ctx)
        {
            return this.CreateTempFile();
        }

        protected override Stream CreateResponseStream(HttpListenerContext ctx)
        {
            return this.CreateTempFile();
        }

        private FileStream CreateTempFile(string extension = "tmp")
        {
            return this._APP_SERVER
                       .Context
                       .CreateAndOpenTempFile(tempDir: this.TempDirectory,
                                              extension: extension);
        }

        private void DefineTempDirectory()
        {
            this.TempDirectory = this._APP_SERVER
                                     .Context
                                     .CreateTempDirectory();
        }

        #endregion Methods (7)
    }
}