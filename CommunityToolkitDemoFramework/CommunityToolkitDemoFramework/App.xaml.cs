// 第一步：使用nuget添加三个包
//     1.CommunityToolkit.Mvvm
//     2.Microsoft.Extensions.DependencyInjection
//     3.Microsoft.Extensions.Hosting
using System;
using System.Windows;
using CommunityToolkitDemoFramework.Services;
using CommunityToolkitDemoFramework.View;
using CommunityToolkitDemoFramework.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommunityToolkitDemoFramework
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        // 第二步：在App的构造函数中写入以下代码实现依赖注入
        // 由于注入的时候使用的单例模式，将App.xaml中的StartupUri="MainWindow.xaml"删除，并重载OnStartup函数。
        public App()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .Build();

            ServiceProvider = host.Services;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // GetService和GetRequiredService的区别如下：
            // 1.如果没有注册GetService返回null，GetRequiredService抛出异常
            var window = ServiceProvider.GetRequiredService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

        private void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            // 第三步：添加Service文件夹，并注入DemoService，就可以在其它构造函数中进行注入了
            services.AddSingleton<IDemoService, DemoService>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<OtherWindowViewModel>();
            services.AddSingleton<MainWindow>();
            // 每次都是获取一个新对象，用完销毁
            services.AddTransient<OtherWindow>();
        }
    }
}
