﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Windows.Data
{
    /// <summary>
    /// A <see cref="ValueConverterBase{TInput, TOutput}" /> that uses a generated list of parameters
    /// from a single string.
    /// </summary>
    /// <typeparam name="TInput">The input value.</typeparam>
    /// <typeparam name="TOutput">The output value.</typeparam>
    public abstract class MultiParamValueConverterBase<TInput, TOutput> : ValueConverterBase<TInput, TOutput>
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected MultiParamValueConverterBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected MultiParamValueConverterBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected MultiParamValueConverterBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected MultiParamValueConverterBase()
            : base()
        {
        }

        #endregion Constrcutors

        #region Methods (9)

        /// <inheriteddoc />
        protected sealed override TOutput OnConvert(TInput value, string parameter, CultureInfo culture)
        {
            IList<string> @params = new List<string>();
            this.GenerateParamListForConvert(parameter, ref @params);

            return this.OnConvert(value, @params, culture);
        }

        /// <inheriteddoc />
        protected virtual TOutput OnConvert(TInput value, IList<string> @params, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <inheriteddoc />
        protected sealed override TInput OnConvertBack(TOutput value, string parameter, CultureInfo culture)
        {
            IList<string> @params = new List<string>();
            this.GenerateParamListForConvertBack(parameter, ref @params);

            return this.OnConvertBack(value, @params, culture);
        }

        /// <inheriteddoc />
        protected virtual TInput OnConvertBack(TOutput value, IList<string> @params, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates the value for the second argument of the <see cref="MultiParamValueConverterBase{TInput, TOutput}.OnConvertBack(TOutput, IList{string}, CultureInfo)" />
        /// method.
        /// </summary>
        /// <param name="paramsAsString">The list with parameters.</param>
        /// <param name="params">
        /// The value for he second argument of the <see cref="MultiParamValueConverterBase{TInput, TOutput}.OnConvertBack(TOutput, IList{string}, CultureInfo)" />
        /// method.
        /// </param>
        protected virtual void GenerateParamListForConvert(string paramsAsString, ref IList<string> @params)
        {
            if (paramsAsString != null)
            {
                @params.AddRange(paramsAsString.Split(';')
                                               .Where(p => string.IsNullOrWhiteSpace(p) == false)
                                               .Select(p => p.Trim())
                                               .Distinct());
            }
            else
            {
                @params = null;
            }
        }

        /// <summary>
        /// Generates the value for the second argument of the <see cref="MultiParamValueConverterBase{TInput, TOutput}.OnConvertBack(TOutput, IList{string}, CultureInfo)" />
        /// method.
        /// </summary>
        /// <param name="paramsAsString">The list with parameters.</param>
        /// <param name="params">
        /// The value for he second argument of the <see cref="MultiParamValueConverterBase{TInput, TOutput}.OnConvertBack(TOutput, IList{string}, CultureInfo)" />
        /// method.
        /// </param>
        protected virtual void GenerateParamListForConvertBack(string paramsAsString, ref IList<string> @params)
        {
            this.GenerateParamListForConvert(paramsAsString, ref @params);
        }

        /// <summary>
        /// Returns a value by a possible list of parameter values.
        /// The last found parameter value "wins" where.
        /// </summary>
        /// <typeparam name="T">Type of the value that should be returned.</typeparam>
        /// <param name="params">
        /// The list of submitted parameter values.
        /// </param>
        /// <param name="valueProviders">
        /// The key/value pairs that define what logic should be called to generate the result value for a specific parameter value.
        /// The key of each entry defines the value that should be looked for in <paramref name="params" />.
        /// The value of each entry defines the logic that should be called if its key was found in <paramref name="params" />.
        /// </param>
        /// <returns>The returns value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueProviders" /> is <see langword="null" />.
        /// </exception>
        protected static T GetLastParamValue<T>(IList<string> @params,
                                                IEnumerable<KeyValuePair<string, Func<string, T>>> valueProviders)
        {
            return GetLastParamValue(@params,
                                     default(T),
                                     valueProviders);
        }

        /// <summary>
        /// Returns a value by a possible list of parameter values.
        /// The last found parameter value "wins" where.
        /// </summary>
        /// <typeparam name="T">Type of the value that should be returned.</typeparam>
        /// <param name="params">
        /// The list of submitted parameter values.
        /// </param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="valueProviders">
        /// The key/value pairs that define what logic should be called to generate the result value for a specific parameter value.
        /// The key of each entry defines the value that should be looked for in <paramref name="params" />.
        /// The value of each entry defines the logic that should be called if its key was found in <paramref name="params" />.
        /// </param>
        /// <returns>The returns value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="valueProviders" /> is <see langword="null" />.
        /// </exception>
        protected static T GetLastParamValue<T>(IList<string> @params,
                                                T defaultValue,
                                                IEnumerable<KeyValuePair<string, Func<string, T>>> valueProviders)
        {
            return GetLastParamValue(@params,
                                     (pl) => defaultValue,
                                     valueProviders);
        }

        /// <summary>
        /// Returns a value by a possible list of parameter values.
        /// The last found parameter value "wins" where.
        /// </summary>
        /// <typeparam name="T">Type of the value that should be returned.</typeparam>
        /// <param name="params">
        /// The list of submitted parameter values.
        /// </param>
        /// <param name="defaultValueProvider">
        /// The logic that is used to return the default value.
        /// The first parameter is the reference of <paramref name="params" />.
        /// </param>
        /// <param name="valueProviders">
        /// The key/value pairs that define what logic should be called to generate the result value for a specific parameter value.
        /// The key of each entry defines the value that should be looked for in <paramref name="params" />.
        /// The value of each entry defines the logic that should be called if its key was found in <paramref name="params" />.
        /// </param>
        /// <returns>The returns value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultValueProvider" /> and/or <paramref name="valueProviders" />
        /// are <see langword="null" />.
        /// </exception>
        protected static T GetLastParamValue<T>(IList<string> @params,
                                                Func<IList<string>, T> defaultValueProvider,
                                                IEnumerable<KeyValuePair<string, Func<string, T>>> valueProviders)
        {
            if (defaultValueProvider == null)
            {
                throw new ArgumentNullException("defaultValueProvider");
            }

            if (valueProviders == null)
            {
                throw new ArgumentNullException("valueProviders");
            }

            Func<string, T> func = null;
            string lastParam = null;
            if (@params != null)
            {
                var lastIndex = -1;
                foreach (var item in valueProviders)
                {
                    var index = @params.IndexOf(item.Key);
                    if (index > lastIndex)
                    {
                        func = item.Value;

                        lastParam = item.Key;
                        lastIndex = index;
                    }

                    if (lastIndex == (@params.Count - 1))
                    {
                        // end reached => nothing more to do
                        break;
                    }
                }
            }

            return func != null ? func(lastParam)
                                : defaultValueProvider(@params);
        }

        #endregion Methods
    }
}