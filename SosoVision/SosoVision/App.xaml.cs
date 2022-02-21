using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
using SosoVisionTool.ViewModels;
using SosoVisionTool.Views;

namespace SosoVision
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISosoLogManager, SosoLogManager>();
            containerRegistry.RegisterSingleton<IConfigureService, ConfigureService>();
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
            containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();
            containerRegistry.RegisterForNavigation<ToolControlBoxView, ToolControlBoxViewModel>();
            containerRegistry.RegisterForNavigation<ToolRunView, ToolRunViewModel>();

            var configureService = Container.Resolve<IConfigureService>();
            if (configureService.SerializationData.ProcedureParams != null)
            {
                foreach (var title in configureService.SerializationData.ProcedureParams)
                {
                    string file = $"config/Vision/{title.Name}/{title.Name}.json";
                    var viewModel = File.Exists(file)
                        ? JsonConvert.DeserializeObject<VisionProcessViewModel>(File.ReadAllText(file))
                        : new VisionProcessViewModel(title.Name);
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

                            foreach (var fileInfo in files)
                            {
                                string head = fileInfo.Name.Substring(0, fileInfo.Name.Length - 5);
                                var tempTree = TreeViewDataAccess.CreateTreeView(head);
                                Type t = JsonConvert.DeserializeObject<Type>(File.ReadAllText(fileInfo.FullName));
                                var tag = Activator.CreateInstance(t);
                                tempTree.Tag = tag;
                                toolTreeView.Items.Add(tempTree);
                            }
                        }
                    }
                    containerRegistry.RegisterInstance(typeof(VisionProcessView), view, title.Name);
                }
            }
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
