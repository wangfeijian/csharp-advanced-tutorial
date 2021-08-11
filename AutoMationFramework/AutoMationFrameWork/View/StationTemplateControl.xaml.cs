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
using System.Windows;
using AutoMationFrameworkDll;
using MaterialDesignThemes.Wpf;
using ToolExtensions;
using System.Linq;
using System.Windows.Controls;

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

        private int _curPage = -1; //0 - XYZU  1 - ABCD

        public StationTemplateControl()
        {
            InitializeComponent();
        }

        private void StationTemplateControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            StationBase sta = ManualOperation.GetStation(this);
            if (sta!=null)
            {
                string[] strAxisName = { "X", "Y", "Z", "U" };
                for (int i = 0; i < _packIcons.GetLength(0); i++)
                {
                    for (int j = 0; j < _packIcons.GetLength(1); j++)
                    {
                        _packIcons[i, j] = (PackIcon) PackIconGrid.FindName("PackIconState" + strAxisName[i] + j);
                    }

                    _showAxisNumBlocks[i] = (TextBlock) PackIconGrid.FindName("ShowAxis" + strAxisName[i]);
                    _showAxisNumBlocks[i].Text = sta.StrAxisName[i];
                    ((TextBlock) PackIconGrid.FindName("ParamAxis" + strAxisName[i])).Text = sta.StrAxisName[i];
                    _axisPosBlocks[i]=(TextBox)PackIconGrid.FindName("AxisPos" + strAxisName[i]);
                    _axisTarBlocks[i]=(TextBox)PackIconGrid.FindName("AxisTar" + strAxisName[i]);
                    _axisSpeedBlocks[i]=(TextBox)PackIconGrid.FindName("AxisSpeed" + strAxisName[i]);
                }

                SwitchToXYZU();
            }
        }

        private void SwitchToXYZU()
        {
            if (_curPage != 0)
            {
                _curPage = 0;

                ButtonABCD.IsEnabled = true;
                ButtonXYZU.IsEnabled = false;

                UpdateUI();
            }
        }

        private void SwitchToABCD()
        {
            if (_curPage != 1)
            {
                _curPage = 1;

                ButtonABCD.IsEnabled = false;
                ButtonXYZU.IsEnabled = true;

                UpdateUI();
            }
        }

        private void UpdateUI()
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
                    PackIconGrid.RowDefinitions[i+1].Height = new GridLength(0);
                    AxisPosParamGird.RowDefinitions[i+1].Height = new GridLength(0);

                    // ReSharper disable once PossibleNullReferenceException
                    ((TextBlock) FindName("TextBlockPositive" + (i + 1))).Visibility = Visibility.Hidden;
                    // ReSharper disable once PossibleNullReferenceException
                    ((TextBlock) FindName("TextBlockNegtive" + (i + 1))).Visibility = Visibility.Hidden;
                    // ReSharper disable once PossibleNullReferenceException
                    ((Button) FindName("ButtonPositive" + (i + 1))).Visibility = Visibility.Hidden;
                    // ReSharper disable once PossibleNullReferenceException
                    ((Button) FindName("ButtonNegtive" + (i + 1))).Visibility = Visibility.Hidden;
                }
                else
                {
                    PackIconGrid.RowDefinitions[i+1].Height = GridLength.Auto;
                    AxisPosParamGird.RowDefinitions[i+1].Height = GridLength.Auto;

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

            if (b != null && b.Name =="ButtonXYZU")
            {
                SwitchToXYZU();
            }
            else
            {
                SwitchToABCD();
            }
        }
    }
}
