// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define CAN_GET_MEMBERS_FROM_TYPE
#define CAN_GET_PROPERTIES_FROM_TYPE
#define KNOWS_PROPERTY_CHANGING
#endif

#if !(NET40 || MONO40 || PORTABLE || PORTABLE40)
#define KNOWS_CALLER_MEMBER_NAME
#endif

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.ComponentModel
{
    #region CLASS: NotifiableBase

    /// <summary>
    /// A basic implementation of <see cref="INotifiable" /> interface.
    /// </summary>
    public abstract class NotifiableBase : ObjectBase, INotifiable
    {
        #region Fields (1)

        private readonly IDictionary<string, object> _PROPERTIES;

        #endregion

        #region Constrcutors (4)

        /// <inheriteddoc />
        protected NotifiableBase(bool synchronized, object sync)
            : base(synchronized: synchronized,
                   sync: sync)
        {
            this._PROPERTIES = this.CreatePropertyStorage() ?? new Dictionary<string, object>();
        }

        /// <inheriteddoc />
        protected NotifiableBase(bool synchronized)
            : this(synchronized: synchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected NotifiableBase(object sync)
            : this(synchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected NotifiableBase()
            : this(synchronized: false)
        {
        }

        #endregion Constrcutors (4)

        #region Delegates and Events (2)

        /// <inheriteddoc />
        public event PropertyChangedEventHandler PropertyChanged;

#if KNOWS_PROPERTY_CHANGING

        /// <inheriteddoc />
        public event global::System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

#endif

        #endregion Delegates and Events (2)

        #region Methods (10)

        /// <summary>
        /// Creates the dictionary that stores all properties and their values.
        /// </summary>
        /// <returns>The property storage.</returns>
        protected virtual IDictionary<string, object> CreatePropertyStorage()
        {
            // create a default instance
            return null;
        }

        /// <summary>
        /// Returns the value of a property.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="expr">The name of the property as lambda expression.</param>
        /// <returns>The value of the property.</returns>
        /// <remarks>If property value does not exist, the default value of the target type will be returned.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expr" /> is <see langword="null" />.
        /// </exception>
        protected T Get<T>(Expression<Func<T>> expr)
        {
            return this.Get<T>(propertyName: GetPropertyName<T>(expr).AsChars());
        }

        /// <summary>
        /// Returns the value of a property.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        /// <remarks>If property value does not exist, the default value of the target type will be returned.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="propertyName" /> is invalid.
        /// </exception>
        protected T Get<T>(
#if KNOWS_CALLER_MEMBER_NAME
                           [global::System.Runtime.CompilerServices.CallerMemberName]
                           IEnumerable<char> propertyName = null
#else
                           IEnumerable<char> propertyName
#endif
        )
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            string pn = propertyName.AsString().Trim();
            if (pn == string.Empty)
            {
                throw new ArgumentException("propertyName");
            }

            return this.InvokeThreadSafe((obj, state) =>
                {
                    var no = (NotifiableBase)obj;

                    object temp;
                    if (no._PROPERTIES.TryGetValue(state.PropertyName, out temp) == false)
                    {
                        // get default value
                        temp = no.GetPropertyDefaultValue<T>(state.PropertyName);
                    }

                    return GlobalConverter.Current
                                          .ChangeType<T>(temp);
                }, funcState: new
                {
                    PropertyName = pn,
                });
        }

        /// <summary>
        /// Returns the default value for a property.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The default value.</returns>
        protected virtual object GetPropertyDefaultValue<T>(string propertyName)
        {
            return default(T);
        }

        private static string GetPropertyName<T>(Expression<Func<T>> expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException("expr");
            }

            var memberExpr = expr.Body as MemberExpression;
            if (memberExpr == null)
            {
                throw new ArgumentException("expr.Body");
            }

            var property = memberExpr.Member as PropertyInfo;
            if (property == null)
            {
                throw new InvalidCastException("expr.Body.Member");
            }

            return property.Name;
        }

        private void Handle_ReceiveNotificationFrom(string nameOfSender, bool hasBeenNotified)
        {
            IEnumerable<PropertyInfo> properties;
#if CAN_GET_PROPERTIES_FROM_TYPE
            properties = this.GetType()
                             .GetProperties(global::System.Reflection.BindingFlags.Public |
                                            global::System.Reflection.BindingFlags.NonPublic |
                                            global::System.Reflection.BindingFlags.Instance);
#else
            properties = global::System.Linq.Enumerable.Empty<PropertyInfo>();
#endif

            var receiveFromProperties =
                properties.Where(p =>
                                 {
                                     var attribs = p.GetCustomAttributes(typeof(global::MarcelJoachimKloubert.CLRToolbox.ComponentModel.ReceiveNotificationFromAttribute),
                                                                         true)
                                                     .Cast<ReceiveNotificationFromAttribute>();

                                     return attribs.Where(a =>
                                         {
                                             if (a.SenderName != nameOfSender)
                                             {
                                                 return false;
                                             }

                                             var aOpts = a.Options;

                                             if (aOpts.HasFlag(ReceiveNotificationFromOptions.Default))
                                             {
                                                 aOpts = ReceiveNotificationFromOptions.IfDifferent;
                                             }

                                             if (aOpts.HasFlag(ReceiveNotificationFromOptions.IfEqual))
                                             {
                                                 // 'has been notified' means: different values
                                                 if (hasBeenNotified == false)
                                                 {
                                                     return true;
                                                 }
                                             }

                                             if (aOpts.HasFlag(ReceiveNotificationFromOptions.IfDifferent))
                                             {
                                                 // 'has been notified' means: different values
                                                 if (hasBeenNotified)
                                                 {
                                                     return true;
                                                 }
                                             }

                                             return false;
                                         }).Any();
                                 });

            receiveFromProperties.ForAll((ctx) =>
                {
#if KNOWS_PROPERTY_CHANGING
                    ctx.State.Object
                             .OnPropertyChanging(ctx.Item.Name
                                                         .AsChars());
#endif
                    ctx.State.Object
                             .OnPropertyChanged(ctx.Item.Name
                                                        .AsChars());
                }, actionState: new
                {
                    Object = this,
                });
        }

        private void Handle_ReceiveValueFrom<T>(string nameOfSender, bool hasBeenNotified, T newValue, object oldValue)
        {
            IEnumerable<MemberInfo> members = Enumerable.Empty<MemberInfo>();
#if CAN_GET_MEMBERS_FROM_TYPE
            var memberBindFlags = global::System.Reflection.BindingFlags.Public |
                                  global::System.Reflection.BindingFlags.NonPublic |
                                  global::System.Reflection.BindingFlags.Instance |
                                  global::System.Reflection.BindingFlags.Static;

            members = members.Concat(this.GetType().GetFields(memberBindFlags))
                             .Concat(this.GetType().GetMethods(memberBindFlags))
                             .Concat(this.GetType().GetProperties(memberBindFlags));
#endif

            var receiveFromMembers = members.Where(m =>
                {
                    var attribs = m.GetCustomAttributes(typeof(global::MarcelJoachimKloubert.CLRToolbox.ComponentModel.ReceiveValueFromAttribute),
                                                               true)
                                   .Cast<ReceiveValueFromAttribute>();

                    return attribs.Where(a =>
                        {
                            if (a.SenderName != nameOfSender)
                            {
                                return false;
                            }

                            var aOpts = a.Options;

                            if (aOpts.HasFlag(ReceiveValueFromOptions.Default))
                            {
                                aOpts = ReceiveValueFromOptions.IfDifferent;
                            }

                            if (aOpts.HasFlag(ReceiveValueFromOptions.IfEqual))
                            {
                                // 'has been notified' means: different values
                                if (hasBeenNotified == false)
                                {
                                    return true;
                                }
                            }

                            if (aOpts.HasFlag(ReceiveValueFromOptions.IfDifferent))
                            {
                                // 'has been notified' means: different values
                                if (hasBeenNotified)
                                {
                                    return true;
                                }
                            }

                            return false;
                        }).Any();
                });

            receiveFromMembers.ForAll((ctx) =>
                {
                    MemberInfo m = ctx.Item;

                    if (m is FieldInfo)
                    {
                        FieldInfo field = (FieldInfo)m;

                        field.SetValue(this, newValue);
                    }
                    else if (m is MethodBase)
                    {
                        MethodBase method = (MethodBase)m;
                        object[] invokationParams;

                        ParameterInfo[] methodParams = method.GetParameters();
                        if (methodParams.Length < 1)
                        {
                            // no parameters
                            invokationParams = new object[0];
                        }
                        else if (methodParams.Length == 1)
                        {
                            // (T newValue)
                            invokationParams = new object[] { newValue };
                        }
                        else if (methodParams.Length == 2)
                        {
                            // (T newValue, T oldValue)
                            invokationParams = new object[] { newValue, oldValue };
                        }
                        else if (methodParams.Length == 3)
                        {
                            // (T newValue, T oldValue, string senderName)
                            invokationParams = new object[] { newValue, oldValue, nameOfSender };
                        }
                        else if (methodParams.Length == 4)
                        {
                            // (T newValue, T oldValue, string senderName, NotificationObjectBase obj)
                            invokationParams = new object[] { newValue, oldValue, nameOfSender, this };
                        }
                        else if (methodParams.Length == 5)
                        {
                            // (T newValue, T oldValue, string senderName, NotificationObjectBase obj, Type targetType)
                            invokationParams = new object[] { newValue, oldValue, nameOfSender, this, ctx.State.ValueType };
                        }
                        else
                        {
                            // (T newValue, T oldValue, string senderName, NotificationObjectBase obj, Type targetType, MemberTypes senderType)

                            invokationParams = new object[]
                            {
                                newValue,
                                oldValue,
                                nameOfSender,
                                this,
                                ctx.State.ValueType,
#if CAN_GET_MEMBERS_FROM_TYPE
                                global::System.Reflection.MemberTypes.Property,
#else
                                16,
#endif                          
                            };
                        }

                        method.Invoke(this,
                                        invokationParams);
                    }
                }, actionState: new
                {
                    ValueType = typeof(T),
                });
        }

        /// <summary>
        /// Raises the <see cref="NotifiableBase.PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has been changed.</param>
        /// <returns>Event was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName" /> is <see langword="null" />.
        /// </exception>
        protected bool OnPropertyChanged(IEnumerable<char> propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName.AsString()));
                return true;
            }

            return false;
        }

