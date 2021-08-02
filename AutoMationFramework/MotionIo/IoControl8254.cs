/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    8254 IO card class                       *
*********************************************************************/

using CommonTools.Tools;
using CommonTools.Manager;
using Adlink;

namespace MotionIO
{
    /// <summary>
    /// 凌华8254自带的IO控制,20进20出，类名必须以"IoCtrl_"前导，否则加载不到
    /// </summary>
    public class IoControl8254 : IoControl
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">索引号</param>
        /// <param name="nCardNo">卡号</param>
        public IoControl8254(int index, int nCardNo) : base(index, nCardNo)
        {
            StrCardName = "8254";
            StrArrayIn = new string[20];
            StrArrayOut = new string[20];
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            //8254自带IO，无需专门的初始化, 通过函数调用判断是否能初始化成功
            try
            {
                int data = 0;
                int nRet = APS168.APS_read_d_input(CardNo, 0, ref data);
                if ((int)APS_Define.ERR_NoError == nRet)
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
            int nInputData = 0;
            int nRet = APS168.APS_read_d_input(CardNo, 0, ref nInputData);
            if ((int)APS_Define.ERR_NoError == nRet)
            {
                nInputData = (nInputData | (nInputData << 24)) >> 8;
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
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoInError");
                    RunInforManager.GetInstance().Error(string.Format(str1, CardNo, "8254", data, nRet));
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
            //    Random rnd1 = new Random();
            //    return rnd1.Next() % 2   == 0 ;
            int nData = 0;
            int nRet = APS168.APS_read_d_input(CardNo, 0, ref nData);
            if ((int)APS_Define.ERR_NoError == nRet)
            {
                nData = (nData | (nData << 24)) >> 8;
                return (nData & (1 << (nIndex - 1))) != 0;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoInBitError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadIn, $"{CardNo}.{nIndex}", string.Format(str1, CardNo, "8254", nIndex, nRet));

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
            int nData = 0;
            int nRet = APS168.APS_read_d_output(CardNo, 0, ref nData);

            if ((int)APS_Define.ERR_NoError == nRet)
            {
                nData = (nData | (nData << 24)) >> 8;
                return (nData & (1 << (nIndex - 1))) != 0;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoOutBitError");

                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut, $"{CardNo}.{nIndex}", string.Format(str1, CardNo, "8254", nIndex, nRet));

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
            int data = 0;
            int nRet = APS168.APS_read_d_output(CardNo, 0, ref data);

            if ((int)APS_Define.ERR_NoError == nRet)
            {
                OutData = nData = (data | (data << 24)) >> 8;
                return true;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoCardReadIoOutError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut, CardNo.ToString(), string.Format(str1, CardNo, "8254", nData, nRet));
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
                if (nIndex > 16)
                    nIndex -= 16;
                else
                    nIndex += 8;
                return APS168.APS_write_d_channel_output(CardNo, 0, nIndex - 1, bBit ? 1 : 0) == 0;
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
                nData = (nData << 8) | (nData >> 24);
                APS168.APS_write_d_output(CardNo, 0, nData);
                return true;
            }
            else
                return false;
        }
    }
}