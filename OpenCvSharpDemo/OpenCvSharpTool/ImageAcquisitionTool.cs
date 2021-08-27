﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using VisionFroOpenCvSharpDll;

namespace OpenCvSharpTool
{
    /// <summary>
    /// 采集类型
    /// </summary>
    public enum ImageGrabType
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

        public ImageGrabType CurrenType { get; set; } = 0;

        public string FileName { get; set; }
        public string DirName { get; set; }
        public object CameraType { get; set; }
        public bool IsContinuousAcquisition { get; set; }
        public bool IsColorImage { get; set; } = true;
        public bool IsVideo { get; set; } = false;
        #endregion

        #region OutputParams

        public WriteableBitmap OutputImage { get; set; }
        public Mat OutputMat { get; set; }

        #endregion
        public ImageAcquisitionTool()
        {
           InitializeComponent();
            ToolDesStr = "图像采集";
            ToolIcon = "\xe967";
        }

        /// <summary>
        /// 初始化输入参数，根据类型
        /// </summary>
        private void InitParams()
        {
            switch (CurrenType)
            {
                //case ImageGrabType.CaptureFile:
                //    InputParams = new List<object>
                //    {
                //        FileName,
                //        IsColorImage,
                //        IsVideo
                //    };
                //    break;
                //case ImageGrabType.CaptureDir:
                //    InputParams = new List<object>
                //    {
                //        DirName,
                //        IsColorImage
                //    };
                //    break;
                //case ImageGrabType.CaptureCamera:
                //    InputParams = new List<object>
                //    {
                //        CameraType,
                //        IsContinuousAcquisition,
                //        IsColorImage
                //    };
                //    break;
            }
        }

        public override void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ToolWindow = new ImageAcquisitionWindow(this);
            base.UIElement_OnPreviewMouseLeftButtonDown(this,e);
        }

        public override void Run()
        {
        }
    }
}
