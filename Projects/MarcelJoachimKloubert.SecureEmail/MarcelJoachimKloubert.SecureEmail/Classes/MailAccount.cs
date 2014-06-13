// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using MarcelJoachimKloubert.CLRToolbox.Windows.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarcelJoachimKloubert.SecureEmail.Classes
{
    public class MailAccount : NotifiableBase
    {
        #region Constructors (1)

        /// <summary>
        /// Gets the list of accounts.
        /// </summary>
        public DispatcherObservableCollection<MailAccount> Accounts
        {
            get;
            private set;
        }

        #endregion

        #region Properties (1)

        public string Name
        {
            get { return this.Get<string>(); }

            set { this.Set(value); }
        }

        #endregion
    }
}