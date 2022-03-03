using System.ComponentModel;
using System.Windows;

namespace SosoVisionTool.Views
{
    /// <summary>
    /// Interaction logic for FindLineToolWindow.xaml
    /// </summary>
    public partial class FindLineToolWindow : Window
    {
        public FindLineToolWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
