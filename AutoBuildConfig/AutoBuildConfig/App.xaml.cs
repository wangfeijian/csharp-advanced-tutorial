using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;

namespace AutoBuildConfig
{

    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static System.Collections.Specialized.NameValueCollection ConfigStr = ConfigurationManager.AppSettings;
        static string ConfigKey = ConfigStr["DllKey"];
        static string ConfigDllName = ConfigKey.Split(',')[0];
        static string ConfigClassName = ConfigKey.Split(',')[1];
        static Type t = Type.GetType(ConfigClassName);
    }
}
