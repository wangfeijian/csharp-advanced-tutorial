using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace AutoBuildConfig.ViewModel
{
    public class PointViewModel : ViewModelBase
    {
        public ICommand SaveConfigCommand { get; set; }
        public ICommand LoadConfigCommand { get; set; }
        public ICommand SaveAsConfigCommand { get; set; }
        public SystemCfg SystemCfg { get; set; }

        private List<StationPoint> stationPoints;

        public List<StationPoint> StationPoints
        {
            get { return stationPoints; }
            set { Set(ref stationPoints, value); }
        }
        public PointViewModel()
        {

            LoadConfig();
            InitCommand();
        }

        private void LoadConfig()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "point.json";

            if (!File.Exists(file))
            {
                string sysFile = AppDomain.CurrentDomain.BaseDirectory + "systemCfg.json";

                SystemCfg = JsonConvert.DeserializeObject<SystemCfg>(File.ReadAllText(sysFile));

                StationPoints = new List<StationPoint>();

                foreach (var station in SystemCfg.StationInfos)
                {
                    StationPoints.Add(new StationPoint { Name = station.StationName, PointInfos = new List<PointInfo>() });
                }
            }
            else
            {
                StationPoints = JsonConvert.DeserializeObject<List<StationPoint>>(File.ReadAllText(file));
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
                    StationPoints = JsonConvert.DeserializeObject<List<StationPoint>>(File.ReadAllText(file));
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
                string value = JsonConvert.SerializeObject(StationPoints);

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
            string file = AppDomain.CurrentDomain.BaseDirectory + "point.json";
            string value = JsonConvert.SerializeObject(StationPoints);
            File.WriteAllText(file, value);

            MessageBox.Show("保存成功！", "提示");
        }
      

    }

    public class PointInfo
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string XPos { get; set; }
        public string YPos { get; set; }
        public string ZPos { get; set; }
        public string UPos { get; set; }
        public string APos { get; set; }
        public string BPos { get; set; }
        public string CPos { get; set; }
        public string DPos { get; set; }
    }

    public class StationPoint
    {
        public string Name { get; set; }
        public List<PointInfo> PointInfos { get; set; }
    }
}
