using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using ImageCapture;
using Newtonsoft.Json;
using Prism.Ioc;
using Prism.Regions;
using SosoVision.Extensions;
using SosoVision.ViewModels;
using SosoVision.Views;
using SosoVisionCommonTool.ConfigData;
using SosoVisionTool.Services;
using SosoVisionTool.Views;

namespace SosoVision.Common
{
    public class ConfigureService : IConfigureService
    {
        private readonly IRegionManager _regionManager;
        public SerializationData SerializationData { get; set; }

        public ConfigureService(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            Configure();
        }

        public void Configure(bool isSave = false)
        {
            if (!isSave)
            {
                SerializationData = File.Exists("config/config.json") ?
                    JsonConvert.DeserializeObject<SerializationData>(File.ReadAllText("config/config.json"))
                    : new SerializationData { ProcedureParams = new ObservableCollection<ProcedureParam>(), 
                                                CameraParams = new ObservableCollection<CameraParam>(), 
                                                ServerParams = new ObservableCollection<ServerParam>() };
            }
            else
            {
                File.WriteAllText("config/config.json", JsonConvert.SerializeObject(SerializationData));

                foreach (var item in SerializationData.CameraParams)
                {
                    var capture = ContainerLocator.Container.Resolve<CaptureBase>(item.CameraId.ToString());
                    capture.Close();
                }

                foreach (var param in SerializationData.ProcedureParams)
                {
                    var view = _regionManager.Regions[PrismManager.MainViewRegionName].GetView(param.Name) as VisionProcessView;
                    if (view == null) return;
                    var viewModel = view?.DataContext as VisionProcessViewModel;
                    var toolRun = view.FindName("ToolRun") as ToolRunView;
                    var toolTreeView = toolRun.FindName("ToolTreeView") as TreeView;
                    foreach (var item in toolTreeView.Items)
                    {
                        string dir = $"config/Vision/{param.Name}/Tools";
                        string dirData = $"config/Vision/{param.Name}/ToolsData";
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        if (!Directory.Exists(dirData))
                        {
                            Directory.CreateDirectory(dirData);
                        }

                        var treeViewItem = item as TreeViewItem;
                        string fileName = $"{dir}/{treeViewItem.Header}.json";
                        string fileDataName = $"{dirData}/{treeViewItem.Header}.json";

                        Type t = treeViewItem.Tag.GetType();
                        var tool = treeViewItem.Tag as ToolBase;
                        Thread.Sleep(50);
                        File.WriteAllText(fileName, JsonConvert.SerializeObject(t));
                        File.WriteAllText(fileDataName, JsonConvert.SerializeObject(tool.DataContext));
                    }
                    if (viewModel != null)
                    {
                        string dir = $"config/Vision/{param.Name}";
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        string fileName = $"{dir}/{param.Name}.json";
                        string fileDataName = $"{dir}/{param.Name}_Data.json";
                        File.WriteAllText(fileName, JsonConvert.SerializeObject(viewModel));
                        File.WriteAllText(fileDataName, JsonConvert.SerializeObject(ContainerLocator.Container.Resolve<ToolRunViewData>(param.Name)));
                    }
                }
            }
        }
    }
}
