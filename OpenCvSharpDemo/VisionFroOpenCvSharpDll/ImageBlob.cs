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
    public enum SelectContourType
    {
        ContourSize,
        ContourArea,
        ContourLocation
    }
    public class ImageBlob
    {
        private SimpleBlobDetector _blobDetector;

        /// <summary>
        /// 获取简单Blob分析图像
        /// </summary>
        /// <param name="pParams">简blob分析所用到的参数</param>
        /// <param name="inputBitmap">原始图片</param>
        /// <param name="info">返回的处理信息</param>
        /// <returns></returns>
        public WriteableBitmap GetBlobedImageSimple(SimpleBlobDetector.Params pParams, WriteableBitmap inputBitmap, ref string info)
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
                            info = "处理成功";
                            return outputMat.ToWriteableBitmap();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                info = e.ToString();
                return inputBitmap;
            }
        }

        /// <summary>
        /// 获取二值化图像
        /// </summary>
        /// <param name="threshold">二值化阈值</param>
        /// <param name="inputBitmap">原始图片</param>
        /// <param name="thresholdMat">输出二值化后的数据</param>
        /// <param name="flag">是否自动二值化处理</param>
        /// <param name="info">处理的信息</param>
        /// <returns></returns>
        public WriteableBitmap GetThresholdImage(double threshold, WriteableBitmap inputBitmap, ref Mat thresholdMat, bool flag, ref string info)
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
                        if (!flag)
                        {
                            Cv2.Threshold(inputMat, outputMat, threshold, 255, ThresholdTypes.BinaryInv);
                        }
                        else
                        {
                            Cv2.Threshold(inputMat, outputMat, 0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Triangle);

                        }

                        thresholdMat = outputMat.Clone();
                        info = "处理成功";
                        return outputMat.ToWriteableBitmap();
                    }
                }

            }
            catch (Exception e)
            {
                info = e.ToString();
                return inputBitmap;
            }
        }

        /// <summary>
        /// 区域轮廓显示
        /// </summary>
        /// <param name="thresholdMat">二值化后的数据</param>
        /// <param name="originBitmap">原始图片</param>
        /// <param name="info">处理的信息</param>
        /// <returns></returns>
        public WriteableBitmap GetContourToImage(Mat thresholdMat, WriteableBitmap originBitmap, ref string info)
        {
            if (thresholdMat == null)
            {
                info = "不存在有效二值化数据";
                return originBitmap;
            }

            try
            {
                using (Mat originMat = originBitmap.ToMat())
                {
                    Point[][] contours;
                    HierarchyIndex[] hierarchy;

                    Cv2.FindContours(thresholdMat, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxNone, new Point(0, 0));

                    for (int i = 0; i < contours.Length; i++)
                    {
                        Cv2.DrawContours(originMat, contours, i, new Scalar(0, 255, 0), hierarchy: hierarchy);
                    }

                    info = "处理成功";
                    return originMat.ToWriteableBitmap();
                }
            }
            catch (Exception e)
            {
                info = e.ToString();
                return originBitmap;
            }
        }

        /// <summary>
        /// 形态学操作，通过此操作获取对应的图像
        /// </summary>
        /// <param name="originImage">原始图片</param>
        /// <param name="srcMat">二值化后的数据</param>
        /// <param name="dstMat">通过形态学处理后的数据</param>
        /// <param name="morphShape">结构类型</param>
        /// <param name="morphType">形态学类型</param>
        /// <param name="size">结构大小</param>
        /// <param name="info">处理的结果</param>
        /// <returns></returns>
        public WriteableBitmap MorphologicalOperations(WriteableBitmap originImage, Mat srcMat, ref Mat dstMat, MorphShapes morphShape, MorphTypes morphType, byte size, ref string info)
        {
            if (srcMat == null)
            {
                info = "不存在有效二值化数据";
                return originImage;
            }

            try
            {
                using (Mat element = Cv2.GetStructuringElement(morphShape, new Size(size, size)))
                {
                    using (Mat dstMatTemp = new Mat())
                    {
                        Cv2.MorphologyEx(srcMat, dstMatTemp, morphType, element);
                        dstMat = dstMatTemp.Clone();
                        info = "处理成功";
                        return GetContourToImage(dstMatTemp, originImage, ref info);
                    }
                }
            }
            catch (Exception e)
            {
                info = e.ToString();
                return originImage;
            }
        }

        /// <summary>
        /// 轮廓筛选
        /// </summary>
        /// <param name="originImage">原始图片</param>
        /// <param name="srcMat">处理源数据</param>
        /// <param name="dstMat">处理后的数据</param>
        /// <param name="seletContourType">筛选类型</param>
        /// <param name="info">筛选信息</param>
        /// <param name="paramInts">筛选参数</param>
        /// <returns></returns>
        public void SelectContourOperation(WriteableBitmap originImage, Mat srcMat,ref Mat dstMat,
            SelectContourType seletContourType, ref string info, params int[] paramInts)
        {
            if (srcMat == null)
            {
                info = "不存在有效二值化数据";
                //return originImage;
            }

            try
            {

                switch (seletContourType)
                {
                    case SelectContourType.ContourSize:

                        //通过轮廓外径筛选
                        dstMat = SelectContourByCondition(srcMat, paramInts, args => (int)Cv2.ArcLength(args,false));
                        info = "处理成功";
                        break;
                    //return GetContourToImage(dstMat, originImage, ref info);

                    case SelectContourType.ContourArea:

                        //通过轮廓面积筛选
                        dstMat = SelectContourByCondition(srcMat, paramInts, args => (int)Cv2.ContourArea(args));
                        info = "处理成功";
                        break;
                    //return GetContourToImage(dstMat, originImage, ref info);

                    case SelectContourType.ContourLocation:

                        //通过行列坐标筛选
                        dstMat = SelectContourByCondition(srcMat, paramInts, null, false);
                        info = "处理成功";
                        break;
                        //return GetContourToImage(dstMat, originImage, ref info);

                }
                //info = "处理失败，选择类型不正确";
                //return originImage;
            }
            catch (Exception e)
            {
                info = e.ToString();
                // return originImage;
            }
        }

        /// <summary>
        /// 根据条件筛选轮廓
        /// </summary>
        /// <param name="srcMat"></param>
        /// <param name="paramInts"></param>
        /// <param name="func">条件</param>
        /// <param name="flag"></param>
        private Mat SelectContourByCondition(Mat srcMat, int[] paramInts, Func<Point[], int> func, bool flag = true)
        {
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(srcMat, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxNone);

            using (Mat originMat =  Mat.Zeros(srcMat.Rows,srcMat.Cols,MatType.CV_8UC1))
            {
                for (int i = 0; i < contours.Length; i++)
                {
                    if (flag)
                    {
                        if (func(contours[i]) > paramInts[0] && func(contours[i]) < paramInts[1])
                        {
                            Cv2.DrawContours(originMat, contours, i, new Scalar(255, 255, 255), Cv2.FILLED);
                        }
                    }
                    else
                    {
                        var m = Cv2.Moments(contours[i], true);
                        int xPos = (int)(m.M10 / m.M00);
                        int yPos = (int)(m.M01 / m.M00);

                        bool condition = xPos > paramInts[0] && xPos < paramInts[1] && yPos > paramInts[2] && yPos < paramInts[3];
                        if (condition)
                        {
                            Cv2.DrawContours(originMat, contours, i, new Scalar(255, 255, 255), Cv2.FILLED);
                        }
                    }
                }

                return originMat.Clone();
            }

        }


        public WriteableBitmap FitSharpeToImage(WriteableBitmap originImage, ref Mat srcMat, ref Mat dstMat, int index, ref string info)
        {
            if (srcMat == null)
            {
                info = "不存在有效拟合数据";
                return originImage;
            }

            try
            {
                switch (index)
                {
                    case 0:
                        using (dstMat = new Mat(originImage.ToMat().Size(), MatType.CV_8UC1))
                        {
                            return FitCircleToImage(srcMat, ref dstMat, originImage, ref info);
                        }
                    case 1:
                        using (dstMat = new Mat(originImage.ToMat().Size(), MatType.CV_8UC1))
                        {
                            return FitRectangleToImage(srcMat, ref dstMat, originImage, ref info);
                        }
                    case 2:
                        using (dstMat = new Mat(originImage.ToMat().Size(), MatType.CV_8UC1))
                        {
                            return FitHullToImage(srcMat, ref dstMat, originImage, ref info);
                        }
                }

                info = "处理失败，选择类型不正确";
                return originImage;
            }
            catch (Exception e)
            {
                info = e.ToString();
                return originImage;
            }
        }

        /// <summary>
        /// 区域轮廓、拟合最小外接矩形
        /// </summary>
        /// <param name="thresholdMat">二值化后的数据</param>
        /// <param name="originBitmap">原始图片</param>
        /// <param name="info">处理的信息</param>
        /// <returns></returns>
        private WriteableBitmap FitRectangleToImage(Mat thresholdMat, ref Mat fitMat, WriteableBitmap originBitmap, ref string info)
        {
            if (thresholdMat == null)
            {
                info = "不存在有效二值化数据";
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
                        RotatedRect rect = Cv2.MinAreaRect(contours[i]);
                        Point[] ps = new Point[4];
                        for (int j = 0; j < 4; j++)
                        {
                            ps[j] = new Point(rect.Points()[j].X, rect.Points()[j].Y);
                        }

                        Cv2.DrawContours(originMat, contours, i, new Scalar(0, 255, 0), hierarchy: hierarchy);

                        Cv2.Polylines(originMat, new[] { ps }, true, new Scalar(0, 0, 255));
                    }

                    info = "处理成功";
                    return originMat.ToWriteableBitmap();
                }
            }
            catch (Exception e)
            {
                info = e.ToString();
                return originBitmap;
            }
        }

        /// <summary>
        /// 区域轮廓、拟合最小外接圆
        /// </summary>
        /// <param name="thresholdMat">二值化后的数据</param>
        /// <param name="originBitmap">原始图片</param>
        /// <param name="info">处理的信息</param>
        /// <returns></returns>
        private WriteableBitmap FitCircleToImage(Mat thresholdMat, ref Mat fitMat, WriteableBitmap originBitmap, ref string info)
        {
            if (thresholdMat == null)
            {
                info = "不存在有效二值化数据";
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
                        Point2f point2F;
                        float radius;
                        Cv2.MinEnclosingCircle(contours[i], out point2F, out radius);

                        Cv2.DrawContours(originMat, contours, i, new Scalar(0, 255, 0), hierarchy: hierarchy);

                        Cv2.Circle(originMat, new Point(point2F.X, point2F.Y), (int)radius, new Scalar(0, 0, 255));
                    }

                    info = "处理成功";
                    return originMat.ToWriteableBitmap();
                }
            }
            catch (Exception e)
            {
                info = e.ToString();
                return originBitmap;
            }
        }

        /// <summary>
        /// 区域轮廓、拟合凸包
        /// </summary>
        /// <param name="thresholdMat">二值化后的数据</param>
        /// <param name="originBitmap">原始图片</param>
        /// <param name="info">处理的信息</param>
        /// <returns></returns>
        private WriteableBitmap FitHullToImage(Mat thresholdMat, ref Mat fitMat, WriteableBitmap originBitmap, ref string info)
        {
            if (thresholdMat == null)
            {
                info = "不存在有效二值化数据";
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
                        Mat r3 = new Mat();
                        var point = Cv2.ConvexHull(contours[i]);

                        Cv2.DrawContours(originMat, contours, i, new Scalar(0, 255, 0), hierarchy: hierarchy);

                        Cv2.DrawContours(originMat, new[] { point }, 0, new Scalar(0, 0, 255));
                    }

                    info = "处理成功";
                    return originMat.ToWriteableBitmap();
                }
            }
            catch (Exception e)
            {
                info = e.ToString();
                return originBitmap;
            }
        }
    }
}
