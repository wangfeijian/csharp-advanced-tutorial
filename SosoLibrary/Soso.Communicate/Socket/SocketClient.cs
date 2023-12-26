#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Communicate.Socket
 * 唯一标识：d14ac475-0686-4fc0-934f-eec172f02428
 * 文件名：SocketClient
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 9:32:15 AM
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
using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;

namespace Soso.Communicate.Socket
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
            Parameter.EndMark = EndMarkParse.Parse(parameter.EndMark);
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
