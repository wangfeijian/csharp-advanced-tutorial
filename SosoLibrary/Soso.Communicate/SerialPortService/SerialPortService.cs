#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Communicate.SerialPortService
 * 唯一标识：0ab41bb7-f0a2-49b0-b623-1d6844be4d3a
 * 文件名：SerialPortService
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 9:30:35 AM
 * 版本：V1.0.0
 * 描述：
 * 1、重新新建文件，自动添加版本注释
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using Soso.Contract.Model;
using System;
using System.IO.Ports;

namespace Soso.Communicate.SerialPortService
{
    public class SerialPortService
    {
        private SerialPort _serialPort;
        private Parity _parity;
        private StopBits _stopBits;
        private Handshake _handshake;
        public SerialPortParameter Parameter { get; private set; }

        public SerialPortService(SerialPortParameter parameter)
        {
            Parameter = parameter;
            Parameter.EndMark = EndMarkParse.Parse(parameter.EndMark);

            try
            {
                _parity = (Parity)Enum.Parse(typeof(Parity), parameter.ParityBit);
                _stopBits = (StopBits)Enum.Parse(typeof(StopBits), parameter.StopBit);
                _handshake = (Handshake)Enum.Parse(typeof(Handshake), parameter.FlowControl);
            }
            catch (Exception ex)
            {
                throw new Exception("Serial port parameter error!", ex);
            }
        }

        public bool Open()
        {
            if (_serialPort == null)
                _serialPort = new SerialPort();

            if (_serialPort.IsOpen == false)
            {
                _serialPort.PortName = Parameter.Name;
                _serialPort.BaudRate = Parameter.BaudRate;
                _serialPort.Parity = _parity;
                _serialPort.DataBits = Parameter.DataBit;
                _serialPort.StopBits = _stopBits;
                _serialPort.Handshake = _handshake;

                _serialPort.ReadTimeout = Parameter.TimeOut;
                _serialPort.WriteTimeout = Parameter.TimeOut;

                _serialPort.NewLine = Parameter.EndMark;

                try
                {
                    _serialPort.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Serial port {Parameter.Description} open fail!", ex);
                }
            }
            return _serialPort.IsOpen;
        }

        public bool IsOpen => _serialPort != null && _serialPort.IsOpen;

        /// <summary>
        /// 往串口中写字节数组
        /// </summary>
        /// <param name="sendBytes">字节数组</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public bool WriteData(byte[] sendBytes, int len)
        {
            if (IsOpen)
            {
                _serialPort.Write(sendBytes, 0, len);

                return true;
            }
            return false;
        }

        /// <summary>
        /// 往串口中写入字符串
        /// </summary>
        /// <param name="sendStr"></param>
        /// <returns></returns>
        public bool WriteString(string sendStr)
        {
            if (IsOpen)
            {
                _serialPort.Write(sendStr);

                return true;
            }
            return false;
        }

        /// <summary>
        /// 往串口中写入一行数据
        /// </summary>
        /// <param name="sendStr"></param>
        /// <returns></returns>
        public bool WriteLine(string sendStr)
        {
            if (IsOpen)
            {
                _serialPort.WriteLine(sendStr);

                return true;
            }
            return false;
        }

        /// <summary>
        /// 从串口中读取一定长度的字节数组
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="len"></param>
        /// <returns>-1：超时；0：失败；字节长度</returns>
        public int ReadData(byte[] bytes, int len)
        {
            int readCount = 0;
            if (IsOpen)
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    while (readCount < len)
                    {
                        TimeSpan timeSpan = DateTime.Now - startTime;
                        if (timeSpan.TotalMilliseconds > Parameter.TimeOut)
                        {
                            break;
                        }

                        int recevieCount = _serialPort.Read(bytes, readCount, len - readCount);
                        readCount += recevieCount;
                    }
                }
                catch
                {
                    return -1;
                }
            }
            return readCount;
        }

        /// <summary>
        /// 从串口中读取一行数据
        /// </summary>
        /// <param name="readStr"></param>
        /// <returns>-1：超时；0：失败；字符长度</returns>
        public int ReadLine(out string readStr)
        {
            readStr = "";
            if (IsOpen)
            {
                try
                {
                    readStr = _serialPort.ReadLine();
                }
                catch
                {
                    return -1;
                }
            }
            return readStr.Length;
        }

        public bool Close()
        {
            if (IsOpen)
            {
                _serialPort.Close();
                return true;
            }

            return false;
        }

        public void ClearBuffer(bool input, bool output)
        {
            if (IsOpen)
            {
                if (input)
                {
                    _serialPort.DiscardInBuffer();
                }

                if (output)
                {
                    _serialPort.DiscardOutBuffer();
                }
            }
        }
    }
}
