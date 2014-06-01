// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution.Commands;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging.Execution
{
    /// <summary>
    /// A basic log command.
    /// </summary>
    public abstract partial class LogCommandBase : CommandBase<ILogMessage>, ILogCommand
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected LogCommandBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected LogCommandBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected LogCommandBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected LogCommandBase()
            : base()
        {
        }

        #endregion Constrcutors (4)

        #region Methods (5)

        private LogCommandExecutionContext CreateBasicExecutionContext(ILogMessage msg)
        {
            var result = new LogCommandExecutionContext();
            result.Arguments = (this.GetExecutionArguments(msg) ?? new object[0]).AsArray();
            result.DoLogMessage = false;
            result.Command = this;
            result.Message = msg;
            result.MessageValueToLog = null;

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ILogCommand.Execute(ILogMessage)"/>
        public new ILogCommandExecutionResult Execute(ILogMessage orgMsg)
        {
            if (this.CanExecute(orgMsg) == false)
            {
                return null;
            }

            var result = new LogCommandExecutionResult();
            result.Command = this;
            result.Message = orgMsg;

            try
            {
                LogCommandExecutionContext ctx = this.CreateBasicExecutionContext(orgMsg);

                this.OnExecute(ctx);

                result.Errors = new Exception[0];
                result.DoLogMessage = ctx.DoLogMessage;
                result.MessageValueToLog = ctx.MessageValueToLog;
            }
            catch (Exception ex)
            {
                var aggEx = ex as AggregateException;
                if (aggEx == null)
                {
                    aggEx = new AggregateException(new Exception[] { ex });
                }

                result.Errors = aggEx.Flatten()
                                     .InnerExceptions
                                     .AsArray();

                result.DoLogMessage = false;
                result.MessageValueToLog = null;
            }

            return result;
        }

        /// <summary>
        /// Returns the list of arguments for an execution.
        /// </summary>
        /// <param name="msg">The log message from where to get the arguments from.</param>
        /// <returns>The list of arguments.</returns>
        protected virtual IEnumerable<object> GetExecutionArguments(ILogMessage msg)
        {
            return null;
        }

        /// <inheriteddoc />
        protected override sealed void OnExecute(ILogMessage param)
        {
            var ctx = this.CreateBasicExecutionContext(param);

            this.OnExecute(ctx);
        }

        /// <summary>
        /// The logic for <see cref="LogCommandBase.Execute(ILogMessage)" /> command.
        /// </summary>
        /// <param name="context">The execution context.</param>
        protected abstract void OnExecute(ILogCommandExecutionContext context);

        #endregion Methods
    }
}