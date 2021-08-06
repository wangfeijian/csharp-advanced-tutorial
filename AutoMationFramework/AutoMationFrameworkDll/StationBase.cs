/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-06                               *
*                                                                    *
*           ModifyTime:     2021-08-06                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Station base class                       *
*********************************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AutoMationFrameworkModel;
using AutoMationFrameworkSystemDll;
using CommonTools.Tools;
using Communicate;

namespace AutoMationFrameworkDll
{
    /// <summary>
    /// 站位状态
    /// </summary>
    public enum StationState
    {
        /// <summary>
        /// 自动运行状态
        /// </summary>
        StateAuto,

        /// <summary>
        /// 就绪状态
        /// </summary>
        StateReady,

        /// <summary>
        /// 急停状态
        /// </summary>
        StateEmg,

        /// <summary>
        /// 暂停状态
        /// </summary>
        StatePause,

        /// <summary>停止状态</summary>
        StateManual,
    }

    public class StationBase : LogBase
    {
        /// <summary>
        /// 站位异常类，表示站位流程内出现流程异常
        /// </summary>
        public class StationException : ApplicationException
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="message">异常消息</param>
            public StationException(string message)
              : base(message)
            {
            }
        }

        /// <summary>
        /// 站位异常类，表示站位正常退出或因其它站位退出信号退出
        /// </summary>
        public class SafeException : ApplicationException
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="message"></param>
            public SafeException(string message)
              : base(message)
            {
            }
        }

        /// <summary>
        /// 手动运行的状态
        /// </summary>
        public enum ManualState
        {
            /// <summary>
            /// 自动运行状态
            /// </summary>
            StateManualRun,

            /// <summary>
            /// 就绪状态
            /// </summary>
            StateManualStop,

            /// <summary>
            /// 急停状态
            /// </summary>
            StateManualPause,

            /// <summary>
            /// 停止状态
            /// </summary>
            StateManualEmg,
        }

        /// <summary>
        /// 最后的log
        /// </summary>
        protected struct LastLog
        {
            /// <summary>
            /// LOG时间
            /// </summary>
            public DateTime Time;

            /// <summary>
            /// log
            /// </summary>
            public string StrLog;
        }

        /// <summary>
        /// 定义一个站位状态变化委托函数
        /// </summary>
        public delegate void ManualStateChangedHandler(ManualState nState);

        /// <summary>
        /// 手动运行时的委托函数
        /// </summary>
        public delegate void delegateFunc();

        /// <summary>
        /// 记录最近的log
        /// </summary>
        protected ConcurrentQueue<LastLog> QueLastLog = new ConcurrentQueue<LastLog>();

        /// <summary>
        /// 输入io数组
        /// </summary>
        public string[] IoIn = new string[0];

        /// <summary>
        /// 输出io数组
        /// </summary>
        public string[] IoOut = new string[0];

        /// <summary>
        /// 每个轴运动方向,true-正，false-负
        /// </summary>
        public readonly bool[] PositiveMove = {
          true,
          true,
          true,
          true,
          true,
          true,
          true,
          true};

        /// <summary>
        /// 轴名字
        /// </summary>
        public readonly string[] StrAxisName = {
          "X",
          "Y",
          "Z",
          "U",
          "A",
          "B",
          "C",
          "D"};

        /// <summary>
        /// 轴号数组
        /// </summary>
        public int[] AxisNumArray = new int[8];

        /// <summary>
        /// 点位集合
        /// </summary>
        public Dictionary<int, PointInfo> DicPoint = new Dictionary<int, PointInfo>();

        /// <summary>
        /// 自动线程的ID
        /// </summary>
        public Thread AutoThread = null;

        /// <summary>
        /// 手动线程的ID
        /// </summary>
        public Thread ManualThread = null;

        private ManualState _manaulState = ManualState.StateManualStop;

        /// <summary>
        /// 站位类绑定的手动自定义页面
        /// </summary>
        public Control ControlManual = null;

        /// <summary>
        /// 站位当前是否全自动运行
        /// </summary>
        private bool _beginCycle = false;

        /// <summary>
        /// 站位名
        /// </summary>
        private string _strName = string.Empty;

        /// <summary>
        /// 站位序号
        /// </summary>
        private int _index = 0;

        /// <summary>
        /// 站位是否启用
        /// </summary>
        private bool _stationEnable = true;

        private List<delegateFunc> _manualFuncList = new List<delegateFunc>();

        private const int MaxLastLogCount = 2;

        private StationState _nCurState;

        /// <summary>
        /// 自动线程是否运行中
        /// </summary>
        private bool _bRunThread;

        /// <summary>
        /// 当前站位状态
        /// </summary>
        public StationState CurState => _nCurState;

        /// <summary>
        /// 定义一个站位状态变化事件
        /// </summary>
        public event ManualStateChangedHandler ManualStateChangedEvent;

        /// <summary>
        /// 当前站位是否处于Deinit状态
        /// </summary>
        public bool IsDeinit { get; set; }

        /// <summary>
        /// 工站基类构造函数
        /// </summary>
        /// <param name="strName">站位名称</param>
        public StationBase(string strName)
        {
            _strName = strName;
        }

        /// <summary>
        /// 是否全自动运行属性
        /// </summary>
        public bool BeginCycle
        {
            set
            {
                _beginCycle = value;
            }
            get
            {
                return _beginCycle;
            }
        }

        /// <summary>
        /// 站位名属性
        /// </summary>
        public string Name => _strName;

        /// <summary>
        /// 站位序号属性
        /// </summary>
        public int Index
        {
            set
            {
                _index = value;
            }
            get
            {
                return _index;
            }
        }

        /// <summary>
        /// 得到站位启动状态or设置站位是否启动 属性
        /// </summary>
        public bool StationEnable
        {
            get
            {
                return _stationEnable;
            }
            set
            {
                _stationEnable = value;
            }
        }

        /// <summary>
        /// 获取轴的数量
        /// </summary>
        public int AxisCount => AxisNumArray.Length;

        /// <summary>
        /// 设置轴号
        /// </summary>
        /// <param name="index">轴序号</param>
        /// <param name="nAxisNo">轴号</param>
        public void SetAxisNo(int index, int nAxisNo)
        {
            AxisNumArray[index] = nAxisNo;
        }

        /// <summary>
        /// 得到轴号
        /// </summary>
        /// <param name="index">轴序号</param>
        /// <returns></returns>
        public int GetAxisNo(int index)
        {
            return AxisNumArray[index];
        }

        /// <summary>
        /// 当前站位是否处于急停状态
        /// </summary>
        /// <returns></returns>
        public bool IsEmg()
        {
            return _nCurState == StationState.StateEmg;
        }

        /// <summary>
        /// 当前站位是否处于停止状态
        /// </summary>
        /// <returns></returns>
        public bool IsManual()
        {
            return ManualThread != null;
        }

        /// <summary>
        /// 得到X轴号
        /// </summary>
        public int AxisX => AxisNumArray[0];

        /// <summary>
        /// 得到Y轴号
        /// </summary>
        public int AxisY => AxisNumArray[1];

        /// <summary>
        /// 得到Z轴号
        /// </summary>
        public int AxisZ => AxisNumArray[2];

        /// <summary>
        /// 得到U轴号
        /// </summary>
        public int AxisU => AxisNumArray[3];

        /// <summary>
        /// 得到A轴号
        /// </summary>
        public int AxisA => AxisNumArray[4];

        /// <summary>
        /// 得到B轴号
        /// </summary>
        public int AxisB => AxisNumArray[5];

        /// <summary>
        /// 得到C轴号
        /// </summary>
        public int AxisC => AxisNumArray[6];

        /// <summary>
        /// 得到D轴号
        /// </summary>
        public int AxisD => AxisNumArray[7];

        /// <summary>
        /// 开始自动运行
        /// </summary>
        public void StartRun()
        {
            lock (QueLastLog)
            {
                while (!QueLastLog.IsEmpty)
                {
                    LastLog result;
                    QueLastLog.TryDequeue(out result);
                }
            }

            if (AutoThread == null)
                AutoThread = new Thread(ThreadProc);

            if ((uint)AutoThread.ThreadState > 0U)
            {
                _bRunThread = true;
                AutoThread.Start();
            }

            if (!SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
                return;

            if (LocationServices.GetLangType() == "en-us")
                ShowLog("Current station is in simulated operation mode");
            else
                ShowLog("当前站位处于模拟运行模式");
        }

        /// <summary>
        /// 停止运行
        /// </summary>
        public void StopRun()
        {
            lock (QueLastLog)
            {
                while (!QueLastLog.IsEmpty)
                {
                    LastLog result;
                    QueLastLog.TryDequeue(out result);
                }
            }
            if (AutoThread == null)
                return;
            _bRunThread = false;

            _nCurState = StationState.StateManual;
            if (!AutoThread.Join(5000))
                AutoThread.Abort();

            AutoThread = null;
        }

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="strXml"></param>
        private void StartScriptFun(string strXml)
        {
        }

        /// <summary>
        /// 站位运行状态切换
        /// </summary>
        /// <param name="nState">将要切换到的站位状态</param>
        public void SwitchState(StationState nState)
        {
            if (_nCurState == nState)
                return;

            _nCurState = nState;
            if (LocationServices.GetLangType() == "en-us")
                ShowLog("State switch to " + nState);
            else
                ShowLog("状态切换至" + nState);
        }

        /// <summary>
        /// 超时提示, 工程师模式时可以忽略超时错误继续运行，其它模式只支持继续等待或停止站位运行
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="bOnlyNotify">只显示确认按钮</param>
        /// <param name="strBindIo">绑定IO数组，对应Yes/No/Cancel</param>
        /// <param name="nKeepTimeS">IO保持时间，下降沿</param>
        /// <returns></returns>
        public bool? ShowMessage(string strText, bool bOnlyNotify = true, string[] strBindIo = null, int nKeepTimeS = 3)
        {
            MessageWindow messageWindow = new MessageWindow(this);
            messageWindow.BindIo = strBindIo;
            messageWindow.IoKeepTimeS = nKeepTimeS;
            messageWindow.NotifyMode = bOnlyNotify;

            string title = "消息提示";
            if (LocationServices.GetLangType() == "en-us")
                title = "Message Tips";
            SingletonPattern<StationManager>.GetInstance().IncreaseOne();
            bool? dialogResult;
            if (bOnlyNotify)
            {
                string strText1 = "确认";
                if (LocationServices.GetLangType() == "en-us")
                    strText1 = "OK";
                messageWindow.SetYesText(strText1);
                dialogResult = messageWindow.MessageShow(strText, title, MessageBoxButton.OK);
            }
            else
                dialogResult = messageWindow.MessageShow(strText, title, MessageBoxButton.YesNoCancel);
            SingletonPattern<StationManager>.GetInstance().DecreaseOne();
            return dialogResult;
        }

        /// <summary>
        /// 仅用作消息框，可以修改按钮默认文本，支持绑定IO
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="strButtonText"></param>
        /// <param name="strBindIo"></param>
        /// <param name="nKeepTimeS"></param>
        /// <returns></returns>
        public bool? ShowMessage(string strText, string[] strButtonText, string[] strBindIo = null, int nKeepTimeS = 3)
        {
            MessageWindow messageWindow = new MessageWindow(this);
            messageWindow.BindIo = strBindIo;
            messageWindow.IoKeepTimeS = nKeepTimeS;
            messageWindow.NotifyMode = true;
            string title = "消息提示";
            if (LocationServices.GetLangType() == "en-us")
                title = "Message Tips";
            if (strButtonText == null)
            {
                string strText1 = "确认";
                if (LocationServices.GetLangType() == "en-us")
                    strText1 = "OK";
                messageWindow.SetYesText(strText1);
                return messageWindow.MessageShow(strText, title, MessageBoxButton.OK);
            }
            if (strButtonText.Length > 2)
            {
                messageWindow.SetYesText(strButtonText[0]);
                messageWindow.SetNoText(strButtonText[1]);
                messageWindow.SetCancelText(strButtonText[2]);
                return messageWindow.MessageShow(strText, title, MessageBoxButton.YesNoCancel);
            }
            if (strButtonText.Length == 2)
            {
                messageWindow.SetYesText(strButtonText[0]);
                messageWindow.SetNoText(strButtonText[1]);
                return messageWindow.MessageShow(strText, title, MessageBoxButton.YesNo);
            }
            if (strButtonText.Length == 1)
            {
                messageWindow.SetYesText(strButtonText[0]);
                return messageWindow.MessageShow(strText, title, MessageBoxButton.OK);
            }
            string strText2 = "确认";
            if (LocationServices.GetLangType() == "en-us")
                strText2 = "OK";
            messageWindow.SetYesText(strText2);
            return messageWindow.MessageShow(strText, title, MessageBoxButton.OK);
        }

        /// <summary>
        /// 显示记录，自带站位名称
        /// </summary>
        /// <param name="strLog"></param>
        /// <param name="level"></param>
        public new void ShowLog(string strLog, LogLevel level = LogLevel.Info)
        {
            string strLog1 = Name + " - " + strLog;
            DateTime now = DateTime.Now;
            if (!IsAllowLog(new LastLog
            {
                Time = now,
                StrLog = strLog1
            }))
                return;
            base.ShowLog(strLog1, level);
        }

        /// <summary>
        /// 是否允许显示日志，如果不做判断在子类里重写此方法return true
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        protected virtual bool IsAllowLog(LastLog log)
        {
            bool flag = true;
            lock (QueLastLog)
            {
                foreach (LastLog lastLog in QueLastLog)
                {
                    if ((log.Time - lastLog.Time).TotalSeconds < 3.0 && log.StrLog == lastLog.StrLog)
                    {
                        flag = false;
                        break;
                    }
                }
                if (QueLastLog.Count >= 2)
                {
                    LastLog result;
                    QueLastLog.TryDequeue(out result);
                }
                QueLastLog.Enqueue(log);
            }
            return flag;
        }

        /// <summary>
        /// 获取运动超时时间设置值
        /// </summary>
        /// <returns>超时时间</returns>
        public static int GetMotionTimeOut()
        {
            int paramInt = SingletonPattern<SystemManager>.GetInstance().GetParamInt("MotionTimeOut");
            if (paramInt == 0)
                return 600;
            return paramInt;
        }

        /// <summary>
        /// 显示超时提示对话框
        /// </summary>
        /// <param name="strInfo">提示消息</param>
        /// <param name="strErrorCode">消息的错误分类码</param>
        /// <returns>0：重置超时 1：忽略超时 ， 报异常退出时无返回值</returns>
        public int ShowTimeOutDlg(string strInfo, string strErrorCode)
        {
            SingletonPattern<IoManager>.GetInstance().AlarmLight(LightState.黄灯闪 | LightState.蜂鸣闪);
            string str = ",是否继续等待?";
            string strLog1 = "等待超时确认异常，退出流程";
            string strLog2 = "等待超时，重置超时继续等待";
            string strLog3 = "等待超时，忽略此超时继续运行";
            if (LocationServices.GetLangType() == "en-us")
            {
                str = ",continue to wait?";
                strLog1 = "Wait for timeout to confirm the exception and exit the process";
                strLog2 = "Wait timeout, reset timeout continue waiting";
                strLog3 = "Wait for timeout, ignore this timeout to continue";
            }
            bool? dialogResult = ShowMessage(strInfo + str, false, new[] { "复位" });

            SingletonPattern<IoManager>.GetInstance().AlarmLight(LightState.绿灯开);
            if (dialogResult == false)
            {
                ShowLog(strLog1, LogLevel.Error);
                throw new StationException(strErrorCode + strInfo);
            }
            if (dialogResult == true)
            {
                ShowLog(strLog2, LogLevel.Warn);
                return 0;
            }
            ShowLog(strLog3, LogLevel.Warn);
            return 1;
        }

        /// <summary>
        /// 显示超时提示对话框
        /// </summary>
        /// <param name="strInfo">消息</param>
        /// <param name="errType">错误类型</param>
        /// <param name="strObject">错误对象</param>
        /// <returns>0：重置超时 1：忽略超时 ， 报异常退出时无返回值</returns>
        public int ShowTimeOutDlg(string strInfo, ErrorType errType, string strObject)
        {
            SingletonPattern<IoManager>.GetInstance().AlarmLight(LightState.黄灯闪 | LightState.蜂鸣闪);
            string str = ",是否继续等待?";
            string strLog1 = "等待超时确认异常，退出流程";
            string strLog2 = "等待超时，重置超时继续等待";
            string strLog3 = "等待超时，忽略此超时继续运行";
            if (LocationServices.GetLangType() == "en-us")
            {
                str = ",continue to wait?";
                strLog1 = "Wait for timeout to confirm the exception and exit the process";
                strLog2 = "Wait timeout, reset timeout continue waiting";
                strLog3 = "Wait for timeout, ignore this timeout to continue";
            }
            SingletonPattern<RunInforManager>.GetInstance().Warning(strInfo);
            bool? dialogResult = ShowMessage(strInfo + str, false, new[] { "复位" });
            SingletonPattern<IoManager>.GetInstance().AlarmLight(LightState.绿灯开);
            if (dialogResult == false)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(((int)errType).ToString());
                stringBuilder.Append(",");
                stringBuilder.Append(errType.ToString());
                stringBuilder.Append(",");
                stringBuilder.Append(strObject);
                stringBuilder.Append(",");
                ShowLog(strLog1, LogLevel.Error);
                throw new StationException(stringBuilder + strInfo);
            }
            if (dialogResult == true)
            {
                ShowLog(strLog2, LogLevel.Warn);
                return 0;
            }
            ShowLog(strLog3, LogLevel.Warn);
            return 1;
        }

        /// <summary>
        /// 等待回原点
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">流程暂停时是否响应暂停 true:等待暂停恢复  false:不响应暂停继续扫描 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int WaitHome(int nAxisNo, int nTimeOutS = 0, bool bShowDialog = true, bool bPause = true)
        {
            int index = Array.IndexOf(AxisNumArray, nAxisNo);
            string str1 = "轴";
            string format1 = "等待{0}回原点";
            string format2 = "{0}等待{1}回原点{2}秒超时";
            string[] strArray = {
                "急停",
                "报警",
                "未励磁",
                "正限位",
                "负限位",
                "停止位置超限"
            };

            string format3 = "{0}出现{1}{2}异常";
            string format4 = "{0}出现{1}异常, 代码{2}";
            string format5 = "等待{0}回原点,超过{1}秒，继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                str1 = "Axis ";
                format1 = "Waiting for {0} to go home";
                format2 = "{0} waiting for {1} to go home for {2} seconds timeout";
                strArray = new[]
                {
                  "EStop",
                  "Alarm",
                  "Servo-Off",
                  "PEL",
                  "MEL",
                  "Stop Out-Of-Limit"
                };
                format3 = "{0} occur {1} {2} exception";
                format4 = "{0} occur {1} exception, error code:{2}";
                format5 = "Wait for {0} to go home, more than {1} seconds,continue to wait";
            }
            string str2 = str1 + nAxisNo;
            if (index < StrAxisName.Length && index >= 0)
                str2 = StrAxisName[index];
            ShowLog(string.Format(format1, str2));
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return 0;
            }
            int num1 = 0;
            if (nTimeOutS == 0)
                nTimeOutS = GetMotionTimeOut();
            bool flag = false;
            int num2;
            while (true)
            {
                num2 = -1;
                if (_nCurState == StationState.StateAuto || !SingletonPattern<MotionManager>.GetInstance().IsEnbaleAxisPause(nAxisNo))
                    num2 = SingletonPattern<MotionManager>.GetInstance().IsHomeNormalStop(nAxisNo);
                switch (num2)
                {
                    case -1:
                        Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                        num1 += SingletonPattern<SystemManager>.GetInstance().ScanTime;
                        if (nTimeOutS != -1 && num1 > nTimeOutS * 1000)
                        {
                            string str3 = string.Format(format2, Name, str2, nTimeOutS);
                            ShowLog(str3, LogLevel.Error);
                            if (bShowDialog)
                            {
                                CheckContinue(bPause);
                                if (ShowTimeOutDlg(str3, ErrorType.ErrMotionHomeTimeOut, nAxisNo.ToString()) == 0)
                                    num1 = 0;
                                else
                                {
                                    CheckContinue(bPause);
                                    return 1;
                                }
                            }
                            else
                            {
                                CheckContinue(bPause);
                                return 1;
                            }
                        }
                        else
                        {
                            int motionTimeOut = GetMotionTimeOut();
                            if (nTimeOutS == -1 && num1 > motionTimeOut * 1000 && !flag)
                            {
                                flag = true;
                                ShowLog(string.Format(format5, str2, motionTimeOut), LogLevel.Warn);
                            }
                            CheckContinue(bPause);
                        }
                        continue;
                    case 0:
                        CheckContinue(bPause);
                        ShowLog(str2 + " Home finished");
                        return 0;
                    default:
                        string str4 = num2 >= 7 ? string.Format(format4, Name, str2, num2 - 10) : string.Format(format3, Name, str2, strArray[num2 - 1]);
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append(1005);
                        stringBuilder.Append(",");
                        stringBuilder.Append(ErrorType.ErrMotionHome.ToString());
                        stringBuilder.Append(",");
                        stringBuilder.Append(nAxisNo);
                        stringBuilder.Append(",");
                        stringBuilder.Append(str4);
                        throw new StationException(stringBuilder.ToString());
                }
            }
        }

        /// <summary>
        /// 等待运动完成
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">流程暂停时是否响应暂停 true:等待暂停恢复  false:不响应暂停继续扫描 </param>
        /// <param name="bShowLog">是否显示日志</param>
        /// <returns>0:未超时  1:超时  </returns>
        public int WaitMotion(int nAxisNo, int nTimeOutS = 0, bool bShowDialog = true, bool bPause = true, bool bShowLog = true)
        {
            int index = Array.IndexOf(AxisNumArray, nAxisNo);
            string str1 = "轴";
            string format1 = "等待{0}轴运动到位";
            string format2 = "{0}等待{1}到位{2}秒超时";
            string[] strArray = {
                "急停",
                "报警",
                "未励磁",
                "正限位",
                "负限位",
                "停止位置超限"
            };
            string format3 = "{0}出现{1}{2}异常";
            string format4 = "{0}出现{1}异常, 代码{2}";
            string format5 = "等待{0}轴运动未到位,超过{1}秒,继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                str1 = "Axis ";
                format1 = "Waiting for {0} motion done";
                format2 = "{0} Waiting for {1} motion done for {2} seconds timeout";
                strArray = new[]
                {
                  "EStop",
                  "Alarm",
                  "Servo-Off",
                  "PEL",
                  "MEL",
                  "Stop Out-Of-Limit"
                };
                format3 = "{0} occur {1} {2} exception";
                format4 = "{0} occur {1} exception, error code:{2}";
                format5 = "Wait for {0} axis movement not in place, more than {1} seconds, continue to wait";
            }
            string str2 = str1 + nAxisNo;
            if (index < StrAxisName.Length && index >= 0)
                str2 = StrAxisName[index];
            if (bShowLog)
                ShowLog(string.Format(format1, str2));
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return 0;
            }
            AxisConfig cfg;
            if (!SingletonPattern<MotionManager>.GetInstance().GetAxisCfg(nAxisNo, out cfg))
                cfg.InPosError = 1000;
            int num1 = 0;
            if (nTimeOutS == 0)
                nTimeOutS = GetMotionTimeOut();
            bool flag = false;
            int num2;
            while (true)
            {
                num2 = -1;
                if (_nCurState == StationState.StateAuto || !SingletonPattern<MotionManager>.GetInstance().IsEnbaleAxisPause(nAxisNo))
                    num2 = SingletonPattern<MotionManager>.GetInstance().IsAxisInPos(nAxisNo, cfg.InPosError);
                switch (num2)
                {
                    case -1:
                        Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                        num1 += SingletonPattern<SystemManager>.GetInstance().ScanTime;
                        if (nTimeOutS != -1 && num1 > nTimeOutS * 1000)
                        {
                            string str3 = string.Format(format2, Name, str2, nTimeOutS);
                            ShowLog(str3, LogLevel.Error);
                            if (bShowDialog)
                            {
                                CheckContinue(bPause);
                                if (ShowTimeOutDlg(str3, ErrorType.ErrMotionTimeOut, nAxisNo.ToString()) == 0)
                                    num1 = 0;
                                else
                                {
                                    CheckContinue(bPause);
                                    return 1;
                                }
                            }
                            else
                            {
                                CheckContinue(bPause);
                                return 1;
                            }
                        }
                        else
                        {
                            int motionTimeOut = GetMotionTimeOut();
                            if (nTimeOutS == -1 && num1 > motionTimeOut * 1000 && !flag)
                            {
                                flag = true;
                                ShowLog(string.Format(format5, str2, motionTimeOut), LogLevel.Warn);
                            }
                            CheckContinue(bPause);
                        }
                        continue;
                    case 0:
                        CheckContinue(bPause);
                        if (bShowLog)
                            ShowLog(str2 + " Motion done");
                        return 0;
                    default:
                        string str4 = num2 >= 7 ? string.Format(format4, Name, str2, (num2 - 10)) : string.Format(format3, Name, str2, strArray[num2 - 1]);
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append(1015);
                        stringBuilder.Append(",");
                        stringBuilder.Append(ErrorType.ErrMotionTimeOut.ToString());
                        stringBuilder.Append(",");
                        stringBuilder.Append(nAxisNo);
                        stringBuilder.Append(",");
                        stringBuilder.Append(str4);
                        throw new StationException(stringBuilder.ToString());
                }
            }
        }

        /// <summary>
        /// 获取IO超时时间设置
        /// </summary>
        /// <returns>超时时间</returns>
        public static int GetIoTimeOut()
        {

            int paramInt = SingletonPattern<SystemManager>.GetInstance().GetParamInt("IoTimeOut");
            if (paramInt == 0)
                return 100;
            return paramInt;
        }

        /// <summary>
        /// 等待IO输入点有效
        /// </summary>
        /// <param name="nCardNo">卡号</param>
        /// <param name="nBitIndex">点位索引 从1开始</param>
        /// <param name="bValid">高电平有效或低电平有效</param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">流程暂停时是否响应暂停 true:等待暂停恢复  false:不响应暂停继续扫描 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int WaitIo(int nCardNo, int nBitIndex, bool bValid, int nTimeOutS = 0, bool bShowDialog = true, bool bPause = true)
        {
            string str1 = SingletonPattern<IoManager>.GetInstance().GetIoInName(nCardNo, nBitIndex);
            string format1 = "等待IO{0}.{1}{2}，有效电平{3}";
            string format2 = "{0}等待IO点IO{1}.{2}{3}有效电平为{4}达到{5}秒超时";
            string format3 = "等待IO{0}.{1}{2}={3}超过{4}秒，继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                format1 = "Waiting for IO {0}.{1} {2}，valid level = {3}";
                format2 = "{0} Waiting for IO {1}.{2} {3} valid level = {4} for {5} seconds timeout";
                format3 = "Wait for IO {0}. {1} {2} = {3} for more than {4} seconds, continue to wait";
                str1 = SingletonPattern<IoManager>.GetInstance().GetIoInTranslate(str1);
            }
            ShowLog(string.Format(format1, nCardNo, nBitIndex, str1, bValid));
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return 0;
            }
            int num = 0;
            if (nTimeOutS == 0)
                nTimeOutS = GetIoTimeOut();
            bool flag1 = false;
            while (true)
            {
                bool flag2;
                do
                {
                    flag2 = SingletonPattern<IoManager>.GetInstance().ReadIoInBit(nCardNo, nBitIndex);
                    if (flag2 == bValid)
                    {
                        if (SingletonPattern<SystemManager>.GetInstance().GetParamBool("AntiShakeEnable"))
                        {
                            Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().GetParamInt("AntiShakeTimeMs"));
                            flag2 = SingletonPattern<IoManager>.GetInstance().ReadIoInBit(nCardNo, nBitIndex);
                        }
                        else
                            break;
                    }
                    else
                    {
                        Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                        num += SingletonPattern<SystemManager>.GetInstance().ScanTime;
                        if (nTimeOutS != -1 && num > nTimeOutS * 1000)
                        {
                            string str2 = string.Format(format2, Name, nCardNo, nBitIndex, str1, bValid.ToString(), nTimeOutS.ToString());
                            ShowLog(str2, LogLevel.Error);
                            if (bShowDialog)
                            {
                                CheckContinue(bPause);
                                if (ShowTimeOutDlg(str2, ErrorType.ErrIoTimeOut, str1) == 0)
                                    num = 0;
                                else
                                {
                                    CheckContinue(bPause);
                                    ShowLog($"IO{nCardNo}.{nBitIndex}{str1} = {flag2}");
                                    return 1;
                                }
                            }
                            else
                            {
                                CheckContinue(bPause);
                                ShowLog($"IO{nCardNo}.{nBitIndex}{str1} = {flag2}");
                                return 1;
                            }
                        }
                        else
                        {
                            int ioTimeOut = GetIoTimeOut();
                            if (nTimeOutS == -1 && num > ioTimeOut * 1000 && !flag1)
                            {
                                flag1 = true;
                                ShowLog(string.Format(format3, nCardNo, nBitIndex, str1, bValid, ioTimeOut), LogLevel.Warn);
                            }
                            CheckContinue(false);
                        }
                    }
                }
                while (flag2 != bValid);
                CheckContinue(bPause);
                ShowLog($"IO{nCardNo}.{nBitIndex}{str1} = {flag2} done");
                return 0;
            }
        }

        /// <summary>
        /// 等待任意IO有效
        /// </summary>
        /// <param name="strIoNames">IO名称列表</param>
        /// <param name="bValid">有效值</param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框, false:不显示</param>
        /// <param name="bPause">保留参数，暂时没用</param>
        /// <returns>有效IO名称，string.Empty表示超时</returns>
        public virtual string WaitAnyIo(string[] strIoNames, bool bValid, int nTimeOutS = -1, bool bShowDialog = true, bool bPause = true)
        {
            string str1 = "等待";
            string str2 = "或者";
            string format = "输入IO{0}={1}";
            string str3 = "出现超时";
            string str4 = ",超过{0}秒，继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                str1 = "Waiting ";
                str2 = " or ";
                format = "IO-IN {0}={1}";
                str3 = " timeout";
                str4 = ", more than {0} seconds, continue to wait";
            }
            string strLog = str1;
            for (int index = 0; index < strIoNames.Length; ++index)
            {
                string ioInTranslate = strIoNames[index];
                if (LocationServices.GetLangType() == "en-us")
                    ioInTranslate = SingletonPattern<IoManager>.GetInstance().GetIoInTranslate(ioInTranslate);
                if (index < strIoNames.Length - 1)
                    strLog = strLog + ioInTranslate + "=" + bValid + str2;
                else
                    strLog = strLog + ioInTranslate + "=" + bValid;
            }
            ShowLog(strLog);
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
            {
                Thread.Sleep(500);
                return strIoNames[0];
            }
            string str5 = "";
            int num = 0;
            if (nTimeOutS == 0)
                nTimeOutS = GetIoTimeOut();
            bool flag = false;
            while (true)
            {
                foreach (string strIoName in strIoNames)
                {
                    if (SingletonPattern<IoManager>.GetInstance().ReadIoInBit(strIoName) == bValid)
                    {
                        if (SingletonPattern<SystemManager>.GetInstance().GetParamBool("AntiShakeEnable"))
                        {
                            Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().GetParamInt("AntiShakeTimeMs"));
                            if (SingletonPattern<IoManager>.GetInstance().ReadIoInBit(strIoName) != bValid)
                                continue;
                        }
                        str5 = strIoName;
                        ShowLog(string.Format(format, strIoName, bValid.ToString()));
                        break;
                    }
                }
                if (str5 == "")
                {
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                    num += SingletonPattern<SystemManager>.GetInstance().ScanTime;
                    if (nTimeOutS != -1 && num > nTimeOutS * 1000)
                    {
                        string str6 = strLog + str3;
                        ShowLog(str6, LogLevel.Error);
                        if (bShowDialog)
                        {
                            CheckContinue(bPause);
                            if (ShowTimeOutDlg(str6, ErrorType.ErrIoTimeOut, strIoNames[0]) == 0)
                                num = 0;
                            else
                                break;
                        }
                        else
                        {
                            CheckContinue(bPause);
                            return string.Empty;
                        }
                    }
                    else
                    {
                        int ioTimeOut = GetIoTimeOut();
                        if (nTimeOutS == -1 && num > ioTimeOut * 1000 && !flag)
                        {
                            flag = true;
                            ShowLog(string.Format(strLog + str4, ioTimeOut), LogLevel.Warn);
                        }
                        CheckContinue(false);
                    }
                }
                else
                {
                    CheckContinue();
                    ShowLog($"IO {str5} = {bValid} done");
                    return str5;
                }
            }
            CheckContinue(bPause);
            return string.Empty;
        }

        /// <summary>
        /// 等待IO输入点有效
        /// </summary>
        /// <param name="strIoName">点位名称</param>
        /// <param name="bValid">高电平有效或低电平有效</param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">流程暂停时是否响应暂停 true:等待暂停恢复  false:不响应暂停继续扫描 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int WaitIo(string strIoName, bool bValid, int nTimeOutS = 0, bool bShowDialog = true, bool bPause = true)
        {
            long num1;
            if (SingletonPattern<IoManager>.GetInstance().DicIn.TryGetValue(strIoName, out num1))
                return WaitIo((int)(num1 >> 8), (int)(num1 & byte.MaxValue), bValid, nTimeOutS, bShowDialog, bPause);
            if (LocationServices.GetLangType() == "en-us")
            {
                MessageBox.Show($"The IO-IN name {strIoName} does not exist, please confirm!", "IO-IN Error");
            }
            else
            {
                MessageBox.Show($"不存在的IO输入点名称 {strIoName}， 请确认配置是否正确", "IO等待输入点出错");
            }
            return 1;
        }

        /// <summary>
        /// 获取寄存器等待超时时间
        /// </summary>
        /// <returns>超时时间</returns>
        public static int GetRegTimeOut()
        {
            int paramInt = SingletonPattern<SystemManager>.GetInstance().GetParamInt("RegTimeOut");
            if (paramInt == 0)
                return 600;
            return paramInt;
        }

        /// <summary>
        /// 等待系统寄存器是否有效
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="bValid"></param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">流程暂停时是否响应暂停 true:等待暂停恢复  false:不响应暂停继续扫描 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int WaitRegBit(int nIndex, bool bValid, int nTimeOutS = -1, bool bShowDialog = true, bool bPause = true)
        {
            string format1 = "等待位寄存器{0}，有效值{1}";
            string format2 = "{0}等待位寄存器{1}为{2}超时";
            string format3 = "等待位寄存器{0} = {1},超过{2}秒,继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                format1 = "Waiting for SysRegBit {0}, valid value {1}";
                format2 = "{0} Waiting for SysRegBit {1} = {2} timeout";
                format3 = "Wait for SysRegBit {0} = {1}, more than {2} seconds, continue to wait";
            }
            ShowLog(string.Format(format1, nIndex, bValid.ToString()));
            int num = 0;
            if (nTimeOutS == 0)
                nTimeOutS = GetRegTimeOut();
            bool flag = false;
            while (true)
            {
                if (SingletonPattern<SystemManager>.GetInstance().GetRegBit(nIndex) != bValid)
                {
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                    num += SingletonPattern<SystemManager>.GetInstance().ScanTime;
                    if (nTimeOutS != -1 && num > nTimeOutS * 1000)
                    {
                        string str = string.Format(format2, Name, nIndex, bValid.ToString());
                        ShowLog(str, LogLevel.Error);
                        if (bShowDialog)
                        {
                            CheckContinue(bPause);
                            if (ShowTimeOutDlg(str, ErrorType.ErrRegBitTimeOut, nIndex.ToString()) == 0)
                                num = 0;
                            else
                                break;
                        }
                        else
                        {
                            CheckContinue(bPause);
                            return 1;
                        }
                    }
                    else
                    {
                        int regTimeOut = GetRegTimeOut();
                        if (nTimeOutS == -1 && num > regTimeOut * 1000 && !flag)
                        {
                            flag = true;
                            ShowLog(string.Format(format3, nIndex, bValid.ToString(), regTimeOut), LogLevel.Warn);
                        }
                        CheckContinue(false);
                    }
                }
                else
                {
                    CheckContinue(bPause);
                    ShowLog($"SysRegBit {nIndex} = {bValid.ToString()} done");
                    return 0;
                }
            }
            CheckContinue(bPause);
            return 1;
        }

        /// <summary>
        /// 等待任意系统寄存器值是否有效
        /// </summary>
        /// <param name="nIndexs">系统寄存器数值</param>
        /// <param name="bValid">有效值</param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框, false:不显示</param>
        /// <param name="bPause">保留参数，暂时没用</param>
        /// <returns>有效寄存器，-1表示超时</returns>
        public virtual int WaitAnyRegBit(int[] nIndexs, bool bValid, int nTimeOutS = -1, bool bShowDialog = true, bool bPause = true)
        {
            string str1 = "等待";
            string str2 = "系统位寄存器";
            string str3 = "或者";
            string format = "系统位寄存器{0}={1}";
            string str4 = "超时";
            string str5 = ",超过{0}秒，继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                str1 = "Waiting for ";
                str2 = " SysBitReg ";
                str3 = " or ";
                format = "SysBitReg {0}={1}";
                str4 = " timeout";
                str5 = ", more than {0} seconds, continue to wait";
            }
            string strLog = str1;
            for (int index = 0; index < nIndexs.Length; ++index)
            {
                if (index < nIndexs.Length - 1)
                    strLog = strLog + str2 + nIndexs[index] + "=" + bValid + str3;
                else
                    strLog = strLog + str2 + nIndexs[index] + "=" + bValid;
            }
            ShowLog(strLog);
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
            {
                Thread.Sleep(500);
                return nIndexs[0];
            }
            int num1 = -1;
            int num2 = 0;
            if (nTimeOutS == 0)
                nTimeOutS = SingletonPattern<SystemManager>.GetInstance().GetParamInt("RegTimeOut");
            bool flag = false;
            while (true)
            {
                foreach (int nIndex in nIndexs)
                {
                    if (SingletonPattern<SystemManager>.GetInstance().GetRegBit(nIndex) == bValid)
                    {
                        num1 = nIndex;
                        ShowLog(string.Format(format, nIndex, bValid.ToString()));
                        break;
                    }
                }
                if (num1 == -1)
                {
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                    num2 += SingletonPattern<SystemManager>.GetInstance().ScanTime;
                    if (nTimeOutS != -1 && num2 > nTimeOutS * 1000)
                    {
                        string str6 = strLog + str4;
                        ShowLog(str6, LogLevel.Error);
                        if (bShowDialog)
                        {
                            CheckContinue(bPause);
                            if (ShowTimeOutDlg(str6, ErrorType.ErrRegBitTimeOut, nIndexs[0].ToString()) == 0)
                                num2 = 0;
                            else
                                break;
                        }
                        else
                        {
                            CheckContinue(bPause);
                            return -1;
                        }
                    }
                    else
                    {
                        int regTimeOut = GetRegTimeOut();
                        if (nTimeOutS == -1 && num2 > regTimeOut * 1000 && !flag)
                        {
                            flag = true;
                            ShowLog(string.Format(strLog + str5, regTimeOut), LogLevel.Warn);
                        }
                        CheckContinue(false);
                    }
                }
                else
                {
                    CheckContinue(bPause);
                    ShowLog($"SysBitReg {num1} = { bValid.ToString()} done");
                    return num1;
                }
            }
            CheckContinue(bPause);
            return -1;
        }

        /// <summary>
        /// 等待系统整型寄存器
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="nTarget"></param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">流程暂停时是否响应暂停 true:等待暂停恢复  false:不响应暂停继续扫描 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int WaitRegInt(int nIndex, int nTarget, int nTimeOutS = -1, bool bShowDialog = true, bool bPause = true)
        {
            string format1 = "等待整型寄存器{0}，有效值{1}";
            string format2 = "{0}等待整型寄存器{1}为{2}超时";
            string format3 = "等待整型寄存器{0} = {1},超过{2}秒，继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                format1 = "Waiting for SysIntReg {0}，valid value = {1}";
                format2 = "{0} Waiting for SysIntReg {1} = {2} timeout";
                format3 = "Wait for SysIntReg {0} = {1},more than {2} seconds,continue to wait";
            }
            ShowLog(string.Format(format1, nIndex, nTarget));
            int num = 0;
            if (nTimeOutS == 0)
                nTimeOutS = GetRegTimeOut();
            bool flag = false;
            while (true)
            {
                if (SingletonPattern<SystemManager>.GetInstance().GetRegInt(nIndex) != nTarget)
                {
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                    num += SingletonPattern<SystemManager>.GetInstance().ScanTime;
                    if (nTimeOutS != -1 && num > nTimeOutS * 1000)
                    {
                        string str = string.Format(format2, Name, nIndex, nTarget);
                        ShowLog(str, LogLevel.Error);
                        if (bShowDialog)
                        {
                            CheckContinue(bPause);
                            if (ShowTimeOutDlg(str, ErrorType.ErrRegIntTimeOut, nIndex.ToString()) == 0)
                                num = 0;
                            else
                                break;
                        }
                        else
                        {
                            CheckContinue(bPause);
                            return 1;
                        }
                    }
                    else
                    {
                        int regTimeOut = GetRegTimeOut();
                        if (nTimeOutS == -1 && num > regTimeOut * 1000 && !flag)
                        {
                            flag = true;
                            ShowLog(string.Format(format3, nIndex, nTarget, regTimeOut), LogLevel.Warn);
                        }
                        CheckContinue(false);
                    }
                }
                else
                {
                    CheckContinue(bPause);
                    ShowLog($"SysIntReg {nIndex} = {nTarget} done");
                    return 0;
                }
            }
            CheckContinue(bPause);
            return 1;
        }

        /// <summary>
        /// 判定串口数据通讯是否超时报警
        /// </summary>
        /// <param name="comLink"></param>
        /// <param name="bShowDialog">超时时是否显示提示对话框卡住流程 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int CheckCommTimeOut(ComLink comLink, bool bShowDialog = false)
        {
            if (comLink.IsTimeOut())
            {
                string str = Name + comLink.m_strName + "读取超时";
                if (LocationServices.GetLangType() == "en-us")
                    str = Name + comLink.m_strName + " timeout";
                ShowLog(str, LogLevel.Error);
                if (bShowDialog)
                {
                    ShowMessage(str, true, new[] { "复位" });
                }
                return 1;
            }
            CheckContinue();
            return 0;
        }

        /// <summary>
        /// 判定网口数据通讯是否超时报警
        /// </summary>
        /// <param name="tcpLink"></param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int CheckCommTimeOut(TcpLink tcpLink, bool bShowDialog = false)
        {
            if (tcpLink.IsTimeOut())
            {
                string str = Name + tcpLink.m_strName + "读取超时";
                if (LocationServices.GetLangType() == "en-us")
                    str = Name + tcpLink.m_strName + " timeout";
                ShowLog(str, LogLevel.Error);
                if (bShowDialog)
                {
                    ShowMessage(str, true, new[] { "复位" });
                }
                return 1;
            }
            CheckContinue();
            return 0;
        }

        /// <summary>
        /// 获取通讯命令超时时间设置
        /// </summary>
        /// <returns>超时时间</returns>
        public static int GetCommTimeOut()
        {
            int paramInt = SingletonPattern<SystemManager>.GetInstance().GetParamInt("CommunicationTimeOut");
            if (paramInt == 0)
                return 600;
            return paramInt;
        }

        /// <summary>
        /// 通过标签名称等待结果
        /// </summary>
        /// <param name="sTagName">标签名称</param>
        /// <param name="sValue">等待结果</param>
        /// <param name="nTimeOutS">超时时间(秒)，-1:表示一直等，0:表示参数OpcTimeOut中配置的时间</param>
        /// <param name="bShowDialog">超时时是否弹出对话框</param>
        /// <param name="bPause">超时时是否暂停</param>
        /// <returns>0:未超时  1:超时</returns>
        public int WaitOpcByTag(string sTagName, string sValue, int nTimeOutS = -1, bool bShowDialog = true, bool bPause = true)
        {
            string format1 = "等待OPC {0}:{1} = {2}";
            string format2 = "等待OPC {0}:{1} = {2} 超时";
            string format3 = "等待OPC {0}:{1} = {2} 超过{3}秒,继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                format1 = "Waiting for OPC {0}:{1} = {2}";
                format2 = "Waiting for OPC {0}:{1} = {2} timeout";
                format3 = "Wait for OPC {0}:{1} = {2} more than {3} seconds,continue to wait";
            }

            string opcTagDesc = SingletonPattern<OpcMgr>.GetInstance().GetOpcTagDesc(sTagName);
            ShowLog(string.Format(format1, sTagName, opcTagDesc, sValue));
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return 0;
            }
            int num = nTimeOutS;
            DateTime now = DateTime.Now;
            if (nTimeOutS == 0)
                num = SingletonPattern<SystemManager>.GetInstance().GetParamInt("OpcTimeOut");
            bool flag = false;
            while (true)
            {
                if (SingletonPattern<OpcMgr>.GetInstance().ReadDataByTag(sTagName) != sValue)
                {
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                    TimeSpan timeSpan = DateTime.Now - now;
                    if (nTimeOutS != -1 && timeSpan.TotalMilliseconds > (num * 1000))
                    {
                        string strInfo = string.Format(format2, sTagName, opcTagDesc, sValue);
                        if (bShowDialog)
                        {
                            CheckContinue(bPause);
                            if (ShowTimeOutDlg(strInfo, ErrorType.ErrOpcTimeOut, sTagName) == 0)
                                now = DateTime.Now;
                            else
                            {
                                CheckContinue(bPause);
                                return 1;
                            }
                        }
                        else
                        {
                            CheckContinue(bPause);
                            return 1;
                        }
                    }
                    else
                    {
                        int paramInt = SingletonPattern<SystemManager>.GetInstance().GetParamInt("OpcTimeOut");
                        if (nTimeOutS == -1 && timeSpan.TotalMilliseconds > (paramInt * 1000) && !flag)
                        {
                            flag = true;
                            ShowLog(string.Format(format3, sTagName, opcTagDesc, sValue, paramInt), LogLevel.Warn);
                        }
                        CheckContinue(false);
                    }
                }
                else
                    break;
            }
            CheckContinue(bPause);
            return 0;
        }

        /// <summary>
        /// 通过标签描述等待结果
        /// </summary>
        /// <param name="sDesc">标签描述</param>
        /// <param name="sValue">等待结果</param>
        /// <param name="nTimeOutS">超时时间，-1:表示一直等，0:表示参数OpcTimeOut中配置的时间</param>
        /// <param name="bShowDialog">超时时是否弹出对话框</param>
        /// <param name="bPause">超时时是否暂停</param>
        /// <returns>0:未超时  1:超时</returns>
        public int WaitOpcByDesc(string sDesc, string sValue, int nTimeOutS = -1, bool bShowDialog = true, bool bPause = true)
        {
            return WaitOpcByTag(SingletonPattern<OpcMgr>.GetInstance().GetOpcTagName(sDesc), sValue, nTimeOutS, bShowDialog, bPause);
        }

        /// <summary>
        /// 在网口上等待指定命令(命令可以多次读取组成,需要注意分隔符)，如果指定时间内未收到指定命令，则超时报警
        /// </summary>
        /// <param name="tcplink"></param>
        /// <param name="strCmd"></param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">流程暂停时是否响应暂停 true:等待暂停恢复  false:不响应暂停继续扫描 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int wait_recevie_cmd(TcpLink tcplink, string strCmd, int nTimeOutS = 0, bool bShowDialog = true, bool bPause = false)
        {
            string format1 = "等待{0}接收命令:{1}";
            string str1 = "接收到不匹配的命令:";
            string str2 = ",继续接收";
            string format2 = "{0}等待接收命令{1}超时";
            string format3 = "等待接收命令{0}超过{1}秒,继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                format1 = "Waiting for {0} receive cmd:{1}";
                str1 = "Mismatched command received:";
                str2 = ",continue to receive";
                format2 = "{0} Waiting for receive cmd:{1} timeout";
                format3 = "Wait for receive cmd:{0} more than {1} seconds,continue to wait";
            }
            ShowLog(string.Format(format1, tcplink.m_strName, strCmd));
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return 0;
            }
            string strData;
            string str3 = "";
            int num = 0;
            if (nTimeOutS == 0)
                nTimeOutS = GetCommTimeOut();
            bool flag = false;
            while (true)
            {
                tcplink.ReadLine(out strData);
                str3 += strData;
                if (strCmd != str3)
                {
                    if (str3.Length > 0)
                        ShowLog(str1 + str3 + str2, LogLevel.Warn);
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                    num += SingletonPattern<SystemManager>.GetInstance().ScanTime + tcplink.m_nTime;
                    if (nTimeOutS != -1 && num > nTimeOutS * 1000)
                    {
                        string str4 = string.Format(format2, Name, strCmd);
                        ShowLog(str4, LogLevel.Error);
                        if (bShowDialog)
                        {
                            CheckContinue(bPause);
                            if (ShowTimeOutDlg(str4, ErrorType.ErrTcpTimeOut, tcplink.m_strName) == 0)
                                num = 0;
                            else
                            {
                                CheckContinue(bPause);
                                return 1;
                            }
                        }
                        else
                        {
                            CheckContinue(bPause);
                            return 1;
                        }
                    }
                    else
                    {
                        int commTimeOut = GetCommTimeOut();
                        if (nTimeOutS == -1 && num > commTimeOut * 1000 && !flag)
                        {
                            flag = true;
                            ShowLog(string.Format(format3, strCmd, commTimeOut), LogLevel.Warn);
                        }
                        CheckContinue(false);
                    }
                }
                else
                    break;
            }
            CheckContinue(bPause);
            return 0;
        }

        /// <summary>
        /// 在网口上等待一行数据，在指定超时时间内，读取到一行则立即返回，否则超时提示
        /// </summary>
        /// <param name="tcplink">网口编号</param>
        /// <param name="strData"></param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">流程暂停时是否响应暂停 true:等待暂停恢复  false:不响应暂停继续扫描 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int wait_recevie_data(TcpLink tcplink, out string strData, int nTimeOutS = 0, bool bShowDialog = true, bool bPause = false)
        {
            string format1 = "等待{0}接收行数据";
            string format2 = "{0}等待接收行数据超时";
            string format3 = "等待接收行数据超过{0}秒,继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                format1 = "Waiting for {0} receive data";
                format2 = "{0} Waiting for receive data timeout";
                format3 = "Wait for receive data more than {0} seconds,continue to wait";
            }
            strData = "";
            ShowLog(string.Format(format1, tcplink.m_strName));
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return 0;
            }
            int num = 0;
            if (nTimeOutS == 0)
                nTimeOutS = GetCommTimeOut();
            bool flag = false;
            while (true)
            {
                tcplink.ReadLine(out strData);
                if (strData.Length <= 0)
                {
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                    num += SingletonPattern<SystemManager>.GetInstance().ScanTime + tcplink.m_nTime;
                    if (nTimeOutS != -1 && num > nTimeOutS * 1000)
                    {
                        string str = string.Format(format2, Name);
                        ShowLog(str, LogLevel.Error);
                        if (bShowDialog)
                        {
                            CheckContinue(bPause);
                            if (ShowTimeOutDlg(str, ErrorType.ErrTcpTimeOut, tcplink.m_strName) == 0)
                                num = 0;
                            else
                            {
                                CheckContinue(bPause);
                                return 1;
                            }
                        }
                        else
                        {
                            CheckContinue(bPause);
                            return 1;
                        }
                    }
                    else
                    {
                        int commTimeOut = GetCommTimeOut();
                        if (nTimeOutS == -1 && num > commTimeOut * 1000 && !flag)
                        {
                            flag = true;
                            ShowLog(string.Format(format3, commTimeOut), LogLevel.Warn);
                        }
                        CheckContinue(false);
                    }
                }
                else
                    break;
            }
            CheckContinue(bPause);
            return 0;
        }

        /// <summary>
        /// 在串口上等待指定命令(命令可以多条命令组成,需要注意分隔符)，如果指定时间内未收到指定命令，则超时报警
        /// </summary>
        /// <param name="comLink">串口编号</param>
        /// <param name="strCmd">欲等待的命令</param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框卡住当前流程, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">流程暂停时是否响应暂停 true:等待暂停恢复  false:不响应暂停继续扫描 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int wait_recevie_cmd(ComLink comLink, string strCmd, int nTimeOutS = -1, bool bShowDialog = true, bool bPause = false)
        {
            string format1 = "等待{0}接收命令:{1}";
            string str1 = "接收到不匹配的命令:";
            string str2 = ",继续接收";
            string format2 = "{0}等待接收命令{1}超时";
            string format3 = "等待接收命令{0}超过{1}秒,继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                format1 = "Waiting for {0} receive cmd:{1}";
                str1 = "Mismatched command received:";
                str2 = ",continue to receive";
                format2 = "{0} Waiting for receive cmd:{1} timeout";
                format3 = "Wait for receive cmd:{0} more than {1} seconds,continue to wait";
            }
            ShowLog(string.Format(format1, comLink.m_strName, strCmd));
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return 0;
            }
            string strData;
            string str3 = "";
            int num = 0;
            if (nTimeOutS == 0)
                nTimeOutS = GetCommTimeOut();
            bool flag = false;
            while (true)
            {
                comLink.ReadLine(out strData);
                str3 += strData;
                if (strCmd != str3)
                {
                    if (strData.Length > 0)
                        ShowLog(str1 + str3 + str2, LogLevel.Warn);
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                    num += SingletonPattern<SystemManager>.GetInstance().ScanTime + comLink.m_nTime;
                    if (nTimeOutS != -1 && num > nTimeOutS * 1000)
                    {
                        string str4 = string.Format(format2, Name, strCmd);
                        ShowLog(str4, LogLevel.Error);
                        if (bShowDialog)
                        {
                            CheckContinue(bPause);
                            if (ShowTimeOutDlg(str4, ErrorType.ErrComTimeOut, comLink.m_strName) == 0)
                                num = 0;
                            else
                            {
                                CheckContinue(bPause);
                                return 1;
                            }
                        }
                        else
                        {
                            CheckContinue(bPause);
                            return 1;
                        }
                    }
                    else
                    {
                        int commTimeOut = GetCommTimeOut();
                        if (nTimeOutS == -1 && num > commTimeOut * 1000 && !flag)
                        {
                            flag = true;
                            ShowLog(string.Format(format3, strCmd, commTimeOut), LogLevel.Warn);
                        }
                        CheckContinue(false);
                    }
                }
                else
                    break;
            }
            CheckContinue(bPause);
            return 0;
        }

        /// <summary>
        /// 在串口上等待一行数据，在指定超时时间内，读取到一行则立即返回，否则超时提示
        /// </summary>
        /// <param name="comLink">串口编号</param>
        /// <param name="strData">返回读到的数据</param>
        /// <param name="nTimeOutS">超时时间(以秒为单位)，-1:一直等待， 0：使用系统参数设置时间， 其它：特定时间</param>
        /// <param name="bShowDialog">超时后是否显示提示对话框并等待确认, true :显示对话框,  false:不显示 </param>
        /// <param name="bPause">流程暂停时是否响应暂停 true:等待暂停恢复  false:不响应暂停继续扫描 </param>
        /// <returns>0:未超时  1:超时  </returns>
        public int wait_recevie_Data(ComLink comLink, out string strData, int nTimeOutS = -1, bool bShowDialog = true, bool bPause = false)
        {
            string format1 = "等待{0}接收行数据";
            string format2 = "{0}等待接收数据超时";
            string format3 = "等待接收数据超过{0}秒,继续等待";
            if (LocationServices.GetLangType() == "en-us")
            {
                format1 = "Waiting for {0} receive data";
                format2 = "{0} Waiting for receive data timeout";
                format3 = "Wait for receive data more than {0} seconds,continue to wait";
            }
            ShowLog(string.Format(format1, comLink.m_strName));
            strData = "";
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
            {
                WaitTimeDelay(500);
                return 0;
            }
            int num = 0;
            if (nTimeOutS == 0)
                nTimeOutS = GetCommTimeOut();
            bool flag = false;
            while (true)
            {
                comLink.ReadLine(out strData);
                if (strData.Length <= 0)
                {
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                    num += SingletonPattern<SystemManager>.GetInstance().ScanTime + comLink.m_nTime;
                    if (nTimeOutS != -1 && num > nTimeOutS * 1000)
                    {
                        string str = string.Format(format2, Name);
                        ShowLog(str, LogLevel.Error);
                        if (bShowDialog)
                        {
                            CheckContinue(bPause);
                            if (ShowTimeOutDlg(str, ErrorType.ErrComTimeOut, comLink.m_strName) == 0)
                                num = 0;
                            else
                            {
                                CheckContinue(bPause);
                                return 1;
                            }
                        }
                        else
                        {
                            CheckContinue(bPause);
                            return 1;
                        }
                    }
                    else
                    {
                        int commTimeOut = GetCommTimeOut();
                        if (nTimeOutS == -1 && num > commTimeOut * 1000 && !flag)
                        {
                            flag = true;
                            ShowLog(string.Format(format3, commTimeOut), LogLevel.Warn);
                        }
                        CheckContinue(false);
                    }
                }
                else
                    break;
            }
            CheckContinue(bPause);
            return 0;
        }

        /// <summary>
        /// 站位公共处理步骤
        /// </summary>
        public void ThreadProc()
        {
            IsDeinit = false;

            while (_bRunThread)
            {
                string strLog1 = "手动停止运行";
                string strLog2 = "本站位出现异常,停止运行";
                if (LocationServices.GetLangType() == "en-us")
                {
                    strLog1 = "Manual stop";
                    strLog2 = "The station is abnormal and stops operation";
                }
                if (_nCurState == StationState.StateAuto)
                {
                    try
                    {
                        if (StationEnable)
                            StationInit();
                        if (_nCurState == StationState.StateManual)
                        {
                            ShowLog(strLog1, LogLevel.Error);
                            break;
                        }
                        if (_nCurState == StationState.StateEmg)
                        {
                            SingletonPattern<StationManager>.GetInstance().EmgStopAllStation();
                            ShowLog(strLog2, LogLevel.Error);
                            continue;
                        }
                        do
                        {
                            if (StationEnable)
                                StationProcess();
                            else
                                DisableRun();
                        }
                        while (_nCurState != StationState.StateManual);

                        ShowLog(strLog1, LogLevel.Error);
                    }
                    catch (StationException ex)
                    {
                        if (_nCurState != StationState.StateEmg)
                        {
                            _nCurState = StationState.StateEmg;
                            if (!SingletonPattern<StationManager>.GetInstance().IsEmg())
                            {
                                SingletonPattern<StationManager>.GetInstance().EmgStopAllStation();
                                SingletonPattern<RunInforManager>.GetInstance().Error(ErrorType.ErrSystem, Name, ex.Message);
                                ShowLog(ex.Message, LogLevel.Error);
                                ShowLog(strLog2, LogLevel.Error);
                            }
                            Debug.WriteLine(ex.Message);
                        }
                        else
                        {
                            ShowLog(ex.Message, LogLevel.Error);
                            Debug.WriteLine(ex.Message);
                        }
                    }
                    catch (SafeException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        ShowLog(strLog1, LogLevel.Error);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _nCurState = StationState.StateEmg;
                        SingletonPattern<StationManager>.GetInstance().EmgStopAllStation();
                        ShowLog(ex.Message, LogLevel.Error);
                        ShowLog(strLog2, LogLevel.Error);
                    }
                }
                Thread.Sleep(50);
            }

            IsDeinit = true;

            if (!StationEnable)
                return;
            StationDeinit();
        }

        /// <summary>
        /// 站位被禁用时空运行
        /// </summary>
        public void DisableRun()
        {
            WaitBegin();
            if (LocationServices.GetLangType() == "en-us")
                ShowLog("This station is disabled, ready to wait", LogLevel.Warn);
            else
                ShowLog("此站已禁用，就绪等待中", LogLevel.Warn);

            WaitTimeDelay(1000);
        }

        /// <summary>
        /// 站位初始化,虚函数，重写需要子类继承
        /// </summary>
        public virtual void StationInit()
        {
        }

        /// <summary>
        /// 站位结束处理过程,虚函数，重写需要子类继承
        /// </summary>
        public virtual void StationDeinit()
        {
        }

        /// <summary>
        /// 站位初始化为安全状态,虚函数，重写需要子类继承
        /// </summary>
        public virtual void InitSecurityState()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool IsStop()
        {
            return _nCurState == StationState.StateManual;
        }

        /// <summary>
        /// 响应流程暂停的处理，比如流水线在暂停时需要停止
        /// </summary>
        public virtual void OnPause()
        {
        }

        /// <summary>
        /// 响应流程恢复的处理，比如流水线在恢复时需要继续运行
        /// </summary>
        public virtual void OnResume()
        {
        }

        /// <summary>
        /// 站位急停,虚函数，重写需要子类继承
        /// </summary>
        public virtual void EmgStop()
        {
            for (int index = 0; index < 4; ++index)
            {
                if (AxisNumArray[index] > 0)
                    SingletonPattern<MotionManager>.GetInstance().StopEmg(AxisNumArray[index]);
            }
        }

        /// <summary>
        /// 手动运行一个函数
        /// </summary>
        /// <param name="func"></param>
        public void ManualRun(delegateFunc func)
        {
            string text1 = "当前正在自动运行中，是否要继续手动运行?";
            string caption = "提示";
            string text2 = "当前正在手动运行中，是否要继续加入手动运行动作队列?";
            if (LocationServices.GetLangType() == "en-us")
            {
                text1 = "Currently running automatically, do you want to continue running manually?";
                caption = "Tips";
                text2 = "You are currently running manually. Do you want to continue to join the manual operation action queue?";
            }
            if (SingletonPattern<StationManager>.GetInstance().IsAutoRunning() && MessageBox.Show(text1, caption, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel || _manualFuncList.Count > 0 && MessageBox.Show(text2, caption, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                return;
            lock (this)
                _manualFuncList.Add(func);
            StartManualRun();
        }

        /// <summary>
        /// 开始手动运行
        /// </summary>
        public void StartManualRun()
        {
            string strLog1 = "当前站位开始手动运行";
            string strLog2 = "当前站位处于模拟运行模式";
            if (LocationServices.GetLangType() == "en-us")
            {
                strLog1 = "Current station starts manual operation";
                strLog2 = "Current station is in simulated operation mode";
            }
            if (ManualThread == null)
                ManualThread = new Thread(ManualRunThread);
            if (ManualThread.ThreadState != System.Threading.ThreadState.Unstarted)
                return;
            _manaulState = ManualState.StateManualRun;
            ManualThread.Start();

            ManualStateChangedEvent?.Invoke(_manaulState);
            ShowLog(strLog1);
            if (SingletonPattern<SystemManager>.GetInstance().IsSimulateRunMode())
                ShowLog(strLog2);
        }

        /// <summary>
        /// 停止手动运行
        /// </summary>
        public void StopManualRun()
        {
            if (ManualThread == null)
                return;
            _manaulState = ManualState.StateManualStop;
            EmgStop();
            if (!ManualThread.Join(5000))
                ManualThread.Abort();
            ManualThread = null;
            _manualFuncList.Clear();

            ManualStateChangedEvent?.Invoke(_manaulState);
            if (LocationServices.GetLangType() == "en-us")
                ShowLog("Current station stops manual operation");
            else
                ShowLog("当前站位停止手动运行");
        }

        /// <summary>
        /// 暂停手动运行流程
        /// </summary>
        public void PauseManualRun()
        {
            if (ManualThread == null || _manaulState != ManualState.StateManualRun)
                return;
            _manaulState = ManualState.StateManualPause;
            OnPause();
            ManualStateChangedEvent?.Invoke(_manaulState);
            if (LocationServices.GetLangType() == "en-us")
                ShowLog("Manual operation is suspended at current station");
            else
                ShowLog("当前站位暂停手动运行");
        }

        /// <summary>
        /// 恢复手动运行流程
        /// </summary>
        public void ResumeManualRun()
        {
            if (ManualThread == null || _manaulState != ManualState.StateManualPause)
                return;
            _manaulState = ManualState.StateManualRun;
            OnResume();
            ManualStateChangedEvent?.Invoke(_manaulState);
            if (LocationServices.GetLangType() == "en-us")
                ShowLog("Continue manual operation at current station");
            else
                ShowLog("当前站位继续手动运行");
        }

        /// <summary>
        /// 手动运行的线程函数
        /// </summary>
        public void ManualRunThread()
        {
            string strLog1 = "本站位出现异常,停止运行";
            string strLog2 = "手动运行停止";
            string str1 = "出现异常";
            string strLog3 = "本站位出现异常,停止手动运行";
            string str2 = "手动运行已结束";
            if (LocationServices.GetLangType() == "en-us")
            {
                strLog1 = "The station is abnormal and stops operation";
                strLog2 = "Manual operation stop";
                str1 = " abnormal ";
                strLog3 = "The station is abnormal, stop manual operation";
                str2 = "Manual operation ended";
            }
            try
            {
                IsDeinit = false;
                while (_manualFuncList.Count > 0)
                {
                    _manualFuncList.ElementAt(0)();
                    lock (this)
                        _manualFuncList.RemoveAt(0);
                }
            }
            catch (StationException ex)
            {
                if (_manaulState != ManualState.StateManualEmg)
                {
                    _manaulState = ManualState.StateManualEmg;
                    if (!SingletonPattern<StationManager>.GetInstance().IsEmg())
                    {
                        SingletonPattern<StationManager>.GetInstance().EmgStopAllStation();
                        SingletonPattern<RunInforManager>.GetInstance().Error(ErrorType.ErrSystem, Name, ex.Message);
                        ShowLog(ex.Message, LogLevel.Error);
                        ShowLog(strLog1, LogLevel.Error);
                    }
                    Debug.WriteLine(ex.Message);
                }
                else
                {
                    ShowLog(ex.Message, LogLevel.Error);
                    Debug.WriteLine(ex.Message, LogLevel.Error);
                }
            }
            catch (SafeException ex)
            {
                Debug.WriteLine(ex.Message);
                ShowLog(strLog2, LogLevel.Error);
                _manaulState = ManualState.StateManualStop;
            }
            catch (Exception ex)
            {
                _manaulState = ManualState.StateManualEmg;
                SingletonPattern<StationManager>.GetInstance().EmgStopAllStation();
                MessageBox.Show(ex.ToString(), Name + str1, MessageBoxButton.OK, MessageBoxImage.Hand);
                ShowLog(ex.Message, LogLevel.Error);
                ShowLog(strLog3, LogLevel.Error);
            }
            finally
            {
                ManualThread = null;
                _manualFuncList.Clear();

                ManualStateChangedEvent?.Invoke(_manaulState);
                ShowLog(str2);
                Debug.WriteLine(str2);
            }
        }

        /// <summary>
        /// 根据轴号获取轴的方向
        /// </summary>
        /// <param name="nAxisNo">轴号，AxisX/AxisY/AxisZ/AxisU</param>
        /// <returns></returns>
        public bool GetAxisPositiveByAxisNo(int nAxisNo)
        {
            return PositiveMove[Array.IndexOf(AxisNumArray, nAxisNo)];
        }

        /// <summary>
        /// 根据轴的索引号获取轴的方向
        /// </summary>
        /// <param name="index">索引号，从0开始</param>
        /// <returns></returns>
        public bool GetAxisPositiveByIndex(int index)
        {
            return PositiveMove[index];
        }

        /// <summary>
        /// 根据轴号取反轴的方向，当发现UI界面上方向和实际方向不一致时取反
        /// </summary>
        /// <param name="nAxisNo">索引号，从0开始</param>
        public void InverseAxisPositiveByAxisNo(int nAxisNo)
        {
            int index = Array.IndexOf(AxisNumArray, nAxisNo);
            PositiveMove[index] = !PositiveMove[index];
        }

        /// <summary>
        /// 根据轴的索引号取反轴的方向，当发现UI界面上方向和实际方向不一致时取反
        /// </summary>
        /// <param name="index">索引号，从0开始</param>
        public void InverseAxisPositiveByIndex(int index)
        {
            PositiveMove[index] = !PositiveMove[index];
        }

        /// <summary>重命名轴名称</summary>
        /// <param name="index">轴号索引</param>
        /// <param name="strNewName">新名称</param>
        public void RenameAxisName(int index, string strNewName)
        {
            StrAxisName[index] = strNewName;
        }

        /// <summary>获取轴的名称</summary>
        /// <param name="index">索引号</param>
        /// <returns></returns>
        public string GetAxisName(int index)
        {
            return StrAxisName[index];
        }

        /// <summary>
        /// 站位处理流程,虚函数，重写需要子类继承
        /// </summary>
        public virtual void StationProcess()
        {
            WaitBegin();

            if (LocationServices.GetLangType() == "en-us")
                ShowLog("Process not written, default ready waiting", LogLevel.Warn);
            else
                ShowLog("流程未编写，默认就绪等待中", LogLevel.Warn);

            WaitTimeDelay(1000);
        }

        /// <summary>
        /// 站位是否已经等待开始
        /// </summary>
        /// <returns></returns>
        public virtual bool IsReady()
        {
            return _nCurState == StationState.StateReady;
        }

        /// <summary>
        /// 等待开始
        /// </summary>
        protected void WaitBegin()
        {
            string strLog = "站位就绪，等待开始";

            if (LocationServices.GetLangType() == "en-us")
                strLog = "Station ready, waiting to start";
            ShowLog(strLog);

            while (true)
            {
                CheckContinue();
                if (!_beginCycle)
                {
                    _nCurState = StationState.StateReady;
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                }
                else
                    break;
            }
            _beginCycle = false;
        }

        /// <summary>
        /// 检查系统当前是否会继续运行
        /// </summary>
        /// <param name="bPause">是否响应暂停状态</param>
        /// <returns></returns>
        public bool CheckContinue(bool bPause = true)
        {
            string str1 = "手动停止";
            string str2 = "外部异常停止";
            if (LocationServices.GetLangType() == "en-us")
            {
                str1 = " manual stop";
                str2 = " external abnormal stop";
            }
            while (true)
            {
                if (SingletonPattern<StationManager>.GetInstance().IsAutoThread(Thread.CurrentThread))
                {
                    switch (_nCurState)
                    {
                        case StationState.StateAuto:
                            return true;
                        case StationState.StateReady:
                            return true;
                        case StationState.StateEmg:
                            if (!IsDeinit)
                                throw new StationException(Name + str2);
                            return true;
                        case StationState.StatePause:
                            if (bPause)
                            {
                                Thread.Sleep(50);
                                break;
                            }
                            return true;
                        case StationState.StateManual:
                            if (!IsDeinit)
                                throw new SafeException(Name + str1);
                            return true;
                    }
                }
                else if (SingletonPattern<StationManager>.GetInstance().IsManaualThread(Thread.CurrentThread) || Thread.CurrentThread == ManualThread)
                {
                    switch (_manaulState)
                    {
                        case ManualState.StateManualRun:
                            return true;
                        case ManualState.StateManualStop:
                            if (!IsDeinit)
                                throw new SafeException(Name + str1);
                            return true;
                        case ManualState.StateManualPause:
                            if (bPause)
                            {
                                Thread.Sleep(50);
                                break;
                            }
                            return true;
                        case ManualState.StateManualEmg:
                            if (!IsDeinit)
                                throw new StationException(Name + str2);
                            return true;
                    }
                }
                else
                    return true;
            }
        }

        /// <summary>
        /// 站位流程延时等待,当写流程需要较长时间等待时使用此函数
        /// </summary>
        /// <param name="nMilliSeconds">时间，以毫秒为单位</param>
        /// <returns>1:已等待指定的时间，无其它返回值(站位出现报警异常会跳出无返回值)</returns>
        public int WaitTimeDelay(int nMilliSeconds)
        {
            int num = 0;
            do
            {
                Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                CheckContinue();
                num += SingletonPattern<SystemManager>.GetInstance().ScanTime;
            }
            while (num <= nMilliSeconds);
            return 1;
        }
    }
}
