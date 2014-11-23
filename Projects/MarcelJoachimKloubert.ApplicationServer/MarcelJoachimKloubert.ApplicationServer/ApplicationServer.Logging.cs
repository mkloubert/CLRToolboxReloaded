// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.ApplicationServer
{
    partial class ApplicationServer
    {
        #region Properties (2)

        /// <summary>
        /// Returns the list of loggers.
        /// </summary>
        public ILogger[] AllLoggers
        {
            get { return this.Get<ILogger[]>(); }

            private set { this.Set(value); }
        }

        /// <summary>
        /// Returns the underlying logger.
        /// </summary>
        public ILogger Logger
        {
            get { return this.Context.Logger; }
        }

        #endregion Properties (2)

        #region Methods (2)

        /// <summary>
        /// Provides the loggers of server.
        /// </summary>
        /// <param name="logger">The underlying logger.</param>
        /// <returns>The loggers to use.</returns>
        public IEnumerable<ILogger> GetLoggers(AggregateLogger logger)
        {
            return this.AllLoggers
                       .Concat(this.Context
                                   .GetAllInstances<ILogger>());
        }

        private void ReloadLoggers()
        {
            this.AllLoggers = new ILogger[]
            {
                ConsoleLogger.Create(),
            };
        }

        #endregion Methods (2)
    }
}