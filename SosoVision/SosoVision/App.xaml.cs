using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using NLog.Targets;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using SosoVision.Common;
using SosoVision.Extensions;
using SosoVision.ViewModels;
using SosoVision.Views;
using SosoVisionTool.Services;
using SosoVisionTool.Tools;
using SosoVisionTool.ViewModels;
using SosoVisionTool.Views;
using ImageCapture;
using System.Reflection;
using SosoVisionCommonTool.Log;
using SosoVision.Server;

namespace SosoVision
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        private Mutex mutex;
        public App()
        {
            Startup += (StartupEventHandler)((s, e) =>
            {
                bool createdNew;
                mutex = new Mutex(true, "SosoVision", out createdNew);

                if (!createdNew)
                {
                    MessageBox.Show("程序已经运行！！");
                    Environment.Exit(0);
                }
            });
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISosoLogManager, SosoLogManager>();
            containerRegistry.RegisterSingleton<IConfigureService, ConfigureService>();
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
            containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
            containerRegistry.RegisterForNavigation<ToolControlBoxView, ToolControlBoxViewModel>();
            var configureService = Container.Resolve<IConfigureService>();

            Assembly cameraAssembly = Assembly.Load("ImageCapture");

            if (configureService.SerializationData.CameraParams != null)
                foreach (var item in configureService.SerializationData.CameraParams)
                {
                    string typeName = $"ImageCapture.{ item.CameraBand}";
                    Type camera = cameraAssembly.GetType(typeName);
                    var cameraInstance = Activator.CreateInstance(camera, item.CameraIP, false);
                    containerRegistry.RegisterInstance(typeof(CaptureBase), cameraInstance, item.CameraId.ToString());
                }

            if (configureService.SerializationData.ServerParams != null)
                foreach (var item in configureService.SerializationData.ServerParams)
                {
                    containerRegistry.RegisterInstance(typeof(SosoVisionServerHelper), new SosoVisionServerHelper(item.ServerIp, item.ServerPort));
                }

            if (configureService.SerializationData.ProcedureParams != null)
            {
                foreach (var title in configureService.SerializationData.ProcedureParams)
                {
                    string file = $"config/Vision/{title.Name}/{title.Name}.json";
                    string fileData = $"config/Vision/{title.Name}/{title.Name}_Data.json";
                    var viewModel = File.Exists(file)
                        ? JsonConvert.DeserializeObject<VisionProcessViewModel>(File.ReadAllText(file))
                        : new VisionProcessViewModel(title.Name);

                    var toolRunData = File.Exists(fileData)
                        ? JsonConvert.DeserializeObject<ToolRunViewData>(File.ReadAllText(fileData))
                        : new ToolRunViewData
                        {
                            ToolOutputDoubleValue = new Dictionary<string, double>(),
                            ToolOutputImage = new Dictionary<string, HalconDotNet.HObject>(),
                            ToolOutputIntValue = new Dictionary<string, int>(),
                            ToolOutputRegion = new Dictionary<string, HalconDotNet.HObject>()
                        };

                    containerRegistry.RegisterInstance(typeof(ToolRunViewData), toolRunData, title.Name);

                    var view = new VisionProcessView { DataContext = viewModel };
                    var toolRun = view.FindName("ToolRun") as ToolRunView;
                    var toolTreeView = toolRun.FindName("ToolTreeView") as TreeView;

                    if (Directory.Exists($"config/Vision/{title.Name}/Tools"))
                    {
                        DirectoryInfo directory = new DirectoryInfo($"config/Vision/{title.Name}/Tools");
                        var files = directory.GetFiles();
                        if (files.Length > 0)
                        {
                            Array.Sort(files, (x, y) => { return x.LastWriteTime.CompareTo(y.LastWriteTime); });
                            string dir = $"config/Vision/{title.Name}/ToolsData";
                            foreach (var fileInfo in files)
                            {
                                string head = fileInfo.Name.Substring(0, fileInfo.Name.Length - 5);
                                Type t = JsonConvert.DeserializeObject<Type>(File.ReadAllText(fileInfo.FullName));
                                var tag = Activator.CreateInstance(t) as ToolBase;
                                var tempTree = tag.CreateTreeView(head);
                                tag.ToolInVision = title.Name;
                                tag.CameraId = title.CameraId.ToString();
                                if (File.Exists($"{dir}/{fileInfo.Name}"))
                                {
                                    tag.DataContext = tag.GetDataContext($"{dir}/{fileInfo.Name}");
                                    var tempDataContext = tag.DataContext as IToolBaseViewModel;
                                    tempDataContext.ToolRunData = ContainerLocator.Container.Resolve<ToolRunViewData>(tag.ToolInVision);
                                    tempDataContext.Capture = ContainerLocator.Container.Resolve<CaptureBase>(tag.CameraId);
                                    tempDataContext.Param = title;
                                }
                                tempTree.Tag = tag;
                                tempTree.PreviewMouseDoubleClick += Tree_PreviewMouseDoubleClick;
                                toolTreeView.Items.Add(tempTree);
                            }
                        }
                    }
                    containerRegistry.RegisterInstance(typeof(VisionProcessView), view, title.Name);
                }
            }
        }

        private void Tree_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            ToolBase tool = treeViewItem.Tag as ToolBase;

            tool.UIElement_OnPreviewMouseDoubleClick(sender, e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var configureService = Container.Resolve<IConfigureService>();
            configureService.Configure(true);

            base.OnExit(e);
        }


        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        protected override void OnInitialized()
        {
            if (Current.MainWindow != null)
            {
                var service = Current.MainWindow.DataContext as IConfigureService;
                service?.Configure();
            }

            base.OnInitialized();
        }
    }
}
