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
        private string _info;
        public int FileNum => _filePaths.Length;
        /// <summary>
        /// 原始图片
        /// </summary>
        private WriteableBitmap _originalImage;
        /// <summary>
        /// 使用简单Blob分析后的图片
        /// </summary>
        private WriteableBitmap _simpleBlobImage;
        /// <summary>
        /// 二值化区域和原始图片合成的图片
        /// </summary>
        private WriteableBitmap _thresholdImage;
        /// <summary>
        /// 形态学处理后和原始图片合成的图片
        /// </summary>
        private WriteableBitmap _morphImage;
        /// <summary>
        /// 筛选轮廓区域合成的图片
        /// </summary>
        private WriteableBitmap _selectContourImage;
        /// <summary>
        /// 拟合图片
        /// </summary>
        private WriteableBitmap _fitImage;
        /// <summary>
        /// 二值化得到的区域
        /// </summary>
        private Mat _thresholdMat;
        /// <summary>
        /// 形态学处理后得到的区域
        /// </summary>
        private Mat _morphMat;
        /// <summary>
        /// 筛选轮廓区域后得到的新区域
        /// </summary>
        private Mat _selectCountourMat;
        /// <summary>
        /// 拟合轮廓区域
        /// </summary>
        private Mat _fitMat;

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
        public ICommand ShowMorphCommand { get; set; }
        public ICommand SelectContourCommand { get; set; }
        public ICommand FitContourCommand { get; set; }

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

        private string processInfo;
        /// <summary>
        /// 处理的信息
        /// </summary>
        public string ProcessInfo
        {
            get { return processInfo; }
            set { Set(ref processInfo, value); }
        }

        private WriteableBitmap originBitmap;
        /// <summary>
        /// 原始图片
        /// </summary>
        public WriteableBitmap OriginBitmap
        {
            get { return originBitmap; }
            set { Set(ref originBitmap, value); }
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

        private bool morphologicalEnable;
        /// <summary>
        /// 是否启用形态学操作
        /// </summary>
        public bool MorphologicalEnable
        {
            get { return morphologicalEnable; }
            set { Set(ref morphologicalEnable, value); }
        }

        private byte morphType = 0;
        /// <summary>
        /// 形态学类型
        /// </summary>
        public byte MorphType
        {
            get { return morphType; }
            set { Set(ref morphType, value); }
        }

        private byte morphShape = 0;
        /// <summary>
        /// 结构形状
        /// </summary>
        public byte MorphShape
        {
            get { return morphShape; }
            set { Set(ref morphShape, value); }
        }

        private byte morphElement = 5;
        /// <summary>
        /// 结构大小
        /// </summary>
        public byte MorphElement
        {
            get { return morphElement; }
            set { Set(ref morphElement, value); }
        }

        private bool contourSelectEnable;
        /// <summary>
        /// 是否启用轮廓筛选
        /// </summary>
        public bool ContourSelectEnable
        {
            get { return contourSelectEnable; }
            set { Set(ref contourSelectEnable, value); }
        }

        private bool contourSizeSelectEnable;
        /// <summary>
        /// 启用轮廓大小筛选
        /// </summary>
        public bool ContourSizeSelectEnable
        {
            get { return contourSizeSelectEnable; }
            set { Set(ref contourSizeSelectEnable, value); }
        }

        private bool contourAreaSelectEnable;
        /// <summary>
        /// 启用轮廓面积筛选
        /// </summary>
        public bool ContourAreaSelectEnable
        {
            get { return contourAreaSelectEnable; }
            set { Set(ref contourAreaSelectEnable, value); }
        }

        private int contourMaxSize = 100;
        /// <summary>
        /// 轮廓大小最大值
        /// </summary>
        public int ContourMaxSize
        {
            get { return contourMaxSize; }
            set { Set(ref contourMaxSize, value); }
        }

        private int contourMinSize = 20;
        /// <summary>
        /// 轮廓大小最小值
        /// </summary>
        public int ContourMinSize
        {
            get { return contourMinSize; }
            set { Set(ref contourMinSize, value); }
        }
        private int contourMinArea = 500;
        /// <summary>
        /// 轮廓面积最小值
        /// </summary>
        public int ContourMinArea
        {
            get { return contourMinArea; }
            set { Set(ref contourMinArea, value); }
        }

        private int contourMaxArea = 1000;
        /// <summary>
        /// 轮廓面积最大值
        /// </summary>
        public int ContourMaxArea
        {
            get { return contourMaxArea; }
            set { Set(ref contourMaxArea, value); }
        }

        private bool contourLocationSelectEnable;
        /// <summary>
        /// 轮廓位置
        /// </summary>
        public bool ContourLocationSelectEnable
        {
            get { return contourLocationSelectEnable; }
            set { Set(ref contourLocationSelectEnable, value); }
        }

        private int contourXStartPos = 50;
        /// <summary>
        /// 轮廓X起始坐标
        /// </summary>
        public int ContourXStartPos
        {
            get { return contourXStartPos; }
            set { Set(ref contourXStartPos, value); }
        }

        private int contourXEndPos = 80;
        /// <summary>
        /// 轮廓X结束坐标
        /// </summary>
        public int ContourXEndPos
        {
            get { return contourXEndPos; }
            set { Set(ref contourXEndPos, value); }
        }

        private int contourYStartPos = 50;
        /// <summary>
        /// 轮廓Y起始坐标
        /// </summary>
        public int ContourYStartPos
        {
            get { return contourYStartPos; }
            set { Set(ref contourYStartPos, value); }
        }

        private int contourYEndPos = 80;
        /// <summary>
        /// 轮廓Y结束坐标
        /// </summary>
        public int ContourYEndPos
        {
            get { return contourYEndPos; }
            set { Set(ref contourYEndPos, value); }
        }

        private bool contourFitEnable;
        /// <summary>
        /// 是否启用轮廓拟合
        /// </summary>
        public bool ContourFitEnable
        {
            get { return contourFitEnable; }
            set { contourFitEnable = value; }
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
            ShowMorphCommand = new RelayCommand<object>(ShowMorphImage);
            SelectContourCommand = new RelayCommand<object>(SelectContourImage);
            FitContourCommand = new RelayCommand<object>(FitContourImage);
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
            if (_imageGrab.GetVideoFromFile(PathTextBoxText, ref _index, ref _info))
            {
                ButtonStartPlayEnable = true;
                ButtonStopPlayEnable = true;
                ProcessInfo = _info;
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
                var image = _imageGrab.GetVideoFrame(isGray, ref _info);
                ProcessInfo = _info;
                if (image == null || _isStop)
                {
                    break;
                }

                DispatcherHelper.Delay(Convert.ToInt32(Frame));
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

            ShowBitmap = _imageBlob.GetBlobedImageSimple(pParams, _originalImage, ref _info);
            ProcessInfo = _info;
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
            OriginBitmap = _originalImage.Clone();
        }

        private void BlobAdvanAnalyzeImage(object sender)
        {
            if (AutoThresholdEnable)
            {
                ShowBitmap = _imageBlob.GetThresholdImage(ThresholdValue, _originalImage, ref _thresholdMat, false, ref _info);
                ProcessInfo = _info;
            }
            else
            {
                ShowBitmap = _imageBlob.GetThresholdImage(ThresholdValue, _originalImage, ref _thresholdMat, true, ref _info);
                ProcessInfo = _info;
            }
            _thresholdImage = ShowBitmap.Clone();
        }

        private void ShowContourImage(object sender)
        {
            if (_thresholdImage == null || _originalImage == null)
            {
                return;
            }

            ShowBitmap = _imageBlob.GetContourToImage(_thresholdMat, _originalImage, ref _info);
            ProcessInfo = _info;
        }

        private void ShowMorphImage(object sender)
        {
            if (_thresholdMat == null || _originalImage == null)
            {
                return;
            }

            ShowBitmap = _imageBlob.MorphologicalOperations(_originalImage, _thresholdMat, ref _morphMat, (MorphShapes)MorphShape, (MorphTypes)MorphType, MorphElement, ref _info);
            ProcessInfo = _info;
            _morphImage = ShowBitmap.Clone();
        }

        private void SelectContourImage(object sender)
        {
            if (_thresholdMat == null || _originalImage == null)
            {
                ProcessInfo = "未采集图片或未进行二值化处理";
                return;
            }

            if (MorphologicalEnable && _morphMat == null)
            {
                ProcessInfo = "选择形态学处理时，请先进行形态学处理，再进行筛选";
                return;
            }

            Mat srcMat = MorphologicalEnable ? _morphMat.Clone() : _thresholdMat.Clone();
            _selectCountourMat = new Mat(srcMat.Size(), MatType.CV_8UC1);

            if (ContourSizeSelectEnable)
            {
                _imageBlob.SelectContourOperation(_originalImage, srcMat, ref _selectCountourMat,
                    SelectContourType.ContourSize, ref _info, ContourMinSize, ContourMaxSize);
                ProcessInfo = _info;
                _selectContourImage = ShowBitmap.Clone();
            }

            if (ContourAreaSelectEnable)
            {
                 _imageBlob.SelectContourOperation(_originalImage, _selectCountourMat, ref _selectCountourMat,
                    SelectContourType.ContourArea, ref _info, ContourMinArea, ContourMaxArea);
                ProcessInfo = _info;
                _selectContourImage = ShowBitmap.Clone();
            }

            if (ContourLocationSelectEnable)
            {
                _imageBlob.SelectContourOperation(_originalImage, _selectCountourMat, ref _selectCountourMat,
                    SelectContourType.ContourLocation, ref _info, ContourXStartPos, ContourXEndPos, ContourYStartPos, ContourYEndPos);
                ProcessInfo = _info;
                _selectContourImage = ShowBitmap.Clone();
            }

            srcMat.Dispose();
        }

        /// <summary>
        /// 拟合轮廓
        /// </summary>
        /// <param name="sender"></param>
        private void FitContourImage(object sender)
        {
            RadioButton button = sender as RadioButton;

            if (button == null)
            {
                return;
            }

            switch (button.Tag.ToString())
            {
                case "0":
                    ShowBitmap = _imageBlob.FitSharpeToImage(_originalImage, ref _selectCountourMat, ref _fitMat, 0, ref _info);
                    ProcessInfo = _info;
                    _fitImage = ShowBitmap.Clone();
                    break;
                case "1":
                    ShowBitmap = _imageBlob.FitSharpeToImage(_originalImage, ref _selectCountourMat, ref _fitMat, 1, ref _info);
                    ProcessInfo = _info;
                    _fitImage = ShowBitmap.Clone();
                    break;
                case "2":
                    ShowBitmap = _imageBlob.FitSharpeToImage(_originalImage, ref _selectCountourMat, ref _fitMat, 2, ref _info);
                    ProcessInfo = _info;
                    _fitImage = ShowBitmap.Clone();
                    break;
            }
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
            ShowBitmap = _imageGrab.GetImageFromFile(fileName, ref _originalImage, RadioButtonGrayImageIsChecked, ref _info);
            OriginBitmap = _originalImage.Clone();
            ProcessInfo = _info;

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


        public class DispatcherHelper
        {
            public static void Delay(int milliSecond)
            {
                int start = Environment.TickCount;
                while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
                {
                    DoEvents();
                }
            }
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