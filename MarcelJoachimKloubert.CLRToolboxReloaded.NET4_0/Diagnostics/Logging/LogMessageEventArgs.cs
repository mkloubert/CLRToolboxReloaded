// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// Arguments for an event that handles a <see cref="ILogMessage" />.
    /// </summary>
    public class LogMessageEventArgs : EventArgs
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageEventArgs" /> class.
        /// </summary>
        /// <param name="msg">
        /// The value for the <see cref="LogMessageEventArgs.Message" /> property.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="msg" /> is <see langword="null" />.
        /// </exception>
        public LogMessageEventArgs(ILogMessage msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }

            this.Message = msg;
        }

        #endregion Constructors

        #region Properties (1)

        /// <summary>
        /// Gets the underlying log message.
        /// </summary>
        public ILogMessage Message
        {
            get;
            private set;
        }

        #endregion Properties
    }
}