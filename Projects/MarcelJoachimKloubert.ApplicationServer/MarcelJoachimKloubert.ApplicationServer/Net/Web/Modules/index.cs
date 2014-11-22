// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.ComponentModel.Composition;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web.Modules
{
    /// <summary>
    /// The start module.
    /// </summary>
    [DefaultWebInterfaceModule]
    [Export(typeof(global::MarcelJoachimKloubert.ApplicationServer.Net.Web.IWebInterfaceModule))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class index : ApplicationServerWebModuleBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="index" /> class.
        /// </summary>
        [ImportingConstructor]
        public index(IApplicationServer server)
            : base(id: new Guid("{22932D5B-CCBA-4A95-B126-8ADB9E4F86A9}"),
                   server: server)
        {
        }

        #endregion Constructors (1)

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnHandle(IWebExecutionContext context, ref bool invokeAfterHandle)
        {
            var resp = context.Response;

            resp.Write(context.TryGetHtmlTemplate("index"));
        }

        #endregion Methods (1)
    }
}