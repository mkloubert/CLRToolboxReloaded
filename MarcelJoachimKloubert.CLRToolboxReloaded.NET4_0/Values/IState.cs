// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using System;

namespace MarcelJoachimKloubert.CLRToolbox.Values
{
    /// <summary>
    /// Describes a state.
    /// </summary>
    /// <typeparam name="T">Type of the value.</typeparam>
    public interface IState<T> : IObject, INotifiable, IEquatable<T>
    {
        #region Properties (1)

        /// <summary>
        /// Gets the current value.
        /// </summary>
        T Value { get; }

        #endregion Properties (1)
    }
}