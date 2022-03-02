using ImageCapture;
using Prism.Events;
using Prism.Mvvm;
using SosoVisionCommonTool.ConfigData;
using SosoVisionTool.Services;
using SosoVisionTool.Views;
using Prism.Ioc;
using Prism.Commands;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using SosoVisionTool.Tools;

namespace SosoVisionTool.ViewModels
{
    public class OutputParamSetToolViewModel : BindableBase, IToolBaseViewModel
    {
        private IEventAggregator _eventAggregator;

        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand DeleteParamCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand<string> AddParamCommand { get; }

        public OutputParamSetToolViewModel()
        {
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            DeleteParamCommand = new DelegateCommand(DeleteParam);
            AddParamCommand = new DelegateCommand<string>(AddParam);
        }
        public OutputParamSetToolViewModel(string visionStep)
        {
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

        private ObservableCollection<DataKeyAndType> _outputDataKeyAndType;

        public ObservableCollection<DataKeyAndType> OutputDataKeyAndType
        {
            get { return _outputDataKeyAndType; }
            set { _outputDataKeyAndType = value; RaisePropertyChanged(); }
        }

        private DataKeyAndType _outputDataKeyAndTypeSelected;

        public DataKeyAndType OutputDataKeyAndTypeSelected
        {
            get { return _outputDataKeyAndTypeSelected; }
            set { _outputDataKeyAndTypeSelected = value; RaisePropertyChanged(); }
        }

        public void Run(ToolBase tool, ref bool result)
        {
            VisionRunData = ContainerLocator.Container.Resolve<AllVisionRunData>("GlobalData");

            if (OutputDataKeyAndType.Count <= 0)
            {
                result = false;
                return;
            }

            string tempResult = string.Empty;
            string tempMessage = string.Empty;

            foreach (var item in OutputDataKeyAndType)
            {
                string tempKey = item.DataKey.Substring(item.DataKey.LastIndexOf('_') + 1, item.DataKey.Length - item.DataKey.LastIndexOf('_') - 1);
               

                switch (item.DataType)
                {
                    case "Image":
                        if (!ToolRunData.ToolOutputImage.ContainsKey(item.DataKey))
                        {
                            result = false;
                            return;
                        }

                        if (VisionRunData.VisionRunImage.ContainsKey(item.DataKey))
                        {
                            VisionRunData.VisionRunImage[item.DataKey] = ToolRunData.ToolOutputImage[item.DataKey];
                        }
                        else
                        {
                            VisionRunData.VisionRunImage.Add(item.DataKey, ToolRunData.ToolOutputImage[item.DataKey]);
                        }
                        break;
                    case "Region":
                        if (!ToolRunData.ToolOutputRegion.ContainsKey(item.DataKey))
                        {
                            result = false;
                            return;
                        }

                        if (VisionRunData.VisionRunRegion.ContainsKey(item.DataKey))
                        {
                            VisionRunData.VisionRunRegion[item.DataKey] = ToolRunData.ToolOutputRegion[item.DataKey];
                        }
                        else
                        {
                            VisionRunData.VisionRunRegion.Add(item.DataKey, ToolRunData.ToolOutputRegion[item.DataKey]);
                        }
                        break;
                    case "Double":
                        if (!ToolRunData.ToolOutputDoubleValue.ContainsKey(item.DataKey))
                        {
                            result = false;
                            return;
                        }

                        if (VisionRunData.VisionRunDoubleValue.ContainsKey(item.DataKey))
                        {
                            VisionRunData.VisionRunDoubleValue[item.DataKey] = ToolRunData.ToolOutputDoubleValue[item.DataKey];
                        }
                        else
                        {
                            VisionRunData.VisionRunDoubleValue.Add(item.DataKey, ToolRunData.ToolOutputDoubleValue[item.DataKey]);
                        }

                        if (string.IsNullOrWhiteSpace(tempResult))
                        {
                            tempMessage = tempKey;
                            tempResult = ToolRunData.ToolOutputDoubleValue[item.DataKey].ToString();
                        }
                        else
                        {
                            tempMessage = $"{tempMessage},{tempKey}";
                            tempResult = $"{tempResult},{ToolRunData.ToolOutputDoubleValue[item.DataKey]}";
                        }
                        break;
                    case "Int":
                        if (!ToolRunData.ToolOutputIntValue.ContainsKey(item.DataKey))
                        {
                            result = false;
                            return;
                        }

                        if (VisionRunData.VisionRunIntValue.ContainsKey(item.DataKey))
                        {
                            VisionRunData.VisionRunIntValue[item.DataKey] = ToolRunData.ToolOutputIntValue[item.DataKey];
                        }
                        else
                        {
                            VisionRunData.VisionRunIntValue.Add(item.DataKey, ToolRunData.ToolOutputIntValue[item.DataKey]);
                        }

                        if (string.IsNullOrWhiteSpace(tempResult))
                        {
                            tempMessage = tempKey;
                            tempResult = ToolRunData.ToolOutputIntValue[item.DataKey].ToString();
                        }
                        else
                        {
                            tempMessage = $"{tempMessage},{tempKey}";
                             tempResult = $"{tempResult},{ToolRunData.ToolOutputIntValue[item.DataKey]}";
                        }
                        break;
                    case "String":
                        if (!ToolRunData.ToolOutputStringValue.ContainsKey(item.DataKey))
                        {
                            result = false;
                            return;
                        }

                        if (VisionRunData.VisionRunStringValue.ContainsKey(item.DataKey))
                        {
                            VisionRunData.VisionRunStringValue[item.DataKey] = ToolRunData.ToolOutputStringValue[item.DataKey];
                        }
                        else
                        {
                            VisionRunData.VisionRunStringValue.Add(item.DataKey, ToolRunData.ToolOutputStringValue[item.DataKey]);
                        }

                        if (string.IsNullOrWhiteSpace(tempResult))
                        {
                            tempMessage = tempKey;
                            tempResult = ToolRunData.ToolOutputStringValue[item.DataKey].ToString();
                        }
                        else
                        {
                            tempMessage = $"{tempMessage},{tempKey}";
                            tempResult = $"{tempResult},{ToolRunData.ToolOutputStringValue[item.DataKey]}";
                        }
                        break;
                }
            }

            HObjectParams tempHobjectCamera = new HObjectParams { Result = tempResult ,ShowMessage = tempMessage};
            _eventAggregator.GetEvent<HObjectEvent>().Publish(tempHobjectCamera);

            result = true;
        }

        private void DeleteParam()
        {
            if (OutputDataKeyAndTypeSelected != null)
            {
                OutputDataKeyAndType.Remove(OutputDataKeyAndTypeSelected);
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

                    var addImageDataKey = from key in OutputDataKeyAndType
                                          where key.DataKey == AddImageDataKey
                                          select key;
                    if (addImageDataKey != null)
                    {
                        MessageBox.Show($"输入参数中已经存在{addImageDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    OutputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddImageDataKey, DataType = "Image" });
                    break;
                case "RegionParam":
                    if (string.IsNullOrWhiteSpace(AddRegionDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addRegionDataKey = from key in OutputDataKeyAndType
                                           where key.DataKey == AddRegionDataKey
                                           select key;
                    if (addRegionDataKey != null)
                    {
                        MessageBox.Show($"输入参数中已经存在{addRegionDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    OutputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddRegionDataKey, DataType = "Region" });
                    break;
                case "DoubleParam":
                    if (string.IsNullOrWhiteSpace(AddDoubleDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addDoubleDataKey = from key in OutputDataKeyAndType
                                           where key.DataKey == AddDoubleDataKey
                                           select key;
                    if (addDoubleDataKey != null)
                    {
                        MessageBox.Show($"输入参数中已经存在{addDoubleDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    OutputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddDoubleDataKey, DataType = "Double" });
                    break;
                case "IntParam":
                    if (string.IsNullOrWhiteSpace(AddIntDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addIntDataKey = from key in OutputDataKeyAndType
                                        where key.DataKey == AddIntDataKey
                                        select key;
                    if (addIntDataKey != null)
                    {
                        MessageBox.Show($"输入参数中已经存在{addIntDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    OutputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddIntDataKey, DataType = "Int" });
                    break;
                case "StringParam":
                    if (string.IsNullOrWhiteSpace(AddStringDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addStringDataKey = from key in OutputDataKeyAndType
                                           where key.DataKey == AddStringDataKey
                                           select key;
                    if (addStringDataKey != null)
                    {
                        MessageBox.Show($"输入参数中已经存在{addStringDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    OutputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddStringDataKey, DataType = "String" });
                    break;
            }
        }
    }
}
