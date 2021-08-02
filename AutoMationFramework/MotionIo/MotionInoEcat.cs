/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-02                               *
*                                                                    *
*           ModifyTime:     2021-08-02                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Motion Ino card control class            *
*********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Inovance.InoMotionCotrollerShop.InoServiceContract.EtherCATConfigApi;
using System.Collections.Generic;
using System.Windows;
using CommonTools.Manager;
using CommonTools.Tools;

namespace MotionIO
{
    /// <summary>
    /// 汇川EtherCAT控制卡资源
    /// </summary>
    public class InoEtherCatCard
    {
        /// <summary>
        /// 控制卡句柄
        /// </summary>
        public UInt64 CardHandle = ImcApi.ERR_HANDLE;

        /// <summary>
        /// 卡内资源
        /// </summary>
        public ImcApi.TRsouresNum Resoures;

        private static InoEtherCatCard _instance;

        private static readonly object Lock = new object();

        private bool _bInited;


        /// <summary>
        /// 是否初始化
        /// </summary>
        public bool IsInited => _bInited;

        /// <summary>
        /// 实例
        /// </summary>
        /// <returns></returns>
        public static InoEtherCatCard Instance()
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        _instance = new InoEtherCatCard();
                    }
                }

            }

            return _instance;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private InoEtherCatCard()
        {
            _bInited = Init();
        }

        /// <summary>
        /// 板卡初始化
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            //TRACE("init card\r\n");
            string str1 = "控制卡IMC30G-E读取卡数量失败, result = {0}";
            string str2 = "控制卡IMC30G-E不存在。";
            string str3 = "控制卡IMC30G-E打开失败, result = {0}";
            string str4 = "控制卡IMC30G-E下载设备文件失败, result = {0}";
            string str5 = "获取主站状态失败,result = {0}";
            string str6 = "启动EtherCAT失败,result = {0}";
            string str7 = "控制卡IMC30G-E下载参数文件失败, result = {0}";
            string str8 = "控制卡IMC30G-E扫描卡内资源失败, result = {0}";
            string str9 = "控制卡IMC30G-E初始化失败";
            if (LocationServices.GetLangType() == "en-us")
            {
                str1 = "Failed to read the number of IMC30G-E control cards, result = {0}";
                str2 = "The control card IMC30G-E does not exist. ";
                str3 = "Failed to open control card IMC30G-E, result = {0}";
                str4 = "IMC30G-E of control card failed to download device file, result = {0}";
                str5 = "Failed to get master status, result = {0}";
                str6 = "Failed to start EtherCAT, result = {0}";
                str7 = "IMC30G-E of control card failed to download parameter file, result = {0}";
                str8 = "Control card IMC30G-E failed to scan resources in the card, result = {0}";
                str9 = "Control card IMC30G-E initialization failed";
            }
            try
            {
                //【1】获取卡数量
                int nCardNum = 0;
                uint ret = ImcApi.IMC_GetCardsNum(ref nCardNum);
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionInit, "InoEcat", string.Format(str1, ret.ToString("x8")));

                    return false;
                }
                else
                {
                    if (nCardNum <= 0)
                    {
                        RunInforManager.GetInstance().Error(ErrorType.ErrMotionInit, "InoEcat", string.Format(str2));

                        return false;
                    }
                }

                //【2】打开卡句柄，打开第一张卡
                ret = ImcApi.IMC_OpenCardHandle(0, ref CardHandle);
                Thread.Sleep(200);
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    CardHandle = ImcApi.ERR_HANDLE;
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionInit, "InoEcat", string.Format(str3, ret.ToString("x8")));

                    return false;
                }

                //【3】下载设备参数
                ret = ImcApi.IMC_DownLoadDeviceConfig(CardHandle, "device_config.xml");
                Thread.Sleep(2000);
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionInit, "InoEcat", string.Format(str4, ret.ToString("x8")));

                    return false;
                }

                uint masterStatus = 0;
                ret = ImcApi.IMC_GetECATMasterSts(CardHandle, ref masterStatus);
                if (ret != 0)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionInit, "InoEcat", string.Format(str5, ret.ToString("x8")));

                    return false;
                }
                else
                {
                    if (masterStatus != ImcApi.EC_MASTER_OP)
                    {
                        ret = ImcApi.IMC_ScanCardECAT(CardHandle, 1);      //默认阻塞式启动EtherCAT
                        if (ret != 0)
                        {
                            RunInforManager.GetInstance().Error(ErrorType.ErrMotionInit, "InoEcat", string.Format(str6, ret.ToString("x8")));

                            return false;
                        }
                    }
                }

                //【5】扫描卡内资源
                ret = ImcApi.IMC_GetCardResource(CardHandle, ref Resoures);
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionInit, "InoEcat", string.Format(str8, ret.ToString("x8")));

                    return false;
                }

                //【6】所有轴断使能
                ImcApi.IMC_AxServoOff(CardHandle, 0, Resoures.axNum);


                //【7】下载系统参数
                ret = ImcApi.IMC_DownLoadSystemConfig(CardHandle, "system_config.xml");
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionInit, "InoEcat", string.Format(str7, ret.ToString("x8")));

                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, str9, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }

    /// <summary>
    /// 汇川EtherCAT运动控制卡封装,类名必须以"Motion_"前导，否则加载不到
    /// </summary>
    public class MotionInoEcat : Motion
    {
        /// <summary>
        /// 控制卡句柄
        /// </summary>
        private UInt64 _hCardHandle = ImcApi.ERR_HANDLE;
        private int _nMultipleInit;

        private Dictionary<int, int> _dictCrdAxis = new Dictionary<int, int>();

        /// <summary>
        /// 回原点参数
        /// </summary>
        private ImcApi.THomingPara _homePara;

        //todo:板卡类应该只初始化一次
        /// <summary>构造函数
        /// 
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
        public MotionInoEcat(int cardIndex, string strName, int nMinAxisNo, int nMaxAxisNo)
            : base(cardIndex, strName, nMinAxisNo, nMaxAxisNo)
        {
            BEnable = false;

            _homePara.homeMethod = 0;
            _homePara.offset = 0;
            _homePara.highVel = 10000;
            _homePara.lowVel = 1000;
            _homePara.acc = 50000;
            _homePara.overtime = 10000;
        }
        /// <summary>
        /// 轴卡初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            try
            {
                _hCardHandle = InoEtherCatCard.Instance().CardHandle;
                if (_hCardHandle != ImcApi.ERR_HANDLE)
                {
                    BEnable = true;
                    return true;
                }
                else
                {
                    BEnable = false;
                    return false;
                }
            }
            catch
            {
                BEnable = false;
                return false;
            }
        }

        /// <summary>
        /// 关闭轴卡
        /// </summary>
        /// <returns></returns>
        public override bool DeInit()
        {
            _nMultipleInit = 0;
            uint ret = ImcApi.IMC_CloseCardHandle(_hCardHandle);
            if (ret == ImcApi.EXE_SUCCESS)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    string str1 = "IMC30G-E板卡库文件关闭出错! result = {0}";
                    if (LocationServices.GetLangType() == "en-us")
                    {
                        str1 = "IMC30G-E board card library file close error! Result = {0}";
                    }
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionDeInit, CardIndex.ToString(), string.Format(str1, ret.ToString("x8")));

                }
                return false;
            }
        }

        /// <summary>
        /// 给予使能
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool ServoOn(int nAxisNo)
        {
            uint ret = ImcApi.IMC_AxServoOn(_hCardHandle, (short)nAxisNo);
            if (ret == ImcApi.EXE_SUCCESS)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionServoOn, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} servo on Error,result = {ret:x8}");

                }
                return false;
            }
        }

        /// <summary>
        /// 断开使能
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool ServoOff(int nAxisNo)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }
            else if (IsCrdMode(nAxisNo) == 1)
            {
                int nCrdNo;
                if (_dictCrdAxis.TryGetValue(nAxisNo, out nCrdNo))
                {
                    ImcApi.IMC_CrdStop(_hCardHandle, (short)nCrdNo, 1);

                    ImcApi.IMC_CrdClrData(_hCardHandle, (short)nCrdNo);

                    ImcApi.IMC_CrdDeleteMtSys(_hCardHandle, (short)nCrdNo);

                    _dictCrdAxis.Remove(nAxisNo);
                }

            }
            else if (IsImmediateMoveMode(nAxisNo) == 1)
            {
                ImcApi.IMC_ImmediateMoveEStop(_hCardHandle, (short)nAxisNo, 1000000.0);
            }

            uint ret = ImcApi.IMC_AxServoOff(_hCardHandle, (short)nAxisNo);
            if (ret == ImcApi.EXE_SUCCESS)
            {
                return true;
            }
            else
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionServoOff, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Axis {nAxisNo} servo off Error,result = {ret:x8}");

                }
                return false;
            }
        }

        /// <summary>
        /// 读取伺服使能状态
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool GetServoState(int nAxisNo)
        {
            int[] axStatus = new int[1];
            uint ret = ImcApi.IMC_GetAxSts(_hCardHandle, (short)nAxisNo, axStatus);
            if (ret == ImcApi.EXE_SUCCESS)
            {
                if ((axStatus[0] & (0x01 << 1)) != 0)
                    return true;
                else
                    return false;
            }
            else
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionState, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Axis {nAxisNo} get servo status Error,result = {ret:x8}");
                }
                return false;
            }
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nMode">回原点参数, 对于8254，此参数代表回原点的方向</param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nMode)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }

            switch ((HomeMode)nMode)
            {
                case HomeMode.OrgP:
                    nMode = 23;
                    break;

                case HomeMode.OrgN:
                    nMode = 27;
                    break;

                case HomeMode.Pel:
                    nMode = 18;
                    break;

                case HomeMode.Mel:
                    nMode = 17;
                    break;

                case HomeMode.OrgPEz:
                    nMode = 7;
                    break;

                case HomeMode.OrgNEz:
                    nMode = 11;
                    break;

                case HomeMode.PelEz:
                    nMode = 2;
                    break;

                case HomeMode.MelEz:
                    nMode = 1;
                    break;

                case HomeMode.EzPel:
                    nMode = 34;
                    break;

                case HomeMode.EzMel:
                    nMode = 33;
                    break;

                default:
                    if (nMode > (int)HomeMode.BusBase && nMode <= (int)HomeMode.BusBase + 35)
                    {
                        nMode -= (int)HomeMode.BusBase;
                    }
                    else
                    {
                        if (BEnable)
                        {
                            RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, GetSysAxisNo(nAxisNo).ToString(),
                                $"IMC30G-E Card Axis {nAxisNo} Home Mode Error");
                        }
                        return false;
                    }
                    break;
            }

            ImcApi.THomingPara homePara = _homePara;

            homePara.homeMethod = (short)nMode;

            uint ret = ImcApi.IMC_StartHoming(_hCardHandle, (short)nAxisNo, ref homePara);

            if (ret == ImcApi.EXE_SUCCESS)
            {
                Thread.Sleep(100);
                //short homingSts = 0;
                return true;
            }
            else
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Axis {nAxisNo} Home Error,result = {ret:x8}");
                }
                return false;
            }
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nMode"></param>
        /// <param name="vm"></param>
        /// <param name="vo"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="offset"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nMode, double vm, double vo, double acc, double dec, double offset = 0, double sFac = 0)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }

            switch ((HomeMode)nMode)
            {
                case HomeMode.OrgP:
                    nMode = 23;
                    break;

                case HomeMode.OrgN:
                    nMode = 27;
                    break;

                case HomeMode.Pel:
                    nMode = 18;
                    break;

                case HomeMode.Mel:
                    nMode = 17;
                    break;

                case HomeMode.OrgPEz:
                    nMode = 7;
                    break;

                case HomeMode.OrgNEz:
                    nMode = 11;
                    break;

                case HomeMode.PelEz:
                    nMode = 2;
                    break;

                case HomeMode.MelEz:
                    nMode = 1;
                    break;

                case HomeMode.EzPel:
                    nMode = 34;
                    break;

                case HomeMode.EzMel:
                    nMode = 33;
                    break;

                default:
                    if (nMode > (int)HomeMode.BusBase && nMode <= (int)HomeMode.BusBase + 35)
                    {
                        nMode -= (int)HomeMode.BusBase;
                    }
                    else
                    {
                        if (BEnable)
                        {
                            RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, GetSysAxisNo(nAxisNo).ToString(),
                                $"IMC30G-E Card Axis {nAxisNo} Home Mode Error");
                        }
                        return false;
                    }
                    break;
            }

            ImcApi.THomingPara homePara = _homePara;

            homePara.homeMethod = (short)nMode;
            homePara.acc = (uint)(vm / acc);
            homePara.highVel = (uint)vm;
            homePara.lowVel = (uint)vo;
            homePara.offset = (int)offset;

            uint ret = ImcApi.IMC_StartHoming(_hCardHandle, (short)nAxisNo, ref homePara);

            if (ret == ImcApi.EXE_SUCCESS)
            {
                Thread.Sleep(100);
                //short homingSts = 0;
                return true;
            }
            else
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Axis {nAxisNo} Home Error,result = {ret:x8}");
                }
                return false;
            }
        }

        /// <summary>
        /// 以绝对位置移动
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nPos">位置</param>
        /// <param name="nSpeed">速度</param>
        /// <returns></returns>
        public override bool AbsMove(int nAxisNo, int nPos, int nSpeed)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }
            // 读取速度
            double dVel = 0;
            double dAcc = 0;
            double dDec = 0;
            uint ret = ImcApi.IMC_GetSingleAxMvPara(_hCardHandle, (short)nAxisNo, ref dVel, ref dAcc, ref dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} get single axis moving parameter Error,result = {ret:x8}");
                }
                return false;
            }
            dAcc = nSpeed * 10;
            dDec = nSpeed * 10;
            // 设置速度
            ret = ImcApi.IMC_SetSingleAxMvPara(_hCardHandle, (short)nAxisNo, nSpeed, dAcc, dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} set single axis moving parameter Error,result = {ret:x8}");
                }
                return false;
            }

            // 执行运动
            short nPosType = 0;//运动模式0：表示绝对位置，1：表示相对位置
            ret = ImcApi.IMC_StartPtpMove(_hCardHandle, (short)nAxisNo, nPos, nPosType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} start PTP move Error,result = {ret:x8}");
                }
                return false;
            }
            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        /// 以绝对位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="fPos"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool AbsMove(int nAxisNo, double fPos, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }
            // 读取速度
            double dVel = vm;
            double dAcc = vm / acc;
            double dDec = vm / dec;
            // 设置速度
            uint ret = ImcApi.IMC_SetSingleAxMvPara(_hCardHandle, (short)nAxisNo, dVel, dAcc, dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} set single axis moving parameter Error,result = {ret:x8}");
                }
                return false;
            }

            //设置平滑曲线
            ret = sFac > 0 ? ImcApi.IMC_SetSingleAxVelType(_hCardHandle, (short)nAxisNo, 1, 100 * (1 - sFac)) : ImcApi.IMC_SetSingleAxVelType(_hCardHandle, (short)nAxisNo, 0, 0);

            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} set single axis vel type Error,result = {ret:x8}");
                }
                return false;
            }

            // 执行运动
            short nPosType = 0;//运动模式0：表示绝对位置，1：表示相对位置
            ret = ImcApi.IMC_StartPtpMove(_hCardHandle, (short)nAxisNo, fPos, nPosType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionAbs, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} start PTP move Error,result = {ret:x8}");
                }
                return false;
            }
            Thread.Sleep(100);
            return true;
        }
        /// <summary>
        /// 以当前位置为起始点进行多轴直线插补
        /// </summary>
        /// <param name="nAixsArray"></param>
        /// <param name="nPosArray"></param>
        /// <param name="vm"></param>
        /// <param name="acc">加速时间</param>
        /// <param name="dec">减速时间</param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool AbsLinearMove(ref int[] nAixsArray, ref double[] nPosArray, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {

            short[] pMaskAxNo = new short[nAixsArray.Length];

            for (int i = 0; i < nAixsArray.Length; i++)
            {
                pMaskAxNo[i] = (short)nAixsArray[i];
            }

            //加速时间转换为加速度
            double dVel = vm;
            double dAcc = vm / acc;
            double dDec = vm / dec;

            //平滑系数的取值范围【0~200】,而我们封装时用的8254取值范围【0~1】
            double dSmooth = sFac * 200;
            if (sFac > 1)
            {
                dSmooth = sFac;
            }

            uint ret = ImcApi.IMC_ImmediateLineMoveInSynVelAcc(_hCardHandle, pMaskAxNo, (short)(nAixsArray.Length), nPosArray, dVel, dAcc, dDec, dSmooth, 0);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "AbsLinearMove",
                    $"IMC30G-E Card Aixs {pMaskAxNo[0]} IMC_ImmediateLineMoveInSynVelAcc Error,result = {ret:x8}");
                return false;
            }

            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        /// 以当前位置为起始点进行多轴直线插补
        /// </summary>
        /// <param name="nAixsArray"></param>
        /// <param name="fPosOffsetArray"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool RelativeLinearMove(ref int[] nAixsArray, ref double[] fPosOffsetArray, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            short[] pMaskAxNo = new short[nAixsArray.Length];

            for (int i = 0; i < nAixsArray.Length; i++)
            {
                pMaskAxNo[i] = (short)nAixsArray[i];
            }

            //加速时间转换为加速度
            double dVel = vm;
            double dAcc = vm / acc;
            double dDec = vm / dec;

            //平滑系数的取值范围【0~200】,而我们封装时用的8254取值范围【0~1】
            double dSmooth = sFac * 200;
            if (sFac > 1)
            {
                dSmooth = sFac;
            }

            uint ret = ImcApi.IMC_ImmediateLineMoveInSynVelAcc(_hCardHandle, pMaskAxNo, (short)(nAixsArray.Length), fPosOffsetArray, dVel, dAcc, dDec, dSmooth, 1);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "RelativeLinearMove",
                    $"IMC30G-E Card Aixs {pMaskAxNo[0]} IMC_ImmediateLineMoveInSynVelAcc Error,result = {ret:x8}");
                return false;
            }

            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        /// 以当前点为起点做圆弧插补运动
        /// </summary>
        /// <param name="nAixsArray"></param>
        /// <param name="fCenterArray"></param>
        /// <param name="fEndArray"></param>
        /// <param name="dir"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool AbsArcMove(ref int[] nAixsArray, ref double[] fCenterArray, ref double[] fEndArray, int dir, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            short[] pMaskAxNo = new short[nAixsArray.Length];

            for (int i = 0; i < nAixsArray.Length; i++)
            {
                pMaskAxNo[i] = (short)nAixsArray[i];
            }

            //加速时间转换为加速度
            double dVel = vm;
            double dAcc = vm / acc;
            double dDec = vm / dec;

            //平滑系数的取值范围【0~200】,而我们封装时用的8254取值范围【0~1】
            double dSmooth = sFac * 200;
            if (sFac > 1)
            {
                dSmooth = sFac;
            }

            uint ret = ImcApi.IMC_ImmediateArcCenterMove(_hCardHandle, pMaskAxNo, (short)(nAixsArray.Length),
                fCenterArray, fEndArray, (short)dir, 0, 0, dVel, dAcc, dDec, dSmooth, 0);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "AbsArcMove",
                    $"IMC30G-E Card Aixs {pMaskAxNo[0]} IMC_ImmediateArcCenterMove Error,result = {ret:x8}");
                return false;
            }

            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        /// 以当前点为起点做相对圆弧插补运动
        /// </summary>
        /// <param name="nAixsArray"></param>
        /// <param name="fCenterOffsetArray"></param>
        /// <param name="fEndArray"></param>
        /// <param name="dir"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool RelativeArcMove(ref int[] nAixsArray, ref double[] fCenterOffsetArray, ref double[] fEndArray, int dir, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            short[] pMaskAxNo = new short[nAixsArray.Length];

            for (int i = 0; i < nAixsArray.Length; i++)
            {
                pMaskAxNo[i] = (short)nAixsArray[i];
            }

            //加速时间转换为加速度
            double dVel = vm;
            double dAcc = vm / acc;
            double dDec = vm / dec;

            //平滑系数的取值范围【0~200】,而我们封装时用的8254取值范围【0~1】
            double dSmooth = sFac * 200;
            if (sFac > 1)
            {
                dSmooth = sFac;
            }

            uint ret = ImcApi.IMC_ImmediateArcCenterMove(_hCardHandle, pMaskAxNo, (short)(nAixsArray.Length),
                fCenterOffsetArray, fEndArray, (short)dir, 0, 0, dVel, dAcc, dDec, dSmooth, 1);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "RelativeArcMove",
                    $"IMC30G-E Card Aixs {pMaskAxNo[0]} IMC_ImmediateArcCenterMove Error,result = {ret:x8}");
                return false;
            }

            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        /// 初始化多轴插补运动
        /// </summary>
        /// <param name="nMtSysNo"></param>
        /// <param name="nAixsArray"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        protected bool InitMultMove(int nMtSysNo, int[] nAixsArray, double vm, double acc)
        {
            //轴映射 将插补轴0,1,2号轴映射到单轴号0,1,2上
            short[] pMaskAxNo = new short[3];
            for (int i = 0; i < 3; i++)
            {
                if (nAixsArray.Length > i)
                {
                    pMaskAxNo[i] = (short)nAixsArray[i];
                    ServoOn(nAixsArray[i]);
                }
                else
                {
                    pMaskAxNo[i] = 31;
                }
            }

            //0号位建立插补坐标系，前瞻数3000段，5000.0的急停减加速度
            uint ret = ImcApi.IMC_CrdSetMtSys(_hCardHandle, (short)nMtSysNo, pMaskAxNo, 3000, 500000.0);
            if ((ret & 0xffff) == 0x0075)
            {
                _nMultipleInit = 1;
            }
            else if (ret == ImcApi.EXE_SUCCESS)
            {
                _nMultipleInit = 1;
            }
            else
            {
                _nMultipleInit = 0;
            }
            ImcApi.IMC_CrdSetTrajVel(_hCardHandle, (short)nMtSysNo, vm);
            ImcApi.IMC_CrdSetTrajAcc(_hCardHandle, (short)nMtSysNo, acc);//设置插补进给速度和加速度
            ImcApi.IMC_CrdSetZeroFlag(_hCardHandle, (short)nMtSysNo, 0);  //每段末速度不强制为0
            ImcApi.IMC_CrdSetIncMode(_hCardHandle, (short)nMtSysNo, 0); //插补编程方式为绝对编程方式
            ImcApi.TCrdAdvParam crdAdvParam = new ImcApi.TCrdAdvParam();
            crdAdvParam.userVelMode = 0;  //系统规划模式
            crdAdvParam.transMode = 1; //过渡模式
            crdAdvParam.noDataProtect = 0; //数据断流无保护
            crdAdvParam.noCoplaneCircOptm = 0; //异面过渡无处理
            crdAdvParam.turnCoef = 1.0; //拐角系数1.0
            crdAdvParam.tol = 0.1; //轨迹精度0.1 unit
            ret = ImcApi.IMC_CrdSetAdvParam(_hCardHandle, (short)nMtSysNo, ref crdAdvParam);  //设置插补高级参数
            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "AbsLinearMove",
                    $"IMC30G-E Card IMC_CrdSetAdvParam Error,result = {ret:x8}");

                return false;
            }

            return true;
        }

        /// <summary>
        /// 相对位置移动
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nPos">位置</param>
        /// <param name="nSpeed">速度</param>
        /// <returns></returns>
        public override bool RelativeMove(int nAxisNo, int nPos, int nSpeed)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }
            // 读取速度
            double dVel = 0;
            double dAcc = 0;
            double dDec = 0;
            uint ret = ImcApi.IMC_GetSingleAxMvPara(_hCardHandle, (short)nAxisNo, ref dVel, ref dAcc, ref dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} get single axis moving parameter Error,result = {ret:x8}");

                }
                return false;
            }
            dAcc = nSpeed * 10;
            dDec = nSpeed * 10;
            // 设置速度
            ret = ImcApi.IMC_SetSingleAxMvPara(_hCardHandle, (short)nAxisNo, nSpeed, dAcc, dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} set single axis moving parameter Error,result = {ret:x8}");

                }
                return false;
            }

            // 执行运动
            short nPosType = 1;//运动模式0：表示绝对位置，1：表示相对位置
            ret = ImcApi.IMC_StartPtpMove(_hCardHandle, (short)nAxisNo, nPos, nPosType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} start PTP move Error,result = {ret:x8}");
                }
                return false;
            }
            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        /// 相对位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="fOffset"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool RelativeMove(int nAxisNo, double fOffset, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }
            // 读取速度
            double dVel = vm;
            double dAcc = vm / acc;
            double dDec = vm / dec;

            // 设置速度
            uint ret = ImcApi.IMC_SetSingleAxMvPara(_hCardHandle, (short)nAxisNo, dVel, dAcc, dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} set single axis moving parameter Error,result = {ret:x8}");
                }
                return false;
            }

            // 执行运动
            short nPosType = 1;//运动模式0：表示绝对位置，1：表示相对位置
            ret = ImcApi.IMC_StartPtpMove(_hCardHandle, (short)nAxisNo, fOffset, nPosType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionRel, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} start PTP move Error,result = {ret:x8}");
                }
                return false;
            }
            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        /// JOG运动
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="bPositive">方向</param>
        /// <param name="bStart">开始标志</param>
        /// <param name="nSpeed">速度</param>
        /// <returns></returns>
        public override bool JogMove(int nAxisNo, bool bPositive, int bStart, int nSpeed)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }
            double tgVel = bPositive ? Math.Abs(nSpeed) : -Math.Abs(nSpeed);
            uint ret = ImcApi.IMC_StartJogMove(_hCardHandle, (short)nAxisNo, tgVel);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionJog, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} start jog Error,result = {ret:x8}");
                }
                return false;
            }
            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        /// 轴正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool StopAxis(int nAxisNo)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_StopHoming(_hCardHandle, (short)nAxisNo, 0);
                Thread.Sleep(100);
                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }
            else if (IsCrdMode(nAxisNo) == 1)
            {
                int nCrdNo;
                if (_dictCrdAxis.TryGetValue(nAxisNo, out nCrdNo))
                {
                    ImcApi.IMC_CrdStop(_hCardHandle, (short)nCrdNo, 0);

                    ImcApi.IMC_CrdClrData(_hCardHandle, (short)nCrdNo);

                    ImcApi.IMC_CrdDeleteMtSys(_hCardHandle, (short)nCrdNo);

                    _dictCrdAxis.Remove(nAxisNo);
                }
            }
            else if (IsImmediateMoveMode(nAxisNo) == 1)
            {
                ImcApi.IMC_ImmediateMoveStop(_hCardHandle, (short)nAxisNo);
            }

            short stopType = 0;//0:平滑停止, 1:急速停止
            uint ret = ImcApi.IMC_AxMoveStop(_hCardHandle, (short)nAxisNo, stopType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionStop, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} stop Error,result = {ret:x8}");
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// 急停
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool StopEmg(int nAxisNo)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_StopHoming(_hCardHandle, (short)nAxisNo, 1);
                Thread.Sleep(100);

                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }
            else if (IsCrdMode(nAxisNo) == 1)
            {
                int nCrdNo;
                if (_dictCrdAxis.TryGetValue(nAxisNo, out nCrdNo))
                {
                    ImcApi.IMC_CrdStop(_hCardHandle, (short)nCrdNo, 1);

                    ImcApi.IMC_CrdClrData(_hCardHandle, (short)nCrdNo);

                    ImcApi.IMC_CrdDeleteMtSys(_hCardHandle, (short)nCrdNo);

                    _dictCrdAxis.Remove(nAxisNo);
                }
            }
            else if (IsImmediateMoveMode(nAxisNo) == 1)
            {
                ImcApi.IMC_ImmediateMoveEStop(_hCardHandle, (short)nAxisNo, 1000000.0);
            }
            short stopType = 1;//0:平滑停止, 1:急速停止
            uint ret = ImcApi.IMC_AxMoveStop(_hCardHandle, (short)nAxisNo, stopType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionEmgStop, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} e-stop Error,result = {ret:x8}");
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取轴卡运动状态
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetMotionState(int nAxisNo)
        {
            //轴状态
            //Bit0:轴驱动报警
            //Bit1:伺服使能
            //Bit2:轴忙状态
            //Bit3:轴到位状态
            //Bit4:正硬限位报警
            //Bit5:负硬限位报警
            //Bit6:正软限位报警
            //Bit7:负软限位报警
            //Bit8:轴位置误差越线标志
            //Bit9:运动急停标志
            //Bit10:总线轴标志
            //Bit11:轴异常报警           
            int[] axStatus = new int[1];
            uint ret = ImcApi.IMC_GetAxSts(_hCardHandle, (short)nAxisNo, axStatus);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionState, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} Get Status Error,result = {ret:x8}");
                }
                return -1;
            }

            return axStatus[0];
        }

        /// <summary>
        /// 获取轴卡运动IO信号
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetMotionIoState(int nAxisNo)
        {
            //轴状态
            //Bit0:轴驱动报警
            //Bit1:伺服使能
            //Bit2:轴忙状态
            //Bit3:轴到位状态
            //Bit4:正硬限位报警
            //Bit5:负硬限位报警
            //Bit6:正软限位报警
            //Bit7:负软限位报警
            //Bit8:轴位置误差越线标志
            //Bit9:运动急停标志
            //Bit10:总线轴标志
            //Bit11:轴异常报警           
            int[] axStatus = new int[1];

            uint ret = ImcApi.IMC_GetAxSts(_hCardHandle, (short)nAxisNo, axStatus);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionState, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} Get Status Error,result = {ret:x8}");
                }
                return -1;
            }
            int mPDigitalInput = 0;
            ret = ImcApi.IMC_GetAxEcatDigitalInput(_hCardHandle, (short)nAxisNo, ref mPDigitalInput);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionState, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} Get Status Error,result = {ret:x8}");

                }
                return -1;
            }

            // 8254 motion io table
            // |-bit0-|--1--|--2--|--3--|--4--|--5--|--6--|--7--|--8--|...|--11--|--12--|
            // |-ALM--|-PEL-|-MEL-|-ORG-|-EMG-|-EZ--|-INP-|-SVO-|-RDY-|...|-SPEL-|-SMEL-|
            long nStdIo = 0;
            if ((axStatus[0] & (0x01 << 0)) != 0)
                nStdIo |= (0x01 << 0);
            if ((axStatus[0] & (0x01 << 1)) != 0)
                nStdIo |= (0x01 << 7);
            if ((axStatus[0] & (0x01 << 3)) != 0)
                nStdIo |= (0x01 << 6);
            if ((axStatus[0] & (0x01 << 6)) != 0)
                nStdIo |= (0x01 << 11);
            if ((axStatus[0] & (0x01 << 7)) != 0)
                nStdIo |= (0x01 << 12);
            if ((axStatus[0] & (0x01 << 9)) != 0)
                nStdIo |= (0x01 << 4);

            if ((mPDigitalInput & (0x01 << 0)) != 0)
                nStdIo |= (0x01 << 2);
            if ((mPDigitalInput & (0x01 << 1)) != 0)
                nStdIo |= (0x01 << 1);
            if ((mPDigitalInput & (0x01 << 2)) != 0)
                nStdIo |= (0x01 << 3);

            if ((((axStatus[0] & (0x01 << 4)) != 0) && ((mPDigitalInput & (0x01 << 1)) == 0))
                || (((axStatus[0] & (0x01 << 5)) != 0) && ((mPDigitalInput & (0x01 << 0)) == 0)))
            {
                ClearError(nAxisNo);
            }

            return nStdIo;
        }

        /// <summary>
        /// 清除错误报警
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool ClearError(int nAxisNo)
        {
            uint ret = ImcApi.IMC_ClrAxSts(_hCardHandle, (short)nAxisNo);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取轴的当前位置
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override double GetAixsPos(int nAxisNo)
        {
            double[] pPrfPos = new double[1];
            uint ret = ImcApi.IMC_GetAxEncPos(_hCardHandle, (short)nAxisNo, pPrfPos);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                return -1;
            }

            return (long)pPrfPos[0];
        }

        /// <summary>
        /// 轴是否正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns>0:正常停止, -1:未到位 其它:急停,报警等</returns>
        public override int IsAxisNormalStop(int nAxisNo)
        {
            //轴状态
            //Bit0:轴驱动报警
            //Bit1:伺服使能
            //Bit2:轴忙状态
            //Bit3:轴到位状态
            //Bit4:正硬限位报警
            //Bit5:负硬限位报警
            //Bit6:正软限位报警
            //Bit7:负软限位报警
            //Bit8:轴位置误差越线标志
            //Bit9:运动急停标志
            //Bit10:总线轴标志
            //Bit11:轴异常报警           
            int[] axStatus = new int[1];
            uint ret = ImcApi.IMC_GetAxSts(_hCardHandle, (short)nAxisNo, axStatus);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                return -1;
            }

            if ((axStatus[0] & (0x01 << 9)) != 0)//急停
            {
                Debug.WriteLine("Axis {0} have Emg signal \r\n", nAxisNo);
                return 1;
            }
            else if ((axStatus[0] & (0x01 << 0)) != 0)//报警
            {
                Debug.WriteLine("Axis {0} have sevo alarm signal \r\n", nAxisNo);
                return 2;
            }
            else if ((axStatus[0] & (0x01 << 1)) == 0)//Servo off
            {
                Debug.WriteLine("Axis {0} have servo signal \r\n", nAxisNo);
                return 3;
            }
            else if ((axStatus[0] & (0x01 << 4)) != 0)//正向硬限位 
            {
                Debug.WriteLine("Axis {0} have PEL signal \r\n", nAxisNo);
                return 4;
            }
            else if ((axStatus[0] & (0x01 << 5)) != 0)//负向硬限位 
            {
                Debug.WriteLine("Axis {0} have MEL signal \r\n", nAxisNo);
                return 5;
            }
            else if ((axStatus[0] & (0x01 << 6)) != 0)//正向软限位 
            {
                Debug.WriteLine("Axis {0} have SPEL signal \r\n", nAxisNo);
                return 4;
            }
            else if ((axStatus[0] & (0x01 << 7)) != 0)//负向软限位 
            {
                Debug.WriteLine("Axis {0} have SMEL signal \r\n", nAxisNo);
                return 5;
            }
            else if ((axStatus[0] & (0x01 << 8)) != 0)//轴位置误差越线标志 
            {
                Debug.WriteLine("Axis {0} position error is too large\r\n", nAxisNo);
                return 2;
            }
            else if ((axStatus[0] & (0x01 << 11)) != 0)//轴异常报警 
            {
                Debug.WriteLine("Axis {0} have axis abnormal alarm\r\n", nAxisNo);
                return 2;
            }
            else if ((axStatus[0] & (0x01 << 2)) != 0
                || (axStatus[0] & (0x01 << 3)) == 0)//未到位
            {
                return -1;//未完成
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 判断轴是否到位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nInPosError"></param>
        /// <returns></returns>
        public override int IsAxisInPos(int nAxisNo, int nInPosError = 1000)
        {
            if (IsCrdMode(nAxisNo) == 1)
            {
                return IsCrdStop(_dictCrdAxis[nAxisNo]);
            }

            int nRet = IsAxisNormalStop(nAxisNo);
            if (nRet == 0)
            {
                double[] pPrfPos = new double[1];
                double[] pEncPos = new double[1];
                uint ret = ImcApi.IMC_GetPrfPos(_hCardHandle, (short)nAxisNo, pPrfPos);
                if (ret != ImcApi.EXE_SUCCESS)
                    return -1;
                ret = ImcApi.IMC_GetAxEncPos(_hCardHandle, (short)nAxisNo, pEncPos);
                if (ret != 0)
                    return -1;

                if (Math.Abs(pPrfPos[0] - pEncPos[0]) > nInPosError)
                    return 6;  //轴停止后位置超限
            }
            return nRet;
        }

        /// <summary>
        /// 位置清零
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool SetPosZero(int nAxisNo)
        {
            uint ret = ImcApi.IMC_SetAxCurPos(_hCardHandle, (short)nAxisNo, 0);
            if (ret != ImcApi.EXE_SUCCESS)
                return false;

            return true;
        }


        /// <summary>
        /// 设置单轴的某一运动参数
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">参数:1:加速度 2:减速度 3:起跳速度 4:结束速度(凌华卡) 5:平滑时间(固高卡S曲线) 其它：自定义扩展</param>
        /// <param name="nData">参数值</param>
        /// <returns></returns>
        public override bool SetAxisParam(int nAxisNo, int nParam, int nData)
        {
            switch (nParam)
            {
                case 1:
                    _homePara.acc = (uint)nData;
                    break;
                case 2:
                    break;

                case 3:
                    _homePara.lowVel = (uint)nData;
                    break;

                case 4:
                    _homePara.highVel = (uint)nData;
                    break;

                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 速度模式旋转轴
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool VelocityMove(int nAxisNo, int nSpeed)
        {
            if (IsHomeMode(nAxisNo) > 0)
            {
                ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
            }
            uint ret = ImcApi.IMC_StartJogMove(_hCardHandle, (short)nAxisNo, nSpeed);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (BEnable)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionVel, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} start Velocity Move Error,result = {ret:x8}");
                }

                return false;
            }
            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        ///此函数8254板卡不提供不使用,回原点内部已经封装好过程处理 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsHomeNormalStop(int nAxisNo)
        {
            //2020-4-16 最新的dll(v1.5.5.0) 回原点成功后自动设置回原点完成，不需要手动设置
            //自动设置回原点完成后，不再处于回原点状态，因此不需要判断是否是回原点模式
            //发送回原点命令后需要延时，不能立即获取回原点状态。
            short nHomeStatus = 0;

            uint ret = ImcApi.IMC_GetHomingStatus(_hCardHandle, (short)nAxisNo, ref nHomeStatus);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, GetSysAxisNo(nAxisNo).ToString(),
                    $"IMC30G-E Card Aixs {nAxisNo} IMC_GetHomingStatus Error,result = {ret:x8}");
                return -1;
            }

            //回零状态，查询值对应的意义如下： 
            //(0)：正在回零中
            //(1)：回零中断或者没有开始启动
            //(2)：回零结束，但没有到设定的目标位置
            //(3)：回零成功
            //(4)：回零中发生错误，同时速度不为 0
            //(5)：回零中发生错误，同时速度为 0

            switch (nHomeStatus)
            {
                case 0:
                    return -1;

                case 1:
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} Home error -  interrupt or no start");
                    return -1;

                case 2:
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} Home error - not inpos");
                    return -1;

                case 3:
                    //最新的dll可以不调用，调用了也没关系
                    ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);
                    return 0;

                default:
                    //最新的dll可以不调用，调用了也没关系
                    ImcApi.IMC_FinishHoming(_hCardHandle, (short)nAxisNo);

                    RunInforManager.GetInstance().Error(ErrorType.ErrMotionHome, GetSysAxisNo(nAxisNo).ToString(),
                        $"IMC30G-E Card Aixs {nAxisNo} Home error");

                    return -1;

            }


        }
        /// <summary>
        /// 是否处于回原点模式
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns>1：处于回原点模式；其他：不是</returns>
        public int IsHomeMode(int nAxisNo)
        {
            short[] nctrlModel = new short[1];
            uint ret = ImcApi.IMC_GetAxPrfMode(_hCardHandle, (short)nAxisNo, nctrlModel);
            if (ret != ImcApi.EXE_SUCCESS)
            {

                return -2;
            }
            int mCtrlModel = nctrlModel[0] & 0x0f;
            if (mCtrlModel != 15)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 插补运动是否停止
        /// </summary>
        /// <param name="nMtSysNo">坐标系号</param>
        /// <returns>0：停止  -1：未完成</returns>
        public int IsMultMoveStop(int nMtSysNo)
        {
            short sts = 0;
            uint ret = ImcApi.IMC_CrdGetArrivalSts(_hCardHandle, (short)nMtSysNo, ref sts);

            if (ret == ImcApi.EXE_SUCCESS)
            {
                if (sts == 1)
                {
                    ImcApi.IMC_CrdStop(_hCardHandle, (short)nMtSysNo, 0);
                    ImcApi.IMC_CrdDeleteMtSys(_hCardHandle, (short)nMtSysNo);
                    _nMultipleInit = 0;
                    return 0;
                }
            }

            return -1;
        }

        /// <summary>
        /// 是否处于多轴插补运动模式
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns>1：多轴插补运动模式；其他：不是</returns>
        public int IsMultMoveMode(int nAxisNo)
        {
            short[] nctrlModel = new short[1];
            uint ret = ImcApi.IMC_GetAxPrfMode(_hCardHandle, (short)nAxisNo, nctrlModel);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                return -2;
            }
            int mCtrlModel = (nctrlModel[0] & 0x70) >> 4;
            if (mCtrlModel == 0x01)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 是否处于多轴插补运动模式
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public int IsImmediateMoveMode(int nAxisNo)
        {
            short[] nPrfModel = new short[1];
            uint ret = ImcApi.IMC_GetAxPrfMode(_hCardHandle, (short)nAxisNo, nPrfModel);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                return -2;
            }

            //当启动进入多轴插补立即运动时，所查询的轴模式值为12。 
            if (nPrfModel[0] == 12)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }


        /// <summary>
        /// 配置连续插补运动，最多只支持三轴
        /// </summary>
        /// <param name="crdNo">坐标系</param>
        /// <param name="nAixsArray">参与插补运动的轴</param>
        /// <param name="bAbsolute">true:绝对位置模式，　false:相对位置模式</param>
        /// <returns></returns>
        public override bool ConfigPointTable(int crdNo, ref int[] nAixsArray, bool bAbsolute)
        {
            //轴映射 将插补轴0,1,2号轴映射到单轴号0,1,2上
            short[] pMaskAxNo = new short[3];
            for (int i = 0; i < 3; i++)
            {
                if (nAixsArray.Length > i)
                {
                    pMaskAxNo[i] = (short)nAixsArray[i];
                    ServoOn(nAixsArray[i]);

                    if (_dictCrdAxis.ContainsKey(nAixsArray[i]))
                    {
                        _dictCrdAxis[nAixsArray[i]] = crdNo;
                    }
                    else
                    {
                        _dictCrdAxis.Add(nAixsArray[i], crdNo);
                    }
                }
                else
                {
                    pMaskAxNo[i] = 31;
                }
            }

            //插补坐标系已经建立,先删除
            ImcApi.IMC_CrdDeleteMtSys(_hCardHandle, (short)crdNo);

            //0号位建立插补坐标系，前瞻数3000段，5000.0的急停减加速度
            uint ret = ImcApi.IMC_CrdSetMtSys(_hCardHandle, (short)crdNo, pMaskAxNo, 3000, 500000.0);
            if ((ret & 0xffff) == 0x0075)
            {
                //插补坐标系已经建立,先删除
                ImcApi.IMC_CrdDeleteMtSys(_hCardHandle, (short)crdNo);

                Thread.Sleep(100);

                ret = ImcApi.IMC_CrdSetMtSys(_hCardHandle, (short)crdNo, pMaskAxNo, 3000, 500000.0);
            }

            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "ConfigPointTable",
                    $"IMC30G-E Card IMC_CrdSetMtSys Error,result = {ret:x8}");

                return false;
            }

            //清除错误
            ImcApi.IMC_CrdClrError(_hCardHandle, (short)crdNo);

            //清除缓存
            ImcApi.IMC_CrdClrData(_hCardHandle, (short)crdNo);

            if (bAbsolute)
            {
                ImcApi.IMC_CrdSetIncMode(_hCardHandle, (short)crdNo, 0); //插补编程方式为绝对编程方式
            }
            else
            {
                ImcApi.IMC_CrdSetIncMode(_hCardHandle, (short)crdNo, 1); //插补编程方式为绝对编程方式
            }


            ImcApi.TCrdAdvParam crdAdvParam = new ImcApi.TCrdAdvParam();
            crdAdvParam.userVelMode = 0;  //系统规划模式
            crdAdvParam.transMode = 1; //过渡模式
            crdAdvParam.noDataProtect = 0; //数据断流无保护
            crdAdvParam.noCoplaneCircOptm = 0; //异面过渡无处理
            crdAdvParam.turnCoef = 1.0; //拐角系数1.0
            crdAdvParam.tol = 0.1; //轨迹精度0.1 unit
            ret = ImcApi.IMC_CrdSetAdvParam(_hCardHandle, (short)crdNo, ref crdAdvParam);  //设置插补高级参数
            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "ConfigPointTable",
                    $"IMC30G-E Card IMC_CrdSetAdvParam Error,result = {ret:x8}");

                return false;
            }

            if (DicBoard.ContainsKey(crdNo))
            {
                DicBoard[crdNo] = nAixsArray.Length;
            }
            else
            {
                DicBoard.Add(crdNo, nAixsArray.Length);
            }


            return true;
        }

        /// <summary>
        /// 连续直线插补运动
        /// </summary>
        /// <param name="crdNo">坐标系</param>
        /// <param name="positionArray">运动点位</param>
        /// <param name="acc">加速时间，ms</param>
        /// <param name="dec">减速时间, ms</param>
        /// <param name="vs">开始速度</param>
        /// <param name="vm">运行速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sf">平滑系数</param>
        /// <returns></returns>
        public override bool PointTable_Line_Move(int crdNo, ref double[] positionArray, double acc, double dec, double vs, double vm, double ve, double sf)
        {
            //判断点位长度和参数插补运动的轴数是否相等
            if (DicBoard[crdNo] != positionArray.Length)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_Line_Move",
                    $"IMC30G-E Card MtSys {crdNo} Dimension error,PointTable_Line_Move");
                return false;
            }

            //设置速度，加速度

            //通过加减速时间计算加减速度
            acc = vm / acc;
            dec = vm / dec;

            //设置运行速度
            ImcApi.IMC_CrdSetTrajVel(_hCardHandle, (short)crdNo, vm);

            //设置加速度和减速度
            ImcApi.IMC_CrdSetTrajAccAndDec(_hCardHandle, (short)crdNo, acc, dec);

            //设置开始速度,结束速度
            ImcApi.IMC_CrdUserVelPlan(_hCardHandle, (short)crdNo, vs, ve);

            //设置平滑系数
            ImcApi.IMC_CrdSetSmoothParam(_hCardHandle, (short)crdNo, (int)sf, sf - (int)sf);

            uint ret;

            //插入运动点位
            if (DicBoard[crdNo] == 3)
            {
                //三轴插补
                ret = ImcApi.IMC_CrdLineXYZ(_hCardHandle, (short)crdNo, positionArray);

                if (ret != ImcApi.EXE_SUCCESS)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_Line_Move",
                        $"IMC30G-E Card IMC_CrdLineXYZ Error,result = {ret:x8}");

                    return false;
                }
            }
            else
            {
                //二轴插补，默认为XY
                ret = ImcApi.IMC_CrdLineXY(_hCardHandle, (short)crdNo, positionArray);

                if (ret != ImcApi.EXE_SUCCESS)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_Line_Move",
                        $"IMC30G-E Card IMC_CrdLineXY Error,result = {ret:x8}");

                    return false;
                }
            }

            //把数据压入队列
            short isFinished = new short();
            ret = ImcApi.IMC_CrdEndData(_hCardHandle, (short)crdNo, ref isFinished);  //把PC FIFO中的线段送入板卡FIFO中
            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_Line_Move",
                    $"IMC30G-E Card IMC_CrdEndData Error,result = {ret:x8}");

                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crdNo"></param>
        /// <param name="centerArray"></param>
        /// <param name="endArray"></param>
        /// <param name="dir"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="vm"></param>
        /// <param name="ve"></param>
        /// <param name="sf"></param>
        /// <returns></returns>
        public override bool PointTable_ArcE_Move(int crdNo, ref double[] centerArray, ref double[] endArray, short dir, double acc, double dec, double vs, double vm, double ve, double sf)
        {
            //判断点位长度和参数插补运动的轴数是否相等
            if (DicBoard[crdNo] != centerArray.Length || DicBoard[crdNo] != 2)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_ArcE_Move",
                    $"IMC30G-E Card MtSys {crdNo} Dimension error,PointTable_Line_Move");
                return false;
            }

            //设置速度，加速度

            //通过加减速时间计算加减速度
            acc = vm / acc;
            dec = vm / dec;

            //设置运行速度
            ImcApi.IMC_CrdSetTrajVel(_hCardHandle, (short)crdNo, vm);

            //设置加速度和减速度
            ImcApi.IMC_CrdSetTrajAccAndDec(_hCardHandle, (short)crdNo, acc, dec);

            //设置开始速度,结束速度
            ImcApi.IMC_CrdUserVelPlan(_hCardHandle, (short)crdNo, vs, ve);

            //设置平滑系数
            ImcApi.IMC_CrdSetSmoothParam(_hCardHandle, (short)crdNo, (int)sf, sf - (int)sf);

            //XY平面内圆心末点编程。Z轴不作圆周运动
            uint ret = ImcApi.IMC_CrdArcCenterXYPlane(_hCardHandle, (short)crdNo, centerArray, endArray, dir);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_ArcE_Move",
                    $"IMC30G-E Card IMC_CrdArcCenterXYPlane Error,result = {ret:x8}");

                return false;
            }

            //把数据压入队列
            short isFinished = new short();
            ret = ImcApi.IMC_CrdEndData(_hCardHandle, (short)crdNo, ref isFinished);  //把PC FIFO中的线段送入板卡FIFO中
            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_ArcE_Move",
                    $"IMC30G-E Card IMC_CrdEndData Error,result = {ret:x8}");

                return false;
            }

            return true;
        }

        /// <summary>
        /// 插补缓冲区输出DO,在插入位置之后插入
        /// </summary>
        /// <param name="crdNo">坐标系号</param>
        /// <param name="nChannel">输出DO的端口号</param>
        /// <param name="bOn">输出DO的电平，0低电平，1 高电平</param>
        /// <returns></returns>
        public override bool PointTable_IO(int crdNo, int nChannel, int bOn)
        {
            uint ret = ImcApi.IMC_CrdSetDO(_hCardHandle, (short)crdNo, (short)nChannel, 0, (short)bOn);

            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_IO",
                    $"IMC30G-E Card IMC_CrdSetDO Error,result = {ret:x8}");

                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="crdNo"></param>
        /// <param name="nMillsecond"></param>
        /// <returns></returns>
        public override bool PointTable_Delay(int crdNo, int nMillsecond)
        {
            uint ret = ImcApi.IMC_CrdWaitTime(_hCardHandle, (short)crdNo, nMillsecond);

            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_Delay",
                    $"IMC30G-E Card IMC_CrdWaitTime Error,result = {ret:x8}");

                return false;
            }

            return true;
        }

        /// <summary>
        /// 确定连续运动列表的BUFF是否已满
        /// </summary>
        /// <param name="crdNo"></param>
        /// <returns></returns>
        public override bool PointTable_IsIdle(int crdNo)
        {
            int nSpace = 0;
            uint ret = ImcApi.IMC_CrdGetSpace(_hCardHandle, (short)crdNo, ref nSpace);

            if (ret != ImcApi.EXE_SUCCESS)
            {
                RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_IsIdle",
                    $"IMC30G-E Card IMC_CrdGetSpace Error,result = {ret:x8}");

                return false;
            }

            return nSpace > 0;
        }

        /// <summary>
        /// 启动或停止一个连续运动
        /// </summary>
        /// <param name="crdNo"></param>
        /// <param name="bStart"></param>
        /// <returns></returns>
        public override bool PointTable_Start(int crdNo, bool bStart)
        {
            if (bStart)
            {
                var ret = ImcApi.IMC_CrdStart(_hCardHandle, (short)crdNo);

                if (ret != ImcApi.EXE_SUCCESS)
                {
                    RunInforManager.GetInstance().Error(ErrorType.ErrMotion, "PointTable_Start",
                        $"IMC30G-E Card IMC_CrdStart Error,result = {ret:x8}");

                    return false;
                }
            }
            else
            {
                ImcApi.IMC_CrdStop(_hCardHandle, (short)crdNo, 0);

                ImcApi.IMC_CrdClrData(_hCardHandle, (short)crdNo);

                ImcApi.IMC_CrdDeleteMtSys(_hCardHandle, (short)crdNo);
            }

            Thread.Sleep(100);
            return true;
        }

        /// <summary>
        /// 插补运动是否停止
        /// </summary>
        /// <param name="crdNo"></param>
        /// <returns>0 - 表示停止 -1 - 未停止</returns>
        private int IsCrdStop(int crdNo)
        {
            short sts = 0;
            //获取插补运动执行缓冲区最后一段的运动状态
            uint ret = ImcApi.IMC_CrdGetArrivalSts(_hCardHandle, (short)crdNo, ref sts);

            if (ret == ImcApi.EXE_SUCCESS)
            {
                if (sts == 1)
                {
                    ImcApi.IMC_CrdStop(_hCardHandle, (short)crdNo, 0);
                    ImcApi.IMC_CrdDeleteMtSys(_hCardHandle, (short)crdNo);
                    return 0;
                }
            }

            return -1;
        }

        /// <summary>
        /// 是否处于多轴插补运动模式
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns>1：多轴插补运动模式；其他：不是</returns>
        private int IsCrdMode(int nAxisNo)
        {
            short[] nPrfModel = new short[1];
            //获取轴规划模式 当该值为-1 时，表示不处于任何规划模式。
            //Bit[15:6]    Bit[6:4]                                         Bit[3:0]    
            //无意义       多轴规划模式                                     单轴规划模式
            //             0X01 : PROFILE_MODE_CRD（插补模式）              0X01 : PROFILE_MODE_PTP（点位模式） 
            //             0X02 : PROFILE_MODE_MULTI_SYNC（多轴同步）       0X02 : PROFILE_MODE_JOG（JOG 模式） 
            //             0x04：PROFILE_MODE_SGL_BANDPT（捆绑 PT模式）     0X03 : PROFILE_MODE_GEAR（电子齿轮模式） 
            //                                                              0X04 : PROFILE_MODE_CAM（电子凸轮模式）
            //                                                              0X05 : PROFILE_MODE_PVT（PVT 模式） 
            //                                                              0X06 : PROFILE_MODE_GANTRY（龙门模式） 
            //                                                              0X07 : PROFILE_MODE_HANDWHEEL（手轮模式）
            //                                                              0X09: PROFILE_MODE_PTPC（点位连续模式）
            //                                                              0X0b: PROFILE_MODE_CRD_SYNC(插补同步轴模式)
            //                                                              0X0f: PROFILE_MODE_HOMING(回零模式)
            uint ret = ImcApi.IMC_GetAxPrfMode(_hCardHandle, (short)nAxisNo, nPrfModel);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                return -2;
            }

            //PROFILE_MODE_CRD（插补模式）
            if ((nPrfModel[0] & 0x10) != 0)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 启用软件正限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public override bool SetSpelEnable(int nAxisNo, bool bEnable)
        {
            uint ret;
            if (bEnable)
            {
                ret = ImcApi.IMC_AxSoftLmtsEnable(_hCardHandle, (short)nAxisNo);
            }
            else
            {
                ret = ImcApi.IMC_AxSoftLmtsDisable(_hCardHandle, (short)nAxisNo);
            }

            return ret == ImcApi.EXE_SUCCESS;
        }

        /// <summary>
        /// 启用软件负限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public override bool SetSmelEnable(int nAxisNo, bool bEnable)
        {
            uint ret;
            if (bEnable)
            {
                ret = ImcApi.IMC_AxSoftLmtsEnable(_hCardHandle, (short)nAxisNo);
            }
            else
            {
                ret = ImcApi.IMC_AxSoftLmtsDisable(_hCardHandle, (short)nAxisNo);
            }

            return ret == ImcApi.EXE_SUCCESS;
        }

        /// <summary>
        /// 设置软件正限位位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override bool SetSpelPos(int nAxisNo, double pos)
        {
            int nSoftPosLimit = 0, nSoftNegLimit = 0;
            ImcApi.IMC_GetAxSoftLimit(_hCardHandle, (short)nAxisNo, ref nSoftPosLimit, ref nSoftNegLimit);

            uint ret = ImcApi.IMC_SetAxSoftLimit(_hCardHandle, (short)nAxisNo, (int)pos, nSoftNegLimit);

            return ret == ImcApi.EXE_SUCCESS;
        }

        /// <summary>
        /// 设置软件负限位位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override bool SetSmelPos(int nAxisNo, double pos)
        {
            int nSoftPosLimit = 0, nSoftNegLimit = 0;
            ImcApi.IMC_GetAxSoftLimit(_hCardHandle, (short)nAxisNo, ref nSoftPosLimit, ref nSoftNegLimit);

            uint ret = ImcApi.IMC_SetAxSoftLimit(_hCardHandle, (short)nAxisNo, nSoftPosLimit, (int)pos);

            return ret == ImcApi.EXE_SUCCESS;
        }
    }
}
