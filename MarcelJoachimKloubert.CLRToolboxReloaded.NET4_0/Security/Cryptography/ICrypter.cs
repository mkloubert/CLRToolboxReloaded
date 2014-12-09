// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Cryptography
{
    /// <summary>
    /// Describes an object that encrypts and decrypts data.
    /// </summary>
    public interface ICrypter : IObject
    {
        #region Properties (1)

        /// <summary>
        /// Gets if that object is able to decrypt data or not.
        /// </summary>
        bool CanDecrypt { get; }

        /// <summary>
        /// Gets if that object is able to encrypt data or not.
        /// </summary>
        bool CanEncrypt { get; }

        #endregion Properties (1)

        #region Methods (8)

        /// <summary>
        /// Decrypts data.
        /// </summary>
        /// <param name="src">The crypted data.</param>
        /// <returns>The uncrypted data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// That object is NOT able to decrypt data.
        /// </exception>
        byte[] Decrypt(IEnumerable<byte> src);

        /// <summary>
        /// Decrypts data.
        /// </summary>
        /// <param name="src">The stream with the crypted data.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <returns>The uncrypted data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// That object is NOT able to decrypt data.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        byte[] Decrypt(Stream src, int? bufferSize = null);

        /// <summary>
        /// Decrypts data.
        /// </summary>
        /// <param name="src">The crypted data.</param>
        /// <param name="dest">The stream where to write the decrypted data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// That object is NOT able to decrypt data.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        void Decrypt(IEnumerable<byte> src, Stream dest);

        /// <summary>
        /// Decrypts data.
        /// </summary>
        /// <param name="src">The stream with the crypted data.</param>
        /// <param name="dest">The stream where to write the decrypted data to.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// That object is NOT able to decrypt data.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read and/or <paramref name="dest" /> cannot be written.
        /// </exception>
        void Decrypt(Stream src, Stream dest, int? bufferSize = null);

        /// <summary>
        /// Encrypts data.
        /// </summary>
        /// <param name="src">The uncrypted data.</param>
        /// <returns>The crypted data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// That object is NOT able to encrypt data.
        /// </exception>
        byte[] Encrypt(IEnumerable<byte> src);

        /// <summary>
        /// Encrypts data.
        /// </summary>
        /// <param name="src">The stream with the uncrypted data.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <returns>The crypted data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// That object is NOT able to encrypt data.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        byte[] Encrypt(Stream src, int? bufferSize = null);

        /// <summary>
        /// Encrypts data.
        /// </summary>
        /// <param name="src">The uncrypted data.</param>
        /// <param name="dest">The stream where to write the encrypted data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// That object is NOT able to encrypt data.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        void Encrypt(IEnumerable<byte> src, Stream dest);

        /// <summary>
        /// Encrypts data.
        /// </summary>
        /// <param name="src">The stream with the uncrypted data.</param>
        /// <param name="dest">The stream where to write the encrypted data to.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// That object is NOT able to encrypt data.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read and/or <paramref name="dest" /> cannot be written.
        /// </exception>
        void Encrypt(Stream src, Stream dest, int? bufferSize = null);

        #endregion Methods (8)
    }
}