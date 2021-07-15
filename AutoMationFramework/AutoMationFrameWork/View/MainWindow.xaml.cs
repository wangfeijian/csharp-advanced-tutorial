using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ListViewItem = System.Windows.Controls.ListViewItem;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LV.SelectedIndex = 0;
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var listItem = sender as ListViewItem;

            switch (listItem.Tag.ToString())
            {
                case "Main":
                    SelectControl("MainUi");
                    break;
                case "Set":
                    SelectControl("SetUi");
                    break;
                case "Camera":
                    SelectControl("CameraUi");
                    break;
                case "Table":
                    SelectControl("TableUi");
                    break;
                case "Alarm":
                    SelectControl("AlarmUi");
                    break;
                case "User":
                    SelectControl("UserUi");
                    break;

            }
        }

        private void SelectControl(string name)
        {
            foreach (var child in MainGrid.Children)
            {
                var control = child as UserControl;

                if (control != null)
                {
                    control.Visibility = control.Name == name ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private void BorderTitle_OnMouseDown(object sender, MouseButtonEventArgs e)
        {

            this.DragMove();
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Tg_Btn.IsChecked == true)
            {
                tt_alarm.Visibility = Visibility.Collapsed;
                tt_camera.Visibility = Visibility.Collapsed;
                tt_home.Visibility = Visibility.Collapsed;
                tt_lang.Visibility = Visibility.Collapsed;
                tt_pause.Visibility = Visibility.Collapsed;
                tt_settings.Visibility = Visibility.Collapsed;
                tt_start.Visibility = Visibility.Collapsed;
                tt_stop.Visibility = Visibility.Collapsed;
                tt_table.Visibility = Visibility.Collapsed;
                tt_user.Visibility = Visibility.Collapsed;
            }
            else
            {
                tt_alarm.Visibility = Visibility.Visible;
                tt_camera.Visibility = Visibility.Visible;
                tt_home.Visibility = Visibility.Visible;
                tt_lang.Visibility = Visibility.Visible;
                tt_pause.Visibility = Visibility.Visible;
                tt_settings.Visibility = Visibility.Visible;
                tt_start.Visibility = Visibility.Visible;
                tt_stop.Visibility = Visibility.Visible;
                tt_table.Visibility = Visibility.Visible;
                tt_user.Visibility = Visibility.Visible;
            }
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false;
        }

        private void WindowsState_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            switch (btn.Name)
            {
                case "MinBtn":
                    if (WindowState != WindowState.Minimized)
                        WindowState = WindowState.Minimized;
                    break;
                case "MaxBtn":
                    if (WindowState != WindowState.Maximized)
                    {
                        WindowState = WindowState.Maximized;
                        btn.Content = "\xe65b";
                    }
                    else
                    {
                        WindowState = WindowState.Normal;
                        btn.Content = "\xe62b";
                    }
                    break;
                case "CloseBtn":
                    if (MessageBox.Show("是否保存布局", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        Close();
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                    break;
            }
        }

        private void MainWindow_Unloaded(object sender, EventArgs e)
        {
            MainUi.SaveMainControlLayout();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainUi.LoadMainControlLayout();
        }
    }
}
