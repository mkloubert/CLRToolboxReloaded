// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

#if !(PORTABLE)
#define KNOWS_PROPERTY_CHANGING
#endif

using System.ComponentModel;

namespace MarcelJoachimKloubert.CLRToolbox.ComponentModel
{
    /// <summary>
    /// Describes an object that reports property changes, e.g.
    /// </summary>
    public interface INotifiable : IObject, INotifyPropertyChanged
#if KNOWS_PROPERTY_CHANGING
        , global::System.ComponentModel.INotifyPropertyChanging
#endif
    {
    }
}