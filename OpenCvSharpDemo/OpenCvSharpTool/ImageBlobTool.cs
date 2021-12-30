using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharpTool;
using VisionFroOpenCvSharpDll;
using Point = System.Windows.Point;

namespace OpenCvSharpTool
{
    [Flags]
    public enum ContourSelectCondition
    {
        None = 0,
        ContourSize = 1 << 0,
        ContourArea = 1 << 1,
        ContourPosition = 1 << 2
    }

    /// <summary>
    /// 图像采集工具
    /// </summary>
    public class ImageBlobTool : ToolBase
    {
        public ImageBlob ImageBlob { get; set; }
        private TreeViewItem _tree;
        #region InputParams
        public WriteableBitmap InputImage { get; set; }
        public string InputKey { get; set; }
        public bool IsAutoThreshold { get; set; }
        public int Threshold { get; set; }
        public MorphTypes MorphType { get; set; }
        public MorphShapes MorphShape { get; set; }
        public byte MorphElement { get; set; }
        public ContourSelectCondition SelectCondition { get; set; }
        public int ContourMaxSize { get; set; }
        public int ContourMinSize { get; set; }
        public int ContourMaxArea { get; set; }
        public int ContourMinArea { get; set; }
        public int ContourXStartPos { get; set; }
        public int ContourXEndPos { get; set; }
        public int ContourYStartPos { get; set; }
        public int ContourYEndPos { get; set; }

        #endregion

        #region OutputParams

        public WriteableBitmap OutputImage { get; set; }
        public Mat OutputMat { get; set; }
        public string Info { get; set; }
        public List<Point> BlobPosList { get; set; }
        public List<double> BlobAreaList { get; set; }
        #endregion
        public ImageBlobTool()
        {
            InitializeComponent();
            ImageBlob = new ImageBlob();
            ToolDesStr = "Blob分析";
            ToolIcon = "\xe729";
        }

        public override void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _tree = sender as TreeViewItem;
            if (_tree == null)
            {
                return;
            }
            ToolWindow = new ImageBlobWindow(this) { ToolTreeViewItem = _tree };

            base.UIElement_OnPreviewMouseLeftButtonDown(this, e);
        }

        public override void Run()
        {
            if (InputKey == null)
            {
                MessageBox.Show("请先配置Blob工具");
                return;
            }
            WriteableBitmap outputImage = null;
            Mat thresholdMat = null;
            Mat morphMat = null;
            Mat selectMat = null;
            string _info = "";
            InputImage = ToolRunBox.OutputObjects[InputKey] as WriteableBitmap;
            if (InputImage == null)
            {
                return;
            }

            ImageBlob.GetThresholdImage(Threshold, InputImage, ref thresholdMat, IsAutoThreshold, ref _info);
            ImageBlob.MorphologicalOperations(InputImage, thresholdMat, ref morphMat, MorphShape,
                MorphType, MorphElement, ref _info);

            if (morphMat==null)
            {
                return;
            }

            Mat srcMat = morphMat.Clone();
            selectMat = srcMat.Clone();

            if ((ContourSelectCondition.ContourSize & SelectCondition) > 0)
            {
               ImageBlob.SelectContourOperation(InputImage, srcMat, ref selectMat,
                    SelectContourType.ContourSize, ref _info, ContourMinSize, ContourMaxSize);
                srcMat = selectMat.Clone();
            }

            if ((ContourSelectCondition.ContourArea & SelectCondition) > 0)
            {
                selectMat = null;
                ImageBlob.SelectContourOperation(InputImage, srcMat, ref selectMat,
                  SelectContourType.ContourArea, ref _info, ContourMinArea, ContourMaxArea);
                srcMat = selectMat.Clone();
            }

            if ((ContourSelectCondition.ContourPosition & SelectCondition) > 0)
            {
                selectMat = null;
                ImageBlob.SelectContourOperation(InputImage, srcMat, ref selectMat,
                  SelectContourType.ContourLocation, ref _info, ContourXStartPos, ContourXEndPos, ContourYStartPos, ContourYEndPos);
            }

            srcMat.Dispose();

            BlobPosList = new List<Point>();
            BlobAreaList = new List<double>();

            ImageBlob.GetContourAreaAndPosition(selectMat,BlobPosList, BlobAreaList);

            OutputImage = ImageBlob.GetContourToImage(selectMat, InputImage, ref _info);
            OutputMat = selectMat?.Clone();
            Info = _info;

            if (ToolRunBox.OutputObjects.ContainsKey("Blob分析_" + nameof(OutputImage)))
            {
                ToolRunBox.OutputObjects["Blob分析_" + nameof(OutputImage)] = OutputImage;
                ToolRunBox.OutputObjects["Blob分析_" + nameof(OutputMat)] = OutputMat;
                ToolRunBox.OutputObjects["Blob分析_" + nameof(BlobPosList)] = BlobPosList;
                ToolRunBox.OutputObjects["Blob分析_" + nameof(BlobAreaList)] = BlobAreaList;
                ToolRunBox.OutputObjects["Blob分析_" + nameof(Info)] = Info;
            }
            else
            {
                ToolRunBox.OutputObjects.Add("Blob分析_" + nameof(OutputImage), OutputImage);
                ToolRunBox.OutputObjects.Add("Blob分析_" + nameof(OutputMat), OutputMat);
                ToolRunBox.OutputObjects.Add("Blob分析_" + nameof(BlobPosList), BlobPosList);
                ToolRunBox.OutputObjects.Add("Blob分析_" + nameof(BlobAreaList), BlobAreaList);
                ToolRunBox.OutputObjects.Add("Blob分析_" + nameof(Info), Info);
            }

            InitOutputParams();
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
                {nameof(OutputMat), OutputMat },
                { nameof(BlobPosList),BlobPosList},
                {nameof(BlobAreaList),BlobAreaList},
                {nameof(Info), Info},

            };

            if (OutputParams != null)
            {
                foreach (var runToolOutputParam in OutputParams)
                {
                    TreeViewItem treeView = new TreeViewItem
                    {
                        Header = runToolOutputParam.Key,
                        ToolTip = runToolOutputParam.Value,
                        Tag = runToolOutputParam.Value
                    };
                    outputItem?.Items.Add(treeView);
                }
            }
        }
    }
}
