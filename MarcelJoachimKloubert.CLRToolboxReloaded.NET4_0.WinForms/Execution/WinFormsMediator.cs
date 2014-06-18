// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Execution;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MarcelJoachimKloubert.CLRToolbox.Windows.Forms.Execution
{
    /// <summary>
    /// A mediator that uses a <see cref="Control" /> for invoking logic on the UI thread.
    /// </summary>
    public sealed class WinFormsMediator : Mediator
    {
        #region Constructors (4)

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator" /> class.
        /// </summary>
        public WinFormsMediator()
            : this(provider: GetMainForm)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the control for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> is <see langword="null" />.
        /// </exception>
        public WinFormsMediator(ControlProvider provider)
            : this(provider: provider,
                   sync: new object())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator" /> class.
        /// </summary>
        /// <param name="sync">The unique object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sync" /> is <see langword="null" />.
        /// </exception>
        public WinFormsMediator(object sync)
            : this(provider: GetMainForm,
                   sync: sync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator" /> class.
        /// </summary>
        /// <param name="provider">The function that provides the control for thread safe operations.</param>
        /// <param name="sync">The unique object for thread safe operations.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public WinFormsMediator(ControlProvider provider, object sync)
            : base(uiAction: ToUIAction(provider),
                   sync: sync)
        {
        }

        #endregion Constructors (4)

        #region Events and delegates (1)

        /// <summary>
        /// Describes a function that provides the control for the UI operations.
        /// </summary>
        /// <param name="mediator">The underlying mediator instance.</param>
        /// <returns>The control for the thread safe UI operations.</returns>
        public delegate Control ControlProvider(WinFormsMediator mediator);

        #endregion Events and delegates

        #region Methods (5)
        
        /// <summary>
        /// Creates a new instance by defining a specific control.
        /// </summary>
        /// <param name="ctrl">The underlying control.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctrl" /> is <see langword="null" />.
        /// </exception>
        public static WinFormsMediator Create(Control ctrl)
        {
            return Create(ctrl: ctrl,
                          sync: new object());
        }

        /// <summary>
        /// Creates a new instance by defining a specific control.
        /// </summary>
        /// <param name="ctrl">The underlying control.</param>
        /// <param name="sync">The unique object for thread safe operations.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ctrl" /> and/or <paramref name="sync" /> are <see langword="null" />.
        /// </exception>
        public static WinFormsMediator Create(Control ctrl, object sync)
        {
            if (ctrl == null)
            {
                throw new ArgumentNullException("ctrl");
            }

            return new WinFormsMediator(provider: (m) => ctrl,
                                        sync: sync);
        }

        private static Control GetMainForm(WinFormsMediator mediator)
        {
            return Control.FromHandle(Process.GetCurrentProcess()
                                             .MainWindowHandle);
        }

        private void InnerUIAction(ControlProvider provider, Action action)
        {
            var ctrl = provider(this);

            if ((ctrl != null) &&
                ctrl.InvokeRequired)
            {
                ctrl.Invoke(new Action<ControlProvider, Action>(this.InnerUIAction),
                            provider, action);

                return;
            }

            action();
        }

        private static UIAction ToUIAction(ControlProvider provider)
        {
            if (provider == null)
            {
                return null;
            }

            return (m, a) =>
                {
                    var wfm = (WinFormsMediator)m;

                    wfm.InnerUIAction(provider, a);
                };
        }

        #endregion Methods (3)
    }
}