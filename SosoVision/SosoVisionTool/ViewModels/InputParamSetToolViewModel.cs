using ImageCapture;
using Prism.Events;
using Prism.Mvvm;
using SosoVisionCommonTool.ConfigData;
using SosoVisionTool.Services;
using SosoVisionTool.Views;
using Prism.Ioc;
using System.Collections.ObjectModel;
using Prism.Commands;
using System;
using System.Linq;
using System.Windows;

namespace SosoVisionTool.ViewModels
{
    public class InputParamSetToolViewModel : BindableBase, IToolBaseViewModel
    {
        private IEventAggregator _eventAggregator;
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand DeleteParamCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand<string> AddParamCommand { get; }
        public InputParamSetToolViewModel()
        {
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            DeleteParamCommand = new DelegateCommand(DeleteParam);
            AddParamCommand = new DelegateCommand<string>(AddParam);
        }


        public InputParamSetToolViewModel(string visionStep)
        {
            InputDataKeyAndType = new ObservableCollection<DataKeyAndType>();
            VisionStep = visionStep;
            ToolRunData = ContainerLocator.Container.Resolve<ToolRunViewData>(VisionStep);
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            DeleteParamCommand = new DelegateCommand(DeleteParam);
            AddParamCommand = new DelegateCommand<string>(AddParam);
        }
        [Newtonsoft.Json.JsonIgnore]
        public ToolRunViewData ToolRunData { get; set; }
        public string VisionStep { get; set; }
        public ProcedureParam Param { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public CaptureBase Capture { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public AllVisionRunData VisionRunData { get; set; }

        private string _addImageDataKey;

        public string AddImageDataKey
        {
            get { return _addImageDataKey; }
            set { _addImageDataKey = value; RaisePropertyChanged(); }
        }

        private string _addRegionDataKey;

        public string AddRegionDataKey
        {
            get { return _addRegionDataKey; }
            set { _addRegionDataKey = value; RaisePropertyChanged(); }
        }

        private string _addDoubleDataKey;

        public string AddDoubleDataKey
        {
            get { return _addDoubleDataKey; }
            set { _addDoubleDataKey = value; RaisePropertyChanged(); }
        }

        private string _addIntDataKey;

        public string AddIntDataKey
        {
            get { return _addIntDataKey; }
            set { _addIntDataKey = value; RaisePropertyChanged(); }
        }

        private string _addStringDataKey;

        public string AddStringDataKey
        {
            get { return _addStringDataKey; }
            set { _addStringDataKey = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<DataKeyAndType> _inputDataKeyAndType;

        public ObservableCollection<DataKeyAndType> InputDataKeyAndType
        {
            get { return _inputDataKeyAndType; }
            set { _inputDataKeyAndType = value; RaisePropertyChanged(); }
        }

        private DataKeyAndType _inputDataKeyAndTypeSelected;

        public DataKeyAndType InputDataKeyAndTypeSelected
        {
            get { return _inputDataKeyAndTypeSelected; }
            set { _inputDataKeyAndTypeSelected = value; RaisePropertyChanged(); }
        }

        public void Run(ToolBase tool, ref bool result)
        {
            VisionRunData = ContainerLocator.Container.Resolve<AllVisionRunData>("GlobalData");

            if (InputDataKeyAndType.Count <= 0)
            {
                result = false;
                return;
            }

            foreach (var item in InputDataKeyAndType)
            {
                switch (item.DataType)
                {
                    case "Image":
                        if (!VisionRunData.VisionRunImage.ContainsKey(item.DataKey))
                        {
                            result = false;
                            return;
                        }

                        if (ToolRunData.ToolOutputImage.ContainsKey(item.DataKey))
                        {
                            ToolRunData.ToolOutputImage[item.DataKey] = VisionRunData.VisionRunImage[item.DataKey];
                        }
                        else
                        {
                            ToolRunData.ToolOutputImage.Add(item.DataKey, VisionRunData.VisionRunImage[item.DataKey]);
                        }
                        break;
                    case "Region":
                        if (!VisionRunData.VisionRunRegion.ContainsKey(item.DataKey))
                        {
                            result = false;
                            return;
                        }

                        if (ToolRunData.ToolOutputRegion.ContainsKey(item.DataKey))
                        {
                            ToolRunData.ToolOutputRegion[item.DataKey] = VisionRunData.VisionRunRegion[item.DataKey];
                        }
                        else
                        {
                            ToolRunData.ToolOutputRegion.Add(item.DataKey, VisionRunData.VisionRunRegion[item.DataKey]);
                        }
                        break;
                    case "Double":
                        if (!VisionRunData.VisionRunDoubleValue.ContainsKey(item.DataKey))
                        {
                            result = false;
                            return;
                        }

                        if (ToolRunData.ToolOutputDoubleValue.ContainsKey(item.DataKey))
                        {
                            ToolRunData.ToolOutputDoubleValue[item.DataKey] = VisionRunData.VisionRunDoubleValue[item.DataKey];
                        }
                        else
                        {
                            ToolRunData.ToolOutputDoubleValue.Add(item.DataKey, VisionRunData.VisionRunDoubleValue[item.DataKey]);
                        }
                        break;
                    case "Int":
                        if (!VisionRunData.VisionRunIntValue.ContainsKey(item.DataKey))
                        {
                            result = false;
                            return;
                        }

                        if (ToolRunData.ToolOutputIntValue.ContainsKey(item.DataKey))
                        {
                            ToolRunData.ToolOutputIntValue[item.DataKey] = VisionRunData.VisionRunIntValue[item.DataKey];
                        }
                        else
                        {
                            ToolRunData.ToolOutputIntValue.Add(item.DataKey, VisionRunData.VisionRunIntValue[item.DataKey]);
                        }
                        break;
                    case "String":
                        if (!VisionRunData.VisionRunStringValue.ContainsKey(item.DataKey))
                        {
                            result = false;
                            return;
                        }

                        if (ToolRunData.ToolOutputStringValue.ContainsKey(item.DataKey))
                        {
                            ToolRunData.ToolOutputStringValue[item.DataKey] = VisionRunData.VisionRunStringValue[item.DataKey];
                        }
                        else
                        {
                            ToolRunData.ToolOutputStringValue.Add(item.DataKey, VisionRunData.VisionRunStringValue[item.DataKey]);
                        }
                        break;
                }
            }
            result = true;
        }
        private void DeleteParam()
        {
            if (InputDataKeyAndTypeSelected != null)
            {
                InputDataKeyAndType.Remove(InputDataKeyAndTypeSelected);
            }
        }

        private void AddParam(string param)
        {
            switch (param)
            {
                case "ImageParam":
                    if (string.IsNullOrWhiteSpace(AddImageDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addImageDataKey = from key in InputDataKeyAndType
                                          where key.DataKey == AddImageDataKey
                                          select key;
                    if (addImageDataKey != null)
                    {
                        MessageBox.Show($"输入参数中已经存在{addImageDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    InputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddImageDataKey, DataType = "Image" });
                    break;
                case "RegionParam":
                    if (string.IsNullOrWhiteSpace(AddRegionDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addRegionDataKey = from key in InputDataKeyAndType
                                           where key.DataKey == AddRegionDataKey
                                           select key;
                    if (addRegionDataKey != null)
                    {
                        MessageBox.Show($"输入参数中已经存在{addRegionDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    InputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddRegionDataKey, DataType = "Region" });
                    break;
                case "DoubleParam":
                    if (string.IsNullOrWhiteSpace(AddDoubleDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addDoubleDataKey = from key in InputDataKeyAndType
                                           where key.DataKey == AddDoubleDataKey
                                           select key;
                    if (addDoubleDataKey != null)
                    {
                        MessageBox.Show($"输入参数中已经存在{addDoubleDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    InputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddDoubleDataKey, DataType = "Double" });
                    break;
                case "IntParam":
                    if (string.IsNullOrWhiteSpace(AddIntDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addIntDataKey = from key in InputDataKeyAndType
                                        where key.DataKey == AddIntDataKey
                                        select key;
                    if (addIntDataKey != null)
                    {
                        MessageBox.Show($"输入参数中已经存在{addIntDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    InputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddIntDataKey, DataType = "Int" });
                    break;
                case "StringParam":
                    if (string.IsNullOrWhiteSpace(AddStringDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addStringDataKey = from key in InputDataKeyAndType
                                           where key.DataKey == AddStringDataKey
                                           select key;
                    if (addStringDataKey != null)
                    {
                        MessageBox.Show($"输入参数中已经存在{addStringDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    InputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddStringDataKey, DataType = "String" });
                    break;
            }
        }
    }
}
