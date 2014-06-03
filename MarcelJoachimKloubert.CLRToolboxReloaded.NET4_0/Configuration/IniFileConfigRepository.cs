// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Configuration
{
    /// <summary>
    /// A config repository based on an INI file.
    /// </summary>
    public class IniFileConfigRepository : KeyValuePairConfigRepository
    {
        #region Fields (3)

        private readonly bool _CAN_WRITE;
        private readonly string _FILE_PATH;
        private static readonly string _TEMP_CHAR = ((char)1).ToString();

        #endregion Fields (3)

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFileConfigRepository"/> class.
        /// </summary>
        /// <param name="filePath">The path of the INI file.</param>
        /// <param name="isReadOnly">Repository is readonly or writable.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filePath" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="filePath" /> is invalid.</exception>
        public IniFileConfigRepository(IEnumerable<char> filePath, bool isReadOnly)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }

            var fp = filePath.AsString();
            if (string.IsNullOrWhiteSpace(fp))
            {
                throw new ArgumentException("filePath");
            }

            this._CAN_WRITE = isReadOnly == false;

            this._FILE_PATH = global::System.IO.Path.GetFullPath(fp);
            if (global::System.IO.File.Exists(this._FILE_PATH))
            {
                this.LoadIniFile();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFileConfigRepository"/> class.
        /// </summary>
        /// <param name="filePath">The path of the INI file.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filePath" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="filePath" /> is invalid.</exception>
        /// <remarks>Repository becomes readonly.</remarks>
        public IniFileConfigRepository(IEnumerable<char> filePath)
            : this(filePath, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFileConfigRepository"/> class.
        /// </summary>
        /// <param name="file">The INI file.</param>
        /// <param name="isReadOnly">Repository is readonly or writable.</param>
        /// <exception cref="NullReferenceException"><paramref name="file" /> is <see langword="null" />.</exception>
        public IniFileConfigRepository(global::System.IO.FileInfo file, bool isReadOnly)
            : this(file.FullName, isReadOnly)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFileConfigRepository"/> class.
        /// </summary>
        /// <param name="file">The INI file.</param>
        /// <exception cref="NullReferenceException"><paramref name="file" /> is <see langword="null" />.</exception>
        /// <remarks>Repository becomes readonly.</remarks>
        public IniFileConfigRepository(global::System.IO.FileInfo file)
            : this(file, true)
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

        #region Methods (11)

        // Protected Methods (10) 

        /// <summary>
        /// Converts back a value for use in that object as value.
        /// </summary>
        /// <param name="input">The input value to convert.</param>
        /// <returns>The converted value.</returns>
        protected virtual IEnumerable<char> FromIniSectionValue(string input)
        {
            var result = (input ?? string.Empty).Replace("\\\\", _TEMP_CHAR);

            result = result.Replace("\\n", "\n")
                           .Replace("\\r", "\r")
                           .Replace("\\0", "\0")
                           .Replace("\\a", "\a")
                           .Replace("\\b", "\b")
                           .Replace("\\t", "\t")
                           .Replace("\\;", ";")
                           .Replace("\\#", "#")
                           .Replace("\\=", "=")
                           .Replace("\\:", ":");

            result = result.Replace(_TEMP_CHAR, "\\");

            return (result != string.Empty ? result : null).AsChars();
        }

        /// <summary>
        /// Gets the encoding for the INI file.
        /// </summary>
        /// <returns>The INI file encoding.</returns>
        protected virtual Encoding GetEncoding()
        {
            return Encoding.UTF8;
        }

        /// <inheriteddoc />
        protected override void OnSetValue<T>(string category, string name, T value, ref bool valueWasSet, bool invokeOnUpdated)
        {
            var strValue = value.AsString();
            if (string.IsNullOrEmpty(strValue))
            {
                strValue = null;
            }

            base.OnSetValue<string>(category,
                                    name,
                                    strValue,
                                    ref valueWasSet,
                                    invokeOnUpdated);
        }

        /// <inheriteddoc />
        protected override void OnTryGetValue<T>(string category, string name, ref T foundValue, ref bool valueWasFound)
        {
            var targetType = typeof(T);

            IEnumerable<char> innerValue = null;
            base.OnTryGetValue<IEnumerable<char>>(category, name,
                                                  ref innerValue, ref valueWasFound);

            if (valueWasFound == false)
            {
                return;
            }

            var throwException = false;
            var strValue = innerValue.AsString();
            object valueToReturn = null;

            if (strValue != null)
            {
                throwException = true;

                if (targetType.Equals(typeof(bool)) ||
                    targetType.Equals(typeof(bool?)))
                {
                    switch (strValue.ToLower().Trim())
                    {
                        case "0":
                        case "no":
                        case "false":
                            valueToReturn = false;
                            throwException = false;
                            break;

                        case "1":
                        case "yes":
                        case "true":
                            valueToReturn = true;
                            throwException = false;
                            break;

                        case "":
                            if (Nullable.GetUnderlyingType(targetType) != null)
                            {
                                // nullable bool
                                throwException = false;
                            }
                            break;
                    }
                }
                else if (Nullable.GetUnderlyingType(targetType) != null)
                {
                    // nullable struct

                    if (string.IsNullOrWhiteSpace(strValue) == false)
                    {
                        valueToReturn = strValue;
                        targetType = Nullable.GetUnderlyingType(targetType);
                    }

                    throwException = false;
                }
                else if (targetType.Equals(typeof(string)) ||
                         targetType.Equals(typeof(global::System.Collections.Generic.IEnumerable<char>)) ||
                         targetType.Equals(typeof(object)))
                {
                    // default: string

                    valueToReturn = strValue;
                    throwException = false;
                }
            }

            if (throwException)
            {
                throw new InvalidCastException();
            }

            foundValue = (T)GlobalConverter.Current
                                           .ChangeType(targetType, valueToReturn);
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
                    foreach (var categoryValues in this._VALUES)
                    {
                        // create section
                        writer.WriteLine("[{0}]",
                                         this.ParseIniSectionName(categoryValues.Key)
                                             .AsString());

                        // writes values
                        categoryValues.Value
                                      .ForEach(ctx =>
                                      {
                                          var repo = ctx.State.Repo;

                                          ctx.State
                                             .Writer.WriteLine(string.Format("{0}={1}",
                                                                             repo.ParseIniSectionKey(ctx.Item.Key).AsString(),
                                                                             repo.ToIniSectionValue(ctx.Item.Value).AsString()));
                                      }, actionState: new
                                      {
                                          Repo = this,
                                          Writer = writer,
                                      });

                        writer.WriteLine();
                    }

                    writer.Flush();

                    var iniFile = new FileInfo(this._FILE_PATH);
                    if (iniFile.Exists)
                    {
                        iniFile.Delete();
                        iniFile.Refresh();
                    }

                    temp.Position = 0;
                    using (var iniFileStream = iniFile.OpenWrite())
                    {
                        temp.CopyTo(iniFileStream);
                    }
                }
            }
        }

        /// <summary>
        /// Parses back the name of an INI section key for use in this object.
        /// </summary>
        /// <param name="input">The input expression.</param>
        /// <returns>The parsed name of the section key.</returns>
        protected virtual IEnumerable<char> ParseBackIniSectionKey(string input)
        {
            return (input ?? string.Empty).Replace("\\=", "=")
                                          .Trim()
                                          .AsChars();
        }

        /// <summary>
        /// Parses back the name of an INI section for use in this object.
        /// </summary>
        /// <param name="input">The input expression.</param>
        /// <returns>The parsed name of the section.</returns>
        protected virtual IEnumerable<char> ParseBackIniSectionName(string input)
        {
            return (input ?? string.Empty).Replace("\\[", "[")
                                          .Replace("\\]", "]")
                                          .Trim()
                                          .AsChars();
        }

        /// <summary>
        /// Parses the name of a section key for use in an INI file.
        /// </summary>
        /// <param name="input">The input expression.</param>
        /// <returns>The parsed name of the section key.</returns>
        protected virtual IEnumerable<char> ParseIniSectionKey(string input)
        {
            return (input ?? string.Empty).Replace("=", "\\=")
                                          .Trim()
                                          .AsChars();
        }

        /// <summary>
        /// Parses the name of a section for use in an INI file.
        /// </summary>
        /// <param name="input">The input expression.</param>
        /// <returns>The parsed name of the section.</returns>
        protected virtual IEnumerable<char> ParseIniSectionName(string input)
        {
            return (input ?? string.Empty).Replace("[", "\\[")
                                          .Replace("]", "\\]")
                                          .Trim()
                                          .AsChars();
        }

        /// <summary>
        /// Converts a value for use in an INI file as section value.
        /// </summary>
        /// <param name="input">The input value to convert.</param>
        /// <returns>The converted value.</returns>
        protected virtual IEnumerable<char> ToIniSectionValue(object input)
        {
            return (input.AsString() ?? string.Empty).Replace("\\", "\\\\")
                                                     .Replace("\n", "\\n")
                                                     .Replace("\r", "\\r")
                                                     .Replace("\0", "\\0")
                                                     .Replace("\a", "\\a")
                                                     .Replace("\b", "\\b")
                                                     .Replace(";", "\\;")
                                                     .Replace("#", "\\#")
                                                     .Replace("=", "\\=")
                                                     .Replace(":", "\\:")
                                                     .Replace("\t", "\\t")
                                                     .AsChars();
        }

        // Private Methods (1) 

        private void LoadIniFile()
        {
            lock (this._SYNC)
            {
                using (var iniFile = File.OpenRead(this._FILE_PATH))
                {
                    using (var reader = new StreamReader(iniFile, this.GetEncoding()))
                    {
                        string currentSection = null;

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.TrimStart();
                            if (line == string.Empty)
                            {
                                // empty line
                                continue;
                            }

                            if (line[0] == ';' ||
                                line[0] == '#')
                            {
                                // comment
                                continue;
                            }

                            if (line[0] == '[')
                            {
                                var section = line.TrimEnd();
                                if (section[section.Length - 1] == ']')
                                {
                                    // (new) section started
                                    currentSection = section.Substring(1, section.Length - 2).Trim();
                                    continue;
                                }
                            }

                            if (currentSection == null)
                            {
                                // no section defined
                                continue;
                            }

                            string name;
                            string value;

                            var equalCharIndex = line.IndexOf('=');
                            if (equalCharIndex > -1)
                            {
                                name = line.Substring(0, equalCharIndex);
                                value = line.Substring(equalCharIndex + 1,
                                                       line.Length - equalCharIndex - 1);
                            }
                            else
                            {
                                // no value defined

                                name = line.TrimEnd();
                                value = null;
                            }

                            if (value != null)
                            {
                                // extract until comment

                                var sharpIndex = value.IndexOf('#');
                                if ((sharpIndex > 0) &&
                                    (value[sharpIndex - 1] != '\\'))
                                {
                                    value = value.Substring(0, sharpIndex);
                                }

                                var semicolonIndex = value.IndexOf(';');
                                if ((semicolonIndex > 0) &&
                                    (value[semicolonIndex - 1] != '\\'))
                                {
                                    value = value.Substring(0, semicolonIndex);
                                }
                            }

                            string configCat;
                            string configName;
                            this.PrepareCategoryAndName(this.ParseBackIniSectionKey(currentSection), this.ParseBackIniSectionName(name),
                                                        out configCat, out configName);

                            var valueWasSet = false;
                            this.OnSetValue<string>(category: configCat,
                                                    name: configName,
                                                    value: this.FromIniSectionValue(value).AsString(),
                                                    valueWasSet: ref valueWasSet,
                                                    invokeOnUpdated: false);
                        }
                    }
                }
            }
        }

        #endregion Methods
    }
}