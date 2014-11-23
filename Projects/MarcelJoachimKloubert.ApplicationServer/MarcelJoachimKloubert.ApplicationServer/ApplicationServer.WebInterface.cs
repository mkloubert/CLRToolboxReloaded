// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Net.Web;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;

namespace MarcelJoachimKloubert.ApplicationServer
{
    partial class ApplicationServer
    {
        #region Fields (2)

        private IHttpServer _httpServer;
        private WebUrlHandler _web_url_handler;

        #endregion Fields (2)

        #region Events (1)

        private void HttpServer_HandleRequest(object sender, HttpRequestEventArgs e)
        {
            var path = e.Request.Address.PathAndQuery ?? string.Empty;

            var found = false;

            if (string.IsNullOrWhiteSpace(path) ||
                (path.Trim() == "/"))
            {
                // default web interface module of server

                this._web_url_handler
                    .Handle_DefaultServerModule(null, e, ref found);
            }
            else
            {
                found = this._web_url_handler
                            .TryHandle(e);
            }

            if (found == false)
            {
                e.Response
                 .DocumentNotFound = true;
            }
        }

        #endregion Events (1)

        #region Methods (6)

        private bool CheckWebInterfaceUser(string username, string password)
        {
            //TODO: implement user / password check

            return (username == "admin") &&
                   (password == "admin");
        }

        private void DisposeHttpServer(ref IHttpServer server)
        {
            using (var s = server)
            {
                server = null;

                this.UnregisterHttpServerEvents(s);

                if (s != null)
                {
                    s.CredentialValidator = (u, p) => false;
                }
            }
        }

        private void DisposeOldHttpServer()
        {
            this.DisposeHttpServer(ref this._httpServer);
        }

        private void StartWebInterface()
        {
            this.DisposeOldHttpServer();

            int port;
            this.Config.TryGetValue("port", out port, "webInterface", defaultVal: 5979);

            bool useSecureHttp;
            this.Config.TryGetValue("useSecureHttp", out useSecureHttp, "webInterface", defaultVal: true);

            var newServer = this.Context.GetInstance<IHttpServer>();
            try
            {
                newServer.CredentialValidator = this.CheckWebInterfaceUser;
                newServer.Port = port;
                newServer.UseSecureHttp = useSecureHttp;

                newServer.HandleRequest += this.HttpServer_HandleRequest;

                newServer.Start();
                this._httpServer = newServer;
            }
            catch
            {
                this.DisposeHttpServer(ref newServer);

                throw;
            }
        }

        private void StopWebInterface()
        {
            this.DisposeOldHttpServer();
        }

        private void UnregisterHttpServerEvents(IHttpServer server)
        {
            if (server == null)
            {
                return;
            }

            server.HandleRequest -= this.HttpServer_HandleRequest;
        }

        #endregion Methods (6)
    }
}