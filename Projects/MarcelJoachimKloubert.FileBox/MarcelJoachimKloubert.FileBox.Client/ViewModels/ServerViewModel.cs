// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Windows.Collections.ObjectModel;
using MarcelJoachimKloubert.FileBox.IO;
using System.Windows.Threading;

namespace MarcelJoachimKloubert.FileBox.Client.ViewModels
{
    public sealed class ServerViewModel : NotifiableBase
    {
        #region Constructors (1)

        internal ServerViewModel(MainViewModel parent, FileBoxConnection conn)
        {
            this.Initialize();

            this.Parent = parent;
            this.Connection = conn;
        }

        #endregion Constructors (1)

        #region Properties (4)

        /// <summary>
        /// Gets the underlying connection.
        /// </summary>
        public FileBoxConnection Connection
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of outbox items.
        /// </summary>
        public DispatcherObservableCollection<FileItem> Inbox
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parent view model.
        /// </summary>
        public MainViewModel Parent
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of outbox items.
        /// </summary>
        public DispatcherObservableCollection<FileItem> Outbox
        {
            get;
            private set;
        }

        #endregion Properties (4)

        #region Methods (1)

        private void Initialize()
        {
            this.Inbox = DispatcherObservableCollection.Create<FileItem>(prio: DispatcherPriority.Background,
                                                                         isBackground: true);

            this.Outbox = DispatcherObservableCollection.Create<FileItem>(prio: DispatcherPriority.Background,
                                                                          isBackground: true);
        }

        #endregion Methods (1)
    }
}