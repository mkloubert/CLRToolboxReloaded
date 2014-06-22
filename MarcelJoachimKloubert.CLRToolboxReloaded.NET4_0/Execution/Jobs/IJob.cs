// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Jobs
{
    /// <summary>
    /// Describes a job.
    /// </summary>
    public interface IJob : IIdentifiable
    {
        #region Methods (4)

        /// <summary>
        /// Checks if that job can be executed at a specific time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>
        /// Can execute at <paramref name="time" /> or not.
        /// </returns>
        bool CanExecute(DateTimeOffset time);
        
        /// <summary>
        /// Is executed if an execution completed.
        /// This is ALWAYS executed after <see cref="IJob.Error(IJobExecutionContext, Exception)" />.
        /// </summary>
        /// <param name="ctx">The underlying context.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctx" /> is <see langword="null" />.
        /// </exception>
        void Completed(IJobExecutionContext ctx);

        /// <summary>
        /// Is executed if an execution failed.
        /// </summary>
        /// <param name="ctx">The underlying context.</param>
        /// <param name="ex">The occured exception(s).</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctx" /> and/or <paramref name="ex" /> are <see langword="null" />.
        /// </exception>
        void Error(IJobExecutionContext ctx, Exception ex);

        /// <summary>
        /// Executes that job.
        /// </summary>
        /// <param name="ctx">The underlying context.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctx" /> is <see langword="null" />.
        /// </exception>
        void Execute(IJobExecutionContext ctx);

        #endregion Methods
    }
}