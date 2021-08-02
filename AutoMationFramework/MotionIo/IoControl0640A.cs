/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    0640A IO card class                      *
*********************************************************************/
using System;

using csIOC0640;
using CommonTools.Manager;
using CommonTools.Tools;


namespace MotionIO
{
    /// <summary>
    /// 雷赛0640A IO卡
    /// </summary>
    public class IoControl0640A : IoControl
    {
        private static int _nStatInit;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nCardNo"></param>
        public IoControl0640A(int index, int nCardNo) : base(index, nCardNo)
        {
            Enable = false;
            StrCardName = "0640A";
            StrArrayIn = new string[32];
            StrArrayOut = new string[32];
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            if (0 == _nStatInit)
            {
                int ret = IOC0640.ioc_board_init();
                if (ret > 0)
                {
                    _nStatInit = 1;
                    Enable = true;
                    return true;
                }
                else
                {
                    string str1 = LocationServices.GetLang("IoCardInitError");
                    Enable = false;

                    RunInforManager.GetInstance().Error(ErrorType.ErrIoInit, CardNo.ToString(), string.Format(str1, CardNo, "0640A", ret));
                    return false;
                }
            }
            else
            {
                Enable = true;
                return true;
            }
        }

        /// <summary>
        /// 释放IO卡
        /// </summary>
        public override void DeInit()
        {
            try
            {
                if (CardNo >= 0)
                {
                    if (1 == _nStatInit)
                    {
                        _nStatInit = 0;
                        IOC0640.ioc_board_close();
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            } //忽略IOC0640关闭多张卡时报错
        }

        /// <summary>
        /// 获取输入信号
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public override bool ReadIoIn(ref int nData)
        {
            nData = IOC0640.ioc_read_inport((ushort)CardNo, 0);
            //雷赛IO卡为PNP卡，与8254相反，为保持一致，需进行取反
            nData = ~nData;
            if (InData != nData)
            {
                InData = nData;
                DataChange = true;
            }
            else
            {
                DataChange = false;
            }
            return true;
        }

        /// <summary>
        /// 获取输入信号
        /// </summary>
        /// <param name="nIndex">输入信号位 取值范围1-47</param>
        /// <returns></returns>
        public override bool ReadIoInBit(int nIndex)
        {
            int ret = IOC0640.ioc_read_inbit((ushort)CardNo, (ushort)nIndex);
            //雷赛IO卡为PNP卡，与8254相反，为保持一致，需进行取反
            if (0 == ret)
                return true;
            else
            {
                string str1 = LocationServices.GetLang("IoCardReadIoInBitError");
                if (Enable)
                    RunInforManager.GetInstance().Error(string.Format(str1, CardNo, "0640A", nIndex, ret));
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
            nData = IOC0640.ioc_read_outport((ushort)CardNo, 0);
            //雷赛IO卡为PNP卡，与8254相反，为保持一致，需进行取反
            nData = ~nData;
            return true;
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nIndex">输出点位 1-48</param>
        /// <returns></returns>
        public override bool ReadIoOutBit(int nIndex)
        {
            int ret = IOC0640.ioc_read_outbit((ushort)CardNo, (ushort)nIndex);
            //雷赛IO卡为PNP卡，与8254相反，为保持一致，需进行取反
            if (0 == ret)
                return true;
            else
            {
                string str1 = LocationServices.GetLang("IoCardReadIoOutBitError");
                if (Enable)
                    RunInforManager.GetInstance().Error(string.Format(str1, CardNo, "0640A", nIndex, ret));
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
                //雷赛IO卡为PNP卡，与8254相反，为保持一致，需进行取反
                UInt32 ret = IOC0640.ioc_write_outbit((ushort)CardNo, (ushort)nIndex, bBit == false ? 1 : 0);
                if (0 == ret)
                    return true;
                else
                {
                    if (Enable)
                    {
                        string str1 = LocationServices.GetLang("IoCardWriteIoBitError");

                        RunInforManager.GetInstance().Error(ErrorType.ErrIoWrite, $"{CardNo}.{nIndex}", string.Format(str1, CardNo, ret, "0640A"));
                    }

                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 输出信号
        /// </summary>
        /// <param name="nData">输出信息</param>
        /// <returns></returns>
        public override bool WriteIo(int nData)
        {
            if (Enable)
            {
                //雷赛IO卡为PNP卡，与8254相反，为保持一致，需进行取反
                nData = ~nData;
                UInt32 ret = IOC0640.ioc_write_outport((ushort)CardNo, 0, (UInt32)nData);
                if (0 == ret)
                    return true;
                else
                {
                    if (Enable)
                    {
                        string str1 = LocationServices.GetLang("IoCardWriteIoError");

                        RunInforManager.GetInstance().Error(ErrorType.ErrIoWrite, $"{CardNo}.{Index}", string.Format(str1, CardNo, ret, "0640A"));
                    }

                    return false;
                }
            }
            return false;
        }

    }

}