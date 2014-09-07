// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Models.Business
{
    /// <summary>
    /// Describes a client.
    /// </summary>
    public interface IClient : IIdentifiable
    {
        #region Methods (2)

        /// <summary>
        /// Returns a list of customers of that client.
        /// </summary>
        /// <returns>The list of customers.</returns>
        IEnumerable<ICustomer> GetCustomers();

        /// <summary>
        /// Returns a list of persons of that client.
        /// </summary>
        /// <returns>The list of persons.</returns>
        IEnumerable<IPerson> GetPersons();

        #endregion Methods (2)
    }
}