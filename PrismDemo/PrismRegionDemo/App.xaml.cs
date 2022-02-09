using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using PrismRegionDemo.Views;

namespace PrismRegionDemo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 使用Prim区域需要注册视图导航
            containerRegistry.RegisterForNavigation<ViewA>();
            containerRegistry.RegisterForNavigation<ViewB>();
            containerRegistry.RegisterForNavigation<ViewC>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        // 使用模块化时通过代码的方式注入view
        //protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        //{
        //    moduleCatalog.AddModule<PrismModuleAProfile>();
        //    moduleCatalog.AddModule<PrismModuleBProfile>();
        //    base.ConfigureModuleCatalog(moduleCatalog);
        //}

        /// <summary>
        /// 通过配置文件夹的方式来实现动态加载模块
        /// </summary>
        /// <returns></returns>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() {ModulePath = @".\Modules"};
        }
    }
}
