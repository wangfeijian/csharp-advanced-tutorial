using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private System.Windows.Controls.Image movingObject;  // 记录当前被拖拽移动的图片
        private System.Windows.Point StartPosition; // 本次移动开始时的坐标点位置
        private System.Windows.Point EndPosition;   // 本次移动结束时的坐标点位置

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

        private void ShowBorder_OnMouseEnter(object sender, MouseEventArgs e)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "cursor.cur";
            if (RadioButtonHand.IsChecked == true)
            {
                ShowImage.Cursor = new Cursor(fileName);
                ShowBorder.Cursor = new Cursor(fileName);
            }
            else
            {
                ShowImage.Cursor = Cursors.Arrow;
                ShowBorder.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// 按下鼠标左键，准备开始拖动图片
        /// </summary>
        private void ShowImage_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (RadioButtonHand.IsChecked == false)
            {
                return;
            }
            System.Windows.Controls.Image img = sender as System.Windows.Controls.Image;

            movingObject = img;
            StartPosition = e.GetPosition(img);
        }

        /// <summary>
        /// 鼠标左键弹起，结束图片的拖动
        /// </summary>
        private void ShowImage_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (RadioButtonHand.IsChecked == false)
            {
                return;
            }
            movingObject = null;
        }

        /// <summary>
        /// 按住鼠标左键，拖动图片平移
        /// </summary>
        private void ShowImage_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (RadioButtonHand.IsChecked == false)
            {
                return;
            }
            System.Windows.Controls.Image img = sender as System.Windows.Controls.Image;
            if (e.LeftButton == MouseButtonState.Pressed && sender == movingObject)
            {
                EndPosition = e.GetPosition(img);

                TransformGroup tg = img.RenderTransform as TransformGroup;
                var tgnew = tg.CloneCurrentValue();
                if (tgnew != null)
                {
                    TranslateTransform tt = tgnew.Children[0] as TranslateTransform;

                    var X = EndPosition.X - StartPosition.X;
                    var Y = EndPosition.Y - StartPosition.Y;
                    tt.X += X;
                    tt.Y += Y;
                }

                // 重新给图像赋值Transform变换属性
                img.RenderTransform = tgnew;
            }
        }

        /// <summary>
        /// 图片放大
        /// </summary>
        private void ButtonZoomIn_OnClick(object sender, RoutedEventArgs e)
        {
            if (ShowImage == null)
            {
                return;
            }
            TransformGroup tg = ShowImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                ScaleTransform st = tgnew.Children[1] as ScaleTransform;
                ShowImage.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                if (st.ScaleX > 0 && st.ScaleX <= 2.0)
                {
                    st.ScaleX += 0.05;
                    st.ScaleY += 0.05;
                }
                else if (st.ScaleX < 0 && st.ScaleX >= -2.0)
                {
                    st.ScaleX -= 0.05;
                    st.ScaleY += 0.05;
                }
            }

            // 重新给图像赋值Transform变换属性
            ShowImage.RenderTransform = tgnew;
        }

        /// <summary>
        /// 图片缩小
        /// </summary>
        private void ButtonZoomOut_OnClick(object sender, RoutedEventArgs e)
        {
            if (ShowImage == null)
            {
                return;
            }

            TransformGroup tg = ShowImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                ScaleTransform st = tgnew.Children[1] as ScaleTransform;
                ShowImage.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                if (st.ScaleX >= 0.2)
                {
                    st.ScaleX -= 0.05;
                    st.ScaleY -= 0.05;
                }
                else if (st.ScaleX <= -0.2)
                {
                    st.ScaleX += 0.05;
                    st.ScaleY -= 0.05;
                }
            }

            // 重新给图像赋值Transform变换属性
            ShowImage.RenderTransform = tgnew;
        }

        private void ShowImage_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ShowImage == null)
            {
                return;
            }
            TransformGroup tg = ShowImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                ScaleTransform st = tgnew.Children[1] as ScaleTransform;
                System.Windows.Point centerPoint = e.GetPosition(ShowBorder);

                st.CenterX = centerPoint.X;
                st.CenterY = centerPoint.Y;

                if (st.ScaleX < 0.1 && st.ScaleY < 0.1 && e.Delta < 0)
                {
                    return;
                }
                st.ScaleX += (double)e.Delta / 1000;
                st.ScaleY += (double)e.Delta / 1000;

            }

            // 重新给图像赋值Transform变换属性
            ShowImage.RenderTransform = tgnew;
        }
    }
}
