// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization
{
    /// <summary>
    /// Describes an object that serializes and deserializes objects
    /// to specific data types by using string data.
    /// </summary>
    public interface IStringSerializer : IObjectSerializer
    {
        #region Methods (6)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.CanDeserialize{T}(object)" />
        bool CanDeserialize<T>(string strData);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.CanDeserialize(object, Type)" />
        bool CanDeserialize(string strData, Type targetType);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.Deserialize{T}(object)" />
        T Deserialize<T>(string strData);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.Deserialize(object, Type)" />
        object Deserialize(string strData, Type deserializeAs);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.Serialize{T}(T)" />
        new string Serialize<T>(T obj);

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IObjectSerializer.Serialize(object, Type)" />
        new string Serialize(object obj, Type serializeAs);

        #endregion Methods (4)
    }
}