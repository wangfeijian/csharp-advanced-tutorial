/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-19                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Language manager class                   *
*********************************************************************/

using System;
using System.Globalization;
using System.IO;
using System.Windows;
using CommonTools.Manager;

namespace CommonTools.Tools
{
    public static class LocationServices
    {
        public static ResourceDictionary GetResource(CultureInfo cultureInfo)
        {
            Uri uri = new Uri(Directory.GetCurrentDirectory()+"\\Language\\" + cultureInfo.Name + ".xaml", UriKind.RelativeOrAbsolute);
            try
            {
                return new ResourceDictionary() { Source = uri };
            }
            catch
            {
                return null;
            }
        }

        public static void SetMessageBox()
        {
            MessageBoxManager.OK = GetLang("OK");
            MessageBoxManager.Cancel = GetLang("Cancel");
            MessageBoxManager.Retry = GetLang("Retry");
            MessageBoxManager.Ignore = GetLang("Ignore");
            MessageBoxManager.Abort = GetLang("Abort");
            MessageBoxManager.Yes = GetLang("Yes");
            MessageBoxManager.No = GetLang("No");

            //Register manager
            MessageBoxManager.Register();
        }

        public static string GetLang(string infoKey)
        {
            try
            {
                return (string)Application.Current.TryFindResource(infoKey) ?? infoKey;
            }
            catch (Exception)
            {
                return infoKey;
            }
        }

        public static string GetLang(string infoKey, string infoValue)
        {
            try
            {
                return (string)Application.Current.TryFindResource(infoKey) ?? infoValue;
            }
            catch (Exception)
            {
                return infoKey;
            }
        }

        public static string GetLangType()
        {
            string lang = GetLang("Language");
            return lang == "DefaultCH" ? "zh-cn" : (lang == "English" ? "en-us" : "zh-cn");
        }
    }
}
