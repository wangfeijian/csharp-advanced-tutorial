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
        public List<StationInfo> StationList = SimpleIoc.Default.GetInstance<SystemConfigViewModel>().SystemConfig.StationInfos;
        public SettingControl()
        {
            InitializeComponent();
            AddStationControl();
        }

        private void AddStationControl()
        {
            foreach (var stationInfo in StationList)
            {
                TabItem item = new TabItem
                {
                    Header = LocationServices.GetLangType() == "en-us"
                        ? stationInfo.StationEngName
                        : stationInfo.StationName,
                    Style   = (Style) Application.Current.Resources["TabItemStyle"],
                    Content = new StationTemplateControl()
                };

                StationTabControl.Items.Add(item);
            }
        }
    }
}
