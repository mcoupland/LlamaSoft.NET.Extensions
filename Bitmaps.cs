using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace LlamaSoft.NET.Extensions
{
    public static class Bitmaps
    {
        public static FileInfo Save(this Bitmap source_bitmap, FileInfo saved_bitmap_file_info)
        {
            try
            {
                source_bitmap.Save(saved_bitmap_file_info.FullName);
                return saved_bitmap_file_info;
            }
            catch (Exception e)
            {
                throw new MultimediaVideoGenericException(
                    "Save_Bitmap.Save(Bitmap source_bitmap, FileInfo saved_bitmap_file_info)",
                    e
                );
            }
        }
        public static Bitmap Desaturate(this Bitmap source_bitmap)
        {
            try
            {
                Rectangle source_rectangle = new Rectangle(0, 0, source_bitmap.Width, source_bitmap.Height);
                //create a blank bitmap the same size as original
                Bitmap desaturated_bitmap = new Bitmap(source_bitmap.Width, source_bitmap.Height);
                Graphics g = Graphics.FromImage(desaturated_bitmap);

                ImageAttributes desaturated_bitmap_attributes = new ImageAttributes();
                desaturated_bitmap_attributes.SetColorMatrix(desaturated_bitmap.GetGrayscaleMatrix());

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(
                    source_bitmap,
                    source_rectangle,
                    0,
                    0,
                    source_bitmap.Width,
                    source_bitmap.Height,
                    GraphicsUnit.Pixel,
                    desaturated_bitmap_attributes
                );

                //dispose the Graphics object
                g.Dispose();
                return desaturated_bitmap;
            }
            catch (Exception e)
            {
                throw new MultimediaVideoGenericException(
                    "Save_Bitmap.Save(Bitmap source_bitmap, FileInfo saved_bitmap_file_info)",
                    e
                );
            }
        }
        public static Bitmap Crop(this Bitmap source_bitmap, int start_x, int start_y, int width, int height)
        {
            Bitmap cloned_bitmap = new Bitmap(source_bitmap);
            Rectangle crop_box = new Rectangle(start_x, start_y, width, height);
            return cloned_bitmap.Clone(crop_box, cloned_bitmap.PixelFormat);
        }
        public static Bitmap GetThumbnail(this Bitmap source_bitmap, int thumbnail_dimension, DimensionType apply_to)
        {
            float bitmap_width = (float)source_bitmap.Width;
            float bitmap_height = (float)source_bitmap.Height;

            float thumbnail_width = 0f;
            float thumbnail_height = 0f;

            if (apply_to == DimensionType.Height)
            {
                thumbnail_height = thumbnail_dimension;
                thumbnail_width = bitmap_width * thumbnail_height / bitmap_height;
            }
            else
            {
                thumbnail_width = thumbnail_dimension;
                thumbnail_height = bitmap_height * thumbnail_width / bitmap_width;
            }
            return Resize(source_bitmap, thumbnail_width, thumbnail_height);
        }
        public static BitmapImage ToBitmapImage(this Bitmap bitmap_to_convert)
        {
            try
            {
                using (MemoryStream bitmap_memory = new MemoryStream())
                {
                    bitmap_to_convert.Save(bitmap_memory, ImageFormat.Bmp);
                    bitmap_memory.Position = 0;
                    BitmapImage converted_bitmapimage = new BitmapImage();
                    converted_bitmapimage.BeginInit();
                    converted_bitmapimage.StreamSource = bitmap_memory;
                    converted_bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    converted_bitmapimage.EndInit();

                    return converted_bitmapimage;
                }
            }
            catch (Exception e)
            {
                throw new MultimediaVideoGenericException(
                    "Conversion.BitmapToImageSource(Bitmap bitmap_to_convert)",
                    e
                );
            }
        }
        public static Orientation GetOrientation(this Bitmap source_bitmap)
        {
            return source_bitmap.Height >= source_bitmap.Width ? Orientation.Portrait : Orientation.Landscape;
        }
        public static Bitmap Resize(this Bitmap original_bitmap, float new_width, float new_height)
        {
            int width = Convert.ToInt32(Math.Floor(new_width));
            int height = Convert.ToInt32(Math.Floor(new_height));
            return Resize(original_bitmap, width, height);
        }
        public static Bitmap Resize(this Bitmap original_bitmap, int new_width, int new_height)
        {
            try
            {
                System.Drawing.Size new_size = new System.Drawing.Size(
                    new_width,
                    new_height
                );
                return new Bitmap(
                    original_bitmap,
                    new_size
                );
            }
            catch (Exception e)
            {
                throw new MultimediaVideoGenericException(
                    "BitmapTools.Resize(Bitmap original_bitmap, Size new_size)",
                    e
                );
            }
        }
        public static Bitmap Scale(this Bitmap source_bitmap, int scale_value, ScaleMethod scale_method = ScaleMethod.MaxDimension)
        {
            if (scale_method == ScaleMethod.Percent) { return Scale(source_bitmap, scale_value); }
            try
            {
                System.Drawing.Size new_size = GetScaledSize(source_bitmap, scale_value);
                return Resize(source_bitmap, new_size.Width, new_size.Height);
            }
            catch (Exception e)
            {
                throw new MultimediaVideoGenericException(
                    "Save_Bitmap.Save(Bitmap source_bitmap, FileInfo saved_bitmap_file_info)",
                    e
                );
            }
        }
        public static Bitmap Crop(this Bitmap picture_bitmap, ushort maxdimension)
        {
            using (picture_bitmap)
            {
                if (picture_bitmap.Width > picture_bitmap.Height) { return CropLandscapePicture(picture_bitmap, maxdimension); }
                else { return CropPortraitPicture(picture_bitmap, maxdimension); }
            }
        }

        #region Overloads
        private static Bitmap Scale(Bitmap original_bitmap, int scale_percent)
        {
            try
            {
                float percentage = (float)scale_percent / 100f;
                float f_width = (float)original_bitmap.Width;
                float f_height = (float)original_bitmap.Height;

                int new_width = Convert.ToInt32(Math.Floor(f_width * percentage));
                int new_height = Convert.ToInt32(Math.Floor(f_height * percentage));

                System.Drawing.Size new_size = new System.Drawing.Size(
                    new_width,
                    new_height
                );
                return new Bitmap(
                    original_bitmap,
                    new_size
                );
            }
            catch (Exception e)
            {
                throw new MultimediaVideoGenericException(
                    "BitmapTools.Resize(Bitmap original_bitmap, Size new_size)",
                    e
                );
            }
        }
        private static Bitmap CropLandscapePicture(Bitmap picture, ushort maxdimension)
        {
            using (picture)
            {
                // Int32Rect needs reference to WindowsBase.dll
                Int32Rect converted_crop = new Int32Rect();
                converted_crop.X = (picture.Width - maxdimension) / 2;
                converted_crop.Y = 0;
                Rectangle crop_box = new Rectangle(converted_crop.X, converted_crop.Y, maxdimension, maxdimension);
                return picture.Clone(crop_box, picture.PixelFormat);
            }
        }
        private static Bitmap CropPortraitPicture(Bitmap picture, ushort maxdimension)
        {
            using (picture)
            {
                Int32Rect converted_crop = new Int32Rect();
                converted_crop.X = 0;
                converted_crop.Y = (picture.Height - maxdimension) / 2; ;
                Rectangle crop_box = new Rectangle(converted_crop.X, converted_crop.Y, maxdimension, maxdimension);
                return picture.Clone(crop_box, picture.PixelFormat);
            }
        }
        private static System.Drawing.Size GetScaledSize(Bitmap source_bitmap, float max_dimension_value)
        {
            Orientation bitmap_orientation = GetOrientation(source_bitmap);
            System.Drawing.SizeF bitmap_size = new SizeF(source_bitmap.Size);
            System.Drawing.SizeF scaled_size = new SizeF(source_bitmap.Size);
            if (bitmap_orientation == Orientation.Landscape)
            {
                scaled_size.Width = max_dimension_value;
                scaled_size.Height = GetDerivedDimension(
                    bitmap_size.Width,
                    max_dimension_value,
                    bitmap_size.Height
                );
            }
            else
            {
                scaled_size.Height = max_dimension_value;
                scaled_size.Width = GetDerivedDimension(
                    bitmap_size.Height,
                    max_dimension_value,
                    bitmap_size.Width
                );
            }
            return new System.Drawing.Size(Convert.ToInt32(Math.Floor(scaled_size.Width)), Convert.ToInt32(Math.Floor(scaled_size.Height)));
        }
        #endregion

        #region Private Methods
        private static ScaleType GetResizeType(int original_value, int new_value)
        {
            return original_value >= new_value ? ScaleType.Reduction : ScaleType.Enlargement;
        }
        private static float GetDerivedDimension(float constant_value, float max_dimension_value, float derived_value)
        {
            ScaleType resize_type = GetResizeType(
                    (int)constant_value,
                    (int)max_dimension_value
                );
            float multiplier = 0f;
            if (resize_type == ScaleType.Reduction)
            {
                multiplier = max_dimension_value / constant_value;
            }
            else
            {
                multiplier = constant_value / max_dimension_value;
            }
            return derived_value * multiplier;
        }
        #endregion
    }
}
