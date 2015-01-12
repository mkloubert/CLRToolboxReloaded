// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Security.Acl
{
    /// <summary>
    /// List of behaviors for an <see cref="IAclRole" />.
    /// </summary>
    public enum AclRoleAllowBehavior
    {
        /// <summary>
        /// Check resources.
        /// </summary>
        CheckResources = 0,

        /// <summary>
        /// Allow nothing.
        /// </summary>
        Nothing,

        /// <summary>
        /// Allow anything.
        /// </summary>
        Anything,
    }
}