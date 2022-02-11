using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using NLog.Targets;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using SosoVision.Common;
using SosoVision.Extensions;
using SosoVision.ViewModels;
using SosoVision.Views;

namespace SosoVision
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public SerializationData SerializationData { get; set; }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISosoLogManager, SosoLogManager>();
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
        }

        protected override void Initialize()
        {
            if (File.Exists("config.json"))
            {
                SerializationData = JsonConvert.DeserializeObject<SerializationData>(File.ReadAllText("config.json"));
            }
            base.Initialize();
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

                var viewModel = Current.MainWindow.DataContext as MainViewModel;
                if (viewModel != null && SerializationData!=null) viewModel.VisionStepCollection = SerializationData.VisionTitle;
            }

            base.OnInitialized();
        }
    }
}
