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
        public FileInfo[] FileInfos { get; set; }
        public int FileIndex { get; set; }

        private HObject _displayImage;

        public HObject DisplayImage
        {
            get { return _displayImage; }
            set { _displayImage = value; RaisePropertyChanged(); }
        }

        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand<string> CaptureTypeChangeCommand { get; }
        public ImageAcquisitionToolViewModel()
        {
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            CaptureTypeChangeCommand = new DelegateCommand<string>(CaptureTypeChange);
        }

        private void CaptureTypeChange(string obj)
        {
            switch (obj)
            {
                case "file":
                    CaptureType = AcquisitionType.File;
                    GetFilePath(true);
                    break;
                case "dir":
                    CaptureType = AcquisitionType.Dir;
                    GetFilePath(false);
                    break;
                case "camera":
                    CaptureType = AcquisitionType.Camera;
                    break;
            }
        }

        public void Run(ToolBase tool, ref bool result)
        {
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
                    HObjectParams tempHobject = new HObjectParams { Image = DisplayImage, VisionStep = tool.ToolInVision, ImageKey = $"{tool.ToolInVision}_{tool.ToolItem.Header}" };
                    _eventAggregator.GetEvent<HObjectEvent>().Publish(tempHobject);
                    break;
                case AcquisitionType.Dir:
                    if (string.IsNullOrWhiteSpace(CapturePath))
                    {
                        MessageBox.Show("请先配置采集源");
                        result = true;
                        return;
                    }
                    if (FileInfos == null)
                    {
                        FileInfos = new DirectoryInfo(CapturePath).GetFiles();
                    }

                    if (FileIndex == FileInfos.Length)
                    {
                        FileIndex = 0;
                    }
                    DisplayImage = HOperatorSetExtension.ReadImage(FileInfos[FileIndex++].FullName);
                    if (DisplayImage == null)
                    {
                        result = true;
                        return;
                    }
                    HObjectParams tempHobjectDir = new HObjectParams { Image = DisplayImage, VisionStep = tool.ToolInVision, ImageKey = $"{tool.ToolInVision}_{tool.ToolItem.Header}"  };
                    _eventAggregator.GetEvent<HObjectEvent>().Publish(tempHobjectDir);
                    break;
                case AcquisitionType.Camera:
                    break;
            }
            TreeViewItem temp = new TreeViewItem { Header = nameof(DisplayImage), ToolTip = DisplayImage.ToString() };
            tool.AddInputOutputTree(temp, false);
            result = true;
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
