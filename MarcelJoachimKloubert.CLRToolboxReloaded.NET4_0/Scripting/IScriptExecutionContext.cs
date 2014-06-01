// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Scripting
{
    /// <summary>
    /// Describes a script execution context.
    /// </summary>
    public interface IScriptExecutionContext : INotifiable
    {
        #region Delegates and events (3)

        /// <summary>
        /// Is invoked if execution was completed.
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// Is invoked if execution was has been failed.
        /// </summary>
        event EventHandler Failed;

        /// <summary>
        /// Is invoked if execution was succeeded.
        /// </summary>
        event EventHandler Succeed;

        #endregion Delegates and events (3)

        #region Data Members (9)

        /// <summary>
        /// Gets the time the execution has been ended.
        /// </summary>
        DateTimeOffset? EndTime { get; }

        /// <summary>
        /// Gets the list of occured exceptions or <see langword="null" /> if execution has not been finished yet.
        /// </summary>
        IList<Exception> Exceptions { get; }

        /// <summary>
        /// Gets the underlying executor.
        /// </summary>
        IScriptExecutor Executor { get; }

        /// <summary>
        /// Gets if the execution has failed or not.
        /// </summary>
        bool HasFailed { get; }

        /// <summary>
        /// Gets if in debug mode or not.
        /// </summary>
        bool IsDebug { get; }

        /// <summary>
        /// Gets if the script is currently executing or not.
        /// </summary>
        bool IsExecuting { get; }

        /// <summary>
        /// Gets the script result.
        /// </summary>
        object Result { get; }

        /// <summary>
        /// Gets the underlying script source.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Gets the start time if available.
        /// </summary>
        DateTimeOffset? StartTime { get; }

        #endregion Data Members (9)

        #region Operations (1)

        /// <summary>
        /// Starts the execution.
        /// </summary>
        void Start();

        #endregion Operations
    }
}