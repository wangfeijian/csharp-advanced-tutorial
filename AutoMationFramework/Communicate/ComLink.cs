/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-08-07                               *
*                                                                    *
*           ModifyTime:     2021-08-07                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Com port class                           *
*********************************************************************/
using System;
using System.IO.Ports;
using System.Diagnostics;
using AutoMationFrameworkSystemDll;
using CommonTools.Tools;

namespace Communicate
{
    /// <summary>
    /// 串口通讯类封装
    /// </summary>
    public class ComLink : LogBase
    {
        /// <summary>
        ///串口号 
        /// </summary>
        public int ComNo;

        /// <summary>
        ///串口定义名称 
        /// </summary>
        public string StrName;

        /// <summary>
        ///波特率 
        /// </summary>
        public int BaudRate;

        /// <summary>
        ///数据位 
        /// </summary>
        public int DataBit;

        /// <summary>
        ///校验位 
        /// </summary>
        public string StrPartiy;

        /// <summary>
        ///停止位 
        /// </summary>
        public string StrStopBit;

        /// <summary>
        ///流控制 
        /// </summary>
        public string StrFlowCtrl;

        /// <summary>
        ///超时时间,单位毫秒
        /// </summary>
        public int Time;

        /// <summary>
        ///缓冲区大小 
        /// </summary>
        public int BufferSzie;

        /// <summary>
        ///命令分隔符标志 
        /// </summary>
        public string StrLineFlag;

        /// <summary>
        ///命令分隔符 
        /// </summary>
        private string StrLine;

        /// <summary>
        /// 状态变更委托
        /// </summary>
        /// <param name="com"></param>
        public delegate void StateChangedHandler(ComLink com);

        /// <summary>
        /// 定义状态变更事件
        /// </summary>
        public event StateChangedHandler StateChangedEvent;

        private bool _bAsysnReceive; //异步接收标志

        /// <summary>
        /// 异步接收数据委托
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        public delegate void DataReceivedHandler(byte[] data, int length);
        /// <summary>
        /// 异步接收数据事件
        /// </summary>
        public event DataReceivedHandler DataReceivedEvent;

        /// <summary>
        /// 系统串口类引用
        /// </summary>
        private SerialPort _serialPort;

        /// <summary>
        /// 读取数据过程中是否已经超时
        /// </summary>
        bool _bTimeOut;

        private object m_lock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nComNo"></param>
        /// <param name="strName"></param>
        /// <param name="nBaudRate"></param>
        /// <param name="nDataBit"></param>
        /// <param name="strPartiy"></param>
        /// <param name="strStopBit"></param>
        /// <param name="strFlowCtrl"></param>
        /// <param name="nTimeMs"></param>
        /// <param name="nBufferSzie"></param>
        /// <param name="strLine"></param>
        public ComLink(int nComNo, string strName, int nBaudRate, int nDataBit, string strPartiy,
            string strStopBit, string strFlowCtrl, int nTimeMs, int nBufferSzie, string strLine)
        {
            ComNo = nComNo;
            StrName = strName;
            BaudRate = nBaudRate;
            DataBit = nDataBit;
            StrPartiy = strPartiy;
            StrStopBit = strStopBit;
            StrFlowCtrl = strFlowCtrl;
            Time = nTimeMs;
            BufferSzie = nBufferSzie;

            StrLineFlag = strLine;
            if (strLine == "CRLF")
            {
                StrLine = "\r\n";
            }
            else if (strLine == "CR")
            {
                StrLine = "\r";
            }
            else if (strLine == "LF")
            {
                StrLine = "\n";
            }
            else if (strLine == "无")
            {
                StrLine = "";
            }
            else if (strLine == "ETX")
            {
                StrLine = "\u0003";
            }
        }


        /// <summary>
        ///打开串口
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (_serialPort == null)
                _serialPort = new SerialPort();

