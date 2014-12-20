// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40)
#define KNOWS_DBNULL
#endif

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization.Json
{
    /// <summary>
    /// A serializer that is based on JSON.NET.
    /// </summary>
    public partial class JsonNetSerializer : StringSerializerBase
    {
        #region Fields (1)

        private readonly SerializerProvider _PROVIDER;

        #endregion Fields (1)

        #region Constructors (4)

        /// <inheriteddoc />
        public JsonNetSerializer(SerializerProvider provider,
                                 bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._PROVIDER = provider;
        }

        /// <inheriteddoc />
        public JsonNetSerializer(SerializerProvider provider,
                                 bool isSynchronized)
            : this(provider: provider,
                   isSynchronized: isSynchronized, sync: new object())
        {
        }

        /// <inheriteddoc />
        public JsonNetSerializer(SerializerProvider provider,
                                 object sync)
            : this(provider: provider,
                   isSynchronized: false, sync: sync)
        {
        }

        /// <inheriteddoc />
        public JsonNetSerializer(SerializerProvider provider)
            : this(provider: provider,
                   isSynchronized: false)
        {
        }

        #endregion Constructors

        #region Events and delegates (1)

        /// <summary>
        /// Delegates that provides the <see cref="JsonSerializer" /> instance to use.
        /// </summary>
        /// <param name="serializer">The underlying instance of that class.</param>
        /// <param name="mode">The current mode / context.</param>
        /// <returns>The instance.</returns>
        public delegate JsonSerializer SerializerProvider(JsonNetSerializer serializer, SerializationMode mode);

        #endregion

        #region Methods (8)

        /// <summary>
        /// Creates a new instance of the <see cref="JsonNetSerializer" /> class.
        /// </summary>
        public static JsonNetSerializer Create()
        {
            return Create(isSynchronized: false,
                          sync: new object());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="JsonNetSerializer" /> class.
        /// </summary>
        /// <param name="isSynchronized">Instance is thread safe or not.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public static JsonNetSerializer Create(bool isSynchronized, object sync)
        {
            return new JsonNetSerializer((s, m) => new JsonSerializer(),
                                         isSynchronized: isSynchronized, sync: sync);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="JsonNetSerializer" /> class.
        /// </summary>
        /// <param name="serializer">The serializer to use.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serializer" /> is <see langword="null" />.
        /// </exception>
        public static JsonNetSerializer Create(JsonSerializer serializer)
        {
            return Create(serializer,
                          isSynchronized: false, sync: new object());
        }

        /// <summary>
        /// Creates a new instance of the <see cref="JsonNetSerializer" /> class.
        /// </summary>
        /// <param name="serializer">The serializer to use.</param>
        /// <param name="isSynchronized">Instance is thread safe or not.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serializer" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static JsonNetSerializer Create(JsonSerializer serializer,
                                               bool isSynchronized, object sync)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            return new JsonNetSerializer((s, m) => serializer,
                                         isSynchronized: isSynchronized, sync: sync);
        }

        /// <inheriteddoc />
        protected override void OnCanDeserialize(string strData, Type deserializeAs, ref bool canDeserialize)
        {
            canDeserialize = true;
        }

        /// <inheriteddoc />
        protected override void OnCanSerialize(string strData, Type serializeAs, ref bool canSerialize)
        {
            canSerialize = true;
        }

        /// <inheriteddoc />
        protected override void OnDeserialize(string strData, Type deserializeAs, ref object obj)
        {
            if (string.IsNullOrWhiteSpace(strData) ||
                (strData.Trim() == "null"))
            {
                obj = null;
                return;
            }

            var serializer = this._PROVIDER(this, SerializationMode.Deserialize);

            using (var strReader = new StringReader(strData))
            {
                using (var jsonReader = new JsonTextReader(strReader))
                {
                    jsonReader.CloseInput = false;

                    if (deserializeAs.Equals(typeof(global::System.Collections.Generic.IDictionary<string, object>)))
                    {
                        deserializeAs = typeof(global::System.Dynamic.ExpandoObject);
                    }

                    obj = serializer.Deserialize(jsonReader, deserializeAs);
                }
            }
        }

        /// <inheriteddoc />
        protected override void OnSerialize(object obj, Type serializeAs, ref StringBuilder strData)
        {
#if KNOWS_DBNULL

            if (global::System.DBNull.Value.Equals(obj))
            {
                obj = null;
            }

#endif

            if (obj == null)
            {
                strData.Append("null");
                return;
            }

            var serializer = this._PROVIDER(this, SerializationMode.Serialize);

            using (var strWriter = new StringWriter(strData))
            {
                using (var jsonWriter = new JsonTextWriter(strWriter))
                {
                    jsonWriter.CloseOutput = false;

                    if (serializeAs.Equals(typeof(global::System.Dynamic.ExpandoObject)))
                    {
                        serializeAs = typeof(global::System.Collections.Generic.IDictionary<string, object>);
                    }

                    serializer.Serialize(jsonWriter, obj, serializeAs);
                }
            }
        }

        #endregion Methods (4)
    }
}