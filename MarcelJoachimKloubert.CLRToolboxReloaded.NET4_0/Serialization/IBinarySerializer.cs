// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization
{
    /// <summary>
    /// Describes an object that serializes / deserializes objects via binary data.
    /// </summary>
    public interface IBinarySerializer : IObjectSerializer
    {
        #region Methods (12)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.CanDeserialize{T}(object)" />
        bool CanDeserialize<T>(IEnumerable<byte> binData);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.CanDeserialize{T}(object)" />
        bool CanDeserialize<T>(Stream src);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.CanDeserialize(object, Type)" />
        bool CanDeserialize(IEnumerable<byte> binData, Type deserializeAs);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.CanDeserialize(object, Type)" />
        bool CanDeserialize(Stream src, Type deserializeAs);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.Deserialize{T}(object)" />
        T Deserialize<T>(IEnumerable<byte> binData);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.Deserialize(object, Type)" />
        T Deserialize<T>(Stream src);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.Deserialize{T}(object)" />
        object Deserialize(IEnumerable<byte> binData, Type deserializeAs);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.Deserialize(object, Type)" />
        object Deserialize(Stream src, Type deserializeAs);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.Serialize{T}(T)" />
        new byte[] Serialize<T>(T obj);

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="target">The stream where to write the serialized data to.</param>
        /// <returns>The serialized data of <paramref name="obj" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="target" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// <paramref name="obj" /> cannot be serialized.
        /// </exception>
        /// <exception cref="IOException">
        /// <paramref name="target" /> cannot be written.
        /// </exception>
        void Serialize<T>(T obj, Stream target);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.Serialize(object, Type)" />
        new byte[] Serialize(object obj, Type serializeAs);

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializeAs">The type the object should be serialized as.</param>
        /// <param name="target">The stream where to write the serialized data to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serializeAs" /> and/or <paramref name="target" /> are <see langword="null" />.
        /// </exception>
        /// <returns>The serialized data of <paramref name="obj" />.</returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="obj" /> cannot be serialized as <paramref name="serializeAs" />.
        /// </exception>
        void Serialize(object obj, Type serializeAs, Stream target);

        #endregion Methods (12)
    }
}