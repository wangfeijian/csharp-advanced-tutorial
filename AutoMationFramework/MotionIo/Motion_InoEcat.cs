/********************************************************************
	created:	2018/09/12
	filename: 	Motion_InoEcat
	file ext:	cs
	author:		gxf
	purpose:	汇川EtherCAT运动控制卡的封装类
*********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Inovance.InoMotionCotrollerShop.InoServiceContract.EtherCATConfigApi;
using System.Collections.Generic;

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
        public UInt64 m_hCardHandle = ImcApi.ERR_HANDLE;

        /// <summary>
        /// 卡内资源
        /// </summary>
        public ImcApi.TRsouresNum m_tResoures = new ImcApi.TRsouresNum();

        private static InoEtherCatCard instance = null;

        private static readonly object m_lock = new object();

        private bool m_bInited = false;


        /// <summary>
        /// 是否初始化
        /// </summary>
        public bool IsInited
        {
            get
            {
                return m_bInited;
            }
        }

        /// <summary>
        /// 实例
        /// </summary>
        /// <returns></returns>
        public static InoEtherCatCard Instance()
        {
            if (instance == null)
            {
                lock (m_lock)
                {
                    if (instance == null)
                    {
                        instance = new InoEtherCatCard();
                    }
                }

            }

            return instance;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private InoEtherCatCard()
        {
            m_bInited = Init();
        }

        /// <summary>
        /// 板卡初始化
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            //TRACE("init card\r\n");
            //string str1 = "控制卡IMC30G-E读取卡数量失败, result = {0}";
            //string str2 = "控制卡IMC30G-E不存在。";
            //string str3 = "控制卡IMC30G-E打开失败, result = {0}";
            //string str4 = "控制卡IMC30G-E下载设备文件失败, result = {0}";
            //string str5 = "获取主站状态失败,result = {0}";
            //string str6 = "启动EtherCAT失败,result = {0}";
            //string str7 = "控制卡IMC30G-E下载参数文件失败, result = {0}";
            //string str8 = "控制卡IMC30G-E扫描卡内资源失败, result = {0}";
            //string str9 = "控制卡IMC30G-E初始化失败";
            //if (LanguageMgr.GetInstance().LanguageID != 0)
            //{
            //    str1 = "Failed to read the number of IMC30G-E control cards, result = {0}";
            //    str2 = "The control card IMC30G-E does not exist. ";
            //    str3 = "Failed to open control card IMC30G-E, result = {0}";
            //    str4 = "IMC30G-E of control card failed to download device file, result = {0}";
            //    str5 = "Failed to get master status, result = {0}";
            //    str6 = "Failed to start EtherCAT, result = {0}";
            //    str7 = "IMC30G-E of control card failed to download parameter file, result = {0}";
            //    str8 = "Control card IMC30G-E failed to scan resources in the card, result = {0}";
            //    str9 = "Control card IMC30G-E initialization failed";
            //}
            try
            {
                //【1】获取卡数量
                int nCardNum = 0;
                uint ret = ImcApi.IMC_GetCardsNum(ref nCardNum);
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,控制卡IMC30G-E读取卡数量失败, result = {0}", ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, "InoEcat",
                    //    string.Format(str1, ret.ToString("x8")));

                    return false;
                }
                else
                {
                    if (nCardNum <= 0)
                    {
                        //WarningMgr.GetInstance().Error(string.Format("30102,ERR-XYT,控制卡IMC30G-E不存在。"));
                        //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, "InoEcat",
                        //    string.Format(str2));

                        return false;
                    }
                }

                //【2】打开卡句柄，打开第一张卡
                ret = ImcApi.IMC_OpenCardHandle(0, ref m_hCardHandle);
                Thread.Sleep(200);
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    m_hCardHandle = ImcApi.ERR_HANDLE;
                    //WarningMgr.GetInstance().Error(string.Format("30103,ERR-XYT,控制卡IMC30G-E打开失败, result = {0}", ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, "InoEcat",
                    //    string.Format(str3, ret.ToString("x8")));

                    return false;
                }

                //【3】下载设备参数
                ret = ImcApi.IMC_DownLoadDeviceConfig(m_hCardHandle, "device_config.xml");
                Thread.Sleep(2000);
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30104,ERR-XYT,控制卡IMC30G-E下载设备文件失败, result = {0}", ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, "InoEcat",
                    //    string.Format(str4, ret.ToString("x8")));

                    return false;
                }

                //【4】启动主站
                //ret = ImcApi.IMC_ScanCardECAT(m_hCardHandle, 1);      //默认阻塞式启动EtherCAT
                //if (ret != ImcApi.EXE_SUCCESS)
                //{
                //    WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,控制卡IMC30G-E启动主站失败, result = {0}", ret.ToString("x8")));
                //    return false;
                //}

                //lin_
                uint masterStatus = 0;
                ret = ImcApi.IMC_GetECATMasterSts(m_hCardHandle, ref masterStatus);
                if (ret != 0)
                {
                    //WarningMgr.GetInstance().Error("获取主站状态失败,错误代码为0x" + ret.ToString("x8"));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, "InoEcat",
                    //    string.Format(str5, ret.ToString("x8")));

                    return false;
                }
                else
                {
                    if (masterStatus != ImcApi.EC_MASTER_OP)
                    {
                        ret = ImcApi.IMC_ScanCardECAT(m_hCardHandle, 1);      //默认阻塞式启动EtherCAT
                        if (ret != 0)
                        {
                            //WarningMgr.GetInstance().Error("启动EtherCAT失败,错误代码为0x" + ret.ToString("x8"));
                            //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, "InoEcat",
                            //    string.Format(str6, ret.ToString("x8")));

                            return false;
                        }
                    }
                }

                //【5】扫描卡内资源
                ret = ImcApi.IMC_GetCardResource(m_hCardHandle, ref m_tResoures);
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,控制卡IMC30G-E扫描卡内资源失败, result = {0}", ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, "InoEcat",
                    //    string.Format(str8, ret.ToString("x8")));

                    return false;
                }

                //【6】所有轴断使能
                ret = ImcApi.IMC_AxServoOff(m_hCardHandle, 0, m_tResoures.axNum);


                //【7】下载系统参数
                ret = ImcApi.IMC_DownLoadSystemConfig(m_hCardHandle, "system_config.xml");
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,控制卡IMC30G-E下载参数文件失败, result = {0}", ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, "InoEcat",
                    //    string.Format(str7, ret.ToString("x8")));

                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.Message, str9, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }

    /// <summary>
    /// 汇川EtherCAT运动控制卡封装,类名必须以"Motion_"前导，否则加载不到
    /// </summary>
    public class Motion_InoEcat : Motion
    {
        /// <summary>
        /// 控制卡句柄
        /// </summary>
        private UInt64 m_hCardHandle = ImcApi.ERR_HANDLE;
        private int m_nMultInit = 0;

        private Dictionary<int, int> m_dictCrdAxis = new Dictionary<int, int>();

        /// <summary>
        /// 回原点参数
        /// </summary>
        private ImcApi.THomingPara m_homePara = new ImcApi.THomingPara();

        //todo:板卡类应该只初始化一次
        /// <summary>构造函数
        /// 
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
        public Motion_InoEcat(int nCardIndex, string strName, int nMinAxisNo, int nMaxAxisNo)
            : base(nCardIndex, strName, nMinAxisNo, nMaxAxisNo)
        {
            m_bEnable = false;

            m_homePara.homeMethod = 0;
            m_homePara.offset = 0;
            m_homePara.highVel = 10000;
            m_homePara.lowVel = 1000;
            m_homePara.acc = 50000;
            m_homePara.overtime = 10000;
        }
        /// <summary>
        /// 轴卡初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            try
            {
                m_hCardHandle = InoEtherCatCard.Instance().m_hCardHandle;
                if (m_hCardHandle != ImcApi.ERR_HANDLE)
                {
                    m_bEnable = true;
                    return true;
                }
                else
                {
                    m_bEnable = false;
                    return false;
                }
            }
            catch
            {
                m_bEnable = false;
                return false;
            }
        }

        /// <summary>
        /// 关闭轴卡
        /// </summary>
        /// <returns></returns>
        public override bool DeInit()
        {
            m_nMultInit = 0;
            uint ret = ImcApi.IMC_CloseCardHandle(m_hCardHandle);
            if (ret == ImcApi.EXE_SUCCESS)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //string str1 = "IMC30G-E板卡库文件关闭出错! result = {0}";
                   // if (LanguageMgr.GetInstance().LanguageID != 0)
                    {
                      //  str1 = "IMC30G-E board card library file close error! Result = {0}";
                    }
                    //WarningMgr.GetInstance().Error(string.Format("30108,ERR-XYT,IMC30G-E板卡库文件关闭出错! result = {0}", ret.ToString("x8")));
                   // WarningMgr.GetInstance().Error(ErrorType.Err_Motion_DeInit, m_nCardIndex.ToString(),
                     //   string.Format(str1, ret.ToString("x8")));

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
            uint ret = ImcApi.IMC_AxServoOn(m_hCardHandle, (short)nAxisNo, 1);
            if (ret == ImcApi.EXE_SUCCESS)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30109,ERR-XYT,IMC30G-E Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret.ToString("x8")));
                   // WarningMgr.GetInstance().Error(ErrorType.Err_Motion_ServoOn, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }
            else if (IsCrdMode(nAxisNo) == 1)
            {
                int nCrdNo;
                if (m_dictCrdAxis.TryGetValue(nAxisNo, out nCrdNo))
                {
                    ImcApi.IMC_CrdStop(m_hCardHandle, (short)nCrdNo, 1);

                    ImcApi.IMC_CrdClrData(m_hCardHandle, (short)nCrdNo);

                    ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)nCrdNo);

                    m_dictCrdAxis.Remove(nAxisNo);
                }

            }
            else if (IsImmediateMoveMode(nAxisNo) == 1)
            {
                ImcApi.IMC_ImmediateMoveEStop(m_hCardHandle, (short)nAxisNo, 1000000.0);
            }

            //if (IsMultMoveMode(nAxisNo) > 0)
            //{
            //    ImcApi.IMC_CrdStop(m_hCardHandle, 0, 1);
            //    ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)0);
            //    m_nMultInit = 0;
            //}

            uint ret = ImcApi.IMC_AxServoOff(m_hCardHandle, (short)nAxisNo, 1);
            if (ret == ImcApi.EXE_SUCCESS)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30110,ERR-XYT,IMC30G-E Card Axis {0} servo off Error,result = {1}", nAxisNo, ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_ServoOff, GetSysAxisNo(nAxisNo).ToString(),
                    //    string.Format("IMC30G-E Card Axis {0} servo off Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
            uint ret = ImcApi.IMC_GetAxSts(m_hCardHandle, (short)nAxisNo, axStatus, 1);
            if (ret == ImcApi.EXE_SUCCESS)
            {
                if ((axStatus[0] & (0x01 << 1)) != 0)
                    return true;
                else
                    return false;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30111,ERR-XYT,IMC30G-E Card Axis {0} get servo status Error,result = {1}", nAxisNo, ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_State, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Axis {0} get servo status Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }

            switch ((HomeMode)nMode)
            {
                case HomeMode.ORG_P:
                    nMode = 23;
                    break;

                case HomeMode.ORG_N:
                    nMode = 27;
                    break;

                case HomeMode.PEL:
                    nMode = 18;
                    break;

                case HomeMode.MEL:
                    nMode = 17;
                    break;

                case HomeMode.ORG_P_EZ:
                    nMode = 7;
                    break;

                case HomeMode.ORG_N_EZ:
                    nMode = 11;
                    break;

                case HomeMode.PEL_EZ:
                    nMode = 2;
                    break;

                case HomeMode.MEL_EZ:
                    nMode = 1;
                    break;

                case HomeMode.EZ_PEL:
                    nMode = 34;
                    break;

                case HomeMode.EZ_MEL:
                    nMode = 33;
                    break;

                default:
                    if (nMode > (int)HomeMode.BUS_BASE && nMode <= (int)HomeMode.BUS_BASE + 35)
                    {
                        nMode -= (int)HomeMode.BUS_BASE;
                    }
                    else
                    {
                        if (m_bEnable)
                        {
                            //WarningMgr.GetInstance().Error(string.Format("30112,ERR-XYT,IMC30G-E Card Axis {0} Home Mode Error", nAxisNo));
                          //  WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                           //     string.Format("IMC30G-E Card Axis {0} Home Mode Error", nAxisNo));

                        }
                        return false;
                    }
                    break;
            }

            ImcApi.THomingPara homePara = m_homePara;

            homePara.homeMethod = (short)nMode;

            uint ret = ImcApi.IMC_StartHoming(m_hCardHandle, (short)nAxisNo, ref homePara);

            if (ret == ImcApi.EXE_SUCCESS)
            {
                Thread.Sleep(100);
                //short homingSts = 0;
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30112,ERR-XYT,IMC30G-E Card Axis {0} Home Error,result = {1}", nAxisNo, ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Axis {0} Home Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }

            switch ((HomeMode)nMode)
            {
                case HomeMode.ORG_P:
                    nMode = 23;
                    break;

                case HomeMode.ORG_N:
                    nMode = 27;
                    break;

                case HomeMode.PEL:
                    nMode = 18;
                    break;

                case HomeMode.MEL:
                    nMode = 17;
                    break;

                case HomeMode.ORG_P_EZ:
                    nMode = 7;
                    break;

                case HomeMode.ORG_N_EZ:
                    nMode = 11;
                    break;

                case HomeMode.PEL_EZ:
                    nMode = 2;
                    break;

                case HomeMode.MEL_EZ:
                    nMode = 1;
                    break;

                case HomeMode.EZ_PEL:
                    nMode = 34;
                    break;

                case HomeMode.EZ_MEL:
                    nMode = 33;
                    break;

                default:
                    if (nMode > (int)HomeMode.BUS_BASE && nMode <= (int)HomeMode.BUS_BASE + 35)
                    {
                        nMode -= (int)HomeMode.BUS_BASE;
                    }
                    else
                    {
                        if (m_bEnable)
                        {
                            //WarningMgr.GetInstance().Error(string.Format("30112,ERR-XYT,IMC30G-E Card Axis {0} Home Mode Error", nAxisNo));
                          //  WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                            //    string.Format("IMC30G-E Card Axis {0} Home Mode Error", nAxisNo));

                        }
                        return false;
                    }
                    break;
            }

            ImcApi.THomingPara homePara = m_homePara;

            homePara.homeMethod = (short)nMode;
            homePara.acc = (uint)(vm / acc);
            homePara.highVel = (uint)vm;
            homePara.lowVel = (uint)vo;
            homePara.offset = (int)offset;

            uint ret = ImcApi.IMC_StartHoming(m_hCardHandle, (short)nAxisNo, ref homePara);

            if (ret == ImcApi.EXE_SUCCESS)
            {
                Thread.Sleep(100);
                //short homingSts = 0;
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30112,ERR-XYT,IMC30G-E Card Axis {0} Home Error,result = {1}", nAxisNo, ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Axis {0} Home Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }
            // 读取速度
            double dVel = 0;
            double dAcc = 0;
            double dDec = 0;
            uint ret = ImcApi.IMC_GetSingleAxMvPara(m_hCardHandle, (short)nAxisNo, ref dVel, ref dAcc, ref dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30113,ERR-XYT,IMC30G-E Card Aixs {0} get single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs, GetSysAxisNo(nAxisNo).ToString(),
                      //  string.Format("IMC30G-E Card Aixs {0} get single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));

                }
                return false;
            }
            dAcc = nSpeed * 10;
            dDec = nSpeed * 10;
            // 设置速度
            ret = ImcApi.IMC_SetSingleAxMvPara(m_hCardHandle, (short)nAxisNo, nSpeed, dAcc, dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30114,ERR-XYT,IMC30G-E Card Aixs {0} set single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));
                  //  WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs, GetSysAxisNo(nAxisNo).ToString(),
                      //  string.Format("IMC30G-E Card Aixs {0} set single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));

                }
                return false;
            }

            // 执行运动
            short nPosType = 0;//运动模式0：表示绝对位置，1：表示相对位置
            ret = ImcApi.IMC_StartPtpMove(m_hCardHandle, (short)nAxisNo, nPos, nPosType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30115,ERR-XYT,IMC30G-E Card Aixs {0} start PTP move Error,result = {1}", nAxisNo, ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs, GetSysAxisNo(nAxisNo).ToString(),
                      //  string.Format("IMC30G-E Card Aixs {0} start PTP move Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }
            // 读取速度
            double dVel = vm;
            double dAcc = vm / acc;
            double dDec = vm / dec;
            // 设置速度
            uint ret = ImcApi.IMC_SetSingleAxMvPara(m_hCardHandle, (short)nAxisNo, dVel, dAcc, dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30114,ERR-XYT,IMC30G-E Card Aixs {0} set single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs, GetSysAxisNo(nAxisNo).ToString(),
                       // string.Format("IMC30G-E Card Aixs {0} set single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));

                }
                return false;
            }

            //设置平滑曲线
            if (sFac > 0)
            {
                ret = ImcApi.IMC_SetSingleAxVelType(m_hCardHandle, (short)nAxisNo, 1, 100 * (1 - sFac));
            }
            else
            {
                ret = ImcApi.IMC_SetSingleAxVelType(m_hCardHandle, (short)nAxisNo, 0, 0);
            }

            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30114,ERR-XYT,IMC30G-E Card Aixs {0} set single axis vel type Error,result = {1}", nAxisNo, ret.ToString("x8")));
                  //  WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Aixs {0} set single axis vel type Error,result = {1}", nAxisNo, ret.ToString("x8")));

                }
                return false;
            }

            // 执行运动
            short nPosType = 0;//运动模式0：表示绝对位置，1：表示相对位置
            ret = ImcApi.IMC_StartPtpMove(m_hCardHandle, (short)nAxisNo, fPos, nPosType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30115,ERR-XYT,IMC30G-E Card Aixs {0} start PTP move Error,result = {1}", nAxisNo, ret.ToString("x8")));
                  //  WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs, GetSysAxisNo(nAxisNo).ToString(),
                      //  string.Format("IMC30G-E Card Aixs {0} start PTP move Error,result = {1}", nAxisNo, ret.ToString("x8")));

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

            //            UINT32 IMC_ImmediateLineMoveInSynVelAcc(UINT64 cardHandle, INT16* pMaskAxNo, INT16
            //axNum, double* pEndPos, double trajVel, double trajAcc, double trajDec, double smoothCoef, INT16
            //type);
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

            uint ret = ImcApi.IMC_ImmediateLineMoveInSynVelAcc(m_hCardHandle, pMaskAxNo, (short)(nAixsArray.Length), nPosArray, dVel, dAcc, dDec, dSmooth, 0);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsLinearMove",
                   // string.Format("IMC30G-E Card Aixs {0} IMC_ImmediateLineMoveInSynVelAcc Error,result = {1}", pMaskAxNo[0], ret.ToString("x8")));
                return false;
            }

            Thread.Sleep(100);
            return true;


            /*
            uint ret = 0;
            try
            {
                int nMtSysNo = 0;
                if (m_nMultInit == 0)
                {
                    if (!InitMultMove(nMtSysNo, nAixsArray, vm, acc))
                    {
                        //WarningMgr.GetInstance().Error(string.Format("30116,ERR-XYT,IMC30G-E Card Aixs {0} MultMove Init Error,result = {1}"));
                        //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsLinearMove",
                        //    string.Format("IMC30G-E Card Aixs {0} MultMove Init Error,result = {1}"));

                        return false;
                    }
                }
                for (int i = 0; i < nPosArray.Length; i += nAixsArray.Length)
                {
                    double[] EndPos = new double[3] { 0, 0, 0 };

                    for (int n = 0; n < Math.Min(EndPos.Length,nAixsArray.Length);n++)
                    {
                        EndPos[n] = nPosArray[i + n];
                    }
                    
                    ret = ImcApi.IMC_CrdLineXYZ(m_hCardHandle, (short)nMtSysNo, EndPos, 0);//在0号坐标系下压入一段末点为(10,10,5)的直线
                    if (ret != ImcApi.EXE_SUCCESS)
                    {
                        //WarningMgr.GetInstance().Error(string.Format("30116,ERR-XYT,IMC30G-E Card Aixs {0} MultMove WritePoint Error,result = {1}"));
                        WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsLinearMove",
                            string.Format("IMC30G-E Card IMC_CrdLineXYZ Error,result = {0}", ret.ToString("x8")));

                        return false;
                    }
                }
                short IsFinished = new short();
                ret = ImcApi.IMC_CrdEndData(m_hCardHandle, (short)nMtSysNo, ref IsFinished);  //把PC FIFO中的线段送入板卡FIFO中
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30116,ERR-XYT,IMC30G-E Card Aixs {0} MultMove WritePoint Error,result = {1}"));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsLinearMove",
                        string.Format("IMC30G-E Card IMC_CrdEndData Error,result = {0}", ret.ToString("x8")));

                    return false;
                }
                ret = ImcApi.IMC_CrdStart(m_hCardHandle, (short)nMtSysNo); //启动插补运动
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30116,ERR-XYT,IMC30G-E Card Aixs {0} MultMove Start Error,result = {1}"));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsLinearMove",
                        string.Format("IMC30G-E Card IMC_CrdStart Error,result = {0}", ret.ToString("x8")));

                    return false;
                }
                Thread.Sleep(100);
                return true;
            }
            catch (Exception ex)
            {
                WarningMgr.GetInstance().Error(ex.Message);
                return false;
            }
            */
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

            uint ret = ImcApi.IMC_ImmediateLineMoveInSynVelAcc(m_hCardHandle, pMaskAxNo, (short)(nAixsArray.Length), fPosOffsetArray, dVel, dAcc, dDec, dSmooth, 1);
            if (ret != ImcApi.EXE_SUCCESS)
            {
              //  WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "RelativeLinearMove",
                  //  string.Format("IMC30G-E Card Aixs {0} IMC_ImmediateLineMoveInSynVelAcc Error,result = {1}", pMaskAxNo[0], ret.ToString("x8")));
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
        /// <param name="Dir"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool AbsArcMove(ref int[] nAixsArray, ref double[] fCenterArray, ref double[] fEndArray, int Dir, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
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

            uint ret = ImcApi.IMC_ImmediateArcCenterMove(m_hCardHandle, pMaskAxNo, (short)(nAixsArray.Length),
                fCenterArray, fEndArray, (short)Dir, 0, 0, dVel, dAcc, dDec, dSmooth, 0);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsArcMove",
                 //   string.Format("IMC30G-E Card Aixs {0} IMC_ImmediateArcCenterMove Error,result = {1}", pMaskAxNo[0], ret.ToString("x8")));
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
        /// <param name="Dir"></param>
        /// <param name="vm"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param>
        /// <param name="vs"></param>
        /// <param name="ve"></param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool RelativeArcMove(ref int[] nAixsArray, ref double[] fCenterOffsetArray, ref double[] fEndArray, int Dir, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
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

            uint ret = ImcApi.IMC_ImmediateArcCenterMove(m_hCardHandle, pMaskAxNo, (short)(nAixsArray.Length),
                fCenterOffsetArray, fEndArray, (short)Dir, 0, 0, dVel, dAcc, dDec, dSmooth, 1);
            if (ret != ImcApi.EXE_SUCCESS)
            {
               // WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "RelativeArcMove",
               //     string.Format("IMC30G-E Card Aixs {0} IMC_ImmediateArcCenterMove Error,result = {1}", pMaskAxNo[0], ret.ToString("x8")));
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
            uint ret = ImcApi.IMC_CrdSetMtSys(m_hCardHandle, (short)nMtSysNo, pMaskAxNo, 3000, 500000.0);
            if ((ret & 0xffff) == 0x0075)
            {
                m_nMultInit = 1;
            }
            else if (ret == ImcApi.EXE_SUCCESS)
            {
                m_nMultInit = 1;
            }
            else
            {
                m_nMultInit = 0; ;
            }
            ret = ImcApi.IMC_CrdSetTrajVel(m_hCardHandle, (short)nMtSysNo, vm);
            ret = ImcApi.IMC_CrdSetTrajAcc(m_hCardHandle, (short)nMtSysNo, acc);//设置插补进给速度和加速度
            ret = ImcApi.IMC_CrdSetZeroFlag(m_hCardHandle, (short)nMtSysNo, 0);  //每段末速度不强制为0
            ret = ImcApi.IMC_CrdSetIncMode(m_hCardHandle, (short)nMtSysNo, 0); //插补编程方式为绝对编程方式
            ImcApi.TCrdAdvParam CrdAdvParam = new ImcApi.TCrdAdvParam();
            CrdAdvParam.userVelMode = 0;  //系统规划模式
            CrdAdvParam.transMode = 1; //过渡模式
            CrdAdvParam.noDataProtect = 0; //数据断流无保护
            CrdAdvParam.noCoplaneCircOptm = 0; //异面过渡无处理
            CrdAdvParam.turnCoef = 1.0; //拐角系数1.0
            CrdAdvParam.tol = 0.1; //轨迹精度0.1 unit
            ret = ImcApi.IMC_CrdSetAdvParam(m_hCardHandle, (short)nMtSysNo, ref CrdAdvParam);  //设置插补高级参数
            if (ret != ImcApi.EXE_SUCCESS)
            {
                //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsLinearMove",
                   //         string.Format("IMC30G-E Card IMC_CrdSetAdvParam Error,result = {0}", ret.ToString("x8")));

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
                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }
            // 读取速度
            double dVel = 0;
            double dAcc = 0;
            double dDec = 0;
            uint ret = ImcApi.IMC_GetSingleAxMvPara(m_hCardHandle, (short)nAxisNo, ref dVel, ref dAcc, ref dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30116,ERR-XYT,IMC30G-E Card Aixs {0} get single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));
                   // WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Aixs {0} get single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));

                }
                return false;
            }
            dAcc = nSpeed * 10;
            dDec = nSpeed * 10;
            // 设置速度
            ret = ImcApi.IMC_SetSingleAxMvPara(m_hCardHandle, (short)nAxisNo, nSpeed, dAcc, dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30117,ERR-XYT,IMC30G-E Card Aixs {0} set single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel, GetSysAxisNo(nAxisNo).ToString(),
                    //    string.Format("IMC30G-E Card Aixs {0} set single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));

                }
                return false;
            }

            // 执行运动
            short nPosType = 1;//运动模式0：表示绝对位置，1：表示相对位置
            ret = ImcApi.IMC_StartPtpMove(m_hCardHandle, (short)nAxisNo, nPos, nPosType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30118,ERR-XYT,IMC30G-E Card Aixs {0} start PTP move Error,result = {1}", nAxisNo, ret.ToString("x8")));
                   // WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Aixs {0} start PTP move Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }
            // 读取速度
            double dVel = vm;
            double dAcc = vm / acc;
            double dDec = vm / dec;

            // 设置速度
            uint ret = ImcApi.IMC_SetSingleAxMvPara(m_hCardHandle, (short)nAxisNo, dVel, dAcc, dDec);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30117,ERR-XYT,IMC30G-E Card Aixs {0} set single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));
                   // WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel, GetSysAxisNo(nAxisNo).ToString(),
                      //  string.Format("IMC30G-E Card Aixs {0} set single axis moving parameter Error,result = {1}", nAxisNo, ret.ToString("x8")));

                }
                return false;
            }

            // 执行运动
            short nPosType = 1;//运动模式0：表示绝对位置，1：表示相对位置
            ret = ImcApi.IMC_StartPtpMove(m_hCardHandle, (short)nAxisNo, fOffset, nPosType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30118,ERR-XYT,IMC30G-E Card Aixs {0} start PTP move Error,result = {1}", nAxisNo, ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel, GetSysAxisNo(nAxisNo).ToString(),
                      //  string.Format("IMC30G-E Card Aixs {0} start PTP move Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }
            double tgVel = bPositive ? Math.Abs(nSpeed) : -Math.Abs(nSpeed);
            uint ret = ImcApi.IMC_StartJogMove(m_hCardHandle, (short)nAxisNo, tgVel);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30119,ERR-XYT,IMC30G-E Card Aixs {0} start jog Error,result = {1}", nAxisNo, ret.ToString("x8")));
                   // WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Jog, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Aixs {0} start jog Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
                ImcApi.IMC_StopHoming(m_hCardHandle, (short)nAxisNo, 0);
                Thread.Sleep(100);
                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }
            else if (IsCrdMode(nAxisNo) == 1)
            {
                int nCrdNo;
                if (m_dictCrdAxis.TryGetValue(nAxisNo, out nCrdNo))
                {
                    ImcApi.IMC_CrdStop(m_hCardHandle, (short)nCrdNo, 0);

                    ImcApi.IMC_CrdClrData(m_hCardHandle, (short)nCrdNo);

                    ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)nCrdNo);

                    m_dictCrdAxis.Remove(nAxisNo);
                }
            }
            else if (IsImmediateMoveMode(nAxisNo) == 1)
            {
                ImcApi.IMC_ImmediateMoveStop(m_hCardHandle, (short)nAxisNo);
            }
            //if(IsMultMoveMode(nAxisNo)>0)
            //{
            //    ImcApi.IMC_CrdStop(m_hCardHandle, 0, 0);
            //    ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)0);
            //    m_nMultInit = 0;
            //}



            short stopType = 0;//0:平滑停止, 1:急速停止
            uint ret = ImcApi.IMC_AxMoveStop(m_hCardHandle, (short)nAxisNo, stopType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30120,ERR-XYT,IMC30G-E Card Aixs {0} stop Error,result = {1}", nAxisNo, ret.ToString("x8")));
                   // WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Stop, GetSysAxisNo(nAxisNo).ToString(),
                    //    string.Format("IMC30G-E Card Aixs {0} stop Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
                ImcApi.IMC_StopHoming(m_hCardHandle, (short)nAxisNo, 1);
                Thread.Sleep(100);

                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }
            else if (IsCrdMode(nAxisNo) == 1)
            {
                int nCrdNo;
                if (m_dictCrdAxis.TryGetValue(nAxisNo, out nCrdNo))
                {
                    ImcApi.IMC_CrdStop(m_hCardHandle, (short)nCrdNo, 1);

                    ImcApi.IMC_CrdClrData(m_hCardHandle, (short)nCrdNo);

                    ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)nCrdNo);

                    m_dictCrdAxis.Remove(nAxisNo);
                }
            }
            else if (IsImmediateMoveMode(nAxisNo) == 1)
            {
                ImcApi.IMC_ImmediateMoveEStop(m_hCardHandle, (short)nAxisNo, 1000000.0);
            }
            //if (IsMultMoveMode(nAxisNo) > 0)
            //{
            //    ImcApi.IMC_CrdStop(m_hCardHandle, 0, 1);
            //    ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)0);
            //    m_nMultInit = 0;
            //}
            short stopType = 1;//0:平滑停止, 1:急速停止
            uint ret = ImcApi.IMC_AxMoveStop(m_hCardHandle, (short)nAxisNo, stopType);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30121,ERR-XYT,IMC30G-E Card Aixs {0} e-stop Error,result = {1}", nAxisNo, ret.ToString("x8")));
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_EmgStop, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Aixs {0} e-stop Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
            uint ret = ImcApi.IMC_GetAxSts(m_hCardHandle, (short)nAxisNo, axStatus, 1);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30122,ERR-XYT,IMC30G-E Card Aixs {0} Get Status Error,result = {1}", nAxisNo, ret.ToString("x8")));
                  //  WarningMgr.GetInstance().Error(ErrorType.Err_Motion_State, GetSysAxisNo(nAxisNo).ToString(),
                    //    string.Format("IMC30G-E Card Aixs {0} Get Status Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
            //uint ret = ImcApi.IMC_ClrAxSts(m_hCardHandle, (short)nAxisNo, 1);
            //if (ret != ImcApi.EXE_SUCCESS)
            //{
            //    if (Enable)
            //        WarningMgr.GetInstance().Error(string.Format("30123,ERR-XYT,IMC30G-E Card Aixs {0} Get Status Error,result = {1}", nAxisNo, ret.ToString("x8")));
            //    return -1;
            //}
            uint ret = ImcApi.IMC_GetAxSts(m_hCardHandle, (short)nAxisNo, axStatus, 1);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30123,ERR-XYT,IMC30G-E Card Aixs {0} Get Status Error,result = {1}", nAxisNo, ret.ToString("x8")));
                  //  WarningMgr.GetInstance().Error(ErrorType.Err_Motion_State, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Aixs {0} Get Status Error,result = {1}", nAxisNo, ret.ToString("x8")));

                }
                return -1;
            }
            int m_pDigitalInput = 0;
            ret = ImcApi.IMC_GetAxEcatDigitalInput(m_hCardHandle, (short)nAxisNo, ref m_pDigitalInput);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30123,ERR-XYT,IMC30G-E Card Aixs {0} Get Status Error,result = {1}", nAxisNo, ret.ToString("x8")));
                  //  WarningMgr.GetInstance().Error(ErrorType.Err_Motion_State, GetSysAxisNo(nAxisNo).ToString(),
                    //    string.Format("IMC30G-E Card Aixs {0} Get Status Error,result = {1}", nAxisNo, ret.ToString("x8")));

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
            //if ((axStatus[0] & (0x01 << 4)) != 0)
            //    nStdIo |= (0x01 << 1);//0000 0001B
            //if ((axStatus[0] & (0x01 << 5)) != 0)
            //    nStdIo |= (0x01 << 2);
            if ((axStatus[0] & (0x01 << 6)) != 0)
                nStdIo |= (0x01 << 11);
            if ((axStatus[0] & (0x01 << 7)) != 0)
                nStdIo |= (0x01 << 12);
            if ((axStatus[0] & (0x01 << 9)) != 0)
                nStdIo |= (0x01 << 4);

            if ((m_pDigitalInput & (0x01 << 0)) != 0)
                nStdIo |= (0x01 << 2);
            if ((m_pDigitalInput & (0x01 << 1)) != 0)
                nStdIo |= (0x01 << 1);
            if ((m_pDigitalInput & (0x01 << 2)) != 0)
                nStdIo |= (0x01 << 3);

            if ((((axStatus[0] & (0x01 << 4)) != 0) && ((m_pDigitalInput & (0x01 << 1)) == 0))
                || (((axStatus[0] & (0x01 << 5)) != 0) && ((m_pDigitalInput & (0x01 << 0)) == 0)))
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
            uint ret = ImcApi.IMC_ClrAxSts(m_hCardHandle, (short)nAxisNo, 1);
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
            uint ret = ImcApi.IMC_GetAxEncPos(m_hCardHandle, (short)nAxisNo, pPrfPos, 1);
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
            uint ret = ImcApi.IMC_GetAxSts(m_hCardHandle, (short)nAxisNo, axStatus, 1);
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
            //if (IsMultMoveMode(nAxisNo)==1)
            //{
            //    return IsMultMoveStop(0);
            //}
            if (IsCrdMode(nAxisNo) == 1)
            {
                return IsCrdStop(m_dictCrdAxis[nAxisNo]);
            }

            int nRet = IsAxisNormalStop(nAxisNo);
            if (nRet == 0)
            {
                double[] pPrfPos = new double[1];
                double[] pEncPos = new double[1];
                uint ret = ImcApi.IMC_GetPrfPos(m_hCardHandle, (short)nAxisNo, pPrfPos, 1);
                if (ret != ImcApi.EXE_SUCCESS)
                    return -1;
                ret = ImcApi.IMC_GetAxEncPos(m_hCardHandle, (short)nAxisNo, pEncPos, 1);
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
            uint ret = ImcApi.IMC_SetAxCurPos(m_hCardHandle, (short)nAxisNo, 0);
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
                    m_homePara.acc = (uint)nData;
                    break;
                case 2:
                    break;

                case 3:
                    m_homePara.lowVel = (uint)nData;
                    break;

                case 4:
                    m_homePara.highVel = (uint)nData;
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
                ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
            }
            uint ret = ImcApi.IMC_StartJogMove(m_hCardHandle, (short)nAxisNo, nSpeed);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30124,ERR-XYT,IMC30G-E Card Aixs {0} start Velocity Move Error,result = {1}", nAxisNo, ret.ToString("x8")));
                  //  WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Vel, GetSysAxisNo(nAxisNo).ToString(),
                     //   string.Format("IMC30G-E Card Aixs {0} start Velocity Move Error,result = {1}", nAxisNo, ret.ToString("x8")));
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

            uint ret = ImcApi.IMC_GetHomingStatus(m_hCardHandle, (short)nAxisNo, ref nHomeStatus);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                    //    string.Format("IMC30G-E Card Aixs {0} IMC_GetHomingStatus Error,result = {1}", nAxisNo, ret.ToString("x8")));
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
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                    //    string.Format("IMC30G-E Card Aixs {0} Home error -  interrupt or no start", nAxisNo));
                    return -1;

                case 2:
                   // WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                    //    string.Format("IMC30G-E Card Aixs {0} Home error - not inpos", nAxisNo));
                    return -1;

                case 3:
                    //最新的dll可以不调用，调用了也没关系
                    ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);
                    return 0;

                default:
                    //最新的dll可以不调用，调用了也没关系
                    ImcApi.IMC_FinishHoming(m_hCardHandle, (short)nAxisNo);

                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                    //    string.Format("IMC30G-E Card Aixs {0} Home error", nAxisNo));

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
            uint ret = ImcApi.IMC_GetAxPrfMode(m_hCardHandle, (short)nAxisNo, nctrlModel);
            if (ret != ImcApi.EXE_SUCCESS)
            {

                return -2;
            }
            int m_ctrlModel = nctrlModel[0] & 0x0f;
            if (m_ctrlModel != 15)
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
            uint ret = ImcApi.IMC_CrdGetArrivalSts(m_hCardHandle, (short)nMtSysNo, ref sts);

            if (ret == ImcApi.EXE_SUCCESS)
            {
                if (sts == 1)
                {
                    ImcApi.IMC_CrdStop(m_hCardHandle, (short)nMtSysNo, 0);
                    ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)nMtSysNo);
                    m_nMultInit = 0;
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
            uint ret = ImcApi.IMC_GetAxPrfMode(m_hCardHandle, (short)nAxisNo, nctrlModel);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                return -2;
            }
            int m_ctrlModel = (nctrlModel[0] & 0x70) >> 4;
            if (m_ctrlModel == 0x01)
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
            uint ret = ImcApi.IMC_GetAxPrfMode(m_hCardHandle, (short)nAxisNo, nPrfModel);
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

                    if (m_dictCrdAxis.ContainsKey(nAixsArray[i]))
                    {
                        m_dictCrdAxis[nAixsArray[i]] = crdNo;
                    }
                    else
                    {
                        m_dictCrdAxis.Add(nAixsArray[i], crdNo);
                    }
                }
                else
                {
                    pMaskAxNo[i] = 31;
                }
            }

            //插补坐标系已经建立,先删除
            uint ret = ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)crdNo);

            //0号位建立插补坐标系，前瞻数3000段，5000.0的急停减加速度
            ret = ImcApi.IMC_CrdSetMtSys(m_hCardHandle, (short)crdNo, pMaskAxNo, 3000, 500000.0);
            if ((ret & 0xffff) == 0x0075)
            {
                //插补坐标系已经建立,先删除
                ret = ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)crdNo);

                Thread.Sleep(100);

                ret = ImcApi.IMC_CrdSetMtSys(m_hCardHandle, (short)crdNo, pMaskAxNo, 3000, 500000.0);
            }

            if (ret != ImcApi.EXE_SUCCESS)
            {
                //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "ConfigPointTable",
                   //         string.Format("IMC30G-E Card IMC_CrdSetMtSys Error,result = {0}", ret.ToString("x8")));

                return false;
            }

            //清除错误
            ret = ImcApi.IMC_CrdClrError(m_hCardHandle, (short)crdNo);

            //清除缓存
            ret = ImcApi.IMC_CrdClrData(m_hCardHandle, (short)crdNo);

            if (bAbsolute)
            {
                ret = ImcApi.IMC_CrdSetIncMode(m_hCardHandle, (short)crdNo, 0); //插补编程方式为绝对编程方式
            }
            else
            {
                ret = ImcApi.IMC_CrdSetIncMode(m_hCardHandle, (short)crdNo, 1); //插补编程方式为绝对编程方式
            }


            ImcApi.TCrdAdvParam CrdAdvParam = new ImcApi.TCrdAdvParam();
            CrdAdvParam.userVelMode = 0;  //系统规划模式
            CrdAdvParam.transMode = 1; //过渡模式
            CrdAdvParam.noDataProtect = 0; //数据断流无保护
            CrdAdvParam.noCoplaneCircOptm = 0; //异面过渡无处理
            CrdAdvParam.turnCoef = 1.0; //拐角系数1.0
            CrdAdvParam.tol = 0.1; //轨迹精度0.1 unit
            ret = ImcApi.IMC_CrdSetAdvParam(m_hCardHandle, (short)crdNo, ref CrdAdvParam);  //设置插补高级参数
            if (ret != ImcApi.EXE_SUCCESS)
            {
               // WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "ConfigPointTable",
                  //          string.Format("IMC30G-E Card IMC_CrdSetAdvParam Error,result = {0}", ret.ToString("x8")));

                return false;
            }

            //m_nCrdDimension = nAixsArray.Length;

            //m_nCrdNo = crdNo;

            if (m_dicBoard.ContainsKey(crdNo))
            {
                m_dicBoard[crdNo] = nAixsArray.Length;
            }
            else
            {
                m_dicBoard.Add(crdNo, nAixsArray.Length);
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
            if (m_dicBoard[crdNo] != positionArray.Length)
            {
               // WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_Line_Move",
                //  string.Format("IMC30G-E Card MtSys {0} Dimension error,PointTable_Line_Move", crdNo));
                return false;
            }

            //设置速度，加速度

            //通过加减速时间计算加减速度
            acc = vm / acc;
            dec = vm / dec;

            //设置运行速度
            uint ret = ImcApi.IMC_CrdSetTrajVel(m_hCardHandle, (short)crdNo, vm);

            //设置加速度和减速度
            ret = ImcApi.IMC_CrdSetTrajAccAndDec(m_hCardHandle, (short)crdNo, acc, dec);

            //设置开始速度,结束速度
            ret = ImcApi.IMC_CrdUserVelPlan(m_hCardHandle, (short)crdNo, vs, ve);

            //设置平滑系数
            ret = ImcApi.IMC_CrdSetSmoothParam(m_hCardHandle, (short)crdNo, (int)sf, sf - (int)sf);

            //插入运动点位
            if (m_dicBoard[crdNo] == 3)
            {
                //三轴插补
                ret = ImcApi.IMC_CrdLineXYZ(m_hCardHandle, (short)crdNo, positionArray);

                if (ret != ImcApi.EXE_SUCCESS)
                {
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_Line_Move",
                     //   string.Format("IMC30G-E Card IMC_CrdLineXYZ Error,result = {0}", ret.ToString("x8")));

                    return false;
                }
            }
            else
            {
                //二轴插补，默认为XY
                ret = ImcApi.IMC_CrdLineXY(m_hCardHandle, (short)crdNo, positionArray);

                if (ret != ImcApi.EXE_SUCCESS)
                {
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_Line_Move",
                       // string.Format("IMC30G-E Card IMC_CrdLineXY Error,result = {0}", ret.ToString("x8")));

                    return false;
                }
            }

            //把数据压入队列
            short IsFinished = new short();
            ret = ImcApi.IMC_CrdEndData(m_hCardHandle, (short)crdNo, ref IsFinished);  //把PC FIFO中的线段送入板卡FIFO中
            if (ret != ImcApi.EXE_SUCCESS)
            {
                //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_Line_Move",
                  //  string.Format("IMC30G-E Card IMC_CrdEndData Error,result = {0}", ret.ToString("x8")));

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
            if (m_dicBoard[crdNo] != centerArray.Length || m_dicBoard[crdNo] != 2)
            {
               // WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_ArcE_Move",
                //  string.Format("IMC30G-E Card MtSys {0} Dimension error,PointTable_Line_Move", crdNo));
                return false;
            }

            //设置速度，加速度

            //通过加减速时间计算加减速度
            acc = vm / acc;
            dec = vm / dec;

            //设置运行速度
            uint ret = ImcApi.IMC_CrdSetTrajVel(m_hCardHandle, (short)crdNo, vm);

            //设置加速度和减速度
            ret = ImcApi.IMC_CrdSetTrajAccAndDec(m_hCardHandle, (short)crdNo, acc, dec);

            //设置开始速度,结束速度
            ret = ImcApi.IMC_CrdUserVelPlan(m_hCardHandle, (short)crdNo, vs, ve);

            //设置平滑系数
            ret = ImcApi.IMC_CrdSetSmoothParam(m_hCardHandle, (short)crdNo, (int)sf, sf - (int)sf);

            //XY平面内圆心末点编程。Z轴不作圆周运动
            ret = ImcApi.IMC_CrdArcCenterXYPlane(m_hCardHandle, (short)crdNo, centerArray, endArray, dir);
            if (ret != ImcApi.EXE_SUCCESS)
            {
                //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_ArcE_Move",
                  //  string.Format("IMC30G-E Card IMC_CrdArcCenterXYPlane Error,result = {0}", ret.ToString("x8")));

                return false;
            }

            //把数据压入队列
            short IsFinished = new short();
            ret = ImcApi.IMC_CrdEndData(m_hCardHandle, (short)crdNo, ref IsFinished);  //把PC FIFO中的线段送入板卡FIFO中
            if (ret != ImcApi.EXE_SUCCESS)
            {
                //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_ArcE_Move",
                  //  string.Format("IMC30G-E Card IMC_CrdEndData Error,result = {0}", ret.ToString("x8")));

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
            uint ret = ImcApi.IMC_CrdSetDO(m_hCardHandle, (short)crdNo, (short)nChannel, 0, (short)bOn);

            if (ret != ImcApi.EXE_SUCCESS)
            {
                //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_IO",
                  //  string.Format("IMC30G-E Card IMC_CrdSetDO Error,result = {0}", ret.ToString("x8")));

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
            uint ret = ImcApi.IMC_CrdWaitTime(m_hCardHandle, (short)crdNo, nMillsecond);

            if (ret != ImcApi.EXE_SUCCESS)
            {
               // WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_Delay",
                 //   string.Format("IMC30G-E Card IMC_CrdWaitTime Error,result = {0}", ret.ToString("x8")));

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
            uint ret = ImcApi.IMC_CrdGetSpace(m_hCardHandle, (short)crdNo, ref nSpace);

            if (ret != ImcApi.EXE_SUCCESS)
            {
               // WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_IsIdle",
                //    string.Format("IMC30G-E Card IMC_CrdGetSpace Error,result = {0}", ret.ToString("x8")));

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
            uint ret = 0;
            if (bStart)
            {
                ret = ImcApi.IMC_CrdStart(m_hCardHandle, (short)crdNo);

                if (ret != ImcApi.EXE_SUCCESS)
                {
                    //WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_Start",
                   //     string.Format("IMC30G-E Card IMC_CrdStart Error,result = {0}", ret.ToString("x8")));

                    return false;
                }
            }
            else
            {
                ret = ImcApi.IMC_CrdStop(m_hCardHandle, (short)crdNo, 0);

                ret = ImcApi.IMC_CrdClrData(m_hCardHandle, (short)crdNo);

                ret = ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)crdNo);
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
            uint ret = ImcApi.IMC_CrdGetArrivalSts(m_hCardHandle, (short)crdNo, ref sts);

            if (ret == ImcApi.EXE_SUCCESS)
            {
                if (sts == 1)
                {
                    ImcApi.IMC_CrdStop(m_hCardHandle, (short)crdNo, 0);
                    ImcApi.IMC_CrdDeleteMtSys(m_hCardHandle, (short)crdNo);
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
            uint ret = ImcApi.IMC_GetAxPrfMode(m_hCardHandle, (short)nAxisNo, nPrfModel);
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
        public override bool SetSPELEnable(int nAxisNo, bool bEnable)
        {
            uint ret = 0;
            if (bEnable)
            {
                ret = ImcApi.IMC_AxSoftLmtsEnable(m_hCardHandle, (short)nAxisNo);
            }
            else
            {
                ret = ImcApi.IMC_AxSoftLmtsDisable(m_hCardHandle, (short)nAxisNo);
            }

            return ret == ImcApi.EXE_SUCCESS;
        }

        /// <summary>
        /// 启用软件负限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public override bool SetSMELEnable(int nAxisNo, bool bEnable)
        {
            uint ret = 0;
            if (bEnable)
            {
                ret = ImcApi.IMC_AxSoftLmtsEnable(m_hCardHandle, (short)nAxisNo);
            }
            else
            {
                ret = ImcApi.IMC_AxSoftLmtsDisable(m_hCardHandle, (short)nAxisNo);
            }

            return ret == ImcApi.EXE_SUCCESS;
        }

        /// <summary>
        /// 设置软件正限位位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override bool SetSPELPos(int nAxisNo, double pos)
        {
            int nSoftPosLimit = 0, nSoftNegLimit = 0;
            uint ret = ImcApi.IMC_GetAxSoftLimit(m_hCardHandle, (short)nAxisNo, ref nSoftPosLimit, ref nSoftNegLimit);

            ret = ImcApi.IMC_SetAxSoftLimit(m_hCardHandle, (short)nAxisNo, (int)pos, nSoftNegLimit);

            return ret == ImcApi.EXE_SUCCESS;
        }

        /// <summary>
        /// 设置软件负限位位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override bool SetSMELPos(int nAxisNo, double pos)
        {
            int nSoftPosLimit = 0, nSoftNegLimit = 0;
            uint ret = ImcApi.IMC_GetAxSoftLimit(m_hCardHandle, (short)nAxisNo, ref nSoftPosLimit, ref nSoftNegLimit);

            ret = ImcApi.IMC_SetAxSoftLimit(m_hCardHandle, (short)nAxisNo, nSoftPosLimit, (int)pos);

            return ret == ImcApi.EXE_SUCCESS;
        }
    }
}
