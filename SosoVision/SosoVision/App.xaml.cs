using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using Prism.Ioc;
using SosoVision.Common;
using SosoVision.ViewModels;
using SosoVision.Views;
using SosoVisionTool.Services;
using SosoVisionTool.ViewModels;
using SosoVisionTool.Views;
using ImageCapture;
using System.Reflection;
using SosoVisionCommonTool.Log;
using SosoVision.Server;
using System.Runtime.InteropServices;
using SosoVisionCommonTool.Authority;
using System.Threading.Tasks;

namespace SosoVision
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        private Mutex mutex;

        [DllImport("Kernel32.dll")]
        public static extern bool SetDllDirectory(string lpPathName);
        public App()
        {
            var directoryInfo = AppDomain.CurrentDomain.BaseDirectory + "Dll";
            if (directoryInfo != null)
                SetDllDirectory(directoryInfo);

            Startup += (s, e) =>
            {
                if (e.Args.Length == 2)
                {
                    AuthorityTool.ChangeEngMode(e.Args[1]);
                }
                else
                {
                    AuthorityTool.ChangeOpMode();
                }

                bool createdNew;
                mutex = new Mutex(true, "SosoVision", out createdNew);

                if (!createdNew)
                {
                    MessageBox.Show("程序已经运行！！");
                    Environment.Exit(0);
                }
            };

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);
                    bool result = AuthorityTool.GetLastInputTime() / 1000 < 600 || AuthorityTool.IsOpMode();
                    if (!result)
                    {
                        AuthorityTool.ChangeOpMode();
                    }
                }
            });

        }

        /// <summary>
        /// 注册程序实例
        /// </summary>
        /// <param name="containerRegistry"></param>
        private void RegisterInstance(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISosoLogManager, SosoLogManager>();
            containerRegistry.RegisterSingleton<IConfigureService, ConfigureService>();
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
            containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
            containerRegistry.RegisterForNavigation<ToolControlBoxView, ToolControlBoxViewModel>();
        }

        /// <summary>
        /// 注册相机对象
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="configureService"></param>
        private void RegisterCameraInstance(IContainerRegistry containerRegistry, IConfigureService configureService)
        {
            Assembly cameraAssembly = Assembly.Load("ImageCapture");

            if (configureService.SerializationData.CameraParams != null)
                foreach (var item in configureService.SerializationData.CameraParams)
                {
                    string typeName = $"ImageCapture.{ item.CameraBand}";
                    Type camera = cameraAssembly.GetType(typeName);
                    var cameraInstance = Activator.CreateInstance(camera, item.CameraIP, false);
                    containerRegistry.RegisterInstance(typeof(CaptureBase), cameraInstance, item.CameraId.ToString());
                }
        }

        /// <summary>
        /// 注册服务对象
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="configureService"></param>
        private void RegisterServerInstance(IContainerRegistry containerRegistry, IConfigureService configureService)
        {
            if (configureService.SerializationData.ServerParams != null)
                foreach (var item in configureService.SerializationData.ServerParams)
                {
                    containerRegistry.RegisterInstance(typeof(SosoVisionServerHelper), new SosoVisionServerHelper(item.ServerIp, item.ServerPort));
                }
        }

        /// <summary>
        /// 注册全局数据
        /// </summary>
        /// <param name="containerRegistry"></param>
        private void RegisterGlobalData(IContainerRegistry containerRegistry)
        {
            string file = $"config/Vision/data.json";

            var globalVisionData = File.Exists(file)
                ? JsonConvert.DeserializeObject<AllVisionRunData>(File.ReadAllText(file))
                : new AllVisionRunData
                {
                    VisionRunDoubleValue = new Dictionary<string, double>(),
                    VisionRunImage = new Dictionary<string, HalconDotNet.HObject>(),
                    VisionRunIntValue = new Dictionary<string, int>(),
                    VisionRunRegion = new Dictionary<string, HalconDotNet.HObject>(),
                    VisionRunStringValue = new Dictionary<string, string>(),
                };

            containerRegistry.RegisterInstance(typeof(AllVisionRunData), globalVisionData, "GlobalData");
        }

        /// <summary>
        /// 注册流程对象及流程中的工具和数据
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="configureService"></param>
        private void RegisterVisionInstance(IContainerRegistry containerRegistry, IConfigureService configureService)
        {
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
                            ToolOutputRegion = new Dictionary<string, HalconDotNet.HObject>(),
                            ToolOutputStringValue = new Dictionary<string, string>(),
                            ToolInputReceiveStringValue = new Dictionary<string, string>(),
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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            try
            {
                // 注册程序实例
                RegisterInstance(containerRegistry);

                // 获取配置服务对象
                var configureService = Container.Resolve<IConfigureService>();

                // 注册相机对象
                RegisterCameraInstance(containerRegistry, configureService);

                // 注册服务对象
                RegisterServerInstance(containerRegistry, configureService);

                // 注册全局数据
                RegisterGlobalData(containerRegistry);

                // 注册流程及工具数据
                RegisterVisionInstance(containerRegistry, configureService);
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message, "提示！");
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
