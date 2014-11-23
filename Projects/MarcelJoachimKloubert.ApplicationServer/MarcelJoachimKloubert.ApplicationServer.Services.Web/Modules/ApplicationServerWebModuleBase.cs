// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Net.Web;
using System;

namespace MarcelJoachimKloubert.ApplicationServer.Services.Web
{
    /// <summary>
    /// A basic web interface module.
    /// </summary>
    public abstract class ApplicationServerWebModuleBase : WebInterfaceModuleBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationServerWebModuleBase" /> class.
        /// </summary>
        /// <param name="id">
        /// The value for the <see cref="WebInterfaceModuleBase.Id" /> property.
        /// </param>
        protected ApplicationServerWebModuleBase(Guid id,
                                                 IApplicationServer server)
            : base(id: id)
        {
            this.Server = server;
        }

        #endregion Constructors (1)

        #region Properties (2)

        /// <summary>
        /// Gets the underlying server instance.
        /// </summary>
        public IApplicationServer Server
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the context of <see cref="ApplicationServerWebModuleBase.Server" />.
        /// </summary>
        public IApplicationServerContext ServerContext
        {
            get { return this.Server.Context; }
        }

        #endregion Properties (2)

        #region Methods (1)
        
        /// <inheriteddoc />
        protected override void OnBeforeHandle(IWebExecutionContext context, ref bool invokeOnHandle)
        {
            base.OnBeforeHandle(context, ref invokeOnHandle);

            context.Response.Compress = true;
        }

        #endregion
    }
}