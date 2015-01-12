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

        #region Constructors (5)

        /// <summary>
        /// Creates a new instance of the <see cref="DelegateDataTransformer" /> class.
        /// </summary>
        /// <param name="transformAction">The transformer action.</param>
        /// <param name="restoreAction">The restore action.</param>
        /// <param name="isSynchronized">New instance should handle thread safe or not.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformAction" />, <paramref name="restoreAction" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public DelegateDataTransformer(TransformAction transformAction, TransformAction restoreAction,
                                       bool isSynchronized, object sync)
            : this(transformAction: transformAction, restoreAction: restoreAction,
                   canTransform: true, canRestore: true,
                   isSynchronized: isSynchronized, sync: sync)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DelegateDataTransformer" /> class.
        /// </summary>
        /// <param name="transformAction">The transformer action.</param>
        /// <param name="restoreAction">The restore action.</param>
        /// <param name="isSynchronized">New instance should handle thread safe or not.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformAction" /> and/or <paramref name="restoreAction" /> are <see langword="null" />.
        /// </exception>
        public DelegateDataTransformer(TransformAction transformAction, TransformAction restoreAction,
                                       bool isSynchronized)
            : this(transformAction: transformAction, restoreAction: restoreAction,
                   isSynchronized: isSynchronized, sync: new object())
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DelegateDataTransformer" /> class.
        /// </summary>
        /// <param name="transformAction">The transformer action.</param>
        /// <param name="restoreAction">The restore action.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformAction" />, <paramref name="restoreAction" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public DelegateDataTransformer(TransformAction transformAction, TransformAction restoreAction,
                                       object sync)
            : this(transformAction: transformAction, restoreAction: restoreAction,
                   isSynchronized: false)
        {
        }

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
                   sync: new object())
        {
        }

        private DelegateDataTransformer(TransformAction restoreAction, bool canRestore,
                                        TransformAction transformAction, bool canTransform,
                                        bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
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

        #endregion Constructors (5)

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

        #region Methods (17)

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
            return CreateRestorer(restoreAction: restoreAction,
                                  isSynchronized: false);
        }

        /// <summary>
        /// Creates a new instance that only restores data.
        /// </summary>
        /// <param name="restoreAction">The action to invoke.</param>
        /// <param name="isSynchronized">New instance handles thread safe or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="restoreAction" /> is <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateRestorer(TransformAction restoreAction,
                                                             bool isSynchronized)
        {
            return CreateRestorer(restoreAction: restoreAction,
                                  isSynchronized: isSynchronized, sync: new object());
        }

        /// <summary>
        /// Creates a new instance that only restores data.
        /// </summary>
        /// <param name="restoreAction">The action to invoke.</param>
        /// <param name="isSynchronized">New instance handles thread safe or not.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="restoreAction" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateRestorer(TransformAction restoreAction,
                                                             bool isSynchronized, object sync)
        {
            return new DelegateDataTransformer(restoreAction: restoreAction, canRestore: true,
                                               transformAction: NotImplementedAction, canTransform: false,
                                               isSynchronized: isSynchronized, sync: sync);
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
            return CreateRestorer(restoreAction: ToRestoreAction(restorer));
        }

        /// <summary>
        /// Creates a new instance that only restores data.
        /// </summary>
        /// <param name="restorer">The transformer to use.</param>
        /// <param name="isSynchronized">New instance handles thread safe or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="restorer" /> is <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateRestorer(IDataTransformer restorer,
                                                             bool isSynchronized)
        {
            return CreateRestorer(restoreAction: ToRestoreAction(restorer),
                                  isSynchronized: isSynchronized);
        }

        /// <summary>
        /// Creates a new instance that only restores data.
        /// </summary>
        /// <param name="restorer">The transformer to use.</param>
        /// <param name="isSynchronized">New instance handles thread safe or not.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="restorer" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateRestorer(IDataTransformer restorer,
                                                             bool isSynchronized, object sync)
        {
            return CreateRestorer(restoreAction: ToRestoreAction(restorer),
                                  isSynchronized: isSynchronized,
                                  sync: sync);
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
            return CreateTransformer(transformAction: transformAction,
                                     isSynchronized: false);
        }

        /// <summary>
        /// Creates a new instance that only transforms data.
        /// </summary>
        /// <param name="transformAction">The action to invoke.</param>
        /// <param name="isSynchronized">New instance handles thread safe or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformAction" /> is <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateTransformer(TransformAction transformAction,
                                                                bool isSynchronized)
        {
            return CreateTransformer(transformAction: transformAction,
                                     isSynchronized: isSynchronized, sync: new object());
        }

        /// <summary>
        /// Creates a new instance that only transforms data.
        /// </summary>
        /// <param name="transformAction">The action to invoke.</param>
        /// <param name="isSynchronized">New instance handles thread safe or not.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformAction" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateTransformer(TransformAction transformAction,
                                                                bool isSynchronized, object sync)
        {
            return new DelegateDataTransformer(restoreAction: NotImplementedAction, canRestore: false,
                                               transformAction: transformAction, canTransform: true,
                                               isSynchronized: isSynchronized, sync: sync);
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
            return CreateTransformer(transformAction: ToTransformerAction(transformer));
        }

        /// <summary>
        /// Creates a new instance that only transforms data.
        /// </summary>
        /// <param name="transformer">The transformer to use.</param>
        /// <param name="isSynchronized">New instance handles thread safe or not.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformer" /> is <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateTransformer(IDataTransformer transformer,
                                                                bool isSynchronized)
        {
            return CreateTransformer(transformAction: ToTransformerAction(transformer),
                                     isSynchronized: isSynchronized);
        }

        /// <summary>
        /// Creates a new instance that only transforms data.
        /// </summary>
        /// <param name="transformer">The transformer to use.</param>
        /// <param name="isSynchronized">New instance handles thread safe or not.</param>
        /// <param name="sync">The object for thread safe operations.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="transformer" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static DelegateDataTransformer CreateTransformer(IDataTransformer transformer,
                                                                bool isSynchronized, object sync)
        {
            return CreateTransformer(transformAction: ToTransformerAction(transformer),
                                     isSynchronized: isSynchronized,
                                     sync: sync);
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

        private static TransformAction ToRestoreAction(IDataTransformer restorer)
        {
            if (restorer == null)
            {
                return null;
            }

            return (t, m, src, dest, bufferSize) => restorer.RestoreData(src, dest, bufferSize);
        }

        private static TransformAction ToTransformerAction(IDataTransformer transformer)
        {
            if (transformer == null)
            {
                return null;
            }

            return (t, m, src, dest, bufferSize) => transformer.TransformData(src, dest, bufferSize);
        }

        #endregion Methods (17)
    }
}