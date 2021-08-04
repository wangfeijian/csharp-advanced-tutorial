/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-19                               *
*                                                                    *
*           ModifyTime:     2021-07-28                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Language manager class                   *
*********************************************************************/

using System;
using System.Globalization;
using System.IO;
using System.Windows;

namespace CommonTools.Tools
{
    /// <summary>
    /// 语言管理静态类
    /// </summary>
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

        /// <summary>
        /// 设置messagebox对话框中的语言
        /// </summary>
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

        /// <summary>
        /// 根据键值获取对应的中、英文
        /// </summary>
        /// <param name="infoKey">键</param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据键值获取对应的中、英文
        /// 如果键值不存在，就使用输入的值
        /// </summary>
        /// <param name="infoKey">键</param>
        /// <param name="infoValue">值</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取软件上次设置的语言
        /// </summary>
        /// <returns></returns>
        public static string GetLangType()
        {
            string lang = GetLang("Language");
            return lang == "DefaultCH" ? "zh-cn" : (lang == "English" ? "en-us" : "zh-cn");
        }
    }
}
