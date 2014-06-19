// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Execution
{
    #region INTERFACE: IMediator

    /// <summary>
    /// Describes a mediator.
    /// </summary>
    public interface IMediator : IObject
    {
    }

    #endregion INTERFACE: IMediator

    #region INTERFACE: IMediatorUIContext

    /// <summary>
    /// Describes a context for mediator action that should be run on the UI (thread).
    /// </summary>
    public interface IMediatorUIContext : IObject
    {
        #region Properties (3)

        /// <summary>
        /// Gets the underlying mediator.
        /// </summary>
        IMediator Mediator { get; }

        /// <summary>
        /// Gets the underlying payload value.
        /// </summary>
        object Payload { get; }

        /// <summary>
        /// Gets the payload type.
        /// </summary>
        Type PayloadType { get; }

        #endregion Properties

        #region Methods (3)

        /// <summary>
        /// Invokes the 'BeginInvoke(AsyncCallback, object)' method
        /// of the underlying action by using that context object as async state object.
        /// </summary>
        /// <param name="cb">The callback to invoke.</param>
        /// <returns>The async result.</returns>
        IAsyncResult BeginInvoke(AsyncCallback cb);

        /// <summary>
        /// Returns the value of <see cref="IMediatorUIContext.Mediator" /> strong typed.
        /// </summary>
        /// <typeparam name="M">The target type.</typeparam>
        /// <returns>The strong typed mediator.</returns>
        M GetMediator<M>() where M : global::MarcelJoachimKloubert.CLRToolbox.Execution.IMediator;

        /// <summary>
        /// Invokes the real action on the current thread.
        /// </summary>
        void Invoke();

        #endregion Methods
    }

    #endregion INTERFACE: IMediatorUIContext

    #region INTERFACE: IMediatorUIContext<TPayload>

    /// <summary>
    ///
    /// </summary>
    /// <see cref="IMediatorUIContext" />
    public interface IMediatorUIContext<TPayload> : IMediatorUIContext
    {
        #region Properties (1)

        /// <summary>
        ///
        /// </summary>
        /// <see cref="IMediatorUIContext.Payload" />
        new TPayload Payload { get; }

        #endregion Properties
    }

    #endregion INTERFACE: IMediatorUIContext<TPayload>

    #region DELEGATE: MediatorAction<TPayload>

    /// <summary>
    /// Describes a mediator action.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload action.</typeparam>
    /// <param name="payload">The submitted payload.</param>
    public delegate void MediatorAction<TPayload>(TPayload payload);

    #endregion DELEGATE: MediatorAction<TPayload>

    #region DELEGATE: MediatorUIAction

    /// <summary>
    /// Describes logic that runs an action on the UI thread.
    /// </summary>
    /// <param name="ctx">The underlying context.</param>
    public delegate void MediatorUIAction(IMediatorUIContext ctx);

    #endregion DELEGATE: MediatorUIAction
}