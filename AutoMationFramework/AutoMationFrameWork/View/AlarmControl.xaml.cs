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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AvalonDock.Themes;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// AlarmControl.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmControl 
    {
        public AlarmControl()
        {
            InitializeComponent();
            this.dockingManager.Theme = new Vs2013LightTheme();
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            var test = this.dockingManager;
            Screen3.Hide();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Screen2.Show();
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            Screen3.Show();
        }
    }
}
