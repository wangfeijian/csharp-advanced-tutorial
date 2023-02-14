using CommunityToolkitDemoFramework.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using WPFDevelopers.Controls;
using WPFDevelopers.Helpers;

namespace CommunityToolkitDemoFramework.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ThemesCollectionProperty =
           DependencyProperty.Register("ThemesCollection", typeof(ObservableCollection<ThemeModel>), typeof(MainWindow),
               new PropertyMetadata(null));

        public ObservableCollection<ThemeModel> ThemesCollection
        {
            get => (ObservableCollection<ThemeModel>)GetValue(ThemesCollectionProperty);
            set => SetValue(ThemesCollectionProperty, value);
        }

        private void LightDark_Checked(object sender, RoutedEventArgs e)
        {
            var lightDark = sender as ToggleButton;
            if (lightDark == null) return;
            var theme = lightDark.IsChecked.Value ? ThemeType.Dark : ThemeType.Light;
            if (App.Theme == theme) return;
            App.Theme = theme;
            ControlsHelper.ToggleLightAndDark(lightDark.IsChecked == true);
        }
    }   
}
