// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Values
{
    /// <summary>
    /// Values of a traffic light.
    /// </summary>
    public enum TrafficLight
    {
        /// <summary>
        /// Gray
        /// </summary>
        None = 0,

        /// <summary>
        /// Green
        /// </summary>
        OK = 1,

        /// <summary>
        /// Yellow
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Red
        /// </summary>
        Error = 3,

        /// <summary>
        /// Yellow / red
        /// </summary>
        FatalError = 4,
    }
}