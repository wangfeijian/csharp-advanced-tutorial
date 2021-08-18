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
        #region �ֶ�

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

        #region ����

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

        #region ������
        private bool selectImageStackPanelEnable;
        /// <summary>
        /// ͼƬԴѡ��StackPanel�Ƿ�����
        /// </summary>
        public bool SelectImageStackPanelEnable
        {
            get { return selectImageStackPanelEnable; }
            set { Set(ref selectImageStackPanelEnable, value); }
        }

        private bool buttonStartPlayEnable;
        /// <summary>
        /// ��ʼ���Ű�ť�Ƿ�����
        /// </summary>
        public bool ButtonStartPlayEnable
        {
            get { return buttonStartPlayEnable; }
            set { Set(ref buttonStartPlayEnable, value); }
        }

        private bool buttonStopPlayEnable;
        /// <summary>
        /// ֹͣ���Ű�ť�Ƿ�����
        /// </summary>
        public bool ButtonStopPlayEnable
        {
            get { return buttonStopPlayEnable; }
            set { Set(ref buttonStopPlayEnable, value); }
        }

        private WriteableBitmap showBitmap;

        /// <summary>
        /// ��ʾ��ͼƬԴ
        /// </summary>
        public WriteableBitmap ShowBitmap
        {
            get { return showBitmap; }
            set { Set(ref showBitmap, value); }
        }

        private string pathTextBoxText;
        /// <summary>
        /// �ļ�·����ʾtextbox��text
        /// </summary>
        public string PathTextBoxText
        {
            get { return pathTextBoxText; }
            set { Set(ref pathTextBoxText, value); }
        }

        private bool radioButtonFileIsChecked;
        /// <summary>
        /// ͼƬ��Դ�Ƿ�Ϊ�ļ�
        /// </summary>
        public bool RadioButtonFileIsChecked
        {
            get { return radioButtonFileIsChecked; }
            set { Set(ref radioButtonFileIsChecked, value); }
        }

        private bool radioButtonDirIsChecked;
        /// <summary>
        /// ͼƬ��Դ�Ƿ�Ϊ�ļ���
        /// </summary>
        public bool RadioButtonDirIsChecked
        {
            get { return radioButtonDirIsChecked; }
            set { Set(ref radioButtonDirIsChecked, value); }
        }

        private bool radioButtonCameraIsChecked;
        /// <summary>
        /// ͼƬ��Դ�Ƿ�Ϊ���
        /// </summary>
        public bool RadioButtonCameraIsChecked
        {
            get { return radioButtonCameraIsChecked; }
            set { Set(ref radioButtonCameraIsChecked, value); }
        }

        private bool radioButtonGrayImageIsChecked = true;
        /// <summary>
        /// �Ƿ�ת��Ϊ�Ҷ�ͼƬ
        /// </summary>
        public bool RadioButtonGrayImageIsChecked
        {
            get { return radioButtonGrayImageIsChecked; }
            set { Set(ref radioButtonGrayImageIsChecked, value); }
        }

        private bool buttonPreviewIsEnabled = true;
        /// <summary>
        /// ��һ��ͼƬ��ť�Ƿ�����
        /// </summary>
        public bool ButtonPreviewIsEnabled
        {
            get { return buttonPreviewIsEnabled; }
            set { Set(ref buttonPreviewIsEnabled, value); }
        }


        private bool buttonNextIsEnabled = true;
        /// <summary>
        /// ��һ��ͼƬ��ť�Ƿ�����
        /// </summary>
        public bool ButtonNextIsEnabled
        {
            get { return buttonNextIsEnabled; }
            set { Set(ref buttonNextIsEnabled, value); }
        }

        private double frame = 30;
        /// <summary>
        /// ������Ƶʱ֡���ʱ��
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
        /// �Ƿ����ûҶ�ֵ����
        /// </summary>
        public bool ByColorEnable
        {
            get { return byColorEnable; }
            set { Set(ref byColorEnable, value); }

        }

        private bool byCircularityEnable;
        /// <summary>
        /// �Ƿ�����Բ�ȹ���
        /// </summary>
        public bool ByCircularityEnable
        {
            get { return byCircularityEnable; }
            set { Set(ref byCircularityEnable, value); }
        }

        private bool byAreaEnable;
        /// <summary>
        /// �Ƿ������������
        /// </summary>
        public bool ByAreaEnable
        {
            get { return byAreaEnable; }
            set { Set(ref byAreaEnable, value); }
        }

        private bool byConvexityEnable;
        /// <summary>
        /// �Ƿ�����͹�Թ���
        /// </summary>
        public bool ByConvexityEnable
        {
            get { return byConvexityEnable; }
            set { Set(ref byConvexityEnable, value); }
        }

        private bool byInertiaEnable;
        /// <summary>
        /// �Ƿ����ù��Աȹ���
        /// </summary>
        public bool ByInertiaEnable
        {
            get { return byInertiaEnable; }
            set { Set(ref byInertiaEnable, value); }
        }

        private float thresholdStep = 10f;
        /// <summary>
        /// ��ֵ�ݽ�ֵ
        /// </summary>
        public float ThresholdStep
        {
            get { return thresholdStep; }
            set { Set(ref thresholdStep, value); }
        }


        private float thresholdMinDis = 10f;
        /// <summary>
        /// ��ֵ֮�����С����
        /// </summary>
        public float ThresholdMinDis
        {
            get { return thresholdMinDis; }
            set { Set(ref thresholdMinDis, value); }
        }

        private uint thresholdTimes = 2;
        /// <summary>
        /// ��ֵ֮���ظ�����
        /// </summary>
        public uint ThresholdTimes
        {
            get { return thresholdTimes; }
            set { Set(ref thresholdTimes, value); }
        }

        private float thresholdMin = 10f;
        /// <summary>
        /// ��ֵ��Сֵ
        /// </summary>
        public float ThresholdMin
        {
            get { return thresholdMin; }
            set { Set(ref thresholdMin, value); }
        }

        private float thresholdMax = 100f;
        /// <summary>
        /// ��ֵ��Сֵ
        /// </summary>
        public float ThresholdMax
        {
            get { return thresholdMax; }
            set { Set(ref thresholdMax, value); }
        }

        private byte byColorValue;
        /// <summary>
        /// ������ɫ����ʱ��ֵ
        /// </summary>
        public byte BycolorValue
        {
            get { return byColorValue; }
            set { Set(ref byColorValue, value); }
        }

        private float byCircularityMinValue;
        /// <summary>
        /// Բ����Сֵ
        /// </summary>
        public float ByCircularityMinValue
        {
            get { return byCircularityMinValue; }
            set { Set(ref byCircularityMinValue, value); }
        }

        private float byCircularityMaxValue;
        /// <summary>
        /// Բ�����ֵ
        /// </summary>
        public float ByCircularityMaxValue
        {
            get { return byCircularityMaxValue; }
            set { Set(ref byCircularityMaxValue, value); }
        }

        private float byAreaMinValue;
        /// <summary>
        /// �����Сֵ
        /// </summary>
        public float ByAreaMinValue
        {
            get { return byAreaMinValue; }
            set { Set(ref byAreaMinValue, value); }
        }

        private float byAreaMaxValue;
        /// <summary>
        /// ������ֵ
        /// </summary>
        public float ByAreaMaxValue
        {
            get { return byAreaMaxValue; }
            set { Set(ref byAreaMaxValue, value); }
        }

        private float byConvexityMinValue;
        /// <summary>
        /// ͹����Сֵ
        /// </summary>
        public float ByConvexityMinValue
        {
            get { return byConvexityMinValue; }
            set { Set(ref byConvexityMinValue, value); }
        }

        private float byConvexityMaxValue;
        /// <summary>
        /// ͹�����ֵ
        /// </summary>
        public float ByConvexityMaxValue
        {
            get { return byConvexityMaxValue; }
            set { Set(ref byConvexityMaxValue, value); }
        }

        private float byInertiaMinValue;
        /// <summary>
        /// ���Ա���Сֵ
        /// </summary>
        public float ByInertiaMinValue
        {
            get { return byInertiaMinValue; }
            set { Set(ref byInertiaMinValue, value); }
        }

        private float byInertiaMaxValue;
        /// <summary>
        /// ���Ա����ֵ
        /// </summary>
        public float ByInertiaMaxValue
        {
            get { return byInertiaMaxValue; }
            set { Set(ref byInertiaMaxValue, value); }
        }

        private double thresholdValue = 100;
        /// <summary>
        /// ��ֵ����ֵ
        /// </summary>
        public double ThresholdValue
        {
            get { return thresholdValue; }
            set { thresholdValue = value; }
        }

        private bool thresholdEnable;
        /// <summary>
        /// �Ƿ����ö�ֵ��
        /// </summary>
        public bool ThresholdEnable
        {
            get { return thresholdEnable; }
            set { Set(ref thresholdEnable, value); }
        }

        private bool autoThresholdEnable;
        /// <summary>
        /// �Ƿ������Զ���ֵ
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

        #region �󶨷���

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

            MessageBox.Show("�ļ���ȡʧ��");
        }

        private void ButtonStartPlayClick(object sender)
        {
            bool isGray = RadioButtonGrayImageIsChecked == true;

            if (_index == 0)
            {
                MessageBox.Show("û�п��õ�֡");
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
            MessageBox.Show("�������");
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

            // ��SimpleBlobDetector������ֵ
            // 1�����ݽ���ѡ���Ƿ�����ĳЩ����
            pParams.FilterByArea = ByAreaEnable;
            pParams.FilterByCircularity = ByCircularityEnable;
            pParams.FilterByColor = ByColorEnable;
            pParams.FilterByConvexity = ByConvexityEnable;
            pParams.FilterByInertia = ByInertiaEnable;

            // 2��������Ҫ�Ĳ����ȸ�ֵ
            pParams.ThresholdStep = ThresholdStep;
            pParams.MinThreshold = ThresholdMin;
            pParams.MaxThreshold = ThresholdMax;
            pParams.MinRepeatability = ThresholdTimes;
            pParams.MinDistBetweenBlobs = ThresholdMinDis;

            // 3������������������ֵ���Խ���
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
                MessageBox.Show("��δ�ɼ�ͼƬ�����Ȳɼ�ͼƬ");
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

        #region ��������

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
                MessageBox.Show("�ļ�����ȷ");
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
                MessageBox.Show("�ļ�����û���ļ�");
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
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//����
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