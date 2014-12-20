// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization
{
    /// <summary>
    /// A basic binary object serializer.
    /// </summary>
    public abstract class BinarySerializerBase : ObjectSerializerBase, IBinarySerializer
    {
        #region Fields (1)

        private readonly SerializeAction _ON_SERIALIZE_ACTION;

        #endregion Fields (1)

        #region Constructors (4)

        /// <inheriteddoc />
        protected BinarySerializerBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._ON_SERIALIZE_ACTION = this.OnSerialize_ThreadSafe;
            }
            else
            {
                this._ON_SERIALIZE_ACTION = this.OnSerialize;
            }
        }

        /// <inheriteddoc />
        protected BinarySerializerBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected BinarySerializerBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected BinarySerializerBase()
            : this(isSynchronized: false)
        {
        }

        #endregion Constructors

        #region Events and delegates (2)

        private delegate void SerializeAction(object obj, Type serializeAs, Stream target);

        #endregion

        #region Methods (20)

        /// <inheriteddoc />
        public bool CanDeserialize<T>(IEnumerable<byte> binData)
        {
            return this.CanDeserialize(binData, typeof(T));
        }

        /// <inheriteddoc />
        public bool CanDeserialize<T>(Stream src)
        {
            return this.CanDeserialize(src, typeof(T));
        }

        /// <inheriteddoc />
        public bool CanDeserialize(IEnumerable<byte> binData, Type deserializeAs)
        {
            using (var src = new MemoryStream(binData.AsArray()))
            {
                return this.CanDeserialize(src, deserializeAs);
            }
        }

        /// <inheriteddoc />
        public bool CanDeserialize(Stream src, Type deserializeAs)
        {
            // other (null) checks are done by
            // "CanDeserialize(object, Type)" method

            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            return this.CanDeserialize((object)src,
                                       deserializeAs);
        }

        /// <inheriteddoc />
        public T Deserialize<T>(IEnumerable<byte> binData)
        {
            return this.ChangeType<T>(this.Deserialize(binData, typeof(T)));
        }

        /// <inheriteddoc />
        public T Deserialize<T>(Stream src)
        {
            return this.ChangeType<T>(this.Deserialize(src, typeof(T)));
        }

        /// <inheriteddoc />
        public object Deserialize(IEnumerable<byte> binData, Type deserializeAs)
        {
            return this.Deserialize((object)binData, deserializeAs);
        }

        /// <inheriteddoc />
        public object Deserialize(Stream src, Type deserializeAs)
        {
            if (deserializeAs == null)
            {
                throw new ArgumentNullException("deserializeAs");
            }

            return this.Deserialize((object)src,
                                    deserializeAs);
        }

        /// <inheriteddoc />
        protected override sealed void OnCanDeserialize(object data, Type deserializeAs, ref bool canDeserialize)
        {
            bool dataIsStream;
            var src = ObjectToStream(data, out dataIsStream);
            try
            {
                if (src != null)
                {
                    this.OnCanDeserialize(src: src,
                                          deserializeAs: deserializeAs,
                                          canDeserialize: ref canDeserialize);
                }
                else
                {
                    canDeserialize = false;
                }
            }
            finally
            {
                if ((src != null) &&
                    (dataIsStream == false))
                {
                    src.Dispose();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ObjectSerializerBase.OnCanDeserialize(object, Type, ref bool)" />
        protected abstract void OnCanDeserialize(Stream src, Type deserializeAs, ref bool canDeserialize);

        /// <inheriteddoc />
        protected override sealed void OnDeserialize(object data, Type deserializeAs, ref object obj)
        {
            bool dataIsStream;
            var src = ObjectToStream(data, out dataIsStream);
            try
            {
                if (src == null)
                {
                    throw new ArgumentException("data");
                }

                if (src.CanRead == false)
                {
                    throw new IOException();
                }

                this.OnDeserialize(src, deserializeAs, ref obj);
            }
            finally
            {
                if ((src != null) &&
                    (dataIsStream == false))
                {
                    src.Dispose();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ObjectSerializerBase.OnDeserialize(object, Type, ref object)" />
        protected virtual void OnDeserialize(Stream src, Type deserializeAs, ref object obj)
        {
            throw new NotImplementedException();
        }

        /// <inheriteddoc />
        protected override sealed void OnSerialize(object obj, Type serializeAs, ref object data)
        {
            using (var target = new MemoryStream())
            {
                this.OnSerialize(obj, serializeAs, target);

                data = target.ToArray();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ObjectSerializerBase.OnSerialize(object, Type, ref object)" />
        protected virtual void OnSerialize(object obj, Type serializeAs, Stream target)
        {
            throw new NotImplementedException();
        }

        private void OnSerialize_ThreadSafe(object obj, Type serializeAs, Stream target)
        {
            lock (this._SYNC)
            {
                this.OnSerialize(obj, serializeAs, target);
            }
        }

        /// <inheriteddoc />
        public new byte[] Serialize<T>(T obj)
        {
            return (byte[])base.Serialize<T>(obj);
        }

        /// <inheriteddoc />
        public void Serialize<T>(T obj, Stream target)
        {
            this.Serialize(obj, typeof(T), target);
        }

        /// <inheriteddoc />
        public new byte[] Serialize(object obj, Type serializeAs)
        {
            return (byte[])base.Serialize(obj, serializeAs);
        }

        /// <inheriteddoc />
        public void Serialize(object obj, Type serializeAs, Stream target)
        {
            if (serializeAs == null)
            {
                throw new ArgumentNullException("serializeAs");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (target.CanWrite == false)
            {
                throw new IOException();
            }

            this._ON_SERIALIZE_ACTION(obj, serializeAs, target);
        }

        private static Stream ObjectToStream(object obj, out bool objIsStream)
        {
            objIsStream = false;

            var result = obj as Stream;
            if (result != null)
            {
                objIsStream = true;
            }
            else
            {
                var binData = obj as IEnumerable<byte>;
                if (binData != null)
                {
                    result = new MemoryStream(binData.AsArray());
                }
                else
                {
                    if ((obj is string) ||
                        (obj is StringBuilder) ||
                        (obj is IEnumerable<char>))
                    {
                        // handle as Base64 string

                        result = new MemoryStream(Convert.FromBase64String(obj.AsString()
                                                                              .Trim()));
                    }
                }
            }

            return result;
        }

        #endregion
    }
}