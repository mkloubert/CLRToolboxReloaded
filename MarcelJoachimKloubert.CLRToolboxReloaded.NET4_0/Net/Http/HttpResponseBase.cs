// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Net.Http
{
    /// <summary>
    /// A basic HTTP request context.
    /// </summary>
    public abstract partial class HttpResponseBase : ObjectBase, IHttpResponse
    {
        #region Fields (1)

        private Stream _stream;

        #endregion Fields (1)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBase" /> class.
        /// </summary>
        /// <param name="stream">The value for the <see cref="HttpResponseBase.Stream" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected HttpResponseBase(Stream stream, object sync)
            : base(isSynchronized: false,
                   sync: sync)
        {
            this._stream = stream;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBase" /> class.
        /// </summary>
        /// <param name="stream">The value for the <see cref="HttpResponseBase.Stream" /> property.</param>
        protected HttpResponseBase(Stream stream)
            : this(stream: stream,
                   sync: new object())
        {
        }

        #endregion Constructors

        #region Properties (12)

        /// <inheriteddoc />
        public virtual bool CanSetStreamCapacity
        {
            get { return false; }
        }

        /// <inheriteddoc />
        public Encoding Charset
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool? Compress
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public string ContentType
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool DirectOutput
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public bool DocumentNotFound
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public abstract IDictionary<string, object> FrontendVars
        {
            get;
        }

        /// <inheriteddoc />
        public abstract IDictionary<string, string> Headers
        {
            get;
        }

        /// <inheriteddoc />
        public bool IsForbidden
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public HttpStatusCode StatusCode
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public string StatusDescription
        {
            get;
            set;
        }

        /// <inheriteddoc />
        public Stream Stream
        {
            get { return this._stream; }
        }

        #endregion Properties

        #region Methods (20)

        /// <inheriteddoc />
        public HttpResponseBase Append(IEnumerable<byte> data)
        {
            var dataArray = data.AsArray();
            if ((dataArray != null) &&
                (dataArray.Length > 0))
            {
                var lastPos = this.Stream.Position;
                try
                {
                    // go to end
                    this.Stream.Position = this.Stream.Length;

                    this.Stream.Write(dataArray, 0, dataArray.Length);
                }
                finally
                {
                    // try restore position
                    this.Stream.Position = lastPos;
                }
            }

            return this;
        }

        IHttpResponse IHttpResponse.Append(IEnumerable<byte> data)
        {
            return this.Append(data: data);
        }

        /// <inheriteddoc />
        public HttpResponseBase Append(string str)
        {
            return this.Append(this.CharsToBytes(str));
        }

        IHttpResponse IHttpResponse.Append(string str)
        {
            return this.Append(str: str);
        }

        /// <summary>
        /// Converts a char sequence to a binary sequence.
        /// </summary>
        /// <param name="str">The chars / stringto convert.</param>
        /// <returns>The chars as bytes.</returns>
        protected virtual IEnumerable<byte> CharsToBytes(string str)
        {
            IEnumerable<byte> result = null;

            if (str != null)
            {
                var cs = this.Charset ?? this.GetDefaultCharset();
                if (cs != null)
                {
                    result = cs.GetBytes(str);
                }
            }

            return result;
        }

        /// <inheriteddoc />
        public virtual HttpResponseBase Clear()
        {
            this.Stream.SetLength(0);

            return this;
        }

        IHttpResponse IHttpResponse.Clear()
        {
            return this.Clear();
        }

        /// <summary>
        /// Returns the default charset.
        /// </summary>
        /// <returns>The default charset.</returns>
        protected virtual Encoding GetDefaultCharset()
        {
            return new UTF8Encoding();
        }

        /// <summary>
        /// The logic for the <see cref="HttpResponseBase.SetStream(Stream, bool)" /> method.
        /// </summary>
        /// <param name="stream">The new stream.</param>
        /// <param name="disposeOld">Dispose old stream or not.</param>
        protected void OnSetStream(Stream stream, bool disposeOld)
        {
            var oldStream = this._stream;
            this._stream = stream;

            if ((oldStream != null) &&
                disposeOld)
            {
                oldStream.Dispose();
            }
        }

        /// <summary>
        /// Defines the logic for <see cref="HttpResponseBase.SetStreamCapacity(int)" /> method.
        /// </summary>
        /// <param name="capacity">
        /// The new capacity. <see langword="null" /> indicates to use a default value.
        /// </param>
        protected virtual void OnSetStreamCapacity(int? capacity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The logic for <see cref="HttpResponseBase.WriteJson{T}(T)" /> method.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="json">The string builder that builds/stores the JSON string of <paramref name="obj" />.</param>
        protected virtual void OnWriteJson<T>(T obj, ref StringBuilder json)
        {
            obj.ToJson(builder: json);
        }

        /// <inheriteddoc />
        public HttpResponseBase Prefix(IEnumerable<byte> data)
        {
            var dataArray = data.AsArray();
            if ((dataArray != null) &&
                (dataArray.Length) > 0)
            {
                using (var backup = new MemoryStream())
                {
                    // backup
                    this.Stream.Position = 0;
                    this.Stream.CopyTo(backup);

                    this.Clear();

                    this.Stream.Write(dataArray, 0, dataArray.Length);

                    // restore
                    backup.Position = 0;
                    backup.CopyTo(this.Stream);
                }
            }

            return this;
        }

        IHttpResponse IHttpResponse.Prefix(IEnumerable<byte> data)
        {
            return this.Prefix(data: data);
        }

        /// <inheriteddoc />
        public HttpResponseBase Prefix(string str)
        {
            return this.Prefix(this.CharsToBytes(str));
        }

        IHttpResponse IHttpResponse.Prefix(string str)
        {
            return this.Prefix(str: str);
        }

        /// <inheriteddoc />
        public HttpResponseBase SetDefaultStreamCapacity()
        {
            this.SetStreamCapacityInner(capacity: null);
            return null;
        }

        IHttpResponse IHttpResponse.SetDefaultStreamCapacity()
        {
            return this.SetDefaultStreamCapacity();
        }

        /// <inheriteddoc />
        public HttpResponseBase SetStream(Stream stream, bool disposeOld = false)
        {
            this.OnSetStream(stream, disposeOld);
            return this;
        }

        IHttpResponse IHttpResponse.SetStream(Stream stream, bool disposeOld)
        {
            return this.SetStream(stream: stream,
                                  disposeOld: disposeOld);
        }

        /// <inheriteddoc />
        public HttpResponseBase SetStreamCapacity(int capacity)
        {
            this.SetStreamCapacityInner(capacity);
            return this;
        }

        IHttpResponse IHttpResponse.SetStreamCapacity(int capacity)
        {
            return this.SetStreamCapacity(capacity: capacity);
        }

        private void SetStreamCapacityInner(int? capacity)
        {
            if (this.CanSetStreamCapacity == false)
            {
                throw new NotSupportedException();
            }

            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }

            this.OnSetStreamCapacity(capacity);
        }

        /// <inheriteddoc />
        public HttpResponseBase SetupForJson()
        {
            this.Charset = new UTF8Encoding();
            this.ContentType = "application/json";
            this.DirectOutput = true;

            return this;
        }

        IHttpResponse IHttpResponse.SetupForJson()
        {
            return this.SetupForJson();
        }

        /// <inheriteddoc />
        public HttpResponseBase Write(IEnumerable<byte> data)
        {
            var dataArray = data.AsArray();
            if (dataArray != null)
            {
                this.Stream
                    .Write(dataArray, 0, dataArray.Length);
            }

            return this;
        }

        IHttpResponse IHttpResponse.Write(IEnumerable<byte> data)
        {
            return this.Write(data: data);
        }

        /// <inheriteddoc />
        public HttpResponseBase Write(string str)
        {
            return this.Write(this.CharsToBytes(str));
        }

        IHttpResponse IHttpResponse.Write(string str)
        {
            return this.Write(str: str);
        }

        /// <inheriteddoc />
        public HttpResponseBase Write(object obj, bool handleDBNullAsNull = true)
        {
            if (obj is IEnumerable<byte>)
            {
                return this.Write(obj as IEnumerable<byte>);
            }

            return this.Write(obj.AsString(handleDbNullAsNull: handleDBNullAsNull));
        }

        IHttpResponse IHttpResponse.Write(object obj, bool handleDBNullAsNull)
        {
            return this.Write(obj: obj,
                              handleDBNullAsNull: handleDBNullAsNull);
        }

        /// <inheriteddoc />
        public HttpResponseBase WriteJavaScript(string js)
        {
            return this.Write("<script type=\"text/javascript\">\n\n")
                       .Write(js)
                       .Write("\n\n</script>");
        }

        IHttpResponse IHttpResponse.WriteJavaScript(string js)
        {
            return this.WriteJavaScript(js: js);
        }

        /// <inheriteddoc />
        public HttpResponseBase WriteJson<T>(T obj)
        {
            var json = new StringBuilder();
            this.OnWriteJson<T>(obj, ref json);

            if (json != null)
            {
                this.Write(json);
            }

            return this;
        }

        IHttpResponse IHttpResponse.WriteJson<T>(T obj)
        {
            return this.WriteJson<T>(obj: obj);
        }

        #endregion Methods
    }
}