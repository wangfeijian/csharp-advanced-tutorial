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

namespace SosoVisionTool.ViewModels
{
    public class InputParamSetToolViewModel : BindableBase, IToolBaseViewModel
    {
        private IEventAggregator _eventAggregator;

        public DelegateCommand DeleteParamCommand { get; }
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
                    InputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddImageDataKey, DataType = "Image" });
                    break;
                case "RegionParam":
                    InputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddRegionDataKey, DataType = "Region" });
                    break;
                case "DoubleParam":
                    InputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddDoubleDataKey, DataType = "Double" });
                    break;
                case "IntParam":
                    InputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddIntDataKey, DataType = "Int" });
                    break;
                case "StringParam":
                    InputDataKeyAndType.Add(new DataKeyAndType { DataKey = AddStringDataKey, DataType = "String" });
                    break;
            }
        }
    }
}
