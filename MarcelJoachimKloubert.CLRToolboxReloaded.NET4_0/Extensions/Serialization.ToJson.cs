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
        #region Methods (9)

        /// <summary>
        /// Serializes an object / value / reference to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type that defines how to handle <paramref name="obj" />.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The object as JSON string.</returns>
        /// <remarks>DBNull values are handles as <see langword="null" /> reference.</remarks>
        public static string ToJson<T>(this T obj)
        {
            return ToJson<T>(obj,
                             handleDbNullAsNull: true);
        }

        /// <summary>
        /// Serializes an object / value / reference to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type that defines how to handle <paramref name="obj" />.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="handleDbNullAsNull">
        /// Handle DBNull as <see langword="null" /> reference or not.
        /// </param>
        /// <returns>The object as JSON string.</returns>
        public static string ToJson<T>(this T obj, bool handleDbNullAsNull)
        {
            return ToJson<T>(obj,
                             settings: null,
                             handleDbNullAsNull: true);
        }

        /// <summary>
        /// Serializes an object / value / reference to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type that defines how to handle <paramref name="obj" />.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="settings">
        /// The optional settings to use.
        /// If <see langword="null" /> the default settings are used (s. <see cref="JsonSerializer.CreateDefault()" />).
        /// </param>
        /// <param name="handleDbNullAsNull">
        /// Handle DBNull as <see langword="null" /> reference or not.
        /// </param>
        /// <returns>The object as JSON string.</returns>
        public static string ToJson<T>(this T obj, JsonSerializerSettings settings, bool handleDbNullAsNull = true)
        {
            StringBuilder builder = new StringBuilder();
            ToJson<T>(obj,
                      builder: builder,
                      settings: settings,
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="builder" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>DBNull values are handles as <see langword="null" /> reference.</remarks>
        public static void ToJson<T>(this T obj, StringBuilder builder)
        {
            ToJson<T>(obj,
                      builder: builder,
                      settings: null,
                      handleDbNullAsNull: true);
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
        public static void ToJson<T>(this T obj, StringBuilder builder, bool handleDbNullAsNull)
        {
            ToJson<T>(obj,
                      builder: builder,
                      settings: null,
                      handleDbNullAsNull: handleDbNullAsNull);
        }

        /// <summary>
        /// Serializes an object / value / reference to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type that defines how to handle <paramref name="obj" />.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="builder">
        /// The string builder that is used to save the JSON data.
        /// </param>
        /// <param name="settings">
        /// The optional settings to use.
        /// If <see langword="null" /> the default settings are used (s. <see cref="JsonSerializer.CreateDefault()" />).
        /// </param>
        /// <param name="handleDbNullAsNull">
        /// Handle DBNull as <see langword="null" /> reference or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="builder" /> is <see langword="null" />.
        /// </exception>
        public static void ToJson<T>(this T obj, StringBuilder builder, JsonSerializerSettings settings, bool handleDbNullAsNull = true)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            using (var writer = new StringWriter(builder))
            {
                ToJson<T>(obj, writer, settings, handleDbNullAsNull);
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
                      settings: null,
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
        public static void ToJson<T>(this T obj, TextWriter writer, bool handleDbNullAsNull)
        {
            ToJson<T>(obj,
                      writer: writer,
                      settings: null,
                      handleDbNullAsNull: handleDbNullAsNull);
        }

        /// <summary>
        /// Serializes an object / value / reference to a JSON string.
        /// </summary>
        /// <typeparam name="T">The type that defines how to handle <paramref name="obj" />.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="writer">The writer to use.</param>
        /// <param name="settings">
        /// The optional settings to use.
        /// If <see langword="null" /> the default settings are used (s. <see cref="JsonSerializer.CreateDefault()" />).
        /// </param>
        /// <param name="handleDbNullAsNull">
        /// Handle DBNull as <see langword="null" /> reference or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="writer" /> is <see langword="null" />.
        /// </exception>
        public static void ToJson<T>(this T obj, TextWriter writer, JsonSerializerSettings settings, bool handleDbNullAsNull = true)
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

            JsonSerializer serializer = settings != null ? JsonSerializer.Create(settings) : JsonSerializer.CreateDefault();

            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.CloseOutput = false;

                serializer.Serialize(jsonWriter, obj,
                                     typeof(T));
            }
        }

        #endregion Methods (9)
    }
}