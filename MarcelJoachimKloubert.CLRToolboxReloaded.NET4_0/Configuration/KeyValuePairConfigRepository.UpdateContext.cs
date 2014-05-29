// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Configuration
{
    partial class KeyValuePairConfigRepository
    {
        #region ENUM: UpdateContext

        /// <summary>
        /// List of update contextes.
        /// </summary>
        protected enum UpdateContext
        {
            /// <summary>
            /// Clear all values
            /// </summary>
            ClearAll,

            /// <summary>
            /// Delete a value
            /// </summary>
            DeleteValue,

            /// <summary>
            /// Set a value
            /// </summary>
            SetValue,
        }

        #endregion ENUM: UpdateContext
    }
}