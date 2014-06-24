// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// A wrapper for a <see cref="Stream" /> that keeps sure that <see cref="Stream.Dispose()" /> method of
    /// <see cref="StreamWrapperBase{TStream}._BASE_STREAM" /> field is NOT called from here.
    /// </summary>
    public sealed partial class NonDisposableStream : StreamWrapperBase
    {
        #region Fields (1)

        private readonly CallBehaviour _CALL_BEHAVIOUR;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="NonDisposableStream" /> class.
        /// </summary>
        /// <param name="baseStream">The value for <see cref="StreamWrapperBase{TStream}._BASE_STREAM" /> field.</param>
        /// <param name="callBehaviour">
        /// The behaviour of <see cref="Stream.Dispose(bool)" /> method.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="baseStream" /> is <see langword="null" />.
        /// </exception>
        public NonDisposableStream(Stream baseStream,
                                   CallBehaviour callBehaviour = CallBehaviour.CallFinalizerPart)
            : base(baseStream)
        {
            this._CALL_BEHAVIOUR = callBehaviour;
        }

        #endregion Constructors

        #region Methods (1)

        /// <inheriteddoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._CALL_BEHAVIOUR.HasFlag(CallBehaviour.CallDisposePart))
                {
                    this.InvokeDispose(true);
                }
            }
            else
            {
                if (this._CALL_BEHAVIOUR.HasFlag(CallBehaviour.CallFinalizerPart))
                {
                    this.InvokeDispose(false);
                }
            }
        }

        #endregion Methods
    }
}