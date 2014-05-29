﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.ComponentModel
{
    /// <summary>
    /// Options for <see cref="ReceiveNotificationFromAttribute" />.
    /// </summary>
    [Flags]
    public enum ReceiveNotificationFromOptions
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,

        /// <summary>
        /// Default options
        /// </summary>
        Default = 1,

        /// <summary>
        /// Only if values are different
        /// </summary>
        IfDifferent = 2,

        /// <summary>
        /// Even if old and new value are equal
        /// </summary>
        IfEqual = 4,
    }
}