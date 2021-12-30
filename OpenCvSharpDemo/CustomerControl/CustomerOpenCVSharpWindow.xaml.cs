using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Image = System.Drawing.Image;
using Path = System.Windows.Shapes.Path;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using StrokeCollection = System.Windows.Ink.StrokeCollection;

namespace CustomerControl
{
    /// <summary>
    /// 绘制模式
    /// </summary>
    enum DrawMode
    {
        /// <summary>
        /// 任意形状
        /// </summary>
        Any,
        /// <summary>
        /// 线
        /// </summary>
        Line,
        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle,
        /// <summary>
        /// 椭圆
        /// </summary>
        Ellipse
    }
    /// <summary>
    /// CustomerOpenCVSharpWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CustomerOpenCVSharpWindow : UserControl
    {
        private System.Windows.Controls.Image movingObject;  // 记录当前被拖拽移动的图片
        private System.Windows.Point StartPosition; // 本次移动开始时的坐标点位置
        private System.Windows.Point EndPosition;   // 本次移动结束时的坐标点位置
        private InkCanvasEditingMode _inkCanvasEditingMode;
        private DrawMode _drawMode;
        private System.Windows.Point _drawPoint;
        private bool _isMove;
        private Stroke _stroke;

        public CustomerOpenCVSharpWindow()
        {
            InitializeComponent();
            StrokeCollections = new StrokeCollection();
            _inkCanvasEditingMode = InkCanvasEditingMode.Ink;
            _drawMode = DrawMode.Any;
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



        public StrokeCollection StrokeCollections
        {
            get { return (StrokeCollection)GetValue(StrokeCollectionsProperty); }
            set { SetValue(StrokeCollectionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeCollections.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeCollectionsProperty =
            DependencyProperty.Register(nameof(StrokeCollection), typeof(StrokeCollection), typeof(CustomerOpenCVSharpWindow));


        public Brush CameraColor
        {
            get { return (Brush)GetValue(CameraColorProperty); }
            set { SetValue(CameraColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CameraColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CameraColorProperty =
            DependencyProperty.Register(nameof(CameraColor), typeof(Brush), typeof(CustomerOpenCVSharpWindow));

        public WriteableBitmap ShowImageBitmap
        {
            get { return (WriteableBitmap)GetValue(ShowImageBitmapProperty); }
            set { SetValue(ShowImageBitmapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowImageBitmap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowImageBitmapProperty =
            DependencyProperty.Register(nameof(ShowImageBitmap), typeof(WriteableBitmap), typeof(CustomerOpenCVSharpWindow));

        public WriteableBitmap SaveImageBitmap
        {
            get { return (WriteableBitmap)GetValue(SaveImageBitmapProperty); }
            set { SetValue(SaveImageBitmapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveImageBitmap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveImageBitmapProperty =
            DependencyProperty.Register(nameof(SaveImageBitmap), typeof(WriteableBitmap), typeof(CustomerOpenCVSharpWindow));


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
                InkCanvasImage.RenderTransform = tgnew;
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
            InkCanvasImage.RenderTransform = tgnew;
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
            InkCanvasImage.RenderTransform = tgnew;
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
            InkCanvasImage.RenderTransform = tgnew;
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
            InkCanvasImage.RenderTransform = tgnew;
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
            InkCanvasImage.RenderTransform = tgnew;
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
            InkCanvasImage.RenderTransform = tgnew;
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
            else
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
        private Color GetPixel(WriteableBitmap wbm, int x, int y, bool flag = true)
        {
            if (y > wbm.PixelHeight - 1 || x > wbm.PixelWidth - 1) return Color.FromRgb(0, 0, 0);
            if (y < 0 || x < 0) return Color.FromRgb(0, 0, 0);
            IntPtr buff = wbm.BackBuffer;
            int Stride = wbm.BackBufferStride;
            Color c;
            unsafe
            {
                byte* pbuff = (byte*)buff.ToPointer();
                int loc = flag ? y * Stride + x * 3 : y * Stride + x;
                c = flag ? Color.FromRgb(pbuff[loc + 2], pbuff[loc + 1], pbuff[loc]) : Color.FromRgb(pbuff[loc], pbuff[loc], pbuff[loc]);
            }
            return c;
        }

        /// <summary>
        /// 保存原始图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveOriginImage(object sender, RoutedEventArgs e)
        {
            if (SaveImageBitmap == null)
            {
                MessageBox.Show("未加载任何图片");
                return;
            }

            try
            {
                SaveImageToFile(SaveImageBitmap);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        /// <summary>
        /// 保存图片到文件
        /// </summary>
        /// <param name="image"></param>
        private void SaveImageToFile(WriteableBitmap image)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                DefaultExt = "jpg",
                Filter = "jpg files(*.jpg)|*.jpg|png files(*.png)|*.png|bmp files(*.bmp)|*.bmp"
            };

            if (sfd.ShowDialog() == true)
            {
                string fileName = sfd.FileName;
                BitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
                switch (fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf(".")))
                {
                    case "jpg":
                        bitmapEncoder = new JpegBitmapEncoder();
                        break;
                    case "png":
                        bitmapEncoder = new PngBitmapEncoder();
                        break;
                    case "bmp":
                        bitmapEncoder = new BmpBitmapEncoder();
                        break;
                }

                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, PixelFormats.Default);
                DrawingVisual drawingVisual = new DrawingVisual();
                using (var dc = drawingVisual.RenderOpen())
                {
                    dc.DrawImage(image, new Rect(0, 0, image.Width, image.Height));
                }
                renderTargetBitmap.Render(drawingVisual);

                bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                using (FileStream file = File.OpenWrite(fileName))
                {
                    bitmapEncoder.Save(file);
                }
            }
        }

        /// <summary>
        /// 保存截图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveProcessImage(object sender, RoutedEventArgs e)
        {
            if (ShowImageBitmap == null)
            {
                MessageBox.Show("图片不存在，未加载！");
                return;
            }

            try
            {
                SaveImageToFile(ShowImageBitmap);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        /// <summary>
        /// 初始化InkCanvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitInkCanvas(object sender, RoutedEventArgs e)
        {
            if (CheckBoxInkCanvasEnable.IsChecked != true)
            {
                return;
            }

            InkCanvasImage.EditingMode = _inkCanvasEditingMode;
            SetInkCanvasDrawingAttributes();
        }

        private void SetInkCanvasDrawingAttributes()
        {
            if (InkCanvasImage == null)
            {
                return;
            }

            DrawingAttributes drawingAttributes = new DrawingAttributes
            {
                Width = 1,
                Height = 1,
                IgnorePressure = true,
                IsHighlighter = false,
                Color = ((SolidColorBrush)((ComboBoxItem)ColorComboBox.SelectedItem).Background).Color
            };

            if (_inkCanvasEditingMode == InkCanvasEditingMode.Ink)
            {
                if (_drawMode == DrawMode.Rectangle)
                {
                    drawingAttributes.StylusTip = StylusTip.Rectangle;
                    _inkCanvasEditingMode = InkCanvasEditingMode.None;
                }
                else if (_drawMode == DrawMode.Ellipse || _drawMode == DrawMode.Line)
                {
                    drawingAttributes.StylusTip = StylusTip.Ellipse;
                    _inkCanvasEditingMode = InkCanvasEditingMode.None;
                }
            }

            InkCanvasImage.EditingMode = _inkCanvasEditingMode;
            InkCanvasImage.DefaultDrawingAttributes = drawingAttributes.Clone();
        }

        private void DrawModeSelect(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton.IsChecked == true)
            {
                int index = Convert.ToInt32(radioButton.Tag);
                _drawMode = (DrawMode)index;
            }

            RadioButtonEdit.IsChecked = true;
            InkCanvasEditingModeSelect(RadioButtonEdit, null);
            SetInkCanvasDrawingAttributes();
        }

        private void ColorComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox.Name != "ColorComboBox")
            {
                return;
            }

            SetInkCanvasDrawingAttributes();
        }

        private void InkCanvasEditingModeSelect(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            InkCanvasImage.UseCustomCursor = radioButton.Tag.ToString() == "1";
            if (radioButton.IsChecked == true)
            {
                int index = Convert.ToInt32(radioButton.Tag);
                _inkCanvasEditingMode = (InkCanvasEditingMode)index;
            }

            SetInkCanvasDrawingAttributes();
        }

        private void InkCanvasImage_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _drawPoint = e.GetPosition(InkCanvasImage);
                _isMove = true;
            }
        }

        private void InkCanvasImage_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMove || _inkCanvasEditingMode != InkCanvasEditingMode.None)
            {
                return;
            }

            System.Windows.Point drawEndPoint = e.GetPosition(InkCanvasImage);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_drawMode == DrawMode.Rectangle)
                {
                    List<System.Windows.Point> points = new List<System.Windows.Point>
                    {
                        new System.Windows.Point(_drawPoint.X,_drawPoint.Y),
                        new System.Windows.Point(_drawPoint.X, drawEndPoint.Y),
                        new System.Windows.Point(drawEndPoint.X, drawEndPoint.Y),
                        new System.Windows.Point(drawEndPoint.X,_drawPoint.Y),
                        new System.Windows.Point(_drawPoint.X,_drawPoint.Y)
                    };

                    StylusPointCollection point = new StylusPointCollection(points);

                    Stroke stroke = new Stroke(point)
                    {
                        DrawingAttributes = InkCanvasImage.DefaultDrawingAttributes.Clone()
                    };

                    if (_stroke != null)
                        StrokeCollections.Remove(_stroke);

                    if (stroke != null)
                        StrokeCollections.Add(stroke);

                    _stroke = stroke;
                }
                else if (_drawMode == DrawMode.Ellipse)
                {
                    List<System.Windows.Point> points = GetEclipsePoints(_drawPoint, drawEndPoint);
                    StylusPointCollection point = new StylusPointCollection(points);

                    Stroke stroke = new Stroke(point)
                    {
                        DrawingAttributes = InkCanvasImage.DefaultDrawingAttributes.Clone()
                    };

                    if (_stroke != null)
                        StrokeCollections.Remove(_stroke);

                    if (stroke != null)
                        StrokeCollections.Add(stroke);

                    _stroke = stroke;
                }
                else if (_drawMode == DrawMode.Line)
                {
                    List<System.Windows.Point> pointList = new List<System.Windows.Point>
                    {
                        new System.Windows.Point(_drawPoint.X, _drawPoint.Y),
                        new System.Windows.Point(drawEndPoint.X, drawEndPoint.Y),
                    };

                    StylusPointCollection point = new StylusPointCollection(pointList);
                    Stroke stroke = new Stroke(point)
                    {
                        DrawingAttributes = InkCanvasImage.DefaultDrawingAttributes.Clone()
                    };
                    if (_stroke != null)
                        StrokeCollections.Remove(_stroke);

                    if (stroke != null)
                        StrokeCollections.Add(stroke);

                    _stroke = stroke;
                }
            }
        }

        /// <summary>
        /// 获取椭圆点的位置
        /// </summary>
        /// <param name="stPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private List<System.Windows.Point> GetEclipsePoints(System.Windows.Point stPoint, System.Windows.Point endPoint)
        {
            double a = 0.5 * (endPoint.X - stPoint.X);
            double b = 0.5 * (endPoint.Y - stPoint.Y);
            List<System.Windows.Point> points = new List<System.Windows.Point>();

            for (double r = 0; r <= 2 * Math.PI; r = r + 0.01)
            {
                points.Add(new System.Windows.Point(0.5 * (stPoint.X + endPoint.X) + a * Math.Cos(r), 0.5 * (stPoint.Y + endPoint.Y) + b * Math.Sin(r)));
            }

            return points;
        }
        private void InkCanvasImage_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_drawMode == DrawMode.Any || _inkCanvasEditingMode != InkCanvasEditingMode.None)
            {
                return;
            }

            StrokeCollections.RemoveAt(StrokeCollections.Count - 1);
            Stroke newStroke = _stroke.Clone();
            StrokeCollections.Add(newStroke);
            _isMove = false;
        }

        private void MenuItemEditRoi_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBoxInkCanvasEnable.IsChecked = !MenuItemEditRoi.IsChecked;
            InitInkCanvas(null, null);
        }

        private void DrawModeMenuItemOnClick(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;

            if (menu == null)
            {
                return;
            }

            switch (menu.Tag.ToString())
            {
                case "0":
                    SetMenuItemDrawMode(RadioButtonDrawAny, RadioButtonDrawLine, RadioButtonDrawCircle, !menu.IsChecked, RadioButtonDrawRectangle);
                    DrawModeSelect(RadioButtonDrawAny, null);
                    break;
                case "1":
                    SetMenuItemDrawMode(RadioButtonDrawLine, RadioButtonDrawAny, RadioButtonDrawCircle, !menu.IsChecked, RadioButtonDrawRectangle);
                    DrawModeSelect(RadioButtonDrawLine, null);
                    break;
                case "2":
                    SetMenuItemDrawMode(RadioButtonDrawRectangle, RadioButtonDrawLine, RadioButtonDrawCircle, !menu.IsChecked, RadioButtonDrawAny);
                    DrawModeSelect(RadioButtonDrawRectangle, null);
                    break;
                case "3":
                    SetMenuItemDrawMode(RadioButtonDrawCircle, RadioButtonDrawLine, RadioButtonDrawAny, !menu.IsChecked, RadioButtonDrawRectangle);
                    DrawModeSelect(RadioButtonDrawCircle, null);
                    break;
            }

        }

        private void EditModeMenuItemOnClick(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;

            if (menu == null)
            {
                return;
            }

            switch (menu.Tag.ToString())
            {
                case "0":
                    SetMenuItemDrawMode(RadioButtonEdit, RadioButtonSelect, RadioButtonDelete, !menu.IsChecked);
                    InkCanvasEditingModeSelect(RadioButtonEdit, null);
                    break;
                case "1":
                    SetMenuItemDrawMode(RadioButtonSelect, RadioButtonEdit, RadioButtonDelete, !menu.IsChecked);
                    InkCanvasEditingModeSelect(RadioButtonSelect, null);
                    break;
                case "2":
                    SetMenuItemDrawMode(RadioButtonDelete, RadioButtonEdit, RadioButtonSelect, !menu.IsChecked);
                    InkCanvasEditingModeSelect(RadioButtonDelete, null);
                    break;
            }

        }

        private void SetMenuItemDrawMode(RadioButton select, RadioButton otherOne, RadioButton otherTwo, bool flag = false, RadioButton otherThree = null)
        {
            select.IsChecked = flag;
            otherOne.IsChecked = !select.IsChecked;
            otherTwo.IsChecked = !select.IsChecked;
            if (otherThree != null)
            {
                otherThree.IsChecked = !select.IsChecked;
            }
        }

    }
}
