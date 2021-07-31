using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTool;
using gts;
using System.Threading;
using System.Runtime.InteropServices;

namespace MotionIO
{
    class Motion_GTS : Motion
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
        /// 
        public Motion_GTS(int nCardIndex, string strName, int nMinAxisNo, int nMaxAxisNo)
            :base(nCardIndex-1, strName, nMinAxisNo, nMaxAxisNo)
        {
            m_bEnable = false;
        }

        /// <summary>
        /// 板卡初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            string str1 = "第{0}张运动控制卡GTS加载配置失败! ";
            string str2 = "第{0}张运动控制卡GTS初始化打开失败! ";
            if (LanguageMgr.GetInstance().LanguageID != 0)
            {
                str1 = "GTS loading configuration of the {0} motion control card failed!";
                str2 = "GTS initialization of the {0} motion control card failed to open!";
            }

            if (0 == mc.GT_Open((short)m_nCardIndex, 0, 1))
            {
                mc.GT_Reset((short)m_nCardIndex);
                if (0 == mc.GT_LoadConfig((short)m_nCardIndex, "GTS400_" + m_nCardIndex.ToString() + ".cfg"))
                {
                    m_bEnable = true;
                    return true;// 0 == gts.mc.GT_SetCardNo((short)m_nCardIndex);
                }
                else
                {
                    m_bEnable = false;
                    //WarningMgr.GetInstance().Error(string.Format("30100,ERR-XYT, 第{0}张运动控制卡GTS加载配置失败! ", m_nCardIndex));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init,m_nCardIndex.ToString(),
                        string.Format(str1, m_nCardIndex));

                    return false;
                }
            }
            else
            {
                m_bEnable = false;
                //WarningMgr.GetInstance().Error(string.Format("30100,ERR-XYT, 第{0}张运动控制卡GTS初始化打开失败! ", m_nCardIndex));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init,m_nCardIndex.ToString(),
                    string.Format(str2, m_nCardIndex));

                return false;
            }
        }

        /// <summary>
        /// 板卡关闭
        /// </summary>
        /// <returns></returns>
        public override bool DeInit()
        {
            return 0 == mc.GT_Close((short)m_nCardIndex);
        }

        /// <summary>
        /// 伺服轴上电
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool ServoOn(int nAxisNo)
        {
            nAxisNo += 1;
            if (0 == gts.mc.GT_AxisOn((short)m_nCardIndex, (short)nAxisNo))
            {
                mc.GT_ClrSts((short)m_nCardIndex,(short)nAxisNo, 1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 伺服轴下电
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool ServoOff(int nAxisNo)
        {
            nAxisNo += 1;
            return 0 == mc.GT_AxisOff((short)m_nCardIndex,(short)nAxisNo);
        }

        /// <summary>
        /// 获取伺服上电状态
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool GetServoState(int nAxisNo)
        {
            nAxisNo += 1;
            int sts = 0;
            uint clk = 0;
            if (0 == gts.mc.GT_GetSts((short)m_nCardIndex, (short)nAxisNo, out sts, 1, out clk))
            {
                return 0 != (sts & (1 << 9));
            }
            return true;
        }

        /// <summary>
        /// 采用自动回原点方式回原点
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nParam">回原点模式</param>
        /// <returns></returns>
        public　override bool Home(int nAxisNo, int nParam)
        {
            int nAxis = nAxisNo + 1;
            mc.GT_ClrSts((short)m_nCardIndex, (short)nAxis, 1);

            uint clk = 0;
            int sts;

            string str1 = "运动控制卡GTS400未使能";
            string str2 = "运动控制卡GTS400急停触发";
            string str3 = "运动控制卡GTS400伺服报警";
            if (LanguageMgr.GetInstance().LanguageID != 0)
            {
                str1 = "Motion control card GTS400 not enabled";
                str2 = "Motion control card GTS400 emergency stop trigger";
                str3 = "Motion control card GTS400 servo alarm";
            }
            //此do-while循环可能不需要添加，需要确认固高卡的smart home是否能够自已撞限位再回原点
            do
            {
                mc.GT_GetSts((short)m_nCardIndex, (short)nAxis, out sts, 1, out clk);
                if ((sts & 0x200) == 0)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,运动控制卡GTS400未使能"));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format(str1));

                    return false;
                }
                if ((sts & 0x100) != 0)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,运动控制卡GTS400急停触发"));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format(str2));

                    return false;
                }
                if ((sts & 0x2) != 0)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,运动控制卡GTS400伺服报警"));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format(str3));

                    return false;
                }
                //还在运动中
                if ((sts & 0x400) != 0)
                {
                    Thread.Sleep(10);
                    continue;
                }
                //判断是否在限位位置,如果在限位位置需要先离开限位

                if (nParam == 0 && (sts & 0x20) != 0)
                {
                    //正限位，向负方向移动
                    RelativeMove(nAxisNo,-1000,10);                  
                }
                else if (nParam == 0)
                {
                    break;
                }
                if (nParam == 1 && (sts & 0x40) != 0)
                {
                    //负限位，向正方向移动
                    RelativeMove(nAxisNo, 1000, 10);
                }
                else if(nParam == 1)
                {
                    break;
                }

            } while (true);


            mc.GT_ClrSts((short)m_nCardIndex, (short)nAxis, 1);
            mc.GT_ZeroPos((short)m_nCardIndex, (short)nAxis, 1);

            //设置回原点参数
            mc.THomePrm prm = new mc.THomePrm();
            mc.GT_GetHomePrm((short)m_nCardIndex, (short)nAxis, out prm);

            short movedir = 1, mode = 0, indexdir = prm.indexDir;

            switch ((HomeMode)nParam)
            {
                case HomeMode.ORG_P:
                    movedir = 1;
                    mode = mc.HOME_MODE_LIMIT_HOME;
                    break;

                case HomeMode.ORG_N:
                    movedir = -1;
                    mode = mc.HOME_MODE_LIMIT_HOME;
                    break;

                case HomeMode.PEL:     
                    movedir = 1;
                    mode = mc.HOME_MODE_LIMIT;

                    break;

                case HomeMode.MEL:        
                    movedir = -1;
                    mode = mc.HOME_MODE_LIMIT;

                    break;
                case HomeMode.ORG_P_EZ:
                    movedir = 1;
                    indexdir = 1;
                    mode = mc.HOME_MODE_LIMIT_HOME_INDEX;
                    break;

                case HomeMode.ORG_N_EZ:
                    movedir = -1;
                    indexdir = -1;
                    mode = mc.HOME_MODE_LIMIT_HOME_INDEX;
                    break;

                case HomeMode.PEL_EZ:
                    movedir = 1;
                    mode = mc.HOME_MODE_LIMIT_INDEX;
                    indexdir = -1;

                    break;

                case HomeMode.MEL_EZ:
                    movedir = -1;
                    mode = mc.HOME_MODE_LIMIT_INDEX;
                    indexdir = 1;

                    break;

                case HomeMode.EZ_PEL:
                    movedir = 1;
                    indexdir = 1;
                    mode = mc.HOME_MODE_INDEX;
                    break;

                case HomeMode.EZ_MEL:
                    movedir = -1;
                    indexdir = -1;
                    mode = mc.HOME_MODE_INDEX;
                    break;

                case HomeMode.CARD:
                    movedir = prm.moveDir;
                    mode = prm.mode;
                    break;


                default:
                    if (m_bEnable)
                    {
                        WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home,GetSysAxisNo(nAxisNo).ToString(),
                            string.Format("GTS Axis {0} HomeMode Error,result = {1}", nAxisNo));

                    }
                    return false;
            }

            //设定回原点的模式，限位+原点模式
            prm.mode = mode;
            prm.moveDir = movedir;
            prm.indexDir = indexdir;
            prm.edge = 1;  //上升沿
            prm.velHigh = 20;
            prm.velLow = 1;
            prm.acc = 0.1;
            prm.dec = 0.1;

            mc.GT_GoHome((short)m_nCardIndex, (short)nAxis, ref prm);       
            return true;
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
            int nAxis = nAxisNo + 1;
            mc.GT_ClrSts((short)m_nCardIndex, (short)nAxis, 1);

            uint clk = 0;
            int sts;

            string str1 = "运动控制卡GTS400未使能";
            string str2 = "运动控制卡GTS400急停触发";
            string str3 = "运动控制卡GTS400伺服报警";
            if (LanguageMgr.GetInstance().LanguageID != 0)
            {
                str1 = "Motion control card GTS400 not enabled";
                str2 = "Motion control card GTS400 emergency stop trigger";
                str3 = "Motion control card GTS400 servo alarm";
            }

            //此do-while循环可能不需要添加，需要确认固高卡的smart home是否能够自已撞限位再回原点
            do
            {
                mc.GT_GetSts((short)m_nCardIndex, (short)nAxis, out sts, 1, out clk);
                if ((sts & 0x200) == 0)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,运动控制卡GTS400未使能"));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format(str1));

                    return false;
                }
                if ((sts & 0x100) != 0)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,运动控制卡GTS400急停触发"));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format(str2));

                    return false;
                }
                if ((sts & 0x2) != 0)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,运动控制卡GTS400伺服报警"));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format(str3));

                    return false;
                }
                //还在运动中
                if ((sts & 0x400) != 0)
                {
                    Thread.Sleep(10);
                    continue;
                }
                //判断是否在限位位置,如果在限位位置需要先离开限位

                if (nParam == 0 && (sts & 0x20) != 0)
                {
                    //正限位，向负方向移动
                    RelativeMove(nAxisNo, -1000, 10);
                }
                else if (nParam == 0)
                {
                    break;
                }
                if (nParam == 1 && (sts & 0x40) != 0)
                {
                    //负限位，向正方向移动
                    RelativeMove(nAxisNo, 1000, 10);
                }
                else if (nParam == 1)
                {
                    break;
                }

            } while (true);


            mc.GT_ClrSts((short)m_nCardIndex, (short)nAxis, 1);
            mc.GT_ZeroPos((short)m_nCardIndex, (short)nAxis, 1);

            //设置回原点参数
            mc.THomePrm prm = new mc.THomePrm();
            mc.GT_GetHomePrm((short)m_nCardIndex, (short)nAxis, out prm);

            short movedir = 1, mode = 0, indexdir = prm.indexDir;

            switch ((HomeMode)nParam)
            {
                case HomeMode.ORG_P:
                    movedir = 1;
                    mode = mc.HOME_MODE_LIMIT_HOME;
                    break;

                case HomeMode.ORG_N:
                    movedir = -1;
                    mode = mc.HOME_MODE_LIMIT_HOME;
                    break;

                case HomeMode.PEL:
                    movedir = 1;
                    mode = mc.HOME_MODE_LIMIT;

                    break;

                case HomeMode.MEL:
                    movedir = -1;
                    mode = mc.HOME_MODE_LIMIT;

                    break;
                case HomeMode.ORG_P_EZ:
                    movedir = 1;
                    indexdir = 1;
                    mode = mc.HOME_MODE_LIMIT_HOME_INDEX;
                    break;

                case HomeMode.ORG_N_EZ:
                    movedir = -1;
                    indexdir = -1;
                    mode = mc.HOME_MODE_LIMIT_HOME_INDEX;
                    break;

                case HomeMode.PEL_EZ:
                    movedir = 1;
                    mode = mc.HOME_MODE_LIMIT_INDEX;
                    indexdir = -1;

                    break;

                case HomeMode.MEL_EZ:
                    movedir = -1;
                    mode = mc.HOME_MODE_LIMIT_INDEX;
                    indexdir = 1;

                    break;

                case HomeMode.EZ_PEL:
                    movedir = 1;
                    indexdir = 1;
                    mode = mc.HOME_MODE_INDEX;
                    break;

                case HomeMode.EZ_MEL:
                    movedir = -1;
                    indexdir = -1;
                    mode = mc.HOME_MODE_INDEX;
                    break;

                case HomeMode.CARD:
                    movedir = prm.moveDir;
                    mode = prm.mode;
                    break;


                default:
                    if (m_bEnable)
                    {
                        WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                            string.Format("GTS Axis {0} HomeMode Error,result = {1}", nAxisNo));

                    }
                    return false;
            }

            //设定回原点的模式，限位+原点模式
            prm.mode = mode;
            prm.moveDir = movedir;
            prm.indexDir = indexdir;
            prm.edge = 1; //上升沿
            prm.velHigh = vm / 1000;  // 单位pls / ms
            prm.velLow = vo / 1000;   // 单位pls / ms
            prm.acc = vm / (1000 * 1000 * acc);   //单位：pls / ms^2
            prm.dec = vm / (1000 * 1000 * dec);   //单位：pls / ms^2

            mc.GT_GoHome((short)m_nCardIndex, (short)nAxis, ref prm);
            return true;
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
            nAxisNo += 1;      
            //设置轴跟限误差
            mc.GT_SetAxisBand((short)m_nCardIndex, (short)nAxisNo, 20, 5);
            mc.TTrapPrm trap = new mc.TTrapPrm();

            short rtn;

            trap.acc = 1;
            trap.dec = 1;
            trap.velStart = 0;
            trap.smoothTime = 25;
            
            if (0 == (rtn = mc.GT_ClrSts((short)m_nCardIndex, (short)nAxisNo, 1))
                && 0 == (rtn = mc.GT_PrfTrap((short)m_nCardIndex, (short)nAxisNo))
                && 0 == (rtn = mc.GT_SetTrapPrm((short)m_nCardIndex, (short)nAxisNo, ref trap))
                && 0 == (rtn = mc.GT_SetVel((short)m_nCardIndex, (short)nAxisNo, (double)nSpeed))
                && 0 == (rtn = mc.GT_SetPos((short)m_nCardIndex, (short)nAxisNo, nPos))
                && 0 == (rtn = mc.GT_Update((short)m_nCardIndex, 1 << (nAxisNo - 1))))
            {
                return true;
            }
            else
            {
                string str1 = "运动控制卡GTS400 AbsMove失败, result = {0}";
                if (LanguageMgr.GetInstance().LanguageID != 0)
                {
                    str1 = "Motion control card GTS400 absmove failed, result = {0}";
                }
                //WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,运动控制卡GTS400 AbsMove失败, result = {0}", rtn));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs,GetSysAxisNo(nAxisNo).ToString(),
                    string.Format(str1, rtn));

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
            nAxisNo += 1;
            //设置轴跟限误差
            mc.GT_SetAxisBand((short)m_nCardIndex, (short)nAxisNo, 20, 5);
            mc.TTrapPrm trap = new mc.TTrapPrm();
            trap.acc = (vm - vs) / (1000 * 1000 * acc);
            trap.dec = (vm - ve) / (1000 * 1000 * dec);
            trap.velStart = vs;
            trap.smoothTime = (short)(sFac * 50);
            short rtn;
            if (0 == (rtn = mc.GT_ClrSts((short)m_nCardIndex, (short)nAxisNo, 1))
                && 0 == (rtn = mc.GT_PrfTrap((short)m_nCardIndex, (short)nAxisNo))
                && 0 == (rtn = mc.GT_SetTrapPrm((short)m_nCardIndex, (short)nAxisNo, ref trap))
                && 0 == (rtn = mc.GT_SetVel((short)m_nCardIndex, (short)nAxisNo, (double)vm))
                && 0 == (rtn = mc.GT_SetPos((short)m_nCardIndex, (short)nAxisNo, (int) fPos))
                && 0 == (rtn = mc.GT_Update((short)m_nCardIndex, 1 << (nAxisNo - 1))))
            {
                return true;
            }
            else
            {
                string str1 = "运动控制卡GTS400 AbsMove失败, result = {0}";
                if (LanguageMgr.GetInstance().LanguageID != 0)
                {
                    str1 = "Motion control card GTS400 absmove failed, result = {0}";
                }
                //WarningMgr.GetInstance().Error(string.Format("30106,ERR-XYT,运动控制卡GTS400 AbsMove失败, result = {0}", rtn));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs,GetSysAxisNo(nAxisNo).ToString(),
                    string.Format(str1, rtn));

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
            nAxisNo += 1;
            //设置轴跟限误差
            mc.GT_SetAxisBand((short)m_nCardIndex, (short)nAxisNo, 20, 5);
            mc.TTrapPrm trap = new mc.TTrapPrm();
            trap.acc = 1;
            trap.dec = 1;
            trap.velStart = 0;
            trap.smoothTime = 25;
            int extPos = 0;
            short rtn;
            if (0 == (rtn = mc.GT_ClrSts((short)m_nCardIndex, (short)nAxisNo, 1))
                && 0 == (rtn = mc.GT_PrfTrap((short)m_nCardIndex, (short)nAxisNo))
                && 0 == (rtn = mc.GT_SetTrapPrm((short)m_nCardIndex, (short)nAxisNo, ref trap))
                && 0 == (rtn = mc.GT_SetVel((short)m_nCardIndex, (short)nAxisNo, (double)nSpeed))
                && 0 == (rtn = mc.GT_GetPos((short)m_nCardIndex, (short)nAxisNo, out extPos))
                && 0 == (rtn = mc.GT_SetPos((short)m_nCardIndex, (short)nAxisNo, extPos + nPos))
                && 0 == (rtn = mc.GT_Update((short)m_nCardIndex, 1 << (nAxisNo - 1))))
            {            
                return true;
            }
            else
            {
                string str1 = "运动控制卡GTS400 RelativeMove失败, result = {0}";
                if (LanguageMgr.GetInstance().LanguageID != 0)
                {
                    str1 = "Motion control card GTS400 relativemove failed, result = {0}";
                }
                //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,运动控制卡GTS400 RelativeMove失败, result = {0}", rtn));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel,GetSysAxisNo(nAxisNo).ToString(),
                    string.Format(str1, rtn));

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
            nAxisNo += 1;
            //设置轴跟限误差
            mc.GT_SetAxisBand((short)m_nCardIndex, (short)nAxisNo, 20, 5);
            mc.TTrapPrm trap = new mc.TTrapPrm();
            trap.acc = (vm - vs)/(1000 * 1000 * acc);
            trap.dec = (vm - ve)/(1000 * 1000 * dec);
            trap.velStart = vs;
            trap.smoothTime = (short)(sFac * 50);
            int extPos = 0;
            short rtn;
            if (0 == (rtn = mc.GT_ClrSts((short)m_nCardIndex, (short)nAxisNo, 1))
                && 0 == (rtn = mc.GT_PrfTrap((short)m_nCardIndex, (short)nAxisNo))
                && 0 == (rtn = mc.GT_SetTrapPrm((short)m_nCardIndex, (short)nAxisNo, ref trap))
                && 0 == (rtn = mc.GT_SetVel((short)m_nCardIndex, (short)nAxisNo, vm))
                && 0 == (rtn = mc.GT_GetPos((short)m_nCardIndex, (short)nAxisNo, out extPos))
                && 0 == (rtn = mc.GT_SetPos((short)m_nCardIndex, (short)nAxisNo, extPos + (int)fOffset))
                && 0 == (rtn = mc.GT_Update((short)m_nCardIndex, 1 << (nAxisNo - 1))))
            {
                return true;
            }
            else
            {
                string str1 = "运动控制卡GTS400 RelativeMove失败, result = {0}";
                if (LanguageMgr.GetInstance().LanguageID != 0)
                {
                    str1 = "Motion control card GTS400 relativemove failed, result = {0}";
                }
                //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,运动控制卡GTS400 RelativeMove失败, result = {0}", rtn));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel,GetSysAxisNo(nAxisNo).ToString(),
                    string.Format(str1, rtn));

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
            axis += 1;
            short rtn;
            mc.TJogPrm trap = new mc.TJogPrm();
            if (0 == (rtn = mc.GT_ClrSts((short)m_nCardIndex, (short)axis, 1))
                && 0 == (rtn = mc.GT_PrfJog((short)m_nCardIndex, (short)axis))
                && 0 == (rtn = mc.GT_GetJogPrm((short)m_nCardIndex, (short)axis, out trap)))
            {
                trap.acc = 0.1;
                trap.dec = 0.1;
                if (0 == (rtn = mc.GT_SetJogPrm((short)m_nCardIndex, (short)axis, ref trap))
                    && 0 == (rtn = mc.GT_SetVel((short)m_nCardIndex, (short)axis, bPositive ? (double)nSpeed : (double)-nSpeed))
                    && 0 == (rtn = mc.GT_Update((short)m_nCardIndex, 1 << (axis - 1))))
                {
                    return true;
                }
            }

            string str1 = "运动控制卡GTS400 JogMove失败, result = {0}";
            if (LanguageMgr.GetInstance().LanguageID != 0)
            {
                str1 = "Motion control card GTS400 JogMove failed, result = {0}";
            }

            //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,运动控制卡GTS400 JogMove失败, result = {0}", rtn));
            WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Jog,GetSysAxisNo(axis).ToString(),
                string.Format(str1, rtn));

            return false;           
        }

        /// <summary>
        /// 轴正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override bool StopAxis(int nAxisNo)
        {
            nAxisNo += 1;         
            short rtn = mc.GT_Stop((short)m_nCardIndex, 1 << (nAxisNo - 1), 0);
            if (0 == rtn)
            {
                return true;
            }
            else
            {
                string str1 = "运动控制卡GTS400 StopAxis失败, result = {0}";
                if (LanguageMgr.GetInstance().LanguageID != 0)
                {
                    str1 = "Motion control card GTS400 StopAxis failed, result = {0}";
                }
                //WarningMgr.GetInstance().Error(string.Format("30109,ERR-XYT,运动控制卡GTS400 StopAxis失败, result = {0}", rtn));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Stop,GetSysAxisNo(nAxisNo).ToString(),
                    string.Format(str1, rtn));

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
            nAxisNo += 1;
            short rtn = mc.GT_Stop((short)m_nCardIndex, 1 << (nAxisNo - 1), 1 << (nAxisNo - 1));
            if (0 == rtn)
            {
                return true;
            }
            else
            {
                string str1 = "运动控制卡GTS400 StopEmg失败, result = {0}";
                if (LanguageMgr.GetInstance().LanguageID != 0)
                {
                    str1 = "Motion control card GTS400 StopEmg failed, result = {0}";
                }
                //WarningMgr.GetInstance().Error(string.Format("30110,ERR-XYT,运动控制卡GTS400 StopEmg失败, result = {0}", rtn));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_EmgStop,GetSysAxisNo(nAxisNo).ToString(),
                    string.Format(str1, rtn));

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
            nAxisNo += 1;
            int sts = 0;
            uint clk = 0;
            short rtn;
            if (0 == (rtn = mc.GT_GetSts((short)m_nCardIndex, (short)nAxisNo, out sts, 1, out clk)))
            {
                return (long)sts;
            }
            else
            {
                string str1 = "运动控制卡GTS400 GetMotionState失败, result = {0}";
                if (LanguageMgr.GetInstance().LanguageID != 0)
                {
                    str1 = "Motion control card GTS400 GetMotionState failed, result = {0}";
                }
                //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,运动控制卡GTS400 GetMotionState失败, result = {0}", rtn));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_State,GetSysAxisNo(nAxisNo).ToString(),
                    string.Format(str1, rtn));

                return -1;
            }
        }
        /// <summary>
        /// 获取轴卡运动IO信号
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override long GetMotionIoState(int nAxisNo)
        {
            //0:保留
            //1:alarm
            //2:保留
            //3:保留
            //4:跟随误差
            //5：正限位触发
            //6：负限位触发
            //7：IO平滑停止触发
            //8：IO急停触发
            //9：电机使能标志
            //10：规划运动标志
            //11：电机到位标志
            uint clk = 0;
            int sts;
            long lStandardIo = 0;
            nAxisNo += 1;
            gts.mc.GT_ClrSts((short)m_nCardIndex, (short)nAxisNo, 1);
            if (0 == gts.mc.GT_GetDi((short)m_nCardIndex, gts.mc.MC_HOME, out sts))
            {
                if ((sts & (0x01 << (nAxisNo - 1))) == 0)
                {
                    lStandardIo |= (0x01 << 3); //原点到位存在第4位
                    lStandardIo |= (0x01 << 5); //零位到位存在第6位
                }
            }
            if (0 == mc.GT_GetSts((short)m_nCardIndex, (short)nAxisNo, out sts, 1, out clk))
            {
                //报警、正限位、负限位、原点、急停、零位、到位、励磁

                if ((sts & (0x01 << 1)) != 0)
                    lStandardIo |= (0x01 << 0);

                if ((sts & (0x01 << 5)) != 0)
                    lStandardIo |= (0x01 << 1);

                if ((sts & (0x01 << 6)) != 0)
                    lStandardIo |= (0x01 << 2);

                if ((sts & (0x01 << 8)) != 0)
                    lStandardIo |= (0x01 << 4);

                if ((sts & (0x01 << 11)) != 0)
                    lStandardIo |= (0x01 << 6);

                if ((sts & (0x01 << 9)) != 0)
                    lStandardIo |= (0x01 << 7);
                return lStandardIo;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取轴的当前位置
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns></returns>
        public override double GetAixsPos(int nAxisNo)
        {
            nAxisNo += 1;
            double pos = 0;
            uint clk = 0;
            if (0 == mc.GT_GetEncPos((short)m_nCardIndex, (short)nAxisNo, out pos, 1, out clk))
            {
                return (long)pos;
            }
            return 0;
        }

        /// <summary>
        /// 轴是否正常停止
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns>0:正常信止, -1:未到位 其它:急停,报警等</returns>
        public override int IsAxisNormalStop(int nAxisNo)
        {
            int sts = 0;
            uint clk = 0;
            nAxisNo += 1;
            if (0 == mc.GT_GetSts((short)m_nCardIndex, (short)nAxisNo, out sts, 1, out clk))
            {
                if (0 != (sts & (1 << 1)))
                {
                    return 2;   //驱动器异常报警
                }
                else if (0 != (sts & (1 << 5)))
                {
                    return 4;   //正限位触发
                }
                else if (0 != (sts & (1 << 6)))
                {
                    return 5;   //负限位触发
                }
                else if (0 == (sts & (1 << 9)))
                {
                    return 3;//伺服掉电
                }

                if (0 == (sts & (1 << 10)))
                {
                    return 0;   //正常停止
                }
                return -1;  //正在运动中
            }
            return -1;
        }

        /// <summary>
        /// 判断轴是否到位
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nInPosError"></param>
        /// <returns></returns>
        public override int IsAxisInPos(int nAxisNo, int nInPosError = 1000)
        {
            int nRet = IsAxisNormalStop(nAxisNo);

            nAxisNo += 1;
            if (0 == nRet)　　　//先判断是否为正常停止
            {
                uint clk = 0;
                double prfPos, encPos;
                //如果设置了跟随误差带时，可以使用“电机到位标志来判定”
                if (0 == mc.GT_GetPrfPos((short)m_nCardIndex, (short)nAxisNo, out prfPos, 1, out clk)
                    && 0 == mc.GT_GetEncPos((short)m_nCardIndex, (short)nAxisNo, out encPos, 1, out clk))
                {
                    if (Math.Abs(prfPos - encPos) > nInPosError)
                    {
                        return 6;
                    }
                }
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
            nAxisNo += 1;
            return 0 == mc.GT_ZeroPos((short)m_nCardIndex, (short)nAxisNo, 1);
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
            nAxisNo += 1;
            mc.TTrapPrm trap　= new mc.TTrapPrm();
            mc.GT_GetTrapPrm((short)m_nCardIndex, (short)nAxisNo, out  trap);

            switch (nParam)
            {
                case 1:  //:1:加速度
                    trap.acc = nData;
                    mc.GT_SetTrapPrm((short)m_nCardIndex, (short)nAxisNo, ref trap);
                    break;
                case 2:  //2:减速度
                    trap.dec = nData;
                    mc.GT_SetTrapPrm((short)m_nCardIndex, (short)nAxisNo, ref trap);
                    break;
                case 3:  //3:起跳速度
                    trap.velStart = nData;
                    mc.GT_SetTrapPrm((short)m_nCardIndex, (short)nAxisNo, ref trap);
                    break;
                case 5:  //5:平滑时间(固高卡S曲线)
                    trap.smoothTime = (short)nData;
                    mc.GT_SetTrapPrm((short)m_nCardIndex, (short)nAxisNo, ref trap);
                    break;
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
            nAxisNo += 1;
            //未实现
            return false;
        }

        /// <summary>
        ///判断是否回原点完成 
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <returns>0:回原点完成, -1:未到位 其它:急停,报警等</returns>
        public override int IsHomeNormalStop(int nAxisNo)
        {
            int nAxis = nAxisNo + 1;
            mc.THomeStatus homeStatus = new mc.THomeStatus();
            mc.GT_GetHomeStatus((short)m_nCardIndex, (short)nAxis, out homeStatus);

            if (homeStatus.stage == mc.HOME_STAGE_END)//回原点已完成
            {
                if (homeStatus.error == mc.HOME_ERROR_NONE)
                {
                    Thread.Sleep(100);
                    mc.GT_ZeroPos((short)m_nCardIndex, (short)nAxis, 1);
                    return 0;
                }
                else if (homeStatus.error == mc.HOME_ERROR_DISABLE)
                {
                    return 3; //未使能
                }
                else if (homeStatus.error == mc.HOME_ERROR_ALARM)
                {
                    return 2;//报警
                }
                else
                    return homeStatus.error + 10;  //其它异常
            }
            else
            {
                return -1;           //未完成    
            }
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
        public override bool AbsLinearMove(ref int[] nAixsArray, ref double[] fPosArray, 
            double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {

            short sRtn;
            mc.TCrdPrm crdPrm　=　new mc.TCrdPrm();
            short coordinateIndex = 1;

            crdPrm.dimension =(short) nAixsArray.Length;                        // 建立三维的坐标系
            crdPrm.synVelMax = vm;                      // 坐标系的最大合成速度是: 500 pulse/ms
            crdPrm.synAccMax = acc;                     // 坐标系的最大合成加速度是: 2 pulse/ms^2
            crdPrm.evenTime = (short)sFac;              // 坐标系的最小匀速时间为0
            //轴号可能需要加１,待定
            crdPrm.profile1 = nAixsArray.Length > 0 ? ((short) nAixsArray[0]) : (short)0;                       // 规划器1对应到X轴                       
            crdPrm.profile2 = nAixsArray.Length > 1 ? (short)nAixsArray[1] : (short)0;                        // 规划器2对应到Y轴
            crdPrm.profile3 = nAixsArray.Length > 2 ? (short)nAixsArray[2] : (short)0;                       // 规划器3对应到Z轴
            crdPrm.profile4 = nAixsArray.Length > 3 ? (short)nAixsArray[3] : (short)0; 
            crdPrm.profile5 = nAixsArray.Length > 4 ? (short)nAixsArray[4] : (short)0; 
            crdPrm.profile6 = nAixsArray.Length > 5 ? (short)nAixsArray[5] : (short)0; 
            crdPrm.profile7 = nAixsArray.Length > 6 ? (short)nAixsArray[6] : (short)0; 
            crdPrm.profile8 = nAixsArray.Length > 7 ? (short)nAixsArray[7] : (short)0; 
            crdPrm.setOriginFlag = 1;                    // 需要设置加工坐标系原点位置
            crdPrm.originPos1 = 0;                     // 加工坐标系原点位置在(0,0,0)，即与机床坐标系原点重合
            crdPrm.originPos2 = 0;
            crdPrm.originPos3 = 0;
            crdPrm.originPos4 = 0;
            crdPrm.originPos5 = 0;
            crdPrm.originPos6 = 0;
            crdPrm.originPos7 = 0;
            crdPrm.originPos8 = 0;

            sRtn = mc.GT_SetCrdPrm((short)m_nCardIndex, coordinateIndex, ref crdPrm);
            sRtn = mc.GT_CrdClear((short)m_nCardIndex, coordinateIndex, 0);
            if(sRtn == 0)
            {
                //使用FIFO　0来运动
                if (nAixsArray.Length == 2)
                    sRtn = mc.GT_LnXY((short)m_nCardIndex, coordinateIndex, (int)fPosArray[0], (int)fPosArray[1], vm, acc, ve, 0);
                else if (nAixsArray.Length == 3)
                    sRtn = mc.GT_LnXYZ((short)m_nCardIndex, coordinateIndex, (int)fPosArray[0], (int)fPosArray[1], (int)fPosArray[2], vm, acc, ve, 0);
                else if (nAixsArray.Length == 4)
                    sRtn = mc.GT_LnXYZA((short)m_nCardIndex, coordinateIndex, (int)fPosArray[0], (int)fPosArray[1], (int)fPosArray[2], (int)fPosArray[3], vm, acc, ve, 0);
                sRtn = mc.GT_CrdStart((short)m_nCardIndex, 1, 0);
            }
            //最多4轴插补
            if (sRtn == 0 && nAixsArray.Length < 5)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(
                    //    string.Format("30114,ERR-XYT,GTS Card axis {0} AbsLinearMove Error, result = {1}", nAixsArray[0], sRtn));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsLinearMove",
                        string.Format("GTS Card axis {0} AbsLinearMove Error, result = {1}", nAixsArray[0], sRtn));
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
        public override bool RelativeLinearMove(ref int[] nAixsArray, ref double[] fPosOffsetArray,
            double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            //暂未实现
            return false;
            
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
        public override bool AbsArcMove(ref int[] nAixsArray, ref double[] fCenterArray, ref double[] fEndArray, int Dir,
            double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            short sRtn = 0;

            mc.TCrdPrm crdPrm = new mc.TCrdPrm();
            short coordinateIndex = 1;

            if(nAixsArray.Length == 2)
            {
                crdPrm.dimension = (short)nAixsArray.Length; // 建立二维的坐标系
                crdPrm.synVelMax = vm;                      // 坐标系的最大合成速度是: 500 pulse/ms
                crdPrm.synAccMax = acc;                     // 坐标系的最大合成加速度是: 2 pulse/ms^2
                crdPrm.evenTime = (short)sFac;              // 坐标系的最小匀速时间为0

                //轴号可能需要加１,待定
                crdPrm.profile1 =　(short) nAixsArray[0];    // 规划器1对应到X轴                       
                crdPrm.profile2 = (short)　nAixsArray[1] ;   // 规划器2对应到Y轴
                crdPrm.profile3 = 0;
                crdPrm.profile4 = 0;
                crdPrm.profile5 = 0;
                crdPrm.profile6 = 0;
                crdPrm.profile7 = 0;
                crdPrm.profile8 = 0;
                crdPrm.setOriginFlag = 1;                  // 需要设置加工坐标系原点位置
                crdPrm.originPos1 = 0;                     // 加工坐标系原点位置在(0,0,0)，即与机床坐标系原点重合
                crdPrm.originPos2 = 0;
                crdPrm.originPos3 = 0;
                crdPrm.originPos4 = 0;
                crdPrm.originPos5 = 0;
                crdPrm.originPos6 = 0;
                crdPrm.originPos7 = 0;
                crdPrm.originPos8 = 0;

                sRtn = mc.GT_SetCrdPrm((short)m_nCardIndex, coordinateIndex, ref crdPrm);
                sRtn = mc.GT_CrdClear((short)m_nCardIndex, coordinateIndex, 0);

                if (sRtn == 0)
                {
                    //圆心为相对座标 //使用FIFO　0来运动
                    int nOffsetX = (int)fCenterArray[0] - (int)GetAixsPos(nAixsArray[0]);
                    int nOffsetY = (int)fCenterArray[1] - (int)GetAixsPos(nAixsArray[1]);
                    sRtn = mc.GT_ArcXYC((short)m_nCardIndex, coordinateIndex, (int)fEndArray[0], (int)fEndArray[1], nOffsetX, nOffsetY, (short)Dir, vm, acc, ve, 0);
                    sRtn = mc.GT_CrdStart((short)m_nCardIndex, 1, 0);
                }
            }
             
            //最多2轴插补
            if (sRtn == 0 && nAixsArray.Length == 2)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(
                    //    string.Format("30116,ERR-XYT,GTS Card axis {0} AbsArcMove Error, result = {1}", nAixsArray[0], sRtn));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "AbsArcMove",
                        string.Format("GTS Card axis {0} AbsArcMove Error, result = {1}", nAixsArray[0], sRtn));
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
        public override bool RelativeArcMove(ref int[] nAixsArray, ref double[] fCenterOffsetArray, ref double[] fEndOffsetArray, int Dir,
            double vm, double acc, double dec, double vs = 0, double ve = 0, double sFac = 0)
        {
            //未实现
            return false;            
        }

        /// <summary>
        /// 配置一个连续运动的缓冲表(4096点的buff)
        /// </summary>
        /// <param name="nPointTableIndex">缓冲列表的序号</param>
        /// <param name="nAixsArray">轴号数组</param>
        /// <param name="bAbsolute">true:绝对位置模式，　false:相对位置模式</param>
        /// <returns></returns>
        public override bool ConfigPointTable(int nPointTableIndex, ref int[] nAixsArray, bool bAbsolute)
        {
            int ret = 0;

            int nBoardID = (nAixsArray[0] / m_nAxisNum) ;

            //不在同一张卡上
            for (int i = 1; i < nAixsArray.Length; ++i)
            {
                if (nBoardID != (nAixsArray[i] / m_nAxisNum))
                {
                    //WarningMgr.GetInstance().Error(
                    // string.Format("30119,ERR-XYT,GTS Card axis {0} ConfigPointTable Error, result = {1}", nAixsArray[i], ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "ConfigPointTable",
                     string.Format("GTS Card axis {0} ConfigPointTable Error, result = {1}", nAixsArray[i], ret));
                    return false;
                }
            }

            m_dicBoard[nPointTableIndex] = nBoardID;

            short sRtn;
            mc.TCrdPrm crdPrm = new mc.TCrdPrm();

            crdPrm.dimension = (short)nAixsArray.Length;                        // 建立三维的坐标系
            crdPrm.synVelMax = 500;                      // 坐标系的最大合成速度是: 500 pulse/ms
            crdPrm.synAccMax = 2;                     // 坐标系的最大合成加速度是: 2 pulse/ms^2
            crdPrm.evenTime = (short)0;              // 坐标系的最小匀速时间为0
            //轴号可能需要加１,待定
            crdPrm.profile1 = nAixsArray.Length > 0 ? ((short)nAixsArray[0]) : (short)0;                       // 规划器1对应到X轴                       
            crdPrm.profile2 = nAixsArray.Length > 1 ? (short)nAixsArray[1] : (short)0;                        // 规划器2对应到Y轴
            crdPrm.profile3 = nAixsArray.Length > 2 ? (short)nAixsArray[2] : (short)0;                       // 规划器3对应到Z轴
            crdPrm.profile4 = nAixsArray.Length > 3 ? (short)nAixsArray[3] : (short)0;
            crdPrm.profile5 = nAixsArray.Length > 4 ? (short)nAixsArray[4] : (short)0;
            crdPrm.profile6 = nAixsArray.Length > 5 ? (short)nAixsArray[5] : (short)0;
            crdPrm.profile7 = nAixsArray.Length > 6 ? (short)nAixsArray[6] : (short)0;
            crdPrm.profile8 = nAixsArray.Length > 7 ? (short)nAixsArray[7] : (short)0;
            crdPrm.setOriginFlag = 1;                    // 需要设置加工坐标系原点位置
            crdPrm.originPos1 = 0;                     // 加工坐标系原点位置在(0,0,0)，即与机床坐标系原点重合
            crdPrm.originPos2 = 0;
            crdPrm.originPos3 = 0;
            crdPrm.originPos4 = 0;
            crdPrm.originPos5 = 0;
            crdPrm.originPos6 = 0;
            crdPrm.originPos7 = 0;
            crdPrm.originPos8 = 0;

            sRtn = mc.GT_SetCrdPrm((short)nBoardID, (short) nPointTableIndex, ref crdPrm);
            sRtn = mc.GT_CrdClear((short)nBoardID, (short)nPointTableIndex, 0);

            //是否打开前瞻功能
        //    gts.mc.TCrdData[] crdData = new gts.mc.TCrdData[300];
        //    sRtn = gts.mc.GT_InitLookAhead((short)nBoardID, (short)nPointTableIndex, 0, 5, 1, 200, ref crdData[0]);

            return sRtn == 0;
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
        /// <param name="ve">终点速度</param>
        /// <param name="sf">S曲线因子</param>
        /// <returns></returns>
        public override bool PointTable_Line_Move(int nPointTableIndex, ref double[] positionArray, double acc, double dec, double vs, double vm, double ve, double sf)
        {
            short nBoardID = (short)m_dicBoard[nPointTableIndex];

            short sRtn;
            mc.TCrdPrm crdPrm = new mc.TCrdPrm();
            sRtn = mc.GT_GetCrdPrm((short)nBoardID, (short)nPointTableIndex, out crdPrm);
            if (crdPrm.dimension != positionArray.Length)
            {
                //WarningMgr.GetInstance().Error(
                //  string.Format("30119,ERR-XYT,GTS Card PointTable {0} Dimension error,PointTable_Line_Move", nPointTableIndex));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_Line_Move",
                  string.Format("GTS Card PointTable {0} Dimension error,PointTable_Line_Move", nPointTableIndex));
                return false;
            }

            if (sRtn == 0)
            {
                //使用FIFO　0来运动
                if (positionArray.Length == 2)
                    sRtn = mc.GT_LnXY(nBoardID,(short)nPointTableIndex, (int)positionArray[0], (int)positionArray[1], vm, acc, ve, 0);
                else if (positionArray.Length == 3)
                    sRtn = mc.GT_LnXYZ(nBoardID, (short)nPointTableIndex, (int)positionArray[0], (int)positionArray[1], (int)positionArray[2], vm, acc, ve, 0);
                else if (positionArray.Length == 4)
                    sRtn = mc.GT_LnXYZA(nBoardID,(short)nPointTableIndex, (int)positionArray[0], (int)positionArray[1], (int)positionArray[2], (int)positionArray[3], vm, acc, ve, 0);
               
            }

            if (sRtn == 0) 
            {
                return true;
            }
            else
            {
                //WarningMgr.GetInstance().Error(
                //    string.Format("30119,ERR-XYT,GTS Card PointTable {0}  PointTable_Line_Move error", nPointTableIndex));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_Line_Move",
                    string.Format("GTS Card PointTable {0}  PointTable_Line_Move error", nPointTableIndex));
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
            short nBoardID = (short)m_dicBoard[nPointTableIndex];

            short sRtn;
            mc.TCrdPrm crdPrm = new mc.TCrdPrm();
            sRtn = mc.GT_GetCrdPrm((short)nBoardID, (short)nPointTableIndex, out crdPrm);
            if (crdPrm.dimension != centerArray.Length)
            {
                //WarningMgr.GetInstance().Error(
                //  string.Format("30119,ERR-XYT,GTS Card PointTable {0} Dimension error,PointTable_Line_Move", nPointTableIndex));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_ArcE_Move",
                  string.Format("GTS Card PointTable {0} Dimension error,PointTable_Line_Move", nPointTableIndex));
                return false;
            }
            if (sRtn == 0)//圆弧插补只支持2轴
            {
                //圆心为相对座标，使用FIFO　0来运动
                int nOffsetX = (int)centerArray[0] - (int)GetAixsPos(crdPrm.profile1);
                int nOffsetY = (int)centerArray[1] - (int)GetAixsPos(crdPrm.profile2);
                sRtn = mc.GT_ArcXYC(nBoardID,(short) nPointTableIndex, (int)endArray[0], (int)endArray[1], nOffsetX, nOffsetY, (short)dir, vm, acc, ve, 0);               
            }

            if (sRtn == 0)
            {
                return true;
            }
            else
            {
                //WarningMgr.GetInstance().Error(
                //    string.Format("30119,ERR-XYT,GTS Card PointTable {0} PointTable_ArcE_Move  error", nPointTableIndex));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "PointTable_ArcE_Move",
                    string.Format("GTS Card PointTable {0} PointTable_ArcE_Move  error", nPointTableIndex));
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
            short nBoardID = (short)m_dicBoard[nPointTableIndex];
            short sRtn;
            // 缓存区数字量输出
            sRtn = mc.GT_BufIO(nBoardID, (short) nPointTableIndex,	(ushort)mc.MC_GPO,		
                (ushort)(1 << nChannel),			            // bit0~bit15全部都输出
               (ushort) (bOn == 1 ? (1 << nChannel) : 0),			// 输出的数值为0x55
                0);				// 向坐标系1的FIFO0缓存区传递该数字量输出
            return sRtn == 0;
        }
        /// <summary>
        /// 启动或停止一个连续运动
        /// </summary>
        /// <param name="nPointTableIndex">连续运动列表的序号</param>
        /// <param name="bStart">true:开始运行, false:停止运行</param>
        /// <returns></returns>
        public override bool PointTable_Start(int nPointTableIndex, bool bStart)
        {

            short nBoardID = (short)m_dicBoard[nPointTableIndex];
            short sRtn;

            if (bStart)
            {
                //将前瞻缓存区数据压入运动缓存区
             //   sRtn = mc.GT_CrdData(nBoardID, 1, System.IntPtr.Zero, 0);
                //使用FIFO　0来运动

                sRtn = mc.GT_CrdStart((short)nBoardID, (short)(1 << (nPointTableIndex - 1)), 0);
            }
            else
            {
                mc.TCrdPrm crdPrm = new mc.TCrdPrm();
                sRtn = mc.GT_GetCrdPrm((short)nBoardID, (short)nPointTableIndex, out crdPrm);
                if(crdPrm.profile1 > 0)
                     StopAxis(crdPrm.profile1);
                if (crdPrm.profile2 > 0)
                    StopAxis(crdPrm.profile2);
                if (crdPrm.profile3 > 0)
                    StopAxis(crdPrm.profile3);
                if (crdPrm.profile4 > 0)
                    StopAxis(crdPrm.profile4);
            }

                return sRtn == 0;

        }

        /// <summary>
        /// 确定连续运动列表的BUFF是否已满
        /// </summary>
        /// <param name="nPointTableIndex"></param>
        /// <returns></returns>
        public override bool PointTable_IsIdle(int nPointTableIndex)
        {
            short nBoardID = (short)m_dicBoard[nPointTableIndex];
            short sRtn;
            int space = 0;
            //使用FIFO　0来运作
            sRtn = mc.GT_CrdSpace((short)nBoardID, (short)nPointTableIndex,out space , 0);
            if (sRtn == 0 && space > 0)
            {
                return true;
            }
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
            short nBoardID = (short)m_dicBoard[nPointTableIndex];
            short sRtn;

            //使用FIFO　0来运作
            sRtn = mc.GT_BufDelay((short)nBoardID, (short)nPointTableIndex , (ushort)nMillsecond, 0);
            if (sRtn == 0) 
            {             
                return true;
            }
            else
                return false;
        }
    }
}
