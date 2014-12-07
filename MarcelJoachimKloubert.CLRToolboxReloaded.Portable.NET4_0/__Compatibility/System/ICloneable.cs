// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace System
{
    /// <summary>
    ///
    /// </summary>
    /// <see href="http://msdn.microsoft.com/en-us/library/system.icloneable%28v=vs.110%29.aspx" />
    public interface ICloneable
    {
        #region Methods (1)

        /// <summary>
        ///
        /// </summary>
        /// <see href="http://msdn.microsoft.com/en-us/library/system.icloneable.clone%28v=vs.110%29.aspx" />
        object Clone();

        #endregion Methods (1)
    }
}