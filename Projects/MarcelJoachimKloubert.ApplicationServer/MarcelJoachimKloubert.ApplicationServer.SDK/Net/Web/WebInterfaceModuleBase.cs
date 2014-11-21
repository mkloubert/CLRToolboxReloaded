// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using MarcelJoachimKloubert.CLRToolbox;
using MarcelJoachimKloubert.CLRToolbox.ComponentModel;
using System;

namespace MarcelJoachimKloubert.ApplicationServer.Net.Web
{
    /// <summary>
    /// A basic web interface module.
    /// </summary>
    public abstract class WebInterfaceModuleBase : NotifiableBase, IWebInterfaceModule
    {
        #region Constructors (1)

        /// <summary>
        /// Initializes a new instance of the <see cref="WebInterfaceModuleBase" /> class.
        /// </summary>
        /// <param name="id">
        /// The value for the <see cref="WebInterfaceModuleBase.Id" /> property.
        /// </param>
        protected WebInterfaceModuleBase(Guid id)
        {
            this.Id = id;
        }

        #endregion Constructors (1)

        #region Methods (8)

        /// <inheriteddoc />
        public bool Equals(Guid other)
        {
            return this.Id == other;
        }

        /// <inheriteddoc />
        public bool Equals(IIdentifiable other)
        {
            return other != null ? this.Equals(other.Id) : false;
        }

        /// <inheriteddoc />
        public override bool Equals(object other)
        {
            if (other is Guid)
            {
                return this.Equals((Guid)other);
            }

            if (other is IIdentifiable)
            {
                return this.Equals((IIdentifiable)other);
            }

            return base.Equals(other);
        }

        /// <inheriteddoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheriteddoc />
        public void Handle(IWebExecutionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // BeforeHandle
            var invokeOnHandle = true;
            this.OnBeforeHandle(context, ref invokeOnHandle);

            // Handle
            var invokeAfterHandle = true;
            var onHandleInvoked = false;
            if (invokeOnHandle)
            {
                this.OnHandle(context, ref invokeAfterHandle);
                onHandleInvoked = true;
            }

            // AfterHandle
            if (invokeAfterHandle)
            {
                this.OnAfterHandle(context, onHandleInvoked);
            }
        }

        /// <summary>
        /// Is invoked AFTER <see cref="WebInterfaceModuleBase.OnHandle(IWebExecutionContext, ref bool)" />
        /// and <see cref="WebInterfaceModuleBase.OnBeforeHandle(IWebExecutionContext, ref bool)" /> are called.
        /// </summary>
        /// <param name="context">The underlying context.</param>
        /// <param name="invokeOnHandle">
        /// <see cref="WebInterfaceModuleBase.OnHandle(IWebExecutionContext, ref bool)" /> method has been invoked or not.
        /// </param>
        protected virtual void OnAfterHandle(IWebExecutionContext context, bool onHandleInvoked)
        {
            // dummy
        }

        /// <summary>
        /// Is invoked BEFORE <see cref="WebInterfaceModuleBase.OnHandle(IWebExecutionContext, ref bool)" />
        /// and <see cref="WebInterfaceModuleBase.OnAfterHandle(IWebExecutionContext, bool)" /> are called.
        /// </summary>
        /// <param name="context">The underlying context.</param>
        /// <param name="invokeOnHandle">
        /// Invoke <see cref="WebInterfaceModuleBase.OnHandle(IWebExecutionContext, ref bool)" /> method or not.
        /// That value is <see langword="true" /> by default.
        /// </param>
        protected virtual void OnBeforeHandle(IWebExecutionContext context, ref bool invokeOnHandle)
        {
            // dummy
        }

        /// <summary>
        /// Stores the logic for the <see cref="WebInterfaceModuleBase.Handle(IWebExecutionContext)" />.
        /// </summary>
        /// <param name="context">The underlying context.</param>
        /// <param name="invokeAfterHandle">
        /// Invoke <see cref="WebInterfaceModuleBase.OnAfterHandle(IWebExecutionContext, bool)" /> method or not.
        /// That value is <see langword="true" /> by default.
        /// </param>
        protected abstract void OnHandle(IWebExecutionContext context, ref bool invokeAfterHandle);

        #endregion Methods (8)

        #region Properties (2)

        /// <inheriteddoc />
        public Guid Id
        {
            get;
            private set;
        }

        /// <inheriteddoc />
        public virtual string Name
        {
            get { return this.GetType().Name; }
        }

        #endregion Properties (2)
    }
}