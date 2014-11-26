// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Collections.Generic;
using MarcelJoachimKloubert.CLRToolbox.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Functions
{
    /// <summary>
    /// A basic dedicated function.
    /// </summary>
    public abstract partial class FunctionBase : IdentifiableBase, IFunction
    {
        #region Fields (2)

        private readonly Guid _ID;
        private readonly Action<FunctionExecutionContext> _ON_EXECUTE_FUNC;

        #endregion Fields (2)

        #region Constrcutors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBase" /> class.
        /// </summary>
        /// <param name="id">The value for the <see cref="FunctionBase.Id" /> property.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected FunctionBase(Guid id, bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
            this._ID = id;

            this._ON_EXECUTE_FUNC = this._IS_SYNCHRONIZED ? new Action<FunctionExecutionContext>(this.OnExecute_ThreadSafe)
                                                          : new Action<FunctionExecutionContext>(this.OnExecute);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBase" /> class.
        /// </summary>
        /// <param name="id">The value for the <see cref="FunctionBase.Id" /> property.</param>
        /// <param name="isSynchronized">The value for the <see cref="ObjectBase.IsSynchronized" /> property.</param>
        protected FunctionBase(Guid id, bool isSynchronized)
            : this(id: id,
                   isSynchronized: isSynchronized,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBase" /> class.
        /// </summary>
        /// <param name="id">The value for the <see cref="FunctionBase.Id" /> property.</param>
        /// <param name="sync">The reference for the <see cref="ObjectBase.SyncRoot" /> property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        protected FunctionBase(Guid id, object sync)
            : this(id: id,
                   isSynchronized: false,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionBase" /> class.
        /// </summary>
        /// <param name="id">The value for the <see cref="FunctionBase.Id" /> property.</param>
        protected FunctionBase(Guid id)
            : this(id: id,
                   isSynchronized: false)
        {
        }

        #endregion Constrcutors (4)

        #region Properties (6)

        /// <summary>
        /// Gets the name of result parameter for the error message.
        /// </summary>
        protected virtual string ErrorParameter
        {
            get { return "Error"; }
        }

        /// <summary>
        /// Gets the name of result parameter for the exit code.
        /// </summary>
        protected virtual string ExitCodeParameter
        {
            get { return "ExitCode"; }
        }

        /// <inheriteddoc />
        public string Fullname
        {
            get
            {
                var ns = this.Namespace;
                ns = string.IsNullOrWhiteSpace(ns) ? null : ns.Trim();

                var name = this.Name;
                name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();

                return string.Format("{0}{1}{2}",
                                     ns,
                                     ns != null ? "." : null,
                                     name);
            }
        }

        /// <inheriteddoc />
        public sealed override Guid Id
        {
            get { return this._ID; }
        }

        /// <inheriteddoc />
        public virtual string Name
        {
            get { return this.GetType().Name; }
        }

        /// <inheriteddoc />
        public virtual string Namespace
        {
            get { return this.GetType().Namespace; }
        }

        #endregion Properties (6)

        #region Methods (14)

        /// <summary>
        /// Creates an empty an initial function execution context instance.
        /// </summary>
        /// <param name="params">The input parameters.</param>
        /// <returns>The created context.</returns>
        protected virtual FunctionExecutionContext CreateExecutionContext(IEnumerable<KeyValuePair<string, object>> @params)
        {
            return new FunctionExecutionContext();
        }

        /// <summary>
        /// Creates and returns the dictionary for first parameter of <see cref="FunctionBase.OnExecute(FunctionExecutionContext)" /> method.
        /// </summary>
        /// <param name="params">The input parameters from <see cref="FunctionBase.Execute(IEnumerable{KeyValuePair{string, object}})" /> method.</param>
        /// <returns>The created dictionary.</returns>
        protected virtual IReadOnlyDictionary<string, object> CreateInputParameterStorage(IEnumerable<KeyValuePair<string, object>> @params)
        {
            IReadOnlyDictionary<string, object> result;
            {
                // keep sure to have a case insensitive dictionary
                var dict = new Dictionary<string, object>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                                emptyIsNull: true));
                @params.ForEach(action: (ctx) => ctx.State
                                                    .Dictionary.Add(ctx.Item),
                                actionState: new
                                {
                                    Dictionary = (IDictionary<string, object>)dict,
                                });

                result = new ReadOnlyDictionaryWrapper<string, object>(dict);
            }

            return result;
        }

        /// <summary>
        /// Creates and returns the dictionary for second parameter of <see cref="FunctionBase.OnExecute(FunctionExecutionContext)" /> method.
        /// </summary>
        /// <param name="input">The submitted input parameters.</param>
        /// <returns>The created dictionary.</returns>
        protected virtual IDictionary<string, object> CreateResultParameterStorage(IReadOnlyDictionary<string, object> input)
        {
            return new Dictionary<string, object>(comparer: EqualityComparerFactory.CreateCaseInsensitiveStringComparer(trim: true,
                                                                                                                        emptyIsNull: true));
        }

        /// <inheriteddoc />
        public IDictionary<string, object> Execute(IEnumerable<KeyValuePair<string, object>> @params = null)
        {
            @params = @params ?? Enumerable.Empty<KeyValuePair<string, object>>();

            var ctx = this.CreateExecutionContext(@params);
            ctx.Input = this.CreateInputParameterStorage(@params);
            ctx.Result = this.CreateResultParameterStorage(ctx.Input);

            string errMsg = null;
            try
            {
                ctx.ExitCode = this.GetInitialExitCode(ctx.Input);
                this.InitializeExecutionContext(ctx);

                ctx.StartTime = AppTime.Now;
                this._ON_EXECUTE_FUNC(ctx);
                ctx.EndTime = AppTime.Now;

                ctx.HasBeenFailed = false;

                this.OnExecutionSucceeded(ctx);
            }
            catch (Exception ex)
            {
                ctx.EndTime = AppTime.Now;
                
                ctx.HasBeenFailed = true;
                ctx.Error = ex;

                ctx.ExitCode = this.GetExitCode(ctx, ex);

                // error message
                {
                    var msg = new StringBuilder();
                    this.GenerateErrorMessage(ctx, ex, ref msg);

                    if (msg != null)
                    {
                        errMsg = msg.ToString();
                    }
                }

                this.OnExecutionError(ctx, ex);
            }
            finally
            {
                if (ctx.ExitCode.HasValue)
                {
                    ctx.Result[this.ExitCodeParameter] = ctx.ExitCode.Value;
                }

                if (errMsg != null)
                {
                    ctx.Result[this.ErrorParameter] = errMsg;
                }

                this.OnExecutionCompleted(ctx);
            }

            return ctx.Result;
        }

        /// <summary>
        /// Generates a message for an exception.
        /// </summary>
        /// <param name="ctx">The underlying execution context.</param>
        /// <param name="ex">The underlying exception.</param>
        /// <param name="message">
        /// The <see cref="StringBuilder" /> were to write the message to.
        /// </param>
        protected virtual void GenerateErrorMessage(FunctionExecutionContext ctx, Exception ex, ref StringBuilder message)
        {
            message.Append(ex.Message);
        }

        /// <summary>
        /// Returns the exit code for a specific exception.
        /// </summary>
        /// <param name="ctx">The underlying execution context.</param>
        /// <param name="ex">The exception.</param>
        /// <returns>The exit code.</returns>
        protected virtual int? GetExitCode(FunctionExecutionContext ctx, Exception ex)
        {
            return -1;
        }

        /// <summary>
        /// Returns the initial exit code code based on the input parameters.
        /// </summary>
        /// <param name="input">The input parameters.</param>
        /// <returns>The exit code.</returns>
        protected virtual int? GetInitialExitCode(IReadOnlyDictionary<string, object> input)
        {
            return 0;
        }

        /// <summary>
        /// Initializes an execution context.
        /// </summary>
        /// <param name="ctx">The context to initialize.</param>
        protected virtual void InitializeExecutionContext(FunctionExecutionContext ctx)
        {
            // dummy
        }

        /// <summary>
        /// Stores the logic for the <see cref="FunctionBase.Execute(IEnumerable{KeyValuePair{string, object}})" /> method.
        /// </summary>
        /// <param name="ctx">The execution context.</param>
        protected abstract void OnExecute(FunctionExecutionContext ctx);

        private void OnExecute_ThreadSafe(FunctionExecutionContext ctx)
        {
            lock (this._SYNC)
            {
                this.OnExecute(ctx);
            }
        }

        /// <summary>
        /// Is invoked AFTER execution was done, even if execution was successful or not.
        /// It is executed after <see cref="FunctionBase.OnExecutionSucceeded(FunctionExecutionContext)" /> and
        /// <see cref="FunctionBase.OnExecutionError(FunctionExecutionContext, Exception)" /> methods.
        /// </summary>
        /// <param name="ctx">The underlying execution context.</param>
        protected virtual void OnExecutionCompleted(FunctionExecutionContext ctx)
        {
            // dummy
        }

        /// <summary>
        /// Is invoked AFTER exception was thrown and exception exit code was set.
        /// </summary>
        /// <param name="ctx">The underlying execution context.</param>
        /// <param name="ex">The thrown exception(s).</param>
        protected virtual void OnExecutionError(FunctionExecutionContext ctx, Exception ex)
        {
            // dummy
        }

        /// <summary>
        /// Is invoked AFTER execution was successful.
        /// </summary>
        /// <param name="ctx">The underlying execution context.</param>
        protected virtual void OnExecutionSucceeded(FunctionExecutionContext ctx)
        {
            // dummy
        }

        #endregion Methods (13)
    }
}