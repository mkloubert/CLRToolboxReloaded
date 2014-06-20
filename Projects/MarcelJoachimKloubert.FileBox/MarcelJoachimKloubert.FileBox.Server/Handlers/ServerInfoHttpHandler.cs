// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Web;
using MarcelJoachimKloubert.FileBox.Server.Extensions;
using MarcelJoachimKloubert.FileBox.Server.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    /// <summary>
    /// The HTTP handler that lists a directory.
    /// </summary>
    public sealed class ServerInfoHttpHandler : FileBoxHttpHandlerBase
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
        public ServerInfoHttpHandler(CheckLoginHandler handler)
            : base(handler: handler)
        {
        }

        #endregion Constrcutors (1)

        #region Methods (4)

        private static object AssemblyToJson(Assembly asm)
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

        private static object MethodToJson(MethodBase method)
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

        /// <inheriteddoc />
        protected override void OnProcessRequest_Authorized(IHttpRequestContext context)
        {
            var result = new JsonResult();

            try
            {
                result.code = 0;

                result.data = new
                    {
                        name = Environment.MachineName,
                    };
            }
            catch (Exception ex)
            {
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

                result.code = -1;
                result.msg = innerEx.Message;
                result.data = new
                    {
                        fullMsg = ex.ToString(),
                        stackTrace = stacktrace,
                        type = TypeToJson(innerEx.GetType()),
                    };
            }

            context.Http.Response.WriteJson(result);
        }

        private static object TypeToJson(Type type)
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

        #endregion Methods (4)
    }
}