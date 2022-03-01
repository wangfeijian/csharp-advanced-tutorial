using System.ComponentModel;
using System.Windows;

namespace SosoVisionTool.Views
{
    /// <summary>
    /// Interaction logic for OutputParamSetToolWindow.xaml
    /// </summary>
    public partial class OutputParamSetToolWindow : Window
    {
        public OutputParamSetToolWindow()
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
