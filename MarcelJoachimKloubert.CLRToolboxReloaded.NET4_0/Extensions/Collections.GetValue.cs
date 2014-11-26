// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (2)

        /// <summary>
        /// Gets a value strong typed from a general dictionary by
        /// using instance from <see cref="GlobalConverter.Current" /> property.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="dict">The dictionary from where to get the value from.</param>
        /// <param name="key">The key.</param>
        /// <param name="provider">The optional format provider for the conversion process.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="key" /> was not found.
        /// </exception>
        public static TResult GetValue<TResult>(this IDictionary<string, object> dict,
                                                string key,
                                                IFormatProvider provider = null)
        {
            return GetValue<TResult>(dict: dict,
                                     key: key,
                                     converter: GlobalConverter.Current,
                                     provider: provider);
        }

        /// <summary>
        /// Gets a value strong typed from a general dictionary by
        /// using a custom converter.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="dict">The dictionary from where to get the value from.</param>
        /// <param name="key">The key.</param>
        /// <param name="provider">The optional format provider for the conversion process.</param>
        /// <returns>The value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> and/or <param name="converter" /> are <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="key" /> was not found.
        /// </exception>
        public static TResult GetValue<TResult>(this IDictionary<string, object> dict,
                                                string key,
                                                IConverter converter,
                                                IFormatProvider provider = null)
        {
            TResult result;
            if (TryGetValue<TResult>(dict: dict,
                                     key: key,
                                     value: out result,
                                     converter: converter,
                                     provider: provider) == false)
            {
                throw new InvalidOperationException(string.Format("'{0}' not found!",
                                                                  key));
            }

            return result;
        }

        #endregion Methods
    }
}