using ImageCapture;
using Prism.Events;
using Prism.Mvvm;
using SosoVisionCommonTool.ConfigData;
using SosoVisionTool.Services;
using SosoVisionTool.Views;
using Prism.Ioc;

namespace SosoVisionTool.ViewModels
{
    public class OutputParamSetToolViewModel : BindableBase, IToolBaseViewModel
    {
        private IEventAggregator _eventAggregator;

        public OutputParamSetToolViewModel()
        {
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
        }
        public OutputParamSetToolViewModel(string visionStep)
        {
            VisionStep = visionStep;
            ToolRunData = ContainerLocator.Container.Resolve<ToolRunViewData>(VisionStep);
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
        }
        [Newtonsoft.Json.JsonIgnore]
        public ToolRunViewData ToolRunData { get; set; }
        public string VisionStep { get; set; }
        public ProcedureParam Param { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public CaptureBase Capture { get; set; }

        public void Run(ToolBase tool, ref bool result)
        {
            result = true;
        }
    }
}
