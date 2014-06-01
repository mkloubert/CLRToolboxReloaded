﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Commands
{
    /// <summary>
    /// A basic command.
    /// </summary>
    /// <typeparam name="TParam">Type of the parameters.</typeparam>
    public abstract class CommandBase<TParam> : ObjectBase, ICommand<TParam>
    {
        #region Constrcutors (4)

        /// <inheriteddoc />
        protected CommandBase(bool isSynchronized, object sync)
            : base(isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected CommandBase(bool isSynchronized)
            : base(isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected CommandBase(object sync)
            : base(sync: sync)
        {
        }

        /// <inheriteddoc />
        protected CommandBase()
            : base()
        {
        }

        #endregion Constrcutors (4)

        #region Delegates and Events (2)

        // Events (2) 

        /// <inheriteddoc />
        public event EventHandler CanExecuteChanged;

        /// <inheriteddoc />
        public event EventHandler<ExecutionErrorEventArgs<TParam>> ExecutionError;

        #endregion Delegates and Events

        #region Methods (5)

        // Public Methods (3) 

        /// <inheriteddoc />
        public virtual bool CanExecute(TParam param)
        {
            return true;
        }

        /// <inheriteddoc />
        public bool? Execute(TParam param)
        {
            if (this.CanExecute(param) == false)
            {
                return null;
            }

            try
            {
                this.OnExecute(param);
                return true;
            }
            catch (Exception ex)
            {
                if (this.RaiseExecutionError(param, ex) == false)
                {
                    // re-throw exception because no event was raised
                    throw;
                }

                return false;
            }
        }

        /// <summary>
        /// Raises the <see cref="CommandBase{TParam}.CanExecuteChanged" /> event.
        /// </summary>
        /// <returns>Event was raised or not.</returns>
        public bool RaiseCanExecuteChanged()
        {
            EventHandler handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
                return true;
            }

            return false;
        }

        // Protected Methods (1) 

        /// <summary>
        /// The logic of the <see cref="CommandBase{TParam}.Execute(TParam)" /> method.
        /// </summary>
        /// <param name="param">The parameter for the execution.</param>
        protected abstract void OnExecute(TParam param);

        // Private Methods (1) 

        private bool RaiseExecutionError(TParam param, Exception ex)
        {
            EventHandler<ExecutionErrorEventArgs<TParam>> handler = this.ExecutionError;
            if (handler != null)
            {
                handler(this, new ExecutionErrorEventArgs<TParam>(param, ex));
                return true;
            }

            return false;
        }

        #endregion Methods
    }
}