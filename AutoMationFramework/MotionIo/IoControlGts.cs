/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    GTS IO card class                        *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTools.Tools;
using CommonTools.Manager;
using gts;


namespace MotionIO
{
    /// <summary>
    /// 固高卡的IO类
    /// </summary>
    public class IoControlGts : IoControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="nCardNo"></param>
        public IoControlGts(int index, int nCardNo)
            : base(index, nCardNo)
        {
            Enable = false;
            StrCardName = "GTS";
            StrArrayIn = new string[16];
            StrArrayOut = new string[16];
        }

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            short nRtn = 0;          
            if (0 == mc.GT_Open((short)CardNo, 0, 1))
            {
                Enable = true;
                return true;
            }
           else
            {
                string str1 = LocationServices.GetLang("IoGtsInitError");
                RunInforManager.GetInstance().Error(ErrorType.ErrIoInit,CardNo.ToString(),
                    string.Format(str1, CardNo, nRtn));

                return false;
            }
        }

        /// <summary>
        /// 反初始化
        /// </summary>
        public override void DeInit()
        {
            mc.GT_Close((short)CardNo);
        }

        /// <summary>
        /// 读取一组IO输入数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool ReadIoIn(ref int data)
        {
            short nRtn = 0;
            nRtn = mc.GT_GetDi((short)CardNo, mc.MC_GPI, out data);
            if (0 == nRtn)
            {
                data = ~data;
                if (InData != data)
                {
                    DataChange = true;

                    InData = data;
                }
                else
                {
                    DataChange = false;
                }

                return true;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoGtsReadIoInError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadIn,CardNo.ToString(),
                        string.Format(str1, CardNo, nRtn));

                }
                return false;
            }
        }

        /// <summary>
        /// 读取一个IO输入点
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public override bool ReadIoInBit(int nIndex)
        {
            int nData = 0;
            if (0 == mc.GT_GetDi((short)CardNo, mc.MC_GPI, out nData))
            {
                nData = ~nData;
                return 0 != (nData & (1 << (nIndex - 1)));
            }
            return false;
        }

        /// <summary>
        /// 读取一组IO输出数据
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public override bool ReadIoOut(ref int nData)
        {
            short nRtn = 0;
            nRtn = mc.GT_GetDo((short)CardNo, mc.MC_GPO, out nData);
            if (0 == nRtn)
            {
                nData = ~nData;
                OutData = nData;
                return true;
            }
            else
            {
                if (Enable)
                {
                    string str1 = LocationServices.GetLang("IoGtsReadIoOutError");
                    RunInforManager.GetInstance().Error(ErrorType.ErrIoReadOut,CardNo.ToString(),
                        string.Format(str1, CardNo, nRtn));
                }
                return false;
            }
        }

        /// <summary>
        /// 读取一个IO输出点
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public override bool ReadIoOutBit(int nIndex)
        {
            int nData = 0;
            if (0 == mc.GT_GetDo((short)CardNo, mc.MC_GPO, out nData))
            {
                nData = ~nData;
                return 0 != (nData & (1 << (nIndex - 1)));
            }
            return false;
        }

        /// <summary>
        /// 写入一个IO输出点
        /// </summary>
        /// <param name="nIndex"></param>
        /// <param name="bBit"></param>
        /// <returns></returns>
        public override bool WriteIoBit(int nIndex, bool bBit)
        {
            return 0 == mc.GT_SetDoBit((short)CardNo, mc.MC_GPO, (short)nIndex, (short)(bBit ? 0 : 1));
        }

        /// <summary>
        /// 写入一组IO输出数据
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public override bool WriteIo(int nData)
        {
            return 0 == mc.GT_SetDo((short)CardNo, mc.MC_GPO, ~nData);
        }
    }
}
