// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Windows.Data
{
    #region CLASS: ValueConverterBase<TInput, TOutput, TParam>

    /// <summary>
    /// A basic strong typed <see cref="IValueConverter" />.
    /// </summary>
    /// <typeparam name="TInput">The source / input data type.</typeparam>
    /// <typeparam name="TParam">The parameter data type.</typeparam>
    /// <typeparam name="TOutput">The destination / output data type.</typeparam>
    public abstract partial class ValueConverterBase<TInput, TOutput, TParam> : ObjectBase, IValueConverter
    {
        #region Fields (2)

        private readonly Func<TOutput, TParam, CultureInfo, TInput> _CONVERT_BACK_FUNC;
        private readonly Func<TInput, TParam, CultureInfo, TOutput> _CONVERT_FUNC;

        #endregion Fields (2)

        #region Constructors (4)

        /// <inheriteddoc />
        protected ValueConverterBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._CONVERT_FUNC = this.OnConvert_ThreadSafe;
                this._CONVERT_BACK_FUNC = this.OnConvertBack_ThreadSafe;
            }
            else
            {
                this._CONVERT_FUNC = this.OnConvert;
                this._CONVERT_BACK_FUNC = this.OnConvertBack;
            }
        }

        /// <inheriteddoc />
        protected ValueConverterBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected ValueConverterBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ValueConverterBase()
            : this(isSynchronized: false)
        {
        }

        #endregion Constructors (4)

        #region Methods (7)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IValueConverter.Convert(object, Type, object, CultureInfo)" />
        public TOutput Convert(TInput value, TParam parameter, CultureInfo culture)
        {
            return this._CONVERT_FUNC(value, parameter, culture);
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Convert(GlobalConverter.Current.ChangeType<TInput>(value),
                                GlobalConverter.Current.ChangeType<TParam>(parameter),
                                culture);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IValueConverter.ConvertBack(object, Type, object, CultureInfo)" />
        public TInput ConvertBack(TOutput value, TParam parameter, CultureInfo culture)
        {
            return this._CONVERT_BACK_FUNC(value, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ConvertBack(GlobalConverter.Current.ChangeType<TOutput>(value),
                                    GlobalConverter.Current.ChangeType<TParam>(parameter),
                                    culture);
        }

        /// <summary>
        /// Stores the logic for the <see cref="ValueConverterBase{TInput, TOutput, TParam}.Convert(TInput, TParam, CultureInfo)" /> method.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The underlying culture.</param>
        /// <returns>The output value.</returns>
        protected virtual TOutput OnConvert(TInput value, TParam parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private TOutput OnConvert_ThreadSafe(TInput value, TParam parameter, CultureInfo culture)
        {
            TOutput result;

            lock (this._SYNC)
            {
                result = this.OnConvert(value, parameter, culture);
            }

            return result;
        }

        /// <summary>
        /// Stores the logic for the <see cref="ValueConverterBase{TInput, TOutput, TParam}.ConvertBack(TOutput, TParam, CultureInfo)" /> method.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The underlying culture.</param>
        /// <returns>The output value.</returns>
        protected virtual TInput OnConvertBack(TOutput value, TParam parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private TInput OnConvertBack_ThreadSafe(TOutput value, TParam parameter, CultureInfo culture)
        {
            TInput result;

            lock (this._SYNC)
            {
                result = this.OnConvertBack(value, parameter, culture);
            }

            return result;
        }

        #endregion Methods
    }

    #endregion

    #region CLASS: ValueConverterBase<TInput, TOutput>

    /// <summary>
    /// A simple version of <see cref="ValueConverterBase{TInput, TOutput, TParam}" /> that handles a parameter
    /// object as parsed string.
    /// </summary>
    /// <typeparam name="TInput">The source / input data type.</typeparam>
    /// <typeparam name="TOutput">The destination / output data type.</typeparam>
    /// <remarks>
    /// The parameter is parsed to a non-null, trimmed and lower case <see cref="string" /> by default.
    /// </remarks>
    public abstract partial class ValueConverterBase<TInput, TOutput> : ValueConverterBase<TInput, TOutput, object>
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected ValueConverterBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ValueConverterBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected ValueConverterBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ValueConverterBase()
            : base()
        {
        }

        #endregion Constructors

        #region Methods (6)

        /// <inheriteddoc />
        protected override sealed TOutput OnConvert(TInput value, object parameter, CultureInfo culture)
        {
            return this.OnConvert(value,
                                  this.ParseConvertParameter(parameter, culture),
                                  culture);
        }

        /// <summary>
        /// The logic for <see cref="ValueConverterBase{TInput, TOutput}.OnConvert(TInput, object, CultureInfo)" /> method.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="parameter">The parameter as <see cref="String" />.</param>
        /// <param name="culture">The underlying culture.</param>
        /// <returns>The output value.</returns>
        protected virtual TOutput OnConvert(TInput value, string parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <inheriteddoc />
        protected override sealed TInput OnConvertBack(TOutput value, object parameter, CultureInfo culture)
        {
            return this.OnConvertBack(value,
                                      this.ParseConvertBackParameter(parameter, culture),
                                      culture);
        }

        /// <summary>
        /// The logic for <see cref="ValueConverterBase{TInput, TOutput}.OnConvertBack(TOutput, object, CultureInfo)" /> method.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="parameter">The parameter as <see cref="String" />.</param>
        /// <param name="culture">The underlying culture.</param>
        /// <returns>The output value.</returns>
        protected virtual TInput OnConvertBack(TOutput value, string parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses a parameter for <see cref="ValueConverterBase{TInput, TOutput}.OnConvert(TInput, string, CultureInfo)" /> method.
        /// </summary>
        /// <param name="input">The input parameter value.</param>
        /// <param name="culture">The underlying culture.</param>
        /// <returns>The parsed parameter value.</returns>
        protected virtual string ParseConvertParameter(object input, CultureInfo culture)
        {
            var result = input.AsString() ?? string.Empty;

            if (culture != null)
            {
                result = result.ToLower(culture);
            }
            else
            {
                result = result.ToLower();
            }

            return result.Trim();
        }

        /// <summary>
        /// Parses a parameter for <see cref="ValueConverterBase{TInput, TOutput}.OnConvertBack(TOutput, string, CultureInfo)" /> method.
        /// </summary>
        /// <param name="input">The input parameter value.</param>
        /// <param name="culture">The underlying culture.</param>
        /// <returns>The parsed parameter value.</returns>
        protected virtual string ParseConvertBackParameter(object input, CultureInfo culture)
        {
            return this.ParseConvertParameter(input, culture);
        }

        #endregion Methods
    }

    #endregion
}