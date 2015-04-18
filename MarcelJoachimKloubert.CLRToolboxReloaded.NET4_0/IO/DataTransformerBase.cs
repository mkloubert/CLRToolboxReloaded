// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE || PORTABLE40 || PORTABLE45)
#define KNOWS_SECURE_STRING
#endif

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// A basic object that transforms data.
    /// </summary>
    public abstract class DataTransformerBase : ObjectBase, IDataTransformer
    {
        #region Fields (2)

        private readonly Action<Stream, Stream, int?> _RESTORE_DATA_ACTION;
        private readonly Action<Stream, Stream, int?> _TRANSFORM_DATA_ACTION;

        #endregion Fields (2)

        #region Constructors (4)

        /// <inheriteddoc />
        protected DataTransformerBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._RESTORE_DATA_ACTION = this.OnRestoreData_ThreadSafe;
                this._TRANSFORM_DATA_ACTION = this.OnTransformData_ThreadSafe;
            }
            else
            {
                this._RESTORE_DATA_ACTION = this.OnRestoreData;
                this._TRANSFORM_DATA_ACTION = this.OnTransformData;
            }
        }

        /// <inheriteddoc />
        protected DataTransformerBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected DataTransformerBase(object sync)
            : this(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected DataTransformerBase()
            : this(isSynchronized: false)
        {
        }

        #endregion Constructors (4)

        #region Properties (2)

        /// <inheriteddoc />
        public abstract bool CanRestoreData
        {
            get;
        }

        /// <inheriteddoc />
        public abstract bool CanTransformData
        {
            get;
        }

        #endregion Properties (2)

        #region Methods (43)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="Stream.CopyTo(Stream, int)" />
        protected void CopyData(Stream src, Stream dest, int? bufferSize = null)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            if (bufferSize.HasValue)
            {
                src.CopyTo(dest, bufferSize.Value);
            }
            else
            {
                src.CopyTo(dest);
            }
        }

        /// <summary>
        /// Destorys the data of a temporary byte array.
        /// </summary>
        /// <param name="array">The array to destroy.</param>
        protected virtual void DestroyTempByteArray(byte[] array)
        {
            // dummy
        }

        /// <summary>
        /// Destroys a temporary stream.
        /// </summary>
        /// <param name="stream">The stream to destroy.</param>
        protected virtual void DestroyTempStream(Stream stream)
        {
            // dummy
        }

        /// <summary>
        /// Destroys a temprary <see cref="StringBuilder" /> instance.
        /// </summary>
        /// <param name="builder">The builder to destroy.</param>
        protected virtual void DestroyTempStringBuilder(StringBuilder builder)
        {
            // dummy
        }

        /// <summary>
        /// Stores the logic for the <see cref="DataTransformerBase.RestoreData(Stream, Stream, int?)" /> method.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        protected virtual void OnRestoreData(Stream src, Stream dest, int? bufferSize)
        {
            throw new NotImplementedException();
        }

        private void OnRestoreData_ThreadSafe(Stream src, Stream dest, int? bufferSize)
        {
            lock (this._SYNC)
            {
                this.OnRestoreData(src, dest, bufferSize);
            }
        }

        /// <summary>
        /// Stores the logic for the <see cref="DataTransformerBase.TransformData(Stream, Stream, int?)" /> method.
        /// </summary>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer size to use to read <paramref name="src" />.
        /// </param>
        protected virtual void OnTransformData(Stream src, Stream dest, int? bufferSize)
        {
            throw new NotImplementedException();
        }

        private void OnTransformData_ThreadSafe(Stream src, Stream dest, int? bufferSize)
        {
            this.OnTransformData(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        public byte[] RestoreData(IEnumerable<byte> blob)
        {
            using (var src = new MemoryStream(blob.AsArray()))
            {
                try
                {
                    return this.RestoreData(src);
                }
                finally
                {
                    this.DestroyTempStream(src);
                }
            }
        }

        /// <inheriteddoc />
        public void RestoreData(IEnumerable<byte> blob, Stream dest)
        {
            using (var src = new MemoryStream(blob.AsArray()))
            {
                try
                {
                    this.RestoreData(src, dest);
                }
                finally
                {
                    this.DestroyTempStream(src);
                }
            }
        }

        /// <inheriteddoc />
        public byte[] RestoreData(Stream src, int? bufferSize = null)
        {
            using (var dest = new MemoryStream())
            {
                try
                {
                    this.RestoreData(src, dest, bufferSize);

                    return dest.ToArray();
                }
                finally
                {
                    this.DestroyTempStream(dest);
                }
            }
        }

        /// <inheriteddoc />
        public void RestoreData(Stream src, Stream dest, int? bufferSize = null)
        {
            if (this.CanRestoreData == false)
            {
                throw new NotSupportedException();
            }

            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            if (src.CanRead == false)
            {
                throw new IOException("src");
            }

            if (dest.CanWrite == false)
            {
                throw new IOException("dest");
            }

            if (bufferSize < 1)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            this._RESTORE_DATA_ACTION(src, dest, bufferSize);
        }

#if KNOWS_SECURE_STRING

        /// <inheriteddoc />
        public global::System.Security.SecureString RestoreSecureString(global::System.IO.Stream src, int? bufferSize = null)
        {
            return this.RestoreSecureString(src, global::System.Text.Encoding.UTF8, bufferSize);
        }

        /// <inheriteddoc />
        public global::System.Security.SecureString RestoreSecureString(global::System.IO.Stream src, global::System.Text.Encoding enc, int? bufferSize = null)
        {
            // other checks are done by 'RestoreData(Stream, int?)'
            // and ToSecureString(byte[], Encoding) methods

            var restored = this.RestoreData(src, bufferSize);
            try
            {
                return ToSecureString(restored, enc);
            }
            finally
            {
                this.DestroyTempByteArray(restored);
            }
        }

        /// <inheriteddoc />
        public global::System.Security.SecureString RestoreSecureString(global::System.Collections.Generic.IEnumerable<byte> src)
        {
            return this.RestoreSecureString(src, global::System.Text.Encoding.UTF8);
        }

        /// <inheriteddoc />
        public global::System.Security.SecureString RestoreSecureString(global::System.Collections.Generic.IEnumerable<byte> src, global::System.Text.Encoding enc)
        {
            // other checks are done by 'RestoreData(IEnumerable<byte>)'
            // and ToSecureString(byte[], Encoding) methods

            var restored = this.RestoreData(src);
            try
            {
                return ToSecureString(restored, enc);
            }
            finally
            {
                this.DestroyTempByteArray(restored);
            }
        }

#endif

        /// <inheriteddoc />
        public string RestoreString(IEnumerable<byte> blob)
        {
            return this.RestoreString(blob, Encoding.UTF8);
        }

        /// <inheriteddoc />
        public string RestoreString(IEnumerable<byte> blob, Encoding enc)
        {
            using (var src = new MemoryStream(blob.AsArray()))
            {
                try
                {
                    return this.RestoreString(src, enc);
                }
                finally
                {
                    this.DestroyTempStream(src);
                }
            }
        }

        /// <inheriteddoc />
        public void RestoreString(IEnumerable<byte> blob, StringBuilder builder)
        {
            this.RestoreString(blob, builder, Encoding.UTF8);
        }

        /// <inheriteddoc />
        public void RestoreString(IEnumerable<byte> blob, StringBuilder builder, Encoding enc)
        {
            using (var src = new MemoryStream(blob.AsArray()))
            {
                try
                {
                    this.RestoreString(src, builder, enc);
                }
                finally
                {
                    this.DestroyTempStream(src);
                }
            }
        }

        /// <inheriteddoc />
        public void RestoreString(IEnumerable<byte> blob, TextWriter writer)
        {
            this.RestoreString(blob, writer, Encoding.UTF8);
        }

        /// <inheriteddoc />
        public void RestoreString(IEnumerable<byte> blob, TextWriter writer, Encoding enc)
        {
            writer.Write(this.RestoreString(blob, enc));
        }

        /// <inheriteddoc />
        public string RestoreString(Stream src, int? bufferSize = null)
        {
            return this.RestoreString(src, Encoding.UTF8, bufferSize);
        }

        /// <inheriteddoc />
        public string RestoreString(Stream src, Encoding enc, int? bufferSize = null)
        {
            if (enc == null)
            {
                throw new ArgumentNullException("enc");
            }

            using (var dest = new MemoryStream())
            {
                try
                {
                    this.RestoreData(src, dest, bufferSize);

                    var restored = dest.ToArray();
                    try
                    {
                        var builder = new StringBuilder(enc.GetString(restored, 0, restored.Length));
                        try
                        {
                            this.UnsaltString(builder, enc);

                            return builder.ToString();
                        }
                        finally
                        {
                            this.DestroyTempStringBuilder(builder);
                        }
                    }
                    finally
                    {
                        this.DestroyTempByteArray(restored);
                    }
                }
                finally
                {
                    this.DestroyTempStream(dest);
                }
            }
        }

        /// <inheriteddoc />
        public void RestoreString(Stream src, StringBuilder builder, int? bufferSize = null)
        {
            this.RestoreString(src, builder, Encoding.UTF8, bufferSize);
        }

        /// <inheriteddoc />
        public void RestoreString(Stream src, StringBuilder builder, Encoding enc, int? bufferSize = null)
        {
            using (var writer = new StringWriter(builder))
            {
                this.RestoreString(src, writer, enc, bufferSize);
            }
        }

        /// <inheriteddoc />
        public void RestoreString(Stream src, TextWriter writer, int? bufferSize = null)
        {
            this.RestoreString(src, writer, Encoding.UTF8, bufferSize);
        }

        /// <inheriteddoc />
        public void RestoreString(Stream src, TextWriter writer, Encoding enc, int? bufferSize = null)
        {
            writer.Write(this.RestoreString(src, enc, bufferSize));
        }

#if KNOWS_SECURE_STRING

        private static global::System.Security.SecureString ToSecureString(byte[] strData, global::System.Text.Encoding enc)
        {
            if (enc == null)
            {
                throw new global::System.ArgumentNullException("enc");
            }

            string str;
            try
            {
                str = enc.GetString(strData, 0, strData.Length);

                var result = new global::System.Security.SecureString();
                for (var i = 0; i < str.Length; i++)
                {
                    result.AppendChar(str[i]);
                }

                return result;
            }
            finally
            {
                str = null;
            }
        }

#endif

        /// <summary>
        /// Salts a string to transform.
        /// </summary>
        /// <param name="str">The string to salt.</param>
        /// <param name="enc">The encoding that is used.</param>
        protected virtual void SaltString(StringBuilder str, Encoding enc)
        {
            // dummy
        }

        /// <inheriteddoc />
        public byte[] TransformData(IEnumerable<byte> blob)
        {
            using (var src = new MemoryStream(blob.AsArray()))
            {
                try
                {
                    return this.TransformData(src);
                }
                finally
                {
                    this.DestroyTempStream(src);
                }
            }
        }

        /// <inheriteddoc />
        public void TransformData(IEnumerable<byte> blob, Stream dest)
        {
            using (var src = new MemoryStream(blob.AsArray()))
            {
                try
                {
                    this.TransformData(src, dest);
                }
                finally
                {
                    this.DestroyTempStream(src);
                }
            }
        }

        /// <inheriteddoc />
        public void TransformData(Stream src, Stream dest, int? bufferSize = null)
        {
            if (this.CanTransformData == false)
            {
                throw new NotSupportedException();
            }

            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            if (src.CanRead == false)
            {
                throw new IOException("src");
            }

            if (dest.CanWrite == false)
            {
                throw new IOException("dest");
            }

            if (bufferSize < 1)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            this._TRANSFORM_DATA_ACTION(src, dest, bufferSize);
        }

        /// <inheriteddoc />
        public byte[] TransformData(Stream src, int? bufferSize = null)
        {
            using (var dest = new MemoryStream())
            {
                try
                {
                    this.TransformData(src, dest, bufferSize);

                    return dest.ToArray();
                }
                finally
                {
                    this.DestroyTempStream(dest);
                }
            }
        }

        /// <inheriteddoc />
        public void TransformString(string str, Stream dest)
        {
            this.TransformString(str, Encoding.UTF8, dest);
        }

        /// <inheriteddoc />
        public void TransformString(string str, Encoding enc, Stream dest)
        {
            if (enc == null)
            {
                throw new ArgumentNullException("enc");
            }

            var builder = new StringBuilder(str ?? string.Empty);
            try
            {
                this.SaltString(builder, enc);

                using (var src = new MemoryStream(enc.GetBytes(builder.ToString())))
                {
                    try
                    {
                        this.TransformData(src, dest);
                    }
                    finally
                    {
                        this.DestroyTempStream(src);
                    }
                }
            }
            finally
            {
                this.DestroyTempStringBuilder(builder);
            }
        }

        /// <inheriteddoc />
        public byte[] TransformString(string str)
        {
            return this.TransformString(str, Encoding.UTF8);
        }

        /// <inheriteddoc />
        public byte[] TransformString(string str, Encoding enc)
        {
            using (var dest = new MemoryStream())
            {
                try
                {
                    this.TransformString(str, enc, dest);

                    return dest.ToArray();
                }
                finally
                {
                    this.DestroyTempStream(dest);
                }
            }
        }

        /// <inheriteddoc />
        public void TransformString(TextReader reader, Stream dest)
        {
            this.TransformString(reader, Encoding.UTF8, dest);
        }

        /// <inheriteddoc />
        public void TransformString(TextReader reader, Encoding enc, Stream dest)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            this.TransformString(reader.ReadToEnd(), enc, dest);
        }

        /// <inheriteddoc />
        public byte[] TransformString(TextReader reader)
        {
            return this.TransformString(reader, Encoding.UTF8);
        }

        /// <inheriteddoc />
        public byte[] TransformString(TextReader reader, Encoding enc)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return this.TransformString(reader.ReadToEnd(), enc);
        }

        /// <summary>
        /// Salts a restored string.
        /// </summary>
        /// <param name="str">The string to unsalt.</param>
        /// <param name="enc">The encoding that is used.</param>
        protected virtual void UnsaltString(StringBuilder str, Encoding enc)
        {
            // dummy
        }

        #endregion Methods (43)
    }
}