// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Security.Acl
{
    /// <summary>
    /// List of behaviors for an <see cref="IAccessControlList" />.
    /// </summary>
    public enum AclAllowBehavior
    {
        /// <summary>
        /// Check roles.
        /// </summary>
        CheckRoles = 0,

        /// <summary>
        /// Allow nothing (blocked).
        /// </summary>
        Nothing,

        /// <summary>
        /// Allow anything (super admin).
        /// </summary>
        Anything,
    }
}