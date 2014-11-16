// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.Execution
{
    partial class LogCommandBase
    {
        #region CLASS: LogCommandExecutionContext

        private sealed class LogCommandExecutionContext : ObjectBase, ILogCommandExecutionContext
        {
            #region Properties (5)

            public object[] Arguments
            {
                get;
                internal set;
            }

            public ILogCommand Command
            {
                get;
                internal set;
            }

            public bool DoLogMessage
            {
                get;
                set;
            }

            public ILogMessage Message
            {
                get;
                internal set;
            }

            public object MessageValueToLog
            {
                get;
                set;
            }

            #endregion CLASS: LogCommandExecutionContext
        }

        #endregion

        #region CLASS: LogCommandExecutionResult

        private sealed class LogCommandExecutionResult : ObjectBase, ILogCommandExecutionResult
        {
            #region Properties (6)

            public ILogCommand Command
            {
                get;
                internal set;
            }

            public bool DoLogMessage
            {
                get;
                internal set;
            }

            public IList<Exception> Errors
            {
                get;
                internal set;
            }

            public bool HasFailed
            {
                get
                {
                    var errs = this.Errors;
                    return (errs != null) &&
                           errs.Any(ex => ex != null);
                }
            }

            public ILogMessage Message
            {
                get;
                internal set;
            }

            public object MessageValueToLog
            {
                get;
                internal set;
            }

            #endregion Properties
        }

        #endregion
    }
}