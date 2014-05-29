// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.ComponentModel
{
    /// <summary>
    /// Defines from where a property should receive notifications.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,
                    AllowMultiple = true,
                    Inherited = false)]
    public sealed class ReceiveNotificationFromAttribute : Attribute
    {
        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveNotificationFromAttribute" /> class.
        /// </summary>
        /// <param name="senderName">
        /// The value for the <see cref="ReceiveNotificationFromAttribute.SenderName" /> property.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="senderName" /> is invalid.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="senderName" /> is <see langword="null" />.
        /// </exception>
        public ReceiveNotificationFromAttribute(string senderName)
            : this(senderName, ReceiveNotificationFromOptions.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveNotificationFromAttribute" /> class.
        /// </summary>
        /// <param name="senderName">
        /// The value for the <see cref="ReceiveNotificationFromAttribute.SenderName" /> property.
        /// </param>
        /// <param name="options">
        /// The value for the <see cref="ReceiveNotificationFromAttribute.Options" /> property.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="senderName" /> is invalid.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="senderName" /> is <see langword="null" />.
        /// </exception>
        public ReceiveNotificationFromAttribute(string senderName,
                                                ReceiveNotificationFromOptions options)
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
        public ReceiveNotificationFromOptions Options
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