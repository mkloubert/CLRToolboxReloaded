// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Serialization.Json
{
    #region CLASS: JsonResult<T>

    /// <summary>
    /// Describes a simple result object that can be (de)serialized via JSON.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="JsonResult{T}.tag" /> property.</typeparam>
    public partial class JsonResult<T>
    {
        #region Properties (3)

        /// <summary>
        /// Gets or sets the result code.
        /// </summary>
        public int? code
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the result message.
        /// </summary>
        public string msg
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the optional object to serialize.
        /// </summary>
        public T tag
        {
            get;
            set;
        }

        #endregion CLASS: JsonResult<T>
    }

    #endregion CLASS: JsonResult<T>

    #region CLASS: JsonResult

    /// <summary>
    /// Describes a simple result object that can be (de)serialized via JSON.
    /// </summary>
    public sealed partial class JsonResult : JsonResult<object>
    {
    }

    #endregion CLASS: JsonResult
}