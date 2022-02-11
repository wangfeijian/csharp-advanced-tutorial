﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using SosoVision.Extensions;
using SosoVision.ViewModels;

namespace SosoVision.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        private SerializationData SerializationData { get; set; }
        public MainView()
        {
            InitializeComponent();
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;

            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void MenuDarkModeButton_Click(object sender, RoutedEventArgs e)
            => ModifyTheme(DarkModeToggleButton.IsChecked == true);

        private static void ModifyTheme(bool isDarkTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
        }

        private void MainView_OnClosing(object sender, CancelEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;

            if (viewModel != null)
            {
                SerializationData = new SerializationData { VisionTitle = viewModel.VisionStepCollection };
            }

            File.WriteAllText("config.json", JsonConvert.SerializeObject(SerializationData));
        }
    }
}
