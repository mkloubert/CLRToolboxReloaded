// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// A basic object.
    /// </summary>
    public abstract class FileBoxObjectBase
    {
        #region Constrcutors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBoxObjectBase" /> class.
        /// </summary>
        protected FileBoxObjectBase()
        {
        }

        #endregion Constrcutors (1)

        #region Methods (2)

        /// <summary>
        /// Creates a Rijndael crypter from a password and a salt.
        /// </summary>
        /// <param name="pwd">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>The Rijndael crypter.</returns>
        protected static Rijndael CreateRijndael(byte[] pwd, byte[] salt)
        {
            var pdb = new Rfc2898DeriveBytes(pwd, salt,
                                             1000);

            var result = Rijndael.Create();
            result.Key = pdb.GetBytes(32);
            result.IV = pdb.GetBytes(16);

            return result;
        }

        /// <summary>
        /// Converts a <see cref="SecureString" /> back to a <see cref="string" /> object.
        /// </summary>
        /// <param name="secStr">the secure string.</param>
        /// <returns>
        /// The UNsecure string or <see langword="null" /> if <paramref name="secStr" />
        /// is also <see langword="null" />.
        /// </returns>
        protected static string ToUnsecureString(SecureString secStr)
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

        #endregion Methods (2)
    }
}