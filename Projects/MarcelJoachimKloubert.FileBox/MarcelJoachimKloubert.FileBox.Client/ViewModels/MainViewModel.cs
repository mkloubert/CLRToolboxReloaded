// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

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

        #region Properties (4)

        /// <summary>
        /// Gets the full path of the application directory.
        /// </summary>
        public string ApplicationDirectory
        {
            get { return Environment.CurrentDirectory; }
        }

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

        #endregion Properties (3)

        #region Methods (10)

        [ReceiveValueFrom("Server")]
        [ReceiveValueFrom("Login")]
        private void ChildViewModel_Changed(IReceiveValueFromArgs e)
        {
            if (e.OldValue != null)
            {
                e.GetOldValue<NotifiableBase>().ErrorsReceived -= this.ChildViewModel_ErrorsReceived;
            }

            if (e.NewValue != null)
            {
                e.GetNewValue<NotifiableBase>().ErrorsReceived += this.ChildViewModel_ErrorsReceived;
            }
        }

        private void ChildViewModel_ErrorsReceived(object sender, ErrorEventArgs e)
        {
            this.OnErrorsReceived(e.GetException());
        }

        /// <summary>
        /// Returns the key file.
        /// </summary>
        /// <returns>The key file.</returns>
        public FileInfo GetKeyFile()
        {
            bool isNew;
            return this.GetKeyFile(out isNew);
        }

        /// <summary>
        /// Returns the key file.
        /// </summary>
        /// <param name="isNew">The variable that defines if key file has to be created (is new) or not.</param>
        /// <returns>The key file.</returns>
        public FileInfo GetKeyFile(out bool isNew)
        {
            FileInfo result;
            isNew = false;

            lock (this._SYNC)
            {
                result = new FileInfo(Path.Combine(this.ApplicationDirectory, "key.xml"));

                if (result.Exists)
                {
                    // test key file
                    string xml;
                    try
                    {
                        xml = File.ReadAllText(result.FullName,
                                               encoding: Encoding.UTF8);

                        var rsa = new RSACryptoServiceProvider();
                        rsa.FromXmlString(xml);

                        // seems to work
                    }
                    catch
                    {
                        // failed => delete old file

                        result.Delete();
                        result.Refresh();
                    }
                    finally
                    {
                        xml = null;
                    }
                }

                if (result.Exists == false)
                {
                    isNew = true;

                    // generate new key and save
                    var rsa = new RSACryptoServiceProvider(2048);
                    File.WriteAllText(result.FullName,
                                      contents: rsa.ToXmlString(includePrivateParameters: true),
                                      encoding: Encoding.UTF8);
                }
            }

            return result;
        }

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

                        // test connection by loading server information
                        var info = conn.GetServerInfo().Result;

                        // check if a new key file has been created
                        bool isKeyFileNew;
                        var keyFile = vm.GetKeyFile(out isKeyFileNew);

                        Func<string> readKeyXml = () =>
                            {
                                return File.ReadAllText(keyFile.FullName,
                                                         encoding: Encoding.UTF8);
                            };

                        var updateKey = false;
                        if (isKeyFileNew)
                        {
                            // key file is new
                            updateKey = true;
                        }
                        else
                        {
                            var serverRsa = info.TryGetRsaCrypter();
                            if (serverRsa == null)
                            {
                                // server has no key
                                updateKey = true;
                            }
                            else
                            {
                                // check id keys are the same

                                var myRsa = new RSACryptoServiceProvider();
                                myRsa.FromXmlString(readKeyXml());

                                if (serverRsa.ExportCspBlob(false)
                                             .SequenceEqual(myRsa.ExportCspBlob(false)) == false)
                                {
                                    // different keys
                                    updateKey = true;
                                }
                            }
                        }

                        if (updateKey)
                        {
                            // update key on server bewfore continue
                            conn.UpdateKey(xml: readKeyXml());
                        }

                        this.Server = new ServerViewModel(this, info);

                        vm.RaiseEventHandler(vm.LoggedIn);
                    }
                    catch (Exception ex)
                    {
                        this.Server = null;

                        vm.OnErrorsReceived(ex);
                    }
                });
        }

        [ReceiveValueFrom("Server")]
        private void Server_Changed(IReceiveValueFromArgs e)
        {
            var vm = e.GetNewValue<ServerViewModel>();
            if (vm == null)
            {
                return;
            }

            vm.ReloadInbox();
        }

        #endregion Methods (8)
    }
}