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
using System.Windows.Shapes;

namespace OpenCvSharpTool
{
    /// <summary>
    /// ImageAcquisitionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImageAcquisitionWindow : Window,IExecuteTool
    {
        public CaptureType AcquisitionType { get; set; }
        public ToolBase ImageGrabTool { get; set; }
        public string FileName { get; set; }
        public string DirName { get; set; }
        public object CameraType { get; set; }
        public bool IsContinuousAcquisition { get; set; }
        public bool IsColorImage { get; set; }
        public bool IsVideo { get; set; }
        public ImageAcquisitionWindow()
        {
            InitializeComponent();
        }

        public void Execute()
        {
            
        }
    }
}
