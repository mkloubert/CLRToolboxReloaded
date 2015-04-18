// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define KNOWS_PROPERTY_CHANGING
#endif

#if (WINRT)
#define GETTER_AND_SETTER_FROM_PROPERTY
#endif

#if !(NET40 || MONO40 || PORTABLE || PORTABLE40)
#define KNOWS_CALLER_MEMBER_NAME
#endif

#if (PORTABLE45)
#define KNOWS_RUNTIME_REFLECTION_EXTENSIONS
#endif

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using MarcelJoachimKloubert.CLRToolbox.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.ComponentModel
{
    /// <summary>
    /// A basic implementation of <see cref="INotifiable" /> interface.
    /// </summary>
    public abstract partial class NotifiableBase : ObjectBase, INotifiable
    {
        #region Fields (1)

        private readonly IDictionary<string, object> _PROPERTIES;

        #endregion Fields (1)

        #region Constructors (4)

        /// <inheriteddoc />
        protected NotifiableBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            this._PROPERTIES = this.CreatePropertyStorage() ?? new Dictionary<string, object>();
        }

        /// <inheriteddoc />
        protected NotifiableBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected NotifiableBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected NotifiableBase()
            : this(isSynchronized: false)
        {
        }
        
        #endregion Constructors (4)

        #region Delegates and Events (2)

        /// <inheriteddoc />
        public event PropertyChangedEventHandler PropertyChanged;

#if KNOWS_PROPERTY_CHANGING

        /// <inheriteddoc />
        public event global::System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

#endif

        #endregion Delegates and Events (2)

        #region Properties (1)

        /// <inheriteddoc />
        public override object Tag
        {
            get { return this.Get(() => this.Tag); }

            set { this.Set(value, () => this.Tag); }
        }

        #endregion Properties (1)

        #region Methods (12)

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
            return this.Get<T>(propertyName: GetPropertyName<T>(expr));
        }

        /// <summary>
        /// Returns the value of a property.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        /// <remarks>If property value does not exist, the default value of the target type will be returned.</remarks>
        protected T Get<T>(
#if KNOWS_CALLER_MEMBER_NAME
                           [global::System.Runtime.CompilerServices.CallerMemberName]
                           string propertyName = null
#else
                           string propertyName
#endif
)
        {
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
                    PropertyName = propertyName,
                });
        }

        /// <summary>
        /// Returns all possible members of that object for notification.
        /// </summary>
        /// <returns>The list of notifiable members of that object.</returns>
        protected virtual IEnumerable<MemberInfo> GetPossibleNotificationMembers()
        {
            var result = Enumerable.Empty<MemberInfo>();
#if KNOWS_RUNTIME_REFLECTION_EXTENSIONS
            result = result.Concat(this.GetType().GetRuntimeFields())
                           .Concat(this.GetType().GetRuntimeMethods())
                           .Concat(this.GetType().GetRuntimeProperties());
#else
            var memberBindFlags = global::System.Reflection.BindingFlags.Public |
                                  global::System.Reflection.BindingFlags.NonPublic |
                                  global::System.Reflection.BindingFlags.Instance |
                                  global::System.Reflection.BindingFlags.Static;

            result = result.Concat(this.GetType().GetFields(memberBindFlags))
                           .Concat(this.GetType().GetMethods(memberBindFlags))
                           .Concat(this.GetType().GetProperties(memberBindFlags));
#endif

            return result;
        }

        /// <summary>
        /// Returns all possible properties of that object for notification.
        /// </summary>
        /// <returns>The list of notifiable properties of that object.</returns>
        protected virtual IEnumerable<PropertyInfo> GetPossibleNotificationProperties()
        {
#if KNOWS_RUNTIME_REFLECTION_EXTENSIONS
            return this.GetType()
                       .GetRuntimeProperties()
                       .Where(p => ReflectionHelper.IsStatic(p) == false);
#else
            return this.GetType()
                       .GetProperties(global::System.Reflection.BindingFlags.Public |
                                      global::System.Reflection.BindingFlags.NonPublic |
                                      global::System.Reflection.BindingFlags.Instance);
#endif
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
            var properties = this.GetPossibleNotificationProperties();

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
                    ctx.State.Object
                             .OnPropertyChanged(ctx.Item.Name);
                }, actionState: new
                {
                    Object = this,
                });
        }

        private void Handle_ReceiveValueFrom<T>(string nameOfSender, bool hasBeenNotified, T newValue, object oldValue)
        {
            var members = this.GetPossibleNotificationMembers();

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
                    var m = ctx.Item;

                    Func<bool, object> getMemberObj = (isStatic) => isStatic == false ? ctx.State.Sender : null;

                    if (m is FieldInfo)
                    {
                        var field = (FieldInfo)m;

                        field.SetValue(getMemberObj(field.IsStatic),
                                       ctx.State.NewValue);
                    }
                    else if (m is PropertyInfo)
                    {
                        var property = (PropertyInfo)m;

                        MethodInfo setter;
#if GETTER_AND_SETTER_FROM_PROPERTY
                        setter = property.SetMethod;
#else
                        setter = property.GetSetMethod(nonPublic: false) ?? property.GetSetMethod(nonPublic: true);
#endif

                        property.SetValue(getMemberObj(setter.IsStatic),
                                          ctx.State.NewValue,
                                          null);
                    }
                    else if (m is MethodBase)
                    {
                        var method = (MethodBase)m;
                        object[] invokationParams = null;

                        var methodParams = method.GetParameters();
                        if (methodParams.Length > 0)
                        {
                            invokationParams = new object[]
                            {
                                new ReceiveValueFromArgs()
                                {
                                    NewValue = ctx.State.NewValue,
                                    OldValue = ctx.State.OldValue,
                                    Sender = ctx.State.Sender,
                                    SenderName = ctx.State.SenderName,
                                    SenderType = 16,    // s. MemberTypes.Property
                                    TargetType = ctx.State.ValueType,
                                },
                            };
                        }

                        method.Invoke(getMemberObj(method.IsStatic),
                                      invokationParams);
                    }
                }, actionState: new
                {
                    NewValue = newValue,
                    OldValue = oldValue,
                    Sender = this,
                    SenderName = nameOfSender,
                    ValueType = typeof(T),
                });
        }

        /// <summary>
        /// Raises the <see cref="NotifiableBase.PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has been changed.</param>
        /// <returns>Event was raised or not.</returns>
        protected bool OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
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
        public bool OnPropertyChanging(string propertyName)
        {
            var handler = this.PropertyChanging;
            if (handler != null)
            {
                handler(this, new global::System.ComponentModel.PropertyChangingEventArgs(propertyName));
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
                               propertyName: GetPropertyName<T>(expr));
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
                              string propertyName = null
#else
                              string propertyName
#endif
)
        {
            object oldValue = null;
            var result = this.InvokeThreadSafe((obj, state) =>
                {
                    var no = (NotifiableBase)obj;

                    var areDifferent = true;

                    if (no._PROPERTIES.TryGetValue(state.PropertyName, out oldValue))
                    {
                        if (object.Equals(value, oldValue))
                        {
                            areDifferent = false;
                        }
                    }

                    if (areDifferent)
                    {
#if KNOWS_PROPERTY_CHANGING
                        no.OnPropertyChanging(state.PropertyName);
#endif
                        no._PROPERTIES[state.PropertyName] = value;
                        no.OnPropertyChanged(state.PropertyName);
                    }

                    return areDifferent;
                }, funcState: new
                {
                    PropertyName = propertyName,
                });

            try
            {
                this.Handle_ReceiveValueFrom<T>(propertyName, result, value, oldValue);
            }
            finally
            {
                this.Handle_ReceiveNotificationFrom(propertyName, result);
            }

            return result;
        }

        #endregion Methods (12)
    }
}