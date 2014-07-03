// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.ComponentModel;

namespace MarcelJoachimKloubert.FileBox
{
    /// <summary>
    /// Describes a progress.
    /// </summary>
    public interface IProgress : IObject, INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Properties (3)

        /// <summary>
        /// Gets a value that represents the category of the current task.
        /// </summary>
        int? Category { get; }

        /// <summary>
        /// Gets the description text.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the percentage value if available.
        /// </summary>
        double? Percentage { get; }

        #endregion Properties (3)
    }
}