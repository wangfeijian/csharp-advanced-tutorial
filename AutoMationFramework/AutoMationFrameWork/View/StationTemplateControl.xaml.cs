/*********************************************************************
*           Author:        wangfeijian                               *
*                                                                    *
*           CreatTime:     2021-06-29                                *
*                                                                    *
*           ModifyTime:    2021-07-27                                *
*                                                                    *
*           Email:         wangfeijianhao@163.com                    *
*                                                                    *
*           Description:   UserControl for station template back code*
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Windows;
using AutoMationFrameworkDll;
using MaterialDesignThemes.Wpf;
using ToolExtensions;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AutoMationFrameWork.View
{
    /// <summary>
    /// StationTemplateControl.xaml 的交互逻辑
    /// </summary>
    public partial class StationTemplateControl
    {
        private PackIcon[,] _packIcons = new PackIcon[4, 9];
        private TextBlock[] _showAxisNumBlocks = new TextBlock[4];
        private TextBox[] _axisPosBlocks = new TextBox[4];
        private TextBox[] _axisTarBlocks = new TextBox[4];
        private TextBox[] _axisSpeedBlocks = new TextBox[4];
        private Button[] _btnIn;   //输入IO按钮数组
        private Button[] _btnsOut;  //输出IO按钮数组
        private string[] _pointBindingStr = { "Index", "StrName", "X", "Y", "Z", "U", "A", "B", "C", "D" };
        private int _curPage = -1; //0 - XYZU  1 - ABCD

        public StationTemplateControl()
        {
            InitializeComponent();
        }

        private void StationTemplateControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            StationBase sta = ManualOperation.GetStation(this);
            if (sta != null)
            {
                string[] strAxisName = { "X", "Y", "Z", "U" };
                for (int i = 0; i < _packIcons.GetLength(0); i++)
                {
                    for (int j = 0; j < _packIcons.GetLength(1); j++)
                    {
                        _packIcons[i, j] = (PackIcon)PackIconGrid.FindName("PackIconState" + strAxisName[i] + j);
                    }

                    _showAxisNumBlocks[i] = (TextBlock)PackIconGrid.FindName("ShowAxis" + strAxisName[i]);
                    _showAxisNumBlocks[i].Text = sta.StrAxisName[i];
                    // ReSharper disable once PossibleNullReferenceException
                    ((TextBlock)PackIconGrid.FindName("ParamAxis" + strAxisName[i])).Text = sta.StrAxisName[i];
                    _axisPosBlocks[i] = (TextBox)PackIconGrid.FindName("AxisPos" + strAxisName[i]);
                    _axisTarBlocks[i] = (TextBox)PackIconGrid.FindName("AxisTar" + strAxisName[i]);
                    _axisSpeedBlocks[i] = (TextBox)PackIconGrid.FindName("AxisSpeed" + strAxisName[i]);
                }

                SwitchToXyzu();
                BindIoButton();

                //隐藏点位中不需要的轴，并把名称改为自定义名称
                for (int i = 0; i < sta.AxisCount; i++)
                {
                    int nAxisNo = sta.GetAxisNo(i);

                    if (nAxisNo > 0)
                    {
                        PointDatGrid.Columns[i + 2].Header = sta.GetAxisName(i);
                    }
                    else
                    {
                        PointDatGrid.Columns[i + 2].Visibility = Visibility.Collapsed;
                    }
                }

                List<PointInfo> pointInfos = new List<PointInfo>(sta.DicPoint.Values); 
                PointDatGrid.ItemsSource = pointInfos;

                int index = 0;
                foreach (var dataGridColumn in PointDatGrid.Columns)
                {
                    if (dataGridColumn.Visibility == Visibility.Visible)
                    {
                        ((System.Windows.Controls.DataGridTextColumn)dataGridColumn).Binding = new Binding(_pointBindingStr[index]) {Mode = BindingMode.TwoWay};
                        index++;
                        continue;
                    }

                    index++;
                }

            }
        }

        private void SwitchToXyzu()
        {
            if (_curPage != 0)
            {
                _curPage = 0;

                ButtonABCD.IsEnabled = true;
                ButtonXYZU.IsEnabled = false;

                UpdateUi();
            }
        }

        private void SwitchToAbcd()
        {
            if (_curPage != 1)
            {
                _curPage = 1;

                ButtonABCD.IsEnabled = false;
                ButtonXYZU.IsEnabled = true;

                UpdateUi();
            }
        }

        /// <summary>
        /// 创建IO输入输出按钮并声明按钮点击事件
        /// </summary>
        private void BindIoButton()
        {
            StationBase sta = ManualOperation.GetStation(this);
            _btnIn = new Button[sta.IoIn.Length];
            _btnsOut = new Button[sta.IoOut.Length];
            int row = sta.IoIn.Length % 2 == 0 ? sta.IoIn.Length / 2 : sta.IoIn.Length / 2 + 1;

            for (int i = 0; i < row; i++)
            {
                IoInGrid.RowDefinitions.Add(new RowDefinition());
            }

            row = sta.IoOut.Length % 2 == 0 ? sta.IoOut.Length / 2 : sta.IoOut.Length / 2 + 1;

            for (int i = 0; i < row; i++)
            {
                IoOutGrid.RowDefinitions.Add(new RowDefinition());
            }

            int index = 0;
            for (int i = 0; i < IoInGrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < IoInGrid.ColumnDefinitions.Count; j++)
                {
                    if (index == sta.IoIn.Length)
                    {
                        break;
                    }
                    Button tempButton = new Button();
                    tempButton.Template = (ControlTemplate)Application.Current.Resources["IoButtonTemplate"];
                    tempButton.Content = sta.IoIn[index];
                    tempButton.Foreground = Brushes.Gray;
                    tempButton.IsEnabled = false;
                    IoInGrid.Children.Add(tempButton);
                    Grid.SetRow(tempButton, i);
                    Grid.SetColumn(tempButton, j);
                    _btnIn[index] = tempButton;
                    index++;
                }
            }

            index = 0;
            for (int i = 0; i < IoOutGrid.RowDefinitions.Count; i++)
            {
                for (int j = 0; j < IoOutGrid.ColumnDefinitions.Count; j++)
                {
                    if (index == sta.IoOut.Length)
                    {
                        break;
                    }
                    Button tempButton = new Button();
                    tempButton.Template = (ControlTemplate)Application.Current.Resources["IoButtonTemplate"];
                    tempButton.Content = sta.IoOut[index];
                    tempButton.Foreground = Brushes.Gray;
                    IoOutGrid.Children.Add(tempButton);
                    Grid.SetRow(tempButton, i);
                    Grid.SetColumn(tempButton, j);
                    _btnsOut[index] = tempButton;
                    index++;
                }
            }
        }

        private void UpdateUi()
        {
            StationBase sta = ManualOperation.GetStation(this);
            if (sta == null)
            {
                return;
            }

            TextBlock[] btnPositiveTextBlocks = { TextBlockPositive1, TextBlockPositive2, TextBlockPositive3, TextBlockPositive4 };
            TextBlock[] btnNegtiveTextBlocks = { TextBlockNegtive1, TextBlockNegtive2, TextBlockNegtive3, TextBlockNegtive4 };
            TextBlock[] showAxisParamBlocks = { ParamAxisX, ParamAxisY, ParamAxisZ, ParamAxisU };
            Button[] btnsServoOn = { ButtonServerX, ButtonServerY, ButtonServerZ, ButtonServerU };

            for (int i = 0; i < 4; i++)
            {
                int nStartIndex = _curPage * 4 + i;

                _showAxisNumBlocks[i].Text = sta.StrAxisName[nStartIndex];
                showAxisParamBlocks[i].Text = sta.StrAxisName[nStartIndex];

                int nAxisNo = sta.GetAxisNo(nStartIndex);
                if (nAxisNo > 0)
                {
                    if (MotionManager.GetInstance().GetServoState(nAxisNo))
                    {
                        btnsServoOn[i].Content = "OFF";
                    }
                    else
                    {
                        btnsServoOn[i].Content = "ON";
                    }
                }

                if (sta.PositiveMove[nStartIndex])
                {
                    btnPositiveTextBlocks[i].Text = sta.StrAxisName[nStartIndex] + "+";
                    btnNegtiveTextBlocks[i].Text = sta.StrAxisName[nStartIndex] + "-";
                }
                else
                {
                    btnPositiveTextBlocks[i].Text = sta.StrAxisName[nStartIndex] + "-";
                    btnNegtiveTextBlocks[i].Text = sta.StrAxisName[nStartIndex] + "+";
                }
            }




            HideUnuseControl(sta); //隐藏工站不用的控件
        }

        /// <summary>
        /// 隐藏工站不用的控件
        /// </summary>
        /// <param name="sta">工站对象</param>
        private void HideUnuseControl(StationBase sta)
        {
            for (int i = 0; i < 4; ++i)
            {
                int nStartIndex = _curPage * 4 + i;
                if (sta.GetAxisNo(nStartIndex) == 0)
                {
                    PackIconGrid.RowDefinitions[i + 1].Height = new GridLength(0);
                    AxisPosParamGird.RowDefinitions[i + 1].Height = new GridLength(0);

                    // ReSharper disable once PossibleNullReferenceException
                    ((TextBlock)FindName("TextBlockPositive" + (i + 1))).Visibility = Visibility.Hidden;
                    // ReSharper disable once PossibleNullReferenceException
                    ((TextBlock)FindName("TextBlockNegtive" + (i + 1))).Visibility = Visibility.Hidden;
                    // ReSharper disable once PossibleNullReferenceException
                    ((Button)FindName("ButtonPositive" + (i + 1))).Visibility = Visibility.Hidden;
                    // ReSharper disable once PossibleNullReferenceException
                    ((Button)FindName("ButtonNegtive" + (i + 1))).Visibility = Visibility.Hidden;
                }
                else
                {
                    PackIconGrid.RowDefinitions[i + 1].Height = GridLength.Auto;
                    AxisPosParamGird.RowDefinitions[i + 1].Height = GridLength.Auto;

                    // ReSharper disable once PossibleNullReferenceException
                    ((TextBlock)FindName("TextBlockPositive" + (i + 1))).Visibility = Visibility.Visible;
                    // ReSharper disable once PossibleNullReferenceException
                    ((TextBlock)FindName("TextBlockNegtive" + (i + 1))).Visibility = Visibility.Visible;
                    // ReSharper disable once PossibleNullReferenceException
                    ((Button)FindName("ButtonPositive" + (i + 1))).Visibility = Visibility.Visible;
                    // ReSharper disable once PossibleNullReferenceException
                    ((Button)FindName("ButtonNegtive" + (i + 1))).Visibility = Visibility.Visible;
                }
            }

        }

        private void SwitchAxis(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            if (b != null && b.Name == "ButtonXYZU")
            {
                SwitchToXyzu();
            }
            else
            {
                SwitchToAbcd();
            }
        }
    }
}
