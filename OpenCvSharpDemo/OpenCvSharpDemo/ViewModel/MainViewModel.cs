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
        private string[] _filePaths;
        private bool _isStop = false;
        private int _index;
        public int FileNum => _filePaths.Length;

        #endregion

        #region ����

        public ICommand SelectImageSourceCommand { get; set; }
        public ICommand GrabImageCommand { get; set; }
        public ICommand PreviewImageCommand { get; set; }
        public ICommand NextImageCommand { get; set; }
        public ICommand LoadVideoCommand { get; set; }
        public ICommand StartPlayCommand { get; set; }
        public ICommand StopPlayCommand { get; set; }

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
            set { Set(ref selectImageStackPanelEnable ,value); }
        }

        private bool buttonStartPlayEnable;
        /// <summary>
        /// ��ʼ���Ű�ť�Ƿ�����
        /// </summary>
        public bool ButtonStartPlayEnable
        {
            get { return buttonStartPlayEnable; }
            set { Set(ref buttonStartPlayEnable ,value); }
        }

        private bool buttonStopPlayEnable;
        /// <summary>
        /// ֹͣ���Ű�ť�Ƿ�����
        /// </summary>
        public bool ButtonStopPlayEnable
        {
            get { return buttonStopPlayEnable; }
            set { Set(ref buttonStopPlayEnable , value); }
        }

        private WriteableBitmap showBitmap;

        /// <summary>
        /// ��ʾ��ͼƬԴ
        /// </summary>
        public WriteableBitmap ShowBitmap
        {
            get { return showBitmap; }
            set { Set(ref showBitmap , value); }
        }

        private string pathTextBoxText;
        /// <summary>
        /// �ļ�·����ʾtextbox��text
        /// </summary>
        public string PathTextBoxText
        {
            get { return pathTextBoxText; }
            set { Set(ref pathTextBoxText , value); }
        }

        private bool radioButtonFileIsChecked;
        /// <summary>
        /// ͼƬ��Դ�Ƿ�Ϊ�ļ�
        /// </summary>
        public bool RadioButtonFileIsChecked
        {
            get { return radioButtonFileIsChecked; }
            set { Set(ref radioButtonFileIsChecked , value); }
        }

        private bool radioButtonDirIsChecked;
        /// <summary>
        /// ͼƬ��Դ�Ƿ�Ϊ�ļ���
        /// </summary>
        public bool RadioButtonDirIsChecked
        {
            get { return radioButtonDirIsChecked; }
            set { Set(ref radioButtonDirIsChecked , value); }
        }

        private bool radioButtonCameraIsChecked;
        /// <summary>
        /// ͼƬ��Դ�Ƿ�Ϊ���
        /// </summary>
        public bool RadioButtonCameraIsChecked
        {
            get { return radioButtonCameraIsChecked; }
            set { Set(ref radioButtonCameraIsChecked , value); }
        }

        private bool radioButtonGrayImageIsChecked = true;
        /// <summary>
        /// �Ƿ�ת��Ϊ�Ҷ�ͼƬ
        /// </summary>
        public bool RadioButtonGrayImageIsChecked
        {
            get { return radioButtonGrayImageIsChecked; }
            set { Set(ref radioButtonGrayImageIsChecked , value); }
        }

        private bool buttonPreviewIsEnabled=true;
        /// <summary>
        /// ��һ��ͼƬ��ť�Ƿ�����
        /// </summary>
        public bool ButtonPreviewIsEnabled
        {
            get { return buttonPreviewIsEnabled; }
            set { Set(ref buttonPreviewIsEnabled , value); }
        }


        private bool buttonNextIsEnabled=true;
        /// <summary>
        /// ��һ��ͼƬ��ť�Ƿ�����
        /// </summary>
        public bool ButtonNextIsEnabled
        {
            get { return buttonNextIsEnabled; }
            set { Set(ref buttonNextIsEnabled , value); }
        }

        private double frame=30;

        public double Frame
        {
            get { return frame; }
            set { Set(ref frame , value); }
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
            bool flag = false;
            ImreadModes imreadModes =
                RadioButtonGrayImageIsChecked == true ? ImreadModes.Grayscale : ImreadModes.Color;
            ShowBitmap = _imageGrab.GetImageFromFile(fileName, imreadModes, ref flag);

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