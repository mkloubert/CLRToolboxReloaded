// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.IO.Compression
{
    partial class DelegateCompressor
    {
        /// <summary>
        /// List of compression modes.
        /// </summary>
        public enum CompressionMode
        {
            /// <summary>
            /// Compress data.
            /// </summary>
            Compress,

            /// <summary>
            /// De-/Uncompress data.
            /// </summary>
            Uncompress,
        }
    }
}
