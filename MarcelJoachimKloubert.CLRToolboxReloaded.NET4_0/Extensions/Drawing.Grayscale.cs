// LICENSE: LGPL 3 - https://www.gnu.org/licenses/lgpl-3.0.txt

// s. https://github.com/mkloubert/CLRToolboxReloaded

using System.Drawing;
using System.Drawing.Imaging;

namespace MarcelJoachimKloubert.CLRToolbox.Extensions.Drawing
{
    static partial class ClrToolboxDrawExtensionMethods
    {
        #region Methods (1)

        /// <summary>
        /// Converts a <see cref="Bitmap" /> to its grayscale version.
        /// </summary>
        /// <param name="input">The input bitmap.</param>
        /// <returns>The converted bitmap or <see langword="null" /> if <paramref name="input" /> is also <see langword="null" />.</returns>
        public static Bitmap Grayscale(this Bitmap input)
        {
            if (input == null)
            {
                return null;
            }

            var result = new Bitmap(input.Width, input.Height);

            try
            {
                using (var g = Graphics.FromImage(result))
                {
                    var colorMatrix = new ColorMatrix(new float[][]
                    {
                       new float[] {.3f, .3f, .3f, 0, 0},
                       new float[] {.59f, .59f, .59f, 0, 0},
                       new float[] {.11f, .11f, .11f, 0, 0},
                       new float[] {0, 0, 0, 1, 0},
                       new float[] {0, 0, 0, 0, 1},
                    });

                    using (var attributes = new ImageAttributes())
                    {
                        attributes.SetColorMatrix(colorMatrix);

                        g.DrawImage(input, new Rectangle(0, 0, input.Width, input.Height),
                                    0, 0, input.Width, input.Height, GraphicsUnit.Pixel, attributes);
                    }
                }
            }
            catch
            {
                result.Dispose();

                throw;
            }

            return result;
        }

        #endregion Methods (1)
    }
}