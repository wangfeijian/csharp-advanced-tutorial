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
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using SosoVision.Common;
using SosoVision.Extensions;
using SosoVisionCommonTool.ConfigData;
using SosoVisionCommonTool.Log;
using SosoVisionTool.Tools;

namespace SosoVision.ViewModels
{
    public class VisionProcessViewModel : BindableBase
    {
        private readonly ISosoLogManager _sosoLogManager;
        private readonly IEventAggregator _eventAggregator;
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

        private Dictionary<string, HObject> _toolRunImage;

        public Dictionary<string, HObject> ToolRunImage
        {
            get { return _toolRunImage; }
            set { _toolRunImage = value; RaisePropertyChanged(); }
        }

        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand LoadedCommand { get; }

        /// <summary>
        /// 此构造函数为反序列化时使用
        /// </summary>
        public VisionProcessViewModel()
        {
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            LoadedCommand = new DelegateCommand(LoadWindow);
            _sosoLogManager = ContainerLocator.Container.Resolve<ISosoLogManager>();

            _eventAggregator.GetEvent<HObjectEvent>().Subscribe((obj) =>
            {
                DisplayImage = obj.Image;
                if (ToolRunImage.ContainsKey(obj.ImageKey))
                {
                    ToolRunImage[obj.ImageKey] = obj.Image;
                }
                else
                    ToolRunImage.Add(obj.ImageKey, obj.Image);
            }, ThreadOption.UIThread, true,
                              companySymbol => companySymbol.VisionStep == ProcedureParam.Name);
        }

        private void LoadWindow()
        {
            if (_isFirstShow)
            {
                FillStyle = FillStyle == "fill" ? "margin" : "fill";

                _isFirstShow = false;
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
            ToolRunImage = new Dictionary<string, HObject>();
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            _sosoLogManager = ContainerLocator.Container.Resolve<ISosoLogManager>();
            LoadedCommand = new DelegateCommand(LoadWindow);

            _eventAggregator.GetEvent<HObjectEvent>().Subscribe((obj) =>
           {
               DisplayImage = obj.Image;
               if (ToolRunImage.ContainsKey(obj.ImageKey))
               {
                   ToolRunImage[obj.ImageKey] = obj.Image;
               }
               else
                   ToolRunImage.Add(obj.ImageKey, obj.Image);
           }, ThreadOption.UIThread, true,
                             companySymbol => companySymbol.VisionStep == ProcedureParam.Name);

        }
    }
}
