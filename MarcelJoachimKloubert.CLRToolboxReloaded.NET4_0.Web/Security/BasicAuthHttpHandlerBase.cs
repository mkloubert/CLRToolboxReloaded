// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace MarcelJoachimKloubert.CLRToolbox.Web.Security
{
    /// <summary>
    /// A basic HTTP handler that uses Basic Auth.
    /// </summary>
    public abstract class BasicAuthHttpHandlerBase : HttpHandlerBase
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected BasicAuthHttpHandlerBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected BasicAuthHttpHandlerBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected BasicAuthHttpHandlerBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected BasicAuthHttpHandlerBase()
            : base()
        {
        }

        #endregion Constrcutors (4)

        #region Properties (1)

        /// <summary>
        /// Gets the name of the realm.
        /// </summary>
        public virtual string RealmName
        {
            get { return "CLR Toolbox Reloaded"; }
        }

        #endregion Properties (1)

        #region Methods (4)

        /// <summary>
        /// Checks login data.
        /// </summary>
        /// <param name="username">The user name in lower case characters or <see langword="null" /> if not defined.</param>
        /// <param name="pwd">The password.</param>
        /// <param name="isLoggedIn">
        /// The variabele where to define if user is logged in or not.
        /// The value is <see langword="false" /> by default.
        /// </param>
        /// <param name="user">
        /// The variable where to store the principal of the logged in user.
        /// The value is <see langword="null" /> by default.
        /// </param>
        protected abstract void CheckLogin(string username, SecureString pwd, ref bool isLoggedIn, ref IPrincipal user);

        /// <inheriteddoc />
        protected override sealed void OnProcessRequest(HttpContext context)
        {
            var now = AppTime.Now;

            IPrincipal loggedInUser = null;
            var isLoggedIn = false;

            var data = (context.Request.ServerVariables["HTTP_AUTHORIZATION"] ?? string.Empty).Trim();
            if (data != string.Empty)
            {
                if (data.ToLower().StartsWith("basic "))
                {
                    try
                    {
                        var base64EncodedData = data.Substring(data.IndexOf(" ")).Trim();
                        var blobData = Convert.FromBase64String(base64EncodedData);

                        var strData = new UTF8Encoding().GetString(blobData);

                        var semicolon = strData.IndexOf(":");
                        if (semicolon > -1)
                        {
                            var username = strData.Substring(0, semicolon).ToLower().Trim();
                            if (username == string.Empty)
                            {
                                username = null;
                            }

                            using (var secPwd = new SecureString())
                            {
                                // extract password
                                string pwd = null;
                                try
                                {
                                    if (semicolon < (strData.Length - 1))
                                    {
                                        pwd = strData.Substring(semicolon + 1);
                                    }

                                    if (pwd != null)
                                    {
                                        // save password to secure string

                                        pwd.ForEach((ctx) =>
                                            {
                                                ctx.State
                                                   .SecurePassword.AppendChar(ctx.Item);
                                            }, actionState: new
                                            {
                                                SecurePassword = secPwd,
                                            });
                                    }
                                }
                                finally
                                {
                                    pwd = null;
                                }

                                // check login
                                this.CheckLogin(username: username, pwd: secPwd,
                                                isLoggedIn: ref isLoggedIn, user: ref loggedInUser);
                            }
                        }
                    }
                    catch
                    {
                        // ignore errors here
                    }
                }
            }

            var httpCtx = new HttpRequestContext()
                {
                    Http = context,
                    Time = now,
                    User = loggedInUser,
                };

            if (isLoggedIn == false)
            {
                this.OnProcessRequest_AuthorizationFailed(httpCtx);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;
            this.OnProcessRequest_Authorized(httpCtx);
        }

        /// <summary>
        /// Is invoked if authorization was successful.
        /// </summary>
        /// <param name="context">The underlying context.</param>
        protected abstract void OnProcessRequest_Authorized(IHttpRequestContext context);

        /// <summary>
        /// Is invoked if authorization failed.
        /// </summary>
        /// <param name="context">The underlying context.</param>
        protected virtual void OnProcessRequest_AuthorizationFailed(IHttpRequestContext context)
        {
            context.Http.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Http.Response.AddHeader("WWW-Authenticate", "BASIC Realm=" + this.RealmName);
        }

        #endregion Methods (4)
    }
}