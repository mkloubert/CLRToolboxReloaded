// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Net.Http;
using MarcelJoachimKloubert.FileBox.Server.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.FileBox.Server.Handlers
{
    internal abstract class HttpHandlerBase : DisposableObjectBase
    {
        #region Fields (4)

        protected readonly FileBoxHost _HOST;
        private readonly IHttpServer _SERVER;

        internal const string GUID_FORMAT = "N";
        internal const string LONG_TIME_FORMAT = "u";

        #endregion Fields (4)

        #region Constructors (1)

        protected HttpHandlerBase(FileBoxHost host, IHttpServer server)
        {
            this._HOST = host;

            this._SERVER = server;

            this._SERVER.HandleRequest += this.HandleRequest;
        }

        #endregion Constructors (1)

        #region Events (1)

        protected abstract void HandleRequest(object sender, HttpRequestEventArgs e);

        #endregion Events (1)

        #region Methods (8)

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

        protected override void OnDispose(DisposeContext ctx)
        {
            this._SERVER.RequestValidator = (r) => false;
            this._SERVER.CredentialValidator = (usr, pwd) => false;
            this._SERVER.PrincipalFinder = (i) => null;

            this._SERVER.HandleRequest -= this.HandleRequest;

            switch (ctx)
            {
                case DisposeContext.DisposeMethod:
                    if (this._SERVER.IsDisposed == false)
                    {
                        this._SERVER.Dispose();
                    }
                    break;

                case DisposeContext.Finalizer:
                    if (this._SERVER.IsRunning)
                    {
                        this._SERVER.Stop();
                    }
                    break;
            }
        }

        internal void Start()
        {
            this._SERVER.Start();
        }

        protected static void SetupJsonResultByException(JsonResult result, Exception ex)
        {
            if (result == null)
            {
                return;
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

        internal void TryDeleteFile(string path)
        {
            this._HOST
                .TryDeleteFile(path: path);
        }

        protected void TryDeleteFile(FileInfo file)
        {
            this._HOST
                .TryDeleteFile(file: file);
        }

        protected static DateTimeOffset? TryParseTime(string str)
        {
            DateTimeOffset? result = null;

            try
            {
                if (string.IsNullOrWhiteSpace(str) == false)
                {
                    DateTimeOffset temp;
                    if (DateTimeOffset.TryParseExact(input: str.Trim(),
                                                     format: LONG_TIME_FORMAT,
                                                     formatProvider: CultureInfo.InvariantCulture,
                                                     styles: DateTimeStyles.None,
                                                     result: out temp))
                    {
                        result = temp;
                    }
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }

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

        #endregion Methods (8)
    }
}