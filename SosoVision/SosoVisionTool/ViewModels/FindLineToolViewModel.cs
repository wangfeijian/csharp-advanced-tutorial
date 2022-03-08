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
    public class FindLineToolViewModel : BindableBase, IToolBaseViewModel
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
        public double RowStart { get; set; }
        public double ColStart { get; set; }
        public double RowEnd { get; set; }
        public double ColEnd { get; set; }

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

        private string _angleSourceKey;

        public string AngleSourceKey
        {
            get { return _angleSourceKey; }
            set { _angleSourceKey = value; RaisePropertyChanged(); }
        }

        public double MatRow { get; set; }
        public double MatCol { get; set; }
        public double MatAngle { get; set; }
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
            HTuple rowBegin, colBegin, rowEnd, colEnd, rowCenter, colCenter, angle;
            HObject outLine;
            bool tempResult = FindLine(out rowBegin, out colBegin, out rowEnd, out colEnd, out angle, out rowCenter, out colCenter, out outLine, false);

            string rowBeginStr = SetMessage(nameof(rowBegin), rowBegin.ToString());
            string colBeginStr = SetMessage(nameof(colBegin), colBegin.ToString());
            string rowEndStr = SetMessage(nameof(rowEnd), rowEnd.ToString());
            string colEndStr = SetMessage(nameof(colEnd), colEnd.ToString());
            string rowCenterStr = SetMessage(nameof(rowCenter), rowCenter.ToString());
            string colCenterStr = SetMessage(nameof(colCenter), colCenter.ToString());
            string angleStr = SetMessage(nameof(angle), angle.ToString());

            string message;
            if (tempResult)
            {
                message = string.Join("\n", "OK", rowBeginStr, colBeginStr, rowEndStr, colEndStr, rowCenterStr, colCenterStr, angleStr);
                ShowRunInfo(message);
            }
            else
            {
                message = string.Join("\n", "NG", rowBeginStr, colBeginStr, rowEndStr, colEndStr, rowCenterStr, colCenterStr, angleStr);
                ShowRunInfo(message, false);
            }

            AddImageToData(key, DisplayImage);
            AddRegionToData(key, DisplayRegion);
            AddRegionToData($"{key}_{nameof(outLine)}", outLine.Clone());
            AddDoubleToData($"{key}_{nameof(rowBegin)}", rowBeginStr);
            AddDoubleToData($"{key}_{nameof(colBegin)}", colBeginStr);
            AddDoubleToData($"{key}_{nameof(rowEnd)}", rowEndStr);
            AddDoubleToData($"{key}_{nameof(colEnd)}", colEndStr);
            AddDoubleToData($"{key}_{nameof(rowCenter)}", rowCenterStr);
            AddDoubleToData($"{key}_{nameof(colCenter)}", colCenterStr);
            AddDoubleToData($"{key}_{nameof(angle)}", angleStr);

            HObjectParams tempHobjectCamera = new HObjectParams { Image = DisplayImage, VisionStep = tool.ToolInVision, ImageKey = key, Region = DisplayRegion, RegionKey = key };
            _eventAggregator.GetEvent<HObjectEvent>().Publish(tempHobjectCamera);

            TreeViewItem temp = new TreeViewItem { Header = nameof(DisplayImage), ToolTip = DisplayImage.ToString() };
            tool.AddInputOutputTree(temp, true);

            temp = new TreeViewItem { Header = nameof(RowSourceKey), ToolTip = ToolRunData.ToolOutputDoubleValue[RowSourceKey].ToString() };
            tool.AddInputOutputTree(temp, true);

            temp = new TreeViewItem { Header = nameof(ColSourceKey), ToolTip = ToolRunData.ToolOutputDoubleValue[ColSourceKey].ToString() };
            tool.AddInputOutputTree(temp, true);

            temp = new TreeViewItem { Header = nameof(AngleSourceKey), ToolTip = ToolRunData.ToolOutputDoubleValue[AngleSourceKey] };
            tool.AddInputOutputTree(temp, true);

            temp = new TreeViewItem { Header = nameof(rowBegin), ToolTip = rowBegin.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(colBegin), ToolTip = colBegin.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(rowEnd), ToolTip = rowEnd.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(colEnd), ToolTip = colEnd.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(rowCenter), ToolTip = rowCenter.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(colCenter), ToolTip = colCenter.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(angle), ToolTip = angle.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(outLine), ToolTip = outLine.ToString() };
            tool.AddInputOutputTree(temp, false);

            result = tempResult;

            rowBegin.Dispose();
            colBegin.Dispose();
            rowEnd.Dispose();
            colEnd.Dispose();
            rowCenter.Dispose();
            colCenter.Dispose();
            angle.Dispose();
            outLine.Dispose();
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

        public FindLineToolViewModel()
        {
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
            TestCommand = new DelegateCommand(Test);
            AddRoiCommand = new DelegateCommand(AddRoi);
        }
        public FindLineToolViewModel(string visionStep)
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
            HTuple rowBegin, colBegin, rowEnd, colEnd, rowCenter, colCenter, angle;
            HObject line;
            bool tempResult = FindLine(out rowBegin, out colBegin, out rowEnd, out colEnd, out angle, out rowCenter, out colCenter, out line);

            string rowBeginStr = SetMessage(nameof(rowBegin), rowBegin.ToString());
            string colBeginStr = SetMessage(nameof(colBegin), colBegin.ToString());
            string rowEndStr = SetMessage(nameof(rowEnd), rowEnd.ToString());
            string colEndStr = SetMessage(nameof(colEnd), colEnd.ToString());
            string angleStr = SetMessage(nameof(angle), angle.ToString());

            string message;
            if (tempResult)
            {
                message = string.Join("\n", "OK", rowBeginStr, colBeginStr, rowEndStr, colEndStr, angleStr);
                ShowRunInfo(message);
            }
            else
            {
                message = string.Join("\n", "NG", rowBeginStr, colBeginStr, rowEndStr, colEndStr, angleStr);
                ShowRunInfo(message, false);
            }

            rowBegin.Dispose();
            colBegin.Dispose();
            rowEnd.Dispose();
            colEnd.Dispose();
            rowCenter.Dispose();
            colCenter.Dispose();
            angle.Dispose();
            line.Dispose();
        }

        private bool FindLine(out HTuple rowBegin, out HTuple colBegin, out HTuple rowEnd, out HTuple colEnd, out HTuple angle, out HTuple rowCenter, out HTuple colCenter, out HObject line, bool isTest = true)
        {
            rowBegin = colBegin = rowEnd = colEnd = angle = rowCenter = colCenter = 999;
            line = null;

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
                if (string.IsNullOrWhiteSpace(RowSourceKey) || string.IsNullOrWhiteSpace(ColSourceKey) || string.IsNullOrWhiteSpace(AngleSourceKey))
                {
                    if (isTest)
                        MessageBox.Show("请先绘制一条直线", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            HTuple homMat2D, metrologyHandle, index, measureRow, measureCol, usedRow, usedCol;
            HObject measureContours, crossContours, tempContours, fitLine;
            HOperatorSet.CreateMetrologyModel(out metrologyHandle);

            try
            {
                if (EnableMat)
                {
                    // 通过仿射拿到跟随的直线
                    HTuple startRow, startCol, endRow, endCol, lineParam = new HTuple();
                    HOperatorSet.VectorAngleToRigid(MatRow, MatCol, MatAngle, ToolRunData.ToolOutputDoubleValue[RowSourceKey], ToolRunData.ToolOutputDoubleValue[ColSourceKey], ToolRunData.ToolOutputDoubleValue[AngleSourceKey], out homMat2D);

                    HOperatorSet.AffineTransPixel(homMat2D, RowStart, ColStart, out startRow, out startCol);
                    HOperatorSet.AffineTransPixel(homMat2D, RowEnd, ColEnd, out endRow, out endCol);

                    lineParam[0] = startRow;
                    lineParam[1] = startCol;
                    lineParam[2] = endRow;
                    lineParam[3] = endCol;

                    // 添加测量参数
                    HOperatorSet.AddMetrologyObjectGeneric(metrologyHandle, "line", lineParam, MeasureLength, MeasureWidth, MeasureSigma, MeasureThreshold, new HTuple("num_measures").TupleConcat("measure_transition"), new HTuple(MeasureNum).TupleConcat(MeasureTransition), out index);

                    homMat2D.Dispose();
                }
                else
                {
                    HTuple tempLineParma = new HTuple(RowStart, ColStart, RowEnd, ColEnd);

                    // 添加测量参数
                    HOperatorSet.AddMetrologyObjectGeneric(metrologyHandle, "line", tempLineParma, MeasureLength, MeasureWidth, MeasureSigma, MeasureThreshold, new HTuple("num_measures").TupleConcat("measure_transition"), new HTuple(MeasureNum).TupleConcat(MeasureTransition), out index);
                }

                // 应用到相关图像
                HOperatorSet.ApplyMetrologyModel(DisplayImage, metrologyHandle);

                // 获取测量工具的查找区域
                HOperatorSet.GetMetrologyObjectMeasures(out measureContours, metrologyHandle, "all", "all", out measureRow, out measureCol);

                if (measureRow.Length == 0 || measureCol.Length == 0)
                {
                    if (isTest)
                        MessageBox.Show("未找到线", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
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

                // 拟合直线
                HTuple nr, nc, dist;
                HOperatorSet.FitLineContourXld(tempContours, "tukey", MeasureNum * 0.9, 0, 15, 5, out rowBegin, out colBegin, out rowEnd, out colEnd, out nr, out nc, out dist);

                HTuple fitLineRow = new HTuple(rowBegin, rowEnd);
                HTuple fitLineCol = new HTuple(colBegin, colEnd);
                HOperatorSet.GenContourPolygonXld(out fitLine, fitLineRow, fitLineCol);

                // 生成延长线
                HObject region;
                HTuple area, ra, rb, phi;
                HOperatorSet.GenRegionContourXld(fitLine, out region, "filled");
                HOperatorSet.AreaCenter(region, out area, out rowCenter, out colCenter);
                HOperatorSet.EllipticAxisPointsXld(fitLine, out ra, out rb, out phi);
                HTuple lineLength = 10000;
                HTuple rowStart = rowCenter - new HTuple(phi + 1.5708).TupleCos() * lineLength;
                HTuple colStart = colCenter - new HTuple(phi + 1.5708).TupleSin() * lineLength;
                HTuple rowEnds = rowCenter - new HTuple(phi - 1.5708).TupleCos() * lineLength;
                HTuple colEnds = colCenter - new HTuple(phi - 1.5708).TupleSin() * lineLength;
                HOperatorSet.GenContourPolygonXld(out region, new HTuple(rowStart, rowEnds), new HTuple(colStart, colEnds));

                line = region.Clone();

                // 求线的角度
                HTuple atan, deg;
                HTuple offsetX = colEnd - colBegin;
                HTuple offsetY = rowEnd - rowBegin;
                HOperatorSet.TupleAtan2(offsetY, offsetX, out atan);
                HOperatorSet.TupleDeg(atan, out deg);
                angle = 180 - deg;

                HObject temp, showTemp;
                HOperatorSet.GenEmptyObj(out temp);

                HOperatorSet.ConcatObj(measureContours, temp, out showTemp);
                temp = showTemp;
                HOperatorSet.ConcatObj(crossContours, temp, out showTemp);
                temp = showTemp;
                HOperatorSet.ConcatObj(line, temp, out showTemp);

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
                fitLine.Dispose();
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
                MatAngle = ToolRunData.ToolOutputDoubleValue[AngleSourceKey];
            }

            try
            {
                GetRegionLineParam(DrawRoi, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetRegionLineParam(HObject roi, bool isAdd)
        {
            HTuple rows, cols;
            HObject skeleton, endPoints, juncPoints, region;
            HOperatorSet.GenEmptyObj(out skeleton);
            HOperatorSet.GenEmptyObj(out endPoints);
            HOperatorSet.GenEmptyObj(out juncPoints);
            HOperatorSet.GenRegionContourXld(roi, out region, "filled");
            HOperatorSet.Skeleton(region, out skeleton);
            HOperatorSet.JunctionsSkeleton(skeleton, out endPoints, out juncPoints);
            HOperatorSet.GetRegionPoints(endPoints, out rows, out cols);
            if (isAdd)
                if (rows.Length != 2)
                {
                    MessageBox.Show("请绘制一条直线！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            RowStart = rows[0];
            ColStart = cols[0];
            RowEnd = rows[1];
            ColEnd = cols[1];
            RoiAdded = true;

            rows.Dispose();
            cols.Dispose();
            skeleton.Dispose();
            endPoints.Dispose();
            juncPoints.Dispose();
            region.Dispose();
            MessageBox.Show("完成模板原始位置及Roi的添加");
        }
    }
}
