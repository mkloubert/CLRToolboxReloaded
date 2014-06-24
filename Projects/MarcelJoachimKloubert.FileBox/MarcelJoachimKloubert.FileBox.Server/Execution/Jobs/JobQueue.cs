// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using System;
using System.Collections.Concurrent;

namespace MarcelJoachimKloubert.FileBox.Server.Execution.Jobs
{
    internal sealed class JobQueue : ObjectBase, IJobQueue
    {
        #region Fields (1)

        internal readonly ConcurrentQueue<IJob> JOBS = new ConcurrentQueue<IJob>();

        #endregion Fields (1)

        #region Method (1)

        public void Enqueue(IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job");
            }

            this.JOBS.Enqueue(job);
        }

        #endregion Method (1)
    }
}