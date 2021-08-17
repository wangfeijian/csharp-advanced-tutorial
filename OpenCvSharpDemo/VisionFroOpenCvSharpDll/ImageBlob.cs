using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;
using OpenCvSharp.WpfExtensions;

namespace VisionFroOpenCvSharpDll
{
    public class ImageBlob
    {
        private SimpleBlobDetector _blobDetector;

        public WriteableBitmap GetBlobedImage(SimpleBlobDetector.Params pParams, WriteableBitmap inputBitmap)
        {
            try
            {
                using (Mat inputMat = new Mat(inputBitmap.PixelHeight, inputBitmap.PixelWidth, MatType.CV_8U))
                {
                    using (Mat outputMat = new Mat())
                    {
                        inputBitmap.ToMat(inputMat);
                        using (_blobDetector = SimpleBlobDetector.Create(pParams))
                        {
                            KeyPoint[] data = _blobDetector.Detect(inputMat);
                            Cv2.DrawKeypoints(inputMat, data, outputMat, Scalar.Red, DrawMatchesFlags.DrawRichKeypoints);
                            return outputMat.ToWriteableBitmap();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
