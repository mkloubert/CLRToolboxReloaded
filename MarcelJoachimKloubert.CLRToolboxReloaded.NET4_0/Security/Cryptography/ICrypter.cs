// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        #region Methods (20)

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
        /// Decrypts a UTF-8 string.
        /// </summary>
        /// <param name="src">The stream with the crypted data.</param>
        /// <param name="builder">The <see cref="StringBuilder" /> where the string should be written to.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="builder" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        void DecryptString(Stream src, StringBuilder builder, int? bufferSize = null);

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="src">The stream with the crypted data.</param>
        /// <param name="builder">The <see cref="StringBuilder" /> where the string should be written to.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" />, <paramref name="builder" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        void DecryptString(Stream src, StringBuilder builder, Encoding enc, int? bufferSize = null);

        /// <summary>
        /// Decrypts a UTF-8 string.
        /// </summary>
        /// <param name="src">The stream with the crypted data.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        string DecryptString(Stream src, int? bufferSize = null);

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="src">The stream with the crypted data.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <returns>The decoded string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        string DecryptString(Stream src, Encoding enc, int? bufferSize = null);

        /// <summary>
        /// Decrypts a UTF-8 string.
        /// </summary>
        /// <param name="src">The crypted data.</param>
        /// <param name="builder">The <see cref="StringBuilder" /> where the string should be written to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="builder" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        void DecryptString(IEnumerable<byte> src, StringBuilder builder);

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="src">The crypted data.</param>
        /// <param name="builder">The <see cref="StringBuilder" /> where the string should be written to.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" />, <paramref name="builder" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        void DecryptString(IEnumerable<byte> src, StringBuilder builder, Encoding enc);

        /// <summary>
        /// Decrypts a UTF-8 string.
        /// </summary>
        /// <param name="src">The crypted data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        string DecryptString(IEnumerable<byte> src);

        /// <summary>
        /// Decrypts a string.
        /// </summary>
        /// <param name="src">The crypted data.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <returns>The decoded string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        string DecryptString(IEnumerable<byte> src, Encoding enc);

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

        /// <summary>
        /// Encrypts a string as UTF-8 data.
        /// </summary>
        /// <param name="str">The string to encrypt.</param>
        /// <param name="dest">The stream where to write the encrypted data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dest" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <remarks>
        /// <see langword="null" /> strings are handled as empty strings.
        /// </remarks>
        void EncryptString(string str, Stream dest);

        /// <summary>
        /// Encrypts a string.
        /// </summary>
        /// <param name="str">The string to encrypt.</param>
        /// <param name="dest">The stream where to write the encrypted data to.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dest" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <remarks>
        /// <see langword="null" /> strings are handled as empty strings.
        /// </remarks>
        void EncryptString(string str, Stream dest, Encoding enc);

        /// <summary>
        /// Encrypts a string as UTF-8 data.
        /// </summary>
        /// <param name="str">The string to encrypt.</param>
        /// <returns>The encrypted data.</returns>
        /// <remarks>
        /// <see langword="null" /> strings are handled as empty strings.
        /// </remarks>
        byte[] EncryptString(string str);

        /// <summary>
        /// Encrypts a string.
        /// </summary>
        /// <param name="str">The string to encrypt.</param>
        /// <param name="enc">The encoding to use.</param>
        /// <returns>The encrypted data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enc" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>
        /// <see langword="null" /> strings are handled as empty strings.
        /// </remarks>
        byte[] EncryptString(string str, Encoding enc);

        #endregion Methods (20)
    }
}