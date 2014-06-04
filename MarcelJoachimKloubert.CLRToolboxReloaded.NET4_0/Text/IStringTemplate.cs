// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

namespace MarcelJoachimKloubert.CLRToolbox.Text
{
    /// <summary>
    /// Describes a template that generates string output.
    /// </summary>
    public interface IStringTemplate : ITemplate
    {
        #region Methods (1)

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="ITemplate.Render()" />
        new string Render();

        #endregion Methods (1)
    }
}