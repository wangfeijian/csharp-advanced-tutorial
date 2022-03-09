using Prism.Mvvm;
using Prism.Ioc;
using SosoVisionTool.Views;
using System.Windows.Controls;
using SosoVisionTool.Services;
using ImageCapture;
using SosoVisionCommonTool.ConfigData;
using Prism.Events;
using HalconDotNet;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Prism.Commands;
using System.Windows;
using System.Linq;
using System;
using SosoVisionTool.Tools;

namespace SosoVisionTool.ViewModels
{
    public class ScriptToolViewModel : BindableBase, IToolBaseViewModel
    {
        [Newtonsoft.Json.JsonIgnore]
        public ToolRunViewData ToolRunData { get; set; }
        private IEventAggregator _eventAggregator;
        public string VisionStep { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public CaptureBase Capture { get; set; }
        public ProcedureParam Param { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand DeleteInputParamCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand<string> AddInputParamCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand DeleteOutputParamCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand AddOutputParamCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand TestCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        private HDevEngine ScriptEngine;

        private string ProcedureDir = AppDomain.CurrentDomain.BaseDirectory + "Scripts";
        public ScriptToolViewModel()
        {
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            DeleteInputParamCommand = new DelegateCommand(DeleteInputParam);
            DeleteOutputParamCommand = new DelegateCommand(DeleteOutputParam);
            AddInputParamCommand = new DelegateCommand<string>(AddInputParam);
            AddOutputParamCommand = new DelegateCommand(AddOutputParam);
            TestCommand = new DelegateCommand(Test);
            ScriptEngine = new HDevEngine();
            ScriptEngine.SetProcedurePath(ProcedureDir.Replace("\\", "/"));
        }
        public ScriptToolViewModel(string visionStep)
        {
            VisionStep = visionStep;
            ToolRunData = ContainerLocator.Container.Resolve<ToolRunViewData>(VisionStep);
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            OutputScriptData = new ObservableCollection<ScriptOutputData>();
            OutputTypeList = new List<string> { "ImageParam", "RegionParam", "DoubleParam", "IntParam", "StringParam" };
            InputScriptData = new ObservableCollection<ScriptInputData>();
            DeleteInputParamCommand = new DelegateCommand(DeleteInputParam);
            DeleteOutputParamCommand = new DelegateCommand(DeleteOutputParam);
            AddInputParamCommand = new DelegateCommand<string>(AddInputParam);
            AddOutputParamCommand = new DelegateCommand(AddOutputParam);
            TestCommand = new DelegateCommand(Test);
            ScriptEngine = new HDevEngine();
            ScriptEngine.SetProcedurePath(ProcedureDir.Replace("\\", "/"));
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

        private string _scriptName;

        public string ScriptName
        {
            get { return _scriptName; }
            set { _scriptName = value; RaisePropertyChanged(); }
        }

        private string _inputParamName;

        public string InputParamName
        {
            get { return _inputParamName; }
            set { _inputParamName = value; RaisePropertyChanged(); }
        }

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

        private ObservableCollection<ScriptInputData> _inputScriptData;

        public ObservableCollection<ScriptInputData> InputScriptData
        {
            get { return _inputScriptData; }
            set { _inputScriptData = value; RaisePropertyChanged(); }
        }

        private ScriptInputData _inputScriptDataSelected;

        public ScriptInputData InputScriptDataSelected
        {
            get { return _inputScriptDataSelected; }
            set { _inputScriptDataSelected = value; RaisePropertyChanged(); }
        }

        public List<string> OutputTypeList { get; set; }

        private string _outputType;

        public string OutputType
        {
            get { return _outputType; }
            set { _outputType = value; RaisePropertyChanged(); }
        }

        private string _outputParamName;

        public string OutputParamName
        {
            get { return _outputParamName; }
            set { _outputParamName = value; }
        }


        private ObservableCollection<ScriptOutputData> _outputScriptData;

        public ObservableCollection<ScriptOutputData> OutputScriptData
        {
            get { return _outputScriptData; }
            set { _outputScriptData = value; RaisePropertyChanged(); }
        }

        private ScriptOutputData _outputScriptDataSelected;

        public ScriptOutputData OutputScriptDataSelected
        {
            get { return _outputScriptDataSelected; }
            set { _outputScriptDataSelected = value; RaisePropertyChanged(); }
        }

        public void Run(ToolBase tool, ref bool result, ref string strResult)
        {
            if (DisplayImage == null)
            {
                result = false;
                return;
            }

            DisplayRegion = null;
            string key = $"{tool.ToolInVision}_{tool.ToolItem.Header}";
            bool tempResult = ExecuteScript(false);

            ShowOutput(tempResult);

            foreach (var item in InputScriptData)
            {
                TreeViewItem temp;
                switch (item.DataType)
                {
                    case "Image":
                        temp = new TreeViewItem { Header = item.DataName, ToolTip = ToolRunData.ToolOutputImage[item.DataKey].ToString() };
                        tool.AddInputOutputTree(temp, true);
                        break;
                    case "Region":
                        temp = new TreeViewItem { Header = item.DataName, ToolTip = ToolRunData.ToolOutputRegion[item.DataKey].ToString() };
                        tool.AddInputOutputTree(temp, true);
                        break;
                    case "Double":
                        temp = new TreeViewItem { Header = item.DataName, ToolTip = ToolRunData.ToolOutputDoubleValue[item.DataKey].ToString() };
                        tool.AddInputOutputTree(temp, true);
                        break;
                    case "Int":
                        temp = new TreeViewItem { Header = item.DataName, ToolTip = ToolRunData.ToolOutputIntValue[item.DataKey].ToString() };
                        tool.AddInputOutputTree(temp, true);
                        break;
                    case "String":
                        temp = new TreeViewItem { Header = item.DataName, ToolTip = ToolRunData.ToolOutputStringValue[item.DataKey] };
                        tool.AddInputOutputTree(temp, true);
                        break;
                }
            }

            foreach (var item in OutputScriptData)
            {
                TreeViewItem temp = new TreeViewItem { Header = item.DataName, ToolTip = item.Value.ToString() };
                tool.AddInputOutputTree(temp, false);
                switch (item.DataType)
                {
                    case "Image":
                        HObject tempImage = item.Value as HObject;
                        AddImageToData($"{key}_{item.DataName}", tempImage);

                        break;
                    case "Region":
                        HObject tempRegion = item.Value as HObject;
                        AddRegionToData($"{key}_{item.DataName}", tempRegion);
                        break;
                    case "Double":
                    case "Int":
                    case "String":
                        AddDoubleToData($"{key}_{item.DataName}", item.Value.ToString());
                        break;
                }
            }

            HObjectParams tempHobjectCamera = new HObjectParams { Image = DisplayImage, VisionStep = tool.ToolInVision, ImageKey = key, Region = DisplayRegion, RegionKey = key };
            _eventAggregator.GetEvent<HObjectEvent>().Publish(tempHobjectCamera);

            result = tempResult;
        }

        private void Test()
        {
            bool result = ExecuteScript();
            ShowOutput(result);
        }

        private void AddImageToData(string key, HObject image)
        {
            if (ToolRunData.ToolOutputImage.ContainsKey(key))
            {
                ToolRunData.ToolOutputImage[key] = image;
                return;
            }

            ToolRunData.ToolOutputImage.Add(key, image);
        }

        private void AddRegionToData(string key, HObject region)
        {
            if (ToolRunData.ToolOutputRegion.ContainsKey(key))
            {
                ToolRunData.ToolOutputRegion[key] = region;
                return;
            }

            ToolRunData.ToolOutputRegion.Add(key, region);
        }

        private void AddStringToData(string key, string value)
        {
            if (ToolRunData.ToolOutputStringValue.ContainsKey(key))
            {
                ToolRunData.ToolOutputStringValue[key] = value;
                return;
            }

            ToolRunData.ToolOutputStringValue.Add(key, value);
        }

        private void AddDoubleToData(string key, string value)
        {
            string temp = value.Substring(value.IndexOf(":") + 1, value.Length - 1 - value.IndexOf(":"));
            double result;

            if (temp.Contains(","))
            {
                AddStringToData(key, temp);
                return;
            }

            if (!double.TryParse(temp, out result))
                result = 999.99;

            if (ToolRunData.ToolOutputDoubleValue.ContainsKey(key))
            {
                ToolRunData.ToolOutputDoubleValue[key] = result;
                return;
            }

            ToolRunData.ToolOutputDoubleValue.Add(key, result);
        }

        private void AddIntToData(string key, string value)
        {
            int result;

            if (value.Contains(","))
            {
                AddStringToData(key, value);
                return;
            }

            if (!int.TryParse(value, out result))
                result = 999;

            if (ToolRunData.ToolOutputIntValue.ContainsKey(key))
            {
                ToolRunData.ToolOutputIntValue[key] = result;
                return;
            }

            ToolRunData.ToolOutputIntValue.Add(key, result);
        }

        private void ShowOutput(bool result)
        {
            List<string> tempList = new List<string>();

            foreach (var item in OutputScriptData)
            {
                if (item.DataType != "Image" && item.DataType != "Region")
                {

                    string tempStr = SetMessage(item.DataName, item.Value.ToString());
                    tempList.Add(tempStr);
                }
            }

            string message;
            if (result)
            {
                string tempMessage = string.Join("\n", tempList);
                message = $"OK\n{tempMessage}";
                ShowRunInfo(message);
            }
            else
            {
                string tempMessage = string.Join("\n", tempList);
                message = $"NG\n{tempMessage}";
                ShowRunInfo(message);
            }
        }

        private void ShowRunInfo(string message, bool IsOk = true)
        {
            DisplayMessage = "";
            MessageColor = IsOk ? "green" : "red";
            DisplayMessage = message;
        }

        private string SetMessage(string name, string value)
        {
            string temp = string.Empty;

            if (value.Contains("["))
            {
                temp = value.Substring(1, value.Length - 2);
            }

            if (!string.IsNullOrWhiteSpace(temp) && temp.Contains(","))
            {
                var strArray = temp.Split(',');

                for (int i = 0; i < strArray.Length; i++)
                {
                    if (strArray[i].Contains("."))
                    {
                        strArray[i] = strArray[i].Substring(0, strArray[i].IndexOf('.') + 4);
                    }
                }

                return $"{name}: {string.Join(",", strArray)}";
            }

            if (value.Contains("."))
            {
                temp = value.Substring(0, value.IndexOf('.') + 3);
            }
            else
            {
                temp = value;
            }

            return $"{name}: {temp}";
        }

        private bool ExecuteScript(bool isTest = true)
        {
            if (string.IsNullOrWhiteSpace(ScriptName))
            {
                if (isTest)
                {
                    MessageBox.Show($"脚本文件名不能为空，请重新配置", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            try
            {
                HDevProcedure procedure = new HDevProcedure(ScriptName);
                HDevProcedureCall procCall = new HDevProcedureCall(procedure);

                foreach (var item in InputScriptData)
                {
                    switch (item.DataType)
                    {
                        case "Image":
                            DisplayImage = ToolRunData.ToolOutputImage[item.DataKey];
                            procCall.SetInputIconicParamObject(item.DataName, DisplayImage);
                            break;
                        case "Region":
                            procCall.SetInputIconicParamObject(item.DataName, ToolRunData.ToolOutputRegion[item.DataKey].Clone());
                            break;
                        case "Double":
                            procCall.SetInputCtrlParamTuple(item.DataName, ToolRunData.ToolOutputDoubleValue[item.DataKey]);
                            break;
                        case "Int":
                            procCall.SetInputCtrlParamTuple(item.DataName, ToolRunData.ToolOutputIntValue[item.DataKey]);
                            break;
                        case "String":
                            procCall.SetInputCtrlParamTuple(item.DataName, ToolRunData.ToolOutputStringValue[item.DataKey]);
                            break;
                    }
                }

                procCall.Execute();

                HObject temp;
                HOperatorSet.GenEmptyObj(out temp);

                foreach (var item in OutputScriptData)
                {
                    switch (item.DataType)
                    {
                        case "Image":
                            HObject tempHImage = procCall.GetOutputIconicParamObject(item.DataName);
                            item.Value = tempHImage.Clone();
                            break;
                        case "Region":
                            HObject tempHRegion = procCall.GetOutputIconicParamObject(item.DataName);
                            HObject xldTemp;
                            item.Value = tempHRegion.Clone();
                            HOperatorSet.ConcatObj(tempHRegion, temp, out xldTemp);
                            temp = xldTemp.Clone();
                            tempHRegion.Dispose();
                            xldTemp.Dispose();
                            break;
                        case "Double":
                            double tempDouble = procCall.GetOutputCtrlParamTuple(item.DataName);
                            item.Value = tempDouble;
                            break;
                        case "Int":
                            int tempInt = procCall.GetOutputCtrlParamTuple(item.DataName);
                            item.Value = tempInt;
                            break;
                        case "String":
                            string tempString = procCall.GetOutputCtrlParamTuple(item.DataName);
                            item.Value = tempString;
                            break;
                    }
                }

                DisplayRegion = temp.Clone();
                temp.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                if (isTest)
                {
                    MessageBox.Show($"HDevEngine exception. {ex.Message}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            }

        }

        private void DeleteInputParam()
        {
            if (InputScriptData != null)
            {
                InputScriptData.Remove(InputScriptDataSelected);
            }
        }

        private void DeleteOutputParam()
        {
            if (OutputScriptData != null)
            {
                OutputScriptData.Remove(OutputScriptDataSelected);
            }
        }

        private void AddInputParam(string param)
        {
            var addInputParamName = from key in InputScriptData
                                    where key.DataName == InputParamName
                                    select key;
            if (addInputParamName.ToList().Count == 1)
            {
                MessageBox.Show($"输入参数中已经存在{addInputParamName}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            switch (param)
            {
                case "ImageParam":
                    if (string.IsNullOrWhiteSpace(AddImageDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addImageDataKey = from key in InputScriptData
                                          where key.DataKey == AddImageDataKey
                                          select key;
                    if (addImageDataKey.ToList().Count == 1)
                    {
                        MessageBox.Show($"输入参数中已经存在{addImageDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    InputScriptData.Add(new ScriptInputData { DataKey = AddImageDataKey, DataType = "Image", DataName = InputParamName });
                    InputParamName = string.Empty;
                    break;
                case "RegionParam":
                    if (string.IsNullOrWhiteSpace(AddRegionDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addRegionDataKey = from key in InputScriptData
                                           where key.DataKey == AddRegionDataKey
                                           select key;
                    if (addRegionDataKey.ToList().Count == 1)
                    {
                        MessageBox.Show($"输入参数中已经存在{addRegionDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    InputScriptData.Add(new ScriptInputData { DataKey = AddRegionDataKey, DataType = "Region", DataName = InputParamName });
                    break;
                case "DoubleParam":
                    if (string.IsNullOrWhiteSpace(AddDoubleDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addDoubleDataKey = from key in InputScriptData
                                           where key.DataKey == AddDoubleDataKey
                                           select key;
                    if (addDoubleDataKey.ToList().Count == 1)
                    {
                        MessageBox.Show($"输入参数中已经存在{addDoubleDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    InputScriptData.Add(new ScriptInputData { DataKey = AddDoubleDataKey, DataType = "Double", DataName = InputParamName });
                    break;
                case "IntParam":
                    if (string.IsNullOrWhiteSpace(AddIntDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addIntDataKey = from key in InputScriptData
                                        where key.DataKey == AddIntDataKey
                                        select key;
                    if (addIntDataKey.ToList().Count == 1)
                    {
                        MessageBox.Show($"输入参数中已经存在{addIntDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    InputScriptData.Add(new ScriptInputData { DataKey = AddIntDataKey, DataType = "Int", DataName = InputParamName });
                    break;
                case "StringParam":
                    if (string.IsNullOrWhiteSpace(AddStringDataKey))
                    {
                        MessageBox.Show($"列表中没有值，无法添加！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var addStringDataKey = from key in InputScriptData
                                           where key.DataKey == AddStringDataKey
                                           select key;

                    if (addStringDataKey.ToList().Count == 1)
                    {
                        MessageBox.Show($"输入参数中已经存在{addStringDataKey}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    InputScriptData.Add(new ScriptInputData { DataKey = AddStringDataKey, DataType = "String", DataName = InputParamName });
                    break;
            }
        }

        private void AddOutputParam()
        {
            if (string.IsNullOrWhiteSpace(OutputParamName))
            {
                MessageBox.Show($"请输入输出参数名称，再进行添加", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var addOutputParamName = from key in OutputScriptData
                                     where key.DataName == OutputParamName
                                     select key;
            if (addOutputParamName.ToList().Count == 1)
            {
                MessageBox.Show($"输入参数中已经存在{addOutputParamName}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string outputType = string.Empty;

            switch (OutputType)
            {
                case "ImageParam":
                    outputType = "Image";
                    break;
                case "RegionParam":
                    outputType = "Region";
                    break;
                case "DoubleParam":
                    outputType = "Double";
                    break;
                case "IntParam":
                    outputType = "Int";
                    break;
                case "StringParam":
                    outputType = "String";
                    break;
            }

            OutputScriptData.Add(new ScriptOutputData { DataType = outputType, DataName = OutputParamName });
            OutputParamName = string.Empty;
        }
    }
}
