// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using System.ComponentModel.Composition;

namespace MarcelJoachimKloubert.ApplicationServer.Services.Loggers.Xml
{
    [Export(typeof(global::MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.ILogger))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class XmlLogger : LoggerBase
    {
        #region Constructors (1)

        [ImportingConstructor]
        internal XmlLogger()
            : base(isSynchronized: true)
        {
        }

        #endregion Constructors (1)

        #region Methods (1)

        protected override void OnLog(ILogMessage msgObj, ref bool succeeded)
        {
        }

        #endregion Methods (1)
    }
}