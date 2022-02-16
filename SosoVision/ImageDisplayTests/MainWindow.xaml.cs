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
using HalconDotNet;

namespace ImageDisplayTests
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private HObject testHObject;
        public MainWindow()
        {
            InitializeComponent();
            HOperatorSet.ReadImage(out testHObject,"D:/Desktop Image/1.jpg");
            ShowImage.DisplayImage = testHObject;
        }
    }
}
