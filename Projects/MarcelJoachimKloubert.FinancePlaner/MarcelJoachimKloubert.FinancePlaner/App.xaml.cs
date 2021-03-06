﻿// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.FinancePlaner.Windows;
using System;
using System.Windows;

namespace MarcelJoachimKloubert.FinancePlaner
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
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
            get { return (global::MarcelJoachimKloubert.FinancePlaner.App)Application.Current; }
        }

        /// <inheriteddoc />
        public new MainWindow MainWindow
        {
            get { return (global::MarcelJoachimKloubert.FinancePlaner.Windows.MainWindow)base.MainWindow; }
        }

        #endregion Properties (2)

        #region Methods (1)

        [STAThread]
        private static int Main(string[] args)
        {
            var a = new App();
            a.InitializeComponent();

            return a.Run(new MainWindow());
        }

        #endregion Methods (1)
    }
}