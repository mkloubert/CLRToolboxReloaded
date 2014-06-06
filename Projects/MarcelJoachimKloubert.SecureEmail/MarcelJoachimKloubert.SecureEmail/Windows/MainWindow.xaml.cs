// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MahApps.Metro.Controls;
using MarcelJoachimKloubert.SecureEmail.ViewModels;

namespace MarcelJoachimKloubert.SecureEmail.Windows
{
    /// <summary>
    /// Code behind for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Constructor (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.ViewModel = new MainViewModel();
        }

        #endregion Constructor (1)

        #region Properties (1)

        /// <summary>
        /// Gets the underlying ViewModel for that window.
        /// </summary>
        public MainViewModel ViewModel
        {
            get { return this.DataContext as MainViewModel; }

            private set { this.DataContext = value; }
        }

        #endregion Properties (1)
    }
}