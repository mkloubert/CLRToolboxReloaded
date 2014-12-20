// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using System;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization
{
    /// <summary>
    /// A basic object serializer.
    /// </summary>
    public abstract class ObjectSerializerBase : ObjectBase, IObjectSerializer
    {
        #region Fields (4)

        private readonly CanPredicate _CAN_DESERIALIZE_PREDICATE;
        private readonly CanPredicate _CAN_SERIALIZE_PREDICATE;
        private readonly SerializerAction _DESERIALIZE_ACTION;
        private readonly SerializerAction _SERIALIZE_ACTION;

        #endregion Fields (4)

        #region Constructors (4)

        /// <inheriteddoc />
        protected ObjectSerializerBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._CAN_DESERIALIZE_PREDICATE = this.OnCanDeserialize_ThreadSafe;
                this._CAN_SERIALIZE_PREDICATE = this.OnCanSerialize_ThreadSafe;
                this._DESERIALIZE_ACTION = this.OnDeserialize_ThreadSafe;
                this._SERIALIZE_ACTION = this.OnSerialize_ThreadSafe;
            }
            else
            {
                this._CAN_DESERIALIZE_PREDICATE = this.OnCanDeserialize;
                this._CAN_SERIALIZE_PREDICATE = this.OnCanSerialize;
                this._DESERIALIZE_ACTION = this.OnDeserialize;
                this._SERIALIZE_ACTION = this.OnSerialize;
            }
        }

        /// <inheriteddoc />
        protected ObjectSerializerBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected ObjectSerializerBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected ObjectSerializerBase()
            : this(isSynchronized: false)
        {
        }

        #endregion Constructors

        #region Events and delegates (2)

        private delegate void CanPredicate(object input, Type type, ref bool result);

        private delegate void SerializerAction(object input, Type type, ref object target);

        #endregion

        #region Properties (1)

        /// <summary>
        /// Gets the converter to use.
        /// </summary>
        protected virtual IConverter Converter
        {
            get { return GlobalConverter.Current; }
        }

        #endregion

        #region Methods (17)

        /// <inheriteddoc />
        public bool CanDeserialize<T>(object data)
        {
            return this.CanDeserialize(data, typeof(T));
        }

        /// <inheriteddoc />
        public bool CanDeserialize(object data, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            if (this.CanDeserialize(data, targetType) == false)
            {
                throw new InvalidCastException();
            }

            var result = false;
            this._CAN_DESERIALIZE_PREDICATE(data, targetType, ref result);

            return result;
        }

        /// <inheriteddoc />
        public bool CanSerialize<T>(T obj)
        {
            return this.CanSerialize(obj, typeof(T));
        }

        /// <inheriteddoc />
        public bool CanSerialize(object obj, Type serializeAs)
        {
            if (serializeAs == null)
            {
                throw new ArgumentNullException("serializeAs");
            }

            if (this.CanSerialize(obj, serializeAs) == false)
            {
                throw new InvalidCastException();
            }

            var result = false;
            this._CAN_SERIALIZE_PREDICATE(obj, serializeAs, ref result);

            return result;
        }

        /// <summary>
        /// Converts an object to a specific data type.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="input">The input object.</param>
        /// <returns>The target value.</returns>
        protected virtual T ChangeType<T>(object input)
        {
            var converter = this.Converter;
            return converter != null ? converter.ChangeType<T>(input)
                                     : (T)input;
        }

        /// <inheriteddoc />
        public T Deserialize<T>(object data)
        {
            return this.ChangeType<T>(this.Deserialize(data, typeof(T)));
        }

        /// <inheriteddoc />
        public object Deserialize(object data, Type deserializeAs)
        {
            if (deserializeAs == null)
            {
                throw new ArgumentNullException("deserializeAs");
            }

            object result = null;
            this._DESERIALIZE_ACTION(data, deserializeAs, ref result);

            return result;
        }

        /// <summary>
        /// Stores the logic for the <see cref="ObjectSerializerBase.CanDeserialize(object, Type)" /> method.
        /// </summary>
        /// <param name="data">The data to check.</param>
        /// <param name="deserializeAs">The target type.</param>
        /// <param name="canDeserialize">The variable where to write if data can be deserialized or not.</param>
        protected abstract void OnCanDeserialize(object data, Type deserializeAs, ref bool canDeserialize);

        private void OnCanDeserialize_ThreadSafe(object data, Type deserializeAs, ref bool canDeserialize)
        {
            lock (this._SYNC)
            {
                this.OnCanDeserialize(data, deserializeAs, ref canDeserialize);
            }
        }

        /// <summary>
        /// Stores the logic for the <see cref="ObjectSerializerBase.CanSerialize(object, Type)" /> method.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <param name="serializeAs">The type the object should be serialized as.</param>
        /// <param name="canSerialize">The variable where to write if object can be serialized or not.</param>
        protected abstract void OnCanSerialize(object obj, Type serializeAs, ref bool canSerialize);

        private void OnCanSerialize_ThreadSafe(object obj, Type serializeAs, ref bool canSerialize)
        {
            lock (this._SYNC)
            {
                this.OnCanSerialize(obj, serializeAs, ref canSerialize);
            }
        }

        /// <summary>
        /// Stores the logic for the <see cref="ObjectSerializerBase.Deserialize(object, Type)" /> method.
        /// </summary>
        /// <param name="data">The data to deserialize.</param>
        /// <param name="deserializeAs">The target type.</param>
        /// <param name="obj">The variable where to write the deserialized object to.</param>
        protected virtual void OnDeserialize(object data, Type deserializeAs, ref object obj)
        {
            throw new NotImplementedException();
        }

        private void OnDeserialize_ThreadSafe(object data, Type deserializeAs, ref object obj)
        {
            lock (this._SYNC)
            {
                this.OnDeserialize(data, deserializeAs, ref obj);
            }
        }

        /// <summary>
        /// Stores the logic for the <see cref="ObjectSerializerBase.Serialize(object, Type)" /> method.
        /// </summary>
        /// <param name="obj">The obj to serialize.</param>
        /// <param name="serializeAs">The type the object should be serialized as.</param>
        /// <param name="data">The variable where to write the serialized data to.</param>
        protected virtual void OnSerialize(object obj, Type serializeAs, ref object data)
        {
            throw new NotImplementedException();
        }

        private void OnSerialize_ThreadSafe(object obj, Type serializeAs, ref object data)
        {
            lock (this._SYNC)
            {
                this.OnSerialize(obj, serializeAs, ref data);
            }
        }

        /// <inheriteddoc />
        public object Serialize<T>(T obj)
        {
            return this.Serialize(obj, typeof(T));
        }

        /// <inheriteddoc />
        public object Serialize(object obj, Type serializeAs)
        {
            if (serializeAs == null)
            {
                throw new ArgumentNullException("serializeAs");
            }

            object result = null;
            this._SERIALIZE_ACTION(obj, serializeAs, ref result);

            return result;
        }

        #endregion Methods (8)
    }
}