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
using HalconDotNet;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// ImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageControl 
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
                HOperatorSet.SetWindowParam(h,"background_color", "#E5B881");
                HOperatorSet.ClearWindow(h);
            }
        }

        private void HSmartWindow_HInitWindow(object sender, EventArgs e)
        {
            SetHalconWindow();
        }

    }
}
