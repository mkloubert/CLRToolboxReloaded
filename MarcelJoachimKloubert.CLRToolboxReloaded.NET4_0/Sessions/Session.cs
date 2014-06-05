// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Sessions
{
    #region CLASS: Session

    /// <summary>
    /// Simple implementation of the <see cref="ISession" /> interface.
    /// </summary>
    public class Session : IdentifiableBase, ISession
    {
        #region Fields (1)

        private Guid _id;

        #endregion CLASS: Session

        #region Properties (2)

        /// <inheriteddoc />
        public override Guid Id
        {
            get { return this._id; }
        }

        /// <inheriteddoc />
        public DateTimeOffset Time
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods (1)

        /// <summary>
        /// Sets the value of <see cref="Session.Id" /> property.
        /// </summary>
        /// <param name="newValue">The (new) value to set.</param>
        public void SetId(Guid newValue)
        {
            this._id = newValue;
        }

        #endregion Methods (1)
    }

    #endregion CLASS: Session

    #region CLASS: Session<TParent>

    /// <summary>
    /// Simple implementation of the <see cref="ISession{TParent}" /> interface.
    /// </summary>
    /// <typeparam name="TParent">Type of the parent object.</typeparam>
    public class Session<TParent> : Session, ISession<TParent>
    {
        #region Properties (1)

        /// <inheriteddoc />
        public TParent Parent
        {
            get;
            set;
        }

        #endregion Properties (1)
    }

    #endregion CLASS: Session<TParent>
}