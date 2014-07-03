// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
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

        #region Events (1)

        /// <summary>
        /// Is invoked if data was read or written via base stream.
        /// </summary>
        public event EventHandler<EventStreamDataTransferedEventArgs> DataTransfered;

        #endregion Events (1)

        #region Methods (4)

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

        /// <inheriteddoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = base.Read(buffer, offset, count);
            this.OnDataTransfered(EventStreamDataTransferedContext.Read,
                                  buffer.Take(result));

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
        }

        #endregion Methods (4)
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