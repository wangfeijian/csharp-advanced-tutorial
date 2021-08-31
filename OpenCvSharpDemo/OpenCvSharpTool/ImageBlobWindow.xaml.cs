using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using VisionFroOpenCvSharpDll;
using Window = System.Windows.Window;

namespace OpenCvSharpTool
{
    /// <summary>
    /// ImageBlobWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImageBlobWindow : Window
    {
        public TreeViewItem ToolTreeViewItem { get; set; }
        public ImageBlobTool ImageBlobTool { get; set; }

        private Mat _thresholdMat;
        private Mat _morphMat;
        private Mat _selectCountourMat;
        public List<string> ImageInputList { get; set; }
        public ImageBlobWindow(ImageBlobTool tool)
        {
            InitializeComponent();
            ImageBlobTool = tool;
            InitComboBox();
        }

        private void InitComboBox()
        {
            ImageInputList = new List<string>();
            foreach (var outputObject in ToolRunBox.OutputObjects)
            {
                if (outputObject.Value is WriteableBitmap)
                {
                    ImageInputList.Add(outputObject.Key);
                }
            }

            ImageSelectCombox.ItemsSource = ImageInputList;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                TreeViewItem inputItem = null;

                if (ToolTreeViewItem != null)
                    foreach (TreeViewItem treeItem in ToolTreeViewItem.Items)
                    {
                        if (treeItem.Header.ToString() == "输入")
                        {
                            inputItem = treeItem;
                            inputItem.Items.Clear();
                        }
                    }

                InitParams();

                if (ImageBlobTool.InputParams != null)
                {
                    foreach (var runToolInputParam in ImageBlobTool.InputParams)
                    {
                        TreeViewItem treeView = new TreeViewItem
                        {
                            Header = runToolInputParam.Key,
                            ToolTip = runToolInputParam.Value,
                            Tag = runToolInputParam.Value
                        };
                        inputItem?.Items.Add(treeView);
                    }
                }

                MessageBox.Show("配置输入参数完成");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        /// <summary>
        /// 初始化输入参数，根据类型
        /// </summary>
        private void InitParams()
        {
            ImageBlobTool.InputParams = new Dictionary<string, object>
            {
                {nameof(ImageBlobTool.InputImage), ImageBlobTool.InputImage},
                {nameof(ImageBlobTool.InputKey), ImageBlobTool.InputKey}
            };

            // 二值化阈值
            ImageBlobTool.IsAutoThreshold = AutoThresholdEnable.IsChecked == true;
            ImageBlobTool.Threshold = (byte)ThresholdSlider.Value;

            if (ImageBlobTool.IsAutoThreshold)
            {
                ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.IsAutoThreshold), ImageBlobTool.IsAutoThreshold);
            }
            else
            {
                ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.Threshold), ImageBlobTool.Threshold);
            }

            // 形态学
            ImageBlobTool.MorphType = (MorphTypes)MorphTypeCombox.SelectedIndex;
            ImageBlobTool.MorphShape = (MorphShapes)MorphShapeCombox.SelectedIndex;
            ImageBlobTool.MorphElement = (byte)MorphologicalElementSizeSlider.Value;
            ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.MorphType), ImageBlobTool.MorphType);
            ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.MorphShape), ImageBlobTool.MorphShape);
            ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.MorphElement), ImageBlobTool.MorphElement);

            // 轮廓筛选
            ImageBlobTool.SelectCondition = ContourSizeSelectEnable.IsChecked == false
                ? ContourSelectCondition.None
                : ContourSelectCondition.ContourSize;
            ImageBlobTool.SelectCondition = ContourAreaSelectEnable.IsChecked == false
                ? ContourSelectCondition.None | ImageBlobTool.SelectCondition
                : ImageBlobTool.SelectCondition | ContourSelectCondition.ContourArea;
            ImageBlobTool.SelectCondition = ContourLocationSelectEnable.IsChecked == false
                ? ContourSelectCondition.None | ImageBlobTool.SelectCondition
                : ImageBlobTool.SelectCondition | ContourSelectCondition.ContourPosition;
            ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.SelectCondition), ImageBlobTool.SelectCondition);

            if (ContourSizeSelectEnable.IsChecked == true)
            {
                ImageBlobTool.ContourMinSize = ContourSizeMin.ParamValue;
                ImageBlobTool.ContourMaxSize = ContourSizeMax.ParamValue;
                ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.ContourMinSize), ImageBlobTool.ContourMinSize);
                ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.ContourMaxSize), ImageBlobTool.ContourMaxSize);
            }

            if (ContourAreaSelectEnable.IsChecked == true)
            {
                ImageBlobTool.ContourMinArea = ContourAreaMin.ParamValue;
                ImageBlobTool.ContourMaxArea = ContourAreaMax.ParamValue;
                ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.ContourMinArea), ImageBlobTool.ContourMinArea);
                ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.ContourMaxArea), ImageBlobTool.ContourMaxArea);
            }

            if (ContourLocationSelectEnable.IsChecked == true)
            {
                ImageBlobTool.ContourXStartPos = ContourXStartPos.ParamValue;
                ImageBlobTool.ContourXEndPos = ContourXEndPos.ParamValue;
                ImageBlobTool.ContourYStartPos = ContourYStartPos.ParamValue;
                ImageBlobTool.ContourYEndPos = ContourYEndPos.ParamValue;
                ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.ContourXStartPos), ImageBlobTool.ContourXStartPos);
                ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.ContourXEndPos), ImageBlobTool.ContourXEndPos);
                ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.ContourYStartPos), ImageBlobTool.ContourYStartPos);
                ImageBlobTool.InputParams.Add(nameof(ImageBlobTool.ContourYEndPos), ImageBlobTool.ContourYEndPos);
            }
        }

        private void ImageSelectCombox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null || comboBox.Name != "ImageSelectCombox")
            {
                return;
            }

            ImageBlobTool.InputKey = ImageSelectCombox.SelectedValue.ToString();
            ImageBlobTool.InputImage = ToolRunBox.OutputObjects[ImageSelectCombox.SelectedValue.ToString()] as WriteableBitmap;
        }

        private void ShowContourEnable_OnClick(object sender, RoutedEventArgs e)
        {
            string info = "";
            TestCameraWindow.ShowImageBitmap = ImageBlobTool.ImageBlob.GetThresholdImage(ThresholdSlider.Value,
                ImageBlobTool.InputImage, ref _thresholdMat, AutoThresholdEnable.IsChecked == true, ref info);
        }

        private void ThresholdSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AutoThresholdEnable.IsChecked == true)
            {
                return;
            }

            ShowContourEnable_OnClick(null, null);
        }

        private void MorphologicalElementSizeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ImageBlobTool?.InputImage == null)
            {
                return;
            }

            string info = "";
            if (_thresholdMat==null)
            {
                MessageBox.Show("请先进行二值化处理");
                return;
            }

            ImageBlobTool.ImageBlob.MorphologicalOperations(ImageBlobTool.InputImage,
                _thresholdMat, ref _morphMat, (MorphShapes) MorphShapeCombox.SelectedIndex,
                (MorphTypes) MorphTypeCombox.SelectedIndex, (byte) MorphologicalElementSizeSlider.Value, ref info);
            TestCameraWindow.ShowImageBitmap = _morphMat.ToWriteableBitmap();
        }

        private void ContourSizeMin_OnValueChange(object sender, RoutedEventArgs e)
        {
            string info = "";
            if (ImageBlobTool?.InputImage == null)
            {
                return;
            }

            if (ImageSelectCombox.SelectedItem==null)
            {
                return;
            }
            if (_thresholdMat == null)
            {
                MessageBox.Show("请先进行二值化处理");
                return;
            }

            Mat srcMat = _morphMat == null ?_thresholdMat.Clone() :  _morphMat.Clone();

            if (ContourSizeSelectEnable.IsChecked==true)
            {
                _selectCountourMat = null;
               ImageBlobTool.ImageBlob.SelectContourOperation(ImageBlobTool.InputImage, srcMat, ref _selectCountourMat,
                    SelectContourType.ContourSize, ref info, ContourSizeMin.ParamValue, ContourSizeMax.ParamValue);
                srcMat = _selectCountourMat.Clone();
            }

            if (ContourAreaSelectEnable.IsChecked==true)
            {
                _selectCountourMat = null;
                ImageBlobTool.ImageBlob.SelectContourOperation(ImageBlobTool.InputImage, srcMat, ref _selectCountourMat,
                    SelectContourType.ContourArea, ref info, ContourAreaMin.ParamValue, ContourAreaMax.ParamValue);
                srcMat = _selectCountourMat.Clone();
            }

            if (ContourLocationSelectEnable.IsChecked==true)
            {
                _selectCountourMat = null;
               ImageBlobTool.ImageBlob.SelectContourOperation(ImageBlobTool.InputImage, srcMat, ref _selectCountourMat,
                    SelectContourType.ContourLocation, ref info, ContourXStartPos.ParamValue, ContourXEndPos.ParamValue, ContourYStartPos.ParamValue, ContourYEndPos.ParamValue);
            }

            //TestCameraWindow.ShowImageBitmap = _selectCountourMat.ToWriteableBitmap();
            TestCameraWindow.ShowImageBitmap = ImageBlobTool.ImageBlob.GetContourToImage(_selectCountourMat, ImageBlobTool.InputImage, ref info);
            srcMat.Dispose();
        }
    }
}
