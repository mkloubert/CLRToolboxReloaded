// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Configuration;
using MarcelJoachimKloubert.CLRToolbox.Windows.Collections.ObjectModel;
using MarcelJoachimKloubert.SecureEmail.Classes;
using MarcelJoachimKloubert.CLRToolbox.Extensions;

namespace MarcelJoachimKloubert.SecureEmail.ViewModels
{
    /// <summary>
    /// Th
    /// </summary>
    public sealed class MainViewModel : NotifiableBase
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instanceo fthe <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
            : base(isSynchronized: true)
        {
            this.Initialize();
        }

        #endregion
        
        #region Properties (1)

        /// <summary>
        /// Gets the list of accounts.
        /// </summary>
        public DispatcherObservableCollection<MailAccount> Accounts
        {
            get;
            private set;
        }

        public IConfigRepository Config
        {
            get;
            private set;
        }

        #endregion Properties (1)

        #region Methods (1)

        private void Initialize()
        {
            this.Config = new IniFileConfigRepository(@"./config.ini",
                                                      isReadOnly: false);
            
            this.Accounts = new DispatcherObservableCollection<MailAccount>();
            this.Config
                .GetCategoryNames()
                .ForEach(ctx =>
                    {
                        var category = ctx.Item;

                        string accountName;
                        ctx.State.Config.TryGetValue(name: "name",
                                                     category: category,
                                                     value: out accountName);

                        if (string.IsNullOrWhiteSpace(accountName))
                        {
                            accountName = category;
                        }

                        ctx.State
                           .AccountList
                           .Add(new MailAccount()
                           {
                               Name = accountName,
                           });
                    }, new
                    {
                        AccountList = this.Accounts,
                        Config = this.Config,
                    });
        }

        #endregion Methods (1)
    }
}