#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Communicate.Socket
 * 唯一标识：75012e5e-41c0-4c0d-8f8a-a8d74c087445
 * 文件名：SocketServer
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 9:34:32 AM
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
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System.Text;

namespace Soso.Communicate.Socket
{
    /// <summary>
    /// Socket服务器类
    /// </summary>
    public class SocketServer
    {
        private AppServer _appServer;
        private bool _enable;
        private string _name;
        public string Name => _name;

        public event SessionHandler<AppSession, CloseReason> ClientDisconnected;
        public event SessionHandler<AppSession> ClientConnected;
        public event RequestHandler<AppSession, StringRequestInfo> DataReceived;
        public SocketServer(SocketServerParameter parameter)
        {
            _enable = parameter.Enable;
            _name = parameter.Name;

            parameter.EndMark = EndMarkParse.Parse(parameter.EndMark);

            if (!string.IsNullOrEmpty(parameter.BeginMark))
            {
                _appServer = new AppServer(new BeginEndFilterFactory(parameter.BeginMark, parameter.EndMark, new BasicRequestInfoParser(parameter.CommandSpliter, parameter.ParameterSpliter)));
            }
            else
            {
                _appServer = new AppServer(new TerminatorReceiveFilterFactory(parameter.EndMark, Encoding.Default, new BasicRequestInfoParser(parameter.CommandSpliter, parameter.ParameterSpliter)));
            }

            _appServer.Setup(parameter.IPAddress, parameter.Port);
            _appServer.NewRequestReceived += NewRequestReceived;
            _appServer.NewSessionConnected += NewSessionConnected;
            _appServer.SessionClosed += SessionClosed;
        }

        public bool Start()
        {
            if (!_enable)
                return false;

            return _appServer.Start();
        }

        public void Stop()
        {
            _appServer.Stop();
        }

        public void Send(AppSession session, string msg)
        {
            session.Send(msg);
        }

        public void Send(AppSession session, byte[] data)
        {
            session.Send(data, 0, data.Length);
        }

        public void Broadcast(string msg)
        {
            foreach (var item in _appServer.GetAllSessions())
            {
                item.Send(msg);
            }
        }

        public void Broadcast(byte[] data)
        {
            foreach (var item in _appServer.GetAllSessions())
            {
                item.Send(data, 0, data.Length);
            }
        }

        private void SessionClosed(AppSession session, CloseReason value)
        {
            ClientDisconnected?.Invoke(session, value);
        }

        private void NewSessionConnected(AppSession session)
        {
            ClientConnected?.Invoke(session);
        }


        private void NewRequestReceived(AppSession session, StringRequestInfo requestInfo)
        {
            DataReceived?.Invoke(session, requestInfo);
        }
    }
}

