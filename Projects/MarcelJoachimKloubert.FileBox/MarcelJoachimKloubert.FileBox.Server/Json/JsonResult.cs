// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox.Server.Json
{
    #region CLASS: JsonResult<TData>

    /// <summary>
    /// A simple object for a JSON result.
    /// </summary>
    /// <typeparam name="TData">Type of the data.</typeparam>
    public class JsonResult<TData>
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
        public TData data
        {
            get;
            set;
        }

        #endregion Properties
    }

    #endregion

    #region CLASS: JsonResult<TData>

    /// <summary>
    /// Simple implementation of <see cref="JsonResult{TData}" />.
    /// </summary>
    public sealed class JsonResult : JsonResult<object>
    {
    }

    #endregion
}