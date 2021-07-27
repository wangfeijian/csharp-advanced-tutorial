/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-06-22                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    UserControl for camera back code         *
*********************************************************************/
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// CameraControl.xaml 的交互逻辑
    /// </summary>
    public partial class CameraControl
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
                HideButton.Margin = new Thickness(5,10,0,0);
                HideButtonIcon.Kind = PackIconKind.ChevronRightCircleOutline;
            }
            else
            {
                HideBorder.Width = 0;
                HideButton.Margin = new Thickness(-25, 10, 0, 0);
                HideButtonIcon.Kind = PackIconKind.ChevronLeftCircleOutline;
            }
        }
    }
}
