/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-07                               *
*                                                                    *
*           ModifyTime:     2021-08-07                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Tcp server manager class                 *
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTools.Tools;
using System.Collections.Concurrent;
using System.Xml;
using AutoMationFrameworkSystemDll;

namespace Communicate
{
    /// <summary>
    /// TCP服务管理类
    /// </summary>
    public class TcpServerManager : SingletonPattern<TcpServerManager>
    {
        /// <summary>
        /// 网口描述定义
        /// </summary>
        public static readonly string[] m_strDescribe = { "序号", "本地IP地址", "监听端口", "启用" };

        /// <summary>
        /// 所有服务集合
        /// </summary>
        public List<AsyncSocketTCPServer> m_listServers = new List<AsyncSocketTCPServer>();

        /// <summary>
        /// 所有连接上的客户端
        /// </summary>
        public Dictionary<AsyncSocketState, AsyncSocketTCPServer> m_dictClients = new Dictionary<AsyncSocketState, AsyncSocketTCPServer>();

        /// <summary>
        /// 客户端连接
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> ClientConnected;

        /// <summary>
        /// 客户端断开连接
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> ClientDisconnected;

        /// <summary>
        /// 服务端状态改变事件
        /// </summary>
        public event EventHandler<EventArgs> ServerStateChanged;

        /// <summary>
        /// 获取TCP服务
        /// </summary>
        /// <param name="index">索引号</param>
        /// <returns></returns>
        public AsyncSocketTCPServer GetTcpServer(int index)
        {
            if (index < m_listServers.Count)
            {
                return m_listServers[index];
            }

            return null;
        }

        /// <summary>
        /// 获取服务数量
        /// </summary>
        /// <returns></returns>
        public int Count
        {
            get { return m_listServers.Count; }
        }

        /// <summary>
        /// 清除服务
        /// </summary>
        public void Clear()
        {
            foreach (var server in m_listServers)
            {
                server.Dispose();
            }
            m_listServers.Clear();
        }

        /// <summary>
        /// 从xml文件中读取定义的网口信息
        /// </summary>
        /// <param name="doc">已打开的xml文档</param>
        public void ReadCfgFromXml(XmlDocument doc)
        {
            Clear();
            
            XmlNodeList xnl = doc.SelectNodes("/SystemCfg/" + "Server");
            if (xnl.Count > 0)
            {
                xnl = xnl.Item(0).ChildNodes;
                if (xnl.Count > 0)
                {
                    foreach (XmlNode xn in xnl)
                    {
                        XmlElement xe = (XmlElement)xn;

                        int nItem = 0;
                        string strNo = xe.GetAttribute(m_strDescribe[nItem++]).Trim();
                        string strIP = xe.GetAttribute(m_strDescribe[nItem++]).Trim();
                        int nListenPort = Convert.ToInt32(xe.GetAttribute(m_strDescribe[nItem++]).Trim());
                        bool bEnable = Convert.ToBoolean(xe.GetAttribute(m_strDescribe[nItem++]).Trim());

                        AsyncSocketTCPServer tcpServer = new AsyncSocketTCPServer(strIP, nListenPort);

                        if (bEnable)
                        {
                            tcpServer.Start();

                            tcpServer.ClientConnected += OnClientConnected;
                            tcpServer.ClientDisconnected += OnClientDisconnected;
                            tcpServer.ServerStateChangedEvent += OnServerStateChanged;
                        }

                        m_listServers.Add(tcpServer);
                    }
                }
            }
        }

        private void OnServerStateChanged(object sender, EventArgs e)
        {
            AsyncSocketTCPServer server = sender as AsyncSocketTCPServer;

            if (server.IsRunning)
            {
                string strMsg = string.Format("服务({0})启动", server.ToString());
                RunInforManager.GetInstance().Info(strMsg);

                ShowLog(strMsg);
            }
            else
            {
                string strMsg = string.Format("服务({0})停止", server.ToString());
                RunInforManager.GetInstance().Info(strMsg);

                ShowLog(strMsg);
            }

            if (ServerStateChanged != null)
            {
                ServerStateChanged(sender, e);
            }
        }

