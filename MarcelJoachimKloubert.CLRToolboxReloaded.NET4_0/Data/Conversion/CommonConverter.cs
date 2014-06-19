// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define CAN_GET_MEMBERS_FROM_TYPE
#define KNOWS_DBNULL
#define STRING_IS_CHAR_SEQUENCE
#endif

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
#if MONO_PORTABLE40 || !(PORTABLE || PORTABLE40 || PORTABLE45)
            return type.IsAssignableFrom(c);
#else
    #if PORTABLE45
            return type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
    #else
            return false;
    #endif
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

                var handled = false;

                // ConvertToAttribute
                {
                    var obj = targetValue;

                    var members = Enumerable.Empty<MemberInfo>();
#if CAN_GET_MEMBERS_FROM_TYPE

                    var memberBindFlags = global::System.Reflection.BindingFlags.Public |
                                          global::System.Reflection.BindingFlags.NonPublic |
                                          global::System.Reflection.BindingFlags.Instance |
                                          global::System.Reflection.BindingFlags.Static;

                    members = members.Concat(obj.GetType().GetFields(memberBindFlags))
                                     .Concat(obj.GetType().GetMethods(memberBindFlags))
                                     .Concat(obj.GetType().GetProperties(memberBindFlags));

#endif

                    var convertToMembers = members.Select(m =>
                        {
                            return new
                            {
                                Attributes = m.GetCustomAttributes(typeof(global::MarcelJoachimKloubert.CLRToolbox.Data.Conversion.ConvertToAttribute),
                                                                   true)
                                              .Cast<ConvertToAttribute>(),
                                Member = m,
                            };
                        }).Where(x =>
                        {
                            return x.Attributes
                                    .Any(a => targetType.Equals(a.TargetType) ||
                                              IsAssignableFrom(targetType, a.TargetType));
                        }).OrderBy(x =>
                        {
                            var sortValue = ulong.MaxValue;

                            if (x.Attributes.Any(a => targetType.Equals(a.TargetType)))
                            {
                                sortValue = 0;
                            }

                            return sortValue;
                        });

                    var converter = convertToMembers.FirstOrDefault();
                    if (converter != null)
                    {
                        var member = converter.Member;
                        handled = true;

                        if (member is MethodBase)
                        {
                            var method = (MethodBase)member;
                            object[] invokationParams;

                            var methodParams = method.GetParameters();
                            if (methodParams.Length < 1)
                            {
                                // no parameters
                                invokationParams = new object[0];
                            }
                            else
                            {
                                invokationParams = new object[]
                                {
                                    new ConvertToArgs()
                                    {
                                        CurrentValue = targetValue,
                                        FormatProvider = provider,
                                        TargetType = targetType,
                                    },
                                };
                            }

                            targetValue = method.Invoke(method.IsStatic == false ? obj : null,
                                                        invokationParams);
                        }
                        else if (member is PropertyInfo)
                        {
                            var property = (PropertyInfo)member;

                            targetValue = property.GetValue(obj, null);
                        }
                        else if (member is FieldInfo)
                        {
                            var field = (FieldInfo)member;

                            targetValue = field.GetValue(obj);
                        }
                        else
                        {
                            handled = false;
                        }

                        if (handled)
                        {
                            this.OnChangeType(targetType: targetType,
                                              targetValue: ref targetValue,
                                              provider: provider);
                        }
                    }
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

                var temp = targetValue;
                ConvertToString(ref temp, provider);

#if STRING_IS_CHAR_SEQUENCE
                targetValue = temp;
#else
                targetValue = (global::MarcelJoachimKloubert.CLRToolbox.CharSequence)temp.AsString();
#endif

                return;
            }

            if (targetType.Equals(typeof(global::MarcelJoachimKloubert.CLRToolbox.CharSequence)))
            {
                var temp = targetValue;
                ConvertToString(ref temp, provider);

                targetValue = (CharSequence)temp.AsString();
                return;
            }

#if !(PORTABLE || PORTABLE40)

            if (targetValue == null)
            {
                if (targetType.IsValueType &&
                    (global::System.Nullable.GetUnderlyingType(targetType) == null))
                {
                    // a (non-nullable) struct, so create instance by use the default parameter-less constructor
                    targetValue = global::System.Activator.CreateInstance(targetType);
                }

                return;
            }

            if (targetType.IsValueType)
            {
                var underlyingType = global::System.Nullable.GetUnderlyingType(targetType);

                if (targetValue != null)
                {
                    if (underlyingType != null)
                    {
                        // try again by converting to underlying nullable struct type

                        this.OnChangeType(targetType: underlyingType,
                                          targetValue: ref targetValue,
                                          provider: provider);
                        return;
                    }
                }
                else
                {
                    if (underlyingType == null)
                    {
                        // a (non-nullable) struct, so create instance by use the default parameter-less constructor

                        targetValue = global::System.Activator.CreateInstance(targetType);
                        return;
                    }
                }
            }

#endif

            // use BCL logic
            {
                if (targetValue is IEnumerable<char>)
                {
                    // keep sure to have a real string
                    // before use 'System.Convert.ChangeType()' method
                    targetValue = targetValue.AsString();
                }

                targetValue = global::System.Convert.ChangeType(value: targetValue,
                                                                conversionType: targetType,
                                                                provider: provider);
            }
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