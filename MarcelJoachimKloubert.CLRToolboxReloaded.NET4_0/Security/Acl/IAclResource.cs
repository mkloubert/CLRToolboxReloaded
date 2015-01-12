// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox.Security.Acl
{
    /// <summary>
    /// Describes a resource for an access control list.
    /// </summary>
    public interface IAclResource : IObject, IEquatable<IAclResource>
    {
        #region Properties (2)

        /// <summary>
        /// Gets if the current resource is allowed or not.
        /// </summary>
        bool IsAllowed { get; }

        /// <summary>
        /// Gets the name of the resource.
        /// </summary>
        string Name { get; }

        #endregion Properties (2)
    }
}