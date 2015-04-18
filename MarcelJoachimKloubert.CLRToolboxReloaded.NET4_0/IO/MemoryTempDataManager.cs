// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.IO;

namespace MarcelJoachimKloubert.CLRToolbox.IO
{
    /// <summary>
    /// A temp manager that handles data in memory.
    /// </summary>
    public class MemoryTempDataManager : TempDataManagerBase
    {
        #region Constructors (2)

        /// <inheriteddoc />
        public MemoryTempDataManager(object sync)
            : base(isSynchronized: false,
                   sync: sync)
        {
        }

        /// <inheriteddoc />
        public MemoryTempDataManager()
            : this(sync: new object())
        {
        }

        #endregion Constructors (2)

        #region Methods (1)

        /// <inheriteddoc />
        protected override Stream OnCreateStream()
        {
            return new MemoryStream();
        }

        #endregion Methods (1)
    }
}