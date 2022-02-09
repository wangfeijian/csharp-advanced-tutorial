using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;

namespace PrismDemo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication  // step 1 : 修改继承基类，前台代码也需要修改
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override Window CreateShell()
        {
            // step 2 : 实现抽象方法 返回主窗口
            // 将前台中StartupUri="MainWindow.xaml"删除，否则会启动两个窗口
            return Container.Resolve<MainWindow>();
        }
    }
}
