// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox
{
    #region INTERFACE: IInitializable

    /// <summary>
    /// Describes an object that can has to be initialized.
    /// </summary>
    public interface IInitializable : IObject
    {
        #region Data Members (2)

        /// <summary>
        /// Gets if the object has been initialized or not.
        /// </summary>
        bool IsInitialized { get; }

        #endregion Data Members (2)

        #region Delegates and events (1)

        /// <summary>
        /// Is invoked after that object has been initialized.
        /// </summary>
        event EventHandler Initialized;

        #endregion Delegates and events

        #region Operations (1)

        /// <summary>
        /// Initializes that object.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Object has already been initialized.
        /// </exception>
        void Initialize();

        #endregion Operations
    }

    #endregion

    #region INTERFACE: IInitializable<TContext>

    /// <summary>
    /// Describes an object that can has to be initialized and needs a context object to do that.
    /// </summary>
    /// <typeparam name="TContext">Type of the object that is used to initialize</typeparam>
    public interface IInitializable<in TContext> : IInitializable
    {
        #region Operations (1)

        /// <summary>
        /// Initializes that object.
        /// </summary>
        /// <param name="ctx">The context to initialize that object.</param>
        /// <exception cref="InvalidOperationException">
        /// Object has already been initialized.
        /// </exception>
        void Initialize(TContext ctx);

        #endregion Operations
    }

    #endregion
}