using System;
using System.Collections.Generic;
using System.Drawing;
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
using Color = System.Windows.Media.Color;

namespace CustomerControl
{
    /// <summary>
    /// CustomerOpenCVSharpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CustomerOpenCVSharpWindow : UserControl
    {
        public CustomerOpenCVSharpWindow()
        {
            InitializeComponent();
        }

        public string CameraColor
        {
            get { return (string)GetValue(CameraColorProperty); }
            set { SetValue(CameraColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CameraColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CameraColorProperty =
            DependencyProperty.Register("CameraColor", typeof(string), typeof(CustomerOpenCVSharpWindow), new PropertyMetadata("#E5B881"));



        public WriteableBitmap ShowImageBitmap
        {
            get { return (WriteableBitmap)GetValue(ShowImageBitmapProperty); }
            set { SetValue(ShowImageBitmapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowImageBitmap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowImageBitmapProperty =
            DependencyProperty.Register("ShowImageBitmap", typeof(WriteableBitmap), typeof(CustomerOpenCVSharpWindow), new PropertyMetadata(null));
    }
}
