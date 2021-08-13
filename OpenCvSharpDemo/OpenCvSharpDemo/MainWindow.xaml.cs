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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using VisionFroOpenCvSharpDll;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Threading;
using OpenCvSharp;
using Window = System.Windows.Window;

namespace OpenCvSharpDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ImageGrab _imageGrab;
        private string[] _filePaths;
        private bool _isStop = false;
        private WriteableBitmap _bitmap;
        private int _index;
        public int FileNum => _filePaths.Length;

        public MainWindow()
        {
            InitializeComponent();
            _imageGrab = new ImageGrab();
        }

        private void ImageSourceButton_OnChecked(object sender, RoutedEventArgs e)
        {
            SelectImageStackPanel.IsEnabled = false;
            ButtonStartPlay.IsEnabled = false;
            ButtonStoptPlay.IsEnabled = false;
            RadioButton button = sender as RadioButton;

            if (button == null)
            {
                return;
            }

            switch (button.Name)
            {
                case "RadioButtonFile":
                    GetFilePath();
                    break;
                case "RadioButtonDir":
                    GetFilePath(false);
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

        private void ButtonGrabImage_OnClick(object sender, RoutedEventArgs e)
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

        private void ShowSingleImage(string fileName)
        {
            bool flag = false;
            ImreadModes imreadModes =
                RadioButtonGrayImage.IsChecked == true ? ImreadModes.Grayscale : ImreadModes.Color;
            _bitmap = _imageGrab.GetImageFromFile(fileName, imreadModes, ref flag);

            if (_bitmap == null)
            {
                MessageBox.Show("文件不正确");
                return;
            }

            ImageShow.Source = _bitmap;
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

            _filePaths = files.ToArray();

            ShowSingleImage(_filePaths[0]);
        }

        private void ShowCameraImage()
        {

        }

        private void ButtonPreview_OnClick(object sender, RoutedEventArgs e)
        {
            ButtonNext.IsEnabled = true;
            if (_index == 0)
            {
                ButtonPreview.IsEnabled = false;
                ShowSingleImage(_filePaths[_index]);
                return;
            }
            _index--;

            ShowSingleImage(_filePaths[_index]);
        }

        private void ButtonNext_OnClick(object sender, RoutedEventArgs e)
        {
            _index++;
            ButtonPreview.IsEnabled = true;
            if (_index == FileNum - 1)
            {
                ButtonNext.IsEnabled = false;
                ShowSingleImage(_filePaths[_index]);
                return;
            }

            ShowSingleImage(_filePaths[_index]);
        }

        private void ButtonLoad_OnClick(object sender, RoutedEventArgs e)
        {
            _index = 0;
            _isStop = false;
            if (_imageGrab.GetVideoFromFile(PathTextBox.Text, ref _index))
            {
                ButtonStartPlay.IsEnabled = true;
                ButtonStoptPlay.IsEnabled = true;
                return;
            }

            MessageBox.Show("文件读取失败");
        }


        private void ButtonStartPlay_OnClick(object sender, RoutedEventArgs e)
        {
            bool isGray = RadioButtonGrayImage.IsChecked == true;

            if (_index == 0)
            {
                MessageBox.Show("没有可用的帧");
                return;
            }

            while (true)
            {
                var image = _imageGrab.GetVideoFrame(isGray);
                if (image == null || _isStop)
                {
                    break;
                }

                Delay(Convert.ToInt32(FrameSlider.Value));
                ImageShow.Source = image;
            }

            ButtonStartPlay.IsEnabled = false;
            ButtonStoptPlay.IsEnabled = false;
            MessageBox.Show("播放完成");
            _isStop = false;
        }

        private void ButtonStoptPlay_OnClick(object sender, RoutedEventArgs e)
        {
            _isStop = true;
        }

        public void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                DispatcherHelper.DoEvents();
            }
        }

        public class DispatcherHelper
        {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public static void DoEvents()
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
                try { Dispatcher.PushFrame(frame); }
                catch (InvalidOperationException) { }
            }
            private static object ExitFrames(object frame)
            {
                ((DispatcherFrame)frame).Continue = false;
                return null;
            }
        }
    }
}
