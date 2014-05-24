// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define KNOWS_DBNULL
#endif

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Checks if an object is <see langword="null" /> or DBNull.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns>Is <see langword="null" /> / DBNull or not.</returns>
        public static bool IsNull<T>(this T obj) where T : class
        {
            if (obj == null)
            {
                return true;
            }

#if KNOWS_DBNULL

            if (global::System.DBNull.Value.Equals(obj))
            {
                return true;
            }

#endif

            return false;
        }

        /// <summary>
        /// Checks if a nullable struct is <see langword="null" />.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <returns>Is <see langword="null" /> or not.</returns>
        public static bool IsNull<T>(this T? value) where T : struct
        {
            return value.HasValue == false;
        }

        #endregion Methods
    }
}