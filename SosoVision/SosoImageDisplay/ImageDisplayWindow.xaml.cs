using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HalconDotNet;
using Microsoft.Win32;
using Prism.Ioc;
using Prism.Events;

namespace ImageDisplay
{
    public partial class ImageDisplayWindow
    {
        public IEventAggregator EventAggregator { get; }
        public ImageDisplayWindow()
        {
            EventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
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

        public string RegionColor
        {
            get { return (string)GetValue(RegionColorProperty); }
            set { SetValue(RegionColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RegionColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegionColorProperty =
            DependencyProperty.Register("RegionColor", typeof(string), typeof(ImageDisplayWindow));

        public string DisplayMessage
        {
            get { return (string)GetValue(DisplayMessageProperty); }
            set { SetValue(DisplayMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayMessage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayMessageProperty =
            DependencyProperty.Register("DisplayMessage", typeof(string), typeof(ImageDisplayWindow), new PropertyMetadata(null,OnDisplayMessageChange));

        private static void OnDisplayMessageChange(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ImageDisplayWindow imageDisplay = source as ImageDisplayWindow;
            if (imageDisplay == null)
            {
                return;
            }
            if (imageDisplay.GetHalconWindow()==null)
            {
                return;
            }

            string name = e.Property.Name;
            if (!(name == "DisplayMessage"))
            {
                return;
            }
            else
            {
                imageDisplay.SetDisplayFont(imageDisplay.GetHalconWindow(), 16, "mono", "true", "false");
                if (imageDisplay.MessageColor == null)
                {
                    imageDisplay.DispMessage(imageDisplay.GetHalconWindow(), (string)e.NewValue, "window", 25, 25, "black", "false");
                }
                else
                {
                    imageDisplay.DispMessage(imageDisplay.GetHalconWindow(), (string)e.NewValue, "window", 25, 25, imageDisplay.MessageColor, "false");
                }
            }
        }

        public string MessageColor
        {
            get { return (string)GetValue(MessageColorProperty); }
            set { SetValue(MessageColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RegionColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageColorProperty =
            DependencyProperty.Register("MessageColor", typeof(string), typeof(ImageDisplayWindow));


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

            try
            {
                HWindow h = ShowImage.HalconWindow;
                if (h != null)
                {
                    HOperatorSet.SetWindowParam(h, "background_color", CameraColor);
                    HOperatorSet.ClearWindow(h);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
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

        public HWindow GetHalconWindow()
        {
            return ShowImage.HalconWindow;
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
            if (temp == null)
            {
                return;
            }
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
            try
            {
                if (DisplayImage == null)
                {
                    return;
                }

                HTuple homMat2D;
                HTuple imageX, imageY;
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

                HTuple channels;
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
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void ShowImagePixel(int row, int col, bool isColor = true)
        {
            try
            {
                HTuple width, height;
                HOperatorSet.GetImageSize(DisplayImage, out width, out height);

                if (row < 0 || row >= height || col < 0 || col >= width)
                {
                    width.Dispose();
                    height.Dispose();
                    return;
                }

                if (isColor)
                {
                    HObject rImg, gImg, bImg;
                    HOperatorSet.Decompose3(DisplayImage, out rImg, out gImg, out bImg);

                    HTuple rVal, gVal, bVal;
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
                    HTuple grayVal;
                    HOperatorSet.GetGrayval(DisplayImage, row, col, out grayVal);

                    TextBlockPixel.Text = grayVal.ToString();

                    grayVal.Dispose();
                }

                width.Dispose();
                height.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
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
            if (temp == null)
            {
                return;
            }
            string index = temp.Tag.ToString();

            DrawObjectToWindow(index);
        }

        /// <summary>
        /// 向窗口画图形
        /// </summary>
        /// <param name="index">类型</param>
        private void DrawObjectToWindow(string index)
        {
            try
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
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void MenuItemDraw_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem temp = sender as MenuItem;
            if (temp == null)
            {
                return;
            }
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
            if (temp == null)
            {
                return;
            }
            string index = temp.Tag.ToString();
            CollectionEdit(index);
        }

        private void CollectionEdit(string index)
        {
            try
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
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
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
            MenuItemRegionColor.IsEnabled = CheckBoxDrawEnable.IsChecked == true;
        }

        private void MenuItemEditRoi_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBoxDrawEnable.IsChecked = !CheckBoxDrawEnable.IsChecked;
            CheckBoxDrawEnable_OnClick(null, null);
        }

        private void MenuItemCollection_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem temp = sender as MenuItem;
            if (temp == null)
            {
                return;
            }
            string index = temp.Tag.ToString();
            CollectionEdit(index);
        }

        private void ButtonClearRegion_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                HObject tempRegion;
                HOperatorSet.GenEmptyObj(out tempRegion);
                DisplayRegion = tempRegion;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        /// <summary>
        /// 保存图片到文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileExt"></param>
        private bool SaveImageToFile(out string fileName, out string fileExt)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                DefaultExt = "jpg",
                Filter = "jpeg files(*.jpeg)|*.jpeg|tiff files(*.tiff)|*.tiff|bmp files(*.bmp)|*.bmp"
            };

            if (sfd.ShowDialog() == true)
            {
                fileName = sfd.FileName;
                fileExt = Path.GetExtension(fileName).Substring(1, Path.GetExtension(fileName).Length - 1);
                return true;
            }

            fileName = "";
            fileExt = "";
            return false;
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            Button temp = sender as Button;
            if (temp == null)
            {
                return;
            }

            if (DisplayImage == null)
            {
                MessageBox.Show("未加载任何图片，无法保存");
                return;
            }

            string fileName, fileExt;
            if (!SaveImageToFile(out fileName, out fileExt))
                return;

            try
            {
                if (temp.Name == "ButtonSave")
                {
                    HOperatorSet.WriteImage(DisplayImage, fileExt, 0, fileName);
                }
                else
                {
                    HObject tempObject;
                    HOperatorSet.DumpWindowImage(out tempObject, ShowImage.HalconWindow);
                    HOperatorSet.WriteImage(tempObject, fileExt, 0, fileName);
                    tempObject.Dispose();
                }

                MessageBox.Show("保存成功！！");
            }
            catch (Exception exception)
            {
                MessageBox.Show("保存失败！！");
                MessageBox.Show(exception.ToString());
            }
        }

        private void MenuItemRegionColor_OnClick(object sender, RoutedEventArgs e)
        {
            MenuItem temp = sender as MenuItem;
            if (temp == null)
            {
                return;
            }

            RegionColor = temp.Header.ToString();
        }

        public void DispMessage(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
    HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_GenParamName = new HTuple(), hv_GenParamValue = new HTuple();
            HTuple hv_Color_COPY_INP_TMP = new HTuple(hv_Color);
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_CoordSystem_COPY_INP_TMP = new HTuple(hv_CoordSystem);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);

            // Initialize local and output iconic variables 
            try
            {
                //This procedure displays text in a graphics window.
                //
                //Input parameters:
                //WindowHandle: The WindowHandle of the graphics window, where
                //   the message should be displayed.
                //String: A tuple of strings containing the text messages to be displayed.
                //CoordSystem: If set to 'window', the text position is given
                //   with respect to the window coordinate system.
                //   If set to 'image', image coordinates are used.
                //   (This may be useful in zoomed images.)
                //Row: The row coordinate of the desired text position.
                //   You can pass a single value or a tuple of values.
                //   See the explanation below.
                //   Default: 12.
                //Column: The column coordinate of the desired text position.
                //   You can pass a single value or a tuple of values.
                //   See the explanation below.
                //   Default: 12.
                //Color: defines the color of the text as string.
                //   If set to [] or '' the currently set color is used.
                //   If a tuple of strings is passed, the colors are used cyclically
                //   for every text position defined by Row and Column,
                //   or every new text line in case of |Row| == |Column| == 1.
                //Box: A tuple controlling a possible box surrounding the text.
                //   Its entries:
                //   - Box[0]: Controls the box and its color. Possible values:
                //     -- 'true' (Default): An orange box is displayed.
                //     -- 'false': No box is displayed.
                //     -- color string: A box is displayed in the given color, e.g., 'white', '#FF00CC'.
                //   - Box[1] (Optional): Controls the shadow of the box. Possible values:
                //     -- 'true' (Default): A shadow is displayed in
                //               darker orange if Box[0] is not a color and in 'white' otherwise.
                //     -- 'false': No shadow is displayed.
                //     -- color string: A shadow is displayed in the given color, e.g., 'white', '#FF00CC'.
                //
                //It is possible to display multiple text strings in a single call.
                //In this case, some restrictions apply on the
                //parameters String, Row, and Column:
                //They can only have either 1 entry or n entries.
                //Behavior in the different cases:
                //   - Multiple text positions are specified, i.e.,
                //       - |Row| == n, |Column| == n
                //       - |Row| == n, |Column| == 1
                //       - |Row| == 1, |Column| == n
                //     In this case we distinguish:
                //       - |String| == n: Each element of String is displayed
                //                        at the corresponding position.
                //       - |String| == 1: String is displayed n times
                //                        at the corresponding positions.
                //   - Exactly one text position is specified,
                //      i.e., |Row| == |Column| == 1:
                //      Each element of String is display in a new textline.
                //
                //
                //Convert the parameters for disp_text.
                if ((int)((new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
                {

                    hv_Color_COPY_INP_TMP.Dispose();
                    hv_Column_COPY_INP_TMP.Dispose();
                    hv_CoordSystem_COPY_INP_TMP.Dispose();
                    hv_Row_COPY_INP_TMP.Dispose();
                    hv_GenParamName.Dispose();
                    hv_GenParamValue.Dispose();

                    return;
                }
                if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
                {
                    hv_Row_COPY_INP_TMP.Dispose();
                    hv_Row_COPY_INP_TMP = 12;
                }
                if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
                {
                    hv_Column_COPY_INP_TMP.Dispose();
                    hv_Column_COPY_INP_TMP = 12;
                }
                //
                //Convert the parameter Box to generic parameters.
                hv_GenParamName.Dispose();
                hv_GenParamName = new HTuple();
                hv_GenParamValue.Dispose();
                hv_GenParamValue = new HTuple();
                if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(0))) != 0)
                {
                    if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleEqual("false"))) != 0)
                    {
                        //Display no box
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "box");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    "false");
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual(
                        "true"))) != 0)
                    {
                        //Set a color other than the default.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "box_color");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    hv_Box.TupleSelect(0));
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                }
                if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(1))) != 0)
                {
                    if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleEqual("false"))) != 0)
                    {
                        //Display no shadow.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "shadow");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    "false");
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                    else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual(
                        "true"))) != 0)
                    {
                        //Set a shadow color other than the default.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                    "shadow_color");
                                hv_GenParamName.Dispose();
                                hv_GenParamName = ExpTmpLocalVar_GenParamName;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                    hv_Box.TupleSelect(1));
                                hv_GenParamValue.Dispose();
                                hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                            }
                        }
                    }
                }
                //Restore default CoordSystem behavior.
                if ((int)(new HTuple(hv_CoordSystem_COPY_INP_TMP.TupleNotEqual("window"))) != 0)
                {
                    hv_CoordSystem_COPY_INP_TMP.Dispose();
                    hv_CoordSystem_COPY_INP_TMP = "image";
                }
                //
                if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    //disp_text does not accept an empty string for Color.
                    hv_Color_COPY_INP_TMP.Dispose();
                    hv_Color_COPY_INP_TMP = new HTuple();
                }
                //
                HOperatorSet.DispText(hv_WindowHandle, hv_String, hv_CoordSystem_COPY_INP_TMP,
                    hv_Row_COPY_INP_TMP, hv_Column_COPY_INP_TMP, hv_Color_COPY_INP_TMP, hv_GenParamName,
                    hv_GenParamValue);

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Text
        // Short Description: Set font independent of OS 
        public void SetDisplayFont(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
            HTuple hv_Bold, HTuple hv_Slant)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = new HTuple(), hv_Fonts = new HTuple();
            HTuple hv_Style = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_AvailableFonts = new HTuple(), hv_Fdx = new HTuple();
            HTuple hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = new HTuple(hv_Font);
            HTuple hv_Size_COPY_INP_TMP = new HTuple(hv_Size);

            // Initialize local and output iconic variables 
            try
            {
                //This procedure sets the text font of the current window with
                //the specified attributes.
                //
                //Input parameters:
                //WindowHandle: The graphics window for which the font will be set
                //Size: The font size. If Size=-1, the default of 16 is used.
                //Bold: If set to 'true', a bold font is used
                //Slant: If set to 'true', a slanted font is used
                //
                hv_OS.Dispose();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    hv_Size_COPY_INP_TMP.Dispose();
                    hv_Size_COPY_INP_TMP = 16;
                }
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    //Restore previous behaviour
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                else
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = hv_Size_COPY_INP_TMP.TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Courier";
                    hv_Fonts[1] = "Courier 10 Pitch";
                    hv_Fonts[2] = "Courier New";
                    hv_Fonts[3] = "CourierNew";
                    hv_Fonts[4] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Consolas";
                    hv_Fonts[1] = "Menlo";
                    hv_Fonts[2] = "Courier";
                    hv_Fonts[3] = "Courier 10 Pitch";
                    hv_Fonts[4] = "FreeMono";
                    hv_Fonts[5] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Luxi Sans";
                    hv_Fonts[1] = "DejaVu Sans";
                    hv_Fonts[2] = "FreeSans";
                    hv_Fonts[3] = "Arial";
                    hv_Fonts[4] = "Liberation Sans";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Times New Roman";
                    hv_Fonts[1] = "Luxi Serif";
                    hv_Fonts[2] = "DejaVu Serif";
                    hv_Fonts[3] = "FreeSerif";
                    hv_Fonts[4] = "Utopia";
                    hv_Fonts[5] = "Liberation Serif";
                }
                else
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple(hv_Font_COPY_INP_TMP);
                }
                hv_Style.Dispose();
                hv_Style = "";
                if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Bold";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Italic";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
                {
                    hv_Style.Dispose();
                    hv_Style = "Normal";
                }
                hv_AvailableFonts.Dispose();
                HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);
                hv_Font_COPY_INP_TMP.Dispose();
                hv_Font_COPY_INP_TMP = "";
                for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
                {
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_AvailableFonts.TupleFind(
                            hv_Fonts.TupleSelect(hv_Fdx));
                    }
                    if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                        {
                            hv_Font_COPY_INP_TMP.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(
                                    hv_Fdx);
                            }
                            break;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    throw new HalconException("Wrong value of control parameter Font");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Font = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
                        hv_Font_COPY_INP_TMP.Dispose();
                        hv_Font_COPY_INP_TMP = ExpTmpLocalVar_Font;
                    }
                }
                HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                throw HDevExpDefaultException;
            }
        }
    }
}
