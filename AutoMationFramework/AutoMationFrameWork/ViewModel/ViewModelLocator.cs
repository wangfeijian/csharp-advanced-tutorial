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

using CommonTools.Servers;
using CommonServiceLocator;
using CommonTools.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Configuration;
using System.Reflection;
using CommonTools.Manager;

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
            SimpleIoc.Default.Register(() => { return (IBuildConfig)t.Assembly.CreateInstance(ConfigClassName); });

            RunInforManager.GetInstance().ReadXmlConfig();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public SysParamControlViewModel SysParam => ServiceLocator.Current.GetInstance<SysParamControlViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}