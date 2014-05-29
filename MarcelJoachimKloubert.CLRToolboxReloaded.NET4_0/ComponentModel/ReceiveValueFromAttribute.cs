// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.ComponentModel
{
    /// <summary>
    /// Defines from where a property, field or method should receive (new) values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field,
                    AllowMultiple = true,
                    Inherited = false)]
    public sealed class ReceiveValueFromAttribute : Attribute
    {
        #region Constructors (3)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveValueFromAttribute" /> class.
        /// </summary>
        /// <param name="senderName">
        /// The value for the <see cref="ReceiveValueFromAttribute.SenderName" /> property.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="senderName" /> is invalid.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="senderName" /> is <see langword="null" />.
        /// </exception>
        public ReceiveValueFromAttribute(string senderName)
            : this(senderName, ReceiveValueFromOptions.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveValueFromAttribute" /> class.
        /// </summary>
        /// <param name="senderName">
        /// The value for the <see cref="ReceiveValueFromAttribute.SenderName" /> property.
        /// </param>
        /// <param name="options">
        /// The value for the <see cref="ReceiveValueFromAttribute.Options" /> property.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="senderName" /> is invalid.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="senderName" /> is <see langword="null" />.
        /// </exception>
        public ReceiveValueFromAttribute(string senderName,
                                         ReceiveValueFromOptions options)
        {
            if (senderName == null)
            {
                throw new ArgumentNullException("senderName");
            }

            this.SenderName = senderName.Trim();
            if (this.SenderName == string.Empty)
            {
                throw new ArgumentException("senderName");
            }

            this.Options = options;
        }

        #endregion Constructors

        #region Properties (2)

        /// <summary>
        /// Gets the options.
        /// </summary>
        public ReceiveValueFromOptions Options
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of sender / sending member of the notification.
        /// </summary>
        public string SenderName
        {
            get;
            private set;
        }

        #endregion Properties
    }
}