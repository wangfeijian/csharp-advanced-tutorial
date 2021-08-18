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
                @"D:\Project\7月\螺丝机\保护罩三号\7_Calib\下相机十二点标定-2020-1003-1444-44\1884716798.jpg");
            Cv2.ImShow("Source", srcImage);
            Cv2.WaitKey(1); // do events
            Mat dstImage = MyFindContours(srcImage);

            Cv2.ImShow("dst", dstImage);
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

        static Mat MyFindContours(Mat srcImage)
        {
            Mat cannyImage = new Mat();
            Mat srcGray = new Mat();

            Cv2.CvtColor(srcImage,srcGray,ColorConversionCodes.RGB2GRAY);
            Cv2.Canny(srcGray, cannyImage, 100, 200);

            Point[][] contours;
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(cannyImage, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, new Point(0, 0));
            Mat dstMat = Mat.Zeros(cannyImage.Size(), srcImage.Type());

            for (int i = 0; i < contours.Length; i++)
            {
                Cv2.DrawContours(srcImage, contours, i, Scalar.Green, 2, LineTypes.Link8, hierarchy);
            }

            return srcImage;
        }
    }
}
