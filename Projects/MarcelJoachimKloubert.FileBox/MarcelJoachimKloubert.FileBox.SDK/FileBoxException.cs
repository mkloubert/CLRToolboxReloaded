// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Runtime.Serialization;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// A file box exception.
    /// </summary>
    public class FileBoxException : Exception
    {
        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBoxException" /> class.
        /// </summary>
        public FileBoxException(JsonResult result)
            : base(message: result != null ? result.msg : null)
        {
            if (result == null)
            {
                return;
            }

            this.Code = result.code;
            this.ErrorData = result.msg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBoxException" /> class.
        /// </summary>
        /// <param name="innerException">The (optional) inner exception.</param>
        public FileBoxException(Exception innerException)
            : base(message: innerException != null ? innerException.Message : null,
                   innerException: innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBoxException" /> class.
        /// </summary>
        /// <param name="code">The value for the <see cref="FileBoxException.Code" /> property.</param>
        /// <param name="message">The (optional) exception message.</param>
        /// <param name="errorData">The value for the <see cref="FileBoxException.ErrorData" /> property.</param>
        public FileBoxException(int? code, string message, object errorData)
            : base(message: message)
        {
            this.Code = code;
            this.ErrorData = errorData;
        }

        /// <inheriteddoc />
        protected FileBoxException(SerializationInfo info, StreamingContext ctx)
            : base(info, ctx)
        {
        }

        #endregion Constructors (4)

        #region Properties (2)

        /// <summary>
        /// Gets the result code.
        /// </summary>
        public int? Code
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the underlying data.
        /// </summary>
        public dynamic ErrorData
        {
            get;
            private set;
        }

        #endregion Properties (2)
    }
}