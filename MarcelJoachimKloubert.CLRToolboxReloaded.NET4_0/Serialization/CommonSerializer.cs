// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization
{
    /// <summary>
    /// A common serializer.
    /// </summary>
    public class CommonSerializer : SerializerBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerBase" /> class.
        /// </summary>
        public CommonSerializer()
            : base(synchronized: false)
        {
        }

        #endregion Constructors

        #region Methods (4)

        /// <summary>
        /// Creates a new <see cref="JsonSerializer" /> instance.
        /// </summary>
        /// <returns>The created instance.</returns>
        protected JsonSerializer CreateJsonSerializer()
        {
            var settings = this.GetJsonSerializerSettings();

            return settings == null ? JsonSerializer.Create() : JsonSerializer.Create(settings);
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
            var serializer = this.CreateJsonSerializer();

            using (var strReader = new StringReader(json))
            {
                using (var jsonReader = new JsonTextReader(strReader))
                {
                    var deserializesAs = typeof(T);

                    if (typeof(T).Equals(typeof(global::System.Collections.Generic.IDictionary<string, object>)))
                    {
                        deserializesAs = typeof(global::System.Dynamic.ExpandoObject);
                    }

                    deserializedObj = GlobalConverter.Current
                                                     .ChangeType<T>(serializer.Deserialize(jsonReader, deserializesAs));
                }
            }
        }

        /// <inheriteddoc />
        protected override void OnToJson<T>(T objToSerialize, ref StringBuilder jsonBuilder)
        {
            var serializer = this.CreateJsonSerializer();

            using (var strWriter = new StringWriter(jsonBuilder))
            {
                using (var jsonWriter = new JsonTextWriter(strWriter))
                {
                    var serializesAs = typeof(T);

                    if (typeof(T).Equals(typeof(global::System.Dynamic.ExpandoObject)))
                    {
                        serializesAs = typeof(global::System.Collections.Generic.IDictionary<string, object>);
                    }

                    serializer.Serialize(jsonWriter, objToSerialize, serializesAs);
                }
            }
        }

        #endregion Methods
    }
}