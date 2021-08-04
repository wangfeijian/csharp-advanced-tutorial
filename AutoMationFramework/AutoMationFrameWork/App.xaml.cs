/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-06-21                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Program entrance                         *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AutoMationFrameWork.ViewModel;
using CommonTools.Tools;
using Newtonsoft.Json;

namespace AutoMationFrameWork
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        private static DispatcherOperationCallback exitFrameCallback = ExitFrame;
        private Mutex mutex;

        [DllImport("Kernel32.dll")]
        public static extern bool SetDllDirectory(string lpPathName);

        public App()
        {
            Startup += (StartupEventHandler)((s, e) =>
            {
               bool createdNew;
               mutex = new Mutex(true, "AutoMationFrameWork", out createdNew);
               ResourceDictionary resource = LocationServices.GetResource(new CultureInfo(GetLangType()));

               if (resource != null)
               {
                   Resources.MergedDictionaries.Add(resource);
                   Thread.CurrentThread.CurrentUICulture = new CultureInfo(GetLangType());
                   LocationServices.SetMessageBox();
               }
               else
               {
                   LocationServices.GetResource(new CultureInfo("zh-CN".ToString()));
                   Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN".ToString());
               }
               if (!createdNew)
               {
                   MessageBox.Show(LocationServices.GetLang("Process Exists"));
                   Environment.Exit(0);
               }
           });

            var directoryInfo = new DirectoryInfo(System.Windows.Forms.Application.ExecutablePath).Parent;
            if (directoryInfo != null)
                SetDllDirectory(directoryInfo.FullName + "\\Dll");
            FrameworkCompatibilityPreferences.AreInactiveSelectionHighlightBrushKeysSupported = false;
        }

         ~App()
         {
             ViewModelLocator.Cleanup();
         }

        private string GetLangType()
        {
            string str = "zh_cn";
            try
            {
                string filename = Directory.GetCurrentDirectory() + "\\Config\\config.json";
                var reslut = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filename));
                return reslut["lang"];
            }
            catch (Exception)
            {
                return str;
            }
        }

        private static object ExitFrame(object state)
        {
            ((DispatcherFrame) state).Continue = false;
            return null;
        }
    }
}
