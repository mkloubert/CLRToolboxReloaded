// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE)
#define KNOWS_PROPERTY_CHANGING
#endif

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MarcelJoachimKloubert.CLRToolbox.ComponentModel
{
    /// <summary>
    /// A basic implementation of <see cref="INotifiable" /> interface.
    /// </summary>
    public abstract class NotifiableBase : ObjectBase, INotifiable
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected NotifiableBase(bool synchronized, object sync)
            : base(synchronized: synchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected NotifiableBase(bool synchronized)
            : base(synchronized: synchronized)
        {
        }

        /// <inheriteddoc />
        protected NotifiableBase(object sync)
            : base(synchronized: true,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected NotifiableBase()
            : base(synchronized: true)
        {
        }

        #endregion Constrcutors (4)

        #region Events (2)

        /// <inheriteddoc />
        public event PropertyChangedEventHandler PropertyChanged;

#if KNOWS_PROPERTY_CHANGING

        /// <inheriteddoc />
        public event global::System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

#endif

        #endregion Events (2)

        #region Events (2)

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

            PropertyChangedEventHandler handler = this.PropertyChanged;
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

            global::System.ComponentModel.PropertyChangingEventHandler handler = this.PropertyChanging;
            if (handler != null)
            {
                handler(this, new global::System.ComponentModel.PropertyChangingEventArgs(propertyName.AsString()));
                return true;
            }

            return false;
        }

#endif

        #endregion Events (2)
    }
}