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

            var result = new Bitmap(input.Width, input.Height,
                                    PixelFormat.Format32bppArgb);

            var bmpData1 = input.LockBits(new Rectangle(0, 0,
                                                        input.Width, input.Height),
                                          ImageLockMode.ReadOnly,
                                          PixelFormat.Format32bppArgb);

            var bmpData2 = result.LockBits(new Rectangle(0, 0,
                                                         result.Width, result.Height),
                                           ImageLockMode.ReadOnly,
                                           PixelFormat.Format32bppArgb);

            unsafe
            {
                var imgPointer1 = (byte*)bmpData1.Scan0;
                var imgPointer2 = (byte*)bmpData2.Scan0;

                for (var y = 0; y < bmpData1.Height; y++)
                {
                    for (var x = 0; x < bmpData1.Width; x++)
                    {
                        var a = (imgPointer1[0] + imgPointer1[1] +
                                imgPointer1[2]) / 3;

                        imgPointer2[0] = (byte)a;
                        imgPointer2[1] = (byte)a;
                        imgPointer2[2] = (byte)a;
                        imgPointer2[3] = imgPointer1[3];

                        imgPointer1 += 4;
                        imgPointer2 += 4;
                    }

                    imgPointer1 += bmpData1.Stride -
                                   (bmpData1.Width * 4);

                    imgPointer2 += bmpData1.Stride -
                                   (bmpData1.Width * 4);
                }
            }

            result.UnlockBits(bmpData2);
            input.UnlockBits(bmpData1);

            return result;
        }

        #endregion Methods (1)
    }
}