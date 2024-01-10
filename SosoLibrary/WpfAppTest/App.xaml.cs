using System;
using Autofac;
using Soso.Contract;
using Soso.Contract.Interface;
using Soso.Log;
using Soso.Services;
using System.Windows;
using WpfAppTest.ViewModel;

namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ConfigureServices();
        }

        private void ConfigureServices()
        {
            DIServices.Instance.AddPrivateCtorInstance<ILogServices, LogServices>();
            DIServices.Instance.ContainerBuilder.RegisterType<MainWindowViewModel>();
            DIServices.Instance.ContainerBuilder.RegisterType<MainWindow>();
            DIServices.Instance.ServicesBuilder();


        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _ = SystemServices.Instance;
            var mainWindow = DIServices.Instance.Container.Resolve<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
