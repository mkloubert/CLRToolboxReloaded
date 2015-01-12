// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region STRUCT: EnumeratorWrapper<TIn, TOut>

    /// <summary>
    /// Wraps an <see cref="IEnumerator{T}" /> for an input type
    /// to use it as enumerator for an output type.
    /// </summary>
    /// <typeparam name="TIn">Input type.</typeparam>
    /// <typeparam name="TOut">Output type.</typeparam>
    public struct EnumeratorWrapper<TIn, TOut> : IEnumerator<TOut>
        where TIn : TOut
    {
        #region Fields (1)

        private readonly IEnumerator<TIn> _ENUMERATOR;

        #endregion Fields (1)

        #region Constructors (2)

        /// <summary>
        /// Creates a new instance of the <see cref="EnumeratorWrapper{TIn, TOut}" /> struct.
        /// </summary>
        /// <param name="seq">The sequence to use.</param>
        /// <exception cref="NullReferenceException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public EnumeratorWrapper(IEnumerable<TIn> seq)
            : this(enumerator: seq.GetEnumerator())
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EnumeratorWrapper{TIn, TOut}" /> struct.
        /// </summary>
        /// <param name="enumerator">The inner enumerator.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enumerator" /> is <see langword="null" />.
        /// </exception>
        public EnumeratorWrapper(IEnumerator<TIn> enumerator)
        {
            if (enumerator == null)
            {
                throw new ArgumentNullException("enumerator");
            }

            this._ENUMERATOR = enumerator;
        }

        #endregion Constructors (2)

        #region Methods (3)

        /// <inheriteddoc />
        public bool MoveNext()
        {
            return this._ENUMERATOR
                       .MoveNext();
        }

        /// <inheriteddoc />
        public void Reset()
        {
            this._ENUMERATOR
                .Reset();
        }

        /// <inheriteddoc />
        public void Dispose()
        {
            this._ENUMERATOR
                .Dispose();
        }

        #endregion Methods (3)

        #region Properties (2)

        /// <inheriteddoc />
        public TOut Current
        {
            get { return this._ENUMERATOR.Current; }
        }

        /// <inheriteddoc />
        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        #endregion Properties (2)
    }

    #endregion STRUCT: EnumeratorWrapper<TIn, TOut>

    #region CLASS: EnumeratorWrapper

    /// <summary>
    /// Factory class for <see cref="EnumeratorWrapper{TIn, TOut}" /> struct.
    /// </summary>
    public static class EnumeratorWrapper
    {
        #region Methods (4)

        /// <summary>
        /// Creates a new instance of the <see cref="EnumeratorWrapper{TIn, TOut}" /> struct.
        /// </summary>
        /// <typeparam name="TIn">Input type.</typeparam>
        /// <param name="seq">The sequence to use.</param>
        /// <exception cref="NullReferenceException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public static EnumeratorWrapper<TIn, object> Create<TIn>(IEnumerable<TIn> seq)
        {
            return Create<TIn, object>(seq: seq);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EnumeratorWrapper{TIn, TOut}" /> struct.
        /// </summary>
        /// <typeparam name="TIn">Input type.</typeparam>
        /// <param name="enumerator">The inner enumerator.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enumerator" /> is <see langword="null" />.
        /// </exception>
        public static EnumeratorWrapper<TIn, object> Create<TIn>(IEnumerator<TIn> enumerator)
        {
            return Create<TIn, object>(enumerator: enumerator);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EnumeratorWrapper{TIn, TOut}" /> struct.
        /// </summary>
        /// <typeparam name="TIn">Input type.</typeparam>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="seq">The sequence to use.</param>
        /// <exception cref="NullReferenceException">
        /// <paramref name="seq" /> is <see langword="null" />.
        /// </exception>
        public static EnumeratorWrapper<TIn, TOut> Create<TIn, TOut>(IEnumerable<TIn> seq)
            where TIn : TOut
        {
            return new EnumeratorWrapper<TIn, TOut>(seq: seq);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="EnumeratorWrapper{TIn, TOut}" /> struct.
        /// </summary>
        /// <typeparam name="TIn">Input type.</typeparam>
        /// <typeparam name="TOut">Output type.</typeparam>
        /// <param name="enumerator">The inner enumerator.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="enumerator" /> is <see langword="null" />.
        /// </exception>
        public static EnumeratorWrapper<TIn, TOut> Create<TIn, TOut>(IEnumerator<TIn> enumerator)
            where TIn : TOut
        {
            return new EnumeratorWrapper<TIn, TOut>(enumerator: enumerator);
        }

        #endregion Methods (4)
    }

    #endregion CLASS: EnumeratorWrapper
}