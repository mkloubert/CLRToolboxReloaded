// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Net;
using System.Web;
using System.Web.Routing;

namespace MarcelJoachimKloubert.CLRToolbox.Web
{
    /// <summary>
    /// A basic HTTP handler.
    /// </summary>
    public abstract class HttpHandlerBase : ObjectBase, IHttpHandler, IRouteHandler
    {
        #region Fields (1)

        private readonly Action<HttpContext> _ON_PROCESS_ACTION;

        #endregion Fields (1)

        #region Constructors (4)

        /// <inheriteddoc />
        protected HttpHandlerBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._ON_PROCESS_ACTION = this.ProcessRequest_ThreadSafe;
            }
            else
            {
                this._ON_PROCESS_ACTION = this.ProcessRequest_NonThreadSafe;
            }
        }

        /// <inheriteddoc />
        protected HttpHandlerBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected HttpHandlerBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected HttpHandlerBase()
            : this(isSynchronized: false)
        {
        }

        #endregion Constructors (4)

        #region Properties (1)

        /// <inheriteddoc />
        public virtual bool IsReusable
        {
            get { return true; }
        }

        #endregion Properties (1)

        #region Methods (5)

        /// <inheriteddoc />
        public virtual IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        /// <inheriteddoc />
        public void ProcessRequest(HttpContext context)
        {
            this._ON_PROCESS_ACTION(context);
        }

        private void ProcessRequest_NonThreadSafe(HttpContext context)
        {
            try
            {
                this.OnProcessRequest(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusDescription = (ex.GetBaseException() ?? ex).Message ?? string.Empty;
            }
            finally
            {
                context.Response.End();
            }
        }

        private void ProcessRequest_ThreadSafe(HttpContext context)
        {
            lock (this._SYNC)
            {
                this.ProcessRequest_NonThreadSafe(context);
            }
        }

        /// <summary>
        /// Stores the logic for the <see cref="HttpHandlerBase.ProcessRequest(HttpContext)" /> method.
        /// </summary>
        /// <param name="context">The underlying context.</param>
        protected abstract void OnProcessRequest(HttpContext context);

        #endregion Methods (5)
    }
}