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
        public bool GetImageFromFile(string fileName, ImreadModes imageReadMode, ref WriteableBitmap bitmap)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                Mat mat = Cv2.ImRead(fileName, imageReadMode);

                bitmap = mat.ToWriteableBitmap();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }
    }
}
