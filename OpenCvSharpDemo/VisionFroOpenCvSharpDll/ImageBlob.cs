using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Internal.Vectors;
using OpenCvSharp.WpfExtensions;

namespace VisionFroOpenCvSharpDll
{
    public class ImageBlob
    {
        private SimpleBlobDetector _blobDetector;

        public WriteableBitmap GetBlobedImageSimple(SimpleBlobDetector.Params pParams, WriteableBitmap inputBitmap)
        {
            try
            {
                using (Mat inputMat = new Mat(inputBitmap.PixelHeight, inputBitmap.PixelWidth, MatType.CV_8U))
                {
                    using (Mat outputMat = new Mat())
                    {
                        if (inputBitmap.Format == PixelFormats.Bgr24)
                        {
                            Cv2.CvtColor(inputBitmap.ToMat(), inputMat, ColorConversionCodes.RGB2GRAY);
                        }
                        else
                        {
                            inputBitmap.ToMat(inputMat);
                        }

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
                return inputBitmap;
            }
        }

        public WriteableBitmap GetThresholdImage(double threshold, WriteableBitmap inputBitmap, ref Mat thresholdMat, bool flag = true)
        {
            try
            {
                using (Mat inputMat = new Mat(inputBitmap.PixelHeight, inputBitmap.PixelWidth, MatType.CV_8U))
                {
                    if (inputBitmap.Format == PixelFormats.Bgr24)
                    {
                        Cv2.CvtColor(inputBitmap.ToMat(), inputMat, ColorConversionCodes.RGB2GRAY);
                    }
                    else
                    {
                        inputBitmap.ToMat(inputMat);
                    }

                    using (Mat outputMat = new Mat())
                    {
                        if (flag)
                        {
                            Cv2.Threshold(inputMat, outputMat, threshold, 255, ThresholdTypes.Binary);
                        }
                        else
                        {
                            Cv2.Threshold(inputMat, outputMat, 0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Triangle);

                        }

                        thresholdMat = outputMat.Clone();
                        return outputMat.ToWriteableBitmap();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return inputBitmap;
            }
        }

        public WriteableBitmap GetContourToImage(Mat thresholdMat, WriteableBitmap originBitmap)
        {
            if (thresholdMat == null)
            {
                return originBitmap;
            }

            try
            {
                using (Mat originMat = originBitmap.ToMat())
                {
                    Point[][] contours;
                    HierarchyIndex[] hierarchy;

                    Cv2.FindContours(thresholdMat, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, new Point(0, 0));

                    for (int i = 0; i < contours.Length; i++)
                    {
                        Cv2.DrawContours(originMat, contours, i, new Scalar(0, 255, 0), 2, LineTypes.Link8, hierarchy);
                    }

                    return originMat.ToWriteableBitmap();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return originBitmap;
            }
        }
    }
}
