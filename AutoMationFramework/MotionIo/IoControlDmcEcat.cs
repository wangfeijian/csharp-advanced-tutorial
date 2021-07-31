/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    DMCEcat IO card class                    *
*********************************************************************/
using System;
using System.Threading;
using CommonTools.Tools;
using CommonTools.Manager;
using csLTDMC;

namespace MotionIO
{
    /// <summary>
    /// 雷赛EtherCAT控制卡的IO控制，类名必须以"IoControl"前导，否则加载不到
    /// </summary>
    public class IoControlDmcEcat : IoControl
    {
        /// <summary>
        /// 控制卡ID
        /// </summary>
        private ushort m_nCardId = 0;

        /// <summary>
        /// 板卡的系统IO数量
        /// </summary>
        private static readonly int SYS_IO_IN_COUNT = 8;

        private static readonly int SYS_IO_OUT_COUNT = 8;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">索引号</param>
        /// <param name="nCardNo">卡号</param>
        public IoControlDmcEcat(int index, int nCardNo) :base(index,nCardNo)
        {
            StrCardName = "DmcEcat";

            if (nCardNo == 0)
            {
                //第一张卡，需要去除8个系统IO
                StrArrayIn = new string[32 - SYS_IO_IN_COUNT];
                StrArrayOut = new string[32 - SYS_IO_OUT_COUNT];
            }
            else
            {
                StrArrayIn = new string[32];
                StrArrayOut = new string[32];
            }
            //StrArrayIn = new string[DmcEtherCatCard.Instance().InputCount];
            //StrArrayOut = new string[DmcEtherCatCard.Instance().OutputCount];
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override bool  Init()
        {
            //DmcEcat自带IO，无需专门的初始化, 通过函数调用判断是否能初始化成功
            try
            {
                int nCardId = DmcEtherCatCard.Instance().CardId;
                if (nCardId >= 0)
                {
                    m_nCardId = (ushort)nCardId;
                    Enable = true;
                    return true;
                }
                else
                {
                    Enable = false;
                    return false;
                }
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
            // 总线卡可以扩展多个IO模块，当I/O数量超过32个的时候，此函数只能读取前面32个I/O数据
            int nInputData = (int)LTDMC.dmc_read_inport(m_nCardId, (ushort)CardNo);

            if (CardNo == 0)
            {
                //前8个位系统IO板卡内部使用
                nInputData >>= SYS_IO_IN_COUNT;
            }

            //雷赛总线板卡的IO是反的，0表示有效，1表示无效
            nInputData = ~nInputData;
           
            if (InData != nInputData)
            {
                DataChange = true;
                InData = data = (int)nInputData;
            }
            else
            {
                DataChange = false;
                data = InData;
            }
            return true;
        }

        /// <summary>
        /// 获取输入信号
        /// </summary>
        /// <param name="nIndex">输入信号位</param>
        /// <returns></returns>
        public override bool ReadIoInBit(int nIndex)
        {
            if (CardNo == 0)
            {
                //前8个位系统IO板卡内部使用
                nIndex += SYS_IO_IN_COUNT;
            }
            else
            {
                //超过32
                nIndex += 32 * CardNo;
            }

            int nBitValue = LTDMC.dmc_read_inbit(m_nCardId, (ushort)(nIndex - 1));

            //雷赛总线板卡的IO是反的，0表示有效，1表示无效
            return (nBitValue == 0);
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nIndex">输出点位</param>
        /// <returns></returns>
        public override bool ReadIoOutBit(int nIndex)
        {
            if (CardNo == 0)
            {
                //前8个位系统IO板卡内部使用
                nIndex += SYS_IO_OUT_COUNT;
            }
            else
            {
                nIndex += 32 * CardNo;
            }

            int nBitValue = LTDMC.dmc_read_outbit(m_nCardId, (ushort)(nIndex - 1));

            //雷赛总线板卡的IO是反的，0表示有效，1表示无效
            return (nBitValue == 0);
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public override bool  ReadIoOut(ref int nData)
        {
            nData = (int)LTDMC.dmc_read_outport(m_nCardId, (ushort)CardNo);

            if (CardNo == 0)
            {
                //前8个位系统IO板卡内部使用
                nData >>= SYS_IO_OUT_COUNT;
            }

            nData = ~nData;

            OutData = nData;

            return true;
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
                if (CardNo == 0)
                {
                    //前8个位系统IO板卡内部使用
                    nIndex += SYS_IO_OUT_COUNT;
                }
                else
                {
                    nIndex += 32 * CardNo;
                }

                //雷赛总线板卡的IO是反的，0表示有效，1表示无效
                short ret = LTDMC.dmc_write_outbit(m_nCardId, (ushort)(nIndex - 1), (ushort)(bBit ? 0: 1));
                if (ret == 0)
                    return true;
                else
                    return false;
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
                //前8个位系统IO板卡内部使用
                int nOutputData = (int)LTDMC.dmc_read_outport(m_nCardId, (ushort)CardNo);

                if (CardNo == 0)
                {
                    //保留前8位的状态
                    nData = (nData >> SYS_IO_OUT_COUNT) | (nOutputData & 0xFF);
                }

                //雷赛总线板卡的IO是反的，0表示有效，1表示无效
                nData = ~nData;

                short ret = LTDMC.dmc_write_outport(m_nCardId, (ushort)CardNo, (uint)nData);
                if (ret == 0)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
    }
}