﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
    }
}