using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LlamaSoft.NET.Extensions
{
    public static class FileInfos
    {
        #region Bitmaps
        public static Image GetImage(this FileInfo image_file_info)
        {
            return GetBitmap(image_file_info) as Image;
        }
        public static Bitmap GetBitmap(this FileInfo bitmap_file_info)
        {
            using (FileStream bitmap_stream = new FileStream(bitmap_file_info.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (Bitmap bitmap_from_file = new Bitmap(bitmap_stream))
                {
                    return new Bitmap(bitmap_from_file);
                }
            }
        }
        public static Bitmap CaptureBitmap(this FileInfo source_video, double capture_time, int sleep_time)
        {
            return source_video.FullName.CaptureBitmap(capture_time, sleep_time);
        }
        public static Bitmap ScaleBitmap(this FileInfo bitmap_file, ushort scale_value, ScaleMethod scale_method = ScaleMethod.MaxDimension)
        {
            return bitmap_file.FullName.ScaleBitmap(scale_value, scale_method);
        }
        public static Bitmap Crop(this FileInfo picturefile, ushort maxdimension)
        {
            return picturefile.FullName.Crop(maxdimension);
        }
        #endregion

        #region Files
        public static FileInfo GetUniqueFileInfoName(this FileInfo desired_file_name)
        {
            if (!Directory.Exists(desired_file_name.DirectoryName))
            {
                DirectoryInfo desired_directory = new DirectoryInfo(desired_file_name.DirectoryName);
                Directory.CreateDirectory(desired_file_name.DirectoryName);
            }
            if (!File.Exists(desired_file_name.FullName))
            {
                return desired_file_name;
            }
            string name_without_extension = desired_file_name.FullName.Replace(
                desired_file_name.Extension,
                string.Empty
            );
            string file_extension = desired_file_name.Extension;
            string unique_file_name = desired_file_name.FullName;
            int unique_counter = 0;
            while (File.Exists(unique_file_name))
            {
                unique_file_name = string.Format(
                    "{0}_{1}.{2}",
                    name_without_extension,
                    unique_counter.ToString(),
                    file_extension
                );
                unique_counter++;
            }
            return new FileInfo(unique_file_name);
        }
        public static FileInfo MoveFile(this FileInfo sourcefileinfo, DirectoryInfo targetdirectoryinfo, bool overwrite = false)
        {
            string separator = targetdirectoryinfo.FullName.EndsWith("\\") ? "" : "\\";
            string target_file = string.Format(@"{0}{1}{2}", targetdirectoryinfo.Parent.FullName, separator, sourcefileinfo.Name);
            if (File.Exists(target_file) && !overwrite)
            {
                throw new FileExistsException(string.Format(@"Target file {0} already exists and overwrite flag is set to false", target_file));
            }
            File.Move(sourcefileinfo.FullName, target_file);
            return new FileInfo(target_file);
        }
        public static string GetFileNameWithoutExtension(this FileInfo filename)
        {
            return filename.Name.Replace(filename.Extension, "");
        }
        #endregion
    }
}
