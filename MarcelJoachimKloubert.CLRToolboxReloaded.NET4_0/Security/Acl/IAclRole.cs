// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Acl
{
    /// <summary>
    /// Describes the role of an access control list.
    /// </summary>
    public interface IAclRole : IObject, IEnumerable<IAclResource>, IEquatable<IAclRole>
    {
        #region Properties (2)

        /// <summary>
        /// Gets the name of the role.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Access a resource by its name.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <returns>The resource or <see langword="null" /> if not found.</returns>
        IAclResource this[string name] { get; }

        #endregion Properties (2)

        #region Methods (4)

        /// <summary>
        /// Checks if all resources are allowed.
        /// </summary>
        /// <param name="resources">The name of the resources.</param>
        /// <returns>All resources are allowed or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resources" /> is <see langword="null" />.
        /// </exception>
        bool AreAllowed(IEnumerable<string> resources);

        /// <summary>
        /// Checks if all resources are allowed.
        /// </summary>
        /// <param name="resources">The name of the resources.</param>
        /// <returns>All resources are allowed or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resources" /> is <see langword="null" />.
        /// </exception>
        bool AreAllowed(params string[] resources);

        /// <summary>
        /// Tries to find a resource inside that role.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <param name="res">The variable where to write the found resource to.</param>
        /// <param name="defRes">If not found, this is the instance to write to <paramref name="res" /> instead.</param>
        /// <returns>Resource was found or not.</returns>
        bool TryGetResource(string name, out IAclResource res, IAclResource defRes = null);

        /// <summary>
        /// Tries to find a resource inside that role.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        /// <param name="res">The variable where to write the found resource to.</param>
        /// <param name="defResProvider">If not found, this is the function that provides instance to write to <paramref name="res" /> instead.</param>
        /// <returns>Resource was found or not.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defResProvider" /> is <see langword="null" />.
        /// </exception>
        bool TryGetResource(string name, out IAclResource res, Func<IAclRole, string, IAclResource> defResProvider);

        #endregion Methods (4)
    }
}