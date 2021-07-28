/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-28                               *
*                                                                    *
*           ModifyTime:     2021-07-28                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Run information manager class            *
*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using log4net.Config;

namespace CommonTools.Manager
{
    /// <summary>
    /// 报警数据
    /// </summary>
    public class WarningData
    {
        /// <summary>
        /// 报警时间
        /// </summary>
        public DateTime Time;

        /// <summary>
        /// 报警等级
        /// </summary>
        public string Level;

        /// <summary>
        /// 报警错误码
        /// </summary>
        public string Code;

        /// <summary>
        /// 报警种类
        /// </summary>
        public string Category;

        /// <summary>
        /// 报警对象
        /// </summary>
        public string Object;

        /// <summary>
        /// 报警信息
        /// </summary>
        public string Msg;

        /// <summary>
        /// 包含完整信息的log
        /// </summary>
        public string Log;
    }

    /// <summary>
    /// 报警事件类封装
    /// </summary>
    public class WarningEventData : EventArgs
    {
        /// <summary>
        /// 判断是增加还是删除信息
        /// </summary>
        public bool BAdd;

        /// <summary>
        /// 增加或删除信息的索引号
        /// </summary>
        public int Index;

        /// <summary>构造函数</summary>
        /// <param name="bAdd"></param>
        /// <param name="nIndex"></param>
        public WarningEventData(bool bAdd, int nIndex)
        {
            BAdd = bAdd;
            Index = nIndex;
        }
    }


    /// <summary>
    /// 软件运行信息管理类
    /// 包括正常、报警、错误信息
    /// </summary>
    public class RunInforManager : SingletonPattern<RunInforManager>
    {
        /// <summary>
        /// 自定义转换委托
        /// </summary>
        /// <param name="wd"></param>
        /// <returns></returns>
        public delegate WarningData TransformHandler(WarningData wd);

        /// <summary>
        /// 内存中的错误列表
        /// </summary>
        private List<WarningData> _listError = new List<WarningData>();

        /// <summary>
        /// 多线程互斥锁
        /// </summary>
        private readonly object _syncLock = new object();

        private bool _bEnableSnapScreen = true;

        /// <summary>
        /// 报警事件
        /// </summary>
        public event EventHandler WarningEventHandler;

        /// <summary>自定义转换事件</summary>
        public event RunInforManager.TransformHandler TransformEvent;

        /// <summary>
        /// 读取日志参数配置
        /// </summary>
        /// <param name="strFile"></param>
        /// <returns></returns>
        public bool ReadXmlConfig(string strFile)
        {
            this.CheckLogPath();
            XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Config\\log4net.config"));
            return true;
        }

        /// <summary>
        /// 根据当前配置的日志文件路径变更log4net的存储路径
        /// </summary>
        private void CheckLogPath()
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                string filename = AppDomain.CurrentDomain.BaseDirectory + "Config\\log4net.config";
                xmlDocument.Load(filename);
                if (!xmlDocument.HasChildNodes)
                    return;
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/configuration/log4net");
                bool flag = false;
                if (xmlNodeList != null && xmlNodeList.Count > 0)
                {
                    XmlNodeList childNodes = xmlNodeList.Item(0)?.ChildNodes;
                    if (childNodes != null && childNodes.Count > 0)
                    {
                        foreach (XmlNode xmlNode in childNodes)
                        {
                            if (xmlNode.Name == "appender")
                            {
                                foreach (XmlNode childNode in xmlNode.ChildNodes)
                                {
                                    if (childNode.Name == "file")
                                    {
                                        XmlElement xmlElement = (XmlElement)childNode;
                                        string attribute = xmlElement.GetAttribute("value");
                                        if (attribute.Length > 0)
                                        {
                                            string paramString = SingletonPattern<SystemManager>.GetInstance().GetParamString("LogSavePath");
                                            string str1 = "";
                                            int num1 = attribute.LastIndexOf('\\');
                                            int length = attribute.LastIndexOf('\\', attribute.Length - 2, attribute.Length - 1);
                                            if (num1 == -1 && length == -1)
                                            {
                                                num1 = attribute.LastIndexOf('/');
                                                length = attribute.LastIndexOf('/', attribute.Length - 2, attribute.Length - 1);
                                            }
                                            if (length != -1 && num1 != -1 && num1 > length)
                                            {
                                                string str2 = attribute.Substring(0, length);
                                                string str3 = attribute.Substring(length + 1, num1 - length - 1);
                                                if (paramString != str2)
                                                {
                                                    string str4 = paramString + "\\" + str3 + "\\";
                                                    xmlElement.SetAttribute("value", str4);
                                                    flag = true;
                                                }
                                            }
                                            else if ((num1 == -1 || num1 == length) && length != -1)
                                            {
                                                str1 = attribute.Substring(0, length);
                                                string str2 = attribute.Substring(length + 1, attribute.Length - length - 1);
                                                string str3 = paramString + "\\" + str2 + "\\";
                                                xmlElement.SetAttribute("value", str3);
                                                flag = true;
                                            }
                                            else
                                            {
                                                //if ((uint)SingletonTemplate<LanguageMgr>.GetInstance().LanguageID > 0U)
                                                //{
                                                //    int num2 = (int)MessageBox.Show("Log4net.config configuration file path setting error");
                                                //}
                                                //else
                                                //{
                                                //    int num3 = (int)MessageBox.Show("log4net.config配置文件路径设置错误");
                                                //}
                                                //flag = false;
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (flag)
                    xmlDocument.Save(filename);
            }
            catch (Exception ex)
            {
                //if ((uint)SingletonTemplate<LanguageMgr>.GetInstance().LanguageID > 0U)
                //{
                //    int num1 = (int)MessageBox.Show(ex.ToString(), "Log system configuration file read failed", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                //}
                //else
                //{
                //    int num2 = (int)MessageBox.Show(ex.ToString(), "日志系统配置文件读取失败", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                //}
            }
        }
    }
}
