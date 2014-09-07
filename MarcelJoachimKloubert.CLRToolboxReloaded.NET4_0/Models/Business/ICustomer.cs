// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Models.Business
{
    /// <summary>
    /// Describes a customer inside a <see cref="IClient" />.
    /// </summary>
    public interface ICustomer : IIdentifiable
    {
        #region Properties (1)

        /// <summary>
        /// Gets the underlying client.
        /// </summary>
        IClient Client { get; }

        #endregion Properties (1)

        #region Methods (1)

        /// <summary>
        /// Returns a list of persons of that customer.
        /// </summary>
        /// <returns>The list of persons.</returns>
        IEnumerable<IPerson> GetPersons();

        #endregion Methods (1)
    }
}