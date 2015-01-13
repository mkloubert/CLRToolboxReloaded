// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// A data transformer that uses delegates to transform ans restore data.
    /// </summary>
    public sealed partial class DelegateDataTransformer : DataTransformerBase
    {
        #region Fields (4)

        private readonly bool _CAN_RESTORE_DATA;
        private readonly bool _CAN_TRANSFORM_DATA;
        private readonly TransformAction _ON_RESTORE_DATA_ACTION;
        private readonly TransformAction _ON_TRANSFORM_DATA_ACTION;

        #endregion Fields (4)

        #region Constructors (2)

        /// <summary>
        /// Creates a new instance of the <see cref="DelegateDataTransformer" /> class.
        /// </summary>
        /// <param name="transformAction">The transformer action.</param>
        /// <param name="restoreAction">The restore action.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformAction" /> and/or <paramref name="restoreAction" /> are <see langword="null" />.
        /// </exception>
        public DelegateDataTransformer(TransformAction transformAction, TransformAction restoreAction)
            : this(transformAction: transformAction, restoreAction: restoreAction,
                   canTransform: true, canRestore: true)
        {
        }

        private DelegateDataTransformer(TransformAction restoreAction, bool canRestore,
                                        TransformAction transformAction, bool canTransform)
        {
            if (restoreAction == null)
            {
                throw new ArgumentNullException("restoreAction");
            }

            if (transformAction == null)
            {
                throw new ArgumentNullException("transformAction");
            }

            this._ON_RESTORE_DATA_ACTION = restoreAction;
            this._CAN_RESTORE_DATA = canRestore;

            this._ON_TRANSFORM_DATA_ACTION = transformAction;
            this._CAN_TRANSFORM_DATA = canTransform;
        }

        #endregion Constructors (2)

        #region Events and delegates (1)

        /// <summary>
        /// Describes a transformer action.
        /// </summary>
        /// <param name="transformer">The underlying transformer instance.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="src">The source stream.</param>
        /// <param name="dest">The destination stream.</param>
        /// <param name="bufferSize">
        /// The buffer size in bytes for read operation to use.
        /// <see langword="null" /> indicates to use the default.
        /// </param>
        public delegate void TransformAction(DelegateDataTransformer transformer, TransformMode mode,
                                             Stream src, Stream dest, int? bufferSize);

        #endregion Events and delegates (1)

        #region Properties (2)

        /// <inheriteddoc />
        public override bool CanRestoreData
        {
            get { return this._CAN_RESTORE_DATA; }
        }

        /// <inheriteddoc />
        public override bool CanTransformData
        {
            get { return this._CAN_TRANSFORM_DATA; }
        }

        #endregion Properties (2)

        #region Methods (6)

        /// <summary>
        /// Creates a new instance that only restores data.
        /// </summary>
        /// <param name="restoreAction">The action to invoke.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="restoreAction" /> is <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateRestorer(TransformAction restoreAction)
        {
            return new DelegateDataTransformer(transformAction: NotImplementedAction, canTransform: false,
                                               restoreAction: restoreAction, canRestore: true);
        }

        /// <summary>
        /// Creates a new instance that only restores data.
        /// </summary>
        /// <param name="restorer">The transformer to use.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="restorer" /> is <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateRestorer(IDataTransformer restorer)
        {
            if (restorer == null)
            {
                throw new ArgumentNullException("restorer");
            }

            return CreateRestorer(restoreAction: (t, m,
                                                  src, dest, bufferSize) => restorer.RestoreData(src, dest, bufferSize));
        }

        /// <summary>
        /// Creates a new instance that only transforms data.
        /// </summary>
        /// <param name="transformAction">The action to invoke.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformAction" /> is <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateTransformer(TransformAction transformAction)
        {
            return new DelegateDataTransformer(transformAction: transformAction, canTransform: true,
                                               restoreAction: NotImplementedAction, canRestore: false);
        }

        /// <summary>
        /// Creates a new instance that only transforms data.
        /// </summary>
        /// <param name="transformer">The transformer to use.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformer" /> is <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateTransformer(IDataTransformer transformer)
        {
            if (transformer == null)
            {
                throw new ArgumentNullException("transformer");
            }

            return CreateTransformer(transformAction: (t, m,
                                                       src, dest, bufferSize) => transformer.TransformData(src, dest, bufferSize));
        }

        private static void NotImplementedAction(DelegateDataTransformer transformer, TransformMode mode,
                                                 Stream src, Stream dest, int? bufferSize)
        {
            throw new NotImplementedException();
        }

        /// <inheriteddoc />
        protected override void OnRestoreData(Stream src, Stream dest, int? bufferSize)
        {
            this._ON_RESTORE_DATA_ACTION(this, TransformMode.Restore,
                                         src, dest, bufferSize);
        }

        /// <inheriteddoc />
        protected override void OnTransformData(Stream src, Stream dest, int? bufferSize)
        {
            this._ON_TRANSFORM_DATA_ACTION(this, TransformMode.Transform,
                                           src, dest, bufferSize);
        }

        #endregion Methods (6)
    }
}