using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// A data transformer that uses multi <see cref="IDataTransformer" /> instances.
    /// </summary>
    public class AggregateDataTransformer : DataTransformerBase, IEnumerable<IDataTransformer>
    {
        #region Fields (1)

        private readonly TransformerProvider _TRANSFORMER_PROVIDER;

        #endregion Fields (1)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateDataTransformer" /> class.
        /// </summary>
        /// <param name="provider">
        /// The function / delegate that provides data transformers to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public AggregateDataTransformer(TransformerProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._TRANSFORMER_PROVIDER = provider;
        }

        #endregion Constructors (1)

        #region Events and delegates (1)

        /// <summary>
        /// A function or method that provides the transformers to use.
        /// </summary>
        /// <param name="transformer">The underlying transformer instance.</param>
        /// <returns>THe transformers to use.</returns>
        public delegate IEnumerable<IDataTransformer> TransformerProvider(AggregateDataTransformer transformer);

        #endregion Events and delegates (1)

        #region Properties (2)

        /// <inheriteddoc />
        public override bool CanRestoreData
        {
            get { return true; }
        }

        /// <inheriteddoc />
        public override bool CanTransformData
        {
            get { return true; }
        }

        #endregion Properties (2)

        #region Methods (11)

        /// <summary>
        /// Closes an old temporary stream that was used for transform and restore operations.
        /// </summary>
        /// <param name="stream">The stream to close.</param>
        protected virtual void CloseTempStream(Stream stream)
        {
            stream.Dispose();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AggregateDataTransformer" /> class.
        /// </summary>
        /// <param name="transformers">The crypters to use.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformers" /> is <see langword="null" />.
        /// </exception>
        public static AggregateDataTransformer Create(params IDataTransformer[] transformers)
        {
            return Create((IEnumerable<IDataTransformer>)transformers);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AggregateDataTransformer" /> class.
        /// </summary>
        /// <param name="transformers">The transformers to use.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformers" /> is <see langword="null" />.
        /// </exception>
        public static AggregateDataTransformer Create(IEnumerable<IDataTransformer> transformers)
        {
            if (transformers == null)
            {
                throw new ArgumentNullException("transformers");
            }

            return new AggregateDataTransformer((t) => transformers);
        }

        /// <summary>
        /// Creates a temporary stream for transform and restore operations.
        /// </summary>
        /// <returns>The created stream.</returns>
        protected virtual Stream CreateTempStream()
        {
            return new MemoryStream();
        }

        /// <summary>
        /// Returns the buffer size in bytes that should be used to read a temp
        /// stream that is used as source.
        /// </summary>
        /// <param name="stream">The temporary stream.</param>
        /// <returns>
        /// The buffer size.
        /// <see langword="null" /> indicates to use the default value.
        /// </returns>
        protected virtual int? GetBufferSizeForTempStream(Stream stream)
        {
            return null;
        }

        /// <inheriteddoc />
        public IEnumerator<IDataTransformer> GetEnumerator()
        {
            return this.GetTransformers()
                       .GetEnumerator();
        }

        /// <inheriteddoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns a list of all transformers to use.
        /// </summary>
        /// <returns>The list of transformers to use.</returns>
        public IEnumerable<IDataTransformer> GetTransformers()
        {
            return (this._TRANSFORMER_PROVIDER(this) ?? Enumerable.Empty<IDataTransformer>()).Where(t => t != null);
        }

        /// <inheriteddoc />
        protected override void OnRestoreData(Stream src, Stream dest, int? bufferSize)
        {
            this.TransformOrRestore(src, dest, bufferSize,
                                    this.GetTransformers().Reverse(),
                                    (t, s, d, bs) => t.RestoreData(s, d, bs));
        }

        /// <inheriteddoc />
        protected override void OnTransformData(Stream src, Stream dest, int? bufferSize)
        {
            this.TransformOrRestore(src, dest, bufferSize,
                                    this.GetTransformers(),
                                    (t, s, d, bs) => t.TransformData(s, d, bs));
        }

        private void TransformOrRestore(Stream src, Stream dest, int? bufferSize,
                                        IEnumerable<IDataTransformer> transformers,
                                        Action<IDataTransformer, Stream, Stream, int?> transformAction)
        {
            Stream currentDest = null;

            using (var e = transformers.GetEnumerator())
            {
                long index = -1;
                Stream currentSrc = null;

                int? currentBufSize = null;
                while (e.MoveNext())
                {
                    ++index;
                    var currentCrypter = e.Current;

                    if (index == 0)
                    {
                        // first operation

                        currentSrc = src;
                        currentBufSize = bufferSize;
                    }
                    else
                    {
                        // last destionation is new source
                        currentSrc = currentDest;
                        currentSrc.Position = 0;

                        currentBufSize = this.GetBufferSizeForTempStream(currentSrc);
                    }

                    currentDest = this.CreateTempStream();
                    try
                    {
                        transformAction(currentCrypter,
                                        currentSrc, currentDest, currentBufSize);
                    }
                    catch
                    {
                        // close before rethrow exception
                        this.CloseTempStream(currentDest);

                        throw;
                    }
                    finally
                    {
                        if (index > 0)
                        {
                            // at that point it is a temp stream
                            this.CloseTempStream(currentSrc);
                        }
                    }
                }
            }

            if (currentDest != null)
            {
                // last but not least:
                // copy to real destination stream

                try
                {
                    currentDest.Position = 0;
                    currentDest.CopyTo(dest);
                }
                finally
                {
                    this.CloseTempStream(currentDest);
                }
            }
        }

        #endregion Methods (11)
    }
}