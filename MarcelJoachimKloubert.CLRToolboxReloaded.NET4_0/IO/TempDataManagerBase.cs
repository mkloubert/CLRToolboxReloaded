// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// A basic object that handles temporary data.
    /// </summary>
    public abstract class TempDataManagerBase : ObjectBase, ITempDataManager
    {
        #region Fields (1)

        private readonly Func<Stream> _ON_CREATE_STREAM_FUNC;

        #endregion Fields (1)

        #region Constrcutors (4)

        /// <inheriteddoc />
        protected TempDataManagerBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            if (this._IS_SYNCHRONIZED)
            {
                this._ON_CREATE_STREAM_FUNC = this.OnCreateStream_ThreadSafe;
            }
            else
            {
                this._ON_CREATE_STREAM_FUNC = this.OnCreateStream;
            }
        }

        /// <inheriteddoc />
        protected TempDataManagerBase(bool isSynchronized)
            : this(isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <inheriteddoc />
        protected TempDataManagerBase(object sync)
            : this(isSynchronized: true,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected TempDataManagerBase()
            : this(isSynchronized: true)
        {
        }

        #endregion Constrcutors (4)

        #region Methods (6)

        /// <inheriteddoc />
        public Stream CreateStream()
        {
            return this.CreateStreamInner(initialData: null,
                                          bufferSize: null,
                                          startAtBeginning: true);
        }

        /// <inheriteddoc />
        public Stream CreateStream(Stream initialData, int? bufferSize = null,
                                   bool startAtBeginning = true)
        {
            if (initialData == null)
            {
                throw new ArgumentNullException("initialData");
            }

            return this.CreateStreamInner(initialData: initialData,
                                          bufferSize: bufferSize,
                                          startAtBeginning: startAtBeginning);
        }

        private Stream CreateStreamInner(Stream initialData, int? bufferSize, bool startAtBeginning)
        {
            if (bufferSize < 1)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            if (initialData != null)
            {
                if (initialData.CanRead == false)
                {
                    throw new IOException("initialData");
                }
            }

            var stream = this._ON_CREATE_STREAM_FUNC();
            try
            {
                if (initialData != null)
                {
                    if (bufferSize.IsNull())
                    {
                        initialData.CopyTo(stream);
                    }
                    else
                    {
                        initialData.CopyTo(stream, bufferSize.Value);
                    }
                }

                if (startAtBeginning)
                {
                    stream.Position = 0;
                }
            }
            catch
            {
                this.DestroyStream(stream);

                throw;
            }

            return stream;
        }

        /// <summary>
        /// Destroys a stream that was created by <see cref="TempDataManagerBase.OnCreateStream()" /> method.
        /// </summary>
        /// <param name="stream">The stream to destroy.</param>
        protected virtual void DestroyStream(Stream stream)
        {
            using (var s = stream)
            {
                // simpy dispose
            }
        }

        /// <summary>
        /// Stores the logic for the <see cref="TempDataManagerBase.CreateStream()" /> method.
        /// </summary>
        /// <returns>The created stream.</returns>
        protected abstract Stream OnCreateStream();

        private Stream OnCreateStream_ThreadSafe()
        {
            Stream result;

            lock (this._SYNC)
            {
                result = this.OnCreateStream();
            }

            return result;
        }

        #endregion Methods (6)
    }
}