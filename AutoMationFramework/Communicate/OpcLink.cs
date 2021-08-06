using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCAutomation;
using System.Net;
using System.Threading;
using System.Windows;
using CommonTools.Tools;

namespace Communicate
{
    /// <summary>
    /// OPC Item信息
    /// </summary>
    public class OpcInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public OPCItem opcItem;
        /// <summary>
        /// 
        /// </summary>
        public int nHandleClient;
        /// <summary>
        /// 
        /// </summary>
        public string strValue;
        /// <summary>
        /// 
        /// </summary>
        public int nQuality;
        /// <summary>
        /// 
        /// </summary>
        public string strTimeStamp;

        /// <summary>
        /// 
        /// </summary>
        public OpcInfo()
        {
            opcItem = null;
            nHandleClient = 0;
            strValue = "";
            nQuality = 0;
            strTimeStamp = "";
        }
    }
    /// <summary>
    /// OPC通信类封装
    /// </summary>
    public class OpcLink
    {
        private OPCServer m_opcServer = null;
        private OPCGroup m_opcGroup = null;
        private bool m_bOpcConnected = false;
        private static object m_objLock = new object();
        private Dictionary<string, OpcInfo> m_dictOpcInfos = new Dictionary<string, OpcInfo>();
        private object m_listServerNames;
        /// <summary>
        /// 
        /// </summary>
        public bool OpcConnected
        {
            get { return this.m_bOpcConnected; }
        }

        /// <summary>
        /// 
        /// </summary>
        public OPCGroup OpcGroup
        {
            get { return this.m_opcGroup; }
        }

        /// <summary>
        /// 枚举本地OPC服务器
        /// </summary>
        public OpcLink()
        {
            try
            {
                this.m_opcServer = new OPCServer();

                string strHostName = Dns.GetHostName();

                m_listServerNames = m_opcServer.GetOPCServers(strHostName);

                
            }
            catch (Exception err)
            {
                string str1 = "枚举本地OPC服务器出错：";
                string str2 = "提示信息";
                if (LocationServices.GetLangType() == "en-us")
                {
                    str1 = "Error enumerating local OPC servers:";
                    str2 = "Prompt information";
                }
                MessageBox.Show(str1 + err.Message, str2, MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// 获取本地计算上的所有OPC服务
        /// </summary>
        /// <returns></returns>
        public object ServerNames
        {
            get
            {
                return m_listServerNames;
            }
            
        }

        /// <summary>
        /// 获取当前的OPC服务
        /// </summary>
        /// <returns></returns>
        public string GetCurrentServerName()
        {
            return this.m_opcServer.ServerName;
        }


        /// <summary>
        /// 获取当前OPC服务的信息
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="strVersion">OPC版本</param>
        public void GetServerInfo(out DateTime StartTime, out string strVersion)
        {
            StartTime = this.m_opcServer.StartTime;
            strVersion = m_opcServer.MajorVersion.ToString() + "." + m_opcServer.MinorVersion.ToString() + "." + m_opcServer.BuildNumber.ToString();
        }

        /// <summary>
        /// 连接OPC服务器
        /// </summary>
        /// <param name="remoteServerIP">OPCServerIP</param>
        /// <param name="remoteServerName">OPCServer名称</param>
        public bool ConnectRemoteServer(string remoteServerIP, string remoteServerName)
        {
            string str1 = "未能连接远程服务器，状态：";
            string str2 = "连接远程服务器出现错误：";
            string str3 = "提示信息";
            if (LocationServices.GetLangType() == "en-us")
            {
                str1 = "Failed to connect to remote server, status:";
                str2 = "Error connecting to remote server:";
                str3 = "Prompt information";
            }

            try
            {
                m_opcServer.Connect(remoteServerName, remoteServerIP);

                if (m_opcServer.ServerState == (int)OPCServerState.OPCRunning)
                {
                    this.m_bOpcConnected = true;
                    return true;
                }
                else
                {
                    MessageBox.Show(str1 + m_opcServer.ServerState.ToString() + "   ");
                    return false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(str2 + err.Message, str3, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        /// <summary>
        /// 断开OPC连接
        /// </summary>
        public void Disconnect()
        {
            if (m_opcServer != null)
            {
                if (m_opcGroup != null)
                {
                    RemoveAllItems();
                    m_opcServer.OPCGroups.RemoveAll();
                    m_opcGroup = null;
                    m_dictOpcInfos.Clear();
                }
               
                m_opcServer.Disconnect();
            }
                
            this.m_bOpcConnected = false;
        }


        /// <summary>
        /// 获取当前OPC服务的状态
        /// </summary>
        /// <returns></returns>
        public int GetServerState()
        {
            return m_opcServer.ServerState;
        }
        
        /// <summary>
        /// 创建组
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns></returns>
        public bool CreateGroup(string groupName)
        {
            try
            {
                if (this.m_opcGroup == null)
                {
                    this.m_opcGroup = this.m_opcServer.OPCGroups.Add(groupName);
                }
                
                return true;
            }
            catch (Exception err)
            {
                string str1 = "创建组出现错误：";
                string str2 = "提示信息";
                if (LocationServices.GetLangType() == "en-us")
                {
                    str1 = "Error creating group:";
                    str2 = "Prompt information";
                }

                MessageBox.Show(str1 + err.Message, str2, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        /// <summary>
        /// 设置组属性
        /// </summary>
        public void SetGroupProperty(bool bDefaultGroupActive, float fDeadband, int nUpdateRate, bool bActive, bool bSubscribed)
        {
            m_opcServer.OPCGroups.DefaultGroupIsActive = bDefaultGroupActive;
            m_opcServer.OPCGroups.DefaultGroupDeadband = fDeadband;
            m_opcGroup.UpdateRate = nUpdateRate;
            m_opcGroup.IsActive = bActive;
            m_opcGroup.IsSubscribed = bSubscribed;
        }

        /// <summary>
        /// 获取当前OPC服务的所有Item
        /// </summary>
        /// <param name="ls"></param>
        public void GetServerItems(ref List<string> ls)
        {
            ls.Clear();
            OPCBrowser opcBrowser = m_opcServer.CreateBrowser();
            //展开分支
            opcBrowser.ShowBranches();
            //展开叶子
            opcBrowser.ShowLeafs(true);
            foreach (object turn in opcBrowser)
                ls.Add(turn.ToString());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="nItemHandleStart"></param>
        public void CreateItems(Array ar, int nItemHandleStart)
        {

            foreach (object turn in ar)
            {
                try
                {
                    OPCItems opcItems = this.m_opcGroup.OPCItems;

                    OpcInfo ih = new OpcInfo();
                    ih.nHandleClient = nItemHandleStart + this.m_dictOpcInfos.Count;
                    ih.opcItem = opcItems.AddItem(turn.ToString(), ih.nHandleClient);

                    this.m_dictOpcInfos.Add((string)turn, ih);

                    Thread.Sleep(200);
                    
                    int nQuality = 0;
                    ReadValue(turn.ToString(), out nQuality);
                }
                catch (Exception err)
                {
                    //没有任何权限的项，都是OPC服务器保留的系统项，此处可不做处理。
                    if (LocationServices.GetLangType() == "en-us")
                    {
                        MessageBox.Show("This item is reserved for the system:" + err.Message, "Prompt information");
                    }
                    else
                    {
                        MessageBox.Show("此项为系统保留项:" + err.Message, "提示信息");
                    }
                }
            }

            this.OpcGroup.DataChange += new OPCAutomation.DIOPCGroupEvent_DataChangeEventHandler(KepGroup_DataChange);
        }

        /// <summary>
        /// 移除所有的Item
        /// </summary>
        public void RemoveAllItems()
        {
            int nItemCount = m_dictOpcInfos.Count;

            if (nItemCount == 0)
            {
                return;
            }

            int[] temp = new int[nItemCount + 1];
            temp[0] = 0;

            int nIndex = 1;
            foreach (var item in m_dictOpcInfos)
            {
                temp[nIndex++] = item.Value.opcItem.ServerHandle;
            }

            Array serverHandlers = (Array)temp;
            Array Errors;

            this.m_opcGroup.OPCItems.Remove(nItemCount, ref serverHandlers, out Errors);

            foreach(int err in Errors)
            {
                if (err != 0)
                {

                }
            }
        }

        /// <summary>
        /// 移除单个Item
        /// </summary>
        /// <param name="sTag">标签名称</param>
        public void RemoveItem(string sTag)
        {
            if (!m_dictOpcInfos.ContainsKey(sTag))
            {
                return;
            }

            int[] temp = new int[2] { 0,this.m_dictOpcInfos[sTag].opcItem.ServerHandle };
            Array serverHandles = (Array)temp;
            Array Errors;

            this.OpcGroup.OPCItems.Remove(1, ref serverHandles, out Errors);

            if ((int)Errors.GetValue(1) != 0)
            {
               if (LocationServices.GetLangType() == "en-us")
                {
                    MessageBox.Show(String.Format("Failed to remove {0}", sTag));
                }
                else
                {
                    MessageBox.Show(String.Format("移除{0}失败", sTag));
                }
                
            }
        }

        /// <summary>
        /// OPC Item数据改变事件响应
        /// </summary>
        /// <param name="TransactionID"></param>
        /// <param name="NumItems"></param>
        /// <param name="ClientHandles"></param>
        /// <param name="ItemValues"></param>
        /// <param name="Qualities"></param>
        /// <param name="TimeStamps"></param>
        void KepGroup_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            for (int i = 1; i <= NumItems; i++)
            {
                foreach (KeyValuePair<string, OpcInfo> kvp in this.m_dictOpcInfos)
                {
                    if (kvp.Value.nHandleClient == (int)ClientHandles.GetValue(i))
                    {
                        if (ItemValues.GetValue(i) == null)
                            continue;

                        kvp.Value.strValue = ItemValues.GetValue(i).ToString();
                        kvp.Value.nQuality = Convert.ToInt32(Qualities.GetValue(i).ToString());
                        kvp.Value.strTimeStamp = TimeStamps.GetValue(i).ToString();
                    }
                }
            }
        }

        /// <summary>
        /// OPC 写Item数据
        /// </summary>
        /// <param name="strTagName">标签名称</param>
        /// <param name="_value">值</param>
        public void WriteValue(string strTagName, object _value)
        {
            lock (m_objLock)
            {
                int[] temp = new int[2] { 0, this.m_dictOpcInfos[strTagName].opcItem.ServerHandle };
                Array serverHandles = (Array)temp;
                object[] valueTemp = new object[2] { "", _value };
                Array values = (Array)valueTemp;
                Array Errors;
                int cancelID;
                this.m_opcGroup.AsyncWrite(1, ref serverHandles, ref values, out Errors, 2009, out cancelID);
                //KepItem.Write(txtWriteTagValue.Text);//这句也可以写入，但并不触发写入事件
                GC.Collect();
            }
        }

        /// <summary>
        /// 异步读数据
        /// </summary>
        /// <param name="strTagName">标签名称</param>
        public void AsyncReadValue(string strTagName)
        {
            lock (m_objLock)
            {
                int[] temp = new int[2] { 0, this.m_dictOpcInfos[strTagName].opcItem.ServerHandle };
                Array serverHandles = (Array)temp;
                Array Errors;
                int cancel;
                this.m_opcGroup.AsyncRead(1, ref serverHandles, out Errors, 2009, out cancel);
                GC.Collect();
            }
        }


        /// <summary>
        /// 同步读数据
        /// </summary>
        /// <param name="strTagName">标签名称</param>
        /// <param name="nQuality">质量</param>
        /// <returns>OPC Item数据</returns>
        public object ReadValue(string strTagName, out int nQuality)
        {
            lock (m_objLock)
            {
                try
                {
                    object obValue = new object();
                    object obQuality = new object();
                    object obTimeStamp = new object();
                    this.m_dictOpcInfos[strTagName].opcItem.Read(1, out obValue, out obQuality, out obTimeStamp);

                    nQuality = Convert.ToInt32(obQuality.ToString());

                    this.m_dictOpcInfos[strTagName].nQuality = nQuality;
                    this.m_dictOpcInfos[strTagName].strTimeStamp = obTimeStamp.ToString();
                    this.m_dictOpcInfos[strTagName].strValue = obValue.ToString();

                    return obValue;
                    //return opcItem.Value;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    nQuality = 0;
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取OPC Item数据
        /// </summary>
        /// <param name="strTagName"></param>
        /// <returns>OPC Item数据</returns>
        public string GetValue(string strTagName)
        {
            return this.m_dictOpcInfos[strTagName].strValue;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="sTag"></param>
        /// <returns></returns>
        public OpcInfo GetOpcItem(string sTag)
        {
            if (m_dictOpcInfos.ContainsKey(sTag))
            {
                return m_dictOpcInfos[sTag];
            }

            return null;
        }
    }
}
