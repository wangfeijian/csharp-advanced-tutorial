/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-07                               *
*                                                                    *
*           ModifyTime:     2021-08-07                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Station point view model class           *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMationFrameworkModel;
using ConfigTools;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace AutoMationFrameworkViewModel
{
    public class PointConfigViewModel: ViewModelBase
    {
        public ICommand SaveConfigCommand { get; set; }
        public ICommand LoadConfigCommand { get; set; }
        public ICommand SaveAsConfigCommand { get; set; }

        public delegate void PropChange();

        public event PropChange PropChangeEvent;

        private List<StationPoint> stationPoints;

        private IBuildConfig BulidConfig;

         public string FileDir { get; set; }

        public List<StationPoint> StationPoints
        {
            get { return stationPoints; }
            set { Set(ref stationPoints, value); }
        }
        public PointConfigViewModel(IBuildConfig buildConfig,string dir)
        {
            FileDir = AppDomain.CurrentDomain.BaseDirectory + "Config\\" + dir;
            string fileName = FileDir + "\\point";
            BulidConfig = buildConfig;
            StationPoints = BulidConfig.LoadConfig<List<StationPoint>>(fileName);
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
                StationPoints = BulidConfig.LoadConfigFromFile<List<StationPoint>>("point");
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
            BulidConfig.SaveConfig(StationPoints, "point");
            MessageBox.Show("保存成功！");
        }
    }
}
