// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Windows.Collections.ObjectModel;
using MarcelJoachimKloubert.FileBox.IO;
using System;
using System.IO;
using System.Windows.Threading;

namespace MarcelJoachimKloubert.FileBox.Client.ViewModels
{
    public sealed class ServerViewModel : NotifiableBase
    {
        #region Constructors (1)

        internal ServerViewModel(MainViewModel parent, ServerInfo info)
        {
            this.Initialize();

            this.Parent = parent;
            this.Info = info;
        }

        #endregion Constructors (1)

        #region Properties (5)

        /// <summary>
        /// Gets the underlying connection.
        /// </summary>
        public FileBoxConnection Connection
        {
            get { return this.Info.Server; }
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
        /// Gets the underlying connection.
        /// </summary>
        public ServerInfo Info
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

        #region Methods (3)

        private void Initialize()
        {
            this.Inbox = DispatcherObservableCollection.Create<FileItem>(prio: DispatcherPriority.Background,
                                                                         isBackground: true);

            this.Outbox = DispatcherObservableCollection.Create<FileItem>(prio: DispatcherPriority.Background,
                                                                          isBackground: true);
        }

        public void ReloadInbox()
        {
            try
            {


                var files = this.Connection.GetInbox();

                this.Inbox.Clear();
                this.Inbox.AddRange(files);
            }
            catch (Exception ex)
            {
                this.OnErrorsReceived(ex);
            }
        }

        #endregion Methods (1)
    }
}