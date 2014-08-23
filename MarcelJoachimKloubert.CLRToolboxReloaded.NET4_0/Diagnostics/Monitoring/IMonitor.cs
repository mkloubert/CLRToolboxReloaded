// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Monitoring
{
    /// <summary>
    /// Describes a monitor.
    /// </summary>
    public interface IMonitor : IObject
    {
        #region Methods (1)

        /// <summary>
        /// Returns the list of (current) items.
        /// </summary>
        /// <returns>The list of (current) items.</returns>
        IEnumerable<IMonitorItem> GetItems();

        #endregion Methods (1)
    }
}