            if (_serialPort.IsOpen == false)
            {
                _serialPort.PortName = "COM" + ComNo.ToString();
                _serialPort.BaudRate = BaudRate;
                _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), StrPartiy);
                _serialPort.DataBits = DataBit;
                _serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), StrStopBit);
                _serialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), StrFlowCtrl);

                _serialPort.ReadTimeout = Time;
                _serialPort.WriteTimeout = Time;

                _serialPort.NewLine = StrLine;

                try
                {
                    _serialPort.Open();
                    if (StateChangedEvent != null)
                        StateChangedEvent(this);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("串口 {0} 打开失败\r\n{1}\r\n", StrName, e);
                    if (SystemManager.GetInstance().IsSimulateRunMode() == false)
                    {
                        //WarningMgr.GetInstance().Error(string.Format("51220,ERR-SSW,串口 {0} 打开失败", StrName));
                        RunInforManager.GetInstance().Error(ErrorType.ErrComOpen, StrName,
                            string.Format("SerialPort {0} open failed", StrName));

                    }
                }
            }
            return _serialPort.IsOpen;
        }

        /// <summary>
        /// 判断是否已经打开
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return _serialPort != null && _serialPort.IsOpen;
        }

        /// <summary>
        /// 判断是否超时
        /// </summary>
        /// <returns></returns>
        public bool IsTimeOut()
        {
            return _bTimeOut;
        }

        /// <summary>
        ///向串口写入数据 
        /// </summary>
        /// <param name="sendBytes"></param>
        /// <param name="nLen"></param>
        /// <returns></returns>
        public bool WriteData(byte[] sendBytes, int nLen)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Write(sendBytes, 0, nLen);

                string strData = Win32Help.ByteToAsciiString(sendBytes);
                string strLog = $"Send to {StrName} : {strData}";
                ShowLog(strLog);
                RunInforManager.GetInstance().Info(strLog);

                return true;
            }
            return false;
        }

        /// <summary>
        ///向串口写入字符串 
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool WriteString(string strData)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Write(strData);

                string strLog = string.Format("Send to {0} : {1}", StrName, strData);
                ShowLog(strLog);
                RunInforManager.GetInstance().Info(strLog);

                return true;
            }
            return false;
        }

        /// <summary>
        ///向串口写入一行字符 
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public bool WriteLine(string strData)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.WriteLine(strData);

                string strLog = string.Format("Send to {0} : {1}", StrName, strData);
                ShowLog(strLog);
                RunInforManager.GetInstance().Info(strLog);

                return true;
            }
            return false;
        }

        /// <summary>
        ///从串口读取数据 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="nLen"></param>
        /// <returns></returns>
        public int ReadData(byte[] bytes, int nLen)
        {
            _bTimeOut = false;
            int nActualReadCount = 0;
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    //有些串口驱动超时无效提前返回的BUG，需要多次读取
                    while (nActualReadCount < nLen)
                    {
                        TimeSpan diffT = DateTime.Now - startTime;
                        if (diffT.TotalMilliseconds > Time)
                        {
                            break;
                        }

                        int rcvCnt = _serialPort.Read(bytes, nActualReadCount, nLen - nActualReadCount);
                        nActualReadCount += rcvCnt;
                        if (nActualReadCount > 0)
                        {
                            string strData = Win32Help.ByteToAsciiString(bytes);

                            string strLog = string.Format("Receive from {0} : {1}", StrName, strData);
                            ShowLog(strLog);
                            RunInforManager.GetInstance().Info(strLog);

                        }
                    }
                }
                catch/*(TimeoutException e)*/
                {
                    _bTimeOut = true;
                    if (StateChangedEvent != null)
                        StateChangedEvent(this);
                }
            }
            return nActualReadCount;
        }

        //public int ReadData(byte[] bytes, int nLen)
        //{
        //    _bTimeOut = false;
        //    int nReadCount = 0;
        //    if (_serialPort.IsOpen)
        //    {
        //        try
        //        {
        //            nReadCount = _serialPort.Read(bytes, 0, nLen);
        //            if (nReadCount > 0)
        //                ShowLog(System.Text.Encoding.Default.GetString(bytes));
        //        }
        //        catch/*(TimeoutException e)*/
        //        {
        //            _bTimeOut = true;
        //            if (StateChangedEvent != null)
        //                StateChangedEvent(this);
        //        }
        //    }
        //    return nReadCount;
        //}


        /// <summary>
        ///从串口读取一行数据 
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public int ReadLine(out string strData)
        {
            _bTimeOut = false;
            strData = "";
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    strData = _serialPort.ReadLine();
                    if (strData.Length > 0)
                    {
                        string strLog = $"Receive from {StrName} : {strData}";

                        ShowLog(strLog);

                        RunInforManager.GetInstance().Info(strLog);
                    }
                }
                catch/*(TimeoutException e)*/
                {
                    _bTimeOut = true;
                    if (StateChangedEvent != null)
                        StateChangedEvent(this);
                }
            }
            return strData.Length;
        }

        /// <summary>
        ///关闭串口 
        /// </summary>
        public void Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort = null;
                _bTimeOut = false;
                if (StateChangedEvent != null)
                    StateChangedEvent(this);
            }
        }


        /// <summary>
        /// 清除缓冲区
        /// </summary>
        /// <param name="bIn">是否清除输入缓冲区</param>
        /// <param name="bOut">是否清除输出缓冲区</param>
        public void ClearBuffer(bool bIn, bool bOut)
        {
            if (_serialPort != null)
            {
                if (bIn)
                    _serialPort.DiscardInBuffer();
                if (bOut)
                    _serialPort.DiscardOutBuffer();

            }
        }

        /// <summary>
        /// 异步接收数据
        /// </summary>
        /// <param name="hander"></param>
        public void BeginAsynReceive(DataReceivedHandler hander)
        {
            _bAsysnReceive = true;
            DataReceivedEvent += hander;
            _serialPort.DataReceived += SerialPort_DataReceived;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (DataReceivedEvent != null && _bAsysnReceive)
            {
                int length = _serialPort.BytesToRead;

                if (length > 0)
                {
                    byte[] data = new byte[length];
                    _serialPort.Read(data, 0, length);

                    DataReceivedEvent(data, length);
                }

            }
        }

        /// <summary>
        /// 结束异步接收数据
        /// </summary>
        public void EndAsynReceive()
        {
            _bAsysnReceive = false;
            if (DataReceivedEvent != null)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;

                foreach (var d in DataReceivedEvent.GetInvocationList())
                {
                    DataReceivedEvent -= d as DataReceivedHandler;
                }
            }

        }

        /// <summary>
        /// 上锁
        /// </summary>
        public void Lock()
        {
            System.Threading.Monitor.Enter(m_lock);
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public void UnLock()
        {
            if (System.Threading.Monitor.IsEntered(m_lock))
            {
                System.Threading.Monitor.Exit(m_lock);
            }

        }
    }
}
