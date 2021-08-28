using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Permissions;
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
using System.Windows.Threading;
using Microsoft.Win32;

namespace OpenCvSharpTool
{
    /// <summary>
    /// ImageAcquisitionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImageAcquisitionWindow : Window
    {
        public ImageAcquisitionTool ImageGrabTool { get; set; }

        private int _index;
        private bool _isStop;
        private string[] _filePath;
        private double _frame = 30;
        public int FileNum => _filePath.Length;

        public ImageAcquisitionWindow(ImageAcquisitionTool toolBase)
        {
            InitializeComponent();
            ImageGrabTool = toolBase;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                TreeViewItem inputItem = null;
                TreeView tree = ImageGrabTool.Content as TreeView;

                if (tree != null)
                    foreach (TreeViewItem treeItem in tree.Items)
                    {
                        foreach (TreeViewItem item in treeItem.Items)
                        {
                            if (item.Header.ToString() == "输入")
                            {
                                inputItem = item;
                                inputItem.Items.Clear();
                            }
                        }
                    }

                InitParams();

                if (ImageGrabTool.InputParams != null)
                {
                    foreach (var runToolInputParam in ImageGrabTool.InputParams)
                    {
                        TreeViewItem treeView = new TreeViewItem
                        {
                            Header = runToolInputParam.Key,
                            ToolTip = runToolInputParam.Value
                        };
                        inputItem?.Items.Add(treeView);
                    }
                }

                MessageBox.Show("配置输入参数完成");
                TestGroup.IsEnabled = true;
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
            ImageGrabTool.FileIndex = 0;
            ImageGrabTool.FileInfos = null;

            switch (ImageGrabTool.CurrenType)
            {
                case ImageGrabType.CaptureFile:
                    ImageGrabTool.FileName = PathTextBox.Text;
                    ImageGrabTool.IsColorImage = RadioButtonColorImage.IsChecked == true;
                    ImageGrabTool.InputParams = new Dictionary<string, object>
                    {
                        {nameof(ImageGrabTool.FileName), ImageGrabTool.FileName},
                        {nameof(ImageGrabTool.IsColorImage), ImageGrabTool.IsColorImage},
                        {nameof(ImageGrabTool.IsVideo), ImageGrabTool.IsVideo},
                    };
                    break;
                case ImageGrabType.CaptureDir:
                    ImageGrabTool.DirName = PathTextBox.Text;
                    ImageGrabTool.IsColorImage = RadioButtonColorImage.IsChecked == true;
                    ImageGrabTool.InputParams = new Dictionary<string, object>
                    {
                        {nameof(ImageGrabTool.DirName), ImageGrabTool.DirName},
                        {nameof(ImageGrabTool.IsColorImage), ImageGrabTool.IsColorImage},
                    };
                    break;
                case ImageGrabType.CaptureCamera:
                    ImageGrabTool.InputParams = new Dictionary<string, object>
                    {
                        {nameof(ImageGrabTool.CameraType), ImageGrabTool.CameraType},
                        {nameof(ImageGrabTool.IsColorImage), ImageGrabTool.IsColorImage},
                        {nameof(ImageGrabTool.IsContinuousAcquisition), ImageGrabTool.IsContinuousAcquisition},
                    };
                    break;
            }
        }

        private void RadioButtonFile_OnClick(object sender, RoutedEventArgs e)
        {
            SelectImageStackPanel.IsEnabled = false;
            ButtonStartPlay.IsEnabled = false;
            ButtonStopPlay.IsEnabled = false;

            RadioButton radioButton = sender as RadioButton;
            int index = Convert.ToInt32(radioButton.Tag.ToString());

            ImageGrabTool.CurrenType = (ImageGrabType)index;

            switch (radioButton.Name)
            {
                case "RadioButtonFile":
                    GetFilePath();
                    ImageGrabTool.FileName = PathTextBox.Text;
                    break;
                case "RadioButtonDir":
                    GetFilePath(false);
                    ImageGrabTool.DirName = PathTextBox.Text;
                    break;
                case "RadioButtonCamera":
                    break;
            }
        }

        private void GetFilePath(bool flag = true)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "All files(*.*)|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;
                string dirName = fileName.Substring(0, fileName.LastIndexOf('\\'));
                PathTextBox.Text = flag ? fileName : dirName;
                return;
            }

