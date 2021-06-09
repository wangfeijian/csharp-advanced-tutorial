using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoBuildConfig.Model;
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

        public delegate void PropChange();

        public event PropChange PropChangeEvent;

        private List<StationPoint> stationPoints;

        private IBuildConfig BulidConfig;

        public List<StationPoint> StationPoints
        {
            get { return stationPoints; }
            set { Set(ref stationPoints, value); }
        }
        public PointViewModel(IBuildConfig buildConfig)
        {
            BulidConfig = buildConfig;
            StationPoints = BulidConfig.LoadConfig<List<StationPoint>>("point");
            InitCommand();
        }

        private void InitCommand()
        {
            SaveConfigCommand = new RelayCommand(SaveConfig);
            LoadConfigCommand = new RelayCommand(LoadConfigFromFile);
            SaveAsConfigCommand = new RelayCommand(SaveAsConfig);
        }

        private void LoadConfigFromFile()
        {
            try
            {
                StationPoints = BulidConfig.LoadConfigFromFile<List<StationPoint>>();
            }
            catch (Exception e)
            {
                MessageBox.Show("未选择文件！" + e);
                return;
            }
            PropChangeEvent?.Invoke();

        }

        private void SaveAsConfig()
        {
            BulidConfig.SaveAsConfig(StationPoints);
        }

        private void SaveConfig()
        {
            BulidConfig.SaveConfig(StationPoints, "point.json");
            MessageBox.Show("保存成功！");
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
