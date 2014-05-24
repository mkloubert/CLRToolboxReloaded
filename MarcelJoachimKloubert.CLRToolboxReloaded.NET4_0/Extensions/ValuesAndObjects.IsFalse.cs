// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Checks if a boolean value has the value <see langword="false" />.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>Is <see langword="false" /> or not.</returns>
        public static bool IsFalse(this bool? value)
        {
            return value == false;
        }

        #endregion Methods
    }
}