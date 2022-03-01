using ImageCapture;
using SosoVisionCommonTool.ConfigData;
using SosoVisionTool.Services;
using SosoVisionTool.Views;

namespace SosoVisionTool.ViewModels
{
    public interface IToolBaseViewModel
    {
        ToolRunViewData ToolRunData { get; set; }
        string VisionStep { get; set; }
        ProcedureParam Param { get; set; }

        CaptureBase Capture { get; set; }
        void Run(ToolBase tool, ref bool result);

    }
}
