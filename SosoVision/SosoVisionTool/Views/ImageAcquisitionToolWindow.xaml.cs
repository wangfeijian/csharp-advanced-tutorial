using SosoVisionTool.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace SosoVisionTool.Views
{
    /// <summary>
    /// Interaction logic for ImageAcquisitionToolWindow.xaml
    /// </summary>
    public partial class ImageAcquisitionToolWindow : Window
    {
        public TreeViewItem ToolTreeViewItem { get; set; }
        public ImageAcquisitionTool AcquisitionTool { get; set; }
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
