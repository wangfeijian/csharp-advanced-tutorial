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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using log4net;
using log4net.Config;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;

namespace CommonTools.Tools
{
    /// <summary>
    /// 错误类型
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info = 0,

        /// <summary>
        /// 急停错误
        /// </summary>
        ErrEmg = 1,

        /// <summary>
        /// 报警
        /// </summary>
        Warn = 2,
      
        ///<summary>
        /// 轴控制错误
        /// </summary>
        ErrMotion = 1000, // 0x000003E8

        /// <summary>
        /// 轴初始化错误
        /// </summary>
        ErrMotionInit = 1001, // 0x000003E9

        /// <summary>
        /// 轴释放错误
        /// </summary>
        ErrMotionDeInit = 1002, // 0x000003EA

        /// <summary>
        /// 轴使能错误
        /// </summary>
        ErrMotionServoOn = 1003, // 0x000003EB

        /// <summary>
        /// 轴去使能错误
        /// </summary>
        ErrMotionServoOff = 1004, // 0x000003EC

        /// <summary>
        /// 轴回原点错误
        /// </summary>
        ErrMotionHome = 1005, // 0x000003ED

        /// <summary>
        /// 轴绝对运动错误
        /// </summary>
        ErrMotionAbs = 1006, // 0x000003EE

        /// <summary>
        /// 轴相对运动错误
        /// </summary>
        ErrMotionRel = 1007, // 0x000003EF

        /// <summary>
        /// 轴JOG运行错误
        /// </summary>
        ErrMotionJog = 1008, // 0x000003F0

        /// <summary>
        /// 轴速度运动错误
        /// </summary>
        ErrMotionVel = 1009, // 0x000003F1

        /// <summary>
        /// 轴停止错误
        /// </summary>
        ErrMotionStop = 1010, // 0x000003F2
        
        /// <summary>
        /// 轴急停错误
        /// </summary>
        ErrMotionEmgStop = 1011, // 0x000003F3

        /// <summary>
        /// 轴状态错误
        /// </summary>
        ErrMotionState = 1012, // 0x000003F4

        /// <summary>
        /// 设置轴位置错误
        /// </summary>
        ErrMotionSetPos = 1013, // 0x000003F5

        /// <summary>
        /// 设置轴参数错误
        /// </summary>
        ErrMotionSetParam = 1014, // 0x000003F6

        /// <summary>
        /// 轴运动超时
        /// </summary>
        ErrMotionTimeOut = 1015, // 0x000003F7

        /// <summary>
        /// 限位超时
        /// </summary>
        ErrMotionElTimeOut = 1016, // 0x000003F8

        /// <summary>
        /// 正限位超时
        /// </summary>
        ErrMotionPelTimeOut = 1017, // 0x000003F9

        /// <summary>
        /// 负限位超时
        /// </summary>
        ErrMotionMelTimeOut = 1018, // 0x000003FA

        /// <summary>
        /// 原点超时
        /// </summary>
        ErrMotionOrgTimeOut = 1019, // 0x000003FB

        /// <summary>
        /// HOME超时
        /// </summary>
        ErrMotionHomeTimeOut = 1020, // 0x000003FC

        /// <summary>
        /// IO控制错误
        /// </summary>                     
        ErrIo = 2000, // 0x000007D0

        /// <summary>
        /// IO初始化错误
        /// </summary>              
        ErrIoInit = 2001, // 0x000007D1

        /// <summary>
        /// IO读输入错误
        /// </summary>                   
        ErrIoReadIn = 2002, // 0x000007D2

        /// <summary>
        /// IO读输出错误
        /// </summary>                       
        ErrIoReadOut = 2003, // 0x000007D3

        /// <summary>
        /// IO写错误
        /// </summary>                        
        ErrIoWrite = 2004, // 0x000007D4

        /// <summary>
        /// IO状态错误
        /// </summary>                    
        ErrIoState = 2005, // 0x000007D5

        /// <summary>
        /// IO超时错误
        /// </summary>                   
        ErrIoTimeOut = 2006, // 0x000007D6

