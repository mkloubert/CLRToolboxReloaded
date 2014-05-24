// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;

namespace MarcelJoachimKloubert.CLRToolbox
{
    /// <summary>
    /// Describes an object that can be compared over a GUID.
    /// </summary>
    public interface IIdentifiable : IObject, IEquatable<Guid>, IEquatable<IIdentifiable>
    {
        #region Data members (1)

        /// <summary>
        /// Gets the unique GUID of that object.
        /// </summary>
        Guid Id { get; }

        #endregion Data members (1)
    }
}