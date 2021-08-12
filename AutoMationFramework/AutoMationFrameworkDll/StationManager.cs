/*********************************************************************
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
using System.Threading;
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
        /// 用于反射获取轴号属性，减小冗余代码
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
        /// 用于反射获取点位中的位置信息，减少冗余代码
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

        #region 委托事件

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

        /// <summary>
        /// 定义一个站位状态变化事件
        /// </summary>
        public event StateChangedHandler StateChangedEvent;


        /// <summary>
        /// 弹窗计数改变事件
        /// </summary>
        public event ShowMessageCountChanged OnShowMessageCountChanged;

        #endregion

        #region 属性字段

        private static object _lockCount = new object();

        /// <summary>
        /// 站位链表
        /// </summary>
        public List<StationBase> _lsStation = new List<StationBase>();

        /// <summary>
        /// 站位集合
        /// </summary>
        public Dictionary<Control, StationBase> DicControlStation = new Dictionary<Control, StationBase>();

        /// <summary>
        /// 站位与自定义手动界面的集合
        /// </summary>
        public Dictionary<Control, StationBase> DicManualControlStation = new Dictionary<Control, StationBase>();

        private StationState _nCurState = StationState.StateManual;

        private int _showMessageCount = 0;
        private bool _isAutoMode;

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
        /// 站位当前状态
        /// </summary>
        public StationState CurrentState => _nCurState;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public StationManager()
        {
            SingletonPattern<RunInforManager>.GetInstance().WarningEventHandler += this.OnWarning;
        }

        #region 站位相关

        /// <summary>
        /// 根据站位名称获取一个站位类指针
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public StationBase GetStation(string strName)
        {
            foreach (StationBase stationBase in _lsStation)
            {
                if (stationBase.Name == strName)
                    return stationBase;
            }
            return null;
        }

        /// <summary>
        /// 获取手动页面窗口对应的站位类
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public StationBase GetStation(Control control)
        {
            return DicControlStation[control];
        }

        /// <summary>
        /// 根据轴号获取站位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="strAxisName"></param>
        /// <returns></returns>
        public StationBase GetStation(int nAxisNo, out string strAxisName)
        {
            strAxisName = LocationServices.GetLangType() == "en-us" ? $"Axis-{ nAxisNo}" : $"轴{ nAxisNo}";
            foreach (StationBase stationBase in _lsStation)
            {
                int index = Array.IndexOf(stationBase.AxisNumArray, nAxisNo);
                if (index >= 0)
                {
                    strAxisName = stationBase.StrAxisName[index];
                    return stationBase;
                }
            }
            return null;
        }

        /// <summary>
        /// 向站位管理器增加站位,由于站位列表已经从配置中读取, 需要更换列表中的类对象引用
        /// </summary>
        /// <param name="control">通用的手动页面类</param>
        /// <param name="station">站位类</param>
        /// <param name="frm_manual">手动自定义页面类</param>
        public void AddStation(Control control, StationBase station, Control controlManual = null)
        {
            int index = 0;
            foreach (StationBase stationBase in _lsStation)
            {
                if (stationBase.Name == station.Name)
                {
                    station.ControlManual = controlManual;
                    if (control != null)
                    {
                        control.Tag = station.Name;
                        DicControlStation.Add(control, station);
                    }
                    if (controlManual != null)
                        DicManualControlStation.Add(controlManual, station);
                    station.DicPoint = _lsStation[index].DicPoint;
                    station.AxisNumArray = _lsStation[index].AxisNumArray;
                    _lsStation[index] = station;
                    return;
                }
                ++index;
            }
            string str = "站位配置未找到";
            if (LocationServices.GetLangType() == "en-us")
                str = " station configuration not found";
            throw new Exception(station.Name + str);
        }

        /// <summary>
        /// 确定当前是否为全自动模式，无需启动按钮
        /// </summary>
        public override void ThreadMonitor()
        {
            while (BRunThread)
            {
                string strLog1 = "当前全自动模式，自动开始运行";
                string strLog2 = "所有站位进入就绪状态，等待开始";
                if (LocationServices.GetLangType() == "en-us")
                {
                    strLog1 = "Current fully automatic mode, automatic start operation";
                    strLog2 = "All stations enter ready state, waiting to start";
                }
                if (_nCurState == StationState.StateReady)
                {
                    if (BAutoMode)
                    {
                        AutoStartAllStation();
                        ShowLog(strLog1);
                    }
                }
                else if (IsAllReady())
                {
                    ChangeState(StationState.StateReady);
                    ShowLog(strLog2);
                }
                Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
            }
        }

        /// <summary>
        /// 所有站位开始运行，自动模式
        /// </summary>
        /// <returns></returns>
        public bool StartRun()
        {
            if (_nCurState != StationState.StateManual)
                return false;
            _showMessageCount = 0;

            string format = "{0}正在手动动作中，请先等待动作完成";
            string caption = "提示";
            string strLog1 = "模拟运行模式,运动和IO等待默认为true";
            string strLog2 = "所有站位进入自动运行状态";
            if (LocationServices.GetLangType() == "en-us")
            {
                format = "{0} is in manual action. Please wait for the action to complete first";
                caption = "Tips";
                strLog1 = "Simulated operation mode, motion and IO wait default to true";
                strLog2 = "All stations enter automatic operation state";
            }
            foreach (StationBase stationBase in _lsStation)
            {
                if (stationBase.IsManual())
                {
                    MessageBox.Show(string.Format(format, stationBase.Name), caption);
                    return false;
                }
            }
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
                ShowLog(strLog1);
            InitAllStationSecurity();
            SingletonPattern<IoManager>.GetInstance().StartMonitor();
            SingletonPattern<MotionManager>.GetInstance().StartMonitor();
            StartMonitor();
            foreach (StationBase stationBase in _lsStation)
            {
                stationBase.SwitchState(StationState.StateAuto);
                stationBase.StartRun();
            }
            ChangeState(StationState.StateAuto);
            ShowLog(strLog2);
            return true;
        }

        /// <summary>
        /// 所有站位停止运行，转为手动
        /// </summary>
        /// <returns></returns>
        public bool StopRun()
        {
            foreach (StationBase stationBase in _lsStation)
            {
                stationBase.SwitchState(StationState.StateManual);
                stationBase.EmgStop();
            }
            SingletonPattern<IoManager>.GetInstance().StopMonitor();
            SingletonPattern<MotionManager>.GetInstance().StopMonitor();
            GetInstance().StopMonitor();
            foreach (StationBase stationBase in _lsStation)
                stationBase.StopRun();
            ChangeState(StationState.StateManual);
            if (LocationServices.GetLangType() == "en-us")
                ShowLog("All stations exit");
            else
                ShowLog("所有站位退出");
            _showMessageCount = 0;
            return true;
        }

        /// <summary>
        /// 所有站在就绪后开始运行一个循环
        /// </summary>
        /// <returns></returns>
        public bool AutoStartAllStation()
        {
            if (_nCurState != StationState.StateReady)
                return false;
            foreach (StationBase stationBase in _lsStation)
            {
                stationBase.BeginCycle = true;
                stationBase.SwitchState(StationState.StateAuto);
            }
            ChangeState(StationState.StateAuto);

            if (LocationServices.GetLangType() == "en-us")
                ShowLog("All stations start to operate automatically");
            else
                ShowLog("所有站位开始自动运行");
            return true;
        }

        /// <summary>
        /// 初始化所有站位为安全状态
        /// </summary>
        private void InitAllStationSecurity()
        {
            foreach (StationBase stationBase in _lsStation)
            {
                stationBase.IsDeinit = false;
                stationBase.InitSecurityState();
            }
            if (LocationServices.GetLangType() == "en-us")
                ShowLog("Initialize all stations to safe state");
            else
                ShowLog("初始化所有站位为安全状态");
        }

        /// <summary>
        /// 停止所有站位的手动运行
        /// </summary>
        /// <returns></returns>
        public bool StopManualRun()
        {
            foreach (StationBase stationBase in _lsStation)
                stationBase.StopManualRun();
            return true;
        }

        /// <summary>
        /// 暂停所有站位运行
        /// </summary>
        /// <returns></returns>
        public bool PauseAllStation()
        {
            if (_nCurState != StationState.StateManual && _nCurState != StationState.StateEmg && _nCurState != StationState.StatePause)
            {
                ChangeState(StationState.StatePause);
                foreach (StationBase stationBase in _lsStation)
                {
                    stationBase.OnPause();
                    stationBase.SwitchState(StationState.StatePause);
                }
                if (LocationServices.GetLangType() == "en-us")
                    ShowLog("All stations are suspended");
                else
                    ShowLog("所有站位进入暂停状态");
                SingletonPattern<MotionManager>.GetInstance().OnPause();
            }
            return true;
        }

        /// <summary>
        /// 将所有站位由暂停中恢复运行
        /// </summary>
        /// <returns></returns>
        public bool ResumeAllStation()
        {
            if (_nCurState != StationState.StatePause)
                return false;
            SingletonPattern<MotionManager>.GetInstance().OnResume();
            foreach (StationBase stationBase in _lsStation)
            {
                stationBase.OnResume();
                stationBase.SwitchState(StationState.StateAuto);
            }
            ChangeState(StationState.StateAuto);

            if (LocationServices.GetLangType() == "en-us")
                ShowLog("All stations resume operation");
            else
                ShowLog("所有站位恢复运行");
            return true;
        }

        /// <summary>
        /// 将所有站位从急停中复位
        /// </summary>
        /// <returns></returns>
        public bool ResetAllStation()
        {
            if (_nCurState != StationState.StateEmg)
                return false;
            string format = "{0}正在手动动作中，请先等待动作完成";
            string caption = "提示";
            string strLog = "所有站位复位";
            if (LocationServices.GetLangType() == "en-us")
            {
                format = "{0} is in manual action, please wait for the action to complete";
                caption = "Tips";
                strLog = "Reset of all stations";
            }
            foreach (StationBase stationBase in _lsStation)
            {
                if (stationBase.IsManual())
                {
                    MessageBox.Show(string.Format(format, stationBase.Name), caption);
                    return false;
                }
            }
            SingletonPattern<RunInforManager>.GetInstance().ClearAllWarning();
            ShowLog(strLog);
            InitAllStationSecurity();
            foreach (StationBase stationBase in _lsStation)
                stationBase.SwitchState(StationState.StateAuto);
            ChangeState(StationState.StateAuto);
            return true;
        }

        /// <summary>
        /// 获取当前手动自定义窗口对应的站位类
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public StationBase GetManualStation(Control control)
        {
            return DicManualControlStation[control];
        }

        /// <summary>
        /// 当前模式是否允许暂停
        /// </summary>
        /// <returns></returns>
        public bool AllowPause()
        {
            return _nCurState != StationState.StateManual && _nCurState != StationState.StateEmg && _nCurState != StationState.StatePause;
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

        #endregion

        #region 配置

        /// <summary>
        /// 读取系统配置文件里的站位信息
        /// </summary>
        public void ReadStationFromCfg()
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
                            pointInfo.Index = Convert.ToInt32(item.Index);
                            pointInfo.StrName = item.Name;

                            for (int i = 0; i < 8; i++)
                            {
                                var pos = item.GetType().GetProperty(((PointNum)i).ToString())?.GetValue(item, null);
                                if (pos != null && !string.IsNullOrEmpty(pos.ToString()))
                                {
                                    pointInfo.Pos[i] = Math.Round(Convert.ToDouble(pos),4);
                                }
                                else
                                {
                                    pointInfo.Pos[i] = -1.0;
                                }
                            }
                            stationBase.DicPoint.Add(pointInfo.Index, pointInfo);
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

        #endregion

        #region 运行状态

        /// <summary>
        /// 当前是否在暂停状态
        /// </summary>
        /// <returns></returns>
        public bool IsPause()
        {
            return _nCurState == StationState.StatePause;
        }

        /// <summary>
        /// 当前是否在自动运行状态
        /// </summary>
        /// <returns></returns>
        public bool IsAutoRunning()
        {
            return _nCurState != StationState.StateManual;
        }

        /// <summary>
        /// 判断所有工站是否全部就绪状态
        /// </summary>
        /// <returns></returns>
        public bool IsAllReady()
        {
            foreach (StationBase stationBase in _lsStation)
            {
                if (!stationBase.IsReady())
                    return false;
            }
            return true;
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

        #endregion

        #region 其它

        /// <summary>
        /// 响应报警函数
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void OnWarning(object Sender, EventArgs e)
        {
            WarningEventData warningEventData = (WarningEventData)e;
            if (!warningEventData.BAdd || SingletonPattern<RunInforManager>.GetInstance().GetWarning(warningEventData.Index).Level == "WARN" || IsEmg())
                return;
            EmgStopAllStation();
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

        #endregion
    }
}
