// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Windows.Collections.ObjectModel;
using System;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.FileBox.Client.ViewModels
{
    /// <summary>
    /// The main view model.
    /// </summary>
    public sealed class MainViewModel : NotifiableBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            this.Initialize();
        }

        #endregion Constructors (1)

        #region Events (2)

        /// <summary>
        /// Is invoked if log in failed.
        /// </summary>
        public event EventHandler LoggedIn;

        /// <summary>
        /// Is invoked if log in was cancelled.
        /// </summary>
        public event EventHandler LoggedInCanceled;

        #endregion Events (2)

        #region Properties (3)

        /// <summary>
        /// Gets if the application is currently busy or not.
        /// </summary>
        public bool IsBusy
        {
            get { return this.Get<bool>(); }

            private set { this.Set(value); }
        }

        /// <summary>
        /// Gets the view model for the login (control).
        /// </summary>
        public LoginViewModel Login
        {
            get { return this.Get<LoginViewModel>(); }

            private set { this.Set(value); }
        }

        /// <summary>
        /// Gets the view model for the server (control).
        /// </summary>
        public ServerViewModel Server
        {
            get { return this.Get<ServerViewModel>(); }

            private set { this.Set(value); }
        }

        #endregion Properties (2)

        #region Methods (5)

        private void Initialize()
        {
            this.Login = new LoginViewModel(this);
        }

        internal void OnBusy(Action<MainViewModel> action)
        {
            this.OnBusy<Action<MainViewModel>>(action: (vm, a) => a(vm),
                                               actionState: action);
        }

        internal void OnBusy<TState>(Action<MainViewModel, TState> action, TState actionState)
        {
            Task.Factory.StartNew((state) =>
                {
                    try
                    {
                        this.IsBusy = true;

                        var tuple = (Tuple<Action<MainViewModel, TState>, TState>)state;

                        var a = tuple.Item1;    // action
                        var @as = tuple.Item2;    // actionState

                        a(this, @as);
                    }
                    finally
                    {
                        this.IsBusy = false;
                    }
                }, state: Tuple.Create<Action<MainViewModel, TState>, TState>(action,
                                                                              actionState));
        }

        internal void OnCancel()
        {
            this.RaiseEventHandler(this.LoggedInCanceled);
        }

        internal void OnLogin()
        {
            this.OnBusy((vm) =>
                {
                    try
                    {
                        var conn = new FileBoxConnection();
                        conn.Host = vm.Login.HostAddress;
                        conn.Port = int.Parse(vm.Login.Port.Trim());
                        conn.User = vm.Login.Username.Trim();

                        // test connection
                        var info = conn.GetServerInfo();

                        this.Server = new ServerViewModel(this, conn);


                        vm.RaiseEventHandler(vm.LoggedIn);
                    }
                    catch (Exception ex)
                    {
                        this.Server = null;

                        vm.OnErrorsReceived(ex);
                    }
                });
        }

        #endregion Methods (5)
    }
}