// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization
{
    /// <summary>
    /// A binary serializer that uses a <see cref="BinaryFormatter" /> instance.
    /// </summary>
    public sealed class BinaryFormatterSerializer : BinarySerializerBase
    {
        #region Fields (1)

        private readonly BinaryFormatter _FORMATTER;

        #endregion Fields (1)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatterSerializer" /> class.
        /// </summary>
        public BinaryFormatterSerializer()
            : this(new BinaryFormatter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatterSerializer" /> class.
        /// </summary>
        /// <param name="formatter">The formatter to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="formatter" /> is <see langword="null" />.
        /// </exception>
        public BinaryFormatterSerializer(BinaryFormatter formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException("formatter");
            }

            this._FORMATTER = formatter;
        }

        #endregion Constructors (2)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying formatter.
        /// </summary>
        public BinaryFormatter Formatter
        {
            get { return this._FORMATTER; }
        }

        #endregion Properties (1)

        #region Methods (4)

        /// <inheriteddoc />
        protected override void OnCanDeserialize(Stream src, Type deserializeAs, ref bool canDeserialize)
        {
            canDeserialize = true;
        }

        /// <inheriteddoc />
        protected override void OnCanSerialize(object obj, Type serializeAs, ref bool canSerialize)
        {
            canSerialize = true;
        }

        /// <inheriteddoc />
        protected override void OnDeserialize(Stream src, Type deserializeAs, ref object obj)
        {
            obj = this._FORMATTER
                      .Deserialize(src);
        }

        /// <inheriteddoc />
        protected override void OnSerialize(object obj, Type serializeAs, Stream target)
        {
            this._FORMATTER
                .Serialize(target, obj);
        }

        #endregion Methods (4)
    }
}