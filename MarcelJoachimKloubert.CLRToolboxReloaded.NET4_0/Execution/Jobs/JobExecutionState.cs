// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Jobs
{
    /// <summary>
    /// List of job execution states.
    /// </summary>
    public enum JobExecutionState
    {
        /// <summary>
        /// Not running
        /// </summary>
        NotRunning = 0,

        /// <summary>
        /// Running
        /// </summary>
        Running,

        /// <summary>
        /// Successful finished
        /// </summary>
        Finished,

        /// <summary>
        /// Cancelling
        /// </summary>
        Cancelling,

        /// <summary>
        /// Canceled
        /// </summary>
        Canceled,

        /// <summary>
        /// Exception / faulted
        /// </summary>
        Faulted,
    }
}