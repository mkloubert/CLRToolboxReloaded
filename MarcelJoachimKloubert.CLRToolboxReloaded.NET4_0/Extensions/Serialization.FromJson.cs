// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (3)

        /// <summary>
        /// Deserializes a JSON string to a dictionary.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>
        /// The deserialized data as dictionary or <see langword="null" />.
        /// </returns>
        public static dynamic FromJson(this IEnumerable<char> json)
        {
            return FromJson<ExpandoObject>(jsonStr: AsString(json));
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
            return FromJson<T>(jsonStr: AsString(json));
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
            if (string.IsNullOrWhiteSpace(jsonStr))
            {
                return default(T);
            }

            var serializer = JsonSerializer.CreateDefault();

            using (var reader = new StringReader(jsonStr))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    jsonReader.CloseInput = false;

                    return serializer.Deserialize<T>(jsonReader);
                }
            }
        }

        #endregion Methods (3)
    }
}