        private void OnClientDisconnected(object sender, AsyncSocketEventArgs e)
        {
            AsyncSocketTCPServer server = (AsyncSocketTCPServer)sender;

            string strMsg = string.Format("服务({0})的客户端{1}已断开",server.ToString(), e.m_state.ToString());
            RunInforManager.GetInstance().Info(strMsg);
            ShowLog(strMsg);

            lock (m_dictClients)
            {
                m_dictClients.Remove(e.m_state);
            }

            if (ClientDisconnected != null)
            {
                ClientDisconnected(sender,e);
            }
        }

        private void OnClientConnected(object sender, AsyncSocketEventArgs e)
        {
            AsyncSocketTCPServer server = (AsyncSocketTCPServer)sender;

            string strMsg = string.Format("服务({0})的客户端{1}已连接", server.ToString(), e.m_state.ToString());
            RunInforManager.GetInstance().Info(strMsg);
            ShowLog(strMsg);

            lock (m_dictClients)
            {
                m_dictClients.Add(e.m_state, server);
            }

            if (ClientConnected != null)
            {
                ClientConnected(sender,e);
            }
        }

        /// <summary>
        /// 根据EndPoint字符串获取客户端
        /// </summary>
        /// <param name="strEndPoint"></param>
        /// <returns></returns>
        public AsyncSocketState GetClient(string strEndPoint)
        {
            foreach (var item in m_dictClients.Keys)
            {
                if (item.ClientSocket.RemoteEndPoint.ToString() == strEndPoint)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="strEndPoint"></param>
        /// <param name="strData"></param>
        /// <returns></returns>
        public void SendAsync(string strEndPoint, string strData)
        {
            AsyncSocketState client = GetClient(strEndPoint);

            AsyncSocketTCPServer server;

            if (m_dictClients.TryGetValue(client, out server))
            {
                server.Send(client, strData);
            }
        }

        /// <summary>
        /// 更新内存参数到表格数据
        /// </summary>
        /// <param name="grid">界面网口表格控件</param>
        //public void UpdateGridFromParam(DataGridView grid)
        //{
        //    grid.Rows.Clear();
            
        //    foreach(var server in m_listServers)
        //    {
        //        int nRow = grid.Rows.Add();

        //        int nCol = 0;

        //        grid.Rows[nRow].Cells[nCol++].Value = nRow + 1;
        //        grid.Rows[nRow].Cells[nCol++].Value = server.Address.ToString();
        //        grid.Rows[nRow].Cells[nCol++].Value = server.Port;
        //        grid.Rows[nRow].Cells[nCol++].Value = server.IsRunning.ToString();
        //    }
        //}

        /// <summary>
        /// 更新表格数据到内存参数
        /// </summary>
        /// <param name="grid">界面网口表格控件</param>
        //public void UpdateParamFromGrid(DataGridView grid)
        //{
        //    Clear();

        //    foreach(DataGridViewRow row in grid.Rows)
        //    {
        //        try
        //        {
        //            int nCol = 0;
        //            int nIndex = Convert.ToInt32(row.Cells[nCol++].Value);
        //            string strIP = row.Cells[nCol++].Value.ToString();
        //            int nListenPort = Convert.ToInt32(row.Cells[nCol++].Value);
        //            bool bEnable = Convert.ToBoolean(row.Cells[nCol++].Value);

        //            AsyncSocketTCPServer tcpServer = new AsyncSocketTCPServer(strIP, nListenPort);

        //            if (bEnable)
        //            {
        //                tcpServer.Start();
        //            }

        //            m_listServers.Add(tcpServer);
        //        }
        //        catch (Exception)
        //        {

                    
        //        }
                
        //    }
        //}

        /// <summary>
        /// 保存内存参数到xml文件
        /// </summary>
        /// <param name="doc">已打开的xml文档</param>
        public void SaveCfgXML(XmlDocument doc)
        {
            XmlNode xnl = doc.SelectSingleNode("SystemCfg");
            XmlNode root = xnl.SelectSingleNode("Server");
            if (root == null)
            {
                root = doc.CreateElement("Server");

                xnl.AppendChild(root);
            }

            root.RemoveAll();

            int nItem = 1;
            foreach (var t in m_listServers)
            {
                XmlElement xe = doc.CreateElement("Server");

                int j = 0;
                xe.SetAttribute(m_strDescribe[j++], (nItem++).ToString());
                xe.SetAttribute(m_strDescribe[j++], t.Address.ToString());
                xe.SetAttribute(m_strDescribe[j++], t.Port.ToString());
                xe.SetAttribute(m_strDescribe[j++], t.IsRunning.ToString());

                root.AppendChild(xe);
            }
        }
    }
}
