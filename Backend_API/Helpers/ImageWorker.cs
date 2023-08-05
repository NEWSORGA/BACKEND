using System.Drawing.Imaging;
using System.Drawing;
using IronSoftware.Drawing;
using System;
namespace ASP_API.Helpers
{
    public class ImageWorker
    {
        public static NetVips.Image CompressImage(AnyBitmap originalPic, int maxWidth, int maxHeight, bool watermark = true, bool transperent = false)
        {
            

            try
            {
                var image = NetVips.Image.ThumbnailBuffer(originalPic.GetBytes(), maxWidth, height: maxHeight);
                return image;

            }
            catch
            {
                return null;
            }
        }
    }
}
