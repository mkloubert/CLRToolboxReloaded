// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define KNOWS_DBNULL
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
        #region Methods (2)

        // Protected Methods (1) 

        /// <inheriteddoc />
        protected override void OnChangeType(Type targetType, ref object targetValue, IFormatProvider provider)
        {
            this.ParseInputValueForChangeType(targetType, ref targetValue, provider);

            if (targetValue != null)
            {
                Type valueType = targetValue.GetType();
                if (valueType.Equals(targetType) ||
                    IsAssignableFrom(targetType, valueType))
                {
                    // no need to convert
                    return;
                }
            }

            if (targetType.Equals(typeof(string)) ||
                targetType.Equals(typeof(global::System.Collections.Generic.IEnumerable<char>)))
            {
                // force to convert to string

                bool handled = false;

#if !(PORTABLE || PROTABLE40)

                if (provider != null)
                {
                    global::System.IConvertible conv = targetValue as global::System.IConvertible;
                    if (conv != null)
                    {
                        targetValue = conv.ToString(provider);
                        handled = true;
                    }
                }

#endif

                if (handled == false)
                {
                    string str = targetValue.AsString();

#if !(PORTABLE || PORTABLE40)

                    if ((str != null) &&
                        (provider != null))
                    {
                        str = str.ToString(provider);
                    }

#endif

                    targetValue = str;
                }

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

        // Private Methods (2) 

        private static bool IsAssignableFrom(Type type, Type c)
        {
#if (PORTABLE || PORTABLE40)
            //TODO
            return false;
#else
            return type.IsAssignableFrom(c);
#endif
        }

        private void ParseInputValueForChangeType(Type targetType, ref object targetValue, IFormatProvider provider)
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