using HalconDotNet;
using ImageCapture;
using Prism.Mvvm;
using Prism.Ioc;
using SosoVisionCommonTool.ConfigData;
using SosoVisionTool.Services;
using SosoVisionTool.Views;
using System;
using System.Collections.Generic;
using Prism.Commands;
using System.Windows;
using SosoVisionTool.Tools;
using Prism.Events;
using System.Windows.Controls;

namespace SosoVisionTool.ViewModels
{
    public class FindLineToolViewModel : BindableBase, IToolBaseViewModel
    {
        [Newtonsoft.Json.JsonIgnore]
        public ToolRunViewData ToolRunData { get; set; }
       public string VisionStep { get; set; }
       public ProcedureParam Param { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public CaptureBase Capture { get; set; }

        public void Run(ToolBase tool, ref bool result, ref string strResult)
        {
            
        }

        public FindLineToolViewModel()
        {

        }
        public FindLineToolViewModel(string visionStep)
        {
            VisionStep = visionStep;
            ToolRunData = ContainerLocator.Container.Resolve<ToolRunViewData>(VisionStep);
        }
        }
}
