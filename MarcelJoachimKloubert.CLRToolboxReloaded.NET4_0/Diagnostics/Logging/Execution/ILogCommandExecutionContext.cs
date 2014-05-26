// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.Execution
{
    /// <summary>
    /// Describes the execution context of a log command.
    /// </summary>
    public interface ILogCommandExecutionContext : IObject
    {
        #region Data Members (5)

        /// <summary>
        /// Gets the list of arguments for the execution.
        /// </summary>
        object[] Arguments { get; }

        /// <summary>
        /// Gets the underlying command.
        /// </summary>
        ILogCommand Command { get; }

        /// <summary>
        /// Gets or sets if the value of <see cref="ILogCommandExecutionContext.MessageValueToLog" />
        /// should be send to "real" log logic or not.
        /// </summary>
        bool DoLogMessage { get; set; }

        /// <summary>
        /// Gets the original message.
        /// </summary>
        ILogMessage Message { get; }

        /// <summary>
        /// Gets or sets the message value to log.
        /// </summary>
        object MessageValueToLog { get; set; }

        #endregion Data Members (5)
    }
}