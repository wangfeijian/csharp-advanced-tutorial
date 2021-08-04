using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMationFrameworkModel;
using CommonTools.Tools;
using ConfigTools;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace AutoMationFrameworkViewModel
{
    public class SystemConfigViewModel : ViewModelBase
    {

        public ICommand SaveConfigCommand { get; private set; }
        public ICommand UpdateConfigCommand { get; private set; }

        private readonly IBuildConfig _bulidConfig;

        public string FileDir { get; set; }

        private SystemCfg systemConfig;

        public SystemCfg SystemConfig
        {
            get { return systemConfig; }
            set { Set(ref systemConfig, value); }
        }

        public SystemConfigViewModel(IBuildConfig buildConfig, string dir)
        {
            FileDir = AppDomain.CurrentDomain.BaseDirectory + "Config\\" + dir;
            string fileName = FileDir + "\\systemCfg";
            _bulidConfig = buildConfig;
            SystemConfig = _bulidConfig.LoadConfig<SystemCfg>(fileName);
            InitCommand();
        }

        private void InitCommand()
        {
            SaveConfigCommand = new RelayCommand(SaveConfig);
            UpdateConfigCommand = new RelayCommand(UpdateConfig);
        }

        private void SaveConfig()
        {
            string file = FileDir + "\\systemCfg";
            _bulidConfig.SaveConfig(SystemConfig, file);
            MessageBox.Show(LocationServices.GetLang("SaveSuccess"));
        }

        private void UpdateConfig()
        {
            Messenger.Default.Send(SystemConfig, "axisCfg");
        }

    }
}
