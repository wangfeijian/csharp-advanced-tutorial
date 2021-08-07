﻿/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-06                               *
*                                                                    *
*           ModifyTime:     2021-08-06                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Station Manager class                    *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AutoMationFrameworkSystemDll;
using AutoMationFrameworkViewModel;
using CommonTools.Tools;
using GalaSoft.MvvmLight.Ioc;

namespace AutoMationFrameworkDll
{
    public class StationManager : SingletonPattern<StationManager>
    {
        /// <summary>
        /// 轴号枚举
        /// </summary>
        enum AxisNum
        {
            AxisX,
            AxisY,
            AxisZ,
            AxisU,
            AxisA,
            AxisB,
            AxisC,
            AxisD,
        }

        /// <summary>
        /// 点位枚举
        /// </summary>
        enum PointNum
        {
            XPos,
            YPos,
            ZPos,
            UPos,
            APos,
            BPos,
            CPos,
            DPos,
        }

        /// <summary>
        /// 弹窗计数改变委托
        /// </summary>
        /// <param name="nCount"></param>
        public delegate void ShowMessageCountChanged(int nCount);

        /// <summary>
        /// 定义一个站位状态变化委托函数
        /// </summary>
        /// <param name="OldState"></param>
        /// <param name="NewState"></param>
        public delegate void StateChangedHandler(StationState OldState, StationState NewState);

        private static object _lockCount = new object();

        /// <summary>
        /// 站位链表
        /// </summary>
        public List<StationBase> _lsStation = new List<StationBase>();

        /// <summary>
        /// 站位集合
        /// </summary>
        public Dictionary<Control, StationBase> DicFormStation = new Dictionary<Control, StationBase>();

        /// <summary>
        /// 站位与自定义手动界面的集合
        /// </summary>
        public Dictionary<Control, StationBase> DicManualFormStation = new Dictionary<Control, StationBase>();

        private StationState _nCurState = StationState.StateManual;

        private int _showMessageCount = 0;
        private bool _isAutoMode;

        /// <summary>
        /// 弹窗计数改变事件
        /// </summary>
        public event ShowMessageCountChanged OnShowMessageCountChanged;

        /// <summary>
        /// 当前是否全自动运行
        /// </summary>
        public bool BAutoMode
        {
            get
            {
                return _isAutoMode;
            }
            set
            {
                _isAutoMode = value;
            }
        }

        /// <summary>
        /// 读取系统配置文件里的站位信息
        /// </summary>
        public void ReadCardFromCfg()
        {
            _lsStation.Clear();

            var stationList = IoManager.GetInstance().SystemConfig.StationInfos;

            if (stationList.Count > 0)
            {
                foreach (var stationInfo in stationList)
                {
                    string stationName = LocationServices.GetLangType() == "en-us"
                        ? stationInfo.StationEngName
                        : stationInfo.StationName;

                    StationBase stationBase = new StationBase(stationName)
                    {
                        Index = Convert.ToInt32(stationInfo.StationIndex)
                    };

                    for (int i = 0; i < 8; i++)
                    {
                        var axisNum = stationInfo.GetType().GetProperty(((AxisNum)i).ToString())?.GetValue(stationInfo, null);
                        if (axisNum != null && !string.IsNullOrEmpty(axisNum.ToString()))
                        {
                            stationBase.SetAxisNo(i, Convert.ToInt32(axisNum));
                        }
                    }

                    _lsStation.Add(stationBase);
                }
            }
        }

