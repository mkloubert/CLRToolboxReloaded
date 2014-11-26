// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (4)

        /// <summary>
        /// Tries to get a value strong typed from a general dictionary by
        /// using instance from <see cref="GlobalConverter.Current" /> property.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="dict">The dictionary from where to get the value from.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">
        /// The variable where to write the found value to.
        /// If not found the value of <paramref name="defValue" /> is set.
        /// </param>
        /// <param name="defValue">
        /// The default value if <paramref name="key" /> does not exist.
        /// </param>
        /// <param name="provider">The optional format provider for the conversion process.</param>
        /// <returns>Value was found or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> is <see langword="null" />.
        /// </exception>
        public static bool TryGetValue<TResult>(this IDictionary<string, object> dict,
                                                string key, out TResult value,
                                                TResult defValue = default(TResult),
                                                IFormatProvider provider = null)
        {
            return TryGetValue<TResult>(dict: dict,
                                        key: key, value: out value,
                                        converter: GlobalConverter.Current,
                                        defValue: defValue,
                                        provider: provider);
        }

        /// <summary>
        /// Tries to get a value strong typed from a general dictionary by
        /// using custom converter.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="dict">The dictionary from where to get the value from.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">
        /// The variable where to write the found value to.
        /// If not found the value of <paramref name="defValue" /> is set.
        /// </param>
        /// <param name="converter">The converter to use.</param>
        /// <param name="defValue">
        /// The default value if <paramref name="key" /> does not exist.
        /// </param>
        /// <param name="provider">The optional format provider for the conversion process.</param>
        /// <returns>Value was found or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> and/or <paramref name="converter" /> are <see langword="null" />.
        /// </exception>
        public static bool TryGetValue<TResult>(this IDictionary<string, object> dict,
                                                string key, out TResult value,
                                                IConverter converter,
                                                TResult defValue = default(TResult),
                                                IFormatProvider provider = null)
        {
            return TryGetValue<TResult>(dict: dict,
                                        key: key, value: out value,
                                        converter: converter,
                                        defValueProvider: (k, t) => defValue,
                                        provider: provider);
        }

        /// <summary>
        /// Tries to get a value strong typed from a general dictionary by
        /// using instance from <see cref="GlobalConverter.Current" /> property.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="dict">The dictionary from where to get the value from.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">
        /// The variable where to write the found value to.
        /// If not found the result of <paramref name="defValueProvider" /> is set.
        /// </param>
        /// <param name="defValueProvider">
        /// The provider that returns the default value if <paramref name="key" /> does not exist.
        /// </param>
        /// <param name="provider">The optional format provider for the conversion process.</param>
        /// <returns>Value was found or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" /> and/or <paramref name="defValueProvider" /> are <see langword="null" />.
        /// </exception>
        public static bool TryGetValue<TResult>(this IDictionary<string, object> dict,
                                                string key, out TResult value,
                                                Func<string, Type, object> defValueProvider,
                                                IFormatProvider provider = null)
        {
            return TryGetValue<TResult>(dict: dict,
                                        key: key, value: out value,
                                        converter: GlobalConverter.Current,
                                        defValueProvider: defValueProvider,
                                        provider: provider);
        }

        /// <summary>
        /// Tries to get a value strong typed from a general dictionary by
        /// using custom converter.
        /// </summary>
        /// <typeparam name="TResult">Result type.</typeparam>
        /// <param name="dict">The dictionary from where to get the value from.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">
        /// The variable where to write the found value to.
        /// If not found the result of <paramref name="defValueProvider" /> is set.
        /// </param>
        /// <param name="converter">The converter to use.</param>
        /// <param name="defValueProvider">
        /// The provider that returns the default value if <paramref name="key" /> does not exist.
        /// </param>
        /// <param name="provider">The optional format provider for the conversion process.</param>
        /// <returns>Value was found or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dict" />, <paramref name="converter" /> and/or <paramref name="defValueProvider" /> are <see langword="null" />.
        /// </exception>
        public static bool TryGetValue<TResult>(this IDictionary<string, object> dict,
                                                string key, out TResult value,
                                                IConverter converter,
                                                Func<string, Type, object> defValueProvider,
                                                IFormatProvider provider = null)
        {
            if (dict == null)
            {
                throw new ArgumentNullException("dict");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            if (defValueProvider == null)
            {
                throw new ArgumentNullException("defValueProvider");
            }

            bool result;
            object temp;
            if (dict.TryGetValue(key, out temp))
            {
                result = true;
            }
            else
            {
                temp = defValueProvider(key, typeof(TResult));
                result = false;
            }

            value = converter.ChangeType<TResult>(value: temp,
                                                  provider: provider);
            return result;
        }

        #endregion Methods
    }
}