using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LlamaSoft.NET.Extensions
{
    public static class Images
    {
        public static BitmapImage ImageToBitmapImage(this Image image_to_convert)
        {
            try
            {
                return ((Bitmap)image_to_convert).ToBitmapImage();
            }
            catch (Exception e)
            {
                throw new MultimediaVideoGenericException(
                    "Conversion.BitmapToImageSource(Bitmap bitmap_to_convert)",
                    e
                );
            }
        }

        /// <summary>
        /// Creates a matrix to convert an image to grayscale
        /// </summary>
        /// <returns></returns>
        public static ColorMatrix GetGrayscaleMatrix(this Image img)
        {
            return new ColorMatrix(
               new float[][]
               {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
               }
           );
        }
    }
}
