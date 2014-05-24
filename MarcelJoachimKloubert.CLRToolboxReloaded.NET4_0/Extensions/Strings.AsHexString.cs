// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Collections.Generic;
using System.Text;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions
{
    static partial class ClrToolboxExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Converts binary data to a hex string.
        /// </summary>
        /// <param name="binData">The data to convert.</param>
        /// <param name="lowerCase">
        /// Return lower (<see langword="true" />) or upper (<see langword="false" />) case characters.
        /// </param>
        /// <returns>The converted data.</returns>
        public static string AsHexString(this IEnumerable<byte> binData, bool lowerCase = true)
        {
            if (binData == null)
            {
                return null;
            }

            var result = new StringBuilder();

            ForEach(binData,
                    (ctx) => ctx.State
                                .HexString.AppendFormat(ctx.State.FormatString,
                                                        ctx.Item),
                    actionState: new
                        {
                            HexString = result,
                            FormatString = lowerCase ? "{0:x2}" : "{0:X2}",
                        });

            return result.ToString();
        }

        #endregion Methods (1)
    }
}