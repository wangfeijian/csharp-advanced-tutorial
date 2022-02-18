using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using DryIoc;
using HalconDotNet;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using SosoVision.Common;
using SosoVision.Extensions;

namespace SosoVision.ViewModels
{
    public class VisionProcessViewModel : BindableBase
    {
        private readonly ISosoLogManager _sosoLogManager;
        private ProcedureParam _procedureParam;

        [Newtonsoft.Json.JsonIgnore]
        private bool _isFirstShow = true;//第一次显示
        public ProcedureParam ProcedureParam
        {
            get { return _procedureParam; }
            set { _procedureParam = value; RaisePropertyChanged(); }
        }
        private HObject _displayImage;

        public HObject DisplayImage
        {
            get { return _displayImage; }
            set { _displayImage = value; RaisePropertyChanged(); }
        }

        private HObject _displayRegion;

        public HObject DisplayRegion
        {
            get { return _displayRegion; }
            set { _displayRegion = value; RaisePropertyChanged(); }
        }

        private string _fillStyle;

        public string FillStyle
        {
            get { return _fillStyle; }
            set { _fillStyle = value; RaisePropertyChanged(); }
        }

        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand LoadImageCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand LoadedCommand { get; }

        /// <summary>
        /// 此构造函数为反序列化时使用
        /// </summary>
        public VisionProcessViewModel()
        {
            LoadImageCommand = new DelegateCommand(LoadImage);
            LoadedCommand = new DelegateCommand(LoadWindow);
            _sosoLogManager = ContainerLocator.Container.Resolve<ISosoLogManager>();
        }

        private void LoadWindow()
        {
            if (_isFirstShow)
            {
                FillStyle = FillStyle == "fill" ? "margin" : "fill";

                _isFirstShow = false;
            }
        }

        private void LoadImage()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                DefaultExt = "jpg",
                Filter = "jpg files(*.jpg)|*.jpg|tiff files(*.tiff)|*.tiff|bmp files(*.bmp)|*.bmp"
            };

            if (!ofd.ShowDialog() == true)
                return;

            try
            {
                HObject tempObject;
                HOperatorSet.ReadImage(out tempObject, ofd.FileName);
                DisplayImage = tempObject;
                _sosoLogManager.ShowLogInfo($"加载图片 {ofd.FileName} 成功");
            }
            catch (Exception e)
            {
                _sosoLogManager.ShowLogError($"加载图片 {ofd.FileName} 失败");

            }
        }

        public VisionProcessViewModel(string title)
        {
            var config = ContainerLocator.Container.Resolve<IConfigureService>();
            foreach (var param in config.SerializationData.ProcedureParams)
            {
                if (param.Name == title)
                {
                    ProcedureParam = param;
                }
            }

            _sosoLogManager = ContainerLocator.Container.Resolve<ISosoLogManager>();
            LoadImageCommand = new DelegateCommand(LoadImage);
            LoadedCommand = new DelegateCommand(LoadWindow);
        }
    }
}
