// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Web;
using System;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    /// <summary>
    /// The HTTP handler that lists a directory.
    /// </summary>
    public sealed class ListHttpHandler : FileBoxHttpHandlerBase
    {
        #region Constrcutors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="" /> class.
        /// </summary>
        /// <param name="handler">
        /// The handler for the <see cref="FileBoxHttpHandlerBase.CheckLogin(string, SecureString, ref bool, ref IPrincipal)" /> method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler" /> is <see langword="null" />.
        /// </exception>
        public ListHttpHandler(CheckLoginHandler handler)
            : base(handler: handler)
        {
        }

        #endregion Constrcutors (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnProcessRequest_Authorized(IHttpRequestContext context)
        {
        }

        #endregion Methods (1)
    }
}