// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    /// <summary>
    /// A thread safe wrapper for an <see cref="IEnumerator{T}" /> instance.
    /// </summary>
    /// <typeparam name="T">Type of the items.</typeparam>
    public sealed class SynchronizedEnumerator<T> : IEnumerator<T>
    {
        #region Fields (2)

        private readonly IEnumerator<T> _ENUMERATOR;
        private readonly object _SYNC;

        #endregion Fields (2)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEnumerator{T}" /> class.
        /// </summary>
        /// <param name="seq">The sequence from where to get the base enumerator from.</param>
        /// <exception cref="NullReferenceException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public SynchronizedEnumerator(IEnumerable<T> seq)
            : this(seq, new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEnumerator{T}" /> class.
        /// </summary>
        /// <param name="seq">The sequence from where to get the base enumerator from.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public SynchronizedEnumerator(IEnumerable<T> seq,
                                      object sync)
            : this(seq.GetEnumerator(),
                   sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEnumerator{T}" /> class.
        /// </summary>
        /// <param name="enumerator">The base enumerator.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enumerator" /> is <see langword="null" />.
        /// </exception>
        public SynchronizedEnumerator(IEnumerator<T> enumerator)
            : this(enumerator, new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEnumerator{T}" /> class.
        /// </summary>
        /// <param name="enumerator">The base enumerator.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enumerator" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public SynchronizedEnumerator(IEnumerator<T> enumerator,
                                      object sync)
        {
            if (enumerator == null)
            {
                throw new ArgumentNullException("enumerator");
            }

            if (sync == null)
            {
                throw new ArgumentNullException("sync");
            }

            this._ENUMERATOR = enumerator;
            this._SYNC = sync;
        }

        #endregion Constrcutors (4)

        #region Methods (3)

        /// <inheriteddoc />
        public void Dispose()
        {
            lock (this._SYNC)
            {
                this._ENUMERATOR
                    .Dispose();
            }
        }

        /// <inheriteddoc />
        public bool MoveNext()
        {
            bool result;

            lock (this._SYNC)
            {
                result = this._ENUMERATOR
                             .MoveNext();
            }

            return result;
        }

        /// <inheriteddoc />
        public void Reset()
        {
            lock (this._SYNC)
            {
                this._ENUMERATOR
                    .Reset();
            }
        }

        #endregion Methods (3)

        #region Properties (4)

        /// <summary>
        /// Gets the base enumerator.
        /// </summary>
        public IEnumerator<T> BaseEnumerator
        {
            get { return this._ENUMERATOR; }
        }

        /// <inheriteddoc />
        public T Current
        {
            get
            {
                T result;

                lock (this._SYNC)
                {
                    result = this._ENUMERATOR.Current;
                }

                return result;
            }
        }

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        /// <summary>
        /// Gets the object that is used for thread safe operations.
        /// </summary>
        public object SyncRoot
        {
            get { return this._SYNC; }
        }

        #endregion Properties (4)
    }
}