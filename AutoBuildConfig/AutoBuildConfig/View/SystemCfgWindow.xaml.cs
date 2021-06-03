﻿using System;
using System.Collections.Generic;
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

namespace AutoBuildConfig.View
{
    /// <summary>
    /// SystemCfgWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SystemCfgWindow : Window
    {
        public SystemCfgWindow()
        {
            InitializeComponent();
        }

        private void Config_Close_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}