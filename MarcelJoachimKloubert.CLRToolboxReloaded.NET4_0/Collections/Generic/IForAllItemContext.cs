// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Collections.Generic
{
    #region INTERFACE: IForAllItemContext<T>

    /// <summary>
    /// Describes a context for a 'ForAll' operation.
    /// </summary>
    /// <typeparam name="T">Type of the underlying item.</typeparam>
    public interface IForAllItemContext<T> : IObject
    {
        #region Data Members (2)

        /// <summary>
        /// Gets the zero based index.
        /// </summary>
        long Index { get; }

        /// <summary>
        /// Gets the underlying item object.
        /// </summary>
        T Item { get; }

        #endregion Data Members
    }

    #endregion INTERFACE: IForAllItemContext<T>

    #region INTERFACE: IForAllItemContext<T, TState>
    
    /// <summary>
    /// Describes a context for a 'ForAll' operation.
    /// </summary>
    /// <typeparam name="T">Type of the underlying item.</typeparam>
    /// <typeparam name="TState">Type of the state item.</typeparam>
    public interface IForAllItemContext<T, TState> : IForAllItemContext<T>
    {
        #region Data Members (1)

        /// <summary>
        /// Gets the underlying state object.
        /// </summary>
        TState State { get; }

        #endregion Data Members
    }

    #endregion INTERFACE: IForAllItemContext<T, TState>
}