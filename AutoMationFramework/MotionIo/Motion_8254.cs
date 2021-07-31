/********************************************************************
	created:	2013/11/12
	filename: 	MOTION_8254
	file ext:	h
	author:		zjh
	purpose:	8254运动控制卡的封装类
*********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using CommonTool;
using Adlink;


namespace MotionIO
{
    /// <summary>
    /// 8254运动控制卡封装,类名必须以"Motion_"前导，否则加载不到
    /// </summary>
    public class Motion_8254 : Motion
    {
        //todo:板卡类应该只初始化一次
        /// <summary>构造函数
        /// 
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
        public Motion_8254(int nCardIndex, string strName, int nMinAxisNo, int nMaxAxisNo)
             : base(nCardIndex, strName, nMinAxisNo, nMaxAxisNo)
        {
            m_bEnable = false;
        }
        /// <summary>
        /// 轴卡初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            //TRACE("init card\r\n");
            int boardID_InBits = 0;
            int mode = (Int32)APS_Define.INIT_AUTO_CARD_ID | (Int32)APS_Define.INIT_PARAM_LOAD_FLASH;

            try
            {
                int ret = APS168.APS_initial(ref boardID_InBits, mode);
                if ((Int32)APS_Define.ERR_NoError == ret)
                {
                    ret = APS168.APS_load_param_from_file("param.xml");
                    if ((Int32)APS_Define.ERR_NoError == ret)
                    {
                        m_bEnable = true;
                        return true;
                    }
                    else
                    {
                        m_bEnable = false;
                        string str1 = "运动控制卡8254读取配置文件失败, result = {0}";
                        if (LanguageMgr.GetInstance().LanguageID != 0)
                        {
                            str1 = "The motion control card 8254 failed to read the configuration file, result = {0}";
                        }
                        //WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,运动控制卡8254读取配置文件失败, result = {0}", ret));
                        WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, m_nCardIndex.ToString(),
                            string.Format(str1, ret));

                        return false;
                    }
                }
                else
                {
                    m_bEnable = false;

                    string str1 = "运动控制卡8254初始化失败! result = {0}";
                    if (LanguageMgr.GetInstance().LanguageID != 0)
                    {
                        str1 = "Initialization of motion control card 8254 failed! Result = {0}";
                    }
                    //WarningMgr.GetInstance().Error(string.Format("30100,ERR-XYT, 运动控制卡8254初始化失败! result = {0}", ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, m_nCardIndex.ToString(),
                        string.Format(str1, ret));

                    return false;
                }
            }
            catch (Exception e)
            {
                m_bEnable = false;
                string str1 = "控制卡8254初始化失败";
                if (LanguageMgr.GetInstance().LanguageID != 0)
                {
                    str1 = "Initialization of control card 8254 failed";
                }
                System.Windows.Forms.MessageBox.Show(e.Message, str1, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// 关闭轴卡
        /// </summary>
        /// <returns></returns>
        public override bool DeInit()
        {
            int ret = APS168.APS_close();
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    string str1 = "8254板卡库文件关闭出错! result = {0}";
                    if (LanguageMgr.GetInstance().LanguageID != 0)
                    {
                        str1 = "8254 board card library file close error! Result = {0}";
                    }
                    //WarningMgr.GetInstance().Error(string.Format("30102,ERR-XYT,8254板卡库文件关闭出错! result = {0}", ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_DeInit, m_nCardIndex.ToString(),
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
            int ret = APS168.APS_set_servo_on(nAxisNo, 1);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30103,ERR-XYT,8254 Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_ServoOn, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret));

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
            int ret = APS168.APS_set_servo_on(nAxisNo, 0);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30104,ERR-XYT,8254 Card Axis {0} servo off Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_ServoOff, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Card Axis {0} servo off Error,result = {1}", nAxisNo, ret));

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
            if ((GetMotionIoState(nAxisNo) & (0x01 << 7)) == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">回原点参数, 对于8254，此参数代表回原点的方向</param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nParam)
        {
            int dir = 1, mode = 0, ez = 0;

            switch ((HomeMode)nParam)
            {
                case HomeMode.ORG_P:
                case HomeMode.ORG_P_EZ:
                    dir = 0;
                    mode = 0;
                    if (nParam == (int)HomeMode.ORG_P_EZ)
                    {
                        ez = 1;
                    }

                    break;

                case HomeMode.ORG_N:
                case HomeMode.ORG_N_EZ:
                    dir = 1;
                    mode = 0;
                    if (nParam == (int)HomeMode.ORG_N_EZ)
                    {
                        ez = 1;
                    }
                    break;

                case HomeMode.PEL:
                case HomeMode.PEL_EZ:
                    dir = 0;
                    mode = 1;
                    if (nParam == (int)HomeMode.PEL_EZ)
                    {
                        ez = 1;
                    }
                    break;

                case HomeMode.MEL:
                case HomeMode.MEL_EZ:
                    dir = 1;
                    mode = 1;
                    if (nParam == (int)HomeMode.MEL_EZ)
                    {
                        ez = 1;
                    }
                    break;

                case HomeMode.EZ_PEL:
                    dir = 0;
                    mode = 2;
                    break;

                case HomeMode.EZ_MEL:
                    dir = 1;
                    mode = 2;
                    break;

                case HomeMode.CARD:
                    {
                        //从卡中读取回原点方式
                        APS168.APS_get_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_MODE, ref mode);
                        APS168.APS_get_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_DIR, ref dir);
                        APS168.APS_get_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_EZA, ref ez);
                    }
                    break;

                default:
                    if (m_bEnable)
                    {
                        //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,8254 Axis {0} HomeMode Error,result = {1}", nAxisNo));
                        WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                            string.Format("8254 Axis {0} HomeMode Error,result = {1}", nAxisNo));

                    }
                    return false;
            }

            APS168.APS_set_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_MODE, mode);
            APS168.APS_set_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_DIR, dir);
            APS168.APS_set_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_EZA, ez);
            Thread.Sleep(50);
            int ret = APS168.APS_home_move(nAxisNo);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,8254 Axis {0} Home Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Axis {0} Home Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">回原点模式</param>
        /// <param name="vm">最大速度</param>
        /// <param name="vo">爬行速度</param>
        /// <param name="acc">加速时间</param>
        /// <param name="dec">减速时间</param>
        /// <param name="offset">原点偏移</param>
        /// <param name="sFac">平滑因子</param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nParam, double vm, double vo, double acc, double dec, double offset = 0, double sFac = 0)
        {
            int dir = 1, mode = 0, ez = 0;

            switch ((HomeMode)nParam)
            {
                case HomeMode.ORG_P:
                case HomeMode.ORG_P_EZ:
                    dir = 0;
                    mode = 0;
                    if (nParam == (int)HomeMode.ORG_P_EZ)
                    {
                        ez = 1;
                    }

                    break;

                case HomeMode.ORG_N:
                case HomeMode.ORG_N_EZ:
                    dir = 1;
                    mode = 0;
                    if (nParam == (int)HomeMode.ORG_N_EZ)
                    {
                        ez = 1;
                    }
                    break;

                case HomeMode.PEL:
                case HomeMode.PEL_EZ:
                    dir = 0;
                    mode = 1;
                    if (nParam == (int)HomeMode.PEL_EZ)
                    {
                        ez = 1;
                    }
                    break;

                case HomeMode.MEL:
                case HomeMode.MEL_EZ:
                    dir = 1;
                    mode = 1;
                    if (nParam == (int)HomeMode.MEL_EZ)
                    {
                        ez = 1;
                    }
                    break;

                case HomeMode.EZ_PEL:
                    dir = 0;
                    mode = 2;
                    break;

                case HomeMode.EZ_MEL:
                    dir = 1;
                    mode = 2;
                    break;

                case HomeMode.CARD:
                    {
                        //从卡中读取回原点方式
                        APS168.APS_get_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_MODE, ref mode);
                        APS168.APS_get_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_DIR, ref dir);
                        APS168.APS_get_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_EZA, ref ez);
                    }
                    break;

                default:
                    if (m_bEnable)
                    {
                        //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,8254 Axis {0} HomeMode Error,result = {1}", nAxisNo));
                        WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                            string.Format("8254 Axis {0} HomeMode Error,result = {1}", nAxisNo));
                    }

                    return false;
            }

            APS168.APS_set_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_MODE, mode);
            APS168.APS_set_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_DIR, dir);
            APS168.APS_set_axis_param(nAxisNo, (Int32)APS_Define.PRA_HOME_EZA, ez);

            APS168.APS_set_axis_param_f(nAxisNo, (Int32)APS_Define.PRA_HOME_CURVE, sFac); //Set s-factor to 0.5 

            APS168.APS_set_axis_param_f(nAxisNo, (Int32)APS_Define.PRA_HOME_ACC, vm / acc); //Set homing acceleration rate 

            APS168.APS_set_axis_param_f(nAxisNo, (Int32)APS_Define.PRA_HOME_VM, vm); //Set homing maximum velocity. 
            APS168.APS_set_axis_param_f(nAxisNo, (Int32)APS_Define.PRA_HOME_VO, vo); //Set homing leave home velocity 
            APS168.APS_set_axis_param_f(nAxisNo, (Int32)APS_Define.PRA_HOME_SHIFT, offset);

            Thread.Sleep(50);
            int ret = APS168.APS_home_move(nAxisNo);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,8254 Axis {0} Home Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Axis {0} Home Error,result = {1}", nAxisNo, ret));

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
            int ret = APS168.APS_absolute_move(nAxisNo, nPos, nSpeed);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,8254 Axis {0} AbsMove Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Axis {0} AbsMove Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 带所有速度参数的绝对位置移动
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
            ASYNCALL p = new ASYNCALL(); //A waiting call 

            //通过加减速时间计算加减速度
            acc = vm / acc;
            dec = vm / dec;

            int ret = APS168.APS_ptp_all(nAxisNo, (Int32)APS_Define.OPT_ABSOLUTE, fPos, vs, vm, ve, acc, dec, sFac, ref p);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,8254 Axis {0} AbsMove Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Axis {0} AbsMove Error,result = {1}", nAxisNo, ret));

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
            int ret = APS168.APS_relative_move(nAxisNo, nPos, nSpeed);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,8254 Axis {0} relative move Error,result is {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Axis {0} relative move Error,result is {1}", nAxisNo, ret));

                }
                return false;
            }
        }


        /// <summary>
        /// 带所有速度参数的相对位置移动
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
            ASYNCALL p = new ASYNCALL(); //A waiting call 

            //通过加减速时间计算加减速度
            acc = vm / acc;
            dec = vm / dec;

            int ret = APS168.APS_ptp_all(nAxisNo, (Int32)APS_Define.OPT_RELATIVE, fOffset, vs, vm, ve, acc, dec, sFac, ref p);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,8254 Axis {0} RelativeMove Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Axis {0} RelativeMove Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }
        /// <summary>
        /// JOG运动
        /// </summary>
        /// <param name="axis">轴号</param>
        /// <param name="bPositive">方向</param>
        /// <param name="bStart">开始标志</param>
        /// <param name="nSpeed">速度</param>
        /// <returns></returns>
        public override bool JogMove(int axis, bool bPositive, int bStart, int nSpeed)
        {
            APS168.APS_set_axis_param(axis, (Int32)APS_Define.PRA_JG_MODE, 0);
            APS168.APS_set_axis_param(axis, (Int32)APS_Define.PRA_JG_DIR, bPositive ? 1 : 0);
            APS168.APS_set_axis_param_f(axis, (Int32)APS_Define.PRA_JG_VM, nSpeed);
            int ret = APS168.APS_jog_start(axis, bStart);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30108,ERR-XYT,8254 Axis {0} jog move Error,result = {1}", axis, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Jog, GetSysAxisNo(axis).ToString(),
                        string.Format("8254 Axis {0} jog move Error,result = {1}", axis, ret));

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
            int ret = APS168.APS_stop_move(nAxisNo);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;

            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30109,ERR-XYT,8254 Card normal stop axis {0} Error, result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Stop, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Card normal stop axis {0} Error, result = {1}", nAxisNo, ret));

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
            int ret = APS168.APS_emg_stop(nAxisNo);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30110,ERR-XYT,8254 Card Emergency stop axis {0} Error, result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_EmgStop, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Card Emergency stop axis {0} Error, result = {1}", nAxisNo, ret));

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
            return APS168.APS_motion_status(nAxisNo);
        }

        /// <summary>
        /// 获取轴卡运动IO信号
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetMotionIoState(int nAxisNo)
        {

            //     Random rnd1 = new Random();

            //     return rnd1.Next();


            return APS168.APS_motion_io_status(nAxisNo);
        }

        /// <summary>
        /// 获取轴的当前位置
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override double GetAixsPos(int nAxisNo)
        {

            int nAxisPos = 0;
            APS168.APS_get_position(nAxisNo, ref nAxisPos);
            return nAxisPos;
        }

        /// <summary>
        /// 轴是否正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns>0:正常信止, -1:未到位 其它:急停,报警等</returns>
        public override int IsAxisNormalStop(int nAxisNo)
        {
            int nRet = APS168.APS_motion_status(nAxisNo);
            //if (nRet == (Int32)APS_Define.ERR_NoError)//函数调用正常返回
            if ((nRet & (Int32)(APS_Define.MTS_NSTP_V)) != 0)//运动完成
            {
                if ((nRet & (Int32)(APS_Define.MTS_ASTP_V)) != 0)//确定错误信息
                {
                    int stop_code = 0;
                    APS168.APS_get_stop_code(nAxisNo, ref stop_code);
                    if (0 == stop_code)  //正常停止
                    {
                        return 0;
                    }
                    if (1 == stop_code)  //急停
                    {
                        Debug.WriteLine("Axis {0} have Emg single \r\n", nAxisNo);
                        return stop_code;
                    }
                    else if (2 == stop_code)  //报警
                    {
                        Debug.WriteLine("Axis {0} have Alm single \r\n", nAxisNo);
                        return stop_code;
                    }
                    else if (3 == stop_code)  //未servo on
                    {
                        Debug.WriteLine("Axis {0} have servo single \r\n", nAxisNo);
                        return stop_code;
                    }
                    else if (4 == stop_code) //正限位   
                    {
                        Debug.WriteLine("Axis {0} have PEL single \r\n", nAxisNo);
                        return stop_code;
                    }
                    else if (5 == stop_code) //负限位
                    {
                        Debug.WriteLine("Axis {0} have MEL single \r\n", nAxisNo);
                        return stop_code;
                    }
                    Debug.WriteLine("Axis {0} stop code is {1} \r\n", nAxisNo, stop_code);

                    return stop_code + 10; //编码6~10保留给IsAxisInPos用

                }
                else
                    return 0;//正常运动完成
            }
            else
                return -1;//未完成
            //}
            //return -1;//调用异常
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
                int nTargetPos = 0;
                int nPos = 0;
                APS168.APS_get_target_position(nAxisNo, ref nTargetPos);
                APS168.APS_get_position(nAxisNo, ref nPos);

                if (Math.Abs(nPos - nTargetPos) > nInPosError)
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
            int ret = APS168.APS_set_position(nAxisNo, 0);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(
                    //    string.Format("30111,ERR-XYT,8254 Card axis {0} SetPosZero Error, result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_SetPos, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Card axis {0} SetPosZero Error, result = {1}", nAxisNo, ret));
                }

                return false;
            }
        }


        /// <summary>
        /// 设置单轴的某一运动参数
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">参数:1:加速度 2:减速度 3:起跳速度 4:结束速度(凌华卡) 5:平滑时间(S曲线) 其它：自定义扩展</param>
        /// <param name="nData">参数值</param>
        /// <returns></returns>
        public override bool SetAxisParam(int nAxisNo, int nParam, int nData)
        {
            int ret = (Int32)APS_Define.ERR_NoError;
            switch (nParam)
            {
                case 1:    //1:加速度              
                    ret = APS168.APS_set_axis_param(nAxisNo, (int)APS_Define.PRA_ACC, nData);
                    break;
                case 2:  // 2:减速度
                    ret = APS168.APS_set_axis_param(nAxisNo, (int)APS_Define.PRA_DEC, nData);
                    break;
                case 3:  //3:起跳速度
                    ret = APS168.APS_set_axis_param(nAxisNo, (int)APS_Define.PRA_VS, nData);
                    break;
                case 4:  //4:结束速度(凌华卡)
                    ret = APS168.APS_set_axis_param(nAxisNo, (int)APS_Define.PRA_VE, nData);
                    break;
                case 5:  //S曲线平滑　PRA_SF == PRA_CURVE
                    ret = APS168.APS_set_axis_param(nAxisNo, (int)APS_Define.PRA_SF, nData);
                    break;
            }

            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(
                    //    string.Format("30112,ERR-XYT,8254 Card axis {0} SetParam Error, result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_SetParam, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Card axis {0} SetParam Error, result = {1}", nAxisNo, ret));
                }

                return false;
            }
        }

        /// <summary>
        /// 速度模式旋转轴
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool VelocityMove(int nAxisNo, int nSpeed)
        {
            int ret = APS168.APS_velocity_move(nAxisNo, nSpeed);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30113,ERR-XYT,8254 Card axis {0} VelocityMove Error, result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Vel, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("8254 Card axis {0} VelocityMove Error, result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        ///此函数8254板卡不提供不使用,回原点内部已经封装好过程处理 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsHomeNormalStop(int nAxisNo)
        {
            return IsAxisNormalStop(nAxisNo);
        }

        /// <summary>
        /// 以当前位置为起始点进行多轴直线插补
        /// </summary>
        /// <param name="nAixsArray">轴数组</param>
        /// <param name="fPosArray">目标点的绝对座标位置</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public override bool AbsLinearMove(ref int[] nAixsArray, ref double[] fPosArray, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            //    int ret =  APS168.APS_absolute_linear_move(nAixsArray.Length, nAixsArray, nPosArray, nMaxSpeed) ;

            double TransPara = 0; //don’t care in buffered mode 
            ASYNCALL p = new ASYNCALL(); //A waiting call 

            //通过加减速时间计算加减速度
            acc = vm / acc;
            dec = vm / dec;

            //绝对位置，不用buffer模式
            int ret = APS168.APS_line_all(nAixsArray.Length, nAixsArray, (Int32)APS_Define.OPT_ABSOLUTE, fPosArray, ref TransPara, vs, vm, ve, acc, dec, sFac, ref p);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(
                    //    string.Format("30114,ERR-XYT,8254 Card axis {0} AbsLinearMove Error, result = {1}", nAixsArray[0], ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsLinearMove",
                        string.Format("8254 Card axis {0} AbsLinearMove Error, result = {1}", nAixsArray[0], ret));
                }

                return false;
            }
        }

        /// <summary>
        /// 以当前位置为起始点进行多轴直线插补
        /// </summary>
        /// <param name="nAixsArray">轴数组,第一个轴为主轴，设定加速度等参数以主轴为基准</param>
        /// <param name="fPosOffsetArray">目标点的相对座标位置</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public override bool RelativeLinearMove(ref int[] nAixsArray, ref double[] fPosOffsetArray, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            //    int ret = APS168.APS_relative_linear_move(nAixsArray.Length, nAixsArray, nPosArray, nMaxSpeed);
            double TransPara = 0; //don’t care in buffered mode 
            ASYNCALL p = new ASYNCALL(); //A waiting call 

            //通过加减速时间计算加减速度
            acc = vm / acc;
            dec = vm / dec;

            //相对位置，不用buffer模式
            int ret = APS168.APS_line_all(nAixsArray.Length, nAixsArray, (Int32)APS_Define.OPT_RELATIVE, fPosOffsetArray, ref TransPara, vs, vm, ve, acc, dec, sFac, ref p);
            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(
                    //    string.Format("30115,ERR-XYT,8254 Card axis {0} RelativeLinearMove Error, result = {1}", nAixsArray[0], ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "RelativeLinearMove",
                        string.Format("8254 Card axis {0} RelativeLinearMove Error, result = {1}", nAixsArray[0], ret));
                }

                return false;
            }
        }

        /// <summary>
        /// 以当前点位为起始点进行两轴圆弧插补运动
        /// </summary>
        /// <param name="nAixsArray">轴数组,第一个轴为主轴，设定加速度等参数以主轴为基准</param>
        /// <param name="fCenterArray">圆心的绝对座标位置</param>   
        /// <param name="fEndArray">终点的绝对座标位置</param>   
        /// <param name="Dir">圆弧的方向，　0:正向，　１：负向</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public override bool AbsArcMove(ref int[] nAixsArray, ref double[] fCenterArray, ref double[] fEndArray, int Dir, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            //   int ret = APS168.APS_absolute_arc_move(nAixsArray.Length, nAixsArray, nCenterArray, nMaxSpeed, nAngle);

            double TransPara = 0; //don’t care in buffered mode 
            ASYNCALL p = new ASYNCALL(); //A waiting call 
            //绝对位置，不用buffer模式

            //通过加减速时间计算加减速度
            acc = vm / acc;
            dec = vm / dec;

            int ret = 0;
            if (nAixsArray.Length == 2)
                ret = APS168.APS_arc2_ce_all(nAixsArray, (Int32)APS_Define.OPT_ABSOLUTE, fCenterArray, fEndArray, (short)Dir, ref TransPara, vs, vm, ve, acc, dec, sFac, ref p);
            else
                ret = (Int32)APS_Define.ERR_Over_Current_Spec;

            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(
                    //    string.Format("30116,ERR-XYT,8254 Card axis {0} AbsArcMove Error, result = {1}", nAixsArray[0], ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsArcMove",
                        string.Format("8254 Card axis {0} AbsArcMove Error, result = {1}", nAixsArray[0], ret));
                }

                return false;
            }
        }

        /// <summary>
        /// 以当前点位为起始点，以偏差位置为圆心，进行多轴圆弧插补运动
        /// </summary>
        /// <param name="nAixsArray">轴数组,第一个轴为主轴，设定加速度等参数以主轴为基准</param>
        /// <param name="fCenterOffsetArray">圆心的绝对座标位置</param>   
        /// <param name="fEndOffsetArray">终点的绝对座标位置</param>   
        /// <param name="Dir">圆弧的方向，　0:正向，　１：负向</param>
        /// <param name="vm">最大速度</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sFac">S曲线因子</param>
        /// <returns></returns>
        public override bool RelativeArcMove(ref int[] nAixsArray, ref double[] fCenterOffsetArray, ref double[] fEndOffsetArray, int Dir, double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            //    int ret = APS168.APS_relative_arc_move(nAixsArray.Length, nAixsArray, nCenterOffsetArray, nMaxSpeed, nAngle);
            double TransPara = 0; //don’t care in buffered mode 
            ASYNCALL p = new ASYNCALL(); //A waiting call 
            //绝对位置，不用buffer模式

            //通过加减速时间计算加减速度
            acc = vm / acc;
            dec = vm / dec;

            int ret = 0;
            if (nAixsArray.Length == 2)
                ret = APS168.APS_arc2_ce_all(nAixsArray, (Int32)APS_Define.OPT_RELATIVE, fCenterOffsetArray, fEndOffsetArray, (short)Dir, ref TransPara, vs, vm, ve, acc, dec, sFac, ref p);
            else
                ret = (Int32)APS_Define.ERR_Over_Current_Spec;

            if ((Int32)APS_Define.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(
                    //    string.Format("30117,ERR-XYT,8254 Card axis {0} RelativeArcMove Error, result = {1}", nAixsArray[0], ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "RelativeArcMove",
                        string.Format("8254 Card axis {0} RelativeArcMove Error, result = {1}", nAixsArray[0], ret));
                }

                return false;
            }
        }

        /// <summary>
        /// 配置一个连续运动的缓冲表(2000点的buff需要更新fimeware)
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="nAixsArray">轴号数组</param>
        /// <param name="bAbsolute">true:绝对位置模式，　false:相对位置模式</param>
        /// <returns></returns>
        public override bool ConfigPointTable(int nPointTableIndex, ref int[] nAixsArray, bool bAbsolute)
        {
            int ret = 0;
            int nBoardID = (nAixsArray[0] / m_nAxisNum) - m_nCardIndex + 1;
            //检查是否在同一张卡上
            for (int i = 1; i < nAixsArray.Length; ++i)
            {
                if (nBoardID != (nAixsArray[i] / m_nAxisNum - m_nCardIndex + 1))
                {
                    //WarningMgr.GetInstance().Error(
                    // string.Format("30119,ERR-XYT,8254 Card axis {0} ConfigPointTable Error, result = {1}", nAixsArray[i], ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "ConfigPointTable",
                     string.Format("8254 Card axis {0} ConfigPointTable Error, result = {1}", nAixsArray[i], ret));
                    return false;
                }
            }

            m_dicBoard[nPointTableIndex] = nBoardID;
            //Enable point table
            ret = APS168.APS_pt_disable(nBoardID, nPointTableIndex);
            ret = APS168.APS_pt_enable(nBoardID, nPointTableIndex, nAixsArray.Length, nAixsArray);

            //Configuration
            if (bAbsolute)
                ret = APS168.APS_pt_set_absolute(nBoardID, nPointTableIndex); //Set to absolute mode
            else
                ret = APS168.APS_pt_set_relative(nBoardID, nPointTableIndex); //Set to relative mode

            ret = APS168.APS_pt_set_trans_buffered(nBoardID, nPointTableIndex); //Set to buffer mode

            return ret == (Int32)APS_Define.ERR_NoError;
        }

        /// <summary>
        /// 向连续运动的缓冲表插入一个直线插补运动（多轴）
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="positionArray">目标位置数组，需要轴号数组匹配</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="vm">最大速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sf">S曲线因子</param>
        /// <returns></returns>
        public override bool PointTable_Line_Move(int nPointTableIndex, ref double[] positionArray,
            double acc, double dec, double vs, double vm, double ve, double sf)
        {
            int nBoardID = m_dicBoard[nPointTableIndex];


            PTINFO Info = new PTINFO();
            APS168.APS_get_pt_info(nBoardID, nPointTableIndex, ref Info);
            if (Info.Dimension != positionArray.Length)
            {
                //WarningMgr.GetInstance().Error(
                //  string.Format("30119,ERR-XYT,8254 Card PointTable {0} Dimension error,PointTable_Line_Move", nPointTableIndex));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_Line_Move",
                  string.Format("8254 Card PointTable {0} Dimension error,PointTable_Line_Move", nPointTableIndex));
                return false;
            }

            int ret = 0;
            PTSTS Status = new PTSTS();
            ret = APS168.APS_get_pt_status(nBoardID, nPointTableIndex, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //通过加减速时间计算加减速度
                acc = vm / acc;
                dec = vm / dec;

                ret = APS168.APS_pt_set_vm(nBoardID, nPointTableIndex, vm); //Set vm

                ret = APS168.APS_pt_set_acc(nBoardID, nPointTableIndex, acc); //Set acc
                ret = APS168.APS_pt_set_dec(nBoardID, nPointTableIndex, dec); //Set dec
                ret = APS168.APS_pt_set_vs(nBoardID, nPointTableIndex, vs); //Set vs             
                ret = APS168.APS_pt_set_ve(nBoardID, nPointTableIndex, ve); //Set ve
                ret = APS168.APS_pt_set_s(nBoardID, nPointTableIndex, sf); //Set sFac


                PTLINE Line = new PTLINE();
                Line.Dim = Info.Dimension;
                Line.Pos = new Double[] { 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < positionArray.Length; ++i)
                {
                    Line.Pos[i] = positionArray[i];
                }

                //Push 1st point to buffer
                ret = APS168.APS_pt_line(nBoardID, nPointTableIndex, ref Line, ref Status);
                return ret == (Int32)APS_Define.ERR_NoError;
            }
            else
            {

                //WarningMgr.GetInstance().Error(
                //string.Format("30119,ERR-XYT,8254 Card PointTable {0} buffer is full, PointTable_Line_Move", nPointTableIndex));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_Line_Move",
                string.Format("30119,ERR-XYT,8254 Card PointTable {0} buffer is full, PointTable_Line_Move", nPointTableIndex));

                return false;
            }
        }

        /// <summary>
        /// 向连续运动的缓冲表插入一个圆弧插补运动（两轴）
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="centerArray">圆弧中心点位置</param>
        /// <param name="endArray">圆弧结束点位置</param>
        /// <param name="dir">圆弧的方向, 0:顺时针，-1:逆时针</param>
        /// <param name="acc">加速度</param>
        /// <param name="dec">减速度</param>
        /// <param name="vs">起始速度</param>
        /// <param name="vm">最大速度</param>
        /// <param name="ve">结束速度</param>
        /// <param name="sf">S曲线因子</param>
        /// <returns></returns>
        public override bool PointTable_ArcE_Move(int nPointTableIndex, ref double[] centerArray, ref double[] endArray, short dir,
                     double acc, double dec, double vs, double vm, double ve, double sf)
        {
            int nBoardID = m_dicBoard[nPointTableIndex];

            PTINFO Info = new PTINFO();
            APS168.APS_get_pt_info(nBoardID, nPointTableIndex, ref Info);
            if (Info.Dimension != centerArray.Length)
            {
                //WarningMgr.GetInstance().Error(
                //     string.Format("30119,ERR-XYT,8254 Card PointTable {0} Dimension error,PointTable_ArcE_Move", nPointTableIndex));

                WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_ArcE_Move",
                     string.Format("8254 Card PointTable {0} Dimension error,PointTable_ArcE_Move", nPointTableIndex));

                return false;
            }

            int ret = 0;
            PTSTS Status = new PTSTS();
            ret = APS168.APS_get_pt_status(nBoardID, nPointTableIndex, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //通过加减速时间计算加减速度
                acc = vm / acc;
                dec = vm / dec;

                ret = APS168.APS_pt_set_vm(nBoardID, nPointTableIndex, vm); //Set vm

                ret = APS168.APS_pt_set_acc(nBoardID, nPointTableIndex, acc); //Set acc
                ret = APS168.APS_pt_set_dec(nBoardID, nPointTableIndex, dec); //Set dec
                ret = APS168.APS_pt_set_vs(nBoardID, nPointTableIndex, vs); //Set vs            
                ret = APS168.APS_pt_set_ve(nBoardID, nPointTableIndex, ve); //Set ve
                ret = APS168.APS_pt_set_s(nBoardID, nPointTableIndex, sf); //Set sFac


                PTA2CE arc = new PTA2CE();
                arc.Center = centerArray;
                arc.End = endArray;
                arc.Dir = dir;

                arc.Index = new byte[Info.AxisArr.Length];
                for (int i = 0; i < Info.AxisArr.Length; ++i)
                    arc.Index[i] = (byte)(Info.AxisArr[i]);

                //Push 1st point to buffer
                ret = APS168.APS_pt_arc2_ce(nBoardID, nPointTableIndex, ref arc, ref Status);
                return ret == (Int32)APS_Define.ERR_NoError;
            }
            else
            {
                //WarningMgr.GetInstance().Error(
                //    string.Format("30119,ERR-XYT,8254 Card PointTable {0} buffer is full,PointTable_Arc_Move", nPointTableIndex));

                WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_ArcE_Move",
                    string.Format("8254 Card PointTable {0} buffer is full,PointTable_Arc_Move", nPointTableIndex));

                return false;
            }
        }

        /// <summary>
        /// 向连续运动缓冲表插入一个与运动同步的IO控制(IO控制在运动之前)
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="nChannel">IO点的序号</param>
        /// <param name="bOn">1：on, 0: off</param>
        /// <returns></returns>
        public override bool PointTable_IO(int nPointTableIndex, int nChannel, int bOn)
        {
            int nBoardID = m_dicBoard[nPointTableIndex];
            int ret = 0;

            if (nChannel > 16)
                nChannel -= 16;
            else
                nChannel += 8;

            ret = APS168.APS_pt_ext_set_do_ch(nBoardID, nPointTableIndex, nChannel - 1, bOn); //Set Do channel 0 to on
            return ret == (Int32)APS_Define.ERR_NoError;
        }
        /// <summary>
        /// 启动或停止一个连续运动
        /// </summary>
        /// <param name="nPointTableIndex">连续运动列表的序号</param>
        /// <param name="bStart">true:开始运行, false:停止运行</param>
        /// <returns></returns>
        public override bool PointTable_Start(int nPointTableIndex, bool bStart)
        {
            int nBoardID = m_dicBoard[nPointTableIndex];
            int ret = 0;
            if (bStart)
                ret = APS168.APS_pt_start(nBoardID, nPointTableIndex);
            else
                ret = APS168.APS_pt_stop(nBoardID, nPointTableIndex);

            return ret == (Int32)APS_Define.ERR_NoError;

        }

        /// <summary>
        /// 确定连续运动列表的BUFF是否已满
        /// </summary>
        /// <param name="nPointTableIndex"></param>
        /// <returns></returns>
        public override bool PointTable_IsIdle(int nPointTableIndex)
        {
            int nBoardID = m_dicBoard[nPointTableIndex];
            int ret = 0;

            PTSTS Status = new PTSTS();
            ret = APS168.APS_get_pt_status(nBoardID, nPointTableIndex, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
                return true;
            else
                return false;
        }

        /// <summary>
        /// 向缓冲区插入一个延时指令
        /// </summary>
        /// <param name="nPointTableIndex"></param>
        /// <param name="nMillsecond">需要延时的毫秒值</param>
        /// <returns></returns>
        public override bool PointTable_Delay(int nPointTableIndex, int nMillsecond)
        {
            int nBoardID = m_dicBoard[nPointTableIndex];
            int ret = 0;

            PTSTS Status = new PTSTS();
            ret = APS168.APS_get_pt_status(nBoardID, nPointTableIndex, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                PTDWL Prof = new PTDWL();
                Prof.DwTime = nMillsecond;
                ret = APS168.APS_pt_dwell(nBoardID, nPointTableIndex, ref Prof, ref Status);
                return ret == (Int32)APS_Define.ERR_NoError;

            }
            else
                return false;
        }

        /// <summary>
        /// 点位表连续运动样例，支持2000个点的BUFFER(需要刷新fimeware),可以运动中插入IO控制
        /// 支持插入直线和圆弧线段，同时支持设定段与段之间的过渡速度，关联的函数太多，无法封装
        /// </summary>
        /// <param name="position"></param>
        public void pointTable_Move(double[] position)
        {
            int boardId = 1;
            int ptbId = 0;          //Point table id 0
            int dimension = 2;      //2D point table  Must be two axis
            int[] AxisID = new int[] { 0, 2 };
            double[] positionArray = position;
            PTSTS Status = new PTSTS();
            PTLINE Line = new PTLINE();
            // PTA2CA Arc2 = new PTA2CA();

            Int32 doChannel = 0; //Do channel 0
            //Int32 doOn = 0;
            Int32 doOff = 1;
            Int32 i = 0;
            Int32 ret = 0;

            //Check servo on or not
            for (i = 0; i < 3; i++)
            {
                ret = APS168.APS_set_servo_on(i, 1);
            }

            Thread.Sleep(500); // Wait stable.

            //Enable point table
            ret = APS168.APS_pt_disable(boardId, ptbId);
            ret = APS168.APS_pt_enable(boardId, ptbId, dimension, AxisID);

            //Configuration
            ret = APS168.APS_pt_set_absolute(boardId, ptbId); //Set to absolute mode
            ret = APS168.APS_pt_set_trans_buffered(boardId, ptbId); //Set to buffer mode
            ret = APS168.APS_pt_set_acc(boardId, ptbId, 10000); //Set acc
            ret = APS168.APS_pt_set_dec(boardId, ptbId, 10000); //Set dec

            //Get status
            //BitSts;	//b0: Is PTB work? [1:working, 0:Stopped]
            //b1: Is point buffer full? [1:full, 0:not full]
            //b2: Is point buffer empty? [1:empty, 0:not empty]
            //b3~b5: reserved

            //Get status
            ret = APS168.APS_get_pt_status(boardId, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 2nd move profile
                ret = APS168.APS_pt_set_vm(boardId, ptbId, 12000); //Set vm to 12000
                ret = APS168.APS_pt_set_ve(boardId, ptbId, 6000); //Set ve to 6000

                //Set do command to sync with 1st point
                ret = APS168.APS_pt_ext_set_do_ch(boardId, ptbId, doChannel, doOff); //Set Do channel 0 to on

                //Set pt line data
                Line.Dim = 2;
                Line.Pos = new Double[] { positionArray[0], positionArray[0], 0, 0, 0, 0 };

                //Push 1st point to buffer
                ret = APS168.APS_pt_line(boardId, ptbId, ref Line, ref Status);
            }
            Thread.Sleep(20);

            //Get status
            ret = APS168.APS_get_pt_status(boardId, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 2nd move profile
                ret = APS168.APS_pt_set_vm(boardId, ptbId, 12000); //Set vm to 12000
                ret = APS168.APS_pt_set_ve(boardId, ptbId, 6000); //Set ve to 6000

                //Set do command to sync with 2nd point
                ret = APS168.APS_pt_ext_set_do_ch(boardId, ptbId, doChannel, doOff); //Set Do channel 0 to on

                //Set pt line data
                Line.Dim = 2;
                Line.Pos = new Double[] { positionArray[1], positionArray[1], 0, 0, 0, 0 };

                //Push 2nd point to buffer
                ret = APS168.APS_pt_line(boardId, ptbId, ref Line, ref Status);
            }
            Thread.Sleep(20);

            //Get status
            ret = APS168.APS_get_pt_status(boardId, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 2nd move profile
                ret = APS168.APS_pt_set_vm(boardId, ptbId, 12000); //Set vm to 12000
                ret = APS168.APS_pt_set_ve(boardId, ptbId, 6000); //Set ve to 6000

                //Set do command to sync with 
                ret = APS168.APS_pt_ext_set_do_ch(boardId, ptbId, doChannel, doOff); //Set Do channel 0 to on

                //Set pt line data
                Line.Dim = 2;
                Line.Pos = new Double[] { positionArray[2], positionArray[2], 0, 0, 0, 0 };

                //Push point to buffer
                ret = APS168.APS_pt_line(boardId, ptbId, ref Line, ref Status);
            }
            Thread.Sleep(20);

            //Get status
            ret = APS168.APS_get_pt_status(boardId, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 2nd move profile
                ret = APS168.APS_pt_set_vm(boardId, ptbId, 12000); //Set vm to 12000
                ret = APS168.APS_pt_set_ve(boardId, ptbId, 6000); //Set ve to 6000

                //Set do command to sync with 4th point
                ret = APS168.APS_pt_ext_set_do_ch(boardId, ptbId, doChannel, doOff); //Set Do channel 0 to on

                //Set pt line data
                Line.Dim = 2;
                Line.Pos = new Double[] { positionArray[3], positionArray[3], 0, 0, 0, 0 };

                //Push 2nd point to buffer
                ret = APS168.APS_pt_line(boardId, ptbId, ref Line, ref Status);
            }
            Thread.Sleep(20);

            //Get status
            ret = APS168.APS_get_pt_status(boardId, ptbId, ref Status);
            if ((Status.BitSts & 0x02) == 0) //Buffer is not Full
            {
                //Set 4th move profile
                ret = APS168.APS_pt_set_vm(boardId, ptbId, 12000); //Set vm to 12000
                ret = APS168.APS_pt_set_ve(boardId, ptbId, 500); //Set ve to 500

                //Set do command to sync with 5th point
                ret = APS168.APS_pt_ext_set_do_ch(boardId, ptbId, doChannel, doOff); //Set Do channel 0 to on

                //Set pt line data
                Line.Dim = 2;
                Line.Pos = new Double[] { positionArray[4], positionArray[4], 0, 0, 0, 0 };

                //Push 4th point to buffer
                ret = APS168.APS_pt_line(boardId, ptbId, ref Line, ref Status);
            }
            Thread.Sleep(20);

            ret = APS168.APS_pt_start(boardId, ptbId);
        }

        /// <summary>
        /// 启用软件正限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public override bool SetSPELEnable(int nAxisNo, bool bEnable)
        {
            int ret = APS168.APS_set_axis_param(nAxisNo, (int)APS_Define.PRA_SPEL_EN, bEnable ? 1 : 0);

            return ret == (int)APS_Define.ERR_NoError;
        }

        /// <summary>
        /// 启用软件负限位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        public override bool SetSMELEnable(int nAxisNo, bool bEnable)
        {
            int ret = APS168.APS_set_axis_param(nAxisNo, (int)APS_Define.PRA_SMEL_EN, bEnable ? 1 : 0);

            return ret == (int)APS_Define.ERR_NoError;
        }

        /// <summary>
        /// 设置软件正限位位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override bool SetSPELPos(int nAxisNo, double pos)
        {
            int ret = APS168.APS_set_axis_param_f(nAxisNo, (int)APS_Define.PRA_SPEL_POS, pos);

            return ret == (int)APS_Define.ERR_NoError;
        }

        /// <summary>
        /// 设置软件负限位位置
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override bool SetSMELPos(int nAxisNo, double pos)
        {
            int ret = APS168.APS_set_axis_param_f(nAxisNo, (int)APS_Define.PRA_SMEL_POS, pos);

            return ret == (int)APS_Define.ERR_NoError;
        }
    }
}
