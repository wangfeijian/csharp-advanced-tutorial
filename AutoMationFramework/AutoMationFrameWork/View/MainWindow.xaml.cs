/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-06-22                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Window for main back code                *
*********************************************************************/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommonTools;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Lv.SelectedIndex = 0;
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var listItem = sender as ListViewItem;

            if (listItem != null)
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

            DragMove();
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (TgBtn.IsChecked == true)
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
            TgBtn.IsChecked = false;
        }

        private void WindowsState_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn != null)
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
                        if (MessageBox.Show(LocationServices.GetLang("SaveLayout"), LocationServices.GetLang("Tips"),
                                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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

        private void LV_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                // ListView拦截鼠标滚轮事件
                e.Handled = true;

                // 激发一个鼠标滚轮事件，冒泡给外层ListView接收到
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                if (parent != null) parent.RaiseEvent(eventArg);
            }
        }

        private void StartButton_OnSelected(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            StartLabel.Foreground = new SolidColorBrush(Color.FromRgb(196,196,199));

            PauseButton.IsEnabled = true;
            PauseLabel.Foreground = new SolidColorBrush(Color.FromRgb(105,31,255)); //"#691fff"

            StopButton.IsEnabled = true;
            StopLabel.Foreground = new SolidColorBrush(Color.FromRgb(255,0,0)); //"#ff0000"
        }

        private void PauseButton_OnSelected(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StartLabel.Foreground = new SolidColorBrush(Color.FromRgb(0,101,105));// "#006569";

            PauseButton.IsEnabled = false;
            PauseLabel.Foreground = new SolidColorBrush(Color.FromRgb(196,196,199));

            StopButton.IsEnabled = true;
            StopLabel.Foreground = new SolidColorBrush(Color.FromRgb(255,0,0));
        }

        private void StopButton_OnSelected(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StartLabel.Foreground = new SolidColorBrush(Color.FromRgb(0,101,105));// "#006569";

            PauseButton.IsEnabled = false;
            PauseLabel.Foreground = new SolidColorBrush(Color.FromRgb(196,196,199));

            StopButton.IsEnabled = false;
            StopLabel.Foreground = new SolidColorBrush(Color.FromRgb(196,196,199));
        }

        private void SelectLanguage(object sender, RoutedEventArgs e)
        {
            SelectLanguageWindow selectLanguage = new SelectLanguageWindow();
            selectLanguage.ShowDialog();
        }
    }
}
