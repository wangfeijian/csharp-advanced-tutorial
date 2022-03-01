using System.Drawing;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;

namespace ImageCapture
{
    public static class CaptureImageConverter
    {
        /// <summary>
        /// Bitmap转WriteableBitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="tempWriteableBitmap"></param>
        /// <returns></returns>
        public static WriteableBitmap BitmapToWriteableBitmap(this Bitmap bitmap)
        {
            WriteableBitmap tempWriteableBitmap;
            Mat tempMat = bitmap.ToMat();
            using (tempMat)
            {
                tempWriteableBitmap = tempMat.ToWriteableBitmap();
            }

            return tempWriteableBitmap;
        }

        /// <summary>
        /// WriteableBitmap转Bitmap
        /// </summary>
        /// <param name="writeableBitmap"></param>
        /// <returns></returns>
        public static Bitmap WriteableBitmapToBitmap(this WriteableBitmap writeableBitmap)
        {
            Bitmap tempBitmap;
            Mat tempMat = writeableBitmap.ToMat();
            using (tempMat)
            {
                tempBitmap = tempMat.ToBitmap();
            }

            return tempBitmap;
        }
    }
}
