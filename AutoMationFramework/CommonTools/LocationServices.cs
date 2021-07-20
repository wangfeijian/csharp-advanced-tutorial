using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CommonTools
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
                return (ResourceDictionary)null;
            }
        }

        public static string GetLang(string InfoKey)
        {
            try
            {
                return (string)Application.Current.TryFindResource((object)InfoKey) ?? InfoKey;
            }
            catch (Exception ex)
            {
                return InfoKey;
            }
        }

        public static string GetLang(string InfoKey, string InfoValue)
        {
            try
            {
                return (string)Application.Current.TryFindResource((object)InfoKey) ?? InfoValue;
            }
            catch (Exception ex)
            {
                return InfoKey;
            }
        }

        public static string GetLangType()
        {
            string lang = GetLang("Language");
            return lang == "DefaultCH" ? "zh-cn" : (lang == "English" ? "en-us" : "zh-cn");
        }
    }
}
