using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        private TreeViewItem _tree;
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
        public Mat ImageAcquisitionToolOutputMat { get; set; }
        public string Info { get; set; }
        public FileInfo[] FileInfos { get; set; }
        public int FileIndex { get; set; } 

        #endregion
        public ImageAcquisitionTool()
        {
            InitializeComponent();
            ImageGrab = new ImageGrab();
            ToolDesStr = "图像采集";
            ToolIcon = "\xe967";
        }

        public override void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
             _tree = sender as TreeViewItem;
            if (_tree == null)
            {
                return;
            }
            ToolWindow = new ImageAcquisitionWindow(this) {ToolTreeViewItem = _tree};
            
            base.UIElement_OnPreviewMouseLeftButtonDown(this, e);
        }

        public override void Run()
        {
            WriteableBitmap outputImage = null;
            string _info = "";

            if (InputParams.ContainsKey("FileName"))
            {
                OutputImage = ImageGrab.GetImageFromFile(FileName, ref outputImage, !IsColorImage, ref _info);
                Info = _info;
                InitOutputParams();
            }
            else if (InputParams.ContainsKey("DirName"))
            {
                if (FileInfos == null)
                {
                    FileInfos = new DirectoryInfo(DirName).GetFiles();
                }

                if (FileIndex == FileInfos.Length)
                {
                    FileIndex = 0;
                }

                OutputImage = ImageGrab.GetImageFromFile(FileInfos[FileIndex++].FullName, ref outputImage, !IsColorImage, ref _info);
                Info = _info;
                InitOutputParams();
            }
            else if (InputParams.ContainsKey("CameraType"))
            {

            }

        }

        private void InitOutputParams()
        {
            TreeViewItem outputItem = null;

            if (_tree != null)
                foreach (TreeViewItem treeItem in _tree.Items)
                {
                        if (treeItem.Header.ToString() == "输出")
                        {
                            outputItem = treeItem;
                            outputItem.Items.Clear();
                        }
                }

            OutputParams = new Dictionary<string, object>
            {
                {nameof(OutputImage), OutputImage},
                {nameof(Info), Info},
            };

            if (OutputParams != null)
            {
                foreach (var runToolOutputParam in OutputParams)
                {
                    TreeViewItem treeView = new TreeViewItem
                    {
                        Header = runToolOutputParam.Key,
                        ToolTip = runToolOutputParam.Value
                    };
                    outputItem?.Items.Add(treeView);
                }
            }
        }
    }
}
