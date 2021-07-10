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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// CameraControl.xaml 的交互逻辑
    /// </summary>
    public partial class CameraControl : UserControl
    {
        public CameraControl()
        {
            InitializeComponent();
        }

        private void HideButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (HideBorder.Width<=0)
            {
                HideBorder.Width = 300;
                HideButton.Margin = new Thickness(-10,10,0,0);
                HideButtonIcon.Kind = PackIconKind.ChevronRightBoxOutline;
            }
            else
            {
                HideBorder.Width = 0;
                HideButton.Margin = new Thickness(-25, 10, 0, 0);
                HideButtonIcon.Kind = PackIconKind.ChevronLeftBoxOutline;
            }
        }
    }
}
