// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Jobs
{
    /// <summary>
    /// A basic job.
    /// </summary>
    public abstract class JobBase : IdentifiableBase, IJob
    {
        #region Fields (5)

        private readonly Func<DateTimeOffset, bool> _CAN_EXECUTE_ACTION;
        private readonly Action<IJobExecutionContext> _COMPLETED_ACTION;
        private readonly Action<IJobExecutionContext> _EXECUTE_ACTION;
        private readonly Action<IJobExecutionContext, Exception> _ERROR_ACTION;
        private readonly Guid _ID;

        #endregion Fields

        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="JobBase" /> class.
        /// </summary>
        /// <param name="id">The value for <see cref="JobBase.Id" /> property.</param>
        /// <param name="isSynchronized">Job should handle thread safe or not.</param>
        /// <param name="sync">The value for <see cref="ObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected JobBase(Guid id, bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            this._ID = id;

            if (this._IS_SYNCHRONIZED)
            {
                this._CAN_EXECUTE_ACTION = this.OnCanExecute_ThreadSafe;
                this._COMPLETED_ACTION = this.OnCompleted_ThreadSafe;
                this._ERROR_ACTION = this.OnError_ThreadSafe;
                this._EXECUTE_ACTION = this.OnExecute_ThreadSafe;
            }
            else
            {
                this._CAN_EXECUTE_ACTION = this.OnCanExecute_NonThreadSafe;
                this._COMPLETED_ACTION = this.OnCompleted;
                this._ERROR_ACTION = this.OnError;
                this._EXECUTE_ACTION = this.OnExecute;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobBase" /> class.
        /// </summary>
        /// <param name="id">The value for <see cref="JobBase.Id" /> property.</param>
        /// <param name="sync">The value for <see cref="ObjectBase._SYNC" /> field.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected JobBase(Guid id, object sync)
            : this(id,
                   isSynchronized: false, sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobBase" /> class.
        /// </summary>
        /// <param name="id">The value for <see cref="JobBase.Id" /> property.</param>
        /// <param name="isSynchronized">Job should handle thread safe or not.</param>
        protected JobBase(Guid id, bool isSynchronized)
            : this(id,
                   isSynchronized: isSynchronized, sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobBase" /> class.
        /// </summary>
        /// <param name="id">The value for <see cref="JobBase.Id" /> property.</param>
        protected JobBase(Guid id)
            : this(id, isSynchronized: false)
        {
        }

        #endregion Constructors

        #region Properties (1)

        /// <inheriteddoc />
        public override Guid Id
        {
            get { return this._ID; }
        }

        #endregion Properties

        #region Methods (13)

        /// <inheriteddoc />
        public bool CanExecute(DateTimeOffset time)
        {
            return this._CAN_EXECUTE_ACTION(time);
        }

        /// <inheriteddoc />
        public void Completed(IJobExecutionContext ctx)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }

            this._COMPLETED_ACTION(ctx);
        }

        /// <inheriteddoc />
        public void Error(IJobExecutionContext ctx, Exception ex)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }

            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            this._ERROR_ACTION(ctx, ex);
        }

        /// <inheriteddoc />
        public void Execute(IJobExecutionContext ctx)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }

            this._EXECUTE_ACTION(ctx);
        }

        /// <summary>
        /// The logic for the <see cref="JobBase.CanExecute(DateTimeOffset)" /> method.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="canExecuteJob">
        /// The variable where to write the result for <see cref="JobBase.CanExecute(DateTimeOffset)" /> method.
        /// This value is <see langword="false" /> by default.
        /// </param>
        protected abstract void OnCanExecute(DateTimeOffset time, ref bool canExecuteJob);

        private bool OnCanExecute_NonThreadSafe(DateTimeOffset time)
        {
            var result = false;
            this.OnCanExecute(time, ref result);

            return result;
        }

        private bool OnCanExecute_ThreadSafe(DateTimeOffset time)
        {
            bool result;

            lock (this._SYNC)
            {
                result = this.OnCanExecute_NonThreadSafe(time);
            }

            return result;
        }

        /// <summary>
        /// The logic for the <see cref="JobBase.Completed(IJobExecutionContext)" /> method.
        /// </summary>
        /// <param name="ctx">The underlying context.</param>
        protected virtual void OnCompleted(IJobExecutionContext ctx)
        {
            // dummy
        }

        private void OnCompleted_ThreadSafe(IJobExecutionContext ctx)
        {
            lock (this._SYNC)
            {
                this.OnCompleted(ctx);
            }
        }

        /// <summary>
        /// The logic for the <see cref="JobBase.Error(IJobExecutionContext, Exception)" /> method.
        /// </summary>
        /// <param name="ctx">The underlying context.</param>
        /// <param name="ex">The occured exception(s).</param>
        protected virtual void OnError(IJobExecutionContext ctx, Exception ex)
        {
            // dummy
        }

        private void OnError_ThreadSafe(IJobExecutionContext ctx, Exception ex)
        {
            lock (this._SYNC)
            {
                this.OnError(ctx, ex);
            }
        }

        /// <summary>
        /// The logic for the <see cref="JobBase.Execute(IJobExecutionContext)" /> method.
        /// </summary>
        /// <param name="ctx">The current execution context.</param>
        protected abstract void OnExecute(IJobExecutionContext ctx);

        private void OnExecute_ThreadSafe(IJobExecutionContext ctx)
        {
            lock (this._SYNC)
            {
                this.OnExecute(ctx);
            }
        }

        #endregion Properties
    }
}