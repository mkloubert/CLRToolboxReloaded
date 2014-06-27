// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Security;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    partial class ClientToServerHttpHandler
    {
        #region Methods (1)

        private void ReceiveOutboxFile(HttpRequestEventArgs e)
        {
            var sender = (IServerPrincipal)e.Request.User;

            this.ReceiveBoxFile(e,
                                boxPath: sender.Outbox);
        }

        #endregion Methods (1)
    }
}