// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarcelJoachimKloubert.CLRToolbox.Scripting
{
    partial class ScriptExecutorBase
    {
        #region Nested Classes (1)

        /// <summary>
        /// Simple implementation of <see cref="IScriptExecutionContext" /> interface.
        /// </summary>
        protected sealed class ScriptExecutionContext : NotifiableBase, IScriptExecutionContext
        {
            #region Properties (11)

            /// <inheriteddoc />
            public DateTimeOffset? EndTime
            {
                get { return this.Get(() => this.EndTime); }

                private set { this.Set(value, () => this.EndTime); }
            }

            /// <inheriteddoc />
            public IList<Exception> Exceptions
            {
                get { return this.Get(() => this.Exceptions); }

                private set { this.Set(value, () => this.Exceptions); }
            }

            /// <inheriteddoc />
            public ScriptExecutorBase Executor
            {
                get { return this.Get(() => this.Executor); }

                internal set { this.Set(value, () => this.Executor); }
            }

            IScriptExecutor IScriptExecutionContext.Executor
            {
                get { return this.Executor; }
            }

            /// <inheriteddoc />
            public bool HasFailed
            {
                get
                {
                    var exList = this.Exceptions;

                    return (exList != null) &&
                           exList.Where(ex => ex != null)
                                 .Any();
                }
            }

            /// <inheriteddoc />
            public bool IsDebug
            {
                get { return this.Get(() => this.IsDebug); }

                internal set { this.Set(value, () => this.IsDebug); }
            }

            /// <inheriteddoc />
            public bool IsExecuting
            {
                get { return this.Get(() => this.IsExecuting); }

                private set { this.Set(value, () => this.IsExecuting); }
            }

            /// <inheriteddoc />
            public object Result
            {
                get { return this.Get(() => this.Result); }

                internal set { this.Set(value, () => this.Result); }
            }

            /// <inheriteddoc />
            public string Source
            {
                get { return this.Get(() => this.Source); }

                set { this.Set(value, () => this.Source); }
            }

            /// <inheriteddoc />
            public StartActionHandler StartAction
            {
                get { return this.Get(() => this.StartAction); }

                set { this.Set(value, () => this.StartAction); }
            }

            /// <inheriteddoc />
            public DateTimeOffset? StartTime
            {
                get { return this.Get(() => this.StartTime); }

                private set { this.Set(value, () => this.StartTime); }
            }

            #endregion Properties

            #region Delegates and Events (4)

            /// <inheriteddoc />
            public event EventHandler Completed;

            /// <inheriteddoc />
            public event EventHandler Failed;

            /// <summary>
            /// Describes a handler for the <see cref="ScriptExecutionContext.Start()" /> method.
            /// </summary>
            public delegate void StartActionHandler();

            /// <inheriteddoc />
            public event EventHandler Succeed;

            #endregion Delegates and Events

            #region Methods (1)

            // Public Methods (1) 

            /// <summary>
            ///
            /// </summary>
            /// <see cref="IScriptExecutionContext.Start()" />
            public void Start()
            {
                try
                {
                    this.StartTime = AppTime.Now;
                    this.IsExecuting = true;

                    StartActionHandler action = this.StartAction;
                    if (action != null)
                    {
                        action();
                    }

                    this.Exceptions = null;

                    this.RaiseEventHandler(this.Succeed);
                }
                catch (Exception ex)
                {
                    this.Exceptions = new Exception[] { ex };

                    this.RaiseEventHandler(this.Failed);
                }
                finally
                {
                    this.EndTime = AppTime.Now;
                    this.IsExecuting = false;

                    this.RaiseEventHandler(this.Completed);
                }
            }

            #endregion Methods
        }

        #endregion Nested Classes
    }
}