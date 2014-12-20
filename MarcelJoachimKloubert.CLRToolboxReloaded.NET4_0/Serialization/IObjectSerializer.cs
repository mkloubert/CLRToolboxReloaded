// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization
{
    /// <summary>
    /// Describes an object that serializes and deserializes objects
    /// to specific data types.
    /// </summary>
    public interface IObjectSerializer : IObject
    {
        #region Methods (8)

        /// <summary>
        /// Checks if data can be deserialized to a specific data type.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="data">The data to check.</param>
        /// <returns>Can be deserialized or not.</returns>
        bool CanDeserialize<T>(object data);

        /// <summary>
        /// Checks if data can be deserialized to a specific data type.
        /// </summary>
        /// <param name="data">The data to check.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>Can be deserialized or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="targetType" /> is <see langword="null" />.
        /// </exception>
        bool CanDeserialize(object data, Type targetType);

        /// <summary>
        /// Checks if an object can be serialized.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to check.</param>
        /// <returns>Can be serialized or not.</returns>
        bool CanSerialize<T>(T obj);

        /// <summary>
        /// Checks if an object can be serialized.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="serializeAs">The type the object should be serialized as.</param>
        /// <returns>Can be serialized or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serializeAs" /> is <see langword="null" />.
        /// </exception>
        bool CanSerialize(object obj, Type serializeAs);

        /// <summary>
        /// Deserializes data to an object.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="data">The data to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="data" /> cannot be deserialized to target type.
        /// </exception>
        T Deserialize<T>(object data);

        /// <summary>
        /// Deserializes data to an object.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <param name="deserializeAs">The target type.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="deserializeAs" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// <paramref name="data" /> cannot be deserialized to target type.
        /// </exception>
        object Deserialize(object data, Type deserializeAs);

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized data of <paramref name="obj" />.</returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="obj" /> cannot be serialized.
        /// </exception>
        object Serialize<T>(T obj);

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializeAs">The type the object should be serialized as.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serializeAs" /> is <see langword="null" />.
        /// </exception>
        /// <returns>The serialized data of <paramref name="obj" />.</returns>
        /// <exception cref="InvalidCastException">
        /// <paramref name="obj" /> cannot be serialized as <paramref name="serializeAs" />.
        /// </exception>
        object Serialize(object obj, Type serializeAs);

        #endregion Methods (8)
    }
}