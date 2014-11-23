// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.ApplicationServer.Net.Web;
using System;
using System.ComponentModel.Composition;

namespace MarcelJoachimKloubert.ApplicationServer.Services.Web
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
            var resp = context.Response;

            string page;
            context.Request.REQUEST.TryGetValue("p", out page);

            if (string.IsNullOrWhiteSpace(page))
            {
                page = "index";
            }

            var tpl = context.TryGetHtmlTemplate(page);
            if (tpl == null)
            {
                tpl = context.TryGetHtmlTemplate("index");
            }

            resp.FrontendVars["javascript_code"] = context.TryLoadJavascript("custom");
            resp.FrontendVars["css_stylesheets"] = context.TryLoadStylesheets("custom");

            resp.Write(tpl);
        }

        #endregion Methods (1)
    }
}