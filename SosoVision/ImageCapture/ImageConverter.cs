using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System;

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
            var wb = CreateCompatibleWriteableBitemap(bitmap);
            System.Drawing.Imaging.PixelFormat format = bitmap.PixelFormat;

            if (wb == null)
            {
                wb = new WriteableBitmap(bitmap.Width, bitmap.Height, 0, 0, PixelFormats.Bgra32, null);
                format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }

            BitmapCopyToWriteableBitmap(bitmap, wb, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, format);
            return wb;
        }

        public static WriteableBitmap CreateCompatibleWriteableBitemap(Bitmap src)
        {
            System.Windows.Media.PixelFormat format;

            switch (src.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555:
                    format = System.Windows.Media.PixelFormats.Bgr555;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb565:
                    format = System.Windows.Media.PixelFormats.Bgr565;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    format = System.Windows.Media.PixelFormats.Bgr24;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    format = System.Windows.Media.PixelFormats.Bgr32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    format = System.Windows.Media.PixelFormats.Pbgra32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    format = System.Windows.Media.PixelFormats.Bgra32;
                    break;
                default:
                    return null;
            }

            return new WriteableBitmap(src.Width, src.Height, 0, 0, format, null);
        }

        public static void BitmapCopyToWriteableBitmap(Bitmap src, WriteableBitmap dst, Rectangle srcRect, int destinationX, int destinationY, System.Drawing.Imaging.PixelFormat srcPixeFormat)
        {
            var data = src.LockBits(new Rectangle(new Point(0, 0), src.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, srcPixeFormat);
            dst.WritePixels(new System.Windows.Int32Rect(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height), data.Scan0, data.Height * data.Stride, data.Stride, destinationX, destinationY);
            src.UnlockBits(data);

        }

        /// <summary>
        /// WriteableBitmap转Bitmap
        /// </summary>
        /// <param name="writeableBitmap"></param>
        /// <returns></returns>
        public static Bitmap WriteableBitmapToBitmap(this WriteableBitmap writeableBitmap)
        {
            //System.Drawing.Imaging.PixelFormat format;
            
            //switch (writeableBitmap)
            //{
            //    case PixelFormats.Bgr555:
            //        format =  System.Drawing.Imaging.PixelFormat.Format16bppRgb555;
            //        break;
            //    case PixelFormats.Bgr565:
            //        format = System.Drawing.Imaging.PixelFormat.Format16bppRgb565;
            //        break;
            //    case System.Windows.Media.PixelFormats.Bgr24:
            //        format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            //        break;
            //    case System.Windows.Media.PixelFormats.Bgr32:
            //        format = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            //        break;
            //    case System.Windows.Media.PixelFormats.Pbgra32:
            //        format = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
            //        break;
            //    case System.Windows.Media.PixelFormats.Bgra32:
            //        format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            //        break;
            //    default:
            //        return null;
            //}

            Bitmap tempBitmap = new Bitmap(writeableBitmap.PixelWidth,writeableBitmap.PixelHeight);
            int rPixelBytes = writeableBitmap.BackBufferStride * writeableBitmap.PixelHeight;
            System.Drawing.Imaging.BitmapData data = tempBitmap.LockBits(new Rectangle(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            writeableBitmap.Lock();
            unsafe
            {
                Buffer.MemoryCopy(writeableBitmap.BackBuffer.ToPointer(),data.Scan0.ToPointer(),rPixelBytes,rPixelBytes);
            }

            writeableBitmap.AddDirtyRect(new System.Windows.Int32Rect(0,0,(int)writeableBitmap.Width,(int)writeableBitmap.Height));
            writeableBitmap.Unlock();
            tempBitmap.UnlockBits(data);
            return tempBitmap;
        }
    }
}
