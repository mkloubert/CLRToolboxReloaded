// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Monitoring
{
    /// <summary>
    /// Simple implementation of <see cref="IMonitorItem" /> interface.
    /// </summary>
    public sealed class MonitorItem : ObjectBase, IMonitorItem
    {
        #region Properties (3)

        /// <inheriteddoc />
        public IMonitor Monitor
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public DateTimeOffset LastUpdate
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public MonitorItemState? State
        {
            get;
            set;
        }

        #endregion Properties (3)
    }
}