using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
using Image = System.Drawing.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;

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

        private double xPos;

        public double XPos
        {
            get { return xPos; }
            set { xPos = value; }
        }

        private double yPos;

        public double YPos
        {
            get { return yPos; }
            set { yPos = value; }
        }

        public double ScaleSize { get; set; }

        public string MousePos => $"X:{XPos:f2},  Y:{YPos:f2}";
        public string ScaleSizeString => $"缩放:{ScaleSize:f2}";

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
            DependencyProperty.Register("ShowImageBitmap", typeof(WriteableBitmap), typeof(CustomerOpenCVSharpWindow));


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
            System.Windows.Point point = e.GetPosition(ShowImage);
            double scale = ShowImage.ActualWidth / ShowImageBitmap.PixelWidth;
            XPos = point.X / scale;
            YPos = point.Y / scale;
            TextBlockPos.Text = MousePos;
            ShowPixel();

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
            double scale = 1;
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

                scale = st.ScaleX;
            }

            // 重新给图像赋值Transform变换属性
            ShowImage.RenderTransform = tgnew;
            ShowScaleSize(scale);
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
            double scale = 1;
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

                scale = st.ScaleX;
            }

            // 重新给图像赋值Transform变换属性
            ShowImage.RenderTransform = tgnew;
            ShowScaleSize(scale);
        }

        private void ShowImage_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ShowImage == null)
            {
                return;
            }
            TransformGroup tg = ShowImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            double scale = 1;
            if (tgnew != null)
            {
                ScaleTransform st = tgnew.Children[1] as ScaleTransform;
                System.Windows.Point centerPoint = e.GetPosition(ShowImage);

                st.CenterX = centerPoint.X;
                st.CenterY = centerPoint.Y;

                if (st.ScaleX < 0.1 && st.ScaleY < 0.1 && e.Delta < 0)
                {
                    return;
                }
                st.ScaleX += (double)e.Delta / 1000;
                st.ScaleY += (double)e.Delta / 1000;
                scale = st.ScaleX;
            }

            // 重新给图像赋值Transform变换属性
            ShowImage.RenderTransform = tgnew;
            ShowScaleSize(scale);
        }

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
                    ShowBorder_OnMouseEnter(null, null);
                    break;
                case "平移":
                    RadioButtonHand.IsChecked = !menu.IsChecked;
                    RadioButtonPoint.IsChecked = !RadioButtonHand.IsChecked;
                    ShowBorder_OnMouseEnter(null, null);
                    break;
            }
        }

        /// <summary>
        /// 图片适应窗口显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AdaptWindow(object sender, RoutedEventArgs e)
        {
            if (ShowImage == null)
            {
                return;
            }
            TransformGroup tg = ShowImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                TranslateTransform tt = tgnew.Children[0] as TranslateTransform;
                ScaleTransform st = tgnew.Children[1] as ScaleTransform;
                System.Windows.Point point = ShowBorder.PointToScreen(new System.Windows.Point(0, 0));

                tt.X = 0;
                tt.Y = 0;
                st.CenterX = point.X;
                st.CenterY = point.Y;
                st.ScaleX = 1;
                st.ScaleY = 1;
            }

            // 重新给图像赋值Transform变换属性
            ShowImage.RenderTransform = tgnew;
            ShowScaleSize(1);
        }

        /// <summary>
        /// 图片按缩放比例显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScalingWindow(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            if (ShowImage == null || menu == null)
            {
                return;
            }

            double scale = Convert.ToDouble(menu.Tag);
            TransformGroup tg = ShowImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                ScaleTransform st = tgnew.Children[1] as ScaleTransform;

                st.CenterX = ShowImage.ActualWidth / 2;
                st.CenterY = ShowImage.ActualHeight / 2;
                st.ScaleX = scale;
                st.ScaleY = scale;
            }

            // 重新给图像赋值Transform变换属性
            ShowImage.RenderTransform = tgnew;
            ShowScaleSize(scale);
        }

        private void ShowScaleSize(double scale)
        {
            WriteableBitmap image = ShowImage.Source as WriteableBitmap;
            ScaleSize = ShowImage.ActualWidth * scale / image.PixelWidth * 100;
            TextBlockScale.Text = ScaleSizeString;
        }

        /// <summary>
        /// 图片按像素比例显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScalingPixelWindow(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            Button radioButton = sender as Button;
            if (ShowImage == null || (menu == null && radioButton == null))
            {
                return;
            }

            WriteableBitmap image = ShowImage.Source as WriteableBitmap;
            double scale = menu == null ? Convert.ToDouble(radioButton.Tag) : Convert.ToDouble(menu.Tag);
            scale = scale * (image.PixelWidth / ShowImage.ActualWidth);
            TransformGroup tg = ShowImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                ScaleTransform st = tgnew.Children[1] as ScaleTransform;

                st.CenterX = ShowImage.ActualWidth / 2;
                st.CenterY = ShowImage.ActualHeight / 2;
                st.ScaleX = scale;
                st.ScaleY = scale;
            }

            // 重新给图像赋值Transform变换属性
            ShowImage.RenderTransform = tgnew;
            ShowScaleSize(scale);
        }

        /// <summary>
        /// 在窗口底部显示像素值 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ShowPixel()
        {
            if (ShowImageBitmap.Format == PixelFormats.Gray8)
            {
                Color c = GetPixel(ShowImageBitmap, (int)XPos, (int)YPos, false);
                TextBlockPixel.Text = c.B.ToString();
            }
            else if (ShowImageBitmap.Format == PixelFormats.Bgr24)
            {
                Color c = GetPixel(ShowImageBitmap, (int)XPos, (int)YPos);
                TextBlockPixel.Text = $"R:{c.R},G:{c.G},B:{c.B}";
            }
        }

        /// <summary>
        /// 获取像素值
        /// </summary>
        /// <param name="wbm"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="flag">是否为彩色图片</param>
        /// <returns></returns>
        private Color GetPixel(WriteableBitmap wbm, int x, int y , bool flag=true)
        {
            if (y > wbm.PixelHeight - 1 || x > wbm.PixelWidth - 1) return Color.FromArgb(0, 0, 0, 0);
            if (y < 0 || x < 0) return Color.FromArgb(0, 0, 0, 0);
            IntPtr buff = wbm.BackBuffer;
            int Stride = wbm.BackBufferStride;
            Color c;
            unsafe
            {
                byte* pbuff = (byte*)buff.ToPointer();
                int loc = flag ? y * Stride + x * 3 : y * Stride + x;
                c = Color.FromArgb(pbuff[loc + 3], pbuff[loc + 2], pbuff[loc + 1], pbuff[loc]);
            }
            return c;
        }
    }
}
