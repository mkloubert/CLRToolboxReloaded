// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Functions
{
    partial class FunctionBase
    {
        /// <summary>
        /// The execution context of a function.
        /// </summary>
        protected class FunctionExecutionContext : ObjectBase
        {
            #region Properties (8)

            /// <summary>
            /// Gets the total time for the execution (if all data is available).
            /// </summary>
            public TimeSpan? Duration
            {
                get
                {
                    var start = this.StartTime;
                    var end = this.EndTime;

                    return (start.HasValue && end.HasValue) ? (end - start)
                                                            : (TimeSpan?)null;
                }
            }

            /// <summary>
            /// Gets the end time.
            /// </summary>
            public DateTimeOffset? EndTime
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets the execution error or <see langword="null" /> for no error.
            /// </summary>
            public Exception Error
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets or sets the exit code.
            /// </summary>
            public int? ExitCode
            {
                get;
                set;
            }

            /// <summary>
            /// Gets if the execution has been failed or not.
            /// <see langword="null" /> indicates that execution has not been started yet.
            /// </summary>
            public bool? HasBeenFailed
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets the input parameters.
            /// </summary>
            public IReadOnlyDictionary<string, object> Input
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets the result parameters.
            /// </summary>
            public IDictionary<string, object> Result
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets the starts time.
            /// </summary>
            public DateTimeOffset? StartTime
            {
                get;
                internal set;
            }

            #endregion Properties (8)
        }
    }
}