﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Execution
{
    /// <summary>
    /// Stores the event arguments for an event that reports an execution error
    /// with a parameter.
    /// </summary>
    /// <typeparam name="TParam">Type of the parameter.</typeparam>
    public class ExecutionErrorEventArgs<TParam> : EventArgs
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionErrorEventArgs{TParam}"/> class.
        /// </summary>
        /// <param name="param">
        /// The value for <see cref="ExecutionErrorEventArgs{TParam}.Parameter" /> property.
        /// </param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exception" /> is <see langword="null" />.
        /// </exception>
        public ExecutionErrorEventArgs(TParam param, Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            this.Parameter = param;
            this.Exception = exception;
        }

        #endregion Constructors

        #region Properties (2)

        /// <summary>
        /// Gets the underlying exception.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the underlying parameter.
        /// </summary>
        public TParam Parameter
        {
            get;
            private set;
        }

        #endregion Properties
    }
}