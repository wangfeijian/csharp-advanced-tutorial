using System;
using System.Collections.Generic;
using Prism.Mvvm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SosoVisionTool.Views;
using System.Windows.Controls;
using Prism.Commands;
using Microsoft.Win32;
using HalconDotNet;
using Prism.Ioc;
using System.Windows;
using SosoVisionTool.Tools;
using System.IO;
using Prism.Events;
using SosoVisionTool.Services;
using ImageCapture;
using System.Drawing;
using SosoVisionCommonTool.ConfigData;

namespace SosoVisionTool.ViewModels
{
    public enum AcquisitionType
    {
        File,
        Dir,
        Camera
    }

    public class ImageAcquisitionToolViewModel : BindableBase, IToolBaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        public AcquisitionType CaptureType { get; set; }
        public string CapturePath { get; set; }
        public string VisionStep { get; set; }
        public string CameraId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public CaptureBase Capture { get; set; }
        public ProcedureParam Param { get; set; }

        public bool IsContinuousCapture { get; set; }
        public bool AtCapture { get; set; }
        private bool _radioButtonCameraChecked;
        public bool RadioButtonCameraChecked
        {
            get { return _radioButtonCameraChecked; }
            set { _radioButtonCameraChecked = value; RaisePropertyChanged(); }
        }
        private bool _radioButtonDirChecked;
        public bool RadioButtonDirChecked
        {
            get { return _radioButtonDirChecked; }
            set { _radioButtonDirChecked = value; RaisePropertyChanged(); }
        }
        private bool _radioButtonFileChecked;
        public bool RadioButtonFileChecked
        {
            get { return _radioButtonFileChecked; }
            set { _radioButtonFileChecked = value; RaisePropertyChanged(); }
        }

        [Newtonsoft.Json.JsonIgnore]
        public ToolRunViewData ToolRunData { get; set; }
        public FileInfo[] FileInfos { get; set; }
        public int FileIndex { get; set; }

        private HObject _displayImage;

        public HObject DisplayImage
        {
            get { return _displayImage; }
            set { _displayImage = value; RaisePropertyChanged(); }
        }

        private bool buttonPreviewIsEnabled = true;
        /// <summary>
        /// 上一张图片按钮是否启用
        /// </summary>
        public bool ButtonPreviewIsEnabled
        {
            get { return buttonPreviewIsEnabled; }
            set { buttonPreviewIsEnabled = value; RaisePropertyChanged(); }
        }


        private bool buttonNextIsEnabled = true;
        /// <summary>
        /// 下一张图片按钮是否启用
        /// </summary>
        public bool ButtonNextIsEnabled
        {
            get { return buttonNextIsEnabled; }
            set { buttonNextIsEnabled = value; RaisePropertyChanged(); }
        }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand<string> CaptureTypeChangeCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand TestCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand PreviewCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand NextCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand ApplyParamCommand { get; }
        public ImageAcquisitionToolViewModel()
        {
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            CaptureTypeChangeCommand = new DelegateCommand<string>(CaptureTypeChange);
            TestCommand = new DelegateCommand(Test);
            PreviewCommand = new DelegateCommand(ButtonPreviewClick);
            NextCommand = new DelegateCommand(ButtonNextClick);
            ApplyParamCommand = new DelegateCommand(ApplyParam);
        }

        public ImageAcquisitionToolViewModel(string visionStep, string cameraId)
        {
            VisionStep = visionStep;
            CameraId = cameraId;
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            CaptureTypeChangeCommand = new DelegateCommand<string>(CaptureTypeChange);
            TestCommand = new DelegateCommand(Test);
            PreviewCommand = new DelegateCommand(ButtonPreviewClick);
            NextCommand = new DelegateCommand(ButtonNextClick);
            ApplyParamCommand = new DelegateCommand(ApplyParam);
            ToolRunData = ContainerLocator.Container.Resolve<ToolRunViewData>(VisionStep);
            Capture = ContainerLocator.Container.Resolve<CaptureBase>(CameraId);
        }

