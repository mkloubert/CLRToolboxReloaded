// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(NET40 || PORTABLE40)
#define CAN_INVOKE_ASYNC
#endif

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    #region CLASS: StreamWrapperBase<TStream>

    /// <summary>
    /// A basic wrapper for a <see cref="Stream" />.
    /// </summary>
    public abstract class StreamWrapperBase<TStream> : Stream
        where TStream : global::System.IO.Stream
    {
        #region Fields (2)

        /// <summary>
        /// Stores the inner stream.
        /// </summary>
        protected readonly TStream _BASE_STREAM;

        /// <summary>
        /// Stores the unique object for thread safe operations.
        /// </summary>
        protected readonly object _SYNC;

        #endregion Fields (2)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamWrapperBase{TStream}" /> class.
        /// </summary>
        /// <param name="baseStream">The value for <see cref="StreamWrapperBase{TStream}._BASE_STREAM" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="baseStream" /> is <see langword="null" />.
        /// </exception>
        protected StreamWrapperBase(TStream baseStream)
            : this(baseStream: baseStream,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamWrapperBase{TStream}" /> class.
        /// </summary>
        /// <param name="baseStream">The value for <see cref="StreamWrapperBase{TStream}._BASE_STREAM" /> field.</param>
        /// <param name="sync">The value for <see cref="StreamWrapperBase{TStream}._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="baseStream" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        protected StreamWrapperBase(TStream baseStream, object sync)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }

            if (sync == null)
            {
                throw new ArgumentNullException("sync");
            }

            this._BASE_STREAM = baseStream;
            this._SYNC = sync;
        }

        #endregion Constructors (2)

        #region Properties (10)

        /// <summary>
        /// Gets the base / wrapped stream.
        /// </summary>
        public TStream BaseStream
        {
            get { return this._BASE_STREAM; }
        }

        /// <inheriteddoc />
        public override bool CanRead
        {
            get { return this._BASE_STREAM.CanRead; }
        }

        /// <inheriteddoc />
        public override bool CanSeek
        {
            get { return this._BASE_STREAM.CanSeek; }
        }

        /// <inheriteddoc />
        public override bool CanTimeout
        {
            get { return this._BASE_STREAM.CanTimeout; }
        }

        /// <inheriteddoc />
        public override bool CanWrite
        {
            get { return this._BASE_STREAM.CanWrite; }
        }

        /// <inheriteddoc />
        public override long Length
        {
            get { return this._BASE_STREAM.Length; }
        }

        /// <inheriteddoc />
        public override long Position
        {
            get { return this._BASE_STREAM.Position; }

            set { this._BASE_STREAM.Position = value; }
        }

        /// <inheriteddoc />
        public override int ReadTimeout
        {
            get { return this._BASE_STREAM.ReadTimeout; }

            set { this._BASE_STREAM.ReadTimeout = value; }
        }

        /// <summary>
        /// Gets the unique object for thread safe operations.
        /// </summary>
        public object SyncRoot
        {
            get { return this._SYNC; }
        }

        /// <inheriteddoc />
        public override int WriteTimeout
        {
            get { return this._BASE_STREAM.WriteTimeout; }

            set { this._BASE_STREAM.WriteTimeout = value; }
        }

        #endregion Properties (10)

        #region Methods (19)

        /// <inheriteddoc />
        public override global::System.IAsyncResult BeginRead(byte[] buffer, int offset, int count, global::System.AsyncCallback callback, object state)
        {
            return this._BASE_STREAM.BeginRead(buffer, offset, count, callback, state);
        }

        /// <inheriteddoc />
        public override global::System.IAsyncResult BeginWrite(byte[] buffer, int offset, int count, global::System.AsyncCallback callback, object state)
        {
            return this._BASE_STREAM.BeginWrite(buffer, offset, count, callback, state);
        }

#if !PORTABLE40

        /// <inheriteddoc />
        public override void Close()
        {
            this._BASE_STREAM.Close();
        }

#endif

#if CAN_INVOKE_ASYNC

        /// <inheriteddoc />
        public override global::System.Threading.Tasks.Task CopyToAsync(global::System.IO.Stream destination, int bufferSize, global::System.Threading.CancellationToken cancellationToken)
        {
            return this._BASE_STREAM.CopyToAsync(destination, bufferSize, cancellationToken);
        }

#endif

        /// <inheriteddoc />
        protected override void Dispose(bool disposing)
        {
            this.InvokeDispose(disposing);
        }

        /// <inheriteddoc />
        public override int EndRead(global::System.IAsyncResult asyncResult)
        {
            return this._BASE_STREAM.EndRead(asyncResult);
        }

        /// <inheriteddoc />
        public override void EndWrite(global::System.IAsyncResult asyncResult)
        {
            this._BASE_STREAM.EndWrite(asyncResult);
        }

        /// <inheriteddoc />
        public override void Flush()
        {
            this._BASE_STREAM.Flush();
        }

#if CAN_INVOKE_ASYNC

        /// <inheriteddoc />
        public override global::System.Threading.Tasks.Task FlushAsync(global::System.Threading.CancellationToken cancellationToken)
        {
            return this._BASE_STREAM.FlushAsync(cancellationToken);
        }

#endif

        /// <summary>
        /// Invokes the <see cref="Stream.Dispose(bool)" /> method of
        /// <see cref="StreamWrapperBase{TStream}._BASE_STREAM" /> object.
        /// </summary>
        /// <param name="disposing">The parameter value for the <see cref="Stream.Dispose(bool)" /> method.</param>
        protected void InvokeDispose(bool disposing)
        {
            // find Dispose(bool) method of inner base stream
            var disposeMethod = this._BASE_STREAM
                                    .GetType()
                                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                    .First((m) =>
                                           {
                                               if (m.Name != "Dispose")
                                               {
                                                   // invalid name
                                                   return false;
                                               }

                                               if (m.GetGenericArguments().Length != 0)
                                               {
                                                   // must NOT have generic arguments
                                                   return false;
                                               }

                                               // only one boolean parameter
                                               var @params = m.GetParameters();
                                               return ((@params.Length) == 1) &&
                                                      typeof(bool).Equals(@params[0].ParameterType);
                                           });

            disposeMethod.Invoke(this._BASE_STREAM,
                                 new object[] { disposing });
        }

        /// <inheriteddoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this._BASE_STREAM.Read(buffer, offset, count);
        }

