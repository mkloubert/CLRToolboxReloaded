// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.ComponentModel
{
    #region INTERFACE: IReceiveValueFromArgs

    /// <summary>
    /// Arguments for a method that receives values via <see cref="ReceiveValueFromAttribute" />.
    /// </summary>
    public interface IReceiveValueFromArgs : IObject
    {
        #region Data members (7)

        /// <summary>
        /// Gets if <see cref="IReceiveValueFromArgs.NewValue" /> and
        /// <see cref="IReceiveValueFromArgs.OldValue" /> are different.
        /// </summary>
        bool AreDifferent { get; }

        /// <summary>
        /// The new value.
        /// </summary>
        object NewValue { get; }

        /// <summary>
        /// The old value.
        /// </summary>
        object OldValue { get; }

        /// <summary>
        /// The instance of the sending object.
        /// </summary>
        object Sender { get; }

        /// <summary>
        /// The name of the sending element of <see cref="IReceiveValueFromArgs.Sender" /> (a property, e.g.).
        /// </summary>
        string SenderName { get; }

        /// <summary>
        /// The ID of the sender type (represents the value from 'System.Reflection.MemberTypes' enum).
        /// </summary>
        int SenderType { get; }

        /// <summary>
        /// The target type.
        /// </summary>
        Type TargetType { get; }

        #endregion Data members (7)

        #region Methods (3)

        /// <summary>
        /// Gets the value of <see cref="IReceiveValueFromArgs.NewValue" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The casted value of <see cref="IReceiveValueFromArgs.NewValue" /> property.</returns>
        T GetNewValue<T>();
        
        /// <summary>
        /// Gets the value of <see cref="IReceiveValueFromArgs.OldValue" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The casted value of <see cref="IReceiveValueFromArgs.OldValue" /> property.</returns>
        T GetOldValue<T>();

        /// <summary>
        /// Gets the value of <see cref="IReceiveValueFromArgs.Sender" /> property strong typed.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The casted value of <see cref="IReceiveValueFromArgs.Sender" /> property.</returns>
        T GetSender<T>();

        #endregion
    }

    #endregion INTERFACE: IReceiveValueFromArgs
}