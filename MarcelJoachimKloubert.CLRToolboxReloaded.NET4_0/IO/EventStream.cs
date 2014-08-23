// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !PORTABLE40
#define CLOSE_CAN_BE_OVERWRITTEN
#endif

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    #region EventStreamDataTransferedContext

    /// <summary>
    /// List of contextes for a <see cref="EventStream{TStream}.DataTransfered" /> event.
    /// </summary>
    public enum EventStreamDataTransferedContext
    {
        /// <summary>
        ///
        /// </summary>
        /// <see cref="EventStream{TStream}.Read(byte[], int, int)" />
        Read,

        /// <summary>
        ///
        /// </summary>
        /// <see cref="EventStream{TStream}.Write(byte[], int, int)" />
        Write,
    }

    #endregion EventStreamDataTransferedContext

    #region EventStreamDataTransferedEventArgs

    /// <summary>
    /// Arguments for a <see cref="EventStream{TStream}.DataTransfered" /> event.
    /// </summary>
    public sealed class EventStreamDataTransferedEventArgs : EventArgs
    {
        #region Properties (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="EventStreamDataTransferedEventArgs" /> class.
        /// </summary>
        /// <param name="context">The value for the <see cref="EventStreamDataTransferedEventArgs.Context" /> property.</param>
        /// <param name="buffer">The value for the <see cref="EventStreamDataTransferedEventArgs.Buffer" /> property.</param>
        public EventStreamDataTransferedEventArgs(EventStreamDataTransferedContext context, IEnumerable<byte> buffer)
        {
            this.Buffer = buffer.AsArray();
            this.Context = context;
        }

        #endregion Properties (1)

        #region Properties (2)

        /// <summary>
        /// Gets the underlying data.
        /// </summary>
        public byte[] Buffer
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public EventStreamDataTransferedContext Context
        {
            get;
            private set;
        }

        #endregion Properties (2)
    }

    #endregion EventStreamDataTransferedEventArgs

    #region CLASS: EventStream<TStream>

    /// <summary>
    /// A stream wrapper that raises events on each action.
    /// </summary>
    public class EventStream<TStream> : StreamWrapperBase<TStream>
        where TStream : global::System.IO.Stream
    {
        #region Constructors (2)

        /// <inheriteddoc />
        public EventStream(TStream baseStream)
            : base(baseStream: baseStream)
        {
        }

        /// <inheriteddoc />
        public EventStream(TStream baseStream, object sync)
            : base(baseStream: baseStream,
                   sync: sync)
        {
        }

        #endregion Constructors (2)

        #region Events (4)

        /// <summary>
        /// Is raised after stream has been closed.
        /// </summary>
        public event EventHandler Closed;
        
        /// <summary>
        /// Is raised before stream starts closing.
        /// </summary>
        public event EventHandler<CancelEventArgs> Closing;

        /// <summary>
        /// Is invoked if data was read or written via base stream.
        /// </summary>
        public event EventHandler<EventStreamDataTransferedEventArgs> DataTransfered;
        
        /// <summary>
        /// Is raised after stream has been disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Is raised before stream starts disposing.
        /// </summary>
        public event EventHandler Disposing;

        #endregion Events (2)

        #region Methods (9)

#if CLOSE_CAN_BE_OVERWRITTEN

        /// <inheriteddoc />
        public override void Close()
        {
            var e = new global::System.ComponentModel.CancelEventArgs(cancel: false);
            this.RaiseEventHandler(this.Closing, e);

            if (e.Cancel)
            {
                return;
            }

            base.Close();
            this.RaiseEventHandler(this.Closed);
        }

#endif

        /// <inheriteddoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.RaiseEventHandler(this.Disposing);
            }

#if !CLOSE_CAN_BE_OVERWRITTEN
            var e = new global::System.ComponentModel.CancelEventArgs(cancel: false);
            this.RaiseEventHandler(this.Closing, e);

            if (e.Cancel)
            {
                return;
            }
#endif

            base.Dispose(disposing);

            if (disposing)
            {
                this.RaiseEventHandler(this.Disposed);
            }

#if !CLOSE_CAN_BE_OVERWRITTEN
            this.RaiseEventHandler(this.Closed);
#endif
        }

        /// <summary>
        /// Raises the <see cref="EventStream{TStream}.DataTransfered" /> event.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="buffer">The data.</param>
        /// <returns>Event was raised or not.</returns>
        protected bool OnDataTransfered(EventStreamDataTransferedContext context, IEnumerable<byte> buffer)
        {
            var handler = this.DataTransfered;
            if (handler != null)
            {
                handler(this, new EventStreamDataTransferedEventArgs(context, buffer));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises an event handler for that instance.
        /// </summary>
        /// <param name="handler">The handler to raised.</param>
        /// <returns>Handler was raised or not.</returns>
        protected bool RaiseEventHandler(EventHandler handler)
        {
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Raises an event handler for that instance.
        /// </summary>
        /// <param name="handler">The handler to raised.</param>
        /// <param name="e">The arguments for the handler.</param>
        /// <returns>Handler was raised or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="e" /> is <see langword="null" />.
        /// </exception>
        protected bool RaiseEventHandler<TArgs>(EventHandler<TArgs> handler, TArgs e)
            where TArgs : global::System.EventArgs
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            if (handler != null)
            {
                handler(this, e);
                return true;
            }

            return false;
        }

        /// <inheriteddoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = base.Read(buffer, offset, count);
            this.OnDataTransfered(EventStreamDataTransferedContext.Read,
                                  buffer.Take(result));

            return result;
        }

        /// <inheriteddoc />
        public override int ReadByte()
        {
            var result = base.ReadByte();
            this.OnDataTransfered(EventStreamDataTransferedContext.Read,
                                  result >= 0 ? new byte[] { (byte)result } : Enumerable.Empty<byte>());

            return result;
        }

        /// <inheriteddoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);

            this.OnDataTransfered(EventStreamDataTransferedContext.Write,
                                  buffer);
        }

        /// <inheriteddoc />
        public override void WriteByte(byte value)
        {
            base.WriteByte(value);

            this.OnDataTransfered(EventStreamDataTransferedContext.Write,
                                  new byte[] { value });
        }

        #endregion Methods (8)
    }

    #endregion CLASS: EventStream<TStream>

    #region CLASS: EventStream

    /// <summary>
    /// Simple implementation of <see cref="EventStream{TStream}" />.
    /// </summary>
    public class EventStream : EventStream<Stream>
    {
        #region Constructors (2)

        /// <inheriteddoc />
        public EventStream(Stream baseStream)
            : base(baseStream: baseStream)
        {
        }

        /// <inheriteddoc />
        public EventStream(Stream baseStream, object sync)
            : base(baseStream: baseStream,
                   sync: sync)
        {
        }

        #endregion Constructors (2)
    }

    #endregion CLASS: EventStream
}