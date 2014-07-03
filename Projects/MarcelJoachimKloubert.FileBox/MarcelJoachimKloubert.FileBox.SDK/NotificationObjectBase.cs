// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// A basic object that notifies on property changes.
    /// </summary>
    public abstract class NotificationObjectBase : ObjectBase, INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Constrcutors (2)

        /// <inheriteddoc />
        protected NotificationObjectBase()
            : base()
        {
        }

        /// <inheriteddoc />
        protected NotificationObjectBase(object sync)
            : base(sync: sync)
        {
        }

        #endregion Constrcutors (2)

        #region Events (2)

        /// <inheriteddoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheriteddoc />
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion Events (2)

        #region Methods (6)

        private static string GetPropertyName<T>(Expression<Func<T>> expr)
        {
            if (expr == null)
            {
                throw new ArgumentNullException("expr");
            }

            var memberExpr = expr.Body as MemberExpression;
            if (memberExpr == null)
            {
                throw new ArgumentException("expr");
            }

            var property = memberExpr.Member as PropertyInfo;
            if (property == null)
            {
                throw new InvalidCastException();
            }

            return property.Name;
        }

        /// <summary>
        /// Raises the <see cref="NotifiableBase.PropertyChanging" /> and <see cref="NotifiableBase.PropertyChanged" />
        /// events and updates the underlying field if old and new object are NOT equal.
        /// </summary>
        /// <typeparam name="T">Type of the property / field.</typeparam>
        /// <param name="expr">The expression that contains the property name.</param>
        /// <param name="field">The reference to the underlying field.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>
        /// <paramref name="field" /> and <paramref name="newValue" /> are different or not.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The body of <paramref name="expr" /> is NO <see cref="MemberExpression" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expr" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The body of <paramref name="expr" /> does not represent an expression that contains a property.
        /// </exception>
        protected bool OnPropertyChange<T>(Expression<Func<T>> expr, ref T field, T newValue)
        {
            var propertyName = GetPropertyName<T>(expr: expr);

            if (object.Equals(field, newValue) == false)
            {
                this.OnPropertyChanging(propertyName: propertyName);
                field = newValue;
                this.OnPropertyChanged(propertyName: propertyName);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="NotifiableBase.PropertyChanged" /> event.
        /// </summary>
        /// <param name="expr">The expression that contains the property name.</param>
        /// <exception cref="ArgumentException">
        /// The body of <paramref name="expr" /> is NO <see cref="MemberExpression" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expr" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The body of <paramref name="expr" /> does not represent an expression that contains a property.
        /// </exception>
        protected bool OnPropertyChanged<T>(Expression<Func<T>> expr)
        {
            return this.OnPropertyChanged(propertyName: GetPropertyName<T>(expr: expr));
        }

        /// <summary>
        /// Raises the <see cref="NotifiableBase.PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>Event was raised or not.</returns>
        protected bool OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName: propertyName));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="NotifiableBase.PropertyChanging" /> event.
        /// </summary>
        /// <param name="expr">The expression that contains the property name.</param>
        /// <exception cref="ArgumentException">
        /// The body of <paramref name="expr" /> is NO <see cref="MemberExpression" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="expr" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The body of <paramref name="expr" /> does not represent an expression that contains a property.
        /// </exception>
        protected bool OnPropertyChanging<T>(Expression<Func<T>> expr)
        {
            return this.OnPropertyChanging(propertyName: GetPropertyName<T>(expr: expr));
        }

        /// <summary>
        /// Raises the <see cref="NotifiableBase.PropertyChanging" /> event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>Event was raised or not.</returns>
        protected bool OnPropertyChanging(string propertyName)
        {
            var handler = this.PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName: propertyName));
                return true;
            }

            return false;
        }

        #endregion Methods (6)
    }
}