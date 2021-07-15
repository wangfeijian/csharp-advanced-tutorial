using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AvalonDock.Controls;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using UserControl = System.Windows.Controls.UserControl;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// MainControl.xaml 的交互逻辑
    /// </summary>
    public partial class MainControl : UserControl
    {
        private PerformanceCounter m_CpuCounter;
        private PerformanceCounter m_RamCounter;
        private PerformanceCounter m_GcCounter;
        private string CpuStr;
        private string RamStr;
        private string Ram2Str;

        public MainControl()
        {
            InitializeComponent();

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
            var openFile = new OpenFileDialog { DefaultExt = ".config", Filter = "Config File (.config)|*.config" };
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
                Filter = "Config File (.config)|*.config"
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
            string file = AppDomain.CurrentDomain.BaseDirectory + "AvalonDock.config";
            LoadLayout(file);
        }

        private void LoadLayout(string file)
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
            serializer.LayoutSerializationCallback += (s, args) => { args.Content = args.Content; };

            if (File.Exists(file))
                serializer.Deserialize(file);
        }

        public void SaveMainControlLayout()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "AvalonDock.config";
            SaveLayout(file);
        }

        private void SaveLayout(string file)
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
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

            string strTime = DateTime.Now.ToString("yyyy-MM-dd dddd hh:mm:ss");
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
            var layout = dockingManager.Layout.Descendents().OfType<LayoutAnchorable>();
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
    }
}
