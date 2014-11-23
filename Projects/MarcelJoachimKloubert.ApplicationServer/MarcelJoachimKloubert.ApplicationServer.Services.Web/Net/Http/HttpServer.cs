// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Helpers;
using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Net.Http.Listener;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;

namespace MarcelJoachimKloubert.ApplicationServer.Services.Web.Net.Http
{
    [Export(typeof(global::MarcelJoachimKloubert.CLRToolbox.Net.Http.IHttpServer))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class HttpServer : HttpListenerServer
    {
        #region Fields (2)

        private readonly IApplicationServer _APP_SERVER;
        private readonly Random _RANDOM;

        #endregion Fields (2)

        #region Constructors (1)

        [ImportingConstructor]
        internal HttpServer(IApplicationServer appServer)
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

        private static void CloseListenerStream(HttpListenerContext ctx, Stream stream)
        {
            var fs = stream as FileStream;
            if (fs == null)
            {
                stream.DisposeEx();
                return;
            }

            FileHelper.ShredderAndDeleteFile(fs);
        }

        protected override void CloseRequestStream(HttpListenerContext ctx, Stream stream)
        {
            CloseListenerStream(ctx, stream);
        }

        protected override void CloseResponseStream(HttpListenerContext ctx, Stream stream)
        {
            CloseListenerStream(ctx, stream);
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