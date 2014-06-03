// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Data.Database
{
    #region CLASS: AdoDatabaseBase<TConn>

    /// <summary>
    /// A basic ADO.NET database connection.
    /// </summary>
    /// <typeparam name="TConn">Type of the underlying connection.</typeparam>
    public abstract class AdoDatabaseBase<TConn> : DatabaseBase, IAdoDatabase<TConn>
        where TConn : global::System.Data.IDbConnection
    {
        #region Fields (1)

        private readonly bool _OWNS_CONNECTION;

        #endregion Fields (1)

        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoDatabaseBase{TConn}" /> class.
        /// </summary>
        /// <param name="conn">The value for the <see cref="AdoDatabaseBase{TConn}.Connection" /> property.</param>
        /// <param name="sync">The value for the <see cref="ObjectBase._SYNC" /> field.</param>
        /// <param name="ownsConnection">
        /// Also dispose object in <see cref="AdoDatabaseBase{TConn}.Connection" /> or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> is <see langword="null" />.
        /// </exception>
        protected AdoDatabaseBase(TConn conn, object sync, bool ownsConnection = false)
            : base(sync)
        {
            if (conn == null)
            {
                throw new ArgumentNullException("conn");
            }

            this.Connection = conn;
            this._OWNS_CONNECTION = ownsConnection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoDatabaseBase{TConn}" /> class.
        /// </summary>
        /// <param name="conn">The value for the <see cref="AdoDatabaseBase{TConn}.Connection" /> property.</param>
        /// <param name="ownsConnection">
        /// Also dispose object in <see cref="AdoDatabaseBase{TConn}.Connection" /> or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> is <see langword="null" />.
        /// </exception>
        protected AdoDatabaseBase(TConn conn, bool ownsConnection = false)
            : this(conn,
                   sync: new object(), ownsConnection: ownsConnection)
        {
        }

        #endregion Constructors (2)

        #region Properties (2)

        /// <inheriteddoc />
        public TConn Connection
        {
            get;
            private set;
        }

        IDbConnection IAdoDatabase.Connection
        {
            get { return this.Connection; }
        }

        #endregion Properties

        #region Methods (1)

        /// <inheriteddoc />
        protected override void OnDispose(DisposableObjectBase.DisposeContext context)
        {
            switch (context)
            {
                case DisposeContext.DisposeMethod:
                    {
                        if (this._OWNS_CONNECTION)
                        {
                            this.Connection
                                .Dispose();
                        }
                    }
                    break;
            }
        }

        #endregion Methods
    }

    #endregion

    #region CLASS: AdoDatabaseBase

    /// <summary>
    /// A basic general ADO.NET database connection.
    /// </summary>
    public abstract class AdoDatabaseBase : AdoDatabaseBase<global::System.Data.IDbConnection>
    {
        #region Constructors (2)

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoDatabaseBase" /> class.
        /// </summary>
        /// <param name="conn">The value for the <see cref="AdoDatabaseBase{TConn}.Connection" /> property.</param>
        /// <param name="sync">The value for the <see cref="ObjectBase._SYNC" /> field.</param>
        /// <param name="ownsConnection">
        /// Also dispose object in <see cref="AdoDatabaseBase{TConn}.Connection" /> or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> is <see langword="null" />.
        /// </exception>
        protected AdoDatabaseBase(IDbConnection conn, object sync, bool ownsConnection = false)
            : base(conn, sync,
                   ownsConnection: ownsConnection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoDatabaseBase" /> class.
        /// </summary>
        /// <param name="conn">The value for the <see cref="AdoDatabaseBase{TConn}.Connection" /> property.</param>
        /// <param name="ownsConnection">
        /// Also dispose object in <see cref="AdoDatabaseBase{TConn}.Connection" /> or not.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="conn" /> is <see langword="null" />.
        /// </exception>
        protected AdoDatabaseBase(IDbConnection conn, bool ownsConnection = false)
            : base(conn,
                   ownsConnection: ownsConnection)
        {
        }

        #endregion Constructors
    }

    #endregion
}