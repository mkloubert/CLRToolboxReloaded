// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// A common manager for handling temprary data.
    /// </summary>
    public class CommonTempDataManager : TempDataManagerBase
    {
        #region Methods (2)

        /// <inheriteddoc />
        protected override void DestroyStream(Stream stream)
        {
            if (stream == null)
            {
                return;
            }
        }

        /// <inheriteddoc />
        protected override Stream OnCreateStream()
        {
            return new TempFileStream();
        }

        #endregion Methods (2)
    }
}