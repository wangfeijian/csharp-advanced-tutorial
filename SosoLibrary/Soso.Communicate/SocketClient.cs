using Soso.Contract;
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;

namespace Soso.Communicate
{
    public class SocketClient
    {
        private EasyClient<StringPackageInfo> easyClient;
        private BlockingCollection<string> _blockingCollection = new BlockingCollection<string>();
        public SocketClientParameter Parameter { get; private set; }

        public event EventHandler<string> Error;
        public event EventHandler<string> ServersClosed;
        public SocketClient(SocketClientParameter parameter)
        {
            Parameter = parameter;
            if (Parameter.EndMark.Contains("\\") || string.IsNullOrWhiteSpace(Parameter.EndMark))
            {
                Parameter.EndMark = "\r\n";
            }
        }

        /// <summary>
        /// 判断网口是否打开
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return easyClient != null && easyClient.IsConnected;
        }

        /// <summary>
        /// 打开网口
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            easyClient = new EasyClient<StringPackageInfo>();
            if (!string.IsNullOrEmpty(Parameter.BeginMark))
            {
                easyClient.Initialize(new BeginEndMarkFilter(Parameter.BeginMark, Parameter.EndMark, Parameter.CommandSpliter, Parameter.ParameterSpliter));
            }
            else
            {
                easyClient.Initialize(new OlnyEndMarkFilter(Parameter.EndMark, Parameter.CommandSpliter, Parameter.ParameterSpliter));
            }

            easyClient.NewPackageReceived += EasyClient_NewPackageReceived;
            easyClient.Error += EasyClient_Error;
            easyClient.Closed += EasyClient_Closed;

            return easyClient.ConnectAsync(new IPEndPoint(IPAddress.Parse(Parameter.ServerIPAddress), Parameter.Port)).Result;
        }

        /// <summary>
        /// 关闭网口
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (IsOpen())
            {
                return easyClient.Close().Result;
            }
            return false;
        }

        public void Write(byte[] data)
        {
            if (IsOpen())
            {
                byte[] begin = Encoding.Default.GetBytes(Parameter.BeginMark);
                byte[] end = Encoding.Default.GetBytes(Parameter.EndMark);
                var send = begin.Concat(data).Concat(end);
                easyClient.Send(send.ToArray());
            }
        }

        public void Write(string data)
        {
            if (IsOpen())
            {
                string sendStr = Parameter.BeginMark + data + Parameter.EndMark;
                easyClient.Send(Encoding.Default.GetBytes(sendStr));
            }
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="result"></param>
        /// <returns>返回字符串长度。-1：超时。0：失败</returns>
        public int ReadString(out string result)
        {
            result = string.Empty;
            if (IsOpen())
            {

                result = string.Empty;
                string package;
                if (!_blockingCollection.TryTake(out package, Parameter.TimeOut))
                {
                    return -1;
                }
                result = package;
                return result.Length;
            }

            return 0;
        }

        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="result"></param>
        /// <returns>返回字节数组长度。-1：超时。0：失败</returns>
        public int ReadBytes(out byte[] result)
        {
            result = null;
            if (IsOpen())
            {
                string package;
                if (!_blockingCollection.TryTake(out package, Parameter.TimeOut))
                {
                    return -1;
                }
                result = Encoding.Default.GetBytes(package);
                return result.Length;
            }
            return 0;
        }

        public void ClearBuffer()
        {
            string buffer;
            while (_blockingCollection.Count > 0)
            {
                _blockingCollection.TryTake(out buffer);
            }
        }

        private void EasyClient_Closed(object sender, EventArgs e)
        {
            ServersClosed?.Invoke(this, $"{Parameter.Name} Closed!!\r\nPlease check sever connect state or check protocol!");
        }

        private void EasyClient_Error(object sender, ErrorEventArgs e)
        {
            Error?.Invoke(this, e.Exception.Message);
        }

        private void EasyClient_NewPackageReceived(object sender, PackageEventArgs<StringPackageInfo> e)
        {
            _blockingCollection.Add(e.Package.Key + Parameter.CommandSpliter + e.Package.Body);
        }
    }
}
