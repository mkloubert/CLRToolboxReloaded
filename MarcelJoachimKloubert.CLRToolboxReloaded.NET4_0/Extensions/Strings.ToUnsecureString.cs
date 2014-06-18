// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Converts a <see cref="SecureString" /> back to a <see cref="string" /> object.
        /// </summary>
        /// <param name="secStr">the secure string.</param>
        /// <returns>
        /// The UNsecure string or <see langword="null" /> if <paramref name="secStr" />
        /// is also <see langword="null" />.
        /// </returns>
        public static string ToUnsecureString(this SecureString secStr)
        {
            if (secStr == null)
            {
                return null;
            }

            var ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secStr);
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }

        #endregion Methods (1)
    }
}