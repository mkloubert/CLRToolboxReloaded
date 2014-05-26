// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution.Commands;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.Execution
{
    /// <summary>
    /// Describes a log command.
    /// </summary>
    public interface ILogCommand : ICommand<ILogMessage>
    {
        #region Operations (1)

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="orgMsg">The original message.</param>
        /// <returns>The result of the execution.</returns>
        new ILogCommandExecutionResult Execute(ILogMessage orgMsg);

        #endregion Operations
    }
}