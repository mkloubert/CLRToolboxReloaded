// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Data;

namespace MarcelJoachimKloubert.CLRToolbox.Data.Database
{
    /// <summary>
    /// Stores the connection data for a ADO.NET connection.
    /// </summary>
    /// <typeparam name="P">Type of the provider.</typeparam>
    public class AdoDataConnection<P> : DataConnectionBase<P>, IAdoDataConnection
        where P : global::System.Data.IDbConnection, new()
    {
        #region Constructors (4)

        /// <inheriteddoc />
        protected AdoDataConnection(IEnumerable<char> connStr, bool isSynchronized, object sync)
            : base(connStr,
                   isSynchronized: isSynchronized,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected AdoDataConnection(IEnumerable<char> connStr, bool isSynchronized)
            : base(connStr: connStr,
                   isSynchronized: isSynchronized)
        {
        }

        /// <inheriteddoc />
        protected AdoDataConnection(IEnumerable<char> connStr, object sync)
            : base(connStr: connStr,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        protected AdoDataConnection(IEnumerable<char> connStr)
            : base(connStr: connStr)
        {
        }

        #endregion Constructors (4)

        #region Methods (4)

        /// <inheriteddoc />
        public P GetConnection()
        {
            var result = Activator.CreateInstance<P>();
            result.ConnectionString = this.ConnectionString;

            return result;
        }

        IDbConnection IAdoDataConnection.GetConnection()
        {
            return this.GetConnection();
        }

        /// <inheriteddoc />
        public P OpenConnection()
        {
            var result = this.GetConnection();
            result.Open();

            return result;
        }

        IDbConnection IAdoDataConnection.OpenConnection()
        {
            return this.OpenConnection();
        }

        #endregion Methods
    }
}