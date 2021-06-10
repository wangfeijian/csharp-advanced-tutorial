using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using AutoBuildConfig.Model;
using GalaSoft.MvvmLight.Command;

namespace AutoBuildConfig.ViewModel
{
    public class ParameterViewModel:ViewModelBase
    {
        public ICommand SaveConfigCommand { get; set; }
        public ICommand LoadConfigCommand { get; set; }
        public ICommand SaveAsConfigCommand { get; set; }


        private IBuildConfig BulidConfig;

        private Parameters allParameters;

        public Parameters AllParameters
        {
            get { return allParameters; }
            set { Set(ref allParameters , value); }
        }

        public ParameterViewModel(IBuildConfig buildConfig)
        {
            BulidConfig = buildConfig;
            AllParameters = BulidConfig.LoadConfig<Parameters>("systemParam");
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
                AllParameters = BulidConfig.LoadConfigFromFile<Parameters>("systemParam");
            }
            catch (Exception e)
            {
                MessageBox.Show("未选择文件！" + e);
                return;
            }

        }

        private void SaveAsConfig()
        {
            BulidConfig.SaveAsConfig(AllParameters);
        }

        private void SaveConfig()
        {
            BulidConfig.SaveConfig(AllParameters,"systemParam");
            MessageBox.Show("保存成功！");
        }

    }

    public class ParameterInfo
    {
        public string KeyValue { get; set; }
        public string CurrentValue { get; set; }
        public string Unit { get; set; }
        public string ParamDesc { get; set; }
        public string EnglishDesc { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public string Authority { get; set; }
    }

    public class Parameters
    {
        private List<ParameterInfo> parameterInfos;

        public List<ParameterInfo> ParameterInfos
        {
            get { return parameterInfos; }
            set { parameterInfos = value; }
        }

    }
}
