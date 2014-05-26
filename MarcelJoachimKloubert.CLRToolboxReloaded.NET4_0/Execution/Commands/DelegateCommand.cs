﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Execution.Commands
{
    #region CLASS: DelegateCommand<TParam>

    /// <summary>
    /// A command that uses delegates.
    /// </summary>
    /// <typeparam name="TParam">Type of the parameters.</typeparam>
    public class DelegateCommand<TParam> : CommandBase<TParam>
    {
        #region Fields (2)

        private readonly CanExecutePredicate _CAN_EXECUTE_PREDICATE;
        private readonly ExecuteHandler _EXECUTE_ACTION;

        #endregion CLASS: DelegateCommand<TParam>

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{TParam}"/> class.
        /// </summary>
        /// <param name="executeAction">The logic for <see cref="DelegateCommand{TParam}.OnExecute(TParam)" />.</param>
        /// <param name="canExecutePredicate">The logic for <see cref="DelegateCommand{TParam}.CanExecute(TParam)" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="executeAction" /> is <see langword="null" />.
        /// </exception>
        public DelegateCommand(ExecuteHandler executeAction,
                               CanExecutePredicate canExecutePredicate)
            : base(synchronized: false)
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }

            this._EXECUTE_ACTION = executeAction;
            this._CAN_EXECUTE_PREDICATE = canExecutePredicate ?? base.CanExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand{TParam}"/> class.
        /// </summary>
        /// <param name="executeAction">The logic for <see cref="DelegateCommand{TParam}.OnExecute(TParam)" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="executeAction" /> is <see langword="null" />.
        /// </exception>
        public DelegateCommand(ExecuteHandler executeAction)
            : this(executeAction: executeAction,
                   canExecutePredicate: null)
        {
        }

        #endregion Constructors

        #region Delegates and Events (2)

        // Delegates (2) 

        /// <summary>
        /// Describes a predicate for the <see cref="DelegateCommand{TParam}.CanExecute(TParam)" /> method.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>Command can be executed with the parameter in <paramref name="param" /> or not.</returns>
        public delegate bool CanExecutePredicate(TParam param);

        /// <summary>
        /// Describes a predicate for the <see cref="DelegateCommand{TParam}.OnExecute(TParam)" /> method.
        /// </summary>
        /// <param name="param">The parameter.</param>
        public delegate void ExecuteHandler(TParam param);

        #endregion Delegates and Events

        #region Methods (2)

        // Public Methods (2) 

        /// <inheriteddoc />
        public override bool CanExecute(TParam param)
        {
            return this._CAN_EXECUTE_PREDICATE(param);
        }

        /// <inheriteddoc />
        protected override sealed void OnExecute(TParam param)
        {
            this._EXECUTE_ACTION(param);
        }

        #endregion Methods
    }

    #endregion

    #region CLASS: DelegateCommand

    /// <summary>
    /// A command that uses delegates.
    /// </summary>
    public class DelegateCommand : DelegateCommand<object>
    {
        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="executeAction">The logic for <see cref="DelegateCommand{TParam}.OnExecute(TParam)" />.</param>
        /// <param name="canExecutePredicate">The logic for <see cref="DelegateCommand{TParam}.CanExecute(TParam)" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="executeAction" /> is <see langword="null" />.
        /// </exception>
        public DelegateCommand(ExecuteHandler executeAction,
                               CanExecutePredicate canExecutePredicate)
            : base(executeAction, canExecutePredicate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand" /> class.
        /// </summary>
        /// <param name="executeAction">The logic for <see cref="DelegateCommand{TParam}.OnExecute(TParam)" />.</param>
        /// <param name="canExecutePredicate">The logic for <see cref="DelegateCommand{TParam}.CanExecute(TParam)" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="executeAction" /> is <see langword="null" />.
        /// </exception>
        public DelegateCommand(ExecuteHandlerNoParameter executeAction,
                               CanExecuteHandlerNoParameter canExecutePredicate)
            : this(delegate(object param) { executeAction(); },
                   delegate(object param) { return canExecutePredicate != null ? canExecutePredicate() : true; })
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand" /> class.
        /// </summary>
        /// <param name="executeAction">The logic for <see cref="DelegateCommand{TParam}.OnExecute(TParam)" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="executeAction" /> is <see langword="null" />.
        /// </exception>
        public DelegateCommand(ExecuteHandler executeAction)
            : base(executeAction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand" /> class.
        /// </summary>
        /// <param name="executeAction">The logic for <see cref="DelegateCommand{TParam}.OnExecute(TParam)" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="executeAction" /> is <see langword="null" />.
        /// </exception>
        public DelegateCommand(ExecuteHandlerNoParameter executeAction)
            : this(delegate(object param) { executeAction(); })
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }
        }

        #endregion Constructors

        #region Delegates and Events (2)

        // Delegates (2) 

        /// <inheriteddoc />
        public delegate bool CanExecuteHandlerNoParameter();

        /// <inheriteddoc />
        public delegate void ExecuteHandlerNoParameter();

        #endregion Delegates and Events
    }

    #endregion
}