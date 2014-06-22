// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Jobs
{
    partial class JobScheduler
    {
        #region Nested classes (1)

        /// <summary>
        /// A simple implementation of the <see cref="IJobExecutionContext" /> interface.
        /// </summary>
        protected class JobExecutionResult : ObjectBase, IJobExecutionResult
        {
            #region Properties (6)

            /// <inheriteddoc />
            public JobExecutionContext Context
            {
                get;
                set;
            }

            /// <inheriteddoc />
            public IList<JobException> Errors
            {
                get;
                set;
            }

            /// <inheriteddoc />
            public bool HasFailed
            {
                get
                {
                    var errs = this.Errors;
                    return errs != null &&
                           errs.Any((ex) => ex != null);
                }
            }

            /// <inheriteddoc />
            public DateTimeOffset Time
            {
                get;
                set;
            }

            /// <inheriteddoc />
            public IReadOnlyDictionary<string, object> Vars
            {
                get;
                set;
            }

            IJobExecutionContext IJobExecutionResult.Context
            {
                get { return this.Context; }
            }

            #endregion Properties
        }

        #endregion Nested classes
    }
}