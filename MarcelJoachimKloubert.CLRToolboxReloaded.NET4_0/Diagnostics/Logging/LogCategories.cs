// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define CAN_SERIALIZE
#endif

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Diagnostics.Logging
{
    /// <summary>
    /// List of log categories.
    /// </summary>
#if CAN_SERIALIZE

    [global::System.Runtime.Serialization.DataContract]
#endif
    [Flags]
    public enum LoggerFacadeCategories
    {
        /// <summary>
        /// None
        /// </summary>
#if CAN_SERIALIZE

        [global::System.Runtime.Serialization.EnumMember]
#endif
        None = 0,

        /// <summary>
        /// Information
        /// </summary>
#if CAN_SERIALIZE

        [global::System.Runtime.Serialization.EnumMember]
#endif
        Information = 1,

        /// <summary>
        /// Warnings
        /// </summary>
#if CAN_SERIALIZE

        [global::System.Runtime.Serialization.EnumMember]
#endif
        Warnings = 2,

        /// <summary>
        /// Errors
        /// </summary>
#if CAN_SERIALIZE

        [global::System.Runtime.Serialization.EnumMember]
#endif
        Errors = 4,

        /// <summary>
        /// Fatal errors
        /// </summary>
#if CAN_SERIALIZE

        [global::System.Runtime.Serialization.EnumMember]
#endif
        FatalErrors = 8,

        /// <summary>
        /// Debug
        /// </summary>
#if CAN_SERIALIZE

        [global::System.Runtime.Serialization.EnumMember]
#endif
        Debug = 16,

        /// <summary>
        /// Verbose
        /// </summary>
#if CAN_SERIALIZE

        [global::System.Runtime.Serialization.EnumMember]
#endif
        Verbose = 32,

        /// <summary>
        /// Trace
        /// </summary>
#if CAN_SERIALIZE

        [global::System.Runtime.Serialization.EnumMember]
#endif
        Trace = 64,

        /// <summary>
        /// Tests
        /// </summary>
#if CAN_SERIALIZE

        [global::System.Runtime.Serialization.EnumMember]
#endif
        Assert = 128,

        /// <summary>
        /// TODOs
        /// </summary>
#if CAN_SERIALIZE

        [global::System.Runtime.Serialization.EnumMember]
#endif
        TODO = 256,
    }
}