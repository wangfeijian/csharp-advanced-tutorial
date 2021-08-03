/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-06-29                               *
*                                                                    *
*           ModifyTime:     2021-07-27                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    UserControl for axis back code           *
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using CommonTools.Model;
using HalconDotNet;
using CommonTools.Tools;
using CommonTools.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using MotionIO;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// AxisConfigControl.xaml 的交互逻辑
    /// </summary>
    public partial class AxisConfigControl
    {
        public List<string> BoolValue = new List<string> { "True", "False" };
        public AxisConfigControl()
        {
            InitializeComponent();
            SpelBox.ItemsSource = BoolValue;
            SmelBox.ItemsSource = BoolValue;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            if (e.AddedItems.Count>0)
            {
                var home = e.AddedItems.OfType<HomeMode>();
                var homeModes = home as HomeMode[] ?? home.ToArray();
                if (homeModes.Length>0)
                {
                    var item = homeModes.First();
                    int index = (int) item;
                    HomeTextBlock.Text = LocationServices.GetLang(item.ToString());
                    string path = dir + $"Resources\\AxisHome\\{index}.png";
                    if (File.Exists(path))
                    {
                        ImageAxis.Source = new BitmapImage(new Uri(path));
                    }
                }
            }
        }

    }
}
