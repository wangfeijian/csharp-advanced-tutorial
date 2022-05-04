using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace CameraAndLensSelectAndCalc
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private double _accuracy;

        public double Accuracy
        {
            get { return _accuracy; }
            set { _accuracy = value; NotifyPropertyChange(nameof(Accuracy)); }
        }

        private double _viewWidth;

        public double ViewWidth
        {
            get { return _viewWidth; }
            set { _viewWidth = value; NotifyPropertyChange(nameof(ViewWidth)); }
        }

        private double _viewHeight;

        public double ViewHeight
        {
            get { return _viewHeight; }
            set { _viewHeight = value; NotifyPropertyChange(nameof(ViewHeight)); }
        }

        private int _safetyFactor = 5;

        public int SafetyFactor
        {
            get { return _safetyFactor; }
            set { _safetyFactor = value; NotifyPropertyChange(nameof(SafetyFactor)); }
        }

        private ObservableCollection<CameraShowData>? _roughCameraList;

        public ObservableCollection<CameraShowData>? RoughCameraList
        {
            get { return _roughCameraList; }
            set { _roughCameraList = value; NotifyPropertyChange(nameof(RoughCameraList)); }
        }

        private List<CameraData>? _allCameraData;

        public List<CameraData>? AllCameraData
        {
            get { return _allCameraData; }
            set { _allCameraData = value; NotifyPropertyChange(nameof(AllCameraData)); }
        }

        private List<LensData>? _allLensData;

        public List<LensData>? AllLensData
        {
            get { return _allLensData; }
            set { _allLensData = value; NotifyPropertyChange(nameof(AllLensData)); }
        }

        private List<string>? _selectCameraList;

        public List<string>? SelectCameraList
        {
            get { return _selectCameraList; }
            set { _selectCameraList = value; NotifyPropertyChange(nameof(SelectCameraList)); }
        }

        private string? _selectCamera;

        public string? SelectCamera
        {
            get { return _selectCamera; }
            set { _selectCamera = value; NotifyPropertyChange(nameof(SelectCamera)); }
        }

        private string? _selectLens;

        public string? SelectLens
        {
            get { return _selectLens; }
            set { _selectLens = value; NotifyPropertyChange(nameof(SelectLens)); }
        }

        private int _workingDistance;

        public int WorkingDistance
        {
            get { return _workingDistance; }
            set { _workingDistance = value; NotifyPropertyChange(nameof(WorkingDistance)); }
        }

        private string? _focalLength;

        public string? FocalLength
        {
            get { return _focalLength; }
            set { _focalLength = value; NotifyPropertyChange(nameof(FocalLength)); }
        }

        private string? _hAngle;

        public string? HAngle
        {
            get { return _hAngle; }
            set { _hAngle = value; NotifyPropertyChange(nameof(HAngle)); }
        }

        private string? _vAngle;

        public string? VAngle
        {
            get { return _vAngle; }
            set { _vAngle = value; NotifyPropertyChange(nameof(VAngle)); }
        }

        private string? _viewCalcWidth;

        public string? ViewCalcWidth
        {
            get { return _viewCalcWidth; }
            set { _viewCalcWidth = value; NotifyPropertyChange(nameof(ViewCalcWidth)); }
        }

        private string? _viewCalcHeight;

        public string? ViewCalcHeight
        {
            get { return _viewCalcHeight; }
            set { _viewCalcHeight = value; NotifyPropertyChange(nameof(ViewCalcHeight)); }
        }

        private string? _viewCalcTimes;

        public string? ViewCalcTimes
        {
            get { return _viewCalcTimes; }
            set { _viewCalcTimes = value; NotifyPropertyChange(nameof(ViewCalcTimes)); }
        }


        private CameraData? _cameraData;
        private LensData? _lensData;
        private double _chipWidth, _chipHeight, _pixelSize, _fLength, _chipSize, _lenMatchChip;
        private int _workingDis;
        public DelegateCommand RoughCalcCommand { get; }

        public MainWindowViewModel()
        {
            SelectCameraList = new List<string>();
            AllCameraData = SqlHelper.AsyncQuery<CameraData>("SELECT * FROM camera").Result.ToList();
            AllLensData = SqlHelper.AsyncQuery<LensData>("SELECT * FROM lens").Result.ToList();
            AllCameraData.ForEach((data) => SelectCameraList.Add(data.Model));

            RoughCalcCommand = new DelegateCommand { ActionExecute = RoughCalc };
        }

        private void RoughCalc(object obj)
        {
            RoughCameraList = new ObservableCollection<CameraShowData>();

            if (Accuracy <= 0 || ViewWidth <= 0 || ViewHeight <= 0 || SafetyFactor <= 0)
            {
                MessageBox.Show("必须将参数输入完整再进行计算！！");
                return;
            }

            if (SafetyFactor < 3) SafetyFactor = 3;

            // 相机像素 = 安全系数*视场宽*视场高/(设备精度*设备精度)
            int roughResult = (int)Math.Floor(SafetyFactor * ViewHeight * ViewWidth / (Accuracy * Accuracy) / 10000) * 10000;

            int minSafeFactor = SafetyFactor - 2 >= 3 ? SafetyFactor - 2 : 3;
            int minRought = roughResult / SafetyFactor * minSafeFactor;
            int maxRought = roughResult / SafetyFactor * (SafetyFactor + 2);

            string sql = $"SELECT * FROM camera WHERE Pixels >= {minRought} AND Pixels <= {maxRought}";
            var allDatas = SqlHelper.AsyncQuery<CameraData>(sql);

            allDatas.Result.ToList().ForEach(data => RoughCameraList.Add(new CameraShowData
            {
                Vendors = data.Vendors,
                Model = data.Model,
                Interface = data.Interface,
                Shutter = data.Shutter,
                ChipSize = data.ChipSize,
                PixelSize = data.PixelSize,
                Frame = data.Frame,
                Color = data.Color,
                Pixels = $"{data.Pixels / 10000}万像素"
            }));

            MessageBox.Show($"预估相机分辨率完成，请到第二页查看结果。\n根据软件中的相机数据，一共查询到{allDatas.Result.Count}个匹配相机。");
        }

        public void LensChange(string value)
        {
            if (string.IsNullOrEmpty(SelectCamera))
            {
                MessageBox.Show("请先确定相机型号！");
                SelectLens = string.Empty;
                return;
            }

            string sql = $"SELECT * FROM camera WHERE Model=\'{SelectCamera}\'";
            _cameraData = GetSingleData<CameraData>(sql);
            if (_cameraData == null) return;

            if (_cameraData.ChipSize.Contains("\""))
            {
                GetDoubleForStr(_cameraData.ChipSize, ref _chipSize);
                //string chip = _cameraData.ChipSize.Split('\"')[0];
                //_chipSize = double.Parse(chip);
            }
            else
            {
                _chipSize = 0;
            }

            SelectLens = value;

            if (string.IsNullOrWhiteSpace(SelectLens))
                return;

            sql = $"SELECT * FROM lens WHERE Model=\'{SelectLens}\'";
            _lensData = GetSingleData<LensData>(sql);
            if (_lensData == null) return;

            LensDataGetForStrSplit(_lensData);

            FocalLength = _lensData.FocalLength;

            CalcChipSize(_cameraData);

            _fLength = double.Parse(_lensData.FocalLength);

            //水平视场角 = 2 * arctan(w / 2f)
            //垂直视场角 = 2 * arctan(h / 2f)
            double arc = Math.Atan(_chipWidth / (2 * _fLength));
            double angle = arc / Math.PI * 180;
            HAngle = Math.Round(2 * angle, 2).ToString() + "°";

            arc = Math.Atan(_chipHeight / (2 * _fLength));
            angle = arc / Math.PI * 180;
            VAngle = Math.Round(2 * angle, 2).ToString() + "°";

        }

        private void LensDataGetForStrSplit(LensData lensData)
        {
            if (lensData.MatchingChip.Contains("|"))
            {
                string matchChip = lensData.MatchingChip.Split('|')[0].Trim();

                GetDoubleForStr(matchChip, ref _lenMatchChip);
                //string match = matchChip.Split('\"')[0];
                //_lenMatchChip = double.Parse(match);
            }
            else if (lensData.MatchingChip.Contains("\""))
            {
                GetDoubleForStr(lensData.MatchingChip, ref _lenMatchChip);
                //string matchChip = lensData.MatchingChip.Split('\"')[0];
                //_lenMatchChip = double.Parse(matchChip);
            }
            else
            {
                _lenMatchChip = 0;
            }

            if (lensData.WorkingDistance.Contains("-"))
            {
                string dis = lensData.WorkingDistance.Split('-')[0];
                _workingDis = int.Parse(dis);
            }
            else
            {
                _workingDis = int.Parse(lensData.WorkingDistance);
            }
        }

        private void GetDoubleForStr(string value, ref double target)
        {
            string temp = value.Split('\"')[0];
            if (temp.Contains("/"))
            {
                string[] tempArr = temp.Split('/');
                target = double.Parse(tempArr[0]) / double.Parse(tempArr[1]);
            }
            else
            {
                target = double.Parse(temp);
            }
        }

        private void CalcChipSize(CameraData cameraData)
        {
            if (string.IsNullOrEmpty(cameraData.ChipHeight))
            {
                int pixelHeight = int.Parse(cameraData.PixelHeight);
                int pixelWidth = int.Parse(cameraData.PixelWidth);
                _pixelSize = double.Parse(cameraData.PixelSize);
                _chipWidth = pixelWidth * _pixelSize / 1000;
                _chipHeight = pixelHeight * _pixelSize / 1000;
            }
            else
            {
                _pixelSize = double.Parse(cameraData.PixelSize);
                _chipWidth = double.Parse(cameraData.ChipWidth);
                _chipHeight = double.Parse(cameraData.ChipHeight);
            }
        }

        public void FinalCalcForWidth(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            ViewCalcWidth = value;
            double width = double.Parse(ViewCalcWidth);
            double times = _chipWidth / width;
            double height = _chipHeight / times;
            double focalLength = _fLength / times + _fLength;

            ViewCalcTimes = Math.Round(times, 6).ToString();
            ViewCalcHeight = Math.Round(height, 2).ToString();
            WorkingDistance = (int)Math.Floor(focalLength);

            CheckLensAndCamera();
        }

        public void FinalCalcForHeight(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            ViewCalcHeight = value;
            double height = double.Parse(ViewCalcHeight);
            double times = _chipHeight / height;
            double width = _chipWidth / times;
            double focalLength = _fLength / times + _fLength;

            ViewCalcTimes = Math.Round(times, 6).ToString();
            ViewCalcWidth = Math.Round(width, 2).ToString();
            WorkingDistance = (int)Math.Floor(focalLength);

            CheckLensAndCamera();
        }

        public void FinalCalcForTimes(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            ViewCalcTimes = value;
            double times = double.Parse(ViewCalcTimes);
            double width = _chipWidth / times;
            double height = _chipHeight / times;
            double focalLength = _fLength / times + _fLength;

            ViewCalcWidth = Math.Round(width, 2).ToString();
            ViewCalcHeight = Math.Round(height, 2).ToString();
            WorkingDistance = (int)Math.Floor(focalLength);

            CheckLensAndCamera();
        }

        private void CheckLensAndCamera()
        {
            if (_chipSize == 0 || _lenMatchChip == 0)
            {
                MessageBox.Show("软件数据中缺小相机芯片大小数据或缺小镜头适配芯片数据，请人工确认相机镜头是否匹配！");
                return;
            }

            if (_lenMatchChip < _chipSize)
            {
                MessageBox.Show("镜头适配的芯片大小不适合此相机，请重新选择相机或者镜头！");
                return;
            }

            if (_workingDis > WorkingDistance)
            {
                MessageBox.Show("镜头最小工作距离大于核算出来的工作距离，相机和镜头不匹配，请重新选择相机或镜头！");
                return;
            }
        }

        private T GetSingleData<T>(string sql) where T : class
        {
            var result = SqlHelper.AsyncQuery<T>(sql);

            if (result.Result.Count <= 0)
            {
                MessageBox.Show($"软件数据中不存在此型号的数据：{SelectLens}");
                return null;
            }

            return result.Result[0];
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
