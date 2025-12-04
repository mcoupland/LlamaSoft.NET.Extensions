using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading;

namespace LlamaSoft.NET.Extensions
{
    public static class Strings
    {
        #region Bitmap, Image, and Video
        public static Image GetImage(this string image_full_path)
        {
            return new FileInfo(image_full_path).GetImage();
        }
        public static Bitmap GetBitmap(this string bitmap_full_path)
        {
            return new FileInfo(bitmap_full_path).GetBitmap();
        }
        public static Bitmap CaptureBitmap(this string source_video, double capture_time, int sleep_time)
        {
            try
            {
                MediaPlayer paused_video = source_video.GetPausedVideo(capture_time, sleep_time);
                BitmapFrame image_data_at_position = paused_video.GetVideoImageData();
                BitmapEncoder image_data_encoder = image_data_at_position.GetImageDataEncoder();
                Bitmap captured_bitmap;

                using (MemoryStream stream = new MemoryStream())
                {
                    image_data_encoder.Frames.Add(image_data_at_position);
                    image_data_encoder.Save(stream);
                    byte[] bit = stream.ToArray();
                    captured_bitmap = Bitmap.FromStream(stream) as Bitmap;
                    stream.Close();
                }
                return captured_bitmap;
            }
            catch (Exception e)
            {
                throw new MultimediaVideoGenericException(
                    "Capture_Bitmap.Capture(FileInfo source_video, double capture_time, int sleep_time)",
                    e
                );
            }
        }
        public static Bitmap ScaleBitmap(this string picturesource, ushort scale_value, ScaleMethod scale_method)
        {
            using (Bitmap picture_from_file = Bitmap.FromFile(picturesource) as Bitmap)
            {
                return picture_from_file.Scale(scale_value, scale_method);
            }
            throw new PictureManipulationException(
                string.Format(
                    @"Unknown error occured in {0}, picturesource:{1}, maxdimension:{2}",
                    "ScalePicture",
                    picturesource,
                    scale_value
                )
            );
        }
        public static Bitmap Crop(this string picturefile, ushort maxdimension)
        {
            return picturefile.GetBitmap().Crop(maxdimension);
        }
        public static ImageSource GetBitmapSource(this string source)
        {
            return new Uri(source).GetBitmapSource();
        }

        /// <summary>
        /// Creates a MediaPlayer for the video and jumps to the specified time
        /// </summary>
        /// <param name="video_path">The path to the video that you want to image capture</param>
        /// <param name="time_in_seconds">The video will be paused at this time (in seconds)</param>
        /// <param name="sleep_time">The MediaPlayer needs time to do it's thing, this lets you tweak the wait time in milliseconds</param>
        /// <returns>Returns the MediaPlayer paused at the specified time</returns>
        public static MediaPlayer GetPausedVideo(this string video_path, double time_in_seconds, int sleep_time)
        {
            MediaPlayer video_player = new MediaPlayer { Volume = 0, ScrubbingEnabled = true };
            video_player.Open(new Uri(video_path));
            video_player.Play();
            Thread.Sleep(sleep_time);  // The player is a little finicky, you have to open, pause and sleep for it to work. I've not had good luck using the MediaPlayer events.
            video_player.Pause();
            double video_duration = video_player.NaturalDuration.TimeSpan.TotalSeconds;  // need this value because it is not available in the exception
            if (time_in_seconds < 0 || time_in_seconds > video_duration)
            {
                video_player.Close();
                throw new InvalidTimeException(time_in_seconds, video_path, video_duration);
            }
            video_player.Position = TimeSpan.FromSeconds(time_in_seconds);
            Thread.Sleep(sleep_time);
            return video_player;
        }

        /// <summary>
        /// Encodes the image data, saves the image and closes the video player.
        /// </summary>
        /// <param name="image_path"></param>
        /// <param name="image_data_encoder"></param>
        /// <param name="video_player"></param>
        public static FileInfo SaveImage(this string image_path, BitmapEncoder image_data_encoder, MediaPlayer video_player)
        {
            using (FileStream image_filestream = new FileStream(image_path, FileMode.Create))
            {
                image_data_encoder.Save(image_filestream);
                video_player.Close();
                image_filestream.Close();
                image_filestream.Dispose();
            }
            return new FileInfo(image_path);
        }
        #endregion

        #region Files
        public static FileInfo MoveFile(this string sourcepath, string targetdirectory, bool overwrite = false)
        {
            return new FileInfo(sourcepath).MoveFile(new DirectoryInfo(targetdirectory), overwrite);
        }
        public static string GetFileNameWithoutExtension(this string filename)
        {
            return new FileInfo(filename).GetFileNameWithoutExtension();
        }
        public static List<FileInfo> GetFileInfos(this string directory, string pattern, SearchOption searchoption, ushort limit = ushort.MinValue)
        {
            return new DirectoryInfo(directory).GetFileInfos(pattern, searchoption, limit);
        }
        public static List<string> ReadAllFileLines(this string filename)
        {
            List<string> lines = new List<string>();
            lines.AddRange(File.ReadAllLines(filename));
            return lines;
        }
        #endregion

        #region Overloads
        private static Bitmap ScalePortrait(string picturesource, ushort maxdimension)
        {
            using (Bitmap picture_from_file = Bitmap.FromFile(picturesource) as Bitmap)
            {
                float width = maxdimension;
                float owidth = (float)picture_from_file.Width;
                float multiplier = width / owidth;
                float height = (float)picture_from_file.Height * multiplier;
                ushort scale_width = width.GetIntFloor();
                ushort scale_height = width.GetIntFloor();
                return new Bitmap(picture_from_file, new System.Drawing.Size(scale_width, scale_height));
            }
        }
        private static Bitmap ScaleLandscape(string picturesource, ushort maxdimension)
        {
            using (Bitmap picture_from_file = Bitmap.FromFile(picturesource) as Bitmap)
            {
                float height = maxdimension;
                float oheight = (float)picture_from_file.Height;
                float multiplier = height / oheight;
                float width = (float)picture_from_file.Width * multiplier;
                ushort scale_width = width.GetIntFloor();
                ushort scale_height = height.GetIntFloor();
                return new Bitmap(picture_from_file, new System.Drawing.Size(scale_width, scale_height));
            }
        }
        #endregion
    }
}
