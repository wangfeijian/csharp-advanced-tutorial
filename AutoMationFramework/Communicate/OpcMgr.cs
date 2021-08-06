using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonTools.Tools;
using System.Xml;
using AutoMationFrameworkSystemDll;
using OPCAutomation;

namespace Communicate
{
    /// <summary>
    /// OPC系统管理类
    /// </summary>
    public class OpcMgr : SingletonPattern<OpcMgr>
    {
        private OpcLink m_opcLink;
        private Dictionary<string, string> m_dictTags = new Dictionary<string, string>();
        private bool m_bOpcEnable = false;
        private string m_strServerIP = "";
        private string m_strServerName = "";
        private string m_strGroupName = "";
        private int m_nUpdateRate = 1000;

        /// <summary>
        /// OPC起始HANDLER
        /// </summary>
        public const int OPC_Hander_Start = 1234;

        /// <summary>
        /// 数据变更委托函数定义
        /// </summary>
        /// <param name="TransactionID"></param>
        /// <param name="NumItems"></param>
        /// <param name="ClientHandles"></param>
        /// <param name="ItemValues"></param>
        /// <param name="Qualities"></param>
        /// <param name="TimeStamps"></param>
        public delegate void DataChangeHandler(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps);
        /// <summary>
        /// 获取OpcLink对象
        /// </summary>
        /// <returns></returns>
        public OpcLink GetOpcLink()
        {
            return m_opcLink;
        }

        /// <summary>
        /// 是否启用OPC
        /// </summary>
        public bool OpcEnable
        {
            get
            {
                return m_bOpcEnable;
            }

            set
            {
                m_bOpcEnable = value;
            }
        }

        /// <summary>
        /// OPC服务器IP地址
        /// </summary>
        public string ServerIP
        {
            get
            {
                return m_strServerIP;
            }

            set
            {
                m_strServerIP = value;
            }
        }

        /// <summary>
        /// OPC服务器名称
        /// </summary>
        public string ServerName
        {
            get
            {
                return m_strServerName;
            }

            set
            {
                m_strServerName = value;
            }
        }

        /// <summary>
        /// OPC群组名称
        /// </summary>
        public string GroupName
        {
            get
            {
                return m_strGroupName;
            }

            set
            {
                m_strGroupName = value;
            }
        }

        /// <summary>
        /// 更新频率
        /// </summary>
        public int UpdateRate
        {
            get
            {
                return m_nUpdateRate;
            }

            set
            {
                m_nUpdateRate = value;
            }
        }

