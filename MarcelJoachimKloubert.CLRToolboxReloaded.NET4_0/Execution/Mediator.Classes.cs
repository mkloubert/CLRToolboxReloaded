// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox.Data.Conversion;
using MarcelJoachimKloubert.CLRToolbox.Threading;
using System;
using System.Reflection;

namespace MarcelJoachimKloubert.CLRToolbox.Execution
{
    partial class Mediator
    {
        #region Nested classes (7)

        private interface IMediatorActionItem : IEquatable<Delegate>
        {
            #region Properties (2)

            bool IsAlive { get; }

            ThreadOption Option { get; }

            #endregion Properties
        }

        private interface IMediatorActionItem<TPayload> : IMediatorActionItem, IEquatable<MediatorAction<TPayload>>
        {
            #region Methods (1)

            bool Invoke(TPayload payload);

            #endregion Methods

            #region Properties (1)

            Func<TPayload, bool> Filter { get; }

            #endregion Properties
        }

        private abstract class MediatorActionItemBase : IMediatorActionItem
        {
            #region Constructors (1)

            internal MediatorActionItemBase(ThreadOption option)
            {
                this.Option = option;
            }

            #endregion Constructors
            
            #region Properties (2)

            public abstract bool IsAlive { get; }

            public ThreadOption Option
            {
                get;
                private set;
            }

            #endregion Properties

            #region Methods (3)

            public abstract bool Equals(Delegate other);

            public override bool Equals(object other)
            {
                if (other is Delegate)
                {
                    return this.Equals((Delegate)other);
                }

                return base.Equals(other);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            #endregion Methods
        }

        private abstract class MediatorActionItemBase<TPayload> : MediatorActionItemBase, IMediatorActionItem<TPayload>
        {
            #region Constructors (1)

            internal MediatorActionItemBase(ThreadOption option, Func<TPayload, bool> filter)
                : base(option: option)
            {
                this.Filter = filter;
            }

            #endregion Constructors

            #region Methods (2)

            public bool Equals(MediatorAction<TPayload> other)
            {
                return this.Equals((Delegate)other);
            }

            public abstract bool Invoke(TPayload payload);

            #endregion Methods

            #region Properties (1)

            public Func<TPayload, bool> Filter
            {
                get;
                private set;
            }

            #endregion Properties
        }

        private sealed class HardReferenceActionItem<TPayload> : MediatorActionItemBase<TPayload>
        {
            #region Fields (1)

            private readonly MediatorAction<TPayload> _ACTION;

            #endregion Fields
            
            #region Constructors (1)

            internal HardReferenceActionItem(MediatorAction<TPayload> action,
                                             ThreadOption option,
                                             Func<TPayload, bool> filter)
                : base(option: option,
                       filter: filter)
            {
                this._ACTION = action;
            }

            #endregion Constructors

            #region Properties (1)

            public override bool IsAlive
            {
                get { return true; }
            }

            #endregion Properties

            #region Methods (3)

            public override bool Equals(Delegate other)
            {
                return this._ACTION.Equals(other);
            }

            public override int GetHashCode()
            {
                return this._ACTION.GetHashCode();
            }

            public override bool Invoke(TPayload payload)
            {
                this._ACTION(payload);
                return true;
            }

            #endregion Methods
        }

        private sealed class MediatorUIContext<TPayload> : ObjectBase, IMediatorUIContext<TPayload>
        {
            #region Properties (5)

            internal Action Action
            {
                get;
                set;
            }

            public IMediator Mediator
            {
                get;
                internal set;
            }

            public TPayload Payload
            {
                get;
                internal set;
            }

            object IMediatorUIContext.Payload
            {
                get { return this.Payload; }
            }

            public Type PayloadType
            {
                get { return typeof(TPayload); }
            }

            #endregion

            #region Methods (3)

            public IAsyncResult BeginInvoke(AsyncCallback cb)
            {
                return this.Action
                           .BeginInvoke(callback: cb,
                                        @object: this);
            }

            public M GetMediator<M>() where M : IMediator
            {
                return GlobalConverter.Current
                                      .ChangeType<M>(value: this.Mediator);
            }

            public void Invoke()
            {
                this.Action();
            }

            #endregion
        }

        private sealed class WeakReferenceActionItem<TPayload> : MediatorActionItemBase<TPayload>
        {
            #region Fields (2)

            private readonly MethodInfo _METHOD;
            private readonly WeakReference _WEAK_REFERENCE;

            #endregion Fields

            #region Constructors (1)

            internal WeakReferenceActionItem(MediatorAction<TPayload> action,
                                             ThreadOption option,
                                             Func<TPayload, bool> filter)
                : base(option: option,
                       filter: filter)
            {
                this._METHOD = action.Method;
                this._WEAK_REFERENCE = new WeakReference(action.Target);
            }

            #endregion Constructors

            #region Properties (1)

            public override bool IsAlive
            {
                get { return this.TryGetActionReference() != null; }
            }

            #endregion Properties

            #region Methods (4)

            public override bool Equals(Delegate other)
            {
                var action = this.TryGetActionReference();
                if (action != null)
                {
                    return action.Equals(other);
                }

                if (other == null)
                {
                    return true;
                }

                return false;
            }

            public override int GetHashCode()
            {
                var a = this.TryGetActionReference();

                return a != null ? a.GetHashCode() : 0;
            }

            public override bool Invoke(TPayload payload)
            {
                var action = this.TryGetActionReference();
                if (action != null)
                {
                    action(payload);
                    return true;
                }

                return false;
            }

            private MediatorAction<TPayload> TryGetActionReference()
            {
                if (this._METHOD.IsStatic)
                {
                    return (MediatorAction<TPayload>)Delegate.CreateDelegate(type: typeof(MediatorAction<TPayload>),
                                                                             firstArgument: null,
                                                                             method: this._METHOD);
                }

                var target = this._WEAK_REFERENCE.Target;
                if (target != null)
                {
                    return (MediatorAction<TPayload>)Delegate.CreateDelegate(type: typeof(MediatorAction<TPayload>),
                                                                             firstArgument: target,
                                                                             method: this._METHOD);
                }

                return null;
            }

            #endregion Methods
        }

        #endregion Nested classes
    }
}