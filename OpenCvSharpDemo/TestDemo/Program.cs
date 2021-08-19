using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace TestDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Mat srcImage = Cv2.ImRead(
                @"C:\Users\Public\Documents\MVTec\HALCON-19.11-Progress\examples\images\die\die_03.png");
            Cv2.ImShow("Source", srcImage);
            Cv2.WaitKey(1); // do events
            Mat morpMat = new Mat();
            Mat dstImage = MyFindContours(srcImage,ref morpMat);

            Cv2.ImShow("dst", dstImage);
            Cv2.WaitKey(1);

            Mat morphMat = MorphologicalOperations(morpMat);

            Cv2.ImShow("mor", morphMat);
            Cv2.WaitKey(1);

            ThresholdImage(srcImage);
            Cv2.DestroyAllWindows();

            srcImage.Dispose();
        }

        static void ThresholdImage(Mat srcImage)
        {
            Console.WriteLine("请输入数字0-255：");
            string s = Console.ReadLine();

            while (s != "q")
            {
                double threshold;
                double.TryParse(s, out threshold);

                if (threshold != null)
                {
                    var binaryImage = new Mat(srcImage.Size(), MatType.CV_8UC1);
                    Cv2.Threshold(srcImage, binaryImage, thresh: threshold, maxval: 255, type: ThresholdTypes.Binary);

                    Cv2.ImShow("Key Points", binaryImage);
                    Cv2.WaitKey(1); // do events
                    binaryImage.Dispose();
                }
                Console.WriteLine("如果要退出请输入q，请输入数字0-255：");
                s = Console.ReadLine();
            }
        }

        static Mat MyFindContours(Mat srcImage,ref Mat contoursMat)
        {
            Mat srcMat = new Mat(srcImage.Size(), MatType.CV_8UC1);
            Cv2.CvtColor(srcImage, srcMat, ColorConversionCodes.RGB2GRAY);

            Mat dstMat = new Mat();
            Cv2.Threshold(srcMat, dstMat, thresh: 45, maxval: 255, type: ThresholdTypes.Binary);

            contoursMat = dstMat.Clone();
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(dstMat, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, new Point(0, 0));

            for (int i = 0; i < contours.Length; i++)
            {
                Cv2.DrawContours(srcImage, contours, i, Scalar.Green, 2, LineTypes.Link8, hierarchy);
            }

            return srcImage;
        }

        static Mat MorphologicalOperations(Mat srcMat)
        {
            Mat dstMat = new Mat();
            Mat element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(15.5, 15.5));
            Cv2.MorphologyEx(srcMat, dstMat, MorphTypes.Close, element);
            return dstMat;
        }
    }
}
