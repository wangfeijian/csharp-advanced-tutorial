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
        static void MainProcess(string[] args)
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

        static void Main(string[] args)
        {
            //CreateImageMatOperator();
            CreateBasicMatOperator();
        }

        /// <summary>
        /// Mat对象的创建方法
        /// </summary>
        static void CreateImageMatOperator()
        {
            Mat matA, matC;
            matA = Cv2.ImRead(@"D:\Desktop Image\1.jpg");
            Mat matB=new Mat(matA,new Range(1,100));
            matC = new Mat(matA,new Rect(0,0,500,500));
            Mat matD =new Mat(matA,new Range(500,1000), new Range(500,1000));
            Cv2.ImShow("a",matA);
            Cv2.ImShow("b",matB);
            Cv2.ImShow("c",matC);
            Cv2.ImShow("d",matD);
            Cv2.WaitKey(0);
        }

        static void CreateBasicMatOperator()
        {
            Mat mat = new Mat(2,2,MatType.CV_8UC3,new Scalar(0,255,0));
            Console.WriteLine("默认风格输出");
            Console.WriteLine(Cv2.Format(mat));
            Console.WriteLine("C风格输出");
            Console.WriteLine(Cv2.Format(mat,FormatType.C));
            Console.WriteLine("CSV风格输出");
            Console.WriteLine(Cv2.Format(mat,FormatType.CSV));
            Console.WriteLine("MATLAB风格输出");
            Console.WriteLine(Cv2.Format(mat,FormatType.MATLAB));
            Console.WriteLine("NumPy风格输出");
            Console.WriteLine(Cv2.Format(mat,FormatType.NumPy));
            Console.WriteLine("python风格输出");
            Console.WriteLine(Cv2.Format(mat,FormatType.Python));

            Console.ReadKey();
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
