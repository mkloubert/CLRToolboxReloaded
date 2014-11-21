// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Net.Web;
using System;
using System.ComponentModel.Composition;

namespace MarcelJoachimKloubert.ApplicationServer.Modules.Api.Net.Web.Modules
{
    /// <summary>
    /// The test module.
    /// </summary>
    [Export(typeof(global::MarcelJoachimKloubert.ApplicationServer.Net.Web.IWebInterfaceModule))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class test : ApiWebModuleBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="test" /> class.
        /// </summary>
        [ImportingConstructor]
        public test(IApplicationServer server)
            : base(id: new Guid("{EF9042D3-C3C0-4DE0-AA14-4C30E4E1EDD3}"),
                   server: server)
        {
        }

        #endregion Constructors (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnHandle(IWebExecutionContext context, ref bool invokeAfterHandle)
        {
            context.Response.Write("API Test page");
        }

        #endregion Methods (1)
    }
}