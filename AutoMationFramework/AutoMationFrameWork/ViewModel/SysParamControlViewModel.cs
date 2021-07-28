/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-27                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    System parameter control view model      *
*********************************************************************/
using System;
using System.Windows;
using System.Windows.Input;
using AutoMationFrameWork.Model;
using AutoMationFrameWork.Servers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using CommonTools.Tools;

namespace AutoMationFrameWork.ViewModel
{
    public class SysParamControlViewModel : ViewModelBase
    {
        public ICommand SaveConfigCommand { get; set; }
        public ICommand LoadConfigCommand { get; set; }
        public ICommand SaveAsConfigCommand { get; set; }
        public ICommand SaveDefaultConfigCommand { get; set; }
        public ICommand ResetDefaultConfigCommand { get; set; }


        private readonly IBuildConfig _bulidConfig;

        private Parameters _allParameters;

        public Parameters AllParameters
        {
            get { return _allParameters; }
            set { Set(ref _allParameters, value); }
        }

        public SysParamControlViewModel(IBuildConfig buildConfig)
        {
            _bulidConfig = buildConfig;
            AllParameters = _bulidConfig.LoadConfig<Parameters>("systemParam");
            InitCommand();
        }

        private void InitCommand()
        {
            SaveConfigCommand = new RelayCommand(SaveConfig);
            LoadConfigCommand = new RelayCommand(LoadConfigFromFile);
            SaveAsConfigCommand = new RelayCommand(SaveAsConfig);
            SaveDefaultConfigCommand = new RelayCommand(SaveDefaultConfig);
            ResetDefaultConfigCommand = new RelayCommand(LoadDefaultConfig);
        }

        private void LoadDefaultConfig()
        {
            try
            {
                AllParameters = _bulidConfig.LoadConfig<Parameters>("systemParamDefault");
            }
            catch (Exception e)
            {
                MessageBox.Show(LocationServices.GetLang("NoDefaultConfig") + e);
                return;
            }

            MessageBox.Show(LocationServices.GetLang("LoadDefaultConfigSuccess"));
        }

        private void SaveDefaultConfig()
        {
            _bulidConfig.SaveConfig(AllParameters, "systemParamDefault");
            MessageBox.Show(LocationServices.GetLang("SaveDefaultConfigSuccess"));
        }

        private void LoadConfigFromFile()
        {
            try
            {
                AllParameters = _bulidConfig.LoadConfigFromFile<Parameters>("systemParam");
            }
            catch (Exception e)
            {
                MessageBox.Show(LocationServices.GetLang("UnselectedFile") + e);
            }

        }

        private void SaveAsConfig()
        {
            _bulidConfig.SaveAsConfig(AllParameters);
        }

        private void SaveConfig()
        {
            _bulidConfig.SaveConfig(AllParameters, "systemParam");
            MessageBox.Show(LocationServices.GetLang("SaveSuccess"));
        }

    }
}
