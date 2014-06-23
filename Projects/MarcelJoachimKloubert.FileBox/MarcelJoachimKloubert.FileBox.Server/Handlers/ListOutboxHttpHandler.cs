// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Web;
using MarcelJoachimKloubert.FileBox.Server.Security;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    /// <summary>
    /// The HTTP handler that lists the OUTBOX of the connected user.
    /// </summary>
    public sealed class ListOutboxHttpHandler : ListBoxHttpHandlerBase
    {
        #region Constrcutors (1)

        /// <inheriteddoc />
        public ListOutboxHttpHandler(CheckLoginHandler handler)
            : base(handler: handler)
        {
        }

        #endregion Constrcutors (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override string GetBoxPath(IHttpRequestContext context)
        {
            return context.GetUser<IServerPrincipal>().Outbox;
        }

        #endregion Methods (1)
    }
}