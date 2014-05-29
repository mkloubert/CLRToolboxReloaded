// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define KNOWS_DBNULL
#define STRING_IS_CHAR_SEQUENCE
#endif

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;

namespace MarcelJoachimKloubert.CLRToolbox.Data.Conversion
{
    /// <summary>
    /// Common implementation of an <see cref="IConverter" /> object.
    /// </summary>
    public partial class CommonConverter : ConverterBase
    {
        #region Methods (4)

        private static void ConvertToString(ref object targetValue, IFormatProvider provider)
        {
            var handled = false;

#if !(PORTABLE || PROTABLE40)

            if (provider != null)
            {
                var conv = targetValue as global::System.IConvertible;
                if (conv != null)
                {
                    targetValue = conv.ToString(provider);
                    handled = true;
                }
            }

#endif

            if (handled == false)
            {
                var str = targetValue.AsString();

#if !(PORTABLE || PORTABLE40)

                if ((str != null) &&
                    (provider != null))
                {
                    str = str.ToString(provider);
                }

#endif

                targetValue = str;
            }
        }

        private static bool IsAssignableFrom(Type type, Type c)
        {
#if (PORTABLE || PORTABLE40)
            //TODO
            return false;
#else
            return type.IsAssignableFrom(c);
#endif
        }

        /// <inheriteddoc />
        protected override void OnChangeType(Type targetType, ref object targetValue, IFormatProvider provider)
        {
            ParseInputValueForChangeType(targetType, ref targetValue, provider);

            if (targetValue != null)
            {
                var valueType = targetValue.GetType();
                if (valueType.Equals(targetType) ||
                    IsAssignableFrom(targetType, valueType))
                {
                    // no need to convert
                    return;
                }
            }

            if (targetType.Equals(typeof(string)))
            {
                // force to convert to string

                ConvertToString(ref targetValue, provider);
                return;
            }

            if (targetType.Equals(typeof(global::System.Collections.Generic.IEnumerable<char>)))
            {
                // force to convert to char sequence

                object temp = targetValue;
                ConvertToString(ref temp, provider);

#if STRING_IS_CHAR_SEQUENCE
                targetValue = temp;
#else
                targetValue = (global::MarcelJoachimKloubert.CLRToolbox.CharSequence)temp.AsString();
#endif

                return;
            }

#if !(PORTABLE || PORTABLE40)

            if (targetValue == null)
            {
                if (targetType.IsValueType &&
                    (global::System.Nullable.GetUnderlyingType(targetType) == null)
                )
                {
                    // a (non-nullable) struct, so create instance by use the default parameter-less constructor
                    targetValue = global::System.Activator.CreateInstance(targetType);
                }

                return;
            }

#endif

            // use BCL logic
            targetValue = global::System.Convert.ChangeType(targetValue, targetType, provider);
        }

        private static void ParseInputValueForChangeType(Type targetType, ref object targetValue, IFormatProvider provider)
        {
#if KNOWS_DBNULL

            if (targetType.Equals(typeof(global::System.DBNull)) == false)
            {
                if (global::System.DBNull.Value.Equals(targetValue))
                {
                    targetValue = null;
                }
            }
            else
            {
                // target type is DBNull

                if (targetValue == null)
                {
                    targetValue = global::System.DBNull.Value;
                }
                else
                {
                    if (global::System.DBNull.Value.Equals(targetValue) == false)
                    {
                        throw new global::System.InvalidCastException();
                    }
                }
            }

#endif
        }

        #endregion Methods
    }
}