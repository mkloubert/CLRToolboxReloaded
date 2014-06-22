// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define CAN_SERIALIZE
#endif

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Jobs
{
    #region ENUM: JobExceptionContext

    /// <summary>
    /// List of contextes where a job execution failed.
    /// </summary>
    public enum JobExceptionContext
    {
        /// <summary>
        /// In <see cref="IJob.Completed(IJobExecutionContext)" /> method.
        /// </summary>
        Completed,
        
        /// <summary>
        /// In <see cref="IJob.Error(IJobExecutionContext, Exception)" /> method.
        /// </summary>
        Error,

        /// <summary>
        /// In <see cref="IJob.Execute(IJobExecutionContext)" /> method.
        /// </summary>
        Execute,
    }

    #endregion ENUM: JobExceptionContext

    #region CLASS: JobException

    /// <summary>
    /// An exception for a job execution.
    /// </summary>
    public class JobException : Exception
    {
        #region Constructors (2)

#if CAN_SERIALIZE

        /// <inheriteddoc />
        protected JobException(global::System.Runtime.Serialization.SerializationInfo info,
                               global::System.Runtime.Serialization.StreamingContext context)
            : base(info: info,
                   context: context)
        {
        }

#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="JobException" /> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="context">The value for the <see cref="JobException.Context" /> property.</param>
        public JobException(Exception innerException, JobExceptionContext context)
            : base(message: innerException.Message,
                   innerException: innerException)
        {
            this.Context = context;
        }

        #endregion Constructors (2)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying context.
        /// </summary>
        public JobExceptionContext Context
        {
            get;
            private set;
        }

        #endregion Properties (1)
    }

    #endregion CLASS: JobException
}