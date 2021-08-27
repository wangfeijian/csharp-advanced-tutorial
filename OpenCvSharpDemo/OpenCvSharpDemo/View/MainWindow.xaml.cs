using System;
using System.Diagnostics;

namespace OpenCvSharpDemo.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
