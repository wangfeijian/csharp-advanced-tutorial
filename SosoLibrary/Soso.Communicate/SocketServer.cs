using Soso.Contract;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System.Text;

namespace Soso.Communicate
{
    /// <summary>
    /// Socket服务器类
    /// </summary>
    public class SocketServer
    {
        private AppServer _appServer;
        private bool _enable;

        public event SessionHandler<AppSession, CloseReason> ClientDisconnected;
        public event SessionHandler<AppSession> ClientConnected;
        public event RequestHandler<AppSession, StringRequestInfo> DataReceived;
        public SocketServer(SocketServerParameter parameter)
        {
            _enable = parameter.Enable;
            if (parameter.EndMark.Contains("\\"))
            {
                parameter.EndMark = "\r\n";
            }

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
