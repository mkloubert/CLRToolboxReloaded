﻿// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region INTERFACE: IForEachItemContext<T>

    /// <summary>
    /// Describes a context for a 'ForEach' operation.
    /// </summary>
    /// <typeparam name="T">Type of the underlying item.</typeparam>
    public interface IForEachItemContext<out T> : IForAllItemContext<T>
    {
        #region Data Members (1)

        /// <summary>
        /// Gets or sets if the whole operation should be canceled or not.
        /// </summary>
        bool Cancel { get; set; }

        #endregion Data Members (1)
    }

    #endregion INTERFACE: IForEachItemContext<T>

    #region INTERFACE: IForEachItemContext<T, TState>

    /// <summary>
    /// Describes a context for a 'ForEach' operation.
    /// </summary>
    /// <typeparam name="T">Type of the underlying item.</typeparam>
    /// <typeparam name="TState">Type of the state item.</typeparam>
    public interface IForEachItemContext<out T, out TState> : IForEachItemContext<T>, IForAllItemContext<T, TState>
    {
    }

    #endregion INTERFACE: IForEachItemContext<T, TState>
}