// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Net.Http.Listener;
using NUnit.Framework;
using System.Net;

namespace MarcelJoachimKloubert.CLRToolbox._Tests.Net
{
    /// <summary>
    /// HTTP server tests.
    /// </summary>
    public sealed class HttpServer : TestFixtureBase
    {
        #region Methods (1)

        [Test]
        public void HttpListener()
        {
            var srv = new HttpListenerServer();
            srv.HandleRequest += (s, e) =>
                {
                    if (e.Request.Address != null)
                    {

                    }
                };
            srv.Port = 23979;
            srv.Start();

            var d = new WebClient().DownloadData("http://localhost:23979/a");
        }

        #endregion Methods (1)
    }
}