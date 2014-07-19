// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define KNOWS_DBNULL
#endif

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (4)

        /// <summary>
        /// Serializes an object / value / reference to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type that defines how to handle <paramref name="obj" />.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="handleDbNullAsNull">
        /// Handle DBNull as <see langword="null" /> reference or not.
        /// </param>
        /// <returns>The object as JSON string.</returns>
        public static string ToJson<T>(this T obj, bool handleDbNullAsNull = true)
        {
            var builder = new StringBuilder();
            ToJson<T>(obj: obj,
                      builder: builder,
                      handleDbNullAsNull: handleDbNullAsNull);

            return builder.ToString();
        }

        /// <summary>
        /// Serializes an object / value / reference to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type that defines how to handle <paramref name="obj" />.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="builder">
        /// The string builder that is used to save the JSON data.
        /// </param>
        /// <param name="handleDbNullAsNull">
        /// Handle DBNull as <see langword="null" /> reference or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="builder" /> is <see langword="null" />.
        /// </exception>
        public static void ToJson<T>(this T obj, StringBuilder builder, bool handleDbNullAsNull = true)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            using (var writer = new StringWriter(builder))
            {
                ToJson<T>(obj,
                          writer: writer,
                          handleDbNullAsNull: handleDbNullAsNull);
            }
        }

        /// <summary>
        /// Serializes an object / value / reference to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type that defines how to handle <paramref name="obj" />.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="writer" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>DBNull values are handles as <see langword="null" /> reference.</remarks>
        public static void ToJson<T>(this T obj, TextWriter writer)
        {
            ToJson<T>(obj,
                      writer: writer,
                      handleDbNullAsNull: true);
        }

        /// <summary>
        /// Serializes an object / value / reference to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type that defines how to handle <paramref name="obj" />.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        /// <param name="handleDbNullAsNull">
        /// Handle DBNull as <see langword="null" /> reference or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="writer" /> is <see langword="null" />.
        /// </exception>
        public static void ToJson<T>(this T obj, TextWriter writer, bool handleDbNullAsNull = true)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
#if KNOWS_DBNULL

            if (handleDbNullAsNull &&
                global::System.DBNull.Value.Equals(obj))
            {
                obj = default(T);
            }

#endif

            if (obj == null)
            {
                writer.Write("null");
                return;
            }

            var serializer = JsonSerializer.CreateDefault();

            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.CloseOutput = false;

                serializer.Serialize(jsonWriter, obj,
                                     typeof(T));
            }
        }

        #endregion Methods (4)
    }
}