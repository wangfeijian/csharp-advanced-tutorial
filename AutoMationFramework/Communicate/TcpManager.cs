/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-07                               *
*                                                                    *
*           ModifyTime:     2021-08-07                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Tcp manager class                        *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using AutoMationFrameworkSystemDll;
using AutoMationFrameworkViewModel;
using CommonTools.Tools;
using GalaSoft.MvvmLight.Ioc;

namespace Communicate
{
    /// <summary>
    /// 网口类管理器
    /// </summary>
    public class TcpManager : SingletonPattern<TcpManager>
    {
        /// <summary>
        /// 网络连接列表
        /// </summary>
        private List<TcpLink> _listTcpLink = new List<TcpLink>();


        /// <summary>
        /// 返回对应索引号的对象
        /// </summary>
        /// <param name="index">网口索引号</param>
        /// <returns></returns>
        public TcpLink GetTcpLink(int index)
        {
            if (index < _listTcpLink.Count())
            {
                return _listTcpLink.ElementAt(index);
            }
            return null;
        }

        /// <summary>
        /// 获取系统中网络连接总数
        /// </summary>
        /// <returns></returns>
        public int Count => _listTcpLink.Count;

        /// <summary>
        /// 从xml文件中读取定义的网口信息
        /// </summary>
        public void ReadTcpFromCfg()
        {
            _listTcpLink.Clear();

            var tcpList = SimpleIoc.Default.GetInstance<SystemConfigViewModel>().SystemConfig.EthInfos;

            foreach (var ethInfo in tcpList)
            {
                string strNo = ethInfo.EthNum;
                string strName = ethInfo.EthDefine;
                string strIp = ethInfo.IpAddress;
                string strPort = ethInfo.Port;
                string strTime = ethInfo.TimeOut;
                string strLine = ethInfo.Command;

                _listTcpLink.Add(new TcpLink(Convert.ToInt32(strNo), strName, strIp, Convert.ToInt32(strPort), Convert.ToInt32(strTime), strLine));
            }
        }
    }
}
