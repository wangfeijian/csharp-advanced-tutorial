using System;
using System.Collections.Generic;
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

        public string FillStyle
        {
            get { return (string)GetValue(FillStyleProperty); }
            set { SetValue(FillStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CameraColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillStyleProperty =
            DependencyProperty.Register("FillStyle", typeof(string), typeof(ImageDisplayWindow));

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
        /// 画出来的对象
        /// </summary>
        private List<HTuple> _drawObjects;

        private object _lock = new object();

        /// <summary>
        /// 加载Halcon图像选择工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowImage_OnHInitWindow(object sender, EventArgs e)
        {
            FillStyle = "fill";
            _drawObjects = new List<HTuple>();
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
                case "区域边框":
                    RadioButtonMargin.IsChecked = !menu.IsChecked;
                    RadioButtonFill.IsChecked = !RadioButtonMargin.IsChecked;
                    RadioButtonFill_OnClick(null, null);
                    break;
                case "区域填充":
                    RadioButtonFill.IsChecked = !menu.IsChecked;
                    RadioButtonMargin.IsChecked = !RadioButtonFill.IsChecked;
                    RadioButtonFill_OnClick(null, null);
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
            ShowImageScale();
        }

        /// <summary>
        /// 适应屏幕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullShowImage(object sender, RoutedEventArgs e)
        {
            ShowImage.SetFullImagePart();
            ShowImageScale();
        }

        /// <summary>
        /// 显示鼠标所在的像素位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowImage_OnHMouseMove(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {
            if (DisplayImage == null)
            {
                return;
            }

            HTuple homMat2D = new HTuple();
            HTuple imageX = new HTuple(), imageY = new HTuple();
            HTuple showX = new HTuple { [0] = 0, [1] = 0, [2] = ShowImage.ActualHeight };
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

            int posX = (int)(imageX.D + ShowImage.HImagePart.Left);
            int posY = (int)(imageY.D + ShowImage.HImagePart.Top);
            TextBlockPos.Text = $"X:{posX},  Y:{posY}";
            ShowImageScale();

            HTuple channels = new HTuple();
            HOperatorSet.CountChannels(DisplayImage, out channels);
            if (channels[0].I == 1)
            {
                ShowImagePixel(posY, posX, false);
            }
            else
            {
                ShowImagePixel(posY, posX);
            }

            homMat2D.Dispose();
            imageX.Dispose();
            imageY.Dispose();
            showX.Dispose();
            showY.Dispose();
            partX.Dispose();
            partY.Dispose();
            channels.Dispose();
        }

        private void ShowImagePixel(int row, int col, bool isColor = true)
        {
            HTuple width = new HTuple();
            HTuple height = new HTuple();
            HOperatorSet.GetImageSize(DisplayImage, out width, out height);

            if (row < 0 || row >= height || col < 0 || col >= width)
            {
                width.Dispose();
                height.Dispose();
                return;
            }

            if (isColor)
            {
                HObject rImg = new HObject();
                HObject gImg = new HObject();
                HObject bImg = new HObject();
                HOperatorSet.Decompose3(DisplayImage, out rImg, out gImg, out bImg);

                HTuple rVal = new HTuple();
                HTuple gVal = new HTuple();
                HTuple bVal = new HTuple();
                HOperatorSet.GetGrayval(rImg, row, col, out rVal);
                HOperatorSet.GetGrayval(gImg, row, col, out gVal);
                HOperatorSet.GetGrayval(bImg, row, col, out bVal);

                TextBlockPixel.Text = $"R: {rVal}  G： {gVal}  B: {bVal}";

                rImg.Dispose();
                gImg.Dispose();
                bImg.Dispose();
                rVal.Dispose();
                gVal.Dispose();
                bVal.Dispose();
            }
            else
            {
                HTuple grayVal = new HTuple();
                HOperatorSet.GetGrayval(DisplayImage, row, col, out grayVal);

                TextBlockPixel.Text = grayVal.ToString();

                grayVal.Dispose();
            }

            width.Dispose();
            height.Dispose();
        }

        private void ShowImageScale()
        {
            double scale = ShowImage.ActualWidth / ShowImage.HImagePart.Width * 100;
            TextBlockScale.Text = $"缩放: {scale:f2} %";
        }

        private void ShowImage_OnHMouseWheel(object sender, HSmartWindowControlWPF.HMouseEventArgsWPF e)
        {
            ShowImageScale();
        }


        private void ButtonDrawLine_OnClick(object sender, RoutedEventArgs e)
        {
            Button temp = sender as Button;
            string index = temp.Tag.ToString();

            DrawObjectToWindow(index);
        }

        /// <summary>
        /// 向窗口画图形
        /// </summary>
        /// <param name="index">类型</param>
        private void DrawObjectToWindow(string index)
        {
            HTuple draw = new HTuple();

            switch (index)
            {
                case "1":
                    HOperatorSet.CreateDrawingObjectLine(100, 100, 200, 200, out draw);
                    HOperatorSet.SetDrawingObjectParams(draw, "line_width", 1);
                    break;
                case "2":
                    HOperatorSet.CreateDrawingObjectRectangle2(200, 200, 0, 100, 100, out draw);
                    break;
                case "3":
                    HOperatorSet.CreateDrawingObjectCircle(200, 200, 50, out draw);
                    break;
                case "4":
                    HOperatorSet.CreateDrawingObjectEllipse(200, 200, 0, 100, 60, out draw);
                    break;
            }

            HOperatorSet.AttachDrawingObjectToWindow(ShowImage.HalconWindow, draw);
            _drawObjects.Add(draw);
        }

        private void MenuItemDraw_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem temp = sender as MenuItem;

            string index = temp.Tag.ToString();

            DrawObjectToWindow(index);
        }

        /// <summary>
        /// 将绘制的图形转换成区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            Button temp = sender as Button;
            string index = temp.Tag.ToString();
            CollectionEdit(index);
        }

        private void CollectionEdit(string index)
        {
            HTuple param;
            HObject region;
            HOperatorSet.GenEmptyObj(out region);

            if (_drawObjects.Count > 0)
            {
                foreach (var drawingObject in _drawObjects)
                {
                    HObject unionRegion;
                    HOperatorSet.GenEmptyObj(out unionRegion);
                    HObject tempRegion;
                    HOperatorSet.GetDrawingObjectIconic(out tempRegion, drawingObject);
                    HOperatorSet.GetDrawingObjectParams(drawingObject, "line_width", out param);
                    if (param == 1)
                    {
                        region = tempRegion;
                        DisplayRegion = region;
                        param.Dispose();
                        ClearAllObjects();
                        return;
                    }

                    if (index == "0" || index == "2" || index == "3")
                    {
                        HOperatorSet.Union2(region, tempRegion, out unionRegion);
                    }
                    else
                    {
                        HObject equalRegion;
                        HTuple isEqual;
                        HOperatorSet.GenEmptyObj(out equalRegion);
                        HOperatorSet.TestEqualObj(equalRegion, region, out isEqual);

                        if (isEqual)
                        {
                            HOperatorSet.Union2(region, tempRegion, out unionRegion);
                        }
                        else
                        {
                            HOperatorSet.Intersection(region, tempRegion, out unionRegion);
                        }

                    }

                    region = unionRegion;
                    DisplayRegion = region;
                }

                if (index == "3")
                {
                    HObject interRegion;
                    HOperatorSet.Complement(DisplayRegion, out interRegion);
                    DisplayRegion = interRegion;
                }
            }

            HOperatorSet.Connection(DisplayRegion, out region);
            DisplayRegion = region;
            ClearAllObjects();
        }

        private void ClearAllObjects()
        {
            lock (_lock)
            {
                foreach (HTuple dobj in _drawObjects)
                {
                    HOperatorSet.ClearDrawingObject(dobj);
                }
                _drawObjects.Clear();
            }
        }

        private void RadioButtonFill_OnClick(object sender, RoutedEventArgs e)
        {
            FillStyle = RadioButtonFill.IsChecked == true ? "fill" : "margin";
        }

        private void CheckBoxDrawEnable_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItemDraw.IsEnabled = CheckBoxDrawEnable.IsChecked == true;
            MenuItemEdit.IsEnabled = CheckBoxDrawEnable.IsChecked == true;
            MenuItemDrawFill.IsEnabled = CheckBoxDrawEnable.IsChecked == true;
        }

        private void MenuItemEditRoi_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBoxDrawEnable.IsChecked = !CheckBoxDrawEnable.IsChecked;
            CheckBoxDrawEnable_OnClick(null, null);
        }

        private void MenuItemCollection_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem temp = sender as MenuItem;
            string index = temp.Tag.ToString();
            CollectionEdit(index);
        }

        private void ButtonClearRegion_OnClick(object sender, RoutedEventArgs e)
        {
            HObject tempRegion;
            HOperatorSet.GenEmptyObj(out tempRegion);
            DisplayRegion = tempRegion;

            tempRegion.Dispose();
        }
    }
}
