using System;
using System.Collections.Generic;
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
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            switch (btn.Tag.ToString())
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
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
