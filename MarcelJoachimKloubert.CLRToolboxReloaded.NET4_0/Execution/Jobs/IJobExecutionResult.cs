﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Jobs
{
    /// <summary>
    /// A result of a job execution.
    /// </summary>
    public interface IJobExecutionResult : IObject
    {
        #region Properties (5)

        /// <summary>
        /// Gets the execution context.
        /// </summary>
        IJobExecutionContext Context { get; }

        /// <summary>
        /// Gets the list of occured errors.
        /// </summary>
        IList<JobException> Errors { get; }

        /// <summary>
        /// Gets if the execution has been failed or not.
        /// </summary>
        bool HasFailed { get; }

        /// <summary>
        /// Gets the time execution has been completed.
        /// </summary>
        DateTimeOffset Time { get; }

        /// <summary>
        /// Gets the dictionary that stores the result parameters / values.
        /// </summary>
        /// <remarks>
        /// These are normally the data from an instance of an <see cref="IJobExecutionContext.ResultVars" /> property.
        /// </remarks>
        IReadOnlyDictionary<string, object> Vars { get; }

        #endregion Properties
    }
}