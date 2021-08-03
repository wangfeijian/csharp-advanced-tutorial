/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    System IO manager class                  *
*********************************************************************/
using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using CommonTools.Manager;
using CommonTools.Model;
using CommonTools.Servers;
using CommonTools.Tools;
using CommonTools.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using MotionIO;

namespace AutoMationFrameworkDll
{
    public class IoManager : SingletonPattern<IoManager>
    {
        /// <summary>
        /// 系统输入输出的结构体
        /// </summary>
        public struct IoSysDefine
        {
            /// <summary>
            /// 功能描述
            /// </summary>
            public string StrName;

            /// <summary>
            /// 板卡号
            /// </summary>
            public int Card;

            /// <summary>
            /// 点位号
            /// </summary>
            public int Bit;

            /// <summary>
            /// 电平高低,0-低电平，1-高电平
            /// </summary>
            public bool Level;

            /// <summary>
            /// 当前状态是否已经有效
            /// </summary>
            public bool Trigger;
        }

        public SystemCfg SystemConfig { get; set; }

        /// <summary>
        /// IO卡类指针向量
        /// </summary>
        public List<IoControl> ListCard = new List<IoControl>();

        /// <summary>
        /// IO输入点名称与点位映射
        /// </summary>
        public Dictionary<string, long> DicIn = new Dictionary<string, long>();

        /// <summary>
        /// IO输出点名称与点位映射
        /// </summary>
        public Dictionary<string, long> DicOut = new Dictionary<string, long>();

        /// <summary>
        /// 输入IO点名称翻译
        /// </summary>
        public Dictionary<string, string> DicInTranslate = new Dictionary<string, string>();

        /// <summary>
        /// 输出IO点名称翻译
        /// </summary>
        public Dictionary<string, string> DicOutTranslate = new Dictionary<string, string>();

        /// <summary>
        /// 系统常用IO输入
        /// </summary>
        public List<IoSysDefine> ListIoSystemIn = new List<IoSysDefine>();

        /// <summary>
        /// 系统常用IO输出
        /// </summary>
        public List<IoSysDefine> ListIoSystemOut = new List<IoSysDefine>();

        private static uint _nLightState;
        private IoSysDefine _ioEmgStop;
        private IoSysDefine _ioBegin;
        private IoSysDefine _ioResetting;
        private IoSysDefine _ioDoor;
        private IoSysDefine _ioPause;
        private IoSysDefine _ioRedLight;
        private IoSysDefine _ioYellowLight;
        private IoSysDefine _ioGreenLight;
        private IoSysDefine _ioBuzzing;
        private Thread _lightThread;
        private bool _bRunLightThread;
        private int _nBuzzingTime = 1000;
        private int _nYelloTime = 1000;
        private int _nGreenTime = 1000;
        private int _nRedTime = 1000;

        /// <summary>
        /// 定义一个IO状态变化委托函数
        /// </summary>
        /// <param name="nCard"></param>
        public delegate void IoChangedHandler(int nCard);

        /// <summary>
        /// 定义一个Io状态变化事件
        /// </summary>
        public event IoChangedHandler IoChangedEvent;

        /// <summary>
        /// 灯的状态
        /// </summary>
        public static uint Light => _nLightState;

        /// <summary>
        /// 蜂鸣器闪烁时间间隔
        /// </summary>
        public int BuzzingTime
        {
            get
            {
                return _nBuzzingTime;
            }
            set
            {
                _nBuzzingTime = value;
            }
        }

        /// <summary>
        /// 黄灯闪烁时间间隔
        /// </summary>
        public int YelloTime
        {
            get
            {
                return _nYelloTime;
            }
            set
            {
                _nYelloTime = value;
            }
        }

        /// <summary>
        /// 绿灯闪烁时间间隔
        /// </summary>
        public int GreenTime
        {
            get
            {
                return _nGreenTime;
            }
            set
            {
                _nGreenTime = value;
            }
        }

        /// <summary>
        /// 红灯闪烁时间间隔
        /// </summary>
        public int RedTime
        {
            get
            {
                return _nRedTime;
            }
            set
            {
                _nGreenTime = value;
            }
        }

        /// <summary>卡号总数</summary>
        public int CountCard => ListCard.Count;

        private void IoChanged(int nCard)
        {
            IoChangedEvent?.Invoke(nCard);
        }

