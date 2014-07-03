// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// A basic object.
    /// </summary>
    public abstract class ObjectBase : IObject
    {
        #region Fields (1)

        /// <summary>
        /// Stores an object for thread safe operations.
        /// </summary>
        protected readonly object _SYNC;

        #endregion Fields (1)

        #region Constrcutors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBase" /> class.
        /// </summary>
        protected ObjectBase()
            : this(sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBase" /> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected ObjectBase(object sync)
        {
            if (sync == null)
            {
                throw new ArgumentNullException("sync");
            }

            this._SYNC = sync;
        }

        #endregion Constrcutors (2)

        #region Properties (2)

        /// <inheriteddoc />
        public object SyncRoot
        {
            get { return this._SYNC; }
        }

        /// <inheriteddoc />
        public virtual object Tag
        {
            get;
            set;
        }

        #endregion Properties (2)

        #region Methods (4)

        /// <summary>
        /// Converts / casts a sequence to an array.
        /// </summary>
        /// <typeparam name="T">Type of the elements.</typeparam>
        /// <param name="seq">The sequence to convert / cast.</param>
        /// <returns>
        /// <paramref name="seq" /> as array or <see langword="null" /> if
        /// <paramref name="seq" /> is also <see langword="null" />.
        /// </returns>
        /// <remarks>
        /// If <paramref name="seq" /> is already an array it is simply casted.
        /// </remarks>
        protected static T[] AsArray<T>(IEnumerable<T> seq)
        {
            if (seq == null)
            {
                return null;
            }

            var result = seq as T[];
            if (result == null)
            {
                result = seq.ToArray();
            }

            return result;
        }

        /// <summary>
        /// Creates a Rijndael crypter from a password and a salt.
        /// </summary>
        /// <param name="pwd">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>The Rijndael crypter.</returns>
        protected static Rijndael CreateRijndael(IEnumerable<byte> pwd,
                                                 IEnumerable<byte> salt)
        {
            var pdb = new Rfc2898DeriveBytes(AsArray(pwd), AsArray(salt),
                                             1000);

            var result = Rijndael.Create();
            result.Key = pdb.GetBytes(32);
            result.IV = pdb.GetBytes(16);

            return result;
        }

        /// <summary>
        /// Raises an event handler.
        /// </summary>
        /// <param name="handler">The handler to raise.</param>
        /// <returns>Handler was raised or not.</returns>
        protected bool RaiseEventHandler(EventHandler handler)
        {
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
                return true;
            }

            return false;
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

        #endregion Methods (3)
    }
}