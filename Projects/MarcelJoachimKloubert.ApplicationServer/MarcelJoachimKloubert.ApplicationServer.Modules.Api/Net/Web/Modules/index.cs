// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Net.Web;
using System;
using System.ComponentModel.Composition;

namespace MarcelJoachimKloubert.ApplicationServer.Modules.Api.Net.Web.Modules
{
    /// <summary>
    /// The start module.
    /// </summary>
    [DefaultWebInterfaceModule]
    [Export(typeof(global::MarcelJoachimKloubert.ApplicationServer.Net.Web.IWebInterfaceModule))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class index : ApiWebModuleBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="index" /> class.
        /// </summary>
        [ImportingConstructor]
        public index(IApplicationServer server)
            : base(id: new Guid("{E2E4BD79-D0D4-4255-AFC3-4523DFE78103}"),
                   server: server)
        {
        }

        #endregion Constructors (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnHandle(IWebExecutionContext context, ref bool invokeAfterHandle)
        {
            var resp = context.Response;

            resp.Write("API start page");
        }

        #endregion Methods (1)
    }
}