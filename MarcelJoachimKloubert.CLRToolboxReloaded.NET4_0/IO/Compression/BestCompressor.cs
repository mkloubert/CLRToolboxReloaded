// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Compression
{
    /// <summary>
    /// A compressor that uses the best compression method.
    /// </summary>
    public class BestCompressor : CompressorBase, IEnumerable<ICompressor>
    {
        #region Fields (1)

        private readonly CompressorProvider _COMPRESSOR_PROVIDER;

        #endregion Fields (1)

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="BestCompressor" /> class.
        /// </summary>
        /// <param name="provider">The function / method that provides all available compressors.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public BestCompressor(CompressorProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._COMPRESSOR_PROVIDER = provider;
        }

        #endregion Constructors (1)

        #region Delegates and events (1)

        /// <summary>
        /// Describes a function / method that provides the available compressors.
        /// </summary>
        /// <param name="compressor">The compressor to use.</param>
        /// <returns>The available compressors.</returns>
        public delegate IEnumerable<ICompressor> CompressorProvider(BestCompressor compressor);

        #endregion Delegates and events (1)

        #region Methods (9)

        /// <summary>
        /// Creates a new instance of the <see cref="BestCompressor" /> class.
        /// </summary>
        /// <param name="compressors">The available compressors.</param>
        /// <param name="addDummyCompressor">Add dummy compressor or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="compressors" /> is <see langword="null" />.
        /// </exception>
        public static BestCompressor Create(IEnumerable<ICompressor> compressors,
                                            bool addDummyCompressor = true)
        {
            if (compressors == null)
            {
                throw new ArgumentNullException("compressors");
            }

            if (addDummyCompressor)
            {
                compressors = new ICompressor[] { new DummyCompressor() }.Concat(compressors);
            }

            return new BestCompressor(provider: (c) => compressors);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BestCompressor" /> class.
        /// </summary>
        /// <param name="compressors">The available compressors.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="compressors" /> is <see langword="null" />.
        /// </exception>
        /// <remarks>Adds a dummy compressor.</remarks>
        public static BestCompressor Create(params ICompressor[] compressors)
        {
            return Create(true, compressors);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="BestCompressor" /> class.
        /// </summary>
        /// <param name="addDummyCompressor">Add dummy compressor or not.</param>
        /// <param name="compressors">The available compressors.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="compressors" /> is <see langword="null" />.
        /// </exception>
        public static BestCompressor Create(bool addDummyCompressor, params ICompressor[] compressors)
        {
            return Create((IEnumerable<ICompressor>)compressors,
                          addDummyCompressor);
        }

        private IList<ICompressor> GetCompressorList()
        {
            var result = this.GetCompressors().ToList();
            if (result.Count < 1)
            {
                result.Add(new DummyCompressor());
            }

            return result;
        }

        /// <summary>
        /// Returns a list of available compressors.
        /// </summary>
        /// <returns>The list of available compressors.</returns>
        public IEnumerable<ICompressor> GetCompressors()
        {
            return (this._COMPRESSOR_PROVIDER(this) ?? Enumerable.Empty<ICompressor>()).Where(c => c != null);
        }

        /// <inheriteddoc />
        public IEnumerator<ICompressor> GetEnumerator()
        {
            return this.GetCompressors()
                       .GetEnumerator();
        }

        /// <inheriteddoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheriteddoc />
        protected override void OnCompress(Stream src, Stream dest, int? bufferSize)
        {
            using (var temp = new MemoryStream())
            {
                try
                {
                    // first copy source data to temp
                    this.CopyData(src, temp, bufferSize);

                    var compressors = this.GetCompressorList();
                    try
                    {
                        var compressorCount = compressors.Count;

                        Stream streamToUse = null;
                        try
                        {
                            ICompressor compressorToUse = null;
                            byte compressorIndex = 0;
                            for (var i = 0; i < compressorCount; i++)
                            {
                                var currentCompressor = compressors[i];

                                // reset temp stream with uncompressed data
                                temp.Position = 0;

                                var compressorStream = new MemoryStream();
                                try
                                {
                                    Stream oldStream = null;

                                    // test compressor
                                    currentCompressor.Compress(temp, compressorStream);
                                    if ((streamToUse == null) ||
                                        (compressorStream.Length <= streamToUse.Length))
                                    {
                                        // first compression or
                                        // has maximum the same size like the
                                        // previous compression result

                                        oldStream = streamToUse;

                                        streamToUse = compressorStream;
                                        streamToUse.Position = 0;
                                        compressorToUse = currentCompressor;
                                        compressorIndex = (byte)i;
                                    }

                                    if (oldStream != null)
                                    {
                                        using (var s = oldStream)
                                        {
                                            this.DestroyTempStream(s);
                                        }
                                    }
                                }
                                catch
                                {
                                    using (var s = compressorStream)
                                    {
                                        this.DestroyTempStream(s);
                                    }

                                    throw;
                                }
                            }

                            // first write index of compressor to use
                            dest.WriteByte(compressorIndex);

                            this.CopyData(streamToUse, dest);
                        }
                        finally
                        {
                            using (var s = streamToUse)
                            {
                                if (s != null)
                                {
                                    this.DestroyTempStream(s);
                                }
                            }
                        }
                    }
                    finally
                    {
                        compressors.Clear();
                    }
                }
                finally
                {
                    this.DestroyTempStream(temp);
                }
            }
        }

        /// <inheriteddoc />
        protected override void OnUncompress(Stream src, Stream dest, int? bufferSize)
        {
            var compressors = this.GetCompressorList();
            try
            {
                // uncompress
                //
                // first byte is the index of the compressor
                compressors[src.ReadByte()].Uncompress(src, dest, bufferSize);
            }
            finally
            {
                compressors.Clear();
            }
        }

        #endregion Methods (9)
    }
}