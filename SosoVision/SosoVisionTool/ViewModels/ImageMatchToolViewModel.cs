using HalconDotNet;
using ImageCapture;
using Prism.Mvvm;
using Prism.Ioc;
using SosoVisionCommonTool.ConfigData;
using SosoVisionTool.Services;
using SosoVisionTool.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.Windows;
using SosoVisionTool.Tools;
using Prism.Events;
using System.Windows.Controls;

namespace SosoVisionTool.ViewModels
{
    public class ImageMatchToolViewModel : BindableBase, IToolBaseViewModel
    {
        [Newtonsoft.Json.JsonIgnore]
        public ToolRunViewData ToolRunData { get; set; }

        private IEventAggregator _eventAggregator;

        public string VisionStep { get; set; }
        public ProcedureParam Param { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public CaptureBase Capture { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public DelegateCommand TestCommand { get; }

        public DelegateCommand CreateModelCommand { get; }

        public HTuple ModelId { get; set; }

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

        private string _imageSourceKey;

        public string ImageSourceKey
        {
            get { return _imageSourceKey; }
            set { _imageSourceKey = value; RaisePropertyChanged(); }
        }

        private int _numLevels = 4;

        public int NumLevels
        {
            get { return _numLevels; }
            set { _numLevels = value; RaisePropertyChanged(); }
        }

        private int _contrast = 30;

        public int Contrast
        {
            get { return _contrast; }
            set { _contrast = value; RaisePropertyChanged(); }
        }

        private string _metric = "use_polarity";

        public string Metric
        {
            get { return _metric; }
            set { _metric = value; RaisePropertyChanged(); }
        }

        private string _optimization = "auto";

        public string Optimization
        {
            get { return _optimization; }
            set { _optimization = value; RaisePropertyChanged(); }
        }

        private int _angleStart;

        public int AngleStart
        {
            get { return _angleStart; }
            set
            {
                if (value < 0 || value < 360)
                {
                    value = 0;
                }
                _angleStart = value;
                RaisePropertyChanged();
            }
        }

        private int _angleExtent;

        public int AngleExtent
        {
            get { return _angleExtent; }
            set
            {
                if (value < 0 || value < 360)
                {
                    value = 0;
                }
                _angleExtent = value;
                RaisePropertyChanged();
            }
        }

        private int _minContrast;

        public int MinContrast
        {
            get { return _minContrast; }
            set
            {
                if (value > Contrast)
                {
                    value = 7;
                }
                _minContrast = value;
                RaisePropertyChanged();
            }
        }

        private double _minScore = 0.5;

        public double MinScore
        {
            get { return _minScore; }
            set
            {
                if (value > 1.0 || value < 0)
                {
                    value = 0.5;
                }
                _minScore = value;
                RaisePropertyChanged();
            }
        }

        private int _numMatches = 1;

        public int NumMatches
        {
            get { return _numMatches; }
            set
            {
                if (value <= 0)
                {
                    value = 1;
                }
                _numMatches = value;
                RaisePropertyChanged();
            }
        }

        private double _maxOverlap = 0.5;

        public double MaxOverlap
        {
            get { return _maxOverlap; }
            set
            {
                if (value > 1.0 || value < 0)
                {
                    value = 0.5;
                }
                _maxOverlap = value;
                RaisePropertyChanged();
            }
        }

        private string _subPixel = "least_squares";

        public string SubPixel
        {
            get { return _subPixel; }
            set { _subPixel = value; RaisePropertyChanged(); }
        }

        private int _useLevels = 0;

        public int UseLevels
        {
            get { return _useLevels; }
            set { _useLevels = value; RaisePropertyChanged(); }
        }

        private double _greediness = 0.9;

        public double Greediness
        {
            get { return _greediness; }
            set
            {
                if (value > 1.0 || value < 0)
                {
                    value = 0.9;
                }
                _greediness = value;
                RaisePropertyChanged();
            }
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

        public List<int> NumLevelList { get; set; }
        public List<int> UseLevelList { get; set; }
        public List<int> ContrastList { get; set; }
        public List<string> MetricList { get; set; }
        public List<string> OptimizationList { get; set; }
        public List<string> SubPixelList { get; set; }
        public HObject DrawRoi { get; set; }
        public ImageMatchToolViewModel()
        {
            TestCommand = new DelegateCommand(Test);
            CreateModelCommand = new DelegateCommand(CreateModel);
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();
        }

        public ImageMatchToolViewModel(string visionStep)
        {
            VisionStep = visionStep;
            ToolRunData = ContainerLocator.Container.Resolve<ToolRunViewData>(VisionStep);
            _eventAggregator = ContainerLocator.Container.Resolve<IEventAggregator>();

            NumLevelList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            UseLevelList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            ContrastList = new List<int> { 10, 20, 30, 40, 60, 80, 100, 120, 140, 160 };
            MetricList = new List<string> { "ignore_color_polarity", "ignore_global_polarity", "ignore_local_polarity", "use_polarity" };
            OptimizationList = new List<string> { "auto", "no_pregeneration", "none", "point_reduction_high", "point_reduction_low", "point_reduction_medium", "pregeneration" };
            SubPixelList = new List<string> { "none", "interpolation", "least_squares", "least_squares_high", "least_squares_very_high", "max_deformation 1", "max_deformation 2", "max_deformation 3", "max_deformation 4", "max_deformation 5", "max_deformation 6" };
            TestCommand = new DelegateCommand(Test);
            CreateModelCommand = new DelegateCommand(CreateModel);
        }

        private void ShowRunInfo(string message, bool IsOk = true)
        {
            DisplayMessage = "";
            MessageColor = IsOk ? "green" : "red";
            DisplayMessage = message;
        }

        public void Run(ToolBase tool, ref bool result)
        {
            DisplayRegion = null;
            string key = $"{tool.ToolInVision}_{tool.ToolItem.Header}";
            HTuple row, col, angle, score;
            bool tempResult = FindModel(out row, out col, out angle, out score, false);

            string rowStr = SetMessage(nameof(row), row.ToString());
            string colStr = SetMessage(nameof(col), col.ToString());
            string angleStr = SetMessage(nameof(angle), angle.ToString());
            string scoreStr = SetMessage(nameof(score), score.ToString());

            string message;
            if (tempResult)
            {
                message = string.Join("\n","OK",rowStr, colStr, angleStr, scoreStr);
                ShowRunInfo(message);
            }
            else
            {
                message = string.Join("\n","NG",rowStr, colStr, angleStr, scoreStr);
                ShowRunInfo(message, false);
            }

            AddImageToData(key, DisplayImage);
            AddRegionToData(key, DisplayRegion);
            AddDoubleToData($"{key}_{nameof(row)}", rowStr);
            AddDoubleToData($"{key}_{nameof(col)}", colStr);
            AddDoubleToData($"{key}_{nameof(angle)}", angleStr);
            AddDoubleToData($"{key}_{nameof(score)}", scoreStr);
            HObjectParams tempHobjectCamera = new HObjectParams { Image = DisplayImage, VisionStep = tool.ToolInVision, ImageKey = key, Region = DisplayRegion, RegionKey = key };
            _eventAggregator.GetEvent<HObjectEvent>().Publish(tempHobjectCamera);

            TreeViewItem temp = new TreeViewItem { Header = nameof(DisplayImage), ToolTip = DisplayImage.ToString() };
            tool.AddInputOutputTree(temp, true);

            temp = new TreeViewItem { Header = nameof(row), ToolTip = row.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(col), ToolTip = col.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(angle), ToolTip = angle.ToString() };
            tool.AddInputOutputTree(temp, false);

            temp = new TreeViewItem { Header = nameof(score), ToolTip = score.ToString() };
            tool.AddInputOutputTree(temp, false);

            result = tempResult;
        }

        private string SetMessage(string name, string value)
        {
            string temp=string.Empty;

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

            if (value.Contains("."))
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
            string temp = value.Substring(value.IndexOf(":")+1,value.Length -1- value.IndexOf(":"));
            double result;

            if (temp.Contains(","))
            {
                AddStringToData(key, temp);
                return;
            }

            if(!double.TryParse(temp, out result))
                result = 999.99;

            if (ToolRunData.ToolOutputDoubleValue.ContainsKey(key))
            {
                ToolRunData.ToolOutputDoubleValue[key] = result;
                return;
            }

            ToolRunData.ToolOutputDoubleValue.Add(key, result);
        }

        private bool FindModel(out HTuple row, out HTuple col, out HTuple angle, out HTuple score, bool isTest = true)
        {
            row = col = angle = score = 999;
            if (ImageSourceKey == null)
            {
                if (isTest)
                    MessageBox.Show("请先选择图像源", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            DisplayImage = ToolRunData.ToolOutputImage[ImageSourceKey];

            if (ModelId == null)
            {
                if (isTest)
                    MessageBox.Show("请先制作模板！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                HObject modelContours, contoursAffineTrans;
                HTuple homMat2D;
                HTuple angleStart, angleExtent;
                HOperatorSet.TupleRad(AngleStart, out angleStart);
                HOperatorSet.TupleRad(AngleExtent, out angleExtent);
                HOperatorSet.GetShapeModelContours(out modelContours, ModelId, 1);
                HOperatorSet.FindShapeModel(DisplayImage, ModelId, angleStart, angleExtent, MinScore, NumMatches, MaxOverlap, SubPixel, UseLevels, Greediness, out row, out col, out angle, out score);
                if (score.Length == 0)
                {
                    DisplayRegion = null;
                    row = col = angle = score = 999;
                    if (isTest)
                        MessageBox.Show("模板查找失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                HObject temp;
                HOperatorSet.GenEmptyObj(out temp);
                for (int i = 0; i < row.Length; i++)
                {
                    HObject xldTemp;
                    HOperatorSet.VectorAngleToRigid(0, 0, 0, row[i], col[i], angle[i], out homMat2D);
                    HOperatorSet.AffineTransContourXld(modelContours, out contoursAffineTrans, homMat2D);
                    HOperatorSet.ConcatObj(contoursAffineTrans, temp, out xldTemp);
                    temp = xldTemp;

                }
                DisplayRegion = temp;
                return true;
            }
            catch (Exception ex)
            {
                if (isTest)
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private void Test()
        {
            HTuple row, col, angle, score;
            bool tempResult = FindModel(out row, out col, out angle, out score);

            string rowStr = SetMessage(nameof(row), row.ToString());
            string colStr = SetMessage(nameof(col), col.ToString());
            string angleStr = SetMessage(nameof(angle), angle.ToString());
            string scoreStr = SetMessage(nameof(score), score.ToString());

            string message;
            if (tempResult)
            {
                message = string.Join("\n", "OK", rowStr, colStr, angleStr, scoreStr);
                ShowRunInfo(message);
            }
            else
            {
                message = string.Join("\n", "NG", rowStr, colStr, angleStr, scoreStr);
                ShowRunInfo(message, false);
            }
        }

        private void CreateModel()
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

            try
            {
                HObject imageReduced, modelImages, modelRegions;
                HTuple angleStart, angleExtent, modelID;
                HOperatorSet.TupleRad(AngleStart, out angleStart);
                HOperatorSet.TupleRad(AngleExtent, out angleExtent);

                HOperatorSet.ReduceDomain(DisplayImage, DrawRoi, out imageReduced);
                HOperatorSet.InspectShapeModel(imageReduced, out modelImages, out modelRegions, NumLevels, Contrast);
                DisplayRegion = modelRegions;
                HOperatorSet.CreateShapeModel(imageReduced, NumLevels, angleStart, angleExtent, "auto", Optimization, Metric, Contrast, MinContrast, out modelID);
                ModelId = modelID.Clone();
                MessageBox.Show("模板制作完成！", "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
