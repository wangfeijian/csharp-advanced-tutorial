using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using VisionFroOpenCvSharpDll;

namespace OpenCvSharpTool
{
    /// <summary>
    /// 采集类型
    /// </summary>
    public enum CaptureType
    {
        /// <summary>
        /// 文件
        /// </summary>
        CaptureFile,
        /// <summary>
        /// 文件夹
        /// </summary>
        CaptureDir,
        /// <summary>
        /// 相机
        /// </summary>
        CaptureCamera
    }

    /// <summary>
    /// 图像采集工具
    /// </summary>
    public class ImageAcquisitionTool : ToolBase
    {
        public ImageGrab ImageGrab { get; set; }
        #region InputParams

        public CaptureType CurrenType { get; set; }

        public string FileName { get; set; }
        public string DirName { get; set; }
        public object CameraType { get; set; }
        public bool IsContinuousAcquisition { get; set; }
        public bool IsColorImage { get; set; }
        public bool IsVideo { get; set; }

        #endregion

        #region OutputParams

        public WriteableBitmap OutputImage { get; set; }
        public Mat OutputMat { get; set; }

        #endregion
        public ImageAcquisitionTool(CaptureType type)
        {
            CurrenType = type;
            InitParams();
        }

        /// <summary>
        /// 初始化输入参数，根据类型
        /// </summary>
        private void InitParams()
        {
            switch (CurrenType)
            {
                case CaptureType.CaptureFile:
                    InputParams = new List<object>
                    {
                        FileName,
                        IsColorImage,
                        IsVideo
                    };
                    break;
                case CaptureType.CaptureDir:
                    InputParams = new List<object>
                    {
                        DirName,
                        IsColorImage
                    };
                    break;
                case CaptureType.CaptureCamera:
                    InputParams = new List<object>
                    {
                        CameraType,
                        IsContinuousAcquisition,
                        IsColorImage
                    };
                    break;
            }
        }
        public override void Run()
        {
        }
    }
}
