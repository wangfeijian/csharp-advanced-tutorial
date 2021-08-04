/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    PCIEM60 IO card class                    *
*********************************************************************/

using System;
using System.Threading;
using AutoMationFrameworkSystemDll;
using CommonTools.Tools;
using lctdevice;

namespace MotionIO
{
    /// <summary>
    /// 凌臣EtherCAT控制卡的IO控制，类名必须以"IoCtrl_"前导，否则加载不到
    /// 框架中的每个汇川总线IO卡只支持32个IO，超过32个需要配置多张卡
    /// </summary>
    public class IoControlPcieM60 : IoControl
    {
        /// <summary>
        /// DI组的数量，32个一组
        /// </summary>
        private int _nDiGrpCnt;

        /// <summary>
        /// DO组的数量
        /// </summary>
        private int _nDoGrpCnt;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">索引号</param>
        /// <param name="nCardNo">卡号</param>
        public IoControlPcieM60(int index, int nCardNo) :base(index,nCardNo)
        {
            StrCardName = "PCIeM60";

            //框架中的每个汇川总线IO卡只支持32个IO，超过32个需要配置多张卡
            StrArrayIn = new string[32];
            StrArrayOut = new string[32];

            _nDiGrpCnt = (LctEtherCatCard.Instance().Resoures.DiNum  + 31) / 32;
            _nDoGrpCnt = (LctEtherCatCard.Instance().Resoures.DoNum + 31) / 32;
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override bool  Init()
        {
            try
            {
                if (LctEtherCatCard.Instance().IsInited)
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
            LctEtherCatCard.Instance().DeInit();
        }
        
        /// <summary>
        /// 获取输入信号
        /// </summary>
        /// <param name="data">输入信号信息</param>
        /// <returns></returns>
        public override bool  ReadIoIn(ref int data)
        {
            // 总线卡可以扩展多个IO模块，当I/O数量超过32个的时候，此函数只能读取前面32个I/O数据
            uint nInputData = 0;

            bool bRet = true;

            //从1开始
            short nCardNo = (short)(CardNo * 32 + 1);

            short ret = ecat_motion.M_Get_Digital_Port_Input(nCardNo, ref nInputData, 0);
            if (ret != 0)
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoInError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadIn, "PCIeM60", string.Format(str1,nCardNo,"PCIeM60",data, ret));
                }

                return false;
            }

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
            return bRet;
        }

        /// <summary>
        /// 获取输入信号
        /// </summary>
        /// <param name="nIndex">输入信号位</param>
        /// <returns></returns>
        public override bool ReadIoInBit(int nIndex)
        {
            short diValue = 0;
            short diNo = (short)(CardNo * 32 + nIndex);
            short ret = ecat_motion.M_Get_Digital_Chn_Input(diNo, out diValue,0);
            if (ret != 0)
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoInBitError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadIn, "PCIeM60", string.Format(str1, CardNo, "PCIeM60", nIndex, ret));
                }

                return false;
            }

            return (diValue != 0);
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nIndex">输出点位</param>
        /// <returns></returns>
        public override bool ReadIoOutBit(int nIndex)
        {
            short doValue = 0;
            short doNo = (short)(CardNo * 32 + nIndex);

            short ret = ecat_motion.M_Get_Digital_Chn_Output(doNo, out doValue, 0);
            if (ret != 0)
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoOutBitError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut, "PCIeM60", string.Format(str1, CardNo, "PCIeM60", nIndex, ret));
                }

                return false;
            }

            return (doValue != 0);
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public override bool  ReadIoOut(ref int nData)
        {
            uint nOutputData = 0;

            //从1开始
            short nCardNo = (short)(CardNo * 32 + 1);

            short ret = ecat_motion.M_Get_Digital_Port_Output(nCardNo, ref nOutputData, 0);
            if (ret != 0)
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoOutError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut, "PCIeM60", string.Format(str1, CardNo, "PCIeM60", nData, ret));
                }

                return false;
            }

            nData = (int)nOutputData;
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
                short doNo = (short)(CardNo * 32 + nIndex);

                short ret = ecat_motion.M_Set_Digital_Chn_Output(doNo, bBit ? (short)1 : (short)0, 0);

                if (ret != 0)
                {

                    string str1 = LocationServices.GetLang("IoCardWriteIoBitError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrIoWrite, "PCIeM60", string.Format(str1, CardNo,ret,"PCIeM60"));


                    return false;
                }

                return true;
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
                //从1开始
                short nCardNo = (short)(CardNo * 32 + 1);

                short ret = ecat_motion.M_Set_Digital_Port_Output(nCardNo, (uint)nData,0xFFFFFFFF, 0);
                if (ret != 0)
                {
                    if (Enable)
                    {
                        string str1 = LocationServices.GetLang("IoCardWriteIoError");

                        RunInforManager.GetInstance().Error(ErrorType.ErrIoWrite, "PCIeM60", string.Format(str1, CardNo, ret, "PCIeM60"));
                    }

                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}