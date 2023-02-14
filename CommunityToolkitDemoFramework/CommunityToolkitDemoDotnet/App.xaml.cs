using CommunityToolkitDemoDotnet.Services;
using CommunityToolkitDemoDotnet.ViewModels;
using CommunityToolkitDemoDotnet.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace CommunityToolkitDemoDotnet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            Services = ConfigureServices();
        }
        public IServiceProvider Services { get; }

        public new static App Current => (App)Application.Current;
        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IDemoService, DemoService>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<OtherWindowViewModel>();
            services.AddSingleton<MainWindow>();
            services.AddTransient<OtherWindow>();

            return services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = Current.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
