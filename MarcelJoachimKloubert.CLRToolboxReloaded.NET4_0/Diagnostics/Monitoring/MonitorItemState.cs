// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Monitoring
{
    /// <summary>
    /// List of monitor states.
    /// </summary>
    public enum MonitorItemState
    {
        /// <summary>
        /// OK (green)
        /// </summary>
        OK = 1,

        /// <summary>
        /// Warning (yellow)
        /// </summary>
        Warning,

        /// <summary>
        /// Error (red)
        /// </summary>
        Error,

        /// <summary>
        /// Cryitical (alert; yellow font / red background)
        /// </summary>
        Critical,
    }
}