        /// <summary>
        /// 机器人错误
        /// </summary>                       
        ErrRobot = 3000, // 0x00000BB8

        /// <summary>
        /// 串口通信错误
        /// </summary>                  
        ErrCom = 4000, // 0x00000FA0

        /// <summary>
        /// 串口打开错误
        /// </summary>                
        ErrComOpen = 4001, // 0x00000FA1

        /// <summary>
        /// 串口读错误
        /// </summary>                     
        ErrComRead = 4002, // 0x00000FA2

        /// <summary>
        /// 串口写错误
        /// </summary>                     
        ErrComWrite = 4003, // 0x00000FA3

        /// <summary>
        /// 串口超时错误
        /// </summary>                      
        ErrComTimeOut = 4004, // 0x00000FA4

        /// <summary>
        /// 网络通信错误
        /// </summary>                       
        ErrTcp = 5000, // 0x00001388

        /// <summary>
        /// 网络打开错误
        /// </summary>               
        ErrTcpOpen = 5001, // 0x00001389

        /// <summary>
        /// 网络读错误
        /// </summary>                     
        ErrTcpRead = 5002, // 0x0000138A

        /// <summary>
        /// 网络写错误
        /// </summary>                     
        ErrTcpWrite = 5003, // 0x0000138B

        /// <summary>
        /// 网络超时错误
        /// </summary>                     
        ErrTcpTimeOut = 5004, // 0x0000138C

        /// <summary>
        /// OPC通信错误
        /// </summary>                        
        ErrOpc = 6000, // 0x00001770

        /// <summary>
        /// OPC打开错误
        /// </summary>      
        ErrOpcOpen = 6001, // 0x00001771

        /// <summary>
        /// OPC读错误
        /// </summary>                 
        ErrOpcRead = 6002, // 0x00001772

        /// <summary>
        /// OPC写错误
        /// </summary>            
        ErrOpcWrite = 6003, // 0x00001773

        /// <summary>
        /// OPC超时错误
        /// </summary>                  
        ErrOpcTimeOut = 6004, // 0x00001774

        /// <summary>
        /// PLC通信错误
        /// </summary>                 
        ErrPlc = 7000, // 0x00001B58

        /// <summary>
        /// PLC打开错误
        /// </summary>              
        ErrPlcOpen = 7001, // 0x00001B59

        /// <summary>
        /// PLC读错误
        /// </summary>                    
        ErrPlcRead = 7002, // 0x00001B5A

        /// <summary>
        /// PLC写错误
        /// </summary>                 
        ErrPlcWrite = 7003, // 0x00001B5B

        /// <summary>
        /// PLC超时错误
        /// </summary>                  
        ErrPlcTimeOut = 7004, // 0x00001B5C

        /// <summary>
        /// 流程错误
        /// </summary>                     
        ErrWorkFlow = 8000, // 0x00001F40

        /// <summary>
        /// 位寄存器超时
        /// </summary>                     
        ErrRegBitTimeOut = 8001, // 0x00001F41

        /// <summary>
        /// 整型寄存器超时
        /// </summary>                         
        ErrRegIntTimeOut = 8002, // 0x00001F42

        /// <summary>
        /// 浮点寄存器超时
        /// </summary>                          
        ErrRegDoubleTimeOut = 8003, // 0x00001F43

        /// <summary>
        /// 字符串寄存器超时
        /// </summary>                              
        ErrRegStringTimeOut = 8004, // 0x00001F44

        /// <summary>
        /// 视觉错误
        /// </summary>                             
        ErrVision = 9000, // 0x00002328

        /// <summary>
        /// 视觉打开错误
        /// </summary>                   
        ErrVisionOpen = 9001, // 0x00002329

        /// <summary>
        /// 视觉拍照错误
        /// </summary>                        
        ErrVisionSnap = 9002, // 0x0000232A

        /// <summary>
        /// 视觉处理错误
        /// </summary>                        
        ErrVisionProcess = 9003, // 0x0000232B

