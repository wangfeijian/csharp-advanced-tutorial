/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    DMC3400 IO card class                    *
*********************************************************************/

using AutoMationFrameworkSystemDll;
using CommonTools.Tools;
using csLTDMC;

namespace MotionIO
{
    /// <summary>
    /// 雷赛Dmc3400自带的IO控制,16进16出，类名必须以"IoControl"前导，否则加载不到
    /// </summary>
    public class IoControlDmc3400 : IoControl
    {
        private ushort _nInternalIndex;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">索引号</param>
        /// <param name="nCardNo">卡号</param>
        public IoControlDmc3400(int index, int nCardNo) :base(index,nCardNo)
        {
            StrCardName = "Dmc3400";
            StrArrayIn = new string[20];
            StrArrayOut = new string[20] ;

            _nInternalIndex = (ushort)nCardNo;
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override bool  Init()
        {
            //Dmc3400自带IO，无需专门的初始化, 通过函数调用判断是否能初始化成功
            try
            {
                uint uCountValue = 0;
                short ret = LTDMC.dmc_get_io_count_value(_nInternalIndex, 0, ref uCountValue);
                if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
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
        public override bool  ReadIoIn(ref int data)
        {
            // 只有bit0~bit15是通用输入端口
            int nInputData = (int)(LTDMC.dmc_read_inport(_nInternalIndex, 0) & 0xFFFF);
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

        /// <summary>
        /// 获取输入信号
        /// </summary>
        /// <param name="nIndex">输入信号位</param>
        /// <returns></returns>
        public override bool ReadIoInBit(int nIndex)
        {
            short nIoState = LTDMC.dmc_read_inbit(_nInternalIndex, (ushort)(nIndex - 1));
            return nIoState != 0;
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nIndex">输出点位</param>
        /// <returns></returns>
        public override bool ReadIoOutBit(int nIndex)
        {
            short nIoState = LTDMC.dmc_read_outbit(_nInternalIndex, (ushort)(nIndex - 1));
            return nIoState != 0;
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public override bool  ReadIoOut(ref int nData)
        {            
            int nOutputData = (int)LTDMC.dmc_read_outport(_nInternalIndex, 0);
            OutData = nData = (nOutputData & 0xFFFF);// 只有bit0~bit15是通用输入端口
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
                int onOff = bBit ? 1 : 0;
                short ret = LTDMC.dmc_write_outbit(_nInternalIndex, (ushort)(nIndex - 1), (ushort)onOff);
                if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
                {
                    return true;
                }
                else
                {
                    string str1 = LocationServices.GetLang("IoCardWriteIoBitError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoWrite, $"{CardNo}.{nIndex}",string.Format(str1, CardNo, ret,"DMC3400"));

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
                int uOutData = (int)LTDMC.dmc_read_outport(_nInternalIndex, 0);
                uOutData = (uOutData & ~0xFFFF) | (nData & 0xFFFF);

                short ret = LTDMC.dmc_write_outport(_nInternalIndex, 0, (uint)uOutData);
                if (ret == (short)DMC3400_DEFINE.ERR_NOERR)
                {
                    return true;
                }
                else
                {
                    string str1 = LocationServices.GetLang("IoCardWriteIoError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoWrite,CardNo.ToString(),
                        string.Format(str1, CardNo, ret,"DMC3400"));

                    return false;
                }
            }
            return false;
        }
    }
}