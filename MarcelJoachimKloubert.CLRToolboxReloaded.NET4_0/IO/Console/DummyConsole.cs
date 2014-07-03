// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Console
{
    /// <summary>
    /// A console that does nothing.
    /// </summary>
    public class DummyConsole : ConsoleBase
    {
        #region Methods (3)

        /// <inheriteddoc />
        protected override void OnClear()
        {
        }

        /// <inheriteddoc />
        protected override void OnReadLine(TextWriter line)
        {
        }

        /// <inheriteddoc />
        protected override void OnWrite(string text)
        {
        }

        #endregion Methods (3)
    }
}