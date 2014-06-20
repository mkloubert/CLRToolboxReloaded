// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Windows.Input;
using System.Net;

namespace MarcelJoachimKloubert.FileBox.Client.ViewModels
{
    /// <summary>
    /// The view model for login process.
    /// </summary>
    public sealed class LoginViewModel : NotifiableBase
    {
        #region Constructors (1)

        internal LoginViewModel(MainViewModel parent)
        {
            this.Initialize();

            this.Parent = parent;
        }

        #endregion Constructors (1)

        #region Properties (7)

        /// <summary>
        /// Gets the command for cancceling login.
        /// </summary>
        public SimpleCommand CancelCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the host address of the server.
        /// </summary>
        public string HostAddress
        {
            get { return this.Get<string>(); }

            set { this.Set(value); }
        }

        /// <summary>
        /// Gets the command for the login process.
        /// </summary>
        public SimpleCommand LoginCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parent vide model.
        /// </summary>
        public MainViewModel Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the TCP port of the server.
        /// </summary>
        public string Port
        {
            get { return this.Get<string>(); }

            set { this.Set(value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the password should be saved or not.
        /// </summary>
        public bool? SavePassword
        {
            get { return this.Get<bool?>(); }

            set { this.Set(value); }
        }

        /// <summary>
        /// Gets or sets the username for the server.
        /// </summary>
        public string Username
        {
            get { return this.Get<string>(); }

            set { this.Set(value); }
        }

        #endregion Properties (7)

        #region Methods (5)

        private void Cancel()
        {
            this.Parent.OnCancel();
        }

        [ReceiveValueFrom("HostAddress")]
        [ReceiveValueFrom("Port")]
        [ReceiveValueFrom("Username")]
        private void CheckCanExecuteOfLoginCommand()
        {
            this.LoginCommand.RaiseCanExecuteChanged();
        }

        private void Login()
        {
            this.Parent.OnLogin();
        }

        private bool Login_CanExecute()
        {
            var host = (this.HostAddress ?? string.Empty).Trim();
            if (host != string.Empty)
            {
                int port;
                if (int.TryParse((this.Port ?? string.Empty).Trim(), out port))
                {
                    if ((port >= IPEndPoint.MinPort) && (port <= IPEndPoint.MaxPort))
                    {
                        var user = (this.Username ?? string.Empty).Trim();
                        if (user != string.Empty)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void Initialize()
        {
            this.CancelCommand = new SimpleCommand(executeAction: this.Cancel);
            this.LoginCommand = new SimpleCommand(executeAction: this.Login,
                                                  canExecutePredicate: this.Login_CanExecute);

            this.Reset();
        }

        internal void Reset()
        {
            this.HostAddress = "localhost";
            this.Port = "44302";

            this.Username = "mkloubert";

            this.SavePassword = false;
        }

        #endregion Methods (5)
    }
}