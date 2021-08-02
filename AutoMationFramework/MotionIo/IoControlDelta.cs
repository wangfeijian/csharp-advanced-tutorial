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
            public uint Type;
            /// <summary>
            /// 
            /// </summary>
            public ushort CardId;
            /// <summary>
            /// 
            /// </summary>
            public ushort NodeId;
            /// <summary>
            /// 
            /// </summary>
            public ushort SlotId;
            /// <summary>
            /// 
            /// </summary>
            public ushort PortId;
            /// <summary>
            /// 
            /// </summary>
            public ushort Index;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="cardId"></param>
            /// <param name="nodeId"></param>
            /// <param name="portId"></param>
            /// <param name="slotId"></param>
            /// <param name="index"></param>
            /// <param name="type"></param>
            public DeltaAddr(ushort cardId, ushort nodeId, ushort portId = 0, ushort slotId = 0, ushort index = 0, uint type = 0)
            {
                CardId = cardId;
                NodeId = nodeId;
                SlotId = slotId;
                PortId = portId;
                Index = index;
                Type = type;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public DeltaAddr AddrDi;
        /// <summary>
        /// 
        /// </summary>
        public DeltaAddr AddrDo;

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

            AddrDi = new DeltaAddr((ushort)(nCardNo / 10000 / 1000),
                (ushort)(nCardNo / 10000 % 1000 / 10),
                (ushort)(nCardNo / 10000 % 10));
            AddrDo = new DeltaAddr((ushort)(nCardNo % 10000 / 1000),
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
                short rc = CPCI_DMC.CS_DMC_01_get_devicetype((short)AddrDi.CardId, AddrDi.NodeId, AddrDi.SlotId, ref AddrDi.Type, ref nIdentity);
                rc = CPCI_DMC.CS_DMC_01_get_devicetype((short)AddrDo.CardId, AddrDo.NodeId, AddrDo.SlotId, ref AddrDo.Type, ref nIdentity);
                //Enable
                rc = CPCI_DMC.CS_DMC_01_set_rm_output_active(AddrDo.CardId, AddrDo.NodeId, AddrDo.SlotId, 1);

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

            short nRtn = CPCI_DMC.CS_DMC_01_get_rm_input_value(AddrDi.CardId, AddrDi.NodeId, AddrDi.SlotId, AddrDi.PortId, ref nInputData);
            if (CPCI_DMC_ERR.ERR_NoError == nRtn)
            {
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
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoInError");

                    RunInforManager.GetInstance().Error(string.Format(str1, AddrDi.CardId, "8254", data, nRtn));
                }

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
            ushort nData = 0;

            short nRtn = CPCI_DMC.CS_DMC_01_get_rm_input_single_value(AddrDi.CardId, AddrDi.NodeId, AddrDi.SlotId, AddrDi.PortId, (ushort)(nIndex - 1), ref nData);
            if (CPCI_DMC_ERR.ERR_NoError == nRtn)
            {
                return nData != 0;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoInBitError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadIn, $"{AddrDi.CardId}.{nIndex}", string.Format(str1, AddrDi.CardId,"8254", nIndex, nRtn));
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
            short nRtn = CPCI_DMC.CS_DMC_01_get_rm_output_single_value(AddrDo.CardId, AddrDo.NodeId, AddrDo.SlotId, AddrDo.PortId, (ushort)(nIndex - 1), ref nData);

            if (CPCI_DMC_ERR.ERR_NoError == nRtn)
            {
                return nData != 0;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoOutBitError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut, $"{AddrDo.CardId}.{nIndex}", string.Format(str1, AddrDo.CardId,"8254", nIndex, nRtn));

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
            short nRtn = CPCI_DMC.CS_DMC_01_get_rm_output_value(AddrDo.CardId, AddrDo.NodeId, AddrDo.SlotId, AddrDo.PortId, ref nValue);

            if (CPCI_DMC_ERR.ERR_NoError == nRtn)
            {
                nData = nValue;
                return true;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoOutError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut, AddrDo.CardId.ToString(), string.Format(str1, AddrDo.CardId,"8254",nData, nRtn));

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
                short nRtn = CPCI_DMC.CS_DMC_01_set_rm_output_single_value(AddrDo.CardId, AddrDo.NodeId, AddrDo.SlotId, AddrDo.PortId, (ushort)(nIndex - 1), (ushort)(bBit ? 1 : 0));
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
                short nRtn = CPCI_DMC.CS_DMC_01_set_rm_output_value(AddrDo.CardId, AddrDo.NodeId, AddrDo.SlotId, AddrDo.PortId, (ushort)nData);
                if (CPCI_DMC_ERR.ERR_NoError == nRtn)
                    return true;
            }
            return false;
        }
    }
}