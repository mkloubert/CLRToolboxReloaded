// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (6)

        /// <summary>
        /// Deserializes a JSON string to a dictionary.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>
        /// The deserialized data as dictionary or <see langword="null" />.
        /// </returns>
        public static IDictionary<string, object> FromJson(this IEnumerable<char> json)
        {
            return FromJson(json,
                            settings: null);
        }

        /// <summary>
        /// Deserializes a JSON string to a dictionary.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <param name="settings">
        /// The optional settings to use.
        /// If <see langword="null" /> the default settings are used (s. <see cref="JsonSerializer.CreateDefault()" />).
        /// </param>
        /// <returns>
        /// The deserialized data as dictionary or <see langword="null" />.
        /// </returns>
        public static IDictionary<string, object> FromJson(this IEnumerable<char> json, JsonSerializerSettings settings)
        {
            return FromJson<IDictionary<string, object>>(jsonStr: AsString(json),
                                                         settings: settings);
        }

        /// <summary>
        /// Deserializes a JSON string to an object.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>
        /// The deserialized instance or <see langword="null" />.
        /// </returns>
        public static T FromJson<T>(this IEnumerable<char> json)
        {
            return FromJson<T>(json,
                               settings: null);
        }

        /// <summary>
        /// Deserializes a JSON string to an object.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <param name="settings">
        /// The optional settings to use.
        /// If <see langword="null" /> the default settings are used (s. <see cref="JsonSerializer.CreateDefault()" />).
        /// </param>
        /// <returns>
        /// The deserialized instance or <see langword="null" />.
        /// </returns>
        public static T FromJson<T>(this IEnumerable<char> json, JsonSerializerSettings settings)
        {
            return FromJson<T>(jsonStr: AsString(json),
                               settings: settings);
        }

        /// <summary>
        /// Deserializes a JSON string to an object.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="jsonStr">The JSON string.</param>
        /// <returns>
        /// The deserialized instance or <see langword="null" />.
        /// </returns>
        public static T FromJson<T>(this string jsonStr)
        {
            return FromJson<T>(jsonStr,
                               settings: null);
        }

        /// <summary>
        /// Deserializes a JSON string to an object.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="jsonStr">The JSON string.</param>
        /// <param name="settings">
        /// The optional settings to use.
        /// If <see langword="null" /> the default settings are used (s. <see cref="JsonSerializer.CreateDefault()" />).
        /// </param>
        /// <returns>
        /// The deserialized instance or <see langword="null" />.
        /// </returns>
        public static T FromJson<T>(this string jsonStr, JsonSerializerSettings settings)
        {
			if (string.IsNullOrWhiteSpace(jsonStr))
            {
                return default(T);
            }

            JsonSerializer serializer = settings != null ? JsonSerializer.Create(settings) : JsonSerializer.CreateDefault();

            using (var reader = new StringReader(jsonStr))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    jsonReader.CloseInput = false;

                    return serializer.Deserialize<T>(jsonReader);
                }
            }
        }

        #endregion Methods (6)
    }
}