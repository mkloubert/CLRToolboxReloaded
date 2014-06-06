// LICENSE: GPL 3 - https://www.gnu.org/licenses/gpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;

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

        #region Methods (1)

        private void Initialize()
        {

        }

        #endregion Methods (1)
    }
}