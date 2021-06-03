using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AutoBuildConfig.ViewModel
{
    public class SystemCfgViewModel : ViewModelBase
    {
        public ICommand SaveConfigCommand { get; set; }
        public ICommand LoadConfigCommand { get; set; }
        public ICommand SaveAsConfigCommand { get; set; }
        
        private SystemCfg systemCfg;

        public SystemCfg SystemCfg
        {
            get { return systemCfg; }
            set { Set(ref systemCfg, value); }
        }

        public SystemCfgViewModel()
        {
            LoadConfig();
            InitCommand();
        }

        private void LoadConfig()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "systemCfg.json";

            if (!File.Exists(file))
            {
                SystemCfg = new SystemCfg
                {
                    IoInput = new List<IoInputPoint>(),
                    IoOutput = new List<IoOutputPoint>(),
                    IoCardsList = new List<IoCardInfo>(),
                    SysInput = new List<SysInputPoint>(),
                    SysOutput = new List<SysOutputPoint>(),
                    MotionCardsList = new List<MotionCard>(),
                    EthInfos = new List<EthInfo>(),
                    StationInfos = new List<StationInfo>()
                };
            }
            else
            {
                SystemCfg = JsonConvert.DeserializeObject<SystemCfg>(File.ReadAllText(file));
            }
        }
        private void InitCommand()
        {
            SaveConfigCommand = new RelayCommand(SaveConfig);
            LoadConfigCommand = new RelayCommand(LoadConfigFromFile);
            SaveAsConfigCommand = new RelayCommand(SaveAsConfig);
        }

        private void LoadConfigFromFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                DefaultExt = ".json",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            var result = ofd.ShowDialog();

            if (result == true)
            {
                var file = ofd.FileName;
                try
                {
                    SystemCfg = JsonConvert.DeserializeObject<SystemCfg>(File.ReadAllText(file));
                }
                catch (Exception e)
                {
                    MessageBox.Show("文件错误" + e, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    throw;
                }
                MessageBox.Show("加载成功");
            }
            else
            {
                MessageBox.Show("请选择json文件");
            }

        }

        private void SaveAsConfig()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                DefaultExt = ".json",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            var result = sfd.ShowDialog();
            if (result == true)
            {
                var file = sfd.FileName;
                string value = JsonConvert.SerializeObject(SystemCfg);

                File.WriteAllText(file, value);
                MessageBox.Show("保存成功");
            }
            else
            {
                MessageBox.Show("未指定json文件");
            }
        }

        private void SaveConfig()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "systemCfg.json";
            string value = JsonConvert.SerializeObject(SystemCfg);
            File.WriteAllText(file, value);

            MessageBox.Show("保存成功！", "提示");
        }
    }

    public class IoInputPoint
    {
        public string CardIndex { get; set; }
        public string PointIndex { get; set; }
        public string PointName { get; set; }
        public string PointEngName { get; set; }
    }

    public class IoOutputPoint
    {
        public string CardIndex { get; set; }
        public string PointIndex { get; set; }
        public string PointName { get; set; }
        public string PointEngName { get; set; }
    }

    public class IoCardInfo
    {
        public string CardIndex { get; set; }
        public string CardNum { get; set; }
        public string CardType { get; set; }
    }

    public class SysInputPoint
    {
        public string FuncDesc { get; set; }
        public string CardNum { get; set; }
        public string PointIndex { get; set; }
        public string EffectiveLevel { get; set; }
    }
    public class SysOutputPoint
    {
        public string FuncDesc { get; set; }
        public string CardNum { get; set; }
        public string PointIndex { get; set; }
        public string EffectiveLevel { get; set; }
    }

    public class MotionCard
    {
        public string Index { get; set; }
        public string CardType { get; set; }
        public string MinAxisNum { get; set; }
        public string MaxAxisNum { get; set; }
    }

    public class EthInfo
    {
        public string EthNum { get; set; }
        public string EthDefine { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public string TimeOut { get; set; }
        public string Command { get; set; }
    }

    public class StationInfo
    {
        public string StationIndex { get; set; }
        public string StationName { get; set; }
        public string AxisX { get; set; }
        public string AxisY { get; set; }
        public string AxisZ { get; set; }
        public string AxisU { get; set; }
        public string AxisA { get; set; }
        public string AxisB { get; set; }
        public string AxisC { get; set; }
        public string AxisD { get; set; }
    }

    public class SystemCfg
    {
        private List<IoInputPoint> ioInput;

        public List<IoInputPoint> IoInput
        {
            get { return ioInput; }
            set { ioInput = value; }
        }

        private List<IoOutputPoint> ioOutput;

        public List<IoOutputPoint> IoOutput
        {
            get { return ioOutput; }
            set { ioOutput = value; }
        }

        private List<IoCardInfo> ioCards;

        public List<IoCardInfo> IoCardsList
        {
            get { return ioCards; }
            set { ioCards = value; }
        }

        private List<SysInputPoint> sysInput;

        public List<SysInputPoint> SysInput
        {
            get { return sysInput; }
            set { sysInput = value; }
        }

        private List<SysOutputPoint> sysOutput;

        public List<SysOutputPoint> SysOutput
        {
            get { return sysOutput; }
            set { sysOutput = value; }
        }

        private List<MotionCard> motionCards;

        public List<MotionCard> MotionCardsList
        {
            get { return motionCards; }
            set { motionCards = value; }
        }

        private List<EthInfo> ethInfos;

        public List<EthInfo> EthInfos
        {
            get { return ethInfos; }
            set { ethInfos = value; }
        }

        private List<StationInfo> stationInfos;

        public List<StationInfo> StationInfos
        {
            get { return stationInfos; }
            set { stationInfos = value; }
        }
    }
}