#if CAN_INVOKE_ASYNC

        /// <inheriteddoc />
        public override global::System.Threading.Tasks.Task<int> ReadAsync(byte[] buffer, int offset, int count, global::System.Threading.CancellationToken cancellationToken)
        {
            return this._BASE_STREAM.ReadAsync(buffer, offset, count, cancellationToken);
        }

#endif

        /// <inheriteddoc />
        public override int ReadByte()
        {
            return this._BASE_STREAM.ReadByte();
        }

        /// <inheriteddoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this._BASE_STREAM.Seek(offset, origin);
        }

        /// <inheriteddoc />
        public override void SetLength(long value)
        {
            this._BASE_STREAM.SetLength(value);
        }

        /// <inheriteddoc />
        public override string ToString()
        {
            return this._BASE_STREAM.ToString();
        }

        /// <inheriteddoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            this._BASE_STREAM.Write(buffer, offset, count);
        }

#if CAN_INVOKE_ASYNC

        /// <inheriteddoc />
        public override global::System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, global::System.Threading.CancellationToken cancellationToken)
        {
            return this._BASE_STREAM.WriteAsync(buffer, offset, count, cancellationToken);
        }

#endif

        /// <inheriteddoc />
        public override void WriteByte(byte value)
        {
            this._BASE_STREAM.WriteByte(value);
        }

        #endregion Methods (19)
    }

    #endregion CLASS: StreamWrapperBase<TStream>

    #region CLASS: StreamWrapperBase

    /// <summary>
    /// Basic implementation of <see cref="StreamWrapperBase{TStream}" />.
    /// </summary>
    public abstract class StreamWrapperBase : StreamWrapperBase<Stream>
    {
        #region Constructors (2)

        /// <inheriteddoc />
        protected StreamWrapperBase(Stream baseStream)
            : base(baseStream: baseStream)
        {
        }

        /// <inheriteddoc />
        protected StreamWrapperBase(Stream baseStream, object sync)
            : base(baseStream: baseStream,
                   sync: sync)
        {
        }

        #endregion Constructors (2)
    }

    #endregion CLASS: StreamWrapperBase
}