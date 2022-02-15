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
using Prism.Mvvm;
using Prism.Regions;
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
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISosoLogManager, SosoLogManager>();
            containerRegistry.RegisterSingleton<IConfigureService, ConfigureService>();
            containerRegistry.RegisterForNavigation<HomeView, HomeViewModel>();
            containerRegistry.RegisterForNavigation<SettingView, SettingViewModel>();

            var configureService = Container.Resolve<IConfigureService>();
            if (configureService.SerializationData.ProcedureParams != null)
            {
                foreach (var title in configureService.SerializationData.ProcedureParams)
                {
                    containerRegistry.RegisterInstance(typeof(VisionProcessView), new VisionProcessView(), title.Name);
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
