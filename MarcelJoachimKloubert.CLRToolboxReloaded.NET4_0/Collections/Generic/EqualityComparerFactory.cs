// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    /// <summary>
    /// A factory class for creating <see cref="IEqualityComparer{T}" /> instances.
    /// </summary>
    public static class EqualityComparerFactory
    {
        #region Methods (3)

        /// <summary>
        /// Creates an case insensitive <see cref="IEqualityComparer{T}" /> for <see cref="string" />.
        /// </summary>
        /// <param name="trim">Compare string trimmed or not.</param>
        /// <param name="emptyIsNull">Handle empty strings as <see langword="null" /> reference or not.</param>
        /// <returns>The created comparer.</returns>
        public static IEqualityComparer<string> CreateCaseInsensitiveStringComparer(bool trim = true,
                                                                                    bool emptyIsNull = true)
        {
            return new DelegateEqualityComparer<string>(equalsHandler:
                                                            (x, y) => ParseString(str: x,
                                                                                  trim: trim, emptyIsNull: emptyIsNull) ==
                                                                      ParseString(str: y,
                                                                                  trim: trim, emptyIsNull: emptyIsNull),
                                                        getHashCodeHandler:
                                                            (obj) =>
                                                            {
                                                                var str = ParseString(str: obj,
                                                                                      trim: trim, emptyIsNull: emptyIsNull);

                                                                return str != null ? str.GetHashCode() : 0;
                                                            });
        }

        /// <summary>
        /// Creates a case insensitive <see cref="IEqualityComparer{T}" /> for use in HTTP context.
        /// </summary>
        /// <returns>The created comparer.</returns>
        public static IEqualityComparer<string> CreateHttpKeyComparer()
        {
            return CreateCaseInsensitiveStringComparer(trim: true,
                                                       emptyIsNull: true);
        }

        private static string ParseString(string str, bool trim, bool emptyIsNull)
        {
            var result = str;

            if (result != null)
            {
                result = result.ToLower();

                if (trim)
                {
                    result = result.Trim();
                }

                if ((result == string.Empty) &&
                    emptyIsNull)
                {
                    result = null;
                }
            }

            return result;
        }

        #endregion Methods
    }
}