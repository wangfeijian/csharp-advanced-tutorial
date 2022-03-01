using System.ComponentModel;
using System.Windows;

namespace SosoVisionTool.Views
{
    /// <summary>
    /// Interaction logic for ImageMatchToolWindw.xaml
    /// </summary>
    public partial class ImageMatchToolWindw : Window
    {
        public ImageMatchToolWindw()
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
