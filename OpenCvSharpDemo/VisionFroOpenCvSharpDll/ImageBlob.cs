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
                        if (flag)
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

                    Cv2.FindContours(thresholdMat, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, new Point(0, 0));

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
        public WriteableBitmap SelectContourOperation(WriteableBitmap originImage, ref Mat srcMat,
            ref Mat dstMat,
            SelectContourType seletContourType, ref string info, params int[] paramInts)
        {
            if (srcMat == null)
            {
                info = "不存在有效二值化数据";
                return originImage;
            }

            try
            {
                switch (seletContourType)
                {
                    case SelectContourType.ContourSize:
                        using (Mat originMat = originImage.ToMat())
                        {
                            Point[][] contours;
                            HierarchyIndex[] hierarchy;
                            Cv2.FindContours(srcMat, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, new Point(0, 0));
                            for (int i = 0; i < contours.Length; i++)
                            {
                                if (contours[i].Length > paramInts[0])
                                {
                                    Cv2.DrawContours(originMat, contours, i, new Scalar(0, 255, 0), hierarchy: hierarchy);
                                }
                            }

                            info = "处理成功";
                            return originMat.ToWriteableBitmap();
                        }
                    case SelectContourType.ContourArea:
                        break;
                    case SelectContourType.ContourLocation:
                        break;
                }
                return null;
            }
            catch (Exception e)
            {
                info = e.ToString();
                return originImage;
            }
        }
    }
}
