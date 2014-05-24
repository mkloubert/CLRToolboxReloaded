// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Checks if an object is NOT <see langword="null" /> and NOT DBNull.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns>Is NOT <see langword="null" /> / DBNull.</returns>
        public static bool IsNotNull<T>(this T obj) where T : class
        {
            return IsNull<T>(obj: obj) == false;
        }

        /// <summary>
        /// Checks if a nullable struct is <see langword="null" />.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <returns>Is NOT <see langword="null" /> / DBNull.</returns>
        public static bool IsNotNull<T>(this T? value) where T : struct
        {
            return IsNull<T>(value: value) == false;
        }

        #endregion Methods
    }
}