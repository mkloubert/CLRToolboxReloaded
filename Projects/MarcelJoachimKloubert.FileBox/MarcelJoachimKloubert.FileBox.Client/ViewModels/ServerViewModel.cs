// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Windows.Collections.ObjectModel;
using System;
using System.Windows.Threading;

namespace MarcelJoachimKloubert.FileBox.Client.ViewModels
{
    /// <summary>
    /// The model for the server view.
    /// </summary>
    public sealed class ServerViewModel : NotifiableBase
    {
        #region Constructors (1)

        internal ServerViewModel(MainViewModel parent, IServerInfo info)
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
        public IConnection Connection
        {
            get { return this.Info.Server; }
        }

        /// <summary>
        /// Gets the list of outbox items.
        /// </summary>
        public DispatcherObservableCollection<IFile> Inbox
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the underlying connection.
        /// </summary>
        public IServerInfo Info
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
        public DispatcherObservableCollection<IFile> Outbox
        {
            get;
            private set;
        }

        #endregion Properties (5)

        #region Methods (3)

        private void Initialize()
        {
            this.Inbox = DispatcherObservableCollection.Create<IFile>(prio: DispatcherPriority.Background,
                                                                      isBackground: true);

            this.Outbox = DispatcherObservableCollection.Create<IFile>(prio: DispatcherPriority.Background,
                                                                       isBackground: true);
        }

        public void ReloadInbox()
        {
            try
            {
                // var files = this.Connection.GetInbox();

                // this.Inbox.Clear();
                // this.Inbox.AddRange(files);
            }
            catch (Exception ex)
            {
                this.OnErrorsReceived(ex);
            }
        }

        #endregion Methods (3)
    }
}