/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    DMCCan IO card class                     *
*********************************************************************/


using CommonTools.Tools;
using CommonTools.Manager;
using csLTDMC;

namespace MotionIO
{
    /// <summary>
    /// 雷赛DmcCan自带的IO控制,16进16出，类名必须以"IoCtrl_"前导，否则加载不到
    /// </summary>
    public class IoControlDmcCan : IoControl
    {
        /// <summary>
        /// CAN扩展模块连接的主端子板编号，默认是0
        /// </summary>
        private ushort m_nMainCardNo = 0;

        /// <summary>
        /// CAN扩展模块的节点编号
        /// </summary>
        private ushort m_nNodeNo;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">索引号</param>
        /// <param name="nCardNo">卡号</param>
        public IoControlDmcCan(int index, int nCardNo) :base(index,nCardNo)
        {
            StrCardName = "DmcCan";
            StrArrayIn = new string[20];
            StrArrayOut = new string[20];

            m_nNodeNo = (ushort)nCardNo;
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override bool  Init()
        {
            try
            {
                short ret = LTDMC.dmc_set_can_state(m_nMainCardNo, m_nNodeNo, 1, 0);
                if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
                {
                    Enable = false;
                    return false;
                }

                ushort nNodeNum = 0;
                ushort nCanState = 0;
                ret = LTDMC.dmc_get_can_state(m_nMainCardNo, ref nNodeNum, ref nCanState);
                if (ret != (short)DMC3400_DEFINE.ERR_NOERR)
                {
                    Enable = false;
                    return false;
                }
                if ((m_nNodeNo <= 0) || (m_nNodeNo > nNodeNum))
                {
                    Enable = false;
                    return false;
                }

                if (nCanState != 1)
                {
                    Enable = false;
                    return false;
                }

                Enable = true;
                return true;
            }
            catch
            {
                Enable = false;
                return false;
            }
          }

        /// <summary>
        /// 反初始化
        /// </summary>
        public override void DeInit()
        {
        }
        
        /// <summary>
        /// 获取输入信号
        /// </summary>
        /// <param name="data">输入信号信息</param>
        /// <returns></returns>
        public override bool  ReadIoIn(ref int data)
        {
            if (Enable)
            {
                int nInputData = (int)LTDMC.dmc_read_can_inport(m_nMainCardNo, m_nNodeNo, 0);
                if (InData != nInputData)
                {
                    DataChange = true;
                    InData = data = nInputData;
                }
                else
                {
                    DataChange = false;
                    data = InData;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取输入信号
        /// </summary>
        /// <param name="nIndex">输入信号位</param>
        /// <returns></returns>
        public override bool ReadIoInBit(int nIndex)
        {
            short nIoState = LTDMC.dmc_read_can_inbit(m_nMainCardNo, m_nNodeNo, (ushort)(nIndex - 1));
            return nIoState != 0;
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nIndex">输出点位</param>
        /// <returns></returns>
        public override bool ReadIoOutBit(int nIndex)
        {
            short nIoState = LTDMC.dmc_read_can_outbit(m_nMainCardNo, m_nNodeNo, (ushort)(nIndex - 1));
            return nIoState != 0;
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public override bool  ReadIoOut(ref int nData)
        {            
            if (Enable)
            {
                int nOutputData = (int)LTDMC.dmc_read_can_outport(m_nMainCardNo, m_nNodeNo, 0);
                OutData = nData = nOutputData;
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// 输出信号
        /// </summary>
        /// <param name="nIndex">输出点</param>
        /// <param name="bBit">输出值</param>
        /// <returns></returns>
        public override bool  WriteIoBit(int nIndex, bool bBit)
        {
            if (Enable)
            {
                int onOff = bBit ? 1 : 0;
                short ret = LTDMC.dmc_write_can_outbit(m_nMainCardNo, m_nNodeNo, (ushort)(nIndex - 1), (ushort)onOff);
                if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
                {
                    return true;
                }
                else
                {
                    string str1 = LocationServices.GetLang("IoDmcCanReadIoBitError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoWrite, $"{CardNo}.{nIndex}",string.Format(str1, CardNo, ret));

                    return false;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 输出行信号
        /// </summary>
        /// <param name="nData">输出信息</param>
        /// <returns></returns>
        public override bool  WriteIo(int nData)
        {
            if (Enable)
            {
                short ret = LTDMC.dmc_write_can_outport(m_nMainCardNo, m_nNodeNo, 0, (uint)nData);
                if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
                {
                    return true;
                }
                else
                {
                    string str1 = LocationServices.GetLang("IoDmcCanReadIoError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoWrite,CardNo.ToString(),string.Format(str1, CardNo, ret));

                    return false;
                }
            }

            return false;
        }
    }
}