        /// <summary>
        /// 视觉参数错误
        /// </summary>                           
        ErrVisionParam = 9004, // 0x0000232C

        /// <summary>
        /// 系统错误
        /// </summary>                         
        ErrSystem = 10000, // 0x00002710

        /// <summary>
        /// 未定义错误
        /// </summary>                    
        ErrUnd = 99999, // 0x0001869F
    }

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
        /// <returns></returns>
        public bool ReadXmlConfig(string path)
        {
            if(!CheckLogPath(path)) return false;
            XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "Config\\"+path+"\\log4net.config"));
            return true;
        }

        /// <summary>
        /// 根据当前配置的日志文件路径变更log4net的存储路径
        /// </summary>
        private bool CheckLogPath(string path)
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                string filename = AppDomain.CurrentDomain.BaseDirectory + "Config\\"+path+"\\log4net.config";
                xmlDocument.Load(filename);
                if (!xmlDocument.HasChildNodes)
                    return false;
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
                                                string str2 = attribute.Substring(length + 1, attribute.Length - length - 1);
                                                string str3 = paramString + "\\" + str2 + "\\";
                                                xmlElement.SetAttribute("value", str3);
                                                flag = true;
                                            }
                                            else
                                            {
                                                MessageBox.Show(LocationServices.GetLang("LogPathSetError"),
                                                    LocationServices.GetLang("Tips"), MessageBoxButton.OK,
                                                    MessageBoxImage.Error);
                                                flag = false;
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
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show(LocationServices.GetLang("LogConfigReadError"),
                                                    LocationServices.GetLang("Tips"), MessageBoxButton.OK,
                                                    MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// 保存一条信息日志
        /// </summary>
        /// <param name="strMsg"></param>
        public void Info(string strMsg)
        {
            StringBuilder stringBuilder = new StringBuilder();
            strMsg = strMsg.Replace(",", "，");
            strMsg = strMsg.Replace("\r\n", "");
            strMsg = strMsg.Replace("\r", "");
            strMsg = strMsg.Replace("\n", "");
            stringBuilder.Append(0.ToString());
            stringBuilder.Append(",");
            stringBuilder.Append(ErrorType.Info.ToString());
            stringBuilder.Append(",");
            stringBuilder.Append(strMsg);
            LogManager.GetLogger("root").Info(stringBuilder.ToString());
        }

        /// <summary>
        /// 系统产生警告时调用,增加一条警告日志,并保存到错误日志文件和系统日志
        /// </summary>
        /// <param name="strMsg"></param>
        public void Warning(string strMsg)
        {
            strMsg = strMsg.Replace(",", "，");
            strMsg = strMsg.Replace("\r\n", "");
            strMsg = strMsg.Replace("\r", "");
            strMsg = strMsg.Replace("\n", "");
            LogManager.GetLogger("root").Warn(AddNewError(strMsg, "WARN").Log);
        }

        /// <summary>
        /// 系统产生警告时调用,增加一条警告日志,并保存到错误日志文件和系统日志
        /// </summary>
        /// <param name="strMsg"></param>
        /// <param name="strObject">报警对象</param>
        public void Warning(string strObject, string strMsg)
        {
            StringBuilder stringBuilder = new StringBuilder();
            strMsg = strMsg.Replace(",", "，");
            strMsg = strMsg.Replace("\r\n", "");
            strMsg = strMsg.Replace("\r", "");
            strMsg = strMsg.Replace("\n", "");
            stringBuilder.Append(2.ToString());
            stringBuilder.Append(",");
            stringBuilder.Append(ErrorType.Warn.ToString());
            stringBuilder.Append(",");
            stringBuilder.Append(strObject);
            stringBuilder.Append(",");
            stringBuilder.Append(strMsg);
            stringBuilder.Append(",");
            LogManager.GetLogger("root").Warn(AddNewError(stringBuilder.ToString(), "WARN").Log);
        }

        /// <summary>
        /// 系统产生错误时调用,增加一条错误日志,并保存到错误日志文件和系统日志
        /// </summary>
        /// <param name="strMsg"></param>
        public void Error(string strMsg)
        {
            strMsg = strMsg.Replace(",", "，");
            strMsg = strMsg.Replace("\r\n", "");
            strMsg = strMsg.Replace("\r", "");
            strMsg = strMsg.Replace("\n", "");
            LogManager.GetLogger("root").Error(AddNewError(strMsg, "ERROR").Log);
        }

        /// <summary>
        /// 系统产生错误时调用,增加一条错误日志,并保存到错误日志文件和系统日志
        /// </summary>
        /// <param name="type">错误类型</param>
        /// <param name="strObject">错误对象</param>
        /// <param name="strMsg">错误消息</param>
        public void Error(ErrorType type, string strObject, string strMsg)
        {
            StringBuilder stringBuilder = new StringBuilder();
            strMsg = strMsg.Replace(",", "，");
            strMsg = strMsg.Replace("\r\n", "");
            strMsg = strMsg.Replace("\r", "");
            strMsg = strMsg.Replace("\n", "");
            stringBuilder.Append(((int)type).ToString());
            stringBuilder.Append(",");
            stringBuilder.Append(type.ToString());
            stringBuilder.Append(",");
            stringBuilder.Append(strObject);
            stringBuilder.Append(",");
            stringBuilder.Append(strMsg);
            LogManager.GetLogger("root").Error(AddNewError(stringBuilder.ToString(), "ERROR").Log);
        }

        /// <summary>
        /// 系统产生错误时调用,增加一条错误日志,并保存到错误日志文件和系统日志
        /// </summary>
        /// <param name="strErrCode"></param>
        /// <param name="strType"></param>
        /// <param name="strObject"></param>
        /// <param name="strMsg"></param>
        public void Error(string strErrCode, string strType, string strObject, string strMsg)
        {
            StringBuilder stringBuilder = new StringBuilder();
            strMsg = strMsg.Replace(",", "，");
            strMsg = strMsg.Replace("\r\n", "");
            strMsg = strMsg.Replace("\r", "");
            strMsg = strMsg.Replace("\n", "");
            stringBuilder.Append(strErrCode);
            stringBuilder.Append(",");
            stringBuilder.Append(strType);
            stringBuilder.Append(",");
            stringBuilder.Append(strObject);
            stringBuilder.Append(",");
            stringBuilder.Append(strMsg);
            LogManager.GetLogger("root").Error(AddNewError(stringBuilder.ToString(), "ERROR").Log);
        }

        /// <summary>增加一条新的错误信息</summary>
        /// <param name="strMessage"></param>
        /// <param name="strLevel"></param>
        private WarningData AddNewError(string strMessage, string strLevel)
        {
            WarningData wd = new WarningData();
            wd.Time = DateTime.Now;
            wd.Level = strLevel;
            string[] strArray = strMessage.Split(',');
            if (strArray.Length < 3)
            {
                wd.Code = "99999";
                wd.Category = "Err_Und";
                wd.Msg = strMessage;
            }
            else
            {
                wd.Category = strArray[0];
                wd.Category = strArray[1];
                int index1 = 2;
                if (strArray.Length > 4)
                {
                    wd.Object = strArray[2];
                    index1 = 3;
                }
                StringBuilder stringBuilder = new StringBuilder(strArray[index1]);
                for (int index2 = index1 + 1; index2 < strArray.Length; ++index2)
                {
                    stringBuilder.Append("，");
                    stringBuilder.Append(strArray[index2]);
                }
                wd.Msg = stringBuilder.ToString();
            }
            // ISSUE: reference to a compiler-generated field
            if (TransformEvent != null)
            {
                // ISSUE: reference to a compiler-generated field
                wd = TransformEvent(wd);
            }
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.Append(wd.Code);
            stringBuilder1.Append(",");
            stringBuilder1.Append(wd.Category);
            stringBuilder1.Append(",");
            stringBuilder1.Append(wd.Msg);
            wd.Log = stringBuilder1.ToString();
            lock (_syncLock)
            {
                _listError.Add(wd);
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                WarningEventHandler?.Invoke(this, new WarningEventData(true, _listError.Count - 1));
                if (_bEnableSnapScreen)
                {
                    SnapScreen();
                    _bEnableSnapScreen = false;
                }
                return wd;
            }
        }

        /// <summary>
        /// 判断当前是否存在错误信息
        /// </summary>
        /// <returns></returns>
        public bool HasErrorMsg()
        {
            return _listError.Count > 0;
        }

        /// <summary>
        /// 当前错误报警信息的总个数
        /// </summary>
        public int Count => _listError.Count;

        /// <summary>
        /// 得到错误信息中的最后一条信息
        /// </summary>
        /// <returns></returns>
        public WarningData GetLastMsg()
        {
            return _listError.Last();
        }

        /// <summary>
        /// 获取指定索引位的错误信息
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public WarningData GetWarning(int nIndex)
        {
            if (nIndex < _listError.Count)
                return _listError.ElementAt(nIndex);
            Debug.WriteLine("WarningMgr GetWarning, nIndex > m_listError.Count");
            return new WarningData();
        }

        /// <summary>
        /// 保存错误信息到错误日志文件
        /// </summary>
        /// <param name="wd"></param>
        private void SaveError(WarningData wd)
        {
            StringBuilder stringBuilder = new StringBuilder(wd.Time.ToShortDateString());
            stringBuilder.Append(",");
            stringBuilder.Append(wd.Time.ToLongTimeString());
            stringBuilder.Append(",");
            stringBuilder.Append(wd.Level);
            stringBuilder.Append(",");
            stringBuilder.Append(wd.Code);
            stringBuilder.Append(",");
            stringBuilder.Append(wd.Category);
            stringBuilder.Append(",");
            stringBuilder.Append(wd.Msg);
            stringBuilder.Append(",");
            TimeSpan timeSpan = DateTime.Now - wd.Time;
            stringBuilder.Append(timeSpan.ToString("hh\\:mm\\:ss"));
            LogManager.GetLogger("ErrorLog").Info(stringBuilder.ToString());
        }

        /// <summary>
        /// 清除m_listError的一条信息,并保存此条信息到错误日志文件
        /// </summary>
        /// <param name="nIndex"></param>
        public void ClearWarning(int nIndex)
        {
            if (nIndex < 0)
                return;
            if (nIndex < _listError.Count)
            {
                lock (_syncLock)
                {
                    SaveError(_listError.ElementAt(nIndex));
                    _listError.RemoveAt(nIndex);
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                WarningEventHandler?.Invoke(this, new WarningEventData(false, nIndex));
            }
            if (_listError.Count != 0)
                return;
            _bEnableSnapScreen = true;
        }

        /// <summary>
        /// 清空当前错误信息列表,并保存信息到错误日志文件
        /// </summary>
        public void ClearAllWarning()
        {
            if (_listError.Count <= 0)
                return;
            lock (_syncLock)
            {
                foreach (WarningData wd in _listError)
                    SaveError(wd);
                _listError.Clear();
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            WarningEventHandler?.Invoke(this, new WarningEventData(false, -1));
            _bEnableSnapScreen = true;
        }

        /// <summary>截屏</summary>
        public void SnapScreen()
        {
            new Task(obj =>
            {
                int    width  = (int)SystemParameters.WorkArea.Width;
                int    height = (int)SystemParameters.WorkArea.Height;
                Bitmap bitmap = new Bitmap(width, height);
                Graphics.FromImage(bitmap).CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height));
                DateTime now = DateTime.Now;
                string path = SingletonPattern<SystemManager>.GetInstance().GetImagePath() + "\\ErrorPicture\\" +
                              $"{(object) now:yyyy-MM-dd}";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string filename = path + "\\" + $"{(object) now:HH-mm-ss-fff}.png";
                bitmap.Save(filename, ImageFormat.Png);
            }, "").Start();
        }
    }
}
