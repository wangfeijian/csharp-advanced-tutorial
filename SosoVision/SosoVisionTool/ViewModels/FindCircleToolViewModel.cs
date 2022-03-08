using HalconDotNet;
using ImageCapture;
using Prism.Mvvm;
using Prism.Ioc;
using SosoVisionCommonTool.ConfigData;
using SosoVisionTool.Services;
using SosoVisionTool.Views;
using System;
using System.Collections.Generic;
using Prism.Commands;
using System.Windows;
using SosoVisionTool.Tools;
using Prism.Events;
using System.Windows.Controls;

namespace SosoVisionTool.ViewModels
{
    public class FindCircleToolViewModel : BindableBase, IToolBaseViewModel
    {
        [Newtonsoft.Json.JsonIgnore]
        public ToolRunViewData ToolRunData { get; set; }

        private IEventAggregator _eventAggregator;
        public string VisionStep { get; set; }
        public ProcedureParam Param { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand TestCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand AddRoiCommand { get; }
        [Newtonsoft.Json.JsonIgnore]
        public CaptureBase Capture { get; set; }
        public bool RoiAdded { get; set; }
        public double Row { get; set; }
        public double Col { get; set; }
        public double Radius { get; set; }

        private HObject _displayImage;

        public HObject DisplayImage
        {
            get { return _displayImage; }
            set { _displayImage = value; RaisePropertyChanged(); }
        }
        private HObject _displayRegion;

        public HObject DisplayRegion
        {
            get { return _displayRegion; }
            set { _displayRegion = value; RaisePropertyChanged(); }
        }

        private string _displayMessage;

        public string DisplayMessage
        {
            get { return _displayMessage; }
            set { _displayMessage = value; RaisePropertyChanged(); }
        }

        private string _messageColor;

        public string MessageColor
        {
            get { return _messageColor; }
            set { _messageColor = value; RaisePropertyChanged(); }
        }

        private string _imageSourceKey;

        public string ImageSourceKey
        {
            get { return _imageSourceKey; }
            set { _imageSourceKey = value; RaisePropertyChanged(); }
        }

        private bool _enableMat;

        public bool EnableMat
        {
            get { return _enableMat; }
            set { _enableMat = value; RaisePropertyChanged(); }
        }

        private double _measureLength = 20.0;

        public double MeasureLength
        {
            get { return _measureLength; }
            set
            {
                if (value > 511.0 || value < 1)
                {
                    value = 20.0;
                }
                _measureLength = value;
                RaisePropertyChanged();
            }
        }

        private double _measureWidth = 5.0;

        public double MeasureWidth
        {
            get { return _measureWidth; }
            set
            {
                if (value > 511.0 || value < 1)
                {
                    value = 5.0;
                }
                _measureWidth = value;
                RaisePropertyChanged();
            }
        }

        private double _measureSigma = 1.0;

        public double MeasureSigma
        {
            get { return _measureSigma; }
            set
            {
                if (value > 100.0 || value < 0.4)
                {
                    value = 1.0;
                }
                _measureSigma = value;
                RaisePropertyChanged();
            }
        }

        private double _measureThreshold = 30.0;

        public double MeasureThreshold
        {
            get { return _measureThreshold; }
            set
            {
                if (value > 255.0 || value < 1)
                {
                    value = 30.0;
                }
                _measureThreshold = value;
                RaisePropertyChanged();
            }
        }

        private int _measureNum = 10;

        public int MeasureNum
        {
            get { return _measureNum; }
            set
            {
                if (value > 100 || value < 0)
                {
                    value = 10;
                }
                _measureNum = value;
                RaisePropertyChanged();
            }
        }

        private string _measureTransition = "all";

        public string MeasureTransition
        {
            get { return _measureTransition; }
            set
            {
                _measureTransition = value;
                RaisePropertyChanged();
            }
        }

        private string _rowSourceKey;

        public string RowSourceKey
        {
            get { return _rowSourceKey; }
            set { _rowSourceKey = value; RaisePropertyChanged(); }
        }

        private string _colSourceKey;

        public string ColSourceKey
        {
            get { return _colSourceKey; }
            set { _colSourceKey = value; RaisePropertyChanged(); }
        }

        public double MatRow { get; set; }
        public double MatCol { get; set; }
        public HObject DrawRoi { get; set; }
        public List<string> MeasureTransitionList { get; set; }

        public void Run(ToolBase tool, ref bool result, ref string strResult)
        {
            if (DisplayImage == null)
            {
                result = false;
                return;
            }

            DisplayRegion = null;
            string key = $"{tool.ToolInVision}_{tool.ToolItem.Header}";
            HTuple row, col, radius;
            HObject outCircle;
            bool tempResult = FindCircle(out row, out col, out radius, out outCircle, false);

            string rowStr = SetMessage(nameof(row), row.ToString());
            string colStr = SetMessage(nameof(col), col.ToString());
            string radiusStr = SetMessage(nameof(radius), radius.ToString());

            string message;
            if (tempResult)
            {
                message = string.Join("\n", "OK", rowStr, colStr, radiusStr);
                ShowRunInfo(message);
            }
            else
            {
                message = string.Join("\n", "NG", rowStr, colStr, radiusStr);
                ShowRunInfo(message, false);
            }

            AddImageToData(key, DisplayImage);
            AddRegionToData(key, DisplayRegion);
            AddRegionToData($"{key}_{nameof(outCircle)}", outCircle.Clone());
            AddDoubleToData($"{key}_{nameof(row)}", rowStr);
            AddDoubleToData($"{key}_{nameof(col)}", colStr);
            AddDoubleToData($"{key}_{nameof(radius)}", radiusStr);

            HObjectParams tempHobjectCamera = new HObjectParams { Image = DisplayImage, VisionStep = tool.ToolInVision, ImageKey = key, Region = DisplayRegion, RegionKey = key };
            _eventAggregator.GetEvent<HObjectEvent>().Publish(tempHobjectCamera);

            TreeViewItem temp = new TreeViewItem { Header = nameof(DisplayImage), ToolTip = DisplayImage.ToString() };
            tool.AddInputOutputTree(temp, true);

            temp = new TreeViewItem { Header = nameof(RowSourceKey), ToolTip = ToolRunData.ToolOutputDoubleValue[RowSourceKey].ToString() };
            tool.AddInputOutputTree(temp, true);

            temp = new TreeViewItem { Header = nameof(ColSourceKey), ToolTip = ToolRunData.ToolOutputDoubleValue[ColSourceKey].ToString() };
            tool.AddInputOutputTree(temp, true);

            temp = new TreeViewItem { Header = nameof(row), ToolTip = row.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(col), ToolTip = col.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(radius), ToolTip = radius.ToString() };
            tool.AddInputOutputTree(temp, false);

            result = tempResult;

            row.Dispose();
            col.Dispose();
            radius.Dispose();
            outCircle.Dispose();
        }

        private void ShowRunInfo(string message, bool IsOk = true)
        {
            DisplayMessage = "";
            MessageColor = IsOk ? "green" : "red";
            DisplayMessage = message;
        }

        private string SetMessage(string name, string value)
        {
            string temp = string.Empty;

            if (value.Contains("["))
            {
                temp = value.Substring(1, value.Length - 2);
            }

            if (!string.IsNullOrWhiteSpace(temp) && temp.Contains(","))
            {
                var strArray = temp.Split(',');

                for (int i = 0; i < strArray.Length; i++)
                {
                    if (strArray[i].Contains("."))
                    {
                        strArray[i] = strArray[i].Substring(0, strArray[i].IndexOf('.') + 4);
                    }
                }

                return $"{name}: {string.Join(",", strArray)}";
            }

            if (value.Contains(".") && value.Length >= (value.IndexOf('.') + 3))
            {
                temp = value.Substring(0, value.IndexOf('.') + 3);
            }
            else
            {
                temp = value;
            }

            return $"{name}: {temp}";
        }

        private void AddImageToData(string key, HObject image)
        {
            if (ToolRunData.ToolOutputImage.ContainsKey(key))
            {
                ToolRunData.ToolOutputImage[key] = image;
                return;
            }

            ToolRunData.ToolOutputImage.Add(key, image);
        }

        private void AddRegionToData(string key, HObject region)
        {
            if (ToolRunData.ToolOutputRegion.ContainsKey(key))
            {
                ToolRunData.ToolOutputRegion[key] = region;
                return;
            }

            ToolRunData.ToolOutputRegion.Add(key, region);
        }

        private void AddStringToData(string key, string value)
        {
            if (ToolRunData.ToolOutputStringValue.ContainsKey(key))
            {
                ToolRunData.ToolOutputStringValue[key] = value;
                return;
            }

            ToolRunData.ToolOutputStringValue.Add(key, value);
        }

        private void AddDoubleToData(string key, string value)
        {
            string temp = value.Substring(value.IndexOf(":") + 1, value.Length - 1 - value.IndexOf(":"));
            double result;

            if (temp.Contains(","))
            {
                AddStringToData(key, temp);
                return;
            }

            if (!double.TryParse(temp, out result))
                result = 999.99;

            if (ToolRunData.ToolOutputDoubleValue.ContainsKey(key))
            {
                ToolRunData.ToolOutputDoubleValue[key] = result;
                return;
            }

            ToolRunData.ToolOutputDoubleValue.Add(key, result);
        }

        public FindCircleToolViewModel()
        {
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            TestCommand = new DelegateCommand(Test);
            AddRoiCommand = new DelegateCommand(AddRoi);
        }
        public FindCircleToolViewModel(string visionStep)
        {
            VisionStep = visionStep;
            ToolRunData = ContainerLocator.Container.Resolve<ToolRunViewData>(VisionStep);
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            TestCommand = new DelegateCommand(Test);
            AddRoiCommand = new DelegateCommand(AddRoi);

            MeasureTransitionList = new List<string>() { "all", "positive", "negative" };
        }

        private void Test()
        {
            HTuple row, col, radius;
            HObject circle;
            bool tempResult = FindCircle(out row, out col, out radius, out circle);

            string rowStr = SetMessage(nameof(row), row.ToString());
            string colStr = SetMessage(nameof(col), col.ToString());
            string radiusStr = SetMessage(nameof(radius), radius.ToString());

            string message;
            if (tempResult)
            {
                message = string.Join("\n", "OK", rowStr, colStr, radiusStr);
                ShowRunInfo(message);
            }
            else
            {
                message = string.Join("\n", "NG", rowStr, colStr, radiusStr);
                ShowRunInfo(message, false);
            }

            row.Dispose();
            col.Dispose();
            radius.Dispose();
            circle.Dispose();
        }

        private bool FindCircle(out HTuple row, out HTuple col, out HTuple radius, out HObject circle, bool isTest = true)
        {
            row = col = radius = 999;
            circle = null;

            if (ImageSourceKey == null)
            {
                if (isTest)
                    MessageBox.Show("请先选择图像源", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            DisplayImage = ToolRunData.ToolOutputImage[ImageSourceKey];

            if (!RoiAdded)
            {
                if (isTest)
                    MessageBox.Show("请先添加Roi", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (EnableMat)
                if (string.IsNullOrWhiteSpace(RowSourceKey) || string.IsNullOrWhiteSpace(ColSourceKey))
                {
                    if (isTest)
                        MessageBox.Show("请先绘制一个圆", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            HTuple homMat2D, metrologyHandle, index, measureRow, measureCol, usedRow, usedCol;
            HObject measureContours, crossContours, tempContours, cross;
            HOperatorSet.CreateMetrologyModel(out metrologyHandle);

            try
            {
                if (EnableMat)
                {
                    // 通过仿射拿到跟随的直线
                    HTuple circleRow, circleCol, circleParam = new HTuple();
                    HOperatorSet.VectorAngleToRigid(MatRow, MatCol, 0, ToolRunData.ToolOutputDoubleValue[RowSourceKey], ToolRunData.ToolOutputDoubleValue[ColSourceKey],0, out homMat2D);

                    HOperatorSet.AffineTransPixel(homMat2D, Row, Col, out circleRow, out circleCol);

                    circleParam[0] = circleRow;
                    circleParam[1] = circleCol;
                    circleParam[2] = Radius;

                    // 添加测量参数
                    HOperatorSet.AddMetrologyObjectGeneric(metrologyHandle, "circle", circleParam, MeasureLength, MeasureWidth, MeasureSigma, MeasureThreshold, new HTuple("num_measures").TupleConcat("measure_transition"), new HTuple(MeasureNum).TupleConcat(MeasureTransition), out index);
                    homMat2D.Dispose();
                }
                else
                {
                    HTuple tempCircleParma = new HTuple(Row, Col, Radius);

                    // 添加测量参数
                    HOperatorSet.AddMetrologyObjectGeneric(metrologyHandle, "circle", tempCircleParma, MeasureLength, MeasureWidth, MeasureSigma, MeasureThreshold, new HTuple("num_measures").TupleConcat("measure_transition"), new HTuple(MeasureNum).TupleConcat(MeasureTransition), out index);
                }

                // 应用到相关图像
                HOperatorSet.ApplyMetrologyModel(DisplayImage, metrologyHandle);

                // 获取测量工具的查找区域
                HOperatorSet.GetMetrologyObjectMeasures(out measureContours, metrologyHandle, "all", "all", out measureRow, out measureCol);

                if (measureRow.Length == 0 || measureCol.Length == 0)
                {
                    if (isTest)
                        MessageBox.Show("未找到圆", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // 获取测量结果的行列坐标
                HOperatorSet.GetMetrologyObjectResult(metrologyHandle, "all", "all", "used_edges", "row", out usedRow);
                HOperatorSet.GetMetrologyObjectResult(metrologyHandle, "all", "all", "used_edges", "column", out usedCol);

                if (usedRow.Length == 0 || usedCol.Length == 0)
                {
                    if (isTest)
                        MessageBox.Show("未找到线", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    DisplayRegion = measureContours;
                    return false;
                }

                // 绘制查找到的点
                HOperatorSet.GenCrossContourXld(out crossContours, usedRow, usedCol, 10, new HTuple(45).TupleRad());

                // 获取测量得到的轮廓
                HOperatorSet.GenContourPolygonXld(out tempContours, usedRow, usedCol);

                // 获取圆的圆心和半径
                HOperatorSet.GetMetrologyObjectResult(metrologyHandle, "all", "all", "result_type", "radius", out radius);
                HOperatorSet.GetMetrologyObjectResult(metrologyHandle, "all", "all", "result_type", "row", out row);
                HOperatorSet.GetMetrologyObjectResult(metrologyHandle, "all", "all", "result_type", "column", out col);

                // 画圆和圆心
                HOperatorSet.GenCrossContourXld(out cross, row, col, 10, new HTuple(45).TupleRad());
                HOperatorSet.GenCircleContourXld(out circle, row, col, radius, 0, new HTuple(360).TupleRad(), "positive", 1);


                HObject temp, showTemp;
                HOperatorSet.GenEmptyObj(out temp);

                HOperatorSet.ConcatObj(measureContours, temp, out showTemp);
                temp = showTemp;
                HOperatorSet.ConcatObj(crossContours, temp, out showTemp);
                temp = showTemp;
                HOperatorSet.ConcatObj(circle, temp, out showTemp);
                temp = showTemp;
                HOperatorSet.ConcatObj(cross, temp, out showTemp);

                DisplayRegion = showTemp.Clone();

                HOperatorSet.ClearMetrologyObject(metrologyHandle, "all");

                index.Dispose();
                measureRow.Dispose();
                measureCol.Dispose();
                usedRow.Dispose();
                usedCol.Dispose();
                measureContours.Dispose();
                crossContours.Dispose();
                tempContours.Dispose();
                cross.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void AddRoi()
        {
            if (DisplayRegion == null || DisplayImage == null)
            {
                DrawRoi = null;
                MessageBox.Show("请先加载图像或在图像中绘制Roi区域", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DrawRoi = null;
            if (DrawRoi == null)
            {
                DrawRoi = DisplayRegion.Clone();
            }

            if (EnableMat)
            {
                MatRow = ToolRunData.ToolOutputDoubleValue[RowSourceKey];
                MatCol = ToolRunData.ToolOutputDoubleValue[ColSourceKey];
            }

            try
            {
                GetRegionLineParam(DrawRoi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetRegionLineParam(HObject roi)
        {
            HTuple row, col, area;
            HOperatorSet.AreaCenter(roi, out area, out row, out col);

            Row = row;
            Col = col;
            Radius = Math.Sqrt(area / Math.PI);

            RoiAdded = true;

            row.Dispose();
            col.Dispose();
            area.Dispose(); 
            MessageBox.Show("完成模板原始位置及Roi的添加");
        }
    }
}
