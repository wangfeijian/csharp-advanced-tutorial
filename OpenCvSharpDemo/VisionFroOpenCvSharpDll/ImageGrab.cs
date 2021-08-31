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

        /// <summary>
        /// 从文件读取图片
        /// </summary>
        /// <param name="fileName">文件位置</param>
        /// <param name="originBitmap">原始图片</param>
        /// <param name="flag">是否为灰度图片</param>
        /// <param name="info">采集的信息</param>
        /// <returns></returns>
        public WriteableBitmap GetImageFromFile(string fileName, ref WriteableBitmap originBitmap, bool flag, ref string info)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            try
            {
                using (Mat mat = Cv2.ImRead(fileName))
                {
                    originBitmap = mat.ToWriteableBitmap();
                    if (flag)
                    {
                        using (Mat grayMat = new Mat())
                        {
                            Cv2.CvtColor(mat, grayMat, ColorConversionCodes.RGB2GRAY);
                            info = "采集成功";
                            return grayMat.ToWriteableBitmap();
                        }
                    }

                    info = "采集成功";
                    return mat.ToWriteableBitmap();
                }
            }
            catch (Exception e)
            {
                info = e.ToString();
                return null;
            }
        }

        /// <summary>
        /// 从文件获取视频
        /// </summary>
        /// <param name="fileName">视频路径</param>
        /// <param name="num">帧数</param>
        /// <param name="info">采集信息</param>
        /// <returns></returns>
        public bool GetVideoFromFile(string fileName, ref int num, ref string info)
        {
            _index = 0;
            _video = new VideoCapture();
            if (!File.Exists(fileName))
            {
                _video.Dispose();
                info = "文件不存在";
                return false;
            }

            try
            {
                _video.Open(fileName);
                if (!_video.IsOpened())
                {
                    _video.Dispose();
                    info = "文件打开失败";
                    return false;
                }

                num = _video.FrameCount;
                info = "采集成功";
                return true;
            }
            catch (Exception e)
            {
                info = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 获取视频的帧
        /// </summary>
        /// <param name="isGray">是否为灰度图片</param>
        /// <param name="info">采集信息</param>
        /// <returns></returns>
        public WriteableBitmap GetVideoFrame(bool isGray, ref string info)
        {
            if (_index == _video.FrameCount)
            {
                _video.Dispose();
                info = "视频播放完毕或不存在有效帧";
                return null;
            }

            try
            {


                using (Mat mat = new Mat())
                {
                    _video.Read(mat);
                    if (isGray)
                    {
                        using (Mat newMat = new Mat())
                        {
                            Cv2.CvtColor(mat, newMat, ColorConversionCodes.RGB2GRAY);
                            _index++;
                            info = "采集成功";
                            return newMat.ToWriteableBitmap();
                        }
                    }
                    _index++;
                    info = "采集成功";
                    return mat.ToWriteableBitmap();
                }
            }
            catch (Exception e)
            {
                info = e.ToString();
                return null;
            }
        }
    }
}
