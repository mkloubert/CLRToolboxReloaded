// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Monitoring
{
    /// <summary>
    /// Stores the data of a monitor state.
    /// </summary>
    public interface IMonitorItem : IObject
    {
        #region Properties (3)

        /// <summary>
        /// Gets the underlying monitor.
        /// </summary>
        IMonitor Monitor { get; }

        /// <summary>
        /// Gets the time stamp of the last udate.
        /// </summary>
        DateTimeOffset LastUpdate { get; }

        /// <summary>
        /// Gets the state value.
        /// </summary>
        MonitorItemState? State { get; }

        #endregion Properties (3)
    }
}