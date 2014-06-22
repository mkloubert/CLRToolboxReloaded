// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;

namespace MarcelJoachimKloubert.FileBox.Server.Execution.Jobs
{
    /// <summary>
    /// Describes a job queue.
    /// </summary>
    public interface IJobQueue : IObject
    {
        #region Methods (1)

        /// <summary>
        /// Enqueues a new job (item).
        /// </summary>
        /// <param name="job">The job to enqueue.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="job" /> is <see langword="null" />.
        /// </exception>
        void Enqueue(IJob job);

        #endregion Methods (1)
    }
}