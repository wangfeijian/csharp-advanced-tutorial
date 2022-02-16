using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HalconDotNet;

namespace ImageDisplay
{
    public partial class ImageDisplayWindow : UserControl
    {
        public ImageDisplayWindow()
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
            DependencyProperty.Register("CameraColor", typeof(string), typeof(ImageDisplayWindow));

        public HObject DisplayImage
        {
            get { return (HObject)GetValue(DisplayImageProperty); }
            set { SetValue(DisplayImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CameraColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayImageProperty =
            DependencyProperty.Register("DisplayImage", typeof(HObject), typeof(ImageDisplayWindow));

        public HObject DisplayRegion
        {
            get { return (HObject)GetValue(DisplayRegionProperty); }
            set { SetValue(DisplayRegionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CameraColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayRegionProperty =
            DependencyProperty.Register("DisplayRegion", typeof(HObject), typeof(ImageDisplayWindow));

        /// <summary>
        /// 加载Halcon图像选择工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowImage_OnHInitWindow(object sender, EventArgs e)
        {
            ShowImage.HMoveContent = false;
            HWindow h = ShowImage.HalconWindow;
            if (h != null)
            {
                HOperatorSet.SetWindowParam(h, "background_color", CameraColor);
                HOperatorSet.ClearWindow(h);
            }
        }

        /// <summary>
        /// 右键菜单点击，选择和拖动切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;

            if (menu == null)
            {
                return;
            }

            switch (menu.Header.ToString())
            {
                case "指针":
                    RadioButtonPoint.IsChecked = !menu.IsChecked;
                    RadioButtonHand.IsChecked = !RadioButtonPoint.IsChecked;
                    ShowImage_OnMouseEnter(null, null);
                    break;
                case "平移":
                    RadioButtonHand.IsChecked = !menu.IsChecked;
                    RadioButtonPoint.IsChecked = !RadioButtonHand.IsChecked;
                    ShowImage_OnMouseEnter(null, null);
                    break;
                case "放大":
                    ZoomButton_Click(ButtonZoom, null);
                    break;
                case "缩小":
                    ZoomButton_Click(ButtonNarrow, null);
                    break;
            }
        }

        /// <summary>
        /// 当鼠标进入时，根据模式切换鼠标样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowImage_OnMouseEnter(object sender, MouseEventArgs e)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "Fonts/cursor.cur";
            if (ShowImage.HMoveContent)
            {
                ShowImage.Cursor = new Cursor(fileName);
            }
            else
            {
                ShowImage.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// 放大缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomButton_Click(object sender, RoutedEventArgs e)
        {
            Button temp = sender as Button;
            int scale = temp.ToolTip.ToString() == "放大" ? 10 : -10;
            ShowImage.HZoomWindowContents(ShowImage.ActualWidth / 2, ShowImage.ActualHeight / 2, scale);
        }

        /// <summary>
        /// 适应屏幕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullShowImage(object sender, RoutedEventArgs e)
        {
            ShowImage.SetFullImagePart();
        }

        /// <summary>
        /// 显示鼠标所在的像素位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowImage_OnHMouseMove(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {
            HTuple homMat2D = new HTuple();
            HTuple imageX = new HTuple(), imageY = new HTuple();
            HTuple showX = new HTuple { [0] = 0, [1] = 0, [2] = ShowImage.ActualHeight};
            HTuple showY = new HTuple { [0] = 0, [1] = ShowImage.ActualWidth, [2] = 0 };
            HTuple partX = new HTuple
            {
                [0] = 0,
                [1] = 0,
                [2] = ShowImage.HImagePart.Height,
            };
            HTuple partY = new HTuple
            {
                [0] = 0,
                [1] = ShowImage.HImagePart.Width,
                [2] = 0,
            };
            HOperatorSet.VectorToHomMat2d(showX, showY, partX, partY, out homMat2D);
            HOperatorSet.AffineTransPoint2d(homMat2D, e.X, e.Y, out imageX, out imageY);

            double posX = imageX.D + ShowImage.HImagePart.Left;
            double posY = imageY.D + ShowImage.HImagePart.Top;
            TextBlockPos.Text = $"X:{(int)posX},  Y:{(int)posY}";

            homMat2D.Dispose();
            imageX.Dispose();
            imageY.Dispose();
            showX.Dispose();
            showY.Dispose();
            partX.Dispose();
            partY.Dispose();
        }
    }
}
