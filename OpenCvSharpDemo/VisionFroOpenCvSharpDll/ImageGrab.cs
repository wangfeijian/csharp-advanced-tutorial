using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;

namespace VisionFroOpenCvSharpDll
{
    public class ImageGrab
    {
        private VideoCapture _video;
        private int _index;
        public WriteableBitmap GetImageFromFile(string fileName, ImreadModes imageReadMode, ref bool flag)
        {
            WriteableBitmap bitmap;

            if (!File.Exists(fileName))
            {
                flag = false;
                return null;
            }

            try
            {
                using (Mat mat = Cv2.ImRead(fileName, imageReadMode))
                {
                    bitmap = mat.ToWriteableBitmap();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                flag = false;
                return null;
            }

            flag = true;
            return bitmap;
        }

        public bool GetVideoFromFile(string fileName, ref int num)
        {
            _index = 0;
            _video = new VideoCapture();
            if (!File.Exists(fileName))
            {
                _video.Dispose();
                return false;
            }

            try
            {
                _video.Open(fileName);
                if (!_video.IsOpened())
                {
                    _video.Dispose();
                    return false;
                }

                num = _video.FrameCount;

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public WriteableBitmap GetVideoFrame(bool isGray)
        {
            if (_index == _video.FrameCount)
            {
                _video.Dispose();
                return null;
            }

            using (Mat mat = new Mat())
            {
                _video.Read(mat);
                if (isGray)
                {
                    using (Mat newMat = new Mat())
                    {
                        Cv2.CvtColor(mat, newMat, ColorConversionCodes.BGR2GRAY);
                        _index++;
                        return newMat.ToWriteableBitmap();
                    }
                }
                _index++;
                return mat.ToWriteableBitmap();
            }
        }
    }
}
