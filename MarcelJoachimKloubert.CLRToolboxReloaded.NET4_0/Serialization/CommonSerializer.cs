// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Serialization.Json;
using Newtonsoft.Json;
using System;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization
{
    /// <summary>
    /// A common serializer.
    /// </summary>
    public class CommonSerializer : SerializerBase
    {
        #region Fields (1)

        private readonly JsonSerializerProvider _JSON_SERIALIZER_PROVIDER;

        #endregion Fields (1)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerBase" /> class.
        /// </summary>
        public CommonSerializer()
            : base(isSynchronized: false)
        {
            this._JSON_SERIALIZER_PROVIDER = this.GetJsonNetSerializer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerBase" /> class.
        /// </summary>
        /// <param name="jsonSerializerProvider">The function / method that provides the JSON serializer to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="jsonSerializerProvider" /> is <see langword="null" />.
        /// </exception>
        public CommonSerializer(JsonSerializerProvider jsonSerializerProvider)
            : base(isSynchronized: false)
        {
            if (jsonSerializerProvider == null)
            {
                throw new ArgumentNullException("jsonSerializerProvider");
            }

            this._JSON_SERIALIZER_PROVIDER = jsonSerializerProvider;
        }

        #endregion Constructors

        #region Events and delegates (1)

        /// <summary>
        /// Provides the <see cref="JsonNetSerializer" /> to use.
        /// </summary>
        /// <param name="serializer">The underlying instance of that class.</param>
        /// <returns>The serializer to use.</returns>
        public delegate JsonNetSerializer JsonSerializerProvider(CommonSerializer serializer);

        #endregion

        #region Methods (8)

        /// <summary>
        /// Creates a new instance of the <see cref="CommonSerializer" /> class.
        /// </summary>
        /// <returns>The new instance.</returns>
        public static CommonSerializer Create()
        {
            return new CommonSerializer();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CommonSerializer" /> class.
        /// </summary>
        /// <param name="jsonSerializer">The json serializer to use.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="jsonSerializer" /> is <see langword="null" />.
        /// </exception>
        public static CommonSerializer Create(JsonNetSerializer jsonSerializer)
        {
            if (jsonSerializer == null)
            {
                throw new ArgumentNullException("jsonSerializer");
            }

            return new CommonSerializer((s) => jsonSerializer);
        }

        /// <summary>
        /// Creates a new <see cref="JsonSerializer" /> instance.
        /// </summary>
        /// <returns>The created instance.</returns>
        protected JsonSerializer CreateJsonSerializer()
        {
            var settings = this.GetJsonSerializerSettings();

            return settings == null ? JsonSerializer.Create()
                                    : JsonSerializer.Create(settings);
        }

        private JsonNetSerializer GetJsonNetSerializer(CommonSerializer serializer)
        {
            return JsonNetSerializer.Create();
        }

        private JsonSerializer GetJsonSerializer(JsonNetSerializer serializer, JsonNetSerializer.SerializationMode mode)
        {
            return this.CreateJsonSerializer();
        }

        /// <summary>
        /// Returns the settings for a new <see cref="JsonSerializer" /> instance.
        /// </summary>
        /// <returns>The settings or <see langword="null" /> to take the default one.</returns>
        protected virtual JsonSerializerSettings GetJsonSerializerSettings()
        {
            return null;
        }

        /// <inheriteddoc />
        protected override void OnFromJson<T>(string json, ref T deserializedObj)
        {
            deserializedObj = this._JSON_SERIALIZER_PROVIDER(this)
                                  .Deserialize<T>(strData: json);
        }

        /// <inheriteddoc />
        protected override void OnToJson<T>(T objToSerialize, ref StringBuilder jsonBuilder)
        {
            jsonBuilder.Append(this._JSON_SERIALIZER_PROVIDER(this)
                                   .Serialize<T>(objToSerialize));
        }

        #endregion Methods
    }
}