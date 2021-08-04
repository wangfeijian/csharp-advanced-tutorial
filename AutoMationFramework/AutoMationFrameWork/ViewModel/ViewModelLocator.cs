/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:AutoMationFrameWork"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Configuration;
using System.Reflection;
using System.Windows;
using CommonTools.Tools;
using AutoMationFrameworkDll;
using AutoMationFrameworkViewModel;
using AutoMationFrameworkSystemDll;
using ConfigTools;

namespace AutoMationFrameWork.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            System.Collections.Specialized.NameValueCollection ConfigStr = ConfigurationManager.AppSettings;
            string ConfigKey = ConfigStr["DllKey"];
            string ConfigDllName = ConfigKey.Split(',')[0];
            string ConfigClassName = ConfigKey.Split(',')[1];
            var assembly = Assembly.Load(ConfigDllName);
            Type t = assembly.GetType(ConfigClassName);

            string ConfigDir = ConfigStr["ConfigDir"];

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SysParamControlViewModel>();
            SimpleIoc.Default.Register<SystemConfigViewModel>();
            SimpleIoc.Default.Register(() => { return ConfigDir; });
            SimpleIoc.Default.Register(() => { return (IBuildConfig)t.Assembly.CreateInstance(ConfigClassName); });

            if (ConfigApp(ConfigDir))
            {

            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public SysParamControlViewModel SysParam => ServiceLocator.Current.GetInstance<SysParamControlViewModel>();

        public SystemConfigViewModel SysConfig => ServiceLocator.Current.GetInstance<SystemConfigViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        public bool ConfigApp(string ConfigDir)
        {
            try
            {
                //ErrorCodeMgr.GetInstance();
                RunInforManager.GetInstance();

                if (ConfigManager.GetInstance().LoadConfigFile(ConfigDir))
                {

                    return true;
                }

                MessageBox.Show(LocationServices.GetLang("SysInitError"), LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(LocationServices.GetLang("SysInitError"), LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }
    }
}