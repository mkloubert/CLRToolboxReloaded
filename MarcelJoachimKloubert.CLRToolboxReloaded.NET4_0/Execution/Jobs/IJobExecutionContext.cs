// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Jobs
{
    /// <summary>
    /// Describes an execution context for a job.
    /// </summary>
    public interface IJobExecutionContext : IIdentifiable
    {
        #region Properties (5)

        /// <summary>
        /// Gets if the underlying scheduler wants to cancel the execution of the current job.
        /// </summary>
        bool IsCancelling { get; }

        /// <summary>
        /// Gets the underlying job.
        /// </summary>
        IJob Job { get; }

        /// <summary>
        /// Gets the dictionary that stores the parameters / values for the result.
        /// </summary>
        IDictionary<string, object> ResultVars { get; }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        JobExecutionState State { get; }

        /// <summary>
        /// Gets the execution / start time.
        /// </summary>
        DateTimeOffset Time { get; }

        #endregion Properties
    }
}