            PathTextBox.Text = "";
        }

        private void RadioButtonColorImage_OnClick(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            ImageGrabTool.IsColorImage = radioButton.Name == "RadioButtonColorImage";
        }

        private void ButtonGrabImageClick(object sender, RoutedEventArgs e)
        {
            string fileName = PathTextBox.Text;
            if (RadioButtonFile.IsChecked == true)
            {
                ShowSingleImage(fileName);
            }
            else if (RadioButtonDir.IsChecked == true)
            {
                ShowMultImage(fileName);
            }
            else if (RadioButtonCamera.IsChecked == true)
            {
                ShowCameraImage();
            }
        }

        private void ButtonPreviewClick(object sender, RoutedEventArgs e)
        {
            ButtonNext.IsEnabled = true;
            if (_index == 0)
            {
                ButtonPreview.IsEnabled = false;
                ShowSingleImage(_filePath[_index]);
                return;
            }

            _index--;

            ShowSingleImage(_filePath[_index]);
        }

        private void ButtonNextClick(object sender, RoutedEventArgs e)
        {
            _index++;
            ButtonPreview.IsEnabled = true;
            if (_index == FileNum - 1)
            {
                ButtonNext.IsEnabled = false;
                ShowSingleImage(_filePath[_index]);
                return;
            }

            ShowSingleImage(_filePath[_index]);
        }

        private void ShowSingleImage(string fileName)
        {
            WriteableBitmap originBitmap = null;
            string info = "";
            ShowImageWindow.ShowImageBitmap = ImageGrabTool.ImageGrab.GetImageFromFile(fileName, ref originBitmap,
                RadioButtonGrayImage.IsChecked == true, ref info);
            ImageGrabTool.OutputImage = originBitmap.Clone();
        }

        private void ShowMultImage(string fileName)
        {
            _index = 0;
            SelectImageStackPanel.IsEnabled = true;
            DirectoryInfo dir = new DirectoryInfo(fileName);
            FileInfo[] fileInfos = dir.GetFiles();

            if (fileInfos.Length <= 0)
            {
                MessageBox.Show("文件夹中没有文件");
                return;
            }

            var files = from item in fileInfos
                        select item.FullName;

            _filePath = files.ToArray();

            ShowSingleImage(_filePath[0]);
        }

        private void ButtonLoadVideoClick(object sender, RoutedEventArgs e)
        {
            string _info = "";
            _index = 0;
            _isStop = false;
            if (ImageGrabTool.ImageGrab.GetVideoFromFile(PathTextBox.Text, ref _index, ref _info))
            {
                ButtonStartPlay.IsEnabled = true;
                ButtonStopPlay.IsEnabled = true;
                return;
            }

            MessageBox.Show("文件读取失败");
        }

        private void ShowCameraImage()
        {

        }

        private void FrameSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _frame = FrameSlider.Value;
        }

        private void ButtonStartPlayClick(object sender, RoutedEventArgs e)
        {
            string _info = "";
            bool isGray = RadioButtonGrayImage.IsChecked == true;

            if (_index == 0)
            {
                MessageBox.Show("没有可用的帧");
                return;
            }

            while (true)
            {
                var image = ImageGrabTool.ImageGrab.GetVideoFrame(isGray, ref _info);
                if (image == null || _isStop)
                {
                    break;
                }

                DispatcherHelper.Delay(Convert.ToInt32(_frame));
                ShowImageWindow.ShowImageBitmap = image;
            }

            ButtonStartPlay.IsEnabled = false;
            ButtonStopPlay.IsEnabled = false;
            MessageBox.Show("播放完成");
            _isStop = false;
        }

        private void ButtonStopPlayClick(object sender, RoutedEventArgs e)
        {
            _isStop = true;
        }

        

        private void ButtonTest_OnClick(object sender, RoutedEventArgs e)
        {
            if (RadioButtonFile.IsChecked == true)
            {
                ShowSingleImage(ImageGrabTool.FileName);
            }
            else if (RadioButtonDir.IsChecked == true)
            {
                ShowMultImage(ImageGrabTool.DirName);
            }
            else if (RadioButtonCamera.IsChecked == true)
            {
                ShowCameraImage();
            }
        }
    }
    public class DispatcherHelper
    {
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond) //毫秒
            {
                DoEvents();
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);
            try
            {
                Dispatcher.PushFrame(frame);
            }
            catch (InvalidOperationException)
            {
            }
        }

        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }
}