        public bool LoadPointFromCfg()
        {
            var pointList = SimpleIoc.Default.GetInstance<PointConfigViewModel>().StationPoints;
            try
            {


                foreach (StationBase stationBase in _lsStation)
                {
                    stationBase.DicPoint.Clear();
                    var pointInfos = from item in pointList
                                     where stationBase.Name == (LocationServices.GetLangType() == "en-us" ? item.EngName : item.Name)
                                     select item.PointInfos;

                    var pointInfoList = pointInfos.First();

                    if (pointInfoList.Any())
                    {
                        foreach (var item in pointInfoList)
                        {
                            PointInfo pointInfo = new PointInfo();
                            int index = Convert.ToInt32(item.Index);
                            pointInfo.StrName = item.Name;

                            for (int i = 0; i < 8; i++)
                            {
                                var pos = item.GetType().GetProperty(((PointNum)i).ToString())?.GetValue(item, null);
                                if (pos != null && !string.IsNullOrEmpty(pos.ToString()))
                                {
                                    pointInfo.Pos[i] = Convert.ToDouble(pos);
                                }
                                else
                                {
                                    pointInfo.Pos[i] = -1.0;
                                }
                            }
                            stationBase.DicPoint.Add(index, pointInfo);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                if (LocationServices.GetLangType() == "en-us")
                {
                    MessageBox.Show(e.ToString(), "Reading of point file failed", MessageBoxButton.OK, MessageBoxImage.Hand);
                }
                else
                {
                    MessageBox.Show(e.ToString(), "点位文件读取失败", MessageBoxButton.OK, MessageBoxImage.Hand);
                }
            }

            return false;
        }

        /// <summary>
        /// 站位当前状态
        /// </summary>
        public StationState CurrentState => _nCurState;

        /// <summary>
        /// 当前是否在自动运行状态
        /// </summary>
        /// <returns></returns>
        public bool IsAutoRunning()
        {
            return _nCurState != StationState.StateManual;
        }

        /// <summary>
        /// 计数减一
        /// </summary>
        public void DecreaseOne()
        {
            lock (_lockCount)
            {
                --_showMessageCount;
                OnShowMessageCountChanged?.Invoke(_showMessageCount);
            }
        }
        /// <summary>
        /// 弹窗计数加一
        /// </summary>
        public void IncreaseOne()
        {
            lock (_lockCount)
            {
                ++_showMessageCount;
                OnShowMessageCountChanged?.Invoke(_showMessageCount);
            }
        }

        /// <summary>
        /// 当前是否处于急停模式
        /// </summary>
        /// <returns></returns>
        public bool IsEmg()
        {
            return _nCurState == StationState.StateEmg;
        }

        /// <summary>
        /// 判断一个线程是在自动状态还是手动状态
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public bool IsAutoThread(Thread thread)
        {
            foreach (StationBase stationBase in _lsStation)
            {
                if (thread == stationBase.AutoThread)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断一个线程是在自动状态还是手动状态
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public bool IsManaualThread(Thread thread)
        {
            foreach (StationBase stationBase in _lsStation)
            {
                if (thread == stationBase.ManualThread)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 急停所有站位
        /// </summary>
        /// <returns></returns>
        public bool EmgStopAllStation()
        {
            if (_nCurState == StationState.StateEmg || _nCurState == StationState.StateManual)
                return false;
            if (LocationServices.GetLangType() == "en-us")
                ShowLog("Alarm and emergency stop!!!", LogLevel.Error);
            else
                ShowLog("报警急停!!!", LogLevel.Error);
            ChangeState(StationState.StateEmg);
            foreach (StationBase stationBase in _lsStation)
            {
                stationBase.SwitchState(StationState.StateEmg);
                stationBase.EmgStop();
            }
            foreach (StationBase stationBase in _lsStation)
                stationBase.StopManualRun();
            return true;
        }

        /// <summary>定义一个站位状态变化事件</summary>
        public event StateChangedHandler StateChangedEvent;

        /// <summary>
        /// 站位切换状态同时触发状态变更事件
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(StationState newState)
        {
            if (_nCurState == newState)
                return;
            StationState nCurState = _nCurState;
            _nCurState = newState;
            StateChangedEvent?.Invoke(nCurState, _nCurState);
            SingletonPattern<RunInforManager>.GetInstance().Info(nCurState + " change state to " + newState);
        }
    }
}
