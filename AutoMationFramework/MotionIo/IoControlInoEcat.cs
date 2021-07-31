/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-30                               *
*                                                                    *
*           ModifyTime:     2021-07-30                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    InoEcag IO card class                    *
*********************************************************************/

using System;
using System.Threading;
using CommonTools.Tools;
using CommonTools.Manager;
using Inovance.InoMotionCotrollerShop.InoServiceContract.EtherCATConfigApi;

namespace MotionIO
{
    /// <summary>
    /// 汇川EtherCAT控制卡的IO控制，类名必须以"IoControl"前导，否则加载不到
    /// 框架中的每个汇川总线IO卡只支持32个IO，超过32个需要配置多张卡
    /// </summary>
    public class IoControlInoEcat : IoControl
    {
        /// <summary>
        /// 控制卡句柄
        /// </summary>
        private UInt64 m_hCardHandle = ImcApi.ERR_HANDLE;

        /// <summary>
        /// DI组的数量，汇川的IO以8个为一组
        /// </summary>
        private int m_nDiGrpCnt = 0;

        /// <summary>
        /// DO组的数量
        /// </summary>
        private int m_nDoGrpCnt = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">索引号</param>
        /// <param name="nCardNo">卡号</param>
        public IoControlInoEcat(int index, int nCardNo) :base(index,nCardNo)
        {
            StrCardName = "InoEcat";

            //框架中的每个汇川总线IO卡只支持32个IO，超过32个需要配置多张卡
            //StrArrayIn = new string[InoEtherCatCard.Instance().m_tResoures.diNum];
            //StrArrayOut = new string[InoEtherCatCard.Instance().m_tResoures.doNum];
            StrArrayIn = new string[32];
            StrArrayOut = new string[32];

            m_nDiGrpCnt = InoEtherCatCard.Instance().m_tResoures.diNum / 8;
            m_nDoGrpCnt = InoEtherCatCard.Instance().m_tResoures.doNum / 8;
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override bool  Init()
        {
            //InoEcat自带IO，无需专门的初始化, 通过函数调用判断是否能初始化成功
            try
            {
                m_hCardHandle = InoEtherCatCard.Instance().m_hCardHandle;
                if (m_hCardHandle != ImcApi.ERR_HANDLE)
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
            // 总线卡可以扩展多个IO模块，当I/O数量超过32个的时候，此函数只能读取前面32个I/O数据
            int nInputData = 0;

            //计算本张卡上的有几组DI，一张卡上最多4组
            int nGrpCnt = 0;
            if (m_nDiGrpCnt >= (CardNo + 1) * 4)
            {
                nGrpCnt = 4;
            }
            else if (m_nDiGrpCnt > CardNo * 4)
            {
                nGrpCnt = m_nDiGrpCnt % 4;
            }

            bool bRet = true;
            for (int i = 0; i < nGrpCnt; i++)
            {
                short nGrpDi = 0;
                short groupNo = (short)(i + CardNo * 4);
                uint ret = ImcApi.IMC_GetEcatGrpDi(m_hCardHandle, (short)groupNo, ref nGrpDi);
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    bRet = false;
                    break;
                }

                nInputData |= ((int)nGrpDi & 0xFF) << (i * 8);
            }
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
            short diNo = (short)(CardNo * 32 + nIndex - 1);
            uint ret = ImcApi.IMC_GetEcatDiBit(m_hCardHandle, diNo, ref diValue);
            if (ret != ImcApi.EXE_SUCCESS)
                return false;

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
            short doNo = (short)(CardNo * 32 + nIndex - 1);
            uint ret = ImcApi.IMC_GetEcatDoBit(m_hCardHandle, doNo, ref doValue);
            if (ret != ImcApi.EXE_SUCCESS)
                return false;

            return (doValue != 0);
        }

        /// <summary>
        /// 获取输出信号
        /// </summary>
        /// <param name="nData"></param>
        /// <returns></returns>
        public override bool  ReadIoOut(ref int nData)
        {
            int nOutputData = 0;

            //计算本张卡上的有几组DO，一张卡上最多4组
            int nGrpCnt = 0;
            if (m_nDoGrpCnt >= (CardNo + 1) * 4)
            {
                nGrpCnt = 4;
            }
            else if (m_nDoGrpCnt > CardNo * 4)
            {
                nGrpCnt = m_nDoGrpCnt % 4;
            }

            bool bRet = true;
            for (int i = 0; i < nGrpCnt; i++)
            {
                short nGrpDo = 0;
                short groupNo = (short)(i + CardNo * 4);

                uint ret = ImcApi.IMC_GetEcatGrpDo(m_hCardHandle, groupNo, ref nGrpDo);
                if (ret != ImcApi.EXE_SUCCESS)
                {
                    bRet = false;

                    break;
                }

                nOutputData |= ((int)nGrpDo & 0xFF) << (i * 8);
            }

            nData = nOutputData;
            return bRet;
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
                short doNo = (short)(CardNo * 32 + nIndex - 1);
                uint ret = ImcApi.IMC_SetEcatDoBit(m_hCardHandle, doNo, (short)(bBit ? 1 : 0));
                if (ret == ImcApi.EXE_SUCCESS)
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
                //计算本张卡上的有几组DO，一张卡上最多4组
                int nGrpCnt = 0;
                if (m_nDoGrpCnt >= (CardNo + 1) * 4)
                {
                    nGrpCnt = 4;
                }
                else if (m_nDoGrpCnt > CardNo * 4)
                {
                    nGrpCnt = m_nDoGrpCnt % 4;
                }

                for (int i = 0; i < nGrpCnt; i++)
                {
                    short nGrpDo = (short)((nData >> (i * 8)) & 0xFF);
                    short groupNo = (short)(i + CardNo * 4);

                    uint ret = ImcApi.IMC_SetEcatGrpDo(m_hCardHandle, groupNo, nGrpDo);
                    if (ret != ImcApi.EXE_SUCCESS)
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