        /// <summary>
        /// 从xml文件中读取定义的OPC信息
        /// </summary>
        /// <param name="doc">已打开的XML文件</param>
        public void ReadCfgFromXml(XmlDocument doc)
        {
            m_dictTags.Clear();

            XmlNodeList xnl = doc.SelectNodes("/SystemCfg/" + "Opc");
            if (xnl.Count > 0)
            {
                xnl = xnl.Item(0).ChildNodes;
                if (xnl.Count > 0)
                {
                    XmlElement xe = (XmlElement)xnl.Item(0);

                    string strServerIP = xe.GetAttribute("服务器IP").Trim();
                    string strServerName = xe.GetAttribute("服务器名称").Trim();
                    string strGroupName = xe.GetAttribute("组名称").Trim();
                    string strUpdateRate = xe.GetAttribute("刷新频率").Trim();
                    string strEnable = xe.GetAttribute("启用").Trim();

                    m_bOpcEnable = Convert.ToBoolean(strEnable);
                    m_strGroupName = strGroupName;
                    m_strServerName = strServerName;
                    m_strServerIP = strServerIP;
                    m_nUpdateRate = Convert.ToInt32(strUpdateRate);

                    foreach (XmlNode xn in xnl.Item(0).ChildNodes)
                    {
                        XmlElement e = (XmlElement)xn;

                        string strTag = e.GetAttribute("标签").Trim();
                        string strDesc = e.GetAttribute("描述").Trim();

                        if (m_dictTags.ContainsKey(strDesc))
                        {
                            if (LocationServices.GetLangType() == "en-us")
                            {
                                MessageBox.Show("Duplicate OPC label description -" + strTag + ":" + strDesc);
                            }
                            else
                            {
                                MessageBox.Show("OPC标签描述有重复 - " + strTag + ":" + strDesc);
                            }

                            return;
                        }

                        m_dictTags.Add(strDesc, strTag);
                    }

                    if (m_bOpcEnable)
                    {
                        if(m_opcLink == null)
                        {
                            m_opcLink = new OpcLink();
                        }
                        else
                        {
                            m_opcLink.Disconnect();
                        }

                        //连接IPC服务器
                        if (m_opcLink.ConnectRemoteServer(strServerIP, strServerName))
                        {
                            if (m_opcLink.CreateGroup(strGroupName))
                            {
                                m_opcLink.SetGroupProperty(true, 0, Convert.ToInt32(strUpdateRate), true, true);

                                m_opcLink.CreateItems(m_dictTags.Values.ToArray(), OPC_Hander_Start);
                            }
                            
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 通过标签获取OPC数据
        /// </summary>
        /// <param name="sTag">标签名称</param>
        /// <returns></returns>
        public string ReadDataByTag(string sTag)
        {
            if (m_dictTags.ContainsValue(sTag))
            {
                return m_opcLink.GetValue(sTag);
            }

            return null;
        }

        /// <summary>
        /// 通过描述获取OPC数据
        /// </summary>
        /// <param name="sDesc">描述</param>
        /// <returns></returns>
        public string ReadDataByDesc(string sDesc)
        {
            if (m_dictTags.ContainsKey(sDesc))
            {
                return ReadDataByTag(m_dictTags[sDesc]);
            }

            return null;
        }

        /// <summary>
        /// 通过标签写OPC数据
        /// </summary>
        /// <param name="sTag"></param>
        /// <param name="_value"></param>
        public void WriteDataByTag(string sTag,object _value)
        {
            if(m_dictTags.ContainsValue(sTag))
            {
                m_opcLink.WriteValue(sTag, _value);
            }
        }

        /// <summary>
        /// 通过描述写OPC数据
        /// </summary>
        /// <param name="sDesc"></param>
        /// <param name="_value"></param>
        public void WriteDataByDesc(string sDesc,object _value)
        {
            if (m_dictTags.ContainsKey(sDesc))
            {
                WriteDataByTag(m_dictTags[sDesc],_value);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="handler"></param>
        public void RegistorMsg(DataChangeHandler handler)
        {
            if (m_opcLink.OpcConnected)
            {
                m_opcLink.OpcGroup.DataChange += new OPCAutomation.DIOPCGroupEvent_DataChangeEventHandler(handler);
            }
            
        }

        /// <summary>
        /// 更新内存参数到表格数据
        /// </summary>
        /// <param name="grid">表格</param>
        //public void UpdateGridFromParam(DataGridView grid)
        //{
        //    grid.Rows.Clear();
        //    //初始化DataGridView
        //    foreach (var item in m_dictTags)
        //    {
        //        int index = grid.Rows.Add();
        //        grid.Rows[index].Cells[0].Value = item.Value;
        //        grid.Rows[index].Cells[1].Value = item.Key;

        //        OpcInfo opc = m_opcLink.GetOpcItem(item.Value);

        //        if (opc != null)
        //        {
        //            grid.Rows[index].Cells[2].Value = opc.strValue;
        //            grid.Rows[index].Cells[3].Value = opc.nQuality;
        //            grid.Rows[index].Cells[4].Value = opc.strTimeStamp;
        //        }

                
        //    }
        //}

        /// <summary>
        /// 更新表格数据到内存参数
        /// </summary>
        /// <param name="grid">表格</param>
        //public void UpdateParamFromGrid(DataGridView grid)
        //{
        //    m_dictTags.Clear();
           
        //    foreach(DataGridViewRow item in grid.Rows)
        //    {
        //        object key = item.Cells[1].Value;
        //        object value = item.Cells[0].Value;

        //        if (key != null && value != null)
        //        {
        //            m_dictTags.Add(key.ToString(), value.ToString());
        //        }

                
        //    }
        //}

        /// <summary>
        /// 保存内存参数到xml文件
        /// </summary>
        /// <param name="doc"></param>
        public void SaveCfgXML(XmlDocument doc)
        {
            XmlNode xnl = doc.SelectSingleNode("SystemCfg");

            XmlNode root = xnl.SelectSingleNode("Opc");
            if (root == null)
            {
                root = doc.CreateElement("Opc");

                xnl.AppendChild(root);
            }

            root.RemoveAll();      

            XmlElement child = doc.CreateElement("Opc");

            child.SetAttribute("服务器IP", m_strServerIP);
            child.SetAttribute("服务器名称", m_strServerName);
            child.SetAttribute("组名称", m_strGroupName);
            child.SetAttribute("刷新频率", m_nUpdateRate.ToString());

            child.SetAttribute("启用", m_bOpcEnable.ToString());

            foreach (var item in m_dictTags)
            {
                XmlElement xe = doc.CreateElement("Tag");

                xe.SetAttribute("标签", item.Value);
                xe.SetAttribute("描述", item.Key);

                child.AppendChild(xe);
            }

            root.AppendChild(child);
        }


        /// <summary>
        /// 根据标签名称获取标签描述
        /// </summary>
        /// <param name="sTag">标签名称</param>
        /// <returns></returns>
        public string GetOpcTagDesc(string sTag)
        {
            string strDesc = string.Empty;
            foreach(var item in m_dictTags)
            {
                if(item.Value == sTag)
                {
                    strDesc = item.Key;

                    break;
                }
            }

            return strDesc;
        }

        /// <summary>
        /// 根据标签描述获取标签名称
        /// </summary>
        /// <param name="sTagDesc">标签描述</param>
        /// <returns></returns>
        public string GetOpcTagName(string sTagDesc)
        {
            string strTagName = string.Empty;

            if(m_dictTags.ContainsKey(sTagDesc))
            {
                strTagName = m_dictTags[sTagDesc];
            }

            return strTagName;
        }

    }
}
