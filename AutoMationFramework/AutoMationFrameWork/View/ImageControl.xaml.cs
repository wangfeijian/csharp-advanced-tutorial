/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-01                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    UserControl for image back code          *
*********************************************************************/
using System;
using System.Windows;
using HalconDotNet;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// ImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageControl 
    {


        public string CameraColor
        {
            get { return (string)GetValue(CameraColorProperty); }
            set { SetValue(CameraColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CameraColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CameraColorProperty =
            DependencyProperty.Register("CameraColor", typeof(string), typeof(ImageControl), new PropertyMetadata("#E5B881"));


        public ImageControl()
        {
            InitializeComponent();
        }

        private void SetHalconWindow()
        {
            HWindow h = HSmartWindow.HalconWindow;
            if (h != null)
            {
                HOperatorSet.SetWindowParam(h,"background_color", CameraColor);
                HOperatorSet.ClearWindow(h);
            }
        }

        private void HSmartWindow_HInitWindow(object sender, EventArgs e)
        {
            SetHalconWindow();
        }
    }
}
