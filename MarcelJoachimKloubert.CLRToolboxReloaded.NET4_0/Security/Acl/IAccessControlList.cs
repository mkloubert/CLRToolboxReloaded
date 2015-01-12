// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Acl
{
    /// <summary>
    /// Describes an access control list.
    /// </summary>
    public interface IAccessControlList : IObject, IEnumerable<IAclRole>
    {
        #region Properties (1)

        /// <summary>
        /// Access a role by its name.
        /// </summary>
        /// <param name="name">The name of the role.</param>
        /// <returns>The role or <see langword="null" /> if not found.</returns>
        IAclRole this[string name] { get; }

        #endregion Properties (1)

        #region Methods (4)

        /// <summary>
        /// Checks if all resources of a role are allowed.
        /// </summary>
        /// <param name="role">The name of the role.</param>
        /// <param name="resources">The name of the resources.</param>
        /// <returns>All resources are allowed or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resources" /> is <see langword="null" />.
        /// </exception>
        bool AreAllowed(string role, IEnumerable<string> resources);

        /// <summary>
        /// Checks if all resources of a role are allowed.
        /// </summary>
        /// <param name="role">The name of the role.</param>
        /// <param name="resources">The name of the resources.</param>
        /// <returns>All resources are allowed or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resources" /> is <see langword="null" />.
        /// </exception>
        bool AreAllowed(string role, params string[] resources);

        /// <summary>
        /// Tries to find a role inside that ACL.
        /// </summary>
        /// <param name="name">The name of the role.</param>
        /// <param name="role">The variable where to write the found role to.</param>
        /// <param name="defRole">If not found, this is the instance to write to <paramref name="role" /> instead.</param>
        /// <returns>Role was found or not.</returns>
        bool TryGetRole(string name, out IAclRole role, IAclRole defRole = null);

        /// <summary>
        /// Tries to find a role inside that ACL.
        /// </summary>
        /// <param name="name">The name of the role.</param>
        /// <param name="role">The variable where to write the found role to.</param>
        /// <param name="defRoleProvider">If not found, this is the function that provides instance to write to <paramref name="role" /> instead.</param>
        /// <returns>Role was found or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defRoleProvider" /> is <see langword="null" />.
        /// </exception>
        bool TryGetRole(string name, out IAclRole role, Func<IAccessControlList, string, IAclRole> defRoleProvider);

        #endregion Methods (4)
    }
}