        /// <summary>三色灯以及蜂鸣器状态控制</summary>
        /// <param name="state"></param>
        public void AlarmLight(uint state)
        {
            if (((int)_nLightState & 240) != 0 && ((int)state & 240) == 0)
            {
                _nLightState = state;
                Thread.Sleep(200);
            }
            WrtieSystemIo(_ioRedLight, (state & LightState.红灯开) > 0U);
            WrtieSystemIo(_ioGreenLight, (state & LightState.绿灯开) > 0U);
            WrtieSystemIo(_ioYellowLight, (state & LightState.黄灯开) > 0U);
            WrtieSystemIo(_ioBuzzing, (state & LightState.蜂鸣开) > 0U);
            _nLightState = state;
        }

        private void LightThread()
        {
            bool bEnable = false;
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            while (_bRunLightThread)
            {
                Thread.Sleep(100);
                if ((_nLightState & LightState.蜂鸣闪) > 0U)
                {
                    num1 += 100;
                    if (num1 > _nBuzzingTime)
                    {
                        num1 = 0;
                        WrtieSystemIo(_ioBuzzing, bEnable);
                    }
                }
                if ((_nLightState & LightState.绿灯闪) > 0U)
                {
                    num2 += 100;
                    if (num2 > _nGreenTime)
                    {
                        num2 = 0;
                        WrtieSystemIo(_ioGreenLight, bEnable);
                    }
                }
                if ((_nLightState & LightState.黄灯闪) > 0U)
                {
                    num3 += 100;
                    if (num3 > _nYelloTime)
                    {
                        num3 = 0;
                        WrtieSystemIo(_ioYellowLight, bEnable);
                    }
                }
                if ((_nLightState & LightState.红灯闪) > 0U)
                {
                    num4 += 100;
                    if (num4 > _nRedTime)
                    {
                        num4 = 0;
                        WrtieSystemIo(_ioRedLight, bEnable);
                    }
                }
                bEnable = !bEnable;
            }
        }

        private void StartLightThread()
        {
            if (_lightThread == null)
                _lightThread = new Thread(LightThread);
            if ((uint)_lightThread.ThreadState <= 0U)
                return;
            _bRunLightThread = true;
            _lightThread.Start();
        }

        private void StopLightThread()
        {
            if (_lightThread == null)
                return;
            _bRunLightThread = false;
            if (!_lightThread.Join(5000))
                _lightThread.Abort();
            _lightThread = null;
        }

        /// <summary>
        /// 写入系统功能IO
        /// </summary>
        /// <param name="isd"></param>
        /// <param name="bEnable"></param>
        private void WrtieSystemIo(IoSysDefine isd, bool bEnable)
        {
            if (isd.StrName == null)
                return;
            WriteIoBit(isd.Card, isd.Bit, bEnable ? isd.Level : !isd.Level);
        }

        /// <summary>
        /// 根据板卡名字动态加载对应的板卡类
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="nIndex"></param>
        /// <param name="nCardNo"></param>
        private void AddCard(string strName, int nIndex, int nCardNo)
        {
            string str = LocationServices.GetLang("NotFoundIOCard");

            Type type = Assembly.GetAssembly(typeof(IoControl)).GetType("MotionIO.IoControl" + strName);
            if (type == null)
                throw new Exception(string.Format(str, strName));
            object[] objArray = {
                 nIndex,
                 nCardNo
            };
            ListCard.Add(Activator.CreateInstance(type, objArray) as IoControl);
        }

        /// <summary>
        /// 读取系统配置文件里的IO板卡信息
        /// </summary>
        public void ReadCfgFromFile()
        {
            ListCard.Clear();

            SystemConfig = SimpleIoc.Default.GetInstance<SystemConfigViewModel>().SystemConfig;

            foreach (var ioCardInfo in SystemConfig.IoCardsList)
            {
                string cardIndex = ioCardInfo.CardIndex;
                string cardNum = ioCardInfo.CardNum;
                string cardType = ioCardInfo.CardType.Trim();
                AddCard(cardType, Convert.ToInt32(cardIndex), Convert.ToInt32(cardNum));
            }

            ReadIoInFromCfg();
            ReadIoOutFromCfg();
            ReadSystemIoFromCfg();
        }

        /// <summary>
        /// 获取配置文件中的IO输入点位
        /// </summary>
        private void ReadIoInFromCfg()
        {
            DicIn.Clear();
            DicInTranslate.Clear();
            foreach (var ioInputPoint in SystemConfig.IoInput)
            {
                string cardIndex = ioInputPoint.CardIndex;
                string pointIndex = ioInputPoint.PointIndex;
                string pointName = ioInputPoint.PointName;
                string pointEngName = ioInputPoint.PointEngName;
                if (string.IsNullOrEmpty(pointEngName))
                {
                    pointEngName = pointName;
                }

                int card = Convert.ToInt32(cardIndex);
                int point = Convert.ToInt32(pointIndex);

                if (pointName != string.Empty)
                {
                    try
                    {
                        DicIn.Add(pointName, card << 8 | point);
                        DicInTranslate.Add(pointName, pointEngName);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }

                if (card - 1 < ListCard.Count)
                {
                    string[] pointStrName = ListCard.ElementAt(card - 1).StrArrayIn;
                    if (point - 1 < pointStrName.Length)
                    {
                        pointStrName[point - 1] = pointName;
                    }
                    else
                    {
                        MessageBox.Show(string.Format(LocationServices.GetLang("ConfigIOError"), card, point));
                    }
                }
            }

        }

        /// <summary>
        /// 获取配置文件中的IO输出点位
        /// </summary>
        private void ReadIoOutFromCfg()
        {
            DicOut.Clear();
            DicOutTranslate.Clear();
            foreach (var ioOutputPoint in SystemConfig.IoOutput)
            {
                string cardIndex = ioOutputPoint.CardIndex;
                string pointIndex = ioOutputPoint.PointIndex;
                string pointName = ioOutputPoint.PointName;
                string pointEngName = ioOutputPoint.PointEngName;
                if (string.IsNullOrEmpty(pointEngName))
                {
                    pointEngName = pointName;
                }

                int card = Convert.ToInt32(cardIndex);
                int point = Convert.ToInt32(pointIndex);

                if (pointName != string.Empty)
                {
                    try
                    {
                        DicOut.Add(pointName, card << 8 | point);
                        DicOutTranslate.Add(pointName, pointEngName);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }

                if (card - 1 < ListCard.Count)
                {
                    string[] pointStrName = ListCard.ElementAt(card - 1).StrArrayOut;
                    if (point - 1 < pointStrName.Length)
                    {
                        pointStrName[point - 1] = pointName;
                    }
                    else
                    {
                        MessageBox.Show(string.Format(LocationServices.GetLang("ConfigIOError"), card, point));
                    }
                }
            }
        }

        /// <summary>
        /// 读取系统配置文件里系统常用IO输入输出点配置
        /// </summary>
        public void ReadSystemIoFromCfg()
        {
            ListIoSystemIn.Clear();
            ListIoSystemOut.Clear();

            foreach (var item in SystemConfig.SysInput)
            {
                IoSysDefine ioSysDefine = new IoSysDefine
                {
                    StrName = item.FuncDesc,
                    Card = Convert.ToInt32(item.CardNum),
                    Bit = Convert.ToInt32(item.PointIndex),
                    Level = item.EffectiveLevel.Length > 0U && item.EffectiveLevel == "1"
                };

                ListIoSystemIn.Add(ioSysDefine);
            }
            for (int index = 0; index < ListIoSystemIn.Count; ++index)
            {
                string strName = ListIoSystemIn.ElementAt(index).StrName;
                if (strName == "急停")
                    _ioEmgStop = ListIoSystemIn.ElementAt(index);
                if (strName == "启动")
                    _ioBegin = ListIoSystemIn.ElementAt(index);
                if (strName == "复位")
                    _ioResetting = ListIoSystemIn.ElementAt(index);
                if (strName.IndexOf("安全门", StringComparison.Ordinal) != -1)
                    _ioDoor = ListIoSystemIn.ElementAt(index);
                if (strName.IndexOf("暂停", StringComparison.Ordinal) != -1)
                    _ioPause = ListIoSystemIn.ElementAt(index);
            }

            foreach (var item in SystemConfig.SysOutput)
            {
                IoSysDefine ioSysDefine = new IoSysDefine
                {
                    StrName = item.FuncDesc,
                    Card = Convert.ToInt32(item.CardNum),
                    Bit = Convert.ToInt32(item.PointIndex),
                    Level = item.EffectiveLevel.Length > 0U && item.EffectiveLevel == "1"
                };

                ListIoSystemOut.Add(ioSysDefine);
            }

            for (int index = 0; index < ListIoSystemOut.Count; ++index)
            {
                string strName = ListIoSystemOut.ElementAt(index).StrName;
                if (strName == "红灯")
                    _ioRedLight = ListIoSystemOut.ElementAt(index);
                if (strName == "黄灯")
                    _ioYellowLight = ListIoSystemOut.ElementAt(index);
                if (strName == "绿灯")
                    _ioGreenLight = ListIoSystemOut.ElementAt(index);
                if (strName.IndexOf("蜂鸣", StringComparison.Ordinal) != -1)
                    _ioBuzzing = ListIoSystemOut.ElementAt(index);
            }
        }

        /// <summary>
        /// 读取指定卡的输入状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nData"></param>
        /// <returns></returns>
        public bool ReadIoIn(int nCardIndex, ref int nData)
        {
            if (nCardIndex > ListCard.Count)
                return false;
            return ListCard.ElementAt(nCardIndex - 1).ReadIoIn(ref nData);
        }

        /// <summary>
        /// 读取指定卡的输入位状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public bool ReadIoInBit(int nCardIndex, int nIndex)
        {
            if (nCardIndex > ListCard.Count)
                return false;
            return ListCard.ElementAt(nCardIndex - 1).ReadIoInBit(nIndex);
        }

        /// <summary>
        /// 读取IO输入点状态
        /// </summary>
        /// <param name="strIoName">IO 输入点名称</param>
        /// <returns></returns>
        public bool ReadIoInBit(string strIoName)
        {
            long num1;
            if (DicIn.TryGetValue(strIoName, out num1))
                return ReadIoInBit((int)(num1 >> 8), (int)(num1 & byte.MaxValue));
            string text = string.Format(LocationServices.GetLang("IoInError"), strIoName);
            string caption = LocationServices.GetLang("IoGetError");
            MessageBox.Show(text, caption);
            return false;
        }

        /// <summary>
        /// 获取指定IO输入点的缓冲状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public bool GetIoInState(int nCardIndex, int nIndex)
        {
            if (nCardIndex > ListCard.Count)
                return false;
            return ListCard.ElementAt(nCardIndex - 1).GetIoInState(nIndex);
        }

        /// <summary>
        /// 获取指定IO输入点的缓冲状态
        /// </summary>
        /// <param name="strIoName">IO 输入点名称</param>
        /// <returns></returns>
        public bool GetIoInState(string strIoName)
        {
            long num1;
            if (DicIn.TryGetValue(strIoName, out num1))
                return GetIoInState((int)(num1 >> 8), (int)(num1 & byte.MaxValue));
            string text = string.Format(LocationServices.GetLang("IoInError"), strIoName);
            string caption = LocationServices.GetLang("IoGetError");
            MessageBox.Show(text, caption);
            return false;
        }

        /// <summary>
        /// 读取指定卡的输出状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nData"></param>
        /// <returns></returns>
        public bool ReadIoOut(int nCardIndex, ref int nData)
        {
            if (nCardIndex > ListCard.Count)
                return false;
            return ListCard.ElementAt(nCardIndex - 1).ReadIoOut(ref nData);
        }

        /// <summary>
        /// 读取指定卡IO点的输出缓冲状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public bool GetIoOutState(int nCardIndex, int nIndex)
        {
            if (nCardIndex > ListCard.Count)
                return false;
            return ListCard.ElementAt(nCardIndex - 1).GetIoOutState(nIndex);
        }

        /// <summary>
        /// 读取IO点的输出缓冲状态
        /// </summary>
        /// <param name="strIoName">IO 输出点名称</param>
        /// <returns></returns>
        public bool GetIoOutState(string strIoName)
        {
            long num1;
            if (DicOut.TryGetValue(strIoName, out num1))
                return GetIoOutState((int)(num1 >> 8), (int)(num1 & byte.MaxValue));
            string text = string.Format(LocationServices.GetLang("IoOutError"), strIoName);
            string caption = LocationServices.GetLang("IoSetError");
            MessageBox.Show(text, caption);
            return false;
        }

        /// <summary>
        /// 获取指定IO输出点的状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public bool ReadIoOutBit(int nCardIndex, int nIndex)
        {
            if (nCardIndex > ListCard.Count)
                return false;
            return ListCard.ElementAt(nCardIndex - 1).ReadIoOutBit(nIndex);
        }

        /// <summary>
        /// 读取IO点的输出状态
        /// </summary>
        /// <param name="strIoName">IO 输出点名称</param>
        /// <returns></returns>
        public bool ReadIoOutBit(string strIoName)
        {
            long num1;
            if (DicOut.TryGetValue(strIoName, out num1))
                return ReadIoOutBit((int)(num1 >> 8), (int)(num1 & byte.MaxValue));
            string text = string.Format(LocationServices.GetLang("IoOutError"), strIoName);
            string caption = LocationServices.GetLang("IoSetError");
            MessageBox.Show(text, caption);
            return false;
        }

        /// <summary>
        /// 得到输入IO的名字
        /// </summary>
        /// <param name="nCardIndex">卡号，从1开始</param>
        /// <param name="nIndex">索引号，从1开始</param>
        /// <returns></returns>
        public string GetIoInName(int nCardIndex, int nIndex)
        {
            if (nCardIndex > ListCard.Count)
                return string.Empty;
            return ListCard.ElementAt(nCardIndex - 1).StrArrayIn[nIndex - 1];
        }

        /// <summary>
        /// 得到输出IO的名字
        /// </summary>
        /// <param name="nCardIndex">卡号，从1开始</param>
        /// <param name="nIndex">索引号，从1开始</param>
        /// <returns></returns>
        public string GetIoOutName(int nCardIndex, int nIndex)
        {
            if (nCardIndex > ListCard.Count)
                return string.Empty;
            return ListCard.ElementAt(nCardIndex - 1).StrArrayOut[nIndex - 1];
        }

        /// <summary>
        /// 设置指定IO输出点的状态
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="nIndex"></param>
        /// <param name="bBit"></param>
        /// <returns></returns>
        public bool WriteIoBit(int nCardIndex, int nIndex, bool bBit)
        {
            if (nCardIndex > ListCard.Count)
                return false;
            return ListCard.ElementAt(nCardIndex - 1).WriteIoBit(nIndex, bBit);
        }

        /// <summary>
        /// 设置IO点的输出状态
        /// </summary>
        /// <param name="strIoName">IO 输出点名称</param>
        /// <param name="bBit">将要被设置的状态</param>
        /// <returns></returns>
        public bool WriteIoBit(string strIoName, bool bBit)
        {
            long num1;
            if (DicOut.TryGetValue(strIoName, out num1))
                return WriteIoBit((int)(num1 >> 8), (int)(num1 & byte.MaxValue), bBit);

            string text = string.Format(LocationServices.GetLang("IoOutError"), strIoName);
            string caption = LocationServices.GetLang("IoSetError");
            MessageBox.Show(text, caption);
            return false;
        }

        /// <summary>
        /// 初始化所有IO板卡
        /// </summary>
        /// <returns></returns>
        public bool InitAllCard()
        {
            bool flag = true;
            foreach (IoControl ioCtrl in ListCard)
            {
                if (!ioCtrl.Init())
                    flag = false;
            }
            StartLightThread();
            return flag;
        }

        /// <summary>
        /// 反初始化所有IO板卡
        /// </summary>
        public void DeinitAllCard()
        {
            StopLightThread();
            foreach (IoControl ioCtrl in ListCard)
            {
                if (ioCtrl.IsEnable())
                    ioCtrl.DeInit();
            }
        }

        /// <summary>
        /// 判断系统ＩＯ是否要触发站位状态变更
        /// </summary>
        /// <param name="isd"></param>
        /// <returns></returns>
        public bool IsSystemIoTrigger(IoSysDefine isd)
        {
            if (isd.StrName == null)
                return false;
            if (GetIoInState(isd.Card, isd.Bit) == isd.Level)
                return !isd.Trigger;
            if (isd.Trigger)
                isd.Trigger = false;
            return false;
        }

        /// <summary>
        /// 安全门是否打开
        /// </summary>
        /// <returns></returns>
        public bool IsSafeDoorOpen()
        {
            if (string.IsNullOrEmpty(_ioDoor.StrName))
                return false;
            return ReadIoInBit(_ioDoor.Card, _ioDoor.Bit) == _ioDoor.Level;
        }

        /// <summary>
        /// 系统自动扫描线程函数
        /// </summary>
        public override void ThreadMonitor()
        {
            int nData = 0;
            _ioResetting.Trigger = false;
            _ioBegin.Trigger = false;
            _ioEmgStop.Trigger = false;
            _ioDoor.Trigger = false;
            _ioPause.Trigger = false;
            string strMsg1 = LocationServices.GetLang("EmgPress");
            string strMsg2 = LocationServices.GetLang("SafeDoorOpen");

            for (int index = 0; index < ListCard.Count; ++index)
            {
                if (ListCard.ElementAt(index).ReadIoIn(ref nData) && index == _ioEmgStop.Card - 1 && IsSystemIoTrigger(_ioEmgStop))
                {
                    Thread.Sleep(50);
                    ListCard.ElementAt(index).ReadIoIn(ref nData);
                    if (IsSystemIoTrigger(_ioEmgStop))
                    {
                       // SingletonTemplate<StationMgr>.GetInstance().EmgStopAllStation();
                        SingletonPattern<RunInforManager>.GetInstance().Error(ErrorType.ErrEmg, "EStop", strMsg1);
                    }
                }
            }
            while (BRunThread)
            {
                for (int index = 0; index < ListCard.Count; ++index)
                {
                    if (ListCard.ElementAt(index).ReadIoIn(ref nData) && ListCard.ElementAt(index).IsDataChange())
                    {
                        if (index == _ioEmgStop.Card - 1)
                        {
                            if (IsSystemIoTrigger(_ioEmgStop))
                            {
                                Thread.Sleep(50);
                                ListCard.ElementAt(index).ReadIoIn(ref nData);
                                if (IsSystemIoTrigger(_ioEmgStop))
                                {
                                    //SingletonTemplate<StationMgr>.GetInstance().EmgStopAllStation();
                                    SingletonPattern<RunInforManager>.GetInstance().Error(ErrorType.ErrEmg, "EStop", strMsg1);
                                }
                            }

                            if (IsSystemIoTrigger(_ioResetting))
                            {
                                //SingletonTemplate<StationMgr>.GetInstance().ResetAllStation();
                            }

                            if (IsSystemIoTrigger(_ioBegin) /*&& SingletonTemplate<StationMgr>.GetInstance().IsPause()*/)
                            {
                                if (GetInstance().IsSafeDoorOpen())
                                {
                                    string text = LocationServices.GetLang("SafeDoorOpenContinue");
                                    string caption = LocationServices.GetLang("Warning");
                                    if (MessageBox.Show(text, caption, MessageBoxButton.YesNo,
                                            MessageBoxImage.Exclamation, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                                    {
                                        //SingletonTemplate<StationMgr>.GetInstance().ResumeAllStation();
                                    }
                                }
                                else
                                {
                                //SingletonTemplate<StationMgr>.GetInstance().ResumeAllStation()
                                }
                            }
                            if (IsSystemIoTrigger(_ioDoor) && SingletonPattern<SystemManager>.GetInstance().GetParamBool("SafetyDoor"))
                            {
                                //SingletonTemplate<StationMgr>.GetInstance().PauseAllStation();
                                SingletonPattern<RunInforManager>.GetInstance().Warning(strMsg2);
                            }

                            if (IsSystemIoTrigger(_ioPause))
                            {
                            //SingletonTemplate<StationMgr>.GetInstance().PauseAllStation();
                            }
                        }
                        IoChanged(index);
                    }
                    Thread.Sleep(SingletonPattern<SystemManager>.GetInstance().ScanTime);
                }
            }
        }

        /// <summary>
        /// 获取输入IO名称的翻译
        /// </summary>
        /// <param name="strIoInName"></param>
        /// <returns></returns>
        public string GetIoInTranslate(string strIoInName)
        {
            string str;
            if (DicInTranslate.TryGetValue(strIoInName, out str))
                return str;
            return strIoInName;
        }

        /// <summary>
        /// 获取输出IO名称的翻译
        /// </summary>
        /// <param name="strIoOutName"></param>
        /// <returns></returns>
        public string GetIoOutTranslate(string strIoOutName)
        {
            string str;
            if (DicOutTranslate.TryGetValue(strIoOutName, out str))
                return str;
            return strIoOutName;
        }
    }

    /// <summary>三色灯及蜂鸣器状态定义</summary>
    public class LightState
    {
        /// <summary>所有的关闭</summary>
        public static readonly uint 所有关 = 0;
        /// <summary>绿灯开</summary>
        public static readonly uint 绿灯开 = 1;
        /// <summary>红灯开</summary>
        public static readonly uint 红灯开 = 2;
        /// <summary>黄灯开</summary>
        public static readonly uint 黄灯开 = 4;
        /// <summary>蜂鸣器开</summary>
        public static readonly uint 蜂鸣开 = 8;
        /// <summary>绿灯闪</summary>
        public static readonly uint 绿灯闪 = 16;
        /// <summary>红灯闪</summary>
        public static readonly uint 红灯闪 = 32;
        /// <summary>黄灯闪</summary>
        public static readonly uint 黄灯闪 = 64;
        /// <summary>蜂鸣器闪</summary>
        public static readonly uint 蜂鸣闪 = 128;
    }
}