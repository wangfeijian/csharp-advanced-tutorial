using Prism.Mvvm;
using Prism.Ioc;
using SosoVisionTool.Views;
using System.Windows.Controls;
using SosoVisionTool.Services;
using ImageCapture;
using SosoVisionCommonTool.ConfigData;

namespace SosoVisionTool.ViewModels
{
    public class ImageBlobToolViewModel : BindableBase, IToolBaseViewModel
    {
        public ImageBlobToolViewModel()
        {
        }

        public ImageBlobToolViewModel(string visionStep)
        {
            VisionStep = visionStep;   
            ToolRunData = ContainerLocator.Container.Resolve<ToolRunViewData>(VisionStep);
        }
        [Newtonsoft.Json.JsonIgnore]
        public ToolRunViewData ToolRunData { get; set; }
        public string VisionStep { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public CaptureBase Capture { get; set; }
        public ProcedureParam Param { get; set; }

        public void Run(ToolBase tool, ref bool result)
        {
            TreeViewItem temp = new TreeViewItem { Header = "input", ToolTip = "input" };
            tool.AddInputOutputTree(temp, true);
            temp = new TreeViewItem { Header = "output", ToolTip = "output" };
            tool.AddInputOutputTree(temp, false);
            result = false;
        }
    }
}