#if KNOWS_PROPERTY_CHANGING

        /// <summary>
        /// Raises the <see cref="NotifiableBase.PropertyChanging" /> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that is changing.</param>
        /// <returns>Event was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName" /> is <see langword="null" />.
        /// </exception>
        public bool OnPropertyChanging(global::System.Collections.Generic.IEnumerable<char> propertyName)
        {
            if (propertyName == null)
            {
                throw new global::System.ArgumentNullException("propertyName");
            }

            var handler = this.PropertyChanging;
            if (handler != null)
            {
                handler(this, new global::System.ComponentModel.PropertyChangingEventArgs(propertyName.AsString()));
                return true;
            }

            return false;
        }

#endif

        /// <summary>
        /// Sets a new value for a property.
        /// </summary>
        /// <typeparam name="T">Type of new value.</typeparam>
        /// <param name="value">The new value to set.</param>
        /// <param name="expr">The name of the property as lambda expression.</param>
        /// <returns>Values are different or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expr" /> is <see langword="null" />.
        /// </exception>
        protected bool Set<T>(T value, Expression<Func<T>> expr)
        {
            return this.Set<T>(value: value,
                               propertyName: GetPropertyName<T>(expr).AsChars());
        }

        /// <summary>
        /// Sets a new value for a property.
        /// </summary>
        /// <typeparam name="T">Type of new value.</typeparam>
        /// <param name="value">The new value to set.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="propertyName" /> is <see langword="null" />.
        /// <returns>Values are different or not.</returns>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="propertyName" /> is invalid.
        /// </exception>
        protected bool Set<T>(T value,
#if KNOWS_CALLER_MEMBER_NAME
                              [global::System.Runtime.CompilerServices.CallerMemberName]
                              IEnumerable<char> propertyName = null
#else
                              IEnumerable<char> propertyName
#endif
        )
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException("propertyName");
            }

            string pn = propertyName.AsString().Trim();
            if (pn == string.Empty)
            {
                throw new ArgumentException("propertyName");
            }

            object oldValue = null;
            var result = this.InvokeThreadSafe((obj, state) =>
                {
                    var no = (NotifiableBase)obj;
                    var propertyValues = no._PROPERTIES;

                    var areDifferent = true;

                    if (propertyValues.TryGetValue(pn, out oldValue))
                    {
                        if (object.Equals(value, oldValue))
                        {
                            areDifferent = false;
                        }
                    }

                    if (areDifferent)
                    {
#if KNOWS_PROPERTY_CHANGING
                        this.OnPropertyChanging(pn.AsChars());
#endif
                        propertyValues[pn] = value;
                        no.OnPropertyChanged(pn.AsChars());
                    }

                    return areDifferent;
                }, funcState: new
                {
                    PropertyName = pn,
                });

            this.Handle_ReceiveValueFrom<T>(pn, result, value, oldValue);
            this.Handle_ReceiveNotificationFrom(pn, result);

            return result;
        }

        #endregion Methods (10)
    }

    #endregion
}