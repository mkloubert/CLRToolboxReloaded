// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Threading
{
    /// <summary>
    /// List of thread options.
    /// </summary>
    public enum ThreadOption
    {
        /// <summary>
        /// Run in current thread.
        /// </summary>
        Current,

        /// <summary>
        /// Run in a separate background thread.
        /// </summary>
        Background,

        /// <summary>
        /// Run in a separate background thread, but do not wait until invokation has been finished.
        /// </summary>
        BackgroundNoWait,

        /// <summary>
        /// Run in the thread of the UI thread.
        /// </summary>
        UserInterface,
    }
}