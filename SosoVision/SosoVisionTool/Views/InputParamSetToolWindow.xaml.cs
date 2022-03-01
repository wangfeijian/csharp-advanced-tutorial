using System.ComponentModel;
using System.Windows;

namespace SosoVisionTool.Views
{
    /// <summary>
    /// Interaction logic for InputParamSetToolWindow.xaml
    /// </summary>
    public partial class InputParamSetToolWindow : Window
    {
        public InputParamSetToolWindow()
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
