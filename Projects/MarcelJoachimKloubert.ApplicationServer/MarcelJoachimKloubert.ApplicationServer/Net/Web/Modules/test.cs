// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.ComponentModel.Composition;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web.Modules
{
    /// <summary>
    /// The test module.
    /// </summary>
    [Export(typeof(global::MarcelJoachimKloubert.ApplicationServer.Net.Web.IWebInterfaceModule))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class test : ApplicationServerWebModuleBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="test" /> class.
        /// </summary>
        [ImportingConstructor]
        public test(IApplicationServer server)
            : base(id: new Guid("{CD235DBB-2DF8-4292-8192-0E56C42A01D8}"),
                   server: server)
        {
        }

        #endregion Constructors (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnHandle(IWebExecutionContext context, ref bool invokeAfterHandle)
        {
            context.Response.Write("Test page");
        }

        #endregion Methods (1)
    }
}