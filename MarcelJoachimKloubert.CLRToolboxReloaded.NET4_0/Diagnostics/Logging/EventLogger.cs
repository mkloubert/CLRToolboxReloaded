// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// A logger that raises an event if <see cref="EventLogger.OnLog(ILogMessage, ref bool)" /> method is called.
    /// </summary>
    public class EventLogger : LoggerBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogger" /> class.
        /// </summary>
        public EventLogger()
            : base(synchronized: false)
        {
        }

        #endregion Constructors

        #region Delegates and events (1)

        /// <summary>
        /// Is invoked when an <see cref="ILogMessage" /> object has been arrived in
        /// <see cref="EventLogger.OnLog(ILogMessage, ref bool)" /> method.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> MessageReceived;

        #endregion Delegates and events

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnLog(ILogMessage msg, ref bool succeeded)
        {
            if (this.RaiseEventHandler(this.MessageReceived, new LogMessageEventArgs(msg)) == false)
            {
                succeeded = false;
            }
        }

        #endregion Methods
    }
}