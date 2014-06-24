// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution.Jobs;
using System.Web.Hosting;

namespace MarcelJoachimKloubert.FileBox.Server.Execution.Jobs
{
    internal sealed class FileBoxJobScheduler : JobScheduler, IRegisteredObject
    {
        #region Constructors (1)

        internal FileBoxJobScheduler(JobProvider provider)
            : base(provider: provider)
        {
        }

        #endregion Constructors (1)

        #region Methods (1)

        void IRegisteredObject.Stop(bool immediate)
        {
            this.Stop();
        }

        #endregion Methods (1)
    }
}