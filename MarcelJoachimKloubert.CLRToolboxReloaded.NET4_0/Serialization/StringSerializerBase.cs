// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Serialization
{
    /// <summary>
    /// A serializer that uses string data to serialize and deserialize.
    /// </summary>
    public abstract class StringSerializerBase : ObjectSerializerBase, IStringSerializer
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected StringSerializerBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected StringSerializerBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected StringSerializerBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected StringSerializerBase()
            : base()
        {
        }

        #endregion Constructors

        #region Methods (14)

        /// <inheriteddoc />
        public bool CanDeserialize<T>(string strData)
        {
            return this.CanDeserialize<T>((object)strData);
        }

        /// <inheriteddoc />
        public bool CanDeserialize(string strData, Type targetType)
        {
            return this.CanDeserialize((object)strData, targetType);
        }

        /// <inheriteddoc />
        public T Deserialize<T>(string strData)
        {
            return this.Deserialize<T>((object)strData);
        }

        /// <inheriteddoc />
        public object Deserialize(string strData, Type deserializeAs)
        {
            return this.Deserialize((object)strData, deserializeAs);
        }

        /// <inheriteddoc />
        protected override sealed void OnCanDeserialize(object data, Type deserializeAs, ref bool canDeserialize)
        {
            this.OnCanDeserialize(strData: data.AsString(),
                                  deserializeAs: deserializeAs,
                                  canDeserialize: ref canDeserialize);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ObjectSerializerBase.OnCanDeserialize(object, Type, ref bool)" />
        protected abstract void OnCanDeserialize(string strData, Type deserializeAs, ref bool canDeserialize);

        /// <inheriteddoc />
        protected override sealed void OnCanSerialize(object data, Type serializeAs, ref bool canSerialize)
        {
            this.OnCanSerialize(strData: data.AsString(),
                                serializeAs: serializeAs,
                                canSerialize: ref canSerialize);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ObjectSerializerBase.OnCanDeserialize(object, Type, ref bool)" />
        protected abstract void OnCanSerialize(string strData, Type serializeAs, ref bool canSerialize);

        /// <inheriteddoc />
        protected override sealed void OnDeserialize(object data, Type deserializeAs, ref object obj)
        {
            this.OnDeserialize(strData: data.AsString(),
                               deserializeAs: deserializeAs,
                               obj: ref obj);
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ObjectSerializerBase.OnDeserialize(object, Type, ref object)" />
        protected virtual void OnDeserialize(string strData, Type deserializeAs, ref object obj)
        {
            throw new NotImplementedException();
        }

        /// <inheriteddoc />
        protected override sealed void OnSerialize(object obj, Type serializeAs, ref object data)
        {
            var strData = new StringBuilder();
            this.OnSerialize(obj: obj,
                             serializeAs: serializeAs,
                             strData: ref strData);

            data = strData != null ? strData.ToString()
                                   : null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <see cref="ObjectSerializerBase.OnSerialize(object, Type, ref object)" />
        protected virtual void OnSerialize(object obj, Type serializeAs, ref StringBuilder strData)
        {
            throw new NotImplementedException();
        }

        /// <inheriteddoc />
        public new string Serialize<T>(T obj)
        {
            return base.Serialize<T>(obj)
                       .AsString();
        }

        /// <inheriteddoc />
        public new string Serialize(object obj, Type serializeAs)
        {
            return base.Serialize(obj, serializeAs)
                       .AsString();
        }

        #endregion
    }
}