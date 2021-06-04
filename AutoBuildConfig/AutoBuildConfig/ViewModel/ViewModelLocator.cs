/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:AutoBuildConfig"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System.Configuration;
using System.Reflection;
using AutoBuildConfig.Model;

namespace AutoBuildConfig.ViewModel
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
            Type t = Type.GetType(ConfigClassName);

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
            SimpleIoc.Default.Register<SystemCfgViewModel>();
            SimpleIoc.Default.Register<PointViewModel>();
            SimpleIoc.Default.Register(() => { return (IBuildConfig)t.Assembly.CreateInstance(ConfigClassName); });

        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }


        public SystemCfgViewModel SystemCfg => ServiceLocator.Current.GetInstance<SystemCfgViewModel>();
        public PointViewModel PointCfg => ServiceLocator.Current.GetInstance<PointViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}