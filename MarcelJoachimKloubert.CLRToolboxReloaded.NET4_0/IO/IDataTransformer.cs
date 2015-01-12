// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40 || PORTABLE45)
#define KNOWS_SECURE_STRING
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// Describes an object that transforms (binary) data.
    /// </summary>
    public interface IDataTransformer : IObject
    {
        #region Properties (2)

        /// <summary>
        /// Gets if that transformer can restore transformed data or not.
        /// </summary>
        bool CanRestoreData { get; }

        /// <summary>
        /// Gets if that object can transform data or not.
        /// </summary>
        bool CanTransformData { get; }

        #endregion Properties (2)

        #region Methods (32)

        /// <summary>
        /// Restores transformed data.
        /// </summary>
        /// <param name="blob">The source data.</param>
        /// <returns>The restored data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        byte[] RestoreData(IEnumerable<byte> blob);

        /// <summary>
        /// Restores transformed data.
        /// </summary>
        /// <param name="blob">The source data.</param>
        /// <param name="dest">The destination stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        void RestoreData(IEnumerable<byte> blob, Stream dest);

        /// <summary>
        /// Restores transformed data.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <returns>The restored data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        byte[] RestoreData(Stream src, int? bufferSize = null);

        /// <summary>
        /// Restores transformed data.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read and/or <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        void RestoreData(Stream src, Stream dest, int? bufferSize = null);

#if KNOWS_SECURE_STRING

        /// <summary>
        /// Restores an UTF-8 string as secure string.
        /// </summary>
        /// <param name="src">The stream with the crypted data.</param>
        /// <param name="bufferSize">
        /// The buffer in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        /// <returns>The decoded string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        global::System.Security.SecureString RestoreSecureString(global::System.IO.Stream src, int? bufferSize = null);

        /// <summary>
        /// Restores a string as secure string.
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
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        global::System.Security.SecureString RestoreSecureString(global::System.IO.Stream src, global::System.Text.Encoding enc, int? bufferSize = null);

        /// <summary>
        /// Restores an UTF-8 string as secure string.
        /// </summary>
        /// <param name="src">The crypted data.</param>
        /// <returns>The decoded string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        global::System.Security.SecureString RestoreSecureString(global::System.Collections.Generic.IEnumerable<byte> src);

        /// <summary>
        /// Restores a string as secure string.
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
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        global::System.Security.SecureString RestoreSecureString(global::System.Collections.Generic.IEnumerable<byte> src, global::System.Text.Encoding enc);

#endif

        /// <summary>
        /// Restores transformed data as an UTF-8 string.
        /// </summary>
        /// <param name="blob">The transformed data.</param>
        /// <returns>The restored string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        string RestoreString(IEnumerable<byte> blob);

        /// <summary>
        /// Restores transformed data as a string.
        /// </summary>
        /// <param name="blob">The transformed data.</param>
        /// <param name="enc">The charset to use.</param>
        /// <returns>The restored string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        string RestoreString(IEnumerable<byte> blob, Encoding enc);

        /// <summary>
        /// Restores transformed data as an UTF-8 string.
        /// </summary>
        /// <param name="blob">The transformed data.</param>
        /// <param name="builder">The target where to write the string to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob" /> and/or <paramref name="builder" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        void RestoreString(IEnumerable<byte> blob, StringBuilder builder);

        /// <summary>
        /// Restores transformed data as a string.
        /// </summary>
        /// <param name="blob">The transformed data.</param>
        /// <param name="builder">The target where to write the string to.</param>
        /// <param name="enc">The charset to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob" />, <paramref name="builder" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        void RestoreString(IEnumerable<byte> blob, StringBuilder builder, Encoding enc);

        /// <summary>
        /// Restores transformed data as an UTF-8 string.
        /// </summary>
        /// <param name="blob">The transformed data.</param>
        /// <param name="writer">The target where to write the string to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob" /> and/or <paramref name="writer" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        /// <remarks>
        /// <see cref="TextWriter.Write(string)" /> method is called to append restored string.
        /// </remarks>
        void RestoreString(IEnumerable<byte> blob, TextWriter writer);

        /// <summary>
        /// Restores transformed data as a string.
        /// </summary>
        /// <param name="blob">The transformed data.</param>
        /// <param name="writer">The target where to write the string to.</param>
        /// <param name="enc">The charset to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob" />, <paramref name="writer" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        /// <remarks>
        /// <see cref="TextWriter.Write(string)" /> method is called to append restored string.
        /// </remarks>
        void RestoreString(IEnumerable<byte> blob, TextWriter writer, Encoding enc);

        /// <summary>
        /// Restores transformed data as an UTF-8 string.
        /// </summary>
        /// <param name="src">The transformed data stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <returns>The restored string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        string RestoreString(Stream src, int? bufferSize = null);

        /// <summary>
        /// Restores transformed data as a string.
        /// </summary>
        /// <param name="src">The transformed data stream.</param>
        /// <param name="enc">The charset to use.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <returns>The restored string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        string RestoreString(Stream src, Encoding enc, int? bufferSize = null);

        /// <summary>
        /// Restores transformed data as an UTF-8 string.
        /// </summary>
        /// <param name="src">The transformed data stream.</param>
        /// <param name="builder">The target where to write the string to.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="builder" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        void RestoreString(Stream src, StringBuilder builder, int? bufferSize = null);

        /// <summary>
        /// Restores transformed data as a string.
        /// </summary>
        /// <param name="src">The transformed data stream.</param>
        /// <param name="builder">The target where to write the string to.</param>
        /// <param name="enc">The charset to use.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" />, <paramref name="builder" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        void RestoreString(Stream src, StringBuilder builder, Encoding enc, int? bufferSize = null);

        /// <summary>
        /// Restores transformed data as an UTF-8 string.
        /// </summary>
        /// <param name="src">The transformed data stream.</param>
        /// <param name="writer">The target where to write the string to.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="writer" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        /// <remarks>
        /// <see cref="TextWriter.Write(string)" /> method is called to append restored string.
        /// </remarks>
        void RestoreString(Stream src, TextWriter writer, int? bufferSize = null);

        /// <summary>
        /// Restores transformed data as a string.
        /// </summary>
        /// <param name="src">The transformed data stream.</param>
        /// <param name="writer">The target where to write the string to.</param>
        /// <param name="enc">The charset to use.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" />, <paramref name="writer" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Transformed data cannot be restored.
        /// </exception>
        /// <remarks>
        /// <see cref="TextWriter.Write(string)" /> method is called to append restored string.
        /// </remarks>
        void RestoreString(Stream src, TextWriter writer, Encoding enc, int? bufferSize = null);

        /// <summary>
        /// Transforms data.
        /// </summary>
        /// <param name="blob">The source data.</param>
        /// <returns>The restored data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        byte[] TransformData(IEnumerable<byte> blob);

        /// <summary>
        /// Transforms data.
        /// </summary>
        /// <param name="blob">The source data.</param>
        /// <param name="dest">The destination stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="blob" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        void TransformData(IEnumerable<byte> blob, Stream dest);

        /// <summary>
        /// Transforms data.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <returns>The restored data.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        byte[] TransformData(Stream src, int? bufferSize = null);

        /// <summary>
        /// Transforms data.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="src" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize" /> is invalid (smaller than 1).
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="src" /> cannot be read and/or <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        void TransformData(Stream src, Stream dest, int? bufferSize = null);

        /// <summary>
        /// Transforms a string.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <param name="dest">The stream where to write the transformed data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dest" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        /// <remarks>
        /// <see langword="null" /> strings are handled as empty strings.
        /// </remarks>
        void TransformString(string str, Stream dest);

        /// <summary>
        /// Transforms a string.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <param name="enc">The charset to use.</param>
        /// <param name="dest">The stream where to write the transformed data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enc" /> and/or <paramref name="dest" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        /// <remarks>
        /// <see langword="null" /> strings are handled as empty strings.
        /// </remarks>
        void TransformString(string str, Encoding enc, Stream dest);

        /// <summary>
        /// Transforms an UTF-8  string.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <returns>The transformed string.</returns>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        /// <remarks>
        /// <see langword="null" /> strings are handled as empty strings.
        /// </remarks>
        byte[] TransformString(string str);

        /// <summary>
        /// Transforms a string.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <param name="enc">The charset to use.</param>
        /// <returns>The transformed string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enc" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        /// <remarks>
        /// <see langword="null" /> strings are handled as empty strings.
        /// </remarks>
        byte[] TransformString(string str, Encoding enc);

        /// <summary>
        /// Transforms n UTF-8 string from a <see cref="TextReader" />.
        /// </summary>
        /// <param name="reader">The reader from where to read the whole data from.</param>
        /// <param name="dest">The stream where to write the transformed data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        /// <remarks>
        /// <see cref="TextReader.ReadToEnd()" /> is called to get the string data.
        /// </remarks>
        void TransformString(TextReader reader, Stream dest);

        /// <summary>
        /// Transforms a string from a <see cref="TextReader" />.
        /// </summary>
        /// <param name="reader">The reader from where to read the whole data from.</param>
        /// <param name="enc">The charset to use.</param>
        /// <param name="dest">The stream where to write the transformed data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="dest" /> cannot be written.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        /// <remarks>
        /// <see cref="TextReader.ReadToEnd()" /> is called to get the string data.
        /// </remarks>
        void TransformString(TextReader reader, Encoding enc, Stream dest);

        /// <summary>
        /// Transforms an UTF-8 string from a <see cref="TextReader" />.
        /// </summary>
        /// <param name="reader">The reader from where to read the whole data from.</param>
        /// <returns>The transformed string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        /// <remarks>
        /// <see cref="TextReader.ReadToEnd()" /> is called to get the string data.
        /// </remarks>
        byte[] TransformString(TextReader reader);

        /// <summary>
        /// Transforms a string from a <see cref="TextReader" />.
        /// </summary>
        /// <param name="reader">The reader from where to read the whole data from.</param>
        /// <param name="enc">The charset to use.</param>
        /// <returns>The transformed string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader" /> and/or <paramref name="enc" /> are <see langword="null" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Data transformation is NOT supported.
        /// </exception>
        /// <remarks>
        /// <see cref="TextReader.ReadToEnd()" /> is called to get the string data.
        /// </remarks>
        byte[] TransformString(TextReader reader, Encoding enc);

        #endregion Methods (32)
    }
}