/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-06-22                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    UserControl for alarm back code          *
*********************************************************************/
using System.Windows;
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
            DockingManager.Theme = new Vs2013LightTheme();
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
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
