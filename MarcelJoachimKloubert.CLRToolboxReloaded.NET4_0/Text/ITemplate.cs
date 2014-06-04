// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Text
{
    /// <summary>
    /// Describes a template.
    /// </summary>
    public interface ITemplate : IObject
    {
        #region Methods (1)

        /// <summary>
        /// Renders output.
        /// </summary>
        /// <returns>The rendered output.</returns>
        object Render();

        #endregion Methods (1)
    }
}