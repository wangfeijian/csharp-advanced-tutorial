using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using HalconDotNet;
using ImageCapture;

namespace MeasurementProgram
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string? ip;

        public string? Ip
        {
            get { return ip; }
            set { ip = value; NotifyPropertyChange(nameof(Ip)); }
        }

        private bool isOpen;

        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; NotifyPropertyChange(nameof(IsOpen)); }
        }

        private bool captureEnable = true;

        public bool CaptureEnable
        {
            get { return captureEnable; }
            set { captureEnable = value; NotifyPropertyChange(nameof(CaptureEnable)); }
        }

        private HObject? _displayImage;

        public HObject? DisplayImage
        {
            get { return _displayImage; }
            set { _displayImage = value; NotifyPropertyChange(nameof(DisplayImage)); }
        }

        private HObject? _displayRegion;

        public HObject? DisplayRegion
        {
            get { return _displayRegion; }
            set { _displayRegion = value; NotifyPropertyChange(nameof(DisplayRegion)); }
        }

        private string? fillMode;

        public string? FillMode
        {
            get { return fillMode; }
            set { fillMode = value; NotifyPropertyChange(nameof(FillMode)); }
        }

        private string? _result;

        public string? Result
        {
            get { return _result; }
            set { _result = value; NotifyPropertyChange(nameof(Result)); }
        }

        private double _exposureTime;

        public double ExposureTime
        {
            get { return _exposureTime; }
            set { _exposureTime = value; NotifyPropertyChange(nameof(ExposureTime)); }
        }

        private double _contrast;

        public double Contrast
        {
            get { return _contrast; }
            set { _contrast = value; NotifyPropertyChange(nameof(Contrast)); }
        }

        private double _brightness;

        public double Brightness
        {
            get { return _brightness; }
            set { _brightness = value; NotifyPropertyChange(nameof(Brightness)); }
        }

        public CaptureBase? _imageCapture;
        private bool _isContinuousCapture;
        private bool _atCapture;
        private double _calibValue;
        private double _calibLength;

        public DelegateCommand OpenCameraCommand { get; }
        public DelegateCommand CloseCameraCommand { get; }
        public DelegateCommand CalibCommand { get; }
        public DelegateCommand CaptureTypeChangeCommand { get; }
        public DelegateCommand ApplyParamCommand { get; }
        public DelegateCommand GrabImageCommand { get; }
        public DelegateCommand GetRegionLineParamCommand { get; }
        public DelegateCommand GetRegionCircleParamCommand { get; }
        public MainWindowViewModel()
        {
            Ip = ConfigFileOperator.GetConfigValue("Ip");
            FillMode = "margin";
            Result = "未进行任何测量";
            double.TryParse(ConfigFileOperator.GetConfigValue("CalibValue"), out _calibValue);
            double.TryParse(ConfigFileOperator.GetConfigValue("CalibLength"), out _calibLength);
            OpenCameraCommand = new DelegateCommand { ActionExecute = OpenCamera };
            CloseCameraCommand = new DelegateCommand { ActionExecute = CloseCamera };
            CalibCommand = new DelegateCommand { ActionExecute = CalibCamera };
            CaptureTypeChangeCommand = new DelegateCommand { ActionExecute = CaptureTypeChange };
            ApplyParamCommand = new DelegateCommand { ActionExecute = ApplyParam };
            GrabImageCommand = new DelegateCommand { ActionExecute = GrabImage };
            GetRegionLineParamCommand = new DelegateCommand { ActionExecute = GetRegionLineParam };
            GetRegionCircleParamCommand = new DelegateCommand { ActionExecute = GetRegionCircleParam };
        }

        private void OpenCamera(object obj)
        {
            if (_imageCapture != null && _imageCapture.IsOpen())
            {
                MessageBox.Show("相机已经打开");
                return;
            }
            _imageCapture = new CaptureHik(Ip, false);
            IsOpen = _imageCapture.IsOpen();
        }

        private void CloseCamera(object obj)
        {
            _imageCapture?.Close();

            if (_imageCapture == null)
                IsOpen = false;
            else
                IsOpen = _imageCapture.IsOpen();
        }

        private void CalibCamera(object obj)
        {
            if (DisplayImage == null)
            {
                MessageBox.Show("请先采集一张标定图片");
                return;
            }

            if (DisplayRegion == null)
            {
                MessageBox.Show("请先选择一个区域");
                return;
            }

            try
            {
                HObject imageReduced, regions, connectedRegions, selectedRegions, contours;
                HTuple rows, cols, radius, startPhi, endPhi, pointOrder, distance;
                HOperatorSet.ReduceDomain(DisplayImage, DisplayRegion, out imageReduced);
                HOperatorSet.AutoThreshold(imageReduced, out regions, 2);
                HOperatorSet.Connection(regions, out connectedRegions);
                HOperatorSet.SelectShape(connectedRegions, out selectedRegions, "circularity", "and", 0.96, 0.99);
                DisplayRegion = selectedRegions;
                HOperatorSet.GenContourRegionXld(selectedRegions, out contours, "border");

                HOperatorSet.FitCircleContourXld(contours, "algebraic", -1, 0, 0, 3, 2, out rows, out cols, out radius, out startPhi, out endPhi, out pointOrder);
                HOperatorSet.DistancePp(rows[0], cols[0], rows[1], cols[1], out distance);

                if (rows.Length != 2)
                {
                    _calibValue = 0;
                    Result = $"标定失败,找到{rows.Length}圆";
                    return;
                }
                _calibValue = _calibLength / distance;
                Result = $"标定完成\n像素大小：{_calibValue}";

                imageReduced.Dispose(); regions.Dispose(); connectedRegions.Dispose(); selectedRegions.Dispose(); contours.Dispose();
                rows.Dispose(); cols.Dispose(); radius.Dispose(); startPhi.Dispose(); endPhi.Dispose(); pointOrder.Dispose(); distance.Dispose();
            }
            catch (Exception)
            {
                MessageBox.Show("标定失败！");
                _calibValue = 0;
            }
            finally
            {
                ConfigFileOperator.SetConfigValue("CalibValue", _calibValue.ToString());
            }
        }

        private void CaptureTypeChange(object obj)
        {
            switch (obj.ToString())
            {
                case "single":
                    _isContinuousCapture = false;
                    break;
                case "continuous":
                    _isContinuousCapture = true;
                    break;
                case "stop":
                    _atCapture = false;
                    break;
            }
        }

        private void ApplyParam(object obj)
        {
            _imageCapture?.SetBrightness(Brightness);
            _imageCapture?.SetConstract(Contrast);
            _imageCapture?.SetExposure(ExposureTime);
        }

        private void GrabImage(object obj)
        {
            if (_isContinuousCapture)
            {
                CaptureEnable = false;
                _atCapture = true;
                while (_atCapture)
                {
                    DispatcherHelper.Delay(20);
                    if (_imageCapture?.Grab() == 1)
                    {
                        DisplayImage = _imageCapture.GetImage();
                    }
                }
                CaptureEnable = true;
                return;
            }

            if (_imageCapture?.Grab() == 1)
            {
                DisplayImage = _imageCapture?.GetImage();
            }
        }

        private void GetRegionLineParam(object obj)
        {
            if (DisplayRegion == null)
            {
                MessageBox.Show("请先绘制一条直线");
                Result = "请先绘制一条直线";
                return;
            }

            if (_calibValue == 0)
            {
                MessageBox.Show("请先进行标定，再测量！");
                Result = "请先进行标定，再测量！";
                return;
            }

            try
            {
                HTuple rows, cols, length;

                HOperatorSet.GetContourXld(DisplayRegion, out rows, out cols);

                if (rows.Length != 2)
                {
                    MessageBox.Show("请绘制一条直线！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    Result = "请先绘制一条直线";
                    return;
                }

                HOperatorSet.DistancePp(rows[0], cols[0], rows[1], cols[1], out length);
                double distance = length * _calibValue;
                Result = $"直线长度：\n{distance}mm";

                rows.Dispose();
                cols.Dispose();
                length.Dispose();
            }
            catch (Exception)
            {
                MessageBox.Show("请绘制一条直线！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                Result = "请先绘制一条直线";
            }
        }

        private void GetRegionCircleParam(object obj)
        {
            if (DisplayRegion == null)
            {
                MessageBox.Show("请先绘制一个圆");
                Result = "请先绘制一个圆";
                return;
            }

            if (_calibValue == 0)
            {
                MessageBox.Show("请先进行标定，再测量！");
                Result = "请先进行标定，再测量！";
                return;
            }

            try
            {
                HTuple row, col, area;
                HOperatorSet.AreaCenter(DisplayRegion, out area, out row, out col);

                double areaCircle = area* _calibValue;
                double dis = Math.Sqrt(area / Math.PI) * 2* _calibValue;

                Result = $"圆面积：\n{areaCircle}\n圆直径：\n{dis}mm";

                row.Dispose();
                col.Dispose();
                area.Dispose();
            }
            catch (Exception)
            {
                MessageBox.Show("请先绘制一个圆！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                Result = "请先绘制一个圆";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class DispatcherHelper
    {
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond) //毫秒
            {
                DoEvents();
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);
            try
            {
                Dispatcher.PushFrame(frame);
            }
            catch (InvalidOperationException)
            {

            }
        }

        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }
}
