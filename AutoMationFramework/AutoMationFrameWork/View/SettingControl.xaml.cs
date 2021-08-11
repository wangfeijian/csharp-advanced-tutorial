/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-06-22                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    UserControl for setting back code        *
*********************************************************************/

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AutoMationFrameworkDll;
using AutoMationFrameworkModel;
using AutoMationFrameworkViewModel;
using CommonTools.Tools;
using GalaSoft.MvvmLight.Ioc;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// SettingControl.xaml 的交互逻辑
    /// </summary>
    public partial class SettingControl
    {
        public SettingControl()
        {
            InitializeComponent();
            CustomConfig.AddStation();
            AddStationControl();
        }

        private void AddStationControl()
        {
            foreach (var stationInfo in StationManager.GetInstance().DicControlStation)
            {
                TabItem item = new TabItem
                {
                    Header = stationInfo.Value.StrName,
                    Style   = (Style) Application.Current.Resources["TabItemStyle"],
                    Content = stationInfo.Key
                };

                StationTabControl.Items.Add(item);
            }
        }
    }
}
