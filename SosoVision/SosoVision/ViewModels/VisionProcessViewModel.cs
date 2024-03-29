﻿using System.Collections.Generic;
using DryIoc;
using HalconDotNet;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using SosoVision.Common;
using SosoVisionCommonTool.Authority;
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

        private Dictionary<string, HObject> _toolRunRegion;

        public Dictionary<string, HObject> ToolRunRegion
        {
            get { return _toolRunRegion; }
            set { _toolRunRegion = value; RaisePropertyChanged(); }
        }


        private string _displayMessage;

        public string DisplayMessage
        {
            get { return _displayMessage; }
            set { _displayMessage = value; RaisePropertyChanged(); }
        }

        private string _messageColor;

        public string MessageColor
        {
            get { return _messageColor; }
            set { _messageColor = value; RaisePropertyChanged(); }
        }

        private bool _isNotOpModel;

        [Newtonsoft.Json.JsonIgnore]
        public bool IsNotOpModel
        {
            get { return _isNotOpModel; }
            set { _isNotOpModel = value; RaisePropertyChanged(); }
        }

        public string ResultStr { get; set; }
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

            _eventAggregator.GetEvent<HObjectEvent>().Subscribe(SubscribeHObjectParam, ThreadOption.UIThread, true,
                              companySymbol => companySymbol.VisionStep == ProcedureParam.Name);

            AuthorityTool.ModeChangedEvent += ModeChanged;
            ModeChanged();
        }

        private void SubscribeHObjectParam(HObjectParams obj)
        {
            if (!string.IsNullOrWhiteSpace(obj.Result))
            {
                if (obj.Region!=null)
                {
                    DisplayRegion = obj.Region;
                }
                ResultStr = obj.Result;
                ShowVisionRunResult(obj.Result, obj.ShowMessage);
                return;
            }
            else
            {
                ResultStr = string.Empty;
                DisplayMessage = string.Empty;
            }

            DisplayImage = obj.Image;

            if (obj.Image == null)
                return;

            if (ToolRunImage.ContainsKey(obj.ImageKey))
            {
                ToolRunImage[obj.ImageKey] = obj.Image;
            }
            else
                ToolRunImage.Add(obj.ImageKey, obj.Image);

            DisplayRegion = obj.Region;

            if (obj.Region == null)
                return;

            if (ToolRunRegion.ContainsKey(obj.RegionKey))
            {
                ToolRunRegion[obj.RegionKey] = obj.Region;
            }
            else
                ToolRunRegion.Add(obj.RegionKey, obj.Region);

        }

        private void ShowVisionRunResult(string result, string message)
        {
            DisplayMessage = "";
            var results = result.Split(',');
            var messages = message.Split(',');
            string[] tempStr = new string[results.Length];

            if (results.Length != messages.Length || results.Length <= 0)
                return;

            for (int i = 0; i < results.Length; i++)
            {
                tempStr[i] = $"{messages[i]}: {results[i]}";
            }

            string temp = string.Join("\n", tempStr);
            if (result.Contains("999"))
            {
                DisplayRegion = null;
                MessageColor = "red";
                DisplayMessage = $"NG\n{temp}";
            }
            else
            {
                MessageColor = "green";
                DisplayMessage = $"OK\n{temp}";
            }
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
            ToolRunRegion = new Dictionary<string, HObject>();
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            _sosoLogManager = ContainerLocator.Container.Resolve<ISosoLogManager>();
            LoadedCommand = new DelegateCommand(LoadWindow);

            _eventAggregator.GetEvent<HObjectEvent>().Subscribe(SubscribeHObjectParam, ThreadOption.UIThread, true,
                             companySymbol => companySymbol.VisionStep == ProcedureParam.Name);

            AuthorityTool.ModeChangedEvent += ModeChanged;
            ModeChanged();
        }

        private void ModeChanged()
        {
            IsNotOpModel = AuthorityTool.GetUserMode() == UserMode.Operator ? false : true;
        }
    }
}