        private void CaptureTypeChange(string obj)
        {
            switch (obj)
            {
                case "file":
                    RadioButtonFileChecked = true;
                    RadioButtonCameraChecked = false;
                    RadioButtonDirChecked = false;
                    CaptureType = AcquisitionType.File;
                    GetFilePath(true);
                    break;
                case "dir":
                    RadioButtonFileChecked = false;
                    RadioButtonCameraChecked = false;
                    RadioButtonDirChecked = true;
                    CaptureType = AcquisitionType.Dir;
                    GetFilePath(false);
                    break;
                case "camera":
                    RadioButtonFileChecked = false;
                    RadioButtonCameraChecked = true;
                    RadioButtonDirChecked = false;
                    CaptureType = AcquisitionType.Camera;
                    break;
                case "single":
                    IsContinuousCapture = false;
                    break;
                case "continuous":
                    IsContinuousCapture = true;
                    break;
                case "stop":
                    AtCapture = false;
                    break;
            }
        }
        private void Test()
        {
            switch (CaptureType)
            {
                case AcquisitionType.File:
                    if (string.IsNullOrWhiteSpace(CapturePath))
                    {
                        MessageBox.Show("请先配置采集源");
                        return;
                    }
                    DisplayImage = HOperatorSetExtension.ReadImage(CapturePath);
                    break;
                case AcquisitionType.Dir:
                    if (string.IsNullOrWhiteSpace(CapturePath))
                    {
                        MessageBox.Show("请先配置采集源");
                        return;
                    }
                    FileInfos = new DirectoryInfo(CapturePath).GetFiles();

                    if (FileIndex >= FileInfos.Length)
                    {
                        FileIndex = 0;
                    }
                    DisplayImage = HOperatorSetExtension.ReadImage(FileInfos[FileIndex].FullName);
                    break;
                case AcquisitionType.Camera:
                    if (!AtCapture)
                    {
                        GrabImage();
                    }
                    break;
            }
        }

        private void ApplyParam()
        {
            Capture.SetBrightness(Param.Brightness);
            Capture.SetConstract(Param.Contrast);
            Capture.SetExposure(Param.ExposureTime);
        }

        private void GrabImage(bool isTest = true)
        {
            if (IsContinuousCapture && isTest)
            {
                AtCapture = true;
                while (AtCapture)
                {
                    DispatcherHelper.Delay(1);
                    Capture.Grab();
                    DisplayImage = Capture.GetImage(true);
                }
                return;
            }

            Capture.Grab();
            DisplayImage = Capture.GetImage(true);
        }

        private void ButtonPreviewClick()
        {
            ButtonNextIsEnabled = true;
            if (FileIndex == 0)
            {
                ButtonPreviewIsEnabled = false;
                Test();
                return;
            }

            FileIndex--;
            Test();
        }

        private void ButtonNextClick()
        {
            ButtonPreviewIsEnabled = true;
            if (FileIndex == FileInfos.Length - 1)
            {
                ButtonNextIsEnabled = false;
                Test();
                return;
            }
            FileIndex++;
            Test();
        }

        public void Run(ToolBase tool, ref bool result)
        {
            string key = $"{tool.ToolInVision}_{tool.ToolItem.Header}";
            switch (CaptureType)
            {
                case AcquisitionType.File:
                    if (string.IsNullOrWhiteSpace(CapturePath))
                    {
                        MessageBox.Show("请先配置采集源");
                        result = true;
                        return;
                    }
                    DisplayImage = HOperatorSetExtension.ReadImage(CapturePath);
                    AddImageToData(key, DisplayImage);
                    HObjectParams tempHobject = new HObjectParams { Image = DisplayImage, VisionStep = tool.ToolInVision, ImageKey = key };
                    _eventAggregator.GetEvent<HObjectEvent>().Publish(tempHobject);
                    break;
                case AcquisitionType.Dir:
                    if (string.IsNullOrWhiteSpace(CapturePath))
                    {
                        MessageBox.Show("请先配置采集源");
                        result = true;
                        return;
                    }
                    FileInfos = new DirectoryInfo(CapturePath).GetFiles();

                    if (FileIndex >= FileInfos.Length)
                    {
                        FileIndex = 0;
                    }
                    DisplayImage = HOperatorSetExtension.ReadImage(FileInfos[FileIndex++].FullName);
                    if (DisplayImage == null)
                    {
                        result = true;
                        return;
                    }
                    AddImageToData(key, DisplayImage);
                    HObjectParams tempHobjectDir = new HObjectParams { Image = DisplayImage, VisionStep = tool.ToolInVision, ImageKey = key };
                    _eventAggregator.GetEvent<HObjectEvent>().Publish(tempHobjectDir);
                    break;
                case AcquisitionType.Camera:
                    GrabImage(false);
                    AddImageToData(key, DisplayImage);
                    HObjectParams tempHobjectCamera = new HObjectParams { Image = DisplayImage, VisionStep = tool.ToolInVision, ImageKey = key };
                    _eventAggregator.GetEvent<HObjectEvent>().Publish(tempHobjectCamera);
                    break;
            }
            TreeViewItem temp = new TreeViewItem { Header = nameof(DisplayImage), ToolTip = DisplayImage.ToString() };
            tool.AddInputOutputTree(temp, false);
            result = true;
        }

        private void AddImageToData(string key, HObject image)
        {
            if (ToolRunData.ToolOutputImage.ContainsKey(key))
            {
                ToolRunData.ToolOutputImage[key] = image;
                return;
            }

            ToolRunData.ToolOutputImage.Add(key, DisplayImage);
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
                CapturePath = flag ? fileName : dirName;
                return;
            }

            CapturePath = "";
        }
    }
}
