// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Web.Security;
using MarcelJoachimKloubert.FileBox.Server.Json;
using MarcelJoachimKloubert.FileBox.Server.Security;
using System;
using System.Diagnostics;
using System.Security;
using System.Security.Principal;
using System.Linq;
using System.Reflection;

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

        #region Methods (5)
        
        /// <summary>
        /// Creates a JSON object from an assembly.
        /// </summary>
        /// <param name="asm">The assembly.</param>
        /// <returns>
        /// The JSON object or <see langword="null" /> if <paramref name="asm" /> is also <see langword="null" />.
        /// </returns>
        protected static object AssemblyToJson(Assembly asm)
        {
            if (asm == null)
            {
                return null;
            }

            return new
            {
                name = asm.FullName,
            };
        }

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
        
        /// <summary>
        /// Creates a JSON object from a method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        /// The JSON object or <see langword="null" /> if <paramref name="method" /> is also <see langword="null" />.
        /// </returns>
        protected static object MethodToJson(MethodBase method)
        {
            if (method == null)
            {
                return null;
            }

            return new
            {
                name = method.Name,
                type = TypeToJson(method.DeclaringType),
            };
        }

        /// <summary>
        /// Sets up a <see cref="JsonResult" /> from an <see cref="Exception" /> object.
        /// </summary>
        /// <param name="result">The object to setup.</param>
        /// <param name="ex">The exception from where to get the data from.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="result" /> is <see langword="null" />.
        /// </exception>
        protected static void SetupJsonResultByException(JsonResult result, Exception ex)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            result.code = -1;
            result.msg = null;
            result.data = null;

            if (ex == null)
            {
                return;
            }

            var innerEx = ex.GetBaseException() ?? ex;

            object stacktrace;
            try
            {
                StackTrace st;
#if DEBUG
                st = new StackTrace(e: ex, fNeedFileInfo: true);
#else
                    st = new StackTrace(e: ex);
#endif

                stacktrace = st.GetFrames()
                               .Select(f =>
                               {
                                   return new
                                   {
                                       column = f.GetFileColumnNumber(),
                                       file = f.GetFileName(),
                                       line = f.GetFileLineNumber(),
                                       method = MethodToJson(f.GetMethod()),
                                   };
                               }).ToArray();
            }
            catch
            {
                stacktrace = innerEx.StackTrace;
            }

            
            result.msg = innerEx.Message;
            result.data = new
                {
                    fullMsg = ex.ToString(),
                    stackTrace = stacktrace,
                    type = TypeToJson(innerEx.GetType()),
                };
        }
        
        /// <summary>
        /// Creates a JSON object from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The JSON object or <see langword="null" /> if <paramref name="type" /> is also <see langword="null" />.
        /// </returns>
        protected static object TypeToJson(Type type)
        {
            if (type == null)
            {
                return null;
            }

            return new
            {
                assembly = AssemblyToJson(type.Assembly),
                name = type.FullName,
            };
        }

        #endregion Methods (1)
    }
}