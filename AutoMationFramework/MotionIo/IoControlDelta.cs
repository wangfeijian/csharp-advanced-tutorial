/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    8254 IO delta card class                 *
*********************************************************************/

using System;
using System.Threading;
using CommonTools.Manager;
using CommonTools.Tools;
using PCI_DMC;
using PCI_DMC_ERR;

namespace MotionIO
{
    /// <summary>
    /// 类名必须以"IoControl"前导，否则加载不到
    /// </summary>
    /// // <IoCard 卡序号="4" 卡号="10021001" 卡类型="Detla" />
    public class IoControlDelta : IoControl
    {
        /// <summary>
        /// 
        /// </summary>
        public struct DeltaAddr
        {
            /// <summary>
            /// 
            /// </summary>
            public uint nType;
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
            /// <param name="nType"></param>
            public DeltaAddr(ushort nCardID, ushort nNodeID, ushort nPortID = 0, ushort nSlotID = 0, ushort nIndex = 0, uint nType = 0)
            {
                this.nCardID = nCardID;
                this.nNodeID = nNodeID;
                this.nSlotID = nSlotID;
                this.nPortID = nPortID;
                this.nIndex = nIndex;
                this.nType = nType;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public DeltaAddr addrDI;
        /// <summary>
        /// 
        /// </summary>
        public DeltaAddr addrDO;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">索引号</param>
        /// <param name="nCardNo">卡号</param>
        public IoControlDelta(int index, int nCardNo) : base(index, nCardNo)
        {
            StrCardName = "Delta";
            StrArrayIn = new string[16];
            StrArrayOut = new string[16];

            this.addrDI = new DeltaAddr((ushort)(nCardNo / 10000 / 1000),
                (ushort)(nCardNo / 10000 % 1000 / 10),
                (ushort)(nCardNo / 10000 % 10));
            this.addrDO = new DeltaAddr((ushort)(nCardNo % 10000 / 1000),
                (ushort)(nCardNo % 10000 % 1000 / 10),
                (ushort)(nCardNo % 10000 % 10));
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            //Delta 总线在Motion中已经初始化
            try
            {
                uint nIdentity = 0;
                short rc = CPCI_DMC.CS_DMC_01_get_devicetype((short)this.addrDI.nCardID, this.addrDI.nNodeID, this.addrDI.nSlotID, ref this.addrDI.nType, ref nIdentity);
                rc = CPCI_DMC.CS_DMC_01_get_devicetype((short)this.addrDO.nCardID, this.addrDO.nNodeID, this.addrDO.nSlotID, ref this.addrDO.nType, ref nIdentity);
                //Enable
                rc = CPCI_DMC.CS_DMC_01_set_rm_output_active(this.addrDO.nCardID, this.addrDO.nNodeID, this.addrDO.nSlotID, 1);

                if (CPCI_DMC_ERR.ERR_NoError == rc)
                {
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
        public override bool ReadIoIn(ref int data)
        {
            ushort nInputData = 0;

            short nRtn = CPCI_DMC.CS_DMC_01_get_rm_input_value(this.addrDI.nCardID, this.addrDI.nNodeID, this.addrDI.nSlotID, this.addrDI.nPortID, ref nInputData);
            if (CPCI_DMC_ERR.ERR_NoError == nRtn)
            {
                //nInputData = (nInputData | (nInputData << 24)) >> 8;
                if (InData != (int)nInputData)
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
            else
            {
                //if (Enable)
                //RunInforManager.GetInstance().Error(string.Format("20101,ERR-XYT,第{0}张IO卡8254 ReadIoIn Error,ErrorCode = {1}", CardNo, nRet));
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
            //    Random rnd1 = new Random();
            //    return rnd1.Next() % 2   == 0 ;
            ushort nData = 0;

            short nRtn = CPCI_DMC.CS_DMC_01_get_rm_input_single_value(this.addrDI.nCardID, this.addrDI.nNodeID, this.addrDI.nSlotID, this.addrDI.nPortID, (ushort)(nIndex - 1), ref nData);
            if (CPCI_DMC_ERR.ERR_NoError == nRtn)
            {
                return nData == 0 ? false : true;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("Io8254DeltaReadIoInBitError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadIn, $"{addrDI.nCardID}.{nIndex}", string.Format(str1, addrDI.nCardID, nIndex, nRtn));
                }
                return false;
            }
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nIndex">输出点位</param>
        /// <returns></returns>
        public override bool ReadIoOutBit(int nIndex)
        {
            //   Random rnd1 = new Random();
            //   return rnd1.Next() % 2 == 0;

            ushort nData = 0;
            short nRtn = CPCI_DMC.CS_DMC_01_get_rm_output_single_value(this.addrDO.nCardID, this.addrDO.nNodeID, this.addrDO.nSlotID, this.addrDO.nPortID, (ushort)(nIndex - 1), ref nData);

            if (CPCI_DMC_ERR.ERR_NoError == nRtn)
            {
                return nData == 0 ? false : true;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("Io8254DeltaReadIoOutBitError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut, $"{addrDO.nCardID}.{nIndex}",string.Format(str1, addrDO.nCardID, nIndex, nRtn));

                }
                return false;
            }
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public override bool ReadIoOut(ref int nData)
        {
            ushort nValue = 0;
            short nRtn = CPCI_DMC.CS_DMC_01_get_rm_output_value(this.addrDO.nCardID, this.addrDO.nNodeID, this.addrDO.nSlotID, this.addrDO.nPortID, ref nValue);

            if (CPCI_DMC_ERR.ERR_NoError == nRtn)
            {
                nData = nValue;
                return true;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("Io8254DeltaReadIoOutError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut, addrDO.nCardID.ToString(),string.Format(str1, addrDO.nCardID, nRtn));

                }
                return false;
            }
        }

        /// <summary>
        /// 输出信号
        /// </summary>
        /// <param name="nIndex">输出点</param>
        /// <param name="bBit">输出值</param>
        /// <returns></returns>
        public override bool WriteIoBit(int nIndex, bool bBit)
        {
            if (Enable)
            {
                short nRtn = CPCI_DMC.CS_DMC_01_set_rm_output_single_value(this.addrDO.nCardID, this.addrDO.nNodeID, this.addrDO.nSlotID, this.addrDO.nPortID, (ushort)(nIndex - 1), (ushort)(bBit ? 1 : 0));
                if (CPCI_DMC_ERR.ERR_NoError == nRtn)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 输出行信号
        /// </summary>
        /// <param name="nData">输出信息</param>
        /// <returns></returns>
        public override bool WriteIo(int nData)
        {
            if (Enable)
            {
                short nRtn = CPCI_DMC.CS_DMC_01_set_rm_output_value(this.addrDO.nCardID, this.addrDO.nNodeID, this.addrDO.nSlotID, this.addrDO.nPortID, (ushort)nData);
                if (CPCI_DMC_ERR.ERR_NoError == nRtn)
                    return true;
            }
            return false;
        }
    }
}