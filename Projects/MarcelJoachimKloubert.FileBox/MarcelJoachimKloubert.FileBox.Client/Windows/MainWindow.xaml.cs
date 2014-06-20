// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MahApps.Metro.Controls;
using MarcelJoachimKloubert.CLRToolbox.Extensions.Windows;
using MarcelJoachimKloubert.FileBox.Client.Controls;
using MarcelJoachimKloubert.FileBox.Client.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;

namespace MarcelJoachimKloubert.FileBox.Client.Windows
{
    /// <summary>
    /// Code behind for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            // view model
            {
                var vm = new MainViewModel();
                vm.ErrorsReceived += this.ViewModel_ErrorsReceived;
                vm.LoggedIn += this.ViewModel_LoggedIn;
                vm.LoggedInCanceled += this.ViewModel_LoggedInCanceled;

                this.ViewModel = vm;
            }
        }

        #endregion Constructors (1)

        #region Events and delegates (3)

        private void ViewModel_ErrorsReceived(object sender, ErrorEventArgs e)
        {
            try
            {
                var ex = e.GetException();

                this.Invoke((win, state) =>
                    {
                        MessageBox.Show(owner: win,
                                        messageBoxText: state.Exception.Message ?? string.Empty,
                                        caption: state.Exception.GetType().FullName,
                                        button: MessageBoxButton.OK,
                                        icon: MessageBoxImage.Error);
                    }, actionState: new
                    {
                        Exception = ex.GetBaseException() ?? ex,
                    }, prio: DispatcherPriority.Background);
            }
            catch
            {
                // ignore here
            }
        }

        private void ViewModel_LoggedIn(object sender, EventArgs e)
        {
            this.BeginInvoke((win) =>
                {
                    win.TransitioningContentControl_Main
                       .Content = new ServerViewControl();
                });
        }

        private void ViewModel_LoggedInCanceled(object sender, EventArgs e)
        {
            this.Invoke((win) => win.Close());
        }

        #endregion Events and delegates (3)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying view model.
        /// </summary>
        public MainViewModel ViewModel
        {
            get { return this.Invoke((win) => win.DataContext as MainViewModel); }

            private set
            {
                this.Invoke((win, state) =>
                    {
                        win.DataContext = state.NewValue;
                    }, actionState: new
                    {
                        NewValue = value,
                    });
            }
        }

        #endregion Properties (1)
    }
}