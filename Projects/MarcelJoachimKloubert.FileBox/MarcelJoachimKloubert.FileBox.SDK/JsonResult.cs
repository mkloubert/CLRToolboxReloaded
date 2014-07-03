// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Stores the data of a JSON result.
    /// </summary>
    public sealed class JsonResult
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
        /// Gets or sets the optional object (data) to serialize.
        /// </summary>
        public dynamic data
        {
            get;
            set;
        }

        #endregion Properties
    }
}