/********************************************************************
	created:	2017/03/03
	filename: 	MOTION_Delta
	author:		
	purpose:	台达运动控制卡的封装类
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using PCI_DMC;
using PCI_DMC_ERR;
using CommonTool;

namespace MotionIO
{
    /// <summary>
    /// 
    /// </summary>
    public class MotionTable
    {
        private const int MAX_AXIS = 16;
        private const int MAX_POS = 2000;

        /// <summary>
        /// 
        /// </summary>
        public ushort nCardID = 0;
        /// <summary>
        /// 
        /// </summary>
        public short nTableNo = 0;

        /// <summary>
        /// 
        /// </summary>
        public int[] nAxisNo;  //运动的轴编号
        private int nPointCount = 0;  //点位计数
        private ushort nDoCount = 0;  //输出点计数
        /// <summary>
        /// 
        /// </summary>
        public ushort[] nNodeID = new ushort[MAX_AXIS];
        /// <summary>
        /// 
        /// </summary>
        public ushort[] nSlotID = new ushort[MAX_AXIS];
        /// <summary>
        /// 
        /// </summary>
        public int[] nPosData = new int[MAX_POS];   //点位数据  
        /// <summary>
        /// 
        /// </summary>
        public uint[] nGroupNo = new uint[MAX_POS];       //点位分组
        /// <summary>
        /// 
        /// </summary>
        public int[] nDoStatus = new int[MAX_POS];   //输出点状态

        /// <summary>
        /// 
        /// </summary>
        public int PointCount { get { return this.nPointCount; } }
        /// <summary>
        /// 
        /// </summary>
        public int DoCount { get { return this.nDoCount; } }


        //nTableNo表编号，取值只能是0或1
        //nAxisNo运动的轴编号，要位于同一张板卡上
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nTableNo"></param>
        /// <param name="nAxisNo"></param>
        public MotionTable(short nTableNo, int[] nAxisNo)
        {
            this.nTableNo = nTableNo > 0 ? (short)1 : (short)0;
            if (nAxisNo.Length < 1)
                return;

            int i = 0;
            foreach (int n in nAxisNo)
            {
                Motion_Delta.DeltaAddr addr = Motion_Delta.GetAxisAddr(n);
                this.nCardID = addr.nCardID;
                this.nNodeID[i] = addr.nCardID;
                this.nSlotID[i] = addr.nCardID;
                i++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addrDO"></param>
        /// <returns></returns>
        public bool AddDO(Motion_Delta.DeltaAddr addrDO)
        {
            short ret = CPCI_DMC.CS_DMC_01_set_tablemotion_ioctl_mapping(addrDO.nCardID, this.nTableNo,
                    this.nDoCount++, addrDO.nNodeID, addrDO.nSlotID, addrDO.nIndex);
            if (0 == ret)
            {
                return true;
            }
            else
            {
                //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta Card {0} set_tablemotion_ioctl_mapping Error,result is {1}", addrDO.nCardID, ret));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, addrDO.nCardID.ToString(),
                    string.Format("Delta Card {0} set_tablemotion_ioctl_mapping Error,result is {1}", addrDO.nCardID, ret));

                return false;
            }
        }

        //运动的轴要在同一张板卡上
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nPos"></param>
        /// <param name="nGroupNo"></param>
        /// <param name="nDoStatus"></param>
        public void AddPoint(int[] nPos, uint nGroupNo, int nDoStatus)
        {
            for (int i = 0; i < this.nAxisNo.Length; ++i)
                this.nPosData[this.nPointCount * this.nAxisNo.Length + i] = nPos[i];

            this.nGroupNo[this.nPointCount] = nGroupNo;
            this.nDoStatus[this.nPointCount] = nDoStatus;
            this.nPointCount++;
        }
    }











    /// <summary>
    /// 台达板卡
    /// </summary>
    public class Motion_Delta : Motion
    {
        /// <summary>
        /// 
        /// </summary>
        public struct DeltaAddr
        {
            /// <summary>
            /// 
            /// </summary>
            public ushort nCardID;
            /// <summary>
            /// 
            /// </summary>
            public ushort nNodeID;
            /// <summary>
            /// 
            /// </summary>
            public ushort nSlotID;
            /// <summary>
            /// 
            /// </summary>
            public ushort nPortID;
            /// <summary>
            /// 
            /// </summary>
            public ushort nIndex;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="nCardID"></param>
            /// <param name="nNodeID"></param>
            /// <param name="nPortID"></param>
            /// <param name="nSlotID"></param>
            /// <param name="nIndex"></param>
            public DeltaAddr(ushort nCardID, ushort nNodeID, ushort nPortID = 0, ushort nSlotID = 0, ushort nIndex = 0)
            {
                this.nCardID = nCardID;
                this.nNodeID = nNodeID;
                this.nSlotID = nSlotID;
                this.nPortID = nPortID;
                this.nIndex = nIndex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public static DeltaAddr GetAxisAddr(int nAxisNo)
        {
            return new DeltaAddr((ushort)(nAxisNo / 100), (ushort)(nAxisNo % 100));
        }

        private short nCardCount = 0;


        /// <summary>
        /// 构造初始化
        /// </summary>
        /// <param name="nCardIndex"></param>
        /// <param name="strName"></param>
        /// <param name="nMinAxisNo"></param>
        /// <param name="nMaxAxisNo"></param>
        public Motion_Delta(int nCardIndex, string strName, int nMinAxisNo, int nMaxAxisNo)
            : base(nCardIndex, strName, nMinAxisNo, nMaxAxisNo)
        {
            //nMinAxisNo = nMinAxisNo - 1;
            m_bEnable = false;
        }

        /// <summary>
        ///轴卡初始化 
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            string str1 = "Delta运动控制卡查找失败, result = {0}";
            string str2 = "Delta运动控制卡初始化失败, result = {0}";
            string str3 = "Delta运动控制卡初始化通讯失败, result = {0}";
            if (LanguageMgr.GetInstance().LanguageID != 0)
            {
                str1 = "Delta motion control card lookup failed, result = {0}";
                str2 = "Delta motion control card initialization failed, result = {0}";
                str3 = "Delta motion control card failed to initialize communication, result = {0}";
            }
            //打开板卡
            short ret = CPCI_DMC.CS_DMC_01_open(ref this.nCardCount); //open card
            if (this.nCardCount <= 0)
            {              
                //WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,Delta运动控制卡查找失败, result = {0}", ret));
                WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init,m_nCardIndex.ToString(),
                    string.Format(str1, ret));

                return false;
            }

            ushort i, card_no = 0;
            for (i = 0; i < this.nCardCount; i++)
            {
                //获得板卡ID
                ret = CPCI_DMC.CS_DMC_01_get_CardNo_seq(i, ref card_no);
                //初始化板卡
                ret = CPCI_DMC.CS_DMC_01_pci_initial(card_no);
                m_nCardIndex = card_no;
                if (ret != 0)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,Delta运动控制卡初始化失败, result = {0}", ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init,m_nCardIndex.ToString(),
                        string.Format(str2, ret));
                    return false;
                }
                //初始化总线
                ret = CPCI_DMC.CS_DMC_01_initial_bus(card_no);
                if (ret != 0)
                {
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init, m_nCardIndex.ToString(),
                        string.Format(str2, ret));
                    return false;
                }
                else
                {
                    ushort DeviceInfo = 0;
                    uint[] SlaveTable = new uint[4];
                    ret = CPCI_DMC.CS_DMC_01_start_ring(card_no, 0);                      //Start communication                      
                    ret = CPCI_DMC.CS_DMC_01_get_device_table(card_no, ref DeviceInfo);   //Get Slave Node ID 
                    ret = CPCI_DMC.CS_DMC_01_get_node_table(card_no, ref SlaveTable[0]);
                    if (ret != 0)
                    {
                        //WarningMgr.GetInstance().Error(string.Format("30101,ERR-XYT,Delta运动控制卡初始化通讯失败, result = {0}", ret));
                        WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Init,m_nCardIndex.ToString(),
                            string.Format(str3, ret));
                        return false;
                    }
                }
            }
            m_bEnable = true;
            return true;
        }

        /// <summary>
        ///关闭轴卡 
        /// </summary>
        /// <returns></returns>
        public override bool DeInit()
        {
            short ret = 0;
            ushort i, card_no = 0;
            for (i = 0; i < this.nCardCount; ++i)
            {
                //获得板卡ID
                ret += CPCI_DMC.CS_DMC_01_get_CardNo_seq(i, ref card_no);
                //重置板卡
                ret += CPCI_DMC.CS_DMC_01_reset_card((ushort)m_nCardIndex);
            }
            CPCI_DMC.CS_DMC_01_close();
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    string str1 = "Delta板卡库文件关闭出错! result = {0}";
                    if (LanguageMgr.GetInstance().LanguageID != 0)
                    {
                        str1 = "Delta board library file close error! Result = {0}";
                    }
                    //WarningMgr.GetInstance().Error(string.Format("30102,ERR-XYT,Delta板卡库文件关闭出错! result = {0}", ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_DeInit,m_nCardIndex.ToString(),
                        string.Format(str1, ret));

                }
                return false;
            }
        }

        /// <summary>
        ///开启使能 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool ServoOn(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            //台达卡上电时对报警状态复位
            short ret = CPCI_DMC.CS_DMC_01_set_ralm(addr.nCardID, addr.nNodeID, addr.nSlotID);
            Thread.Sleep(50);
            ret = CPCI_DMC.CS_DMC_01_ipo_set_svon(addr.nCardID, addr.nNodeID, addr.nSlotID, (ushort)1);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30103,ERR-XYT,Delta Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_ServoOn,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Card Aixs {0} servo on Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        ///断开使能 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool ServoOff(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            short ret = CPCI_DMC.CS_DMC_01_ipo_set_svon(addr.nCardID, addr.nNodeID, addr.nSlotID, (ushort)0);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30103,ERR-XYT,Delta Card Aixs {0} servo off Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_ServoOff,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Card Aixs {0} servo off Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 读取伺服使能状态 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool GetServoState(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            uint MC_done = 0;
            short ret = CPCI_DMC.CS_DMC_01_motion_status(addr.nCardID, addr.nNodeID, addr.nSlotID, ref MC_done);
            if (ret == 0)
            {
                if ((MC_done & (0x01 << 8)) == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        ///回原点
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nMode">回零方式</param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nMode)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            ushort lowSpeed = 10, highSpeed = 50;
            double acc = 0.1;
            //lin_todo 根据轴设定 回原点模式、速度
            //nMode 7 / 11
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
                    nMode = 33;
                    break;

                case HomeMode.EZ_MEL:
                    nMode = 34;
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
                            //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,Delta Axis {0} Home Mode Error", nAxisNo));
                            WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home,GetSysAxisNo(nAxisNo).ToString(),
                                string.Format("Delta Axis {0} Home Mode Error", nAxisNo));

                        }
                        return false;
                    }
                    break;
            }


            short ret = CPCI_DMC.CS_DMC_01_set_home_config(addr.nCardID, addr.nNodeID, addr.nSlotID, (ushort)nMode, 0, lowSpeed, highSpeed, acc);
            ret = CPCI_DMC.CS_DMC_01_set_home_move(addr.nCardID, addr.nNodeID, addr.nSlotID);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                int nT = 0;
                while (true)
                {
                    nT++;
                    if (600 == nT) //指令发出后3s没检测到回Home动作则认为出错了
                    {
                        if (m_bEnable)
                        {
                            //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,Delta Axis {0} Home TimeOut Error,result = {1}", nAxisNo, ret));
                            WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home,GetSysAxisNo(nAxisNo).ToString(),
                                string.Format("Delta Axis {0} Home TimeOut Error,result = {1}", nAxisNo, ret));

                        }
                        return false;
                    }
                    if (12 == GetMotionState(nAxisNo))//回Home状态中
                    {
                        return true;
                    }
                    Thread.Sleep(5);
                }
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,Delta Axis {0} Home Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Axis {0} Home Error,result = {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 回原点
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="nMode">回原点模式</param>
        /// <param name="vm">回原点最大速度</param>
        /// <param name="vo">回原点爬行速度</param>
        /// <param name="acc">加速时间</param>
        /// <param name="dec">减速时间</param>
        /// <param name="offset">原点偏移</param>
        /// <param name="sFac"></param>
        /// <returns></returns>
        public override bool Home(int nAxisNo, int nMode, double vm, double vo, double acc, double dec,double offset = 0, double sFac = 0)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);

            ushort lowSpeed = (ushort)vo, highSpeed = (ushort)vm;
            //lin_todo 根据轴设定 回原点模式、速度
            //nMode 7 / 11
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
                    nMode = 33;
                    break;

                case HomeMode.EZ_MEL:
                    nMode = 34;
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
                            //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,Delta Axis {0} Home Mode Error", nAxisNo));
                            WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                                string.Format("Delta Axis {0} Home Mode Error", nAxisNo));
                        }
                        return false;
                    }
                    break;
            }

            short ret = CPCI_DMC.CS_DMC_01_set_home_config(addr.nCardID, addr.nNodeID, addr.nSlotID, (ushort)nMode, (int)offset, lowSpeed, highSpeed, acc);
            ret = CPCI_DMC.CS_DMC_01_set_home_move(addr.nCardID, addr.nNodeID, addr.nSlotID);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                int nT = 0;
                while (true)
                {
                    nT++;
                    if (600 == nT) //指令发出后3s没检测到回Home动作则认为出错了
                    {
                        if (m_bEnable)
                        {
                            //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,Delta Axis {0} Home TimeOut Error,result = {1}", nAxisNo, ret));
                            WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                                string.Format("Delta Axis {0} Home TimeOut Error,result = {1}", nAxisNo, ret));

                        }
                        return false;
                    }
                    if (12 == GetMotionState(nAxisNo))//回Home状态中
                    {
                        return true;
                    }
                    Thread.Sleep(5);
                }
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30105,ERR-XYT,Delta Axis {0} Home Error,result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Home, GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Axis {0} Home Error,result = {1}", nAxisNo, ret));

                }

                return false;
            }
        }

        /// <summary>
        ///以绝对位置移动 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool AbsMove(int nAxisNo, int nPos, int nSpeed)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            short ret = CPCI_DMC.CS_DMC_01_start_ta_move(addr.nCardID, addr.nNodeID, addr.nSlotID, nPos, 0, nSpeed, 0.1, 0.1);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta Axis {0} abs move Error,result is {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Axis {0} abs move Error,result is {1}", nAxisNo, ret));

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
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            short ret;

            if (sFac > 0)
            {
                //S-curve
                ret = CPCI_DMC.CS_DMC_01_start_sa_move(addr.nCardID, addr.nNodeID, addr.nSlotID, (int)fPos, (int)vs, (int)vm, acc, dec);
            }
            else
            {
                //T-curve
                ret = CPCI_DMC.CS_DMC_01_start_ta_move(addr.nCardID, addr.nNodeID, addr.nSlotID, (int)fPos, (int)vs, (int)vm, acc, dec);
                
            }

            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta Axis {0} abs move Error,result is {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Abs,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Axis {0} abs move Error,result is {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        ///相对位置移动
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nPos"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool RelativeMove(int nAxisNo, int nPos, int nSpeed)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            short ret = CPCI_DMC.CS_DMC_01_start_sr_move(addr.nCardID, addr.nNodeID, addr.nSlotID, nPos, nSpeed / 5, nSpeed, 0.05, 0.05);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta Axis {0} relative move Error,result is {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Axis {0} relative move Error,result is {1}", nAxisNo, ret));

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
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            short ret;

            if (sFac > 0)
            {
                //S-curve
                ret = CPCI_DMC.CS_DMC_01_start_sr_move(addr.nCardID, addr.nNodeID, addr.nSlotID, (int)fOffset, (int)vs, (int)vm, acc, dec);
            }
            else
            {
                //T-curve
                ret = CPCI_DMC.CS_DMC_01_start_tr_move(addr.nCardID, addr.nNodeID, addr.nSlotID, (int)fOffset, (int)vs, (int)vm, acc, dec);

            }

            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta Axis {0} relative move Error,result is {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Rel,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Axis {0} relative move Error,result is {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 两轴圆弧插补,S型曲线
        /// 只能是同一板卡上的两个轴
        /// </summary>
        /// <param name="nAxisNo1"></param>
        /// <param name="nAxisNo2"></param>
        /// <param name="nEndXPos"></param>
        /// <param name="nEndYPos"></param>
        /// <param name="fAngle"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool Arc2xy(int nAxisNo1, int nAxisNo2, int nEndXPos, int nEndYPos, double fAngle, int nSpeed)
        {
            DeltaAddr addr1 = GetAxisAddr(nAxisNo1);
            DeltaAddr addr2 = GetAxisAddr(nAxisNo2);

            ushort[] nodeID = { addr1.nNodeID, addr2.nNodeID };
            ushort[] slotID = { 0, 0 };
            short ret = CPCI_DMC.CS_DMC_01_start_sr_arc2_xy(addr1.nCardID, ref nodeID[0], ref slotID[0],
                nEndXPos, nEndYPos, fAngle, 0, nSpeed, 0.1, 0.1);
            if (0 == ret)
                return true;
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta Axis {0},{1} Arc2xy move Error,result is {1}", nAxisNo1, nAxisNo2, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "Arc2xy",
                        string.Format("Delta Axis {0},{1} Arc2xy move Error,result is {1}", nAxisNo1, nAxisNo2, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public bool StartTableMotion(MotionTable table, int nSpeed)
        {
            ushort nAMFNum = 2;  //滤波次数0~2
            ushort nCycle = 1;  //循环次数
            double fAcc = 0.1;

            short ret = CPCI_DMC.CS_DMC_01_start_tablemotion(table.nCardID, ref table.nNodeID[0], ref table.nSlotID[0], table.nTableNo, ref table.nPosData[0], table.PointCount, (short)table.nAxisNo.Length,
                nSpeed, fAcc, nAMFNum, nCycle,
                ref table.nGroupNo[0], ref table.nDoStatus[0]);
            if (0 == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta card {0} StartTableMotion Error,result is {1}", table.nCardID, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion, "StartTableMotion",
                        string.Format("Delta card {0} StartTableMotion Error,result is {1}", table.nCardID, ret));

                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool IsTableMotionEnd(MotionTable table)
        {
            bool bRtn = true;
            foreach (int n in table.nAxisNo)
                bRtn = bRtn && (0 == IsAxisNormalStop(n));
            return bRtn;
        }

        /// <summary>
        ///速度模式
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool VelocityMove(int nAxisNo, int nSpeed)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            short ret = CPCI_DMC.CS_DMC_01_set_velocity_mode(addr.nCardID, addr.nNodeID, addr.nSlotID, 0.05, 0.05);
            ret = CPCI_DMC.CS_DMC_01_set_velocity(addr.nCardID, addr.nNodeID, addr.nSlotID, nSpeed);
            if (CPCI_DMC_ERR.ERR_NoError == ret)
            {
                return true;
            }
            else
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30107,ERR-XYT,Delta Axis {0} velocity move Error,result is {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Vel,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Axis {0} velocity move Error,result is {1}", nAxisNo, ret));

                }
                return false;
            }
        }

        /// <summary>
        ///jog运动 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="bPositive"></param>
        /// <param name="bStrat"></param>
        /// <param name="nSpeed"></param>
        /// <returns></returns>
        public override bool JogMove(int nAxisNo, bool bPositive, int bStrat, int nSpeed)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            return true;
        }

        /// <summary>
        ///轴正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool StopAxis(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            short ret = CPCI_DMC.CS_DMC_01_sd_stop(addr.nCardID, addr.nNodeID, addr.nSlotID, 0.1);
            if (ret != 0)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30109,ERR-XYT,Delta Card normal stop axis {0} Error, result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_Stop,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Card normal stop axis {0} Error, result = {1}", nAxisNo, ret));

                }
                return false;
            }
            return true;
        }

        /// <summary>
        ///急停 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool StopEmg(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            short ret = CPCI_DMC.CS_DMC_01_emg_stop(addr.nCardID, addr.nNodeID, addr.nSlotID);
            if (ret != 0)
            {
                if (m_bEnable)
                {
                    //WarningMgr.GetInstance().Error(string.Format("30109,ERR-XYT,Delta Card emg stop axis {0} Error, result = {1}", nAxisNo, ret));
                    WarningMgr.GetInstance().Error(ErrorType.Err_Motion_EmgStop,GetSysAxisNo(nAxisNo).ToString(),
                        string.Format("Delta Card emg stop axis {0} Error, result = {1}", nAxisNo, ret));

                }
                return false;
            }
            return true;
        }

        /// <summary>
        ///获取轴卡运动状态 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override long GetMotionState(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            ushort MC_done = 0;
            short ret = CPCI_DMC.CS_DMC_01_motion_done(addr.nCardID, addr.nNodeID, addr.nSlotID, ref MC_done);
            /*
            MC_done:
            0:运动位移动作停止
            1:依加速时间进行运动位移
            2:依最大速度进行运动位移
            3:依减速时间进行运动位移
            5:指令在FIFO,尚未进入Buffer(伺服或04PI Md2)
            6:Buffer有运动指令(伺服或04PI Md2)
            12:回Home状态中(伺服或04PI Md2)
            21:PC与DSP的Counter没对准(04PI Md1)
            22:FIFO中有指令(04PI Md1)
            23:Buffer Full(04PI Md1)
            */
            if (ret == 0)
            {
                return MC_done;
            }
            return -1;
        }

        /// <summary>
        ///获取轴卡运动IO信号 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override long GetMotionIoState(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            uint MC_done = 0;
            short ret = CPCI_DMC.CS_DMC_01_motion_status(addr.nCardID, addr.nNodeID, addr.nSlotID, ref MC_done);
            //结合凌华排序0报警、1正限位、2负限位、3原点、4急停、5零位(EZ)、6到位、7励磁(伺服)
            long state = 0xFFD7;//原点、零位给0     1111 1111 1101/D 0111/7
            if (ret == 0)
            {
                if ((MC_done & (0x1 << 9)) == 0) //急停
                    state = state & 0xFFEF;//return 16;
                if ((MC_done & (0x1 << 5)) == 0) //报警
                    state = state & 0xFFFE;//return 1;
                if ((MC_done & (0x1 << 8)) == 0) //未servo on
                    state = state & 0xFF7F; //return 128;
                if ((MC_done & (0x1 << 14)) == 0) //正限位   
                    state = state & 0xFFFD;//return 2;
                if ((MC_done & (0x1 << 15)) == 0) //负限位
                    state = state & 0xFFFB;//return 4;
                if ((MC_done & (0x1 << 10)) == 0) //到位
                    state = state & 0xFFBF;//return 64;
                return state;
            }
            return -1;
            //ret = CPCI_DMC.CS_DMC_01_get_alm_code((ushort)m_nCardIndex, (ushort)nAxisNo, 0, ref MC_done);
        }

        /// <summary>
        ///获取轴的当前位置 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override double GetAixsPos(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            int nPos = 0;
            short ret = CPCI_DMC.CS_DMC_01_get_position(addr.nCardID, addr.nNodeID, addr.nSlotID, ref nPos);
            return nPos;
        }

        /// <summary>
        ///轴是否正常停止 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsAxisNormalStop(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            uint MC_done = 0;
            short ret = CPCI_DMC.CS_DMC_01_motion_status(addr.nCardID, addr.nNodeID, addr.nSlotID, ref MC_done);
            if (ret == 0)
            {
                if ((MC_done & (0x1 << 9)) != 0) //急停
                {
                    Debug.WriteLine("Axis {0} have Emg single \r\n", nAxisNo);
                    return 1;
                }
                if ((MC_done & (0x1 << 5)) != 0) //报警
                {
                    Debug.WriteLine("Axis {0} have Alm single \r\n", nAxisNo);
                    return 2;
                }
                if ((MC_done & (0x1 << 8)) == 0) //未servo on
                {
                    Debug.WriteLine("Axis {0} have servo single \r\n", nAxisNo);
                    return 3;
                }
                if ((MC_done & (0x1 << 14)) != 0) //正限位  
                {
                    Debug.WriteLine("Axis {0} have PEL single \r\n", nAxisNo);
                    return 4;
                }
                if ((MC_done & (0x1 << 15)) != 0) //负限位
                {
                    Debug.WriteLine("Axis {0} have MEL single \r\n", nAxisNo);
                    return 5;
                }
                if ((MC_done & (0x1 << 10)) != 0 && 0 == GetMotionState(nAxisNo)) //到位
                    return 0;
                return -1;
            }
            else
                return -1;//调用异常
        }

        /// <summary>
        /// 轴号是否在范围内
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <param name="nInPosError">到位误差</param>
        /// <returns></returns>
        public override int IsAxisInPos(int nAxisNo,int nInPosError = 1000)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            int nRet = IsAxisNormalStop(nAxisNo);
            if (nRet == 0)
            {
                int nTargetPos = 0;
                int nPos = 0;
                CPCI_DMC.CS_DMC_01_get_target_pos(addr.nCardID, addr.nNodeID, addr.nSlotID, ref nTargetPos);
                CPCI_DMC.CS_DMC_01_get_position(addr.nCardID, addr.nNodeID, addr.nSlotID, ref nPos);

                if (Math.Abs(nPos - nTargetPos) > nInPosError)
                    return 6;  //轴停止后位置超限
            }
            return nRet;
        }

        /// <summary>
        ///位置置零 
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool SetPosZero(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            return (CPCI_DMC.CS_DMC_01_set_position(addr.nCardID, addr.nNodeID, addr.nSlotID, 0) == 0);
        }

        /// <summary>
        /// 回原点是否正常停止
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override int IsHomeNormalStop(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            //回原点到位需要多个判断,轴正常停止且回Home动作已经完成
            if ((0 == IsAxisNormalStop(nAxisNo) && 0 == GetMotionState(nAxisNo)))
                return 0;
            else
                return -1;
        }

        /// <summary>
        /// 清除报警
        /// </summary>
        /// <param name="nAxisNo"></param>
        /// <returns></returns>
        public override bool ClearError(int nAxisNo)
        {
            DeltaAddr addr = GetAxisAddr(nAxisNo);
            //清除报警
            int ret = CPCI_DMC.CS_DMC_01_set_ralm(addr.nCardID, addr.nNodeID, addr.nSlotID);

            return (ret == CPCI_DMC_ERR.ERR_NoError);
        }
    }
}
