// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Jobs
{
    partial class JobScheduler
    {
        #region Nested classes (1)

        /// <summary>
        /// A simple implementation of the <see cref="IJobExecutionContext" /> interface.
        /// </summary>
        protected class JobExecutionContext : IdentifiableBase, IJobExecutionContext
        {
            #region Fields (1)

            private readonly Guid _ID;

            #endregion

            #region Constructors (2)
            
            /// <summary>
            /// Initializes a new instance of the <see cref="JobExecutionContext" /> class.
            /// </summary>
            /// <remarks>The constructor generates a new unique value for <see cref="JobExecutionContext.Id" /> property.</remarks>
            public JobExecutionContext()
                : this(Guid.NewGuid())
            {

            }

            /// <summary>
            /// Initializes a new instance of the <see cref="JobExecutionContext" /> class.
            /// </summary>
            /// <param name="id">The value for the <see cref="JobExecutionContext.Id" /> property.</param>
            public JobExecutionContext(Guid id)
            {
                this._ID = id;
            }

            #endregion

            #region Properties (6)
            
            /// <inheriteddoc />
            public bool IsCancelling
            {
                get { return this.State == JobExecutionState.Cancelling; }
            }

            /// <inheriteddoc />
            public IJob Job
            {
                get;
                set;
            }
            
            /// <inheriteddoc />
            public override Guid Id
            {
                get { return this._ID; }
            }

            /// <inheriteddoc />
            public IDictionary<string, object> ResultVars
            {
                get;
                set;
            }

            /// <inheriteddoc />
            public DateTimeOffset Time
            {
                get;
                set;
            }

            /// <inheriteddoc />
            public JobExecutionState State
            {
                get;
                set;
            }

            #endregion Properties
        }

        #endregion Nested classes
    }
}