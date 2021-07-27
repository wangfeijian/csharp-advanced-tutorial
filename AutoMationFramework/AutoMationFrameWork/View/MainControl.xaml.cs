/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-06-22                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    UserControl for main back code           *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using CommonTools;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// MainControl.xaml 的交互逻辑
    /// </summary>
    public partial class MainControl 
    {
        private PerformanceCounter m_CpuCounter;
        private PerformanceCounter m_RamCounter;
        private PerformanceCounter m_GcCounter;
        private string CpuStr;
        private string RamStr;
        private string Ram2Str;
        private Dictionary<string, string> lang;

        public MainControl()
        {
            InitializeComponent();

            string filename = Directory.GetCurrentDirectory() + "\\Config\\config.json";
            lang = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filename));
            string strProcessName = Process.GetCurrentProcess().ProcessName;
            m_CpuCounter = new PerformanceCounter("Process", "% Processor Time", strProcessName);
            m_RamCounter = new PerformanceCounter("Process", "Working Set - Private", strProcessName);
            m_GcCounter = new PerformanceCounter(".NET CLR Memory", "% Time in GC", strProcessName);

            var showTimer = new DispatcherTimer();
            showTimer.Tick += ShowTime;
            showTimer.Interval = new TimeSpan(0, 0, 0, 1);
            showTimer.Start();
            LogRightButton.Click += LogRightButton_OnClick;
            MechineRightButton.Click += MechineRightButton_OnClick;
            SystemRightButton.Click += SystemRightButton_OnClick;
            LoadRightButton.Click += LoadLayoutFromFile;
            SaveRightButton.Click += SaveLayoutToFile;
        }

        private void LoadLayoutFromFile(object sender, RoutedEventArgs e)
        {
            var openFile = new OpenFileDialog
            {
                DefaultExt = ".config",
                Filter = "Config File (.config)|*.config",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory+"Config\\",
            };
            if (openFile.ShowDialog() == true)
            {
                string file = openFile.FileName;
                try
                {
                    LoadLayout(file);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"加载失败！请确定文件是否为layout配置文件！{exception}");
                    return;
                }
                MessageBox.Show("加载成功！");
            }
            else
            {
                MessageBox.Show("加载失败！未选择文件！");
            }
        }

        private void SaveLayoutToFile(object sender, RoutedEventArgs e)
        {
            var saveFile = new SaveFileDialog
            {
                DefaultExt = ".config",
                FileName = "AvalonDock",
                Filter = "Config File (.config)|*.config",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory+"Config\\",
            };


            if (saveFile.ShowDialog() == true)
            {
                string file = saveFile.FileName;
                SaveLayout(file);
                MessageBox.Show("保存成功！");
            }
            else
            {
                MessageBox.Show("保存失败！未指定文件！");
            }
        }

        public void LoadMainControlLayout()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "Config\\AvalonDock.config";
            LoadLayout(file);
        }

        private void LoadLayout(string file)
        {
            var serializer = new XmlLayoutSerializer(DockingManager);
            serializer.LayoutSerializationCallback += (s, args) => { args.Content = args.Content; };

            if (File.Exists(file))
                serializer.Deserialize(file);
        }

        public void SaveMainControlLayout()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "Config\\AvalonDock.config";
            SaveLayout(file);
        }

        private void SaveLayout(string file)
        {
            var serializer = new XmlLayoutSerializer(DockingManager);
            serializer.Serialize(file);
        }

        private void ShowTime(object sender, EventArgs e)
        {
            double cpu = m_CpuCounter.NextValue();
            double ram = m_RamCounter.NextValue();
            double gc = m_GcCounter.NextValue();
            string level = "B";

            ByteConvert(ref ram, ref level);

            CpuStr = cpu.ToString("F1") + "%";
            RamStr = $"{ram:F1}";
            Ram2Str = level;

            CpuProgressBar.Value = cpu;
            RamProgressBar.Value = gc;
            CpuTextBlock.Text = CpuStr;
            RamTextBlock.Text = RamStr;
            Ram2TextBlock.Text = Ram2Str;
            string strTime = "";
            if (lang["lang"] == "zh_cn")
            {
                strTime = DateTime.Now.ToString("yyyy-MM-dd dddd hh:mm:ss");
            }
            else if (lang["lang"] == "en_us")
            {
                string dayOfWeek = DateTime.Now.DayOfWeek.ToString().Substring(0, 3).ToUpper();
                string date = DateTime.Now.ToString("MM/dd/yy");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string now = $"{date} {dayOfWeek} {time}";
                strTime = now;
            }
            TimeTextBlock.Text = strTime;
        }

        private void ByteConvert(ref double v, ref string level)
        {
            switch (level)
            {
                case "B":
                    if (v / 1024 > 1)
                    {
                        level = "KB";
                        v /= 1024;
                        ByteConvert(ref v, ref level);
                    }
                    break;

                case "KB":
                    if (v / 1024 > 1)
                    {
                        level = "MB";
                        v /= 1024;
                        ByteConvert(ref v, ref level);
                    }
                    break;

                case "MB":
                    if (v / 1024 > 1)
                    {
                        level = "GB";
                        v /= 1024;
                        ByteConvert(ref v, ref level);
                    }
                    break;
            }
        }

        private void SystemRightButton_OnClick(object sender, RoutedEventArgs e)
        {
            ControlLayout(SystemRightButton.IsChecked, "SystemInfoLayoutId");
        }

        private void ControlLayout(bool isChecked, string contentId)
        {
            var layout = DockingManager.Layout.Descendents().OfType<LayoutAnchorable>();
            var layoutAnchrableValue = from value in layout
                                       where value.ContentId == contentId
                                       select value;

            if (isChecked)
            {
                layoutAnchrableValue.First().Show();
            }
            else
            {
                layoutAnchrableValue.First().Hide();
            }
        }

        private void MechineRightButton_OnClick(object sender, RoutedEventArgs e)
        {
            ControlLayout(MechineRightButton.IsChecked, "MechineInfoLayoutId");
        }

        private void LogRightButton_OnClick(object sender, RoutedEventArgs e)
        {
            ControlLayout(LogRightButton.IsChecked, "LogInfoLayoutId");
        }

        private void DockingManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            var layout = DockingManager.Layout.Descendents().OfType<LayoutAnchorable>();
            foreach (var layoutAnchorable in layout)
            {
                if (layoutAnchorable.ContentId == "SystemInfoLayoutId")
                {
                    layoutAnchorable.Title = LocationServices.GetLang("SystemInfo");
                }
                else if (layoutAnchorable.ContentId == "MechineInfoLayoutId")
                {
                    layoutAnchorable.Title = LocationServices.GetLang("MachineInfo");
                }
                else if (layoutAnchorable.ContentId == "LogInfoLayoutId")
                {
                    layoutAnchorable.Title = LocationServices.GetLang("Log");
                }
            }

            var layoutDoc = DockingManager.Layout.Descendents().OfType<LayoutDocument>();
            layoutDoc.First().Title = LocationServices.GetLang("MainControl");
        }
    }
}
