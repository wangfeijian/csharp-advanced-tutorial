/********************************************************************
	created:	2017/04/19
	filename: 	MOTION_DMC3400
	file ext:	h
	author:		gxf
	purpose:	DMC3400运动控制卡的封装类
*********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using CommonTool;
using csLTDMC;


namespace MotionIO
{
    /// <summary>
    /// DMC3400运动控制卡封装,类名必须以"Motion_"前导，否则加载不到
    /// </summary>
    public class Motion_Dmc3400 : Motion
    {
        private UInt16 m_nInternalIndex = 0;

        //todo:板卡类应该只初始化一次
        /// <summary>构造函数
        /// 
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
        public Motion_Dmc3400(int nCardIndex, string strName,int nMinAxisNo, int nMaxAxisNo)
            :base(nCardIndex, strName, nMinAxisNo, nMaxAxisNo)
        {
            m_nInternalIndex = (UInt16)(nCardIndex - 1);
            m_bEnable = false;
        }
        /// <summary>
        /// 轴卡初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            //TRACE("init card\r\n");
            string str1 = "运动控制卡DMC3400初始化失败, result = {0}";
            string str2 = "运动控制卡DMC3400读取配置文件失败, result = {0}";
            string str3 = "控制卡DMC3400初始化失败";
            if (LanguageMgr.GetInstance().LanguageID != 0)
            {
                str1 = "Initialization of motion control card DMC3400 failed, result = {0}";
                str2 = "The motion control card DMC3400 failed to read the configuration file, result = {0}";
                str3 = "Initialization of control card DMC3400 failed";
            }

            try
            {
                //获取卡数量
                short nCardNum = LTDMC.dmc_board_init();//获取卡数量
                if (nCardNum <= 0 || nCardNum > 8)
                {
                    m_bEnable = false;
                    //WarningMgr.GetInstance().Error(string.Format("30100,ERR-XYT,运动控制卡DMC3400初始化失败, result = {0}", nCardNum));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, m_nCardIndex.ToString(),
                        string.Format(str1, nCardNum));

                    return false;
                }

                short ret = LTDMC.dmc_download_configfile(m_nInternalIndex, "DMC3400CFG.cfg");
                if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
                {
                    m_bEnable = false;
                    //WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,运动控制卡DMC3400读取配置文件失败, result = {0}", ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init,m_nCardIndex.ToString(),
                        string.Format(str2, ret));

                    return false;
                }

                m_bEnable = true;
                return true;
            }
            catch (Exception e)
            {
                m_bEnable = false;
                System.Windows.Forms.MessageBox.Show(e.Message, str3, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }                   
        }

        /// <summary>
        /// 关闭轴卡
        /// </summary>
        /// <returns></returns>
        public override bool DeInit()
        {
            short ret = LTDMC.dmc_board_close();
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    string str1 = "DMC3400板卡库文件关闭出错! result = {0}";
                    if (LanguageMgr.GetInstance().LanguageID != 0)
                    {
                        str1 = "DMC3400 board card library file close error! Result = {0}";
                    }
                    //WarningMgr.GetInstance().Error(string.Format("30102,ERR-XYT,DMC3400板卡库文件关闭出错! result = {0}", ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_DeInit,m_nCardIndex.ToString(),
                        string.Format(str1, ret));

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
            short ret = LTDMC.dmc_write_sevon_pin(m_nInternalIndex, (ushort)nAxisNo, 0);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30103,ERR-XYT,DMC3400 Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_ServoOn,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret));

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
            short ret = LTDMC.dmc_write_sevon_pin(m_nInternalIndex, (ushort)nAxisNo, 1);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30103,ERR-XYT,DMC3400 Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_ServoOff,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret));

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
            short nOnOff = LTDMC.dmc_read_sevon_pin(m_nInternalIndex, (ushort)nAxisNo);
            return (nOnOff == 0);
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">回原点参数, 对于DMC3400，此参数代表回原点的方向</param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nParam)
        {
            short ret = LTDMC.dmc_home_move(m_nInternalIndex, (ushort)nAxisNo);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,DMC3400 Axis {0} Home Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} Home Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nParam"></param>
        /// <param name="vm"></param>
        /// <param name="vo"></param>
        /// <param name="acc"></param>
        /// <param name="dec"></param> 
        /// <param name="offset"></param>
        /// <param name="sFac"></param>    
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nParam, double vm, double vo, double acc, double dec, double offset = 0, double sFac = 0)
        {
            //获取回原点配置
            short ret = LTDMC.dmc_home_move(m_nInternalIndex, (ushort)nAxisNo);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,DMC3400 Axis {0} Home Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} Home Error,result = {1}", nAxisNo, ret));

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
            short ret = LTDMC.dmc_change_speed(m_nInternalIndex, (ushort)nAxisNo, nSpeed, 0);
            if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,DMC3400 Axis {0} abs move (change speed) Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} abs move (change speed) Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }

            ret = LTDMC.dmc_pmove(m_nInternalIndex, (ushort)nAxisNo, nPos, 1);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,DMC3400 Axis {0} abs move Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} abs move Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
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
            //设置单轴速度曲线 S段参数值
            short ret = LTDMC.dmc_set_s_profile(m_nInternalIndex, (ushort)nAxisNo, 0, sFac);

            ret = LTDMC.dmc_set_profile(m_nInternalIndex, (ushort)nAxisNo, vs, vm,acc,dec,ve);
            if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,DMC3400 Axis {0} abs move (change speed) Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} abs move (change speed) Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }

            ret = LTDMC.dmc_pmove(m_nInternalIndex, (ushort)nAxisNo, (int)fPos, 1);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,DMC3400 Axis {0} abs move Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} abs move Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
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
            short ret = LTDMC.dmc_change_speed(m_nInternalIndex, (ushort)nAxisNo, nSpeed, 0);
            if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,DMC3400 Axis {0} relative move (change speed) Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} relative move (change speed) Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }

            ret = LTDMC.dmc_pmove(m_nInternalIndex, (ushort)nAxisNo, nPos, 0);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,DMC3400 Axis {0} relative move Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} relative move Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 相对运动
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
            //设置单轴速度曲线 S段参数值
            short ret = LTDMC.dmc_set_s_profile(m_nInternalIndex, (ushort)nAxisNo, 0, sFac);

            ret = LTDMC.dmc_set_profile(m_nInternalIndex, (ushort)nAxisNo, vs, vm, acc, dec, ve);
            if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,DMC3400 Axis {0} relative move (change speed) Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} relative move (change speed) Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }

            ret = LTDMC.dmc_pmove(m_nInternalIndex, (ushort)nAxisNo, (int)fOffset, 0);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,DMC3400 Axis {0} relative move Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} relative move Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
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
            short ret = LTDMC.dmc_change_speed(m_nInternalIndex, (ushort)nAxisNo, nSpeed, 0);
            if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30108,ERR-XYT,DMC3400 Axis {0} jog move (change speed) Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Jog,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} jog move (change speed) Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }

            int nJogDir = bPositive ? 1 : 0;
            ret = LTDMC.dmc_vmove(m_nInternalIndex, (ushort)nAxisNo, (ushort)nJogDir);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30108,ERR-XYT,DMC3400 Axis {0} jog move Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Jog,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} jog move Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 轴正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool StopAxis(int nAxisNo)
        {
            short ret = LTDMC.dmc_stop(m_nInternalIndex, (ushort)nAxisNo, 0);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30109,ERR-XYT,DMC3400 Card normal stop axis {0} Error, result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Stop,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Card normal stop axis {0} Error, result = {1}", nAxisNo, ret));

                }
                return false;
            }  
        }
        
        /// <summary>
        /// 急停
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool StopEmg(int nAxisNo)
        {
            short ret = LTDMC.dmc_stop(m_nInternalIndex, (ushort)nAxisNo, 0);
            if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30110,ERR-XYT,DMC3400 Card Emergency stop axis {0} Error, result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_EmgStop,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Card Emergency stop axis {0} Error, result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }
        
        /// <summary>
        /// 获取轴卡运动状态
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetMotionState(int nAxisNo)
        {
            int nStopFlag = LTDMC.dmc_check_done(m_nInternalIndex, (ushort)nAxisNo);
            return nStopFlag == 0 ? 1 : 0;
        }
        
        /// <summary>
        /// 获取轴卡运动IO信号
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetMotionIoState(int nAxisNo)
        {
            UInt32 uDmc3400Io = LTDMC.dmc_axis_io_status(m_nInternalIndex, (ushort)nAxisNo);
            // 0:ALARM
            // 1:EL+
            // 2:EL-
            // 3:EMG
            // 4:ORG
            // 6:SL+
            // 7:SL-
            // 8:INP
            // 9:EZ
            // 10:RDY
            // 11:DSTP

            long lStandardIo = 0;
            if ((uDmc3400Io & (0x01 << 0)) != 0)
                lStandardIo |= (0x01 << 0);
            if ((uDmc3400Io & (0x01 << 1)) != 0)
                lStandardIo |= (0x01 << 1);
            if ((uDmc3400Io & (0x01 << 2)) != 0)
                lStandardIo |= (0x01 << 2);
            if ((uDmc3400Io & (0x01 << 3)) != 0)
                lStandardIo |= (0x01 << 4);
            if ((uDmc3400Io & (0x01 << 4)) != 0)
                lStandardIo |= (0x01 << 3);
            if ((uDmc3400Io & (0x01 << 6)) != 0)
                lStandardIo |= (0x01 << 11);
            if ((uDmc3400Io & (0x01 << 7)) != 0)
                lStandardIo |= (0x01 << 12);
            if ((uDmc3400Io & (0x01 << 8)) != 0)
                lStandardIo |= (0x01 << 6);
            if ((uDmc3400Io & (0x01 << 9)) != 0)
                lStandardIo |= (0x01 << 5);

            short nOnOff = LTDMC.dmc_read_sevon_pin(m_nInternalIndex, (ushort)nAxisNo);
            if (nOnOff != 0)
                lStandardIo |= (0x01 << 7);

            return lStandardIo;
        }

        /// <summary>
        /// 获取轴的当前位置
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override double GetAixsPos(int nAxisNo)
        {
            long lFeedbackPos = LTDMC.dmc_get_encoder(m_nInternalIndex, (ushort)nAxisNo);
            return lFeedbackPos;
        }

        /// <summary>
        /// 轴是否正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns>0:正常信止, -1:未停止 其它:急停,报警等</returns>
        public override int IsAxisNormalStop(int nAxisNo)
        {
            int nStopFlag = LTDMC.dmc_check_done(m_nInternalIndex, (ushort)nAxisNo);
            if (nStopFlag == 0)
                return -1;  //正在运行

            // 0:ALARM
            // 1:EL+
            // 2:EL-
            // 3:EMG
            // 4:ORG
            // 6:SL+
            // 7:SL-
            // 8:INP
            // 9:EZ
            // 10:RDY
            // 11:DSTP
            UInt32 uMotionIo = LTDMC.dmc_axis_io_status(m_nInternalIndex, (ushort)nAxisNo);
            if ((uMotionIo & (0x01 << 3)) != 0)//急停
            {
                Debug.WriteLine("Axis {0} have Emg single \r\n", nAxisNo);
                return 1;
            }
            else if ((uMotionIo & (0x01 << 0)) != 0)//伺服报警
            {
                Debug.WriteLine("Axis {0} have Alm single \r\n", nAxisNo);
                return 2;
            }
            else if ((uMotionIo & (0x01 << 1)) != 0)//正限位
            {
                Debug.WriteLine("Axis {0} have PEL single \r\n", nAxisNo);
                return 4;
            }
            else if ((uMotionIo & (0x01 << 2)) != 0)//负限位
            {
                Debug.WriteLine("Axis {0} have PEL single \r\n", nAxisNo);
                return 5;
            }
            else if ((uMotionIo & (0x01 << 6)) != 0)//软限位正向
            {
                Debug.WriteLine("Axis {0} arrive at maximum soft limit. \r\n", nAxisNo);
                return 4;
            }
            else if ((uMotionIo & (0x01 << 7)) != 0)//软限位负向
            {
                Debug.WriteLine("Axis {0} arrive at minimum soft limit. \r\n", nAxisNo);
                return 5;
            }
            else if (!GetServoState(nAxisNo))    // 没有Servo-On
            {
                Debug.WriteLine("Axis {0} have servo-off single \r\n", nAxisNo);
                return 3;
            }
            else
            {
                return 0;   // 正常停止
            }
        }
          
        /// <summary>
        /// 判断轴是否到位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nInPosError">到位误差</param>
        /// <returns></returns>
        public override int IsAxisInPos(int nAxisNo, int nInPosError = 1000)
        {
            int nRet = IsAxisNormalStop(nAxisNo);
            if (nRet == 0)
            {
                int nTargetPos = LTDMC.dmc_get_position(m_nInternalIndex, (ushort)nAxisNo);
                int nFeedBackPos = LTDMC.dmc_get_encoder(m_nInternalIndex, (ushort)nAxisNo);

                if (Math.Abs(nFeedBackPos - nTargetPos) > nInPosError)
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
            short ret = LTDMC.dmc_set_encoder(m_nInternalIndex, (ushort)nAxisNo, 0);
            if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
            {
                return false;
            }

            ret = LTDMC.dmc_set_position(m_nInternalIndex, (ushort)nAxisNo, 0);
            if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
            {
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
            short ret = LTDMC.dmc_change_speed(m_nInternalIndex, (ushort)nAxisNo, Math.Abs(nSpeed), 0);
            if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30111,ERR-XYT,DMC3400 Axis {0} velocity move (change speed) Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Vel,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("DMC3400 Axis {0} velocity move (change speed) Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }

            int nDir = nSpeed > 0 ? 1 : 0;
            ret = LTDMC.dmc_vmove(m_nInternalIndex, (ushort)nAxisNo, (ushort)nDir);
            if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
            {
                //WarningMgr.GetInstance().Error(string.Format("30112,ERR-XYT,DMC3400 Card axis {0} VelocityMove Error, result = {1}", nAxisNo, ret));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Vel,GetSysAxisNo(nAxisNo).ToString(),
                    string.Format("DMC3400 Card axis {0} VelocityMove Error, result = {1}", nAxisNo, ret));

                return false;
            }

            return true;
        }

        /// <summary>
        ///此函数DMC3400板卡不提供不使用,回原点内部已经封装好过程处理 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsHomeNormalStop(int nAxisNo)
        {
            return IsAxisNormalStop(nAxisNo);
        }
    }
}
