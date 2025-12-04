using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;

namespace LlamaSoft.NET.Extensions
{
    public static class Multimedia
    {        
        public static ImageSource GetBitmapSource(this Uri source)
        {
            var bitmap_image = new BitmapImage();
            bitmap_image.BeginInit();
            bitmap_image.UriSource = source;
            bitmap_image.CacheOption = BitmapCacheOption.OnLoad;
            bitmap_image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap_image.EndInit();
            return bitmap_image;
            throw new FileNotFoundException(string.Format(@"Argument source {0} does not identify an existing file", source.OriginalString));
        }    

        /// <summary>
        ///  This can actually get playing status! (full disclosure, this is a stackoverflow copy/paste)
        /// </summary>        
        public static MediaState GetMediaState(this MediaElement myMedia)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }

        /// <summary>
        /// Gets the image data from the paused video
        /// </summary>
        /// <param name="video_player"></param>
        /// <returns>Returns a BitmapFrame ready to be encoded</returns>
        public static BitmapFrame GetVideoImageData(this MediaPlayer video_player)
        {
            int pixel_width = video_player.NaturalVideoWidth;  // For some reason you have to set these values in a variable.
            int pixel_height = video_player.NaturalVideoHeight;  // Using *.NaturalVideoWidth or *.NaturalVideoHeight does not work
            RenderTargetBitmap rtb = new RenderTargetBitmap
            (
                pixel_width,
                pixel_height,
                96,
                96,
                PixelFormats.Pbgra32
            );
            Rect crop = new Rect(0, 0, pixel_width, pixel_height);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawVideo(video_player, crop);
            }
            rtb.Render(dv);
            return BitmapFrame.Create(rtb).GetCurrentValueAsFrozen() as BitmapFrame;
        }

        /// <summary>
        /// Gets the encoder that will properly encode the image data into an image
        /// </summary>
        /// <param name="image_data"></param>
        /// <returns>Returns an encoder for the image data</returns>
        public static BitmapEncoder GetImageDataEncoder(this BitmapFrame image_data)
        {
            BitmapEncoder encoder_for_image_data = new PngBitmapEncoder();
            encoder_for_image_data.Frames.Add(image_data as BitmapFrame);
            return encoder_for_image_data;
        }

    }
}
