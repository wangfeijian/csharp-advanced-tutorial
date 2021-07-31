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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonTools.Model;
using CommonTools.Servers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using CommonTools.Tools;
using GalaSoft.MvvmLight.Messaging;

namespace CommonTools.ViewModel
{
    public class SysParamControlViewModel : ViewModelBase
    {
        public ICommand SaveConfigCommand { get; set; }
        public ICommand LoadConfigCommand { get; set; }
        public ICommand SaveAsConfigCommand { get; set; }
        public ICommand SaveDefaultConfigCommand { get; set; }
        public ICommand ResetDefaultConfigCommand { get; set; }
        public ICommand CellValueChangeCommand { get; set; }
        //public ICommand LoadingRowCommand { get; set; }


        private readonly IBuildConfig _bulidConfig;

        private Parameters _allParameters;

        public Parameters AllParameters
        {
            get { return _allParameters; }
            set { Set(ref _allParameters, value); }
        }

        public string FileDir { get; set; }

        public SysParamControlViewModel(IBuildConfig buildConfig, string dir)
        {
            FileDir = AppDomain.CurrentDomain.BaseDirectory + "Config\\" + dir;
            string fileName = FileDir + "\\systemParam";
            _bulidConfig = buildConfig;
            AllParameters = _bulidConfig.LoadConfig<Parameters>(fileName);
            InitCommand();
        }

        private void InitCommand()
        {
            SaveConfigCommand = new RelayCommand(SaveConfig);
            LoadConfigCommand = new RelayCommand(LoadConfigFromFile);
            SaveAsConfigCommand = new RelayCommand(SaveAsConfig);
            SaveDefaultConfigCommand = new RelayCommand(SaveDefaultConfig);
            ResetDefaultConfigCommand = new RelayCommand(LoadDefaultConfig);
            CellValueChangeCommand = new RelayCommand<object>(CellValueChangedCheck);
            //LoadingRowCommand = new RelayCommand<DataGridRowEventArgs>(LoadingRowInit);
        }

        //private void LoadingRowInit(DataGridRowEventArgs e)
        //{
        //    var dataRow = e.Row.DataContext as ParamInfo;
        //    if (dataRow != null)
        //    {
        //        string strValue = dataRow.CurrentValue;
        //        string strMin = dataRow.MinValue;
        //        string strMax = dataRow.MaxValue;

        //        if (strMin == string.Empty || strMax == string.Empty) return;
        //        double value = 0;
        //        try
        //        {
        //            value = Convert.ToDouble(strValue);
        //        }
        //        catch
        //        {
        //            dataRow.CurrentValue = strMin;
        //            //MessageBox.Show(LocationServices.GetLang("ParamError"), LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
        //            //Environment.Exit(0);
        //        }

        //        double min = Convert.ToDouble(strMin);
        //        double max = Convert.ToDouble(strMax);
        //        if (value > max || value < min)
        //        {
        //            dataRow.CurrentValue = strMin;
        //            //MessageBox.Show(LocationServices.GetLang("ParamError"), LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
        //            //Environment.Exit(0);
        //        }
        //    }
        //}

        private void CellValueChangedCheck(object obj)
        {
            DataGrid dataGrid = obj as DataGrid;

            var seleCellContent = dataGrid?.Columns[1].GetCellContent(dataGrid.Items[dataGrid.SelectedIndex]) as TextBox;

            if (seleCellContent != null)
            {
                string currentValue = seleCellContent.Text;
                string strValue = AllParameters.ParameterInfos[dataGrid.SelectedIndex].CurrentValue;
                string strMin = AllParameters.ParameterInfos[dataGrid.SelectedIndex].MinValue;
                string strMax = AllParameters.ParameterInfos[dataGrid.SelectedIndex].MaxValue;

                if (strMin == string.Empty || strMax == string.Empty) return;
                double value = 0;
                try
                {
                    value = Convert.ToDouble(currentValue);
                }
                catch
                {
                    ((TextBox)dataGrid.Columns[1].GetCellContent(dataGrid.Items[dataGrid.SelectedIndex])).Text = strValue;
                    MessageBox.Show(LocationServices.GetLang("ParamError"), LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                double min = Convert.ToDouble(strMin);
                double max = Convert.ToDouble(strMax);
                if (value > max || value < min)
                {
                    ((TextBox)dataGrid.Columns[1].GetCellContent(dataGrid.Items[dataGrid.SelectedIndex])).Text = strValue;
                    MessageBox.Show(LocationServices.GetLang("ParamError"), LocationServices.GetLang("Tips"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void LoadDefaultConfig()
        {
            string file = FileDir + "\\systemParamDefault";
            try
            {
                AllParameters = _bulidConfig.LoadConfig<Parameters>(file);
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
            string file = FileDir + "\\systemParamDefault";
            _bulidConfig.SaveConfig(AllParameters, file);
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
            string file = FileDir + "\\systemParam";
            _bulidConfig.SaveConfig(AllParameters, file);
            MessageBox.Show(LocationServices.GetLang("SaveSuccess"));
        }

    }
}
