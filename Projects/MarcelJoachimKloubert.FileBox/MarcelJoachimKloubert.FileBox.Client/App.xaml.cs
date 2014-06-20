// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.FileBox.Client.Windows;
using System;
using System.Net;
using System.Windows;

namespace MarcelJoachimKloubert.FileBox.Client
{
    /// <summary>
    /// Code behind for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constructors (1)

        private App()
        {
        }

        #endregion Constructors (1)

        #region Properties (2)

        /// <inheriteddoc />
        public static new App Current
        {
            get { return (global::MarcelJoachimKloubert.FileBox.Client.App)Application.Current; }
        }

        /// <inheriteddoc />
        public new MainWindow MainWindow
        {
            get { return (global::MarcelJoachimKloubert.FileBox.Client.Windows.MainWindow)base.MainWindow; }
        }

        #endregion Properties (2)

        #region Methods (1)

        [STAThread]
        private static int Main(string[] args)
        {
            var a = new App();
            a.InitializeComponent();

            //TODO: handle whitelist by user!!!
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                {
                    return true;
                };

            return a.Run(new MainWindow());
        }

        #endregion Methods (1)
    }
}