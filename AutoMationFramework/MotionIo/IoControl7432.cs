/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    7432 IO card class                       *
*********************************************************************/

using CommonTools.Tools;
using Adlink;
using CommonTools.Manager;


namespace MotionIO
{
    /// <summary>
    /// 凌华7432 IO卡, 类名必须以"IoCtrl_"前导，否则加载不到
    /// </summary>
    public class IoControl7432 : IoControl
    {
        private int _nInternalCardNo;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nCardNo"></param>
        public IoControl7432(int index, int nCardNo) : base(index, nCardNo)
        {
            Enable = false;
            StrCardName = "7432";
            StrArrayIn = new string[32];
            StrArrayOut = new string[32];
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            short ret = DASK.Register_Card(DASK.PCI_7432, (ushort)CardNo);
            if (DASK.NoError <= ret)
            {
                _nInternalCardNo = ret;
                Enable = true;
                DASK.DO_WritePort((ushort)_nInternalCardNo, 0, 0);
                return true;
            }
            else
            {
                Enable = false;
                string str1 = LocationServices.GetLang("Io7432InitError");
                RunInforManager.GetInstance().Error(ErrorType.ErrIoInit, _nInternalCardNo.ToString(), string.Format(str1, CardNo, ret));
                return false;
            }
        }

        /// <summary>
        /// 释放IO卡
        /// </summary>
        public override void DeInit()
        {
            if (_nInternalCardNo >= 0)
                DASK.Release_Card((ushort)_nInternalCardNo);
        }

        /// <summary>
        /// 获取输入信号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool ReadIoIn(ref int data)
        {
            uint nInputData;
            short ret = DASK.DI_ReadPort((ushort)_nInternalCardNo, 0, out nInputData);
            if (DASK.NoError == ret)
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
                    string str1 = LocationServices.GetLang("Io7432ReadIoInError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadIn, _nInternalCardNo.ToString(), string.Format(str1, CardNo, ret));

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
            ushort nData;
            short ret = DASK.DI_ReadLine((ushort)_nInternalCardNo, 0, (ushort)(nIndex - 1), out nData);
            if (DASK.NoError == ret)
            {
                return nData != 0;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("Io7432ReadIoInBitError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadIn, $"{_nInternalCardNo}.{nIndex}", string.Format(str1, CardNo, nIndex, ret));

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
            uint tmp;
            short ret = DASK.DO_ReadPort((ushort)_nInternalCardNo, 0, out tmp);
            if (DASK.NoError == ret)
            {
                nData = OutData = (int)tmp;
                return true;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("Io7432ReadIoOutError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut, _nInternalCardNo.ToString(), string.Format(str1, CardNo, ret));
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
            ushort nData;
            short ret = DASK.DO_ReadLine((ushort)_nInternalCardNo, 0, (ushort)(nIndex - 1), out nData);
            if (DASK.NoError == ret)
            {
                return nData != 0;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("Io7432ReadIoOutBitError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut, $"{_nInternalCardNo}.{nIndex}", string.Format(str1, CardNo, nIndex, ret));
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
                return DASK.DO_WriteLine((ushort)_nInternalCardNo, 0, (ushort)(nIndex - 1), bBit ? ((ushort)(1)) : ((ushort)(0))) == 0;
            else
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
                return DASK.DO_WritePort((ushort)_nInternalCardNo, 0, (uint)nData) == 0;
            else
                return false;
        }

    }

}