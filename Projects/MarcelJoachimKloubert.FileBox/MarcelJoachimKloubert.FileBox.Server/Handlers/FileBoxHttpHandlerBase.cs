// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Web.Security;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.Security;
using System.Security.Principal;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    /// <summary>
    /// A basic HTTP handler.
    /// </summary>
    public abstract class FileBoxHttpHandlerBase : BasicAuthHttpHandlerBase
    {
        #region Fields (1)

        private readonly CheckLoginHandler _CHECK_LOGIN_HANDLER;

        #endregion Fields (1)

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
        protected FileBoxHttpHandlerBase(CheckLoginHandler handler)
            : base(isSynchronized: false)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            this._CHECK_LOGIN_HANDLER = handler;
        }

        #endregion Constrcutors (1)

        #region Events and delegates (1)

        /// <summary>
        /// A login handler for a <see cref="FileBoxHttpHandlerBase.CheckLogin(string, SecureString, ref bool, ref IPrincipal)" /> method.
        /// </summary>
        /// <param name="username">The lower case username or <see langword="null" /> if not defined.</param>
        /// <param name="pwd">The password or <see langword="null" /> if not defined.</param>
        /// <param name="user">The variable where to define the logged in user object.</param>
        public delegate void CheckLoginHandler(string username, string pwd,
                                               ref IServerPrincipal user);

        #endregion Events and delegates (1)

        #region Properties (1)
        
        /// <inheriteddoc />
        public override string RealmName
        {
            get { return "FileBox"; }
        }

        #endregion

        #region Methods (1)

        /// <inheriteddoc />
        protected override sealed void CheckLogin(string username, SecureString pwd,
                                                  ref bool isLoggedIn, ref IPrincipal user)
        {
            string strPwd;
            try
            {
                strPwd = pwd.ToUnsecureString();
                if (strPwd == string.Empty)
                {
                    strPwd = null;
                }

                IServerPrincipal fbUser = null;
                this._CHECK_LOGIN_HANDLER(username: username, pwd: strPwd,
                                          user: ref fbUser);

                user = fbUser;
                isLoggedIn = user != null;
            }
            finally
            {
                strPwd = null;
            }
        }

        #endregion Methods (1)
    }
}