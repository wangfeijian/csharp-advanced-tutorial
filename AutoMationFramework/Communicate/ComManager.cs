/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-07                               *
*                                                                    *
*           ModifyTime:     2021-08-07                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Com port manager class                   *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using AutoMationFrameworkSystemDll;
using AutoMationFrameworkViewModel;
using GalaSoft.MvvmLight.Ioc;

namespace Communicate
{
    /// <summary>
    /// 串口系统管理类
    /// </summary>
    public class ComManager : SingletonPattern<ComManager>
    {
        /// <summary>
        /// 串口定义列表
        /// </summary>
        private List<ComLink> _listComLink = new List<ComLink>();

        /// <summary>
        /// 从xml文件中读取定义的串口信息
        /// </summary>
        public void ReadComFromCfg()
        {
            _listComLink.Clear();

            var comList = SimpleIoc.Default.GetInstance<SystemConfigViewModel>().SystemConfig.ComInfos;

            foreach (var comInfo in comList)
            {
                string strNo = comInfo.ComNum;
                string strName = comInfo.ComDefine;
                string strBaudRate = comInfo.BaudRate;
                string strDataBit = comInfo.DataByte;
                string strPartiy = comInfo.CheckByte;
                string strStopBit = comInfo.StopByte;
                string strFlowCtrl = comInfo.StreamControl;
                string strTime = comInfo.TimeOut;
                string strBufferSize = comInfo.BufferSize;
                string strLine = comInfo.Command;

                _listComLink.Add(new ComLink(Convert.ToInt32(strNo), strName, Convert.ToInt32(strBaudRate),
                                    Convert.ToInt32(strDataBit), strPartiy, strStopBit, strFlowCtrl,
                                    Convert.ToInt32(strTime), Convert.ToInt32(strBufferSize), strLine));
            }
        }


        /// <summary>
        /// 返回对应索引的对象
        /// </summary>
        /// <param name="nIndex">索引号</param>
        /// <returns></returns>
        public ComLink GetComLink(int nIndex)
        {
            if (nIndex < _listComLink.Count())
            {
                return _listComLink.ElementAt(nIndex);
            }
            return null;
        }

        /// <summary>
        /// 获取系统串口总数
        /// </summary>
        /// <returns></returns>
        public int Count => _listComLink.Count;
    }
}
