using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommonTools;
using Newtonsoft.Json;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// SelectLanguageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectLanguageWindow : Window
    {
        public SelectLanguageWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            string filename = Directory.GetCurrentDirectory() + "\\Config\\config.json";
            var lang = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filename));

            switch (LangCombox.SelectedIndex)
            {
                case 0:
                    SaveLanguageConfig(lang,"zh_cn",filename);
                    break;
                case 1:
                     SaveLanguageConfig(lang,"en_us",filename);
                    break;
                default:
                    MessageBox.Show(LocationServices.GetLang("UnSelectLang"));
                    return;
            }
        }

        private void SaveLanguageConfig(Dictionary<string, string> lang, string type, string path)
        {
            if (lang["lang"] == type)
            {
                MessageBox.Show(LocationServices.GetLang("UnchangeSelectLang"));
            }
            else
            {
                lang["lang"] = type;
                string result = JsonConvert.SerializeObject(lang);
                File.WriteAllText(path, result);
                MessageBox.Show(LocationServices.GetLang("ChangeSelectLang"));
                Environment.Exit(0);
            }
        }
    }
}
