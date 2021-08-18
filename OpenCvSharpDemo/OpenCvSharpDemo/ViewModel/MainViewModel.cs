using System.Windows.Input;
using GalaSoft.MvvmLight;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.CommandWpf;
using VisionFroOpenCvSharpDll;
using System.Windows.Controls;
using Microsoft.Win32;
using OpenCvSharp;
using System.Windows;
using System.IO;
using System.Linq;
using System;
using System.Security.Permissions;
using System.Windows.Threading;

namespace OpenCvSharpDemo.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region 字段

        private ImageGrab _imageGrab;
        private ImageBlob _imageBlob;
        private string[] _filePaths;
        private bool _isStop = false;
        private int _index;
        public int FileNum => _filePaths.Length;
        private WriteableBitmap _originalImage;
        private WriteableBitmap _simpleBlobImage;
        private WriteableBitmap _thresholdImage;
        private Mat _thresholdMat;

        #endregion

        #region 命令

        public ICommand SelectImageSourceCommand { get; set; }
        public ICommand GrabImageCommand { get; set; }
        public ICommand PreviewImageCommand { get; set; }
        public ICommand NextImageCommand { get; set; }
        public ICommand LoadVideoCommand { get; set; }
        public ICommand StartPlayCommand { get; set; }
        public ICommand StopPlayCommand { get; set; }
        public ICommand BlobAnalyzeCommand { get; set; }
        public ICommand BlobAnalyzeAdvanceCommand { get; set; }
        public ICommand ShowOriginImageCommand { get; set; }
        public ICommand ShowContourCommand { get; set; }

        #endregion
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            _imageGrab = new ImageGrab();
            _imageBlob = new ImageBlob();
            InitCommand();
        }

        #region 绑定属性
        private bool selectImageStackPanelEnable;
        /// <summary>
        /// 图片源选择StackPanel是否启用
        /// </summary>
        public bool SelectImageStackPanelEnable
        {
            get { return selectImageStackPanelEnable; }
            set { Set(ref selectImageStackPanelEnable, value); }
        }

        private bool buttonStartPlayEnable;
        /// <summary>
        /// 开始播放按钮是否启用
        /// </summary>
        public bool ButtonStartPlayEnable
        {
            get { return buttonStartPlayEnable; }
            set { Set(ref buttonStartPlayEnable, value); }
        }

        private bool buttonStopPlayEnable;
        /// <summary>
        /// 停止播放按钮是否启用
        /// </summary>
        public bool ButtonStopPlayEnable
        {
            get { return buttonStopPlayEnable; }
            set { Set(ref buttonStopPlayEnable, value); }
        }

        private WriteableBitmap showBitmap;

        /// <summary>
        /// 显示的图片源
        /// </summary>
        public WriteableBitmap ShowBitmap
        {
            get { return showBitmap; }
            set { Set(ref showBitmap, value); }
        }

        private string pathTextBoxText;
        /// <summary>
        /// 文件路径显示textbox的text
        /// </summary>
        public string PathTextBoxText
        {
            get { return pathTextBoxText; }
            set { Set(ref pathTextBoxText, value); }
        }

        private bool radioButtonFileIsChecked;
        /// <summary>
        /// 图片来源是否为文件
        /// </summary>
        public bool RadioButtonFileIsChecked
        {
            get { return radioButtonFileIsChecked; }
            set { Set(ref radioButtonFileIsChecked, value); }
        }

        private bool radioButtonDirIsChecked;
        /// <summary>
        /// 图片来源是否为文件夹
        /// </summary>
        public bool RadioButtonDirIsChecked
        {
            get { return radioButtonDirIsChecked; }
            set { Set(ref radioButtonDirIsChecked, value); }
        }

        private bool radioButtonCameraIsChecked;
        /// <summary>
        /// 图片来源是否为相机
        /// </summary>
        public bool RadioButtonCameraIsChecked
        {
            get { return radioButtonCameraIsChecked; }
            set { Set(ref radioButtonCameraIsChecked, value); }
        }

        private bool radioButtonGrayImageIsChecked = true;
        /// <summary>
        /// 是否转换为灰度图片
        /// </summary>
        public bool RadioButtonGrayImageIsChecked
        {
            get { return radioButtonGrayImageIsChecked; }
            set { Set(ref radioButtonGrayImageIsChecked, value); }
        }

        private bool buttonPreviewIsEnabled = true;
        /// <summary>
        /// 上一张图片按钮是否启用
        /// </summary>
        public bool ButtonPreviewIsEnabled
        {
            get { return buttonPreviewIsEnabled; }
            set { Set(ref buttonPreviewIsEnabled, value); }
        }


        private bool buttonNextIsEnabled = true;
        /// <summary>
        /// 下一张图片按钮是否启用
        /// </summary>
        public bool ButtonNextIsEnabled
        {
            get { return buttonNextIsEnabled; }
            set { Set(ref buttonNextIsEnabled, value); }
        }

        private double frame = 30;
        /// <summary>
        /// 播放视频时帧间隔时间
        /// </summary>
        public double Frame
        {
            get { return frame; }
            set { Set(ref frame, value); }
        }

        private bool blobEnable;

        public bool BlobEnable
        {
            get { return blobEnable; }
            set { Set(ref blobEnable, value); }
        }

        private bool byColorEnable;
        /// <summary>
        /// 是否启用灰度值过滤
        /// </summary>
        public bool ByColorEnable
        {
            get { return byColorEnable; }
            set { Set(ref byColorEnable, value); }

        }

        private bool byCircularityEnable;
        /// <summary>
        /// 是否启用圆度过滤
        /// </summary>
        public bool ByCircularityEnable
        {
            get { return byCircularityEnable; }
            set { Set(ref byCircularityEnable, value); }
        }

        private bool byAreaEnable;
        /// <summary>
        /// 是否启用面积过滤
        /// </summary>
        public bool ByAreaEnable
        {
            get { return byAreaEnable; }
            set { Set(ref byAreaEnable, value); }
        }

        private bool byConvexityEnable;
        /// <summary>
        /// 是否启用凸性过滤
        /// </summary>
        public bool ByConvexityEnable
        {
            get { return byConvexityEnable; }
            set { Set(ref byConvexityEnable, value); }
        }

        private bool byInertiaEnable;
        /// <summary>
        /// 是否启用惯性比过滤
        /// </summary>
        public bool ByInertiaEnable
        {
            get { return byInertiaEnable; }
            set { Set(ref byInertiaEnable, value); }
        }

        private float thresholdStep = 10f;
        /// <summary>
        /// 阈值递进值
        /// </summary>
        public float ThresholdStep
        {
            get { return thresholdStep; }
            set { Set(ref thresholdStep, value); }
        }


        private float thresholdMinDis = 10f;
        /// <summary>
        /// 阈值之间的最小距离
        /// </summary>
        public float ThresholdMinDis
        {
            get { return thresholdMinDis; }
            set { Set(ref thresholdMinDis, value); }
        }

        private uint thresholdTimes = 2;
        /// <summary>
        /// 阈值之间重复次数
        /// </summary>
        public uint ThresholdTimes
        {
            get { return thresholdTimes; }
            set { Set(ref thresholdTimes, value); }
        }

        private float thresholdMin = 10f;
        /// <summary>
        /// 阈值最小值
        /// </summary>
        public float ThresholdMin
        {
            get { return thresholdMin; }
            set { Set(ref thresholdMin, value); }
        }

        private float thresholdMax = 100f;
        /// <summary>
        /// 阈值最小值
        /// </summary>
        public float ThresholdMax
        {
            get { return thresholdMax; }
            set { Set(ref thresholdMax, value); }
        }

        private byte byColorValue;
        /// <summary>
        /// 启用颜色过滤时的值
        /// </summary>
        public byte BycolorValue
        {
            get { return byColorValue; }
            set { Set(ref byColorValue, value); }
        }

        private float byCircularityMinValue;
        /// <summary>
        /// 圆度最小值
        /// </summary>
        public float ByCircularityMinValue
        {
            get { return byCircularityMinValue; }
            set { Set(ref byCircularityMinValue, value); }
        }

        private float byCircularityMaxValue;
        /// <summary>
        /// 圆度最大值
        /// </summary>
        public float ByCircularityMaxValue
        {
            get { return byCircularityMaxValue; }
            set { Set(ref byCircularityMaxValue, value); }
        }

        private float byAreaMinValue;
        /// <summary>
        /// 面积最小值
        /// </summary>
        public float ByAreaMinValue
        {
            get { return byAreaMinValue; }
            set { Set(ref byAreaMinValue, value); }
        }

        private float byAreaMaxValue;
        /// <summary>
        /// 面积最大值
        /// </summary>
        public float ByAreaMaxValue
        {
            get { return byAreaMaxValue; }
            set { Set(ref byAreaMaxValue, value); }
        }

        private float byConvexityMinValue;
        /// <summary>
        /// 凸性最小值
        /// </summary>
        public float ByConvexityMinValue
        {
            get { return byConvexityMinValue; }
            set { Set(ref byConvexityMinValue, value); }
        }

        private float byConvexityMaxValue;
        /// <summary>
        /// 凸性最大值
        /// </summary>
        public float ByConvexityMaxValue
        {
            get { return byConvexityMaxValue; }
            set { Set(ref byConvexityMaxValue, value); }
        }

        private float byInertiaMinValue;
        /// <summary>
        /// 惯性比最小值
        /// </summary>
        public float ByInertiaMinValue
        {
            get { return byInertiaMinValue; }
            set { Set(ref byInertiaMinValue, value); }
        }

        private float byInertiaMaxValue;
        /// <summary>
        /// 惯性比最大值
        /// </summary>
        public float ByInertiaMaxValue
        {
            get { return byInertiaMaxValue; }
            set { Set(ref byInertiaMaxValue, value); }
        }

        private double thresholdValue = 100;
        /// <summary>
        /// 二值化阈值
        /// </summary>
        public double ThresholdValue
        {
            get { return thresholdValue; }
            set { thresholdValue = value; }
        }

        private bool thresholdEnable;
        /// <summary>
        /// 是否启用二值化
        /// </summary>
        public bool ThresholdEnable
        {
            get { return thresholdEnable; }
            set { Set(ref thresholdEnable, value); }
        }

        private bool autoThresholdEnable;
        /// <summary>
        /// 是否启用自动阈值
        /// </summary>
        public bool AutoThresholdEnable
        {
            get { return autoThresholdEnable; }
            set { Set(ref autoThresholdEnable, value); }
        }
        #endregion

        private void InitCommand()
        {
            SelectImageSourceCommand = new RelayCommand<object>(ImageSourceButtonClick);
            GrabImageCommand = new RelayCommand<object>(ButtonGrabImageClick);
            PreviewImageCommand = new RelayCommand<object>(ButtonPreviewClick);
            NextImageCommand = new RelayCommand<object>(ButtonNextClick);
            LoadVideoCommand = new RelayCommand<object>(ButtonLoadVideoClick);
            StartPlayCommand = new RelayCommand<object>(ButtonStartPlayClick);
            StopPlayCommand = new RelayCommand<object>(ButtonStopPlayClick);
            BlobAnalyzeCommand = new RelayCommand<object>(BlobAnalyzeImage);
            BlobAnalyzeAdvanceCommand = new RelayCommand<object>(BlobAdvanAnalyzeImage);
            ShowOriginImageCommand = new RelayCommand<object>(ShowOriginImage);
            ShowContourCommand = new RelayCommand<object>(ShowContourImage);
        }

        #region 绑定方法

        private void ImageSourceButtonClick(object sender)
        {
            SelectImageStackPanelEnable = false;
            buttonStartPlayEnable = false;
            buttonStopPlayEnable = false;
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

        private void ButtonGrabImageClick(object sender)
        {
            string fileName = PathTextBoxText;
            if (RadioButtonFileIsChecked == true)
            {
                ShowSingleImage(fileName);
            }
            else if (RadioButtonDirIsChecked == true)
            {
                ShowMultImage(fileName);
            }
            else if (RadioButtonCameraIsChecked == true)
            {
                ShowCameraImage();
            }
        }

        private void ButtonPreviewClick(object sender)
        {
            ButtonNextIsEnabled = true;
            if (_index == 0)
            {
                ButtonPreviewIsEnabled = false;
                ShowSingleImage(_filePaths[_index]);
                return;
            }
            _index--;

            ShowSingleImage(_filePaths[_index]);
        }

        private void ButtonNextClick(object sender)
        {
            _index++;
            ButtonPreviewIsEnabled = true;
            if (_index == FileNum - 1)
            {
                ButtonNextIsEnabled = false;
                ShowSingleImage(_filePaths[_index]);
                return;
            }

            ShowSingleImage(_filePaths[_index]);
        }

        private void ButtonLoadVideoClick(object sender)
        {
            _index = 0;
            _isStop = false;
            if (_imageGrab.GetVideoFromFile(PathTextBoxText, ref _index))
            {
                ButtonStartPlayEnable = true;
                ButtonStopPlayEnable = true;
                return;
            }

            MessageBox.Show("文件读取失败");
        }

        private void ButtonStartPlayClick(object sender)
        {
            bool isGray = RadioButtonGrayImageIsChecked == true;

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

                Delay(Convert.ToInt32(Frame));
                ShowBitmap = image;
            }

            ButtonStartPlayEnable = false;
            ButtonStopPlayEnable = false;
            MessageBox.Show("播放完成");
            _isStop = false;
        }

        private void ButtonStopPlayClick(object sender)
        {
            _isStop = true;
        }

        private void BlobAnalyzeImage(object sender)
        {
            if (!BlobEnable)
            {
                return;
            }

            SimpleBlobDetector.Params pParams = new SimpleBlobDetector.Params();

            // 给SimpleBlobDetector参数赋值
            // 1、根据界面选择是否启用某些参数
            pParams.FilterByArea = ByAreaEnable;
            pParams.FilterByCircularity = ByCircularityEnable;
            pParams.FilterByColor = ByColorEnable;
            pParams.FilterByConvexity = ByConvexityEnable;
            pParams.FilterByInertia = ByInertiaEnable;

            // 2、几个必要的参数先赋值
            pParams.ThresholdStep = ThresholdStep;
            pParams.MinThreshold = ThresholdMin;
            pParams.MaxThreshold = ThresholdMax;
            pParams.MinRepeatability = ThresholdTimes;
            pParams.MinDistBetweenBlobs = ThresholdMinDis;

            // 3、设置其它参数，数值来自界面
            pParams.BlobColor = BycolorValue;
            pParams.MinArea = ByAreaMinValue;
            pParams.MaxArea = ByAreaMaxValue;
            pParams.MinCircularity = ByCircularityMinValue;
            pParams.MaxCircularity = ByCircularityMaxValue;
            pParams.MinConvexity = ByConvexityMinValue;
            pParams.MaxConvexity = ByConvexityMaxValue;
            pParams.MinInertiaRatio = ByInertiaMinValue;
            pParams.MaxInertiaRatio = ByInertiaMaxValue;

            ShowBitmap = _imageBlob.GetBlobedImageSimple(pParams, _originalImage);
            _simpleBlobImage = ShowBitmap.Clone();
        }

        private void ShowOriginImage(object sender)
        {
            if (_originalImage == null)
            {
                MessageBox.Show("还未采集图片，请先采集图片");
                return;
            }

            ShowBitmap = _originalImage.Clone();
        }

        private void BlobAdvanAnalyzeImage(object sender)
        {
            if (AutoThresholdEnable)
            {
                ShowBitmap = _imageBlob.GetThresholdImage(ThresholdValue, _originalImage, ref _thresholdMat, false);
            }
            else
            {
                ShowBitmap = _imageBlob.GetThresholdImage(ThresholdValue, _originalImage, ref _thresholdMat);
            }
            _thresholdImage = ShowBitmap.Clone();
        }

        private void ShowContourImage(object sender)
        {
            if (_thresholdImage == null || _originalImage == null)
            {
                return;
            }

            ShowBitmap = _imageBlob.GetContourToImage(_thresholdMat, _originalImage);
        }
        #endregion

        #region 其它方法

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
                PathTextBoxText = flag ? fileName : dirName;
                return;
            }

            PathTextBoxText = "";
        }

        private void ShowSingleImage(string fileName)
        {
            ShowBitmap = _imageGrab.GetImageFromFile(fileName, ref _originalImage, RadioButtonGrayImageIsChecked);

            if (ShowBitmap == null)
            {
                MessageBox.Show("文件不正确");
                return;
            }
        }

        private void ShowMultImage(string fileName)
        {
            _index = 0;
            SelectImageStackPanelEnable = true;
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
        #endregion
    }
}