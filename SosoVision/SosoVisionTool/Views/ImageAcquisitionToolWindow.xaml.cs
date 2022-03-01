using System.ComponentModel;
using System.Windows;

namespace SosoVisionTool.Views
{
    /// <summary>
    /// Interaction logic for ImageAcquisitionToolWindow.xaml
    /// </summary>
    public partial class ImageAcquisitionToolWindow : Window
    {
        public ImageAcquisitionToolWindow()
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
