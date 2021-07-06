using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HalconDotNet;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// ImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageControl : UserControl
    {
        public ImageControl()
        {
            InitializeComponent();
        }

        private void SetHalconWindow()
        {
            HWindow h = HSmartWindow.HalconWindow;
            if (h != null)
            {
                HOperatorSet.SetWindowParam(h,"background_color", "#50bc93");
                HOperatorSet.ClearWindow(h);
            }
        }

        private void HSmartWindow_HInitWindow(object sender, EventArgs e)
        {
            SetHalconWindow();
        }

    }
}
