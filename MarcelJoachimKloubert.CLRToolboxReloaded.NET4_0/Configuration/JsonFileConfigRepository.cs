// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Configuration
{
    /// <summary>
    /// A config repository based on a JSON file.
    /// </summary>
    public class JsonFileConfigRepository : KeyValuePairConfigRepository
    {
        #region Fields (2)

        private readonly bool _CAN_WRITE;
        private readonly string _FILE_PATH;

        #endregion Fields

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFileConfigRepository"/> class.
        /// </summary>
        /// <param name="filePath">The path of the INI file.</param>
        /// <param name="isReadOnly">Repository is readonly or writable.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filePath" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="filePath" /> is invalid.</exception>
        public JsonFileConfigRepository(string filePath, bool isReadOnly = true)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("filePath");
            }

            this._CAN_WRITE = isReadOnly == false;

            this._FILE_PATH = Path.GetFullPath(filePath);
            if (File.Exists(this._FILE_PATH))
            {
                this.LoadJsonFile();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFileConfigRepository"/> class.
        /// </summary>
        /// <param name="file">The INI file.</param>
        /// <param name="isReadOnly">Repository is readonly or writable.</param>
        /// <exception cref="NullReferenceException"><paramref name="file" /> is <see langword="null" />.</exception>
        public JsonFileConfigRepository(FileInfo file, bool isReadOnly = true)
            : this(file.FullName, isReadOnly)
        {
        }

        #endregion Constructors

        #region Properties (2)

        /// <inheriteddoc />
        public override bool CanWrite
        {
            get { return this._CAN_WRITE; }
        }

        /// <summary>
        /// Gets the full path of the underlying INI file.
        /// </summary>
        public string FilePath
        {
            get { return this._FILE_PATH; }
        }

        #endregion Properties

        #region Methods (8)

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
        /// Gets the encoding for the INI file.
        /// </summary>
        /// <returns>The INI file encoding.</returns>
        protected virtual Encoding GetEncoding()
        {
            return Encoding.UTF8;
        }

        /// <summary>
        /// Returns the settings for a new <see cref="JsonSerializer" /> instance.
        /// </summary>
        /// <returns>The settings or <see langword="null" /> to take the default one.</returns>
        protected virtual JsonSerializerSettings GetJsonSerializerSettings()
        {
            return null;
        }

        private void LoadJsonFile()
        {
            lock (this._SYNC)
            {
                using (var jsonFile = File.OpenRead(this._FILE_PATH))
                {
                    using (var reader = new StreamReader(jsonFile, this.GetEncoding()))
                    {
                        using (var jsonReader = new JsonTextReader(reader))
                        {
                            var serializer = this.CreateJsonSerializer();

                            this._VALUES.Clear();
                            {
                                var values = serializer.Deserialize<IDictionary<string, object>>(jsonReader);
                                if (values != null)
                                {
                                    values.ForEach((ctx) =>
                                    {
                                        var repo = ctx.State.Repo;
                                        var category = ctx.Item.Key;

                                        var jObj = (JObject)ctx.Item.Value;
                                        repo._VALUES[category ?? string.Empty] = this.ToDictionary(jObj);
                                    }, actionState: new
                                    {
                                        Repo = this,
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <inheriteddoc />
        protected override void OnUpdated(UpdateContext context, string category, string name)
        {
            base.OnUpdated(context: context,
                           category: category, name: name);

            using (var temp = new MemoryStream())
            {
                using (var writer = new StreamWriter(temp, this.GetEncoding()))
                {
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        var serializer = this.CreateJsonSerializer();

                        serializer.Serialize(jsonWriter,
                                             this._VALUES,
                                             typeof(global::System.Collections.Generic.IDictionary<string, object>));

                        jsonWriter.Flush();

                        var jsonFile = new FileInfo(this._FILE_PATH);
                        if (jsonFile.Exists)
                        {
                            jsonFile.Delete();
                            jsonFile.Refresh();
                        }

                        temp.Position = 0;
                        using (var jsonFileStream = jsonFile.OpenWrite())
                        {
                            temp.CopyTo(jsonFileStream);
                        }
                    }
                }
            }
        }

        private object[] ToArray(JArray arr,
                                 int level = 0,
                                 int maxLevel = 64)
        {
            if (arr == null)
            {
                return null;
            }

            if (level > maxLevel)
            {
                // maximum reached

                return arr.Cast<object>()
                          .ToArray();
            }

            var result = new object[arr.Count];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = this.ToClrObject(arr[i]);
            }

            return result;
        }

        private object ToClrObject(object obj,
                                   int level = 0,
                                   int maxLevel = 64)
        {
            if (obj.IsNull())
            {
                return null;
            }

            if (level > maxLevel)
            {
                // maximum reached
                return obj;
            }

            if (obj is JArray)
            {
                return this.ToArray((JArray)obj);
            }

            if (obj is JObject)
            {
                return this.ToDictionary((JObject)obj);
            }

            // this check MUST BE AT THE END of that method!
            if (obj is JToken)
            {
                return this.ToClrObject(((JToken)obj).ToObject<object>(),
                                        level: level + 1,
                                        maxLevel: maxLevel);
            }

            return obj;
        }

        private IDictionary<string, object> ToDictionary(JObject obj,
                                                         int level = 0,
                                                         int maxLevel = 64)
        {
            if (obj == null)
            {
                return null;
            }

            var result = new Dictionary<string, object>();

            if (level <= maxLevel)
            {
                obj.Properties().ForEach((ctx) =>
                    {
                        var property = ctx.Item;
                        var provider = ctx.State.Provider;

                        string cat;
                        string name;
                        provider.PrepareCategoryAndName(null, property.Name,
                                                        out cat, out name);

                        ctx.State.Result.Add(name ?? string.Empty,
                                             provider.ToClrObject(property.Value));
                    }, actionState: new
                    {
                        Provider = this,
                        Result = result,
                    });
            }
            else
            {
                // maximum level reached

                using (var e = obj.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        var i = e.Current;

                        result.Add(i.Key ?? string.Empty,
                                   i.Value);
                    }
                }
            }

            return result;
        }

        #endregion Methods
    }
}