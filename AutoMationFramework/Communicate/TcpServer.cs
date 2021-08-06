using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communicate
{
    /// <summary>
    /// Socket实现的异步TCP服务器
    /// </summary>
    public class AsyncSocketTCPServer : IDisposable
    {
        #region Fields
        /// <summary>
        /// 服务器程序同意的最大客户端连接数
        /// </summary>
        private int m_nMaxClient;

        /// <summary>
        /// 当前的连接的客户端数
        /// </summary>
        private int m_nClientCount;

        /// <summary>
        /// 服务器使用的异步socket
        /// </summary>
        private Socket m_serverSock;


        /// <summary>
        /// 客户端会话列表
        /// </summary>
        private List<AsyncSocketState> m_listClients;

        private bool m_bDisposed = false;

        #endregion

        #region Properties

        /// <summary>
        /// 服务器是否正在执行
        /// </summary>
        public bool IsRunning
        {
            get;
            private set;
         }
        /// <summary>
        /// 监听的IP地址
        /// </summary>
        public IPAddress Address { get; private set; }
        /// <summary>
        /// 监听的port
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// 通信使用的编码
        /// </summary>
        public Encoding Encoding { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="listenPort">监听的port</param>
        public AsyncSocketTCPServer(int listenPort)
            : this(IPAddress.Any, listenPort, 1024)
        {
        }

        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="lisenPort">监听的port</param>
        /// <param name="maxClient">最大监听数量</param>
        public AsyncSocketTCPServer(string ip,int lisenPort,int maxClient = 1024)
            : this(IPAddress.Parse(ip),lisenPort,maxClient)
        {

        }

        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="localEP">监听的终结点</param>
        public AsyncSocketTCPServer(IPEndPoint localEP)
            : this(localEP.Address, localEP.Port, 1024)
        {
        }

        /// <summary>
        /// 异步Socket TCP服务器
        /// </summary>
        /// <param name="localIPAddress">监听的IP地址</param>
        /// <param name="listenPort">监听的port</param>
        /// <param name="maxClient">最大客户端数量</param>
        public AsyncSocketTCPServer(IPAddress localIPAddress, int listenPort, int maxClient)
        {
            this.Address = localIPAddress;
            this.Port = listenPort;
            this.Encoding = Encoding.Default;

            m_nMaxClient = maxClient;
            m_listClients = new List<AsyncSocketState>();
            //m_serverSock = new Socket(localIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        #endregion

        #region Method

        /// <summary>
        /// 启动服务器
        /// </summary>
        public void Start()
        {
            try
            {
                if (!IsRunning)
                {
                    IsRunning = true;
                    m_serverSock = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    m_serverSock.Bind(new IPEndPoint(this.Address, this.Port));
                    m_serverSock.Listen(1024);
                    m_serverSock.BeginAccept(new AsyncCallback(HandleAcceptConnected), m_serverSock);

                    RaiseStateChanged();
                }
            }
            catch (Exception ex)
            {
                RaiseOtherException(null, ex.Message);
            }
            
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="backlog">
        /// 服务器所同意的挂起连接序列的最大长度
        /// </param>
        public void Start(int backlog)
        {
            try
            {
                if (!IsRunning)
                {
                    IsRunning = true;
                    m_serverSock = new Socket(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    m_serverSock.Bind(new IPEndPoint(this.Address, this.Port));
                    m_serverSock.Listen(backlog);
                    m_serverSock.BeginAccept(new AsyncCallback(HandleAcceptConnected), m_serverSock);

                    RaiseStateChanged();
                }
            }
            catch (Exception ex)
            {
                RaiseOtherException(null, ex.Message);
            }
            
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                m_serverSock.Close();

                RaiseStateChanged();
                //TODO 关闭对全部客户端的连接
                CloseAllClient();
            }
        }

        /// <summary>
        /// 处理客户端连接
        /// </summary>
        /// <param name="ar"></param>
        private void HandleAcceptConnected(IAsyncResult ar)
        {
            if (IsRunning)
            {
                Socket server = (Socket)ar.AsyncState;
                Socket client = server.EndAccept(ar);

                //检查是否达到最大的同意的客户端数目
                if (m_nClientCount >= m_nMaxClient)
                {
                    //C-TODO 触发事件
                    RaiseOtherException(null);
                }
                else
                {
                    AsyncSocketState state = new AsyncSocketState(client);
                    lock (m_listClients)
                    {
                        m_listClients.Add(state);
                        m_nClientCount++;
                        RaiseClientConnected(state); //触发客户端连接事件
                    }
                    state.RecvDataBuffer = new byte[client.ReceiveBufferSize];
                    //開始接受来自该客户端的数据
                    client.BeginReceive(state.RecvDataBuffer, 0, state.RecvDataBuffer.Length, SocketFlags.None,
                     new AsyncCallback(HandleDataReceived), state);
                }
                //接受下一个请求
                server.BeginAccept(new AsyncCallback(HandleAcceptConnected), ar.AsyncState);
            }
        }
        /// <summary>
        /// 处理客户端数据
        /// </summary>
        /// <param name="ar"></param>
        private void HandleDataReceived(IAsyncResult ar)
        {
            if (IsRunning)
            {
                AsyncSocketState state = (AsyncSocketState)ar.AsyncState;
                Socket client = state.ClientSocket;
                try
                {
                    //假设两次開始了异步的接收,所以当客户端退出的时候
                    //会两次执行EndReceive
                    int recv = client.EndReceive(ar);
                    if (recv == 0)
                    {
                        //C- TODO 触发事件 (关闭客户端)
                        Close(state);
                        RaiseNetError(state);
                        return;
                    }
                    //TODO 处理已经读取的数据 ps:数据在state的RecvDataBuffer中
                    state.Length = recv;
                    //C- TODO 触发数据接收事件
                    RaiseDataReceived(state);
                }
                catch (Exception)
                {
                    //C- TODO 异常处理
                    Close(state);
                    RaiseNetError(state);
                }
                finally
                {
                    //继续接收来自来客户端的数据
                    if (client.Connected)
                    {
                        client.BeginReceive(state.RecvDataBuffer, 0, state.RecvDataBuffer.Length, SocketFlags.None,
                     new AsyncCallback(HandleDataReceived), state);
                    }
                    
                }
            }
        }

        /// <summary>
        /// 广播数据
        /// </summary>
        /// <param name="data"></param>
        public void Broadcast(byte[] data)
        {
            foreach (var client in m_listClients)
            {
                Send(client, data);
            }
        }

        /// <summary>
        /// 广播数据
        /// </summary>
        /// <param name="data"></param>
        public void Broadcast(string data)
        {
            foreach (var client in m_listClients)
            {
                Send(client, data);
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="state">接收数据的客户端会话</param>
        /// <param name="data">数据报文</param>
        public void Send(AsyncSocketState state, byte[] data)
        {
            RaisePrepareSend(state);
            Send(state.ClientSocket, data);
        }

        /// <summary>
        /// 发送字符串数据
        /// </summary>
        /// <param name="state"></param>
        /// <param name="strData"></param>
        public void Send(AsyncSocketState state,string strData)
        {
            RaisePrepareSend(state);

            byte[] data = this.Encoding.GetBytes(strData);

            Send(state.ClientSocket, data);
        }

        /// <summary>
        /// 异步发送数据至指定的客户端
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="data">报文</param>
        public void Send(Socket client, byte[] data)
        {
            if (!IsRunning)
                throw new InvalidProgramException("This TCP Scoket server has not been started.");

            if (client == null)
                throw new ArgumentNullException("client");

            if (data == null)
                throw new ArgumentNullException("data");

            if (client.Connected)
            {
                client.BeginSend(data, 0, data.Length, SocketFlags.None,
             new AsyncCallback(SendDataEnd), client);
            }
        }

        /// <summary>
        /// 发送数据完成处理函数
        /// </summary>
        /// <param name="ar">目标客户端Socket</param>
        private void SendDataEnd(IAsyncResult ar)
        {
            ((Socket)ar.AsyncState).EndSend(ar);
            RaiseCompletedSend(null);
        }

        /// <summary>
        /// 获取EndPoint字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}:{1}",Address.ToString(),Port);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 与客户端的连接已建立事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> ClientConnected;
        /// <summary>
        /// 与客户端的连接已断开事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> ClientDisconnected;

        /// <summary>
        /// 触发客户端连接事件
        /// </summary>
        /// <param name="state"></param>
        private void RaiseClientConnected(AsyncSocketState state)
        {
            if (ClientConnected != null)
            {
                ClientConnected(this, new AsyncSocketEventArgs(state));
            }
        }
        /// <summary>
        /// 触发客户端连接断开事件
        /// </summary>
        /// <param name="state"></param>
        private void RaiseClientDisconnected(AsyncSocketState state)
        {
            if (ClientDisconnected != null)
            {
                ClientDisconnected(this, new AsyncSocketEventArgs(state));
            }
        }

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> DataReceived;

        private void RaiseDataReceived(AsyncSocketState state)
        {
            if (DataReceived != null)
            {
                DataReceived(this, new AsyncSocketEventArgs(state));
            }
        }

        /// <summary>
        /// 发送数据前的事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> PrepareSend;

        /// <summary>
        /// 触发发送数据前的事件
        /// </summary>
        /// <param name="state"></param>
        private void RaisePrepareSend(AsyncSocketState state)
        {
            if (PrepareSend != null)
            {
                PrepareSend(this, new AsyncSocketEventArgs(state));
            }
        }

        /// <summary>
        /// 数据发送完成事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> CompletedSend;

        /// <summary>
        /// 触发数据发送完成的事件
        /// </summary>
        /// <param name="state"></param>
        private void RaiseCompletedSend(AsyncSocketState state)
        {
            if (CompletedSend != null)
            {
                CompletedSend(this, new AsyncSocketEventArgs(state));
            }
        }

        /// <summary>
        /// 网络错误事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> NetError;
        /// <summary>
        /// 触发网络错误事件
        /// </summary>
        /// <param name="state"></param>
        private void RaiseNetError(AsyncSocketState state)
        {
            if (NetError != null)
            {
                NetError(this, new AsyncSocketEventArgs(state));
            }
        }

        /// <summary>
        /// 服务连接状态事件
        /// </summary>
        public event EventHandler<EventArgs> ServerStateChangedEvent;
        private void RaiseStateChanged()
        {
            if (ServerStateChangedEvent != null)
            {
                ServerStateChangedEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// 异常事件
        /// </summary>
        public event EventHandler<AsyncSocketEventArgs> OtherException;
        /// <summary>
        /// 触发异常事件
        /// </summary>
        /// <param name="state"></param>
        /// <param name="descrip"></param>
        private void RaiseOtherException(AsyncSocketState state, string descrip)
        {
            if (OtherException != null)
            {
                OtherException(this, new AsyncSocketEventArgs(descrip, state));
            }
        }
        private void RaiseOtherException(AsyncSocketState state)
        {
            RaiseOtherException(state, "");
        }
        #endregion

        #region Close
        /// <summary>
        /// 关闭一个与客户端之间的会话
        /// </summary>
        /// <param name="state">须要关闭的客户端会话对象</param>
        public void Close(AsyncSocketState state)
        {
            if (state != null)
            {
                state.Datagram = null;
                state.RecvDataBuffer = null;

                lock(m_listClients)
                {
                    m_listClients.Remove(state);
                    m_nClientCount--;
                }
                //TODO 触发关闭事件

                RaiseClientDisconnected(state);

                state.Close();
            }
        }
        /// <summary>
        /// 关闭全部的客户端会话,与全部的客户端连接会断开
        /// </summary>
        public void CloseAllClient()
        {
            lock(m_listClients)
            {
                foreach (AsyncSocketState client in m_listClients)
                {
                    if (client != null)
                    {
                        client.Datagram = null;
                        client.RecvDataBuffer = null;

                        //TODO 触发关闭事件

                        RaiseClientDisconnected(client);

                        client.Close();
                    }
                }
                m_nClientCount = 0;
                m_listClients.Clear();
            }
        }
        #endregion

        #region 释放
        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release 
        /// both managed and unmanaged resources; <c>false</c> 
        /// to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.m_bDisposed)
            {
                if (disposing)
                {
                    try
                    {
                        Stop();
                        if (m_serverSock != null)
                        {
                            m_serverSock = null;
                        }
                    }
                    catch (SocketException)
                    {
                        //TODO
                        RaiseOtherException(null);
                    }
                }
                m_bDisposed = true;
            }
        }
        #endregion
    }


    /// <summary>
    /// 异步Socket TCP事件參数类
    /// </summary>
    public class AsyncSocketEventArgs : EventArgs
    {
        /// <summary>
        /// 提示信息
        /// </summary>
        public string m_strMsg;

        /// <summary>
        /// client状态封装类
        /// </summary>
        public AsyncSocketState m_state;

        /// <summary>
        /// 是否已经处理过了
        /// </summary>
        public bool IsHandled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public AsyncSocketEventArgs(string msg)
        {
            this.m_strMsg = msg;
            IsHandled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public AsyncSocketEventArgs(AsyncSocketState state)
        {
            this.m_state = state;
            IsHandled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="state"></param>
        public AsyncSocketEventArgs(string msg, AsyncSocketState state)
        {
            this.m_strMsg = msg;
            this.m_state = state;
            IsHandled = false;
        }
    }

    /// <summary>
    /// 异步SOCKET TCP 中用来存储客户端状态信息的类
    /// </summary>
    public class AsyncSocketState
    {
        #region 字段
        /// <summary>
        /// 接收数据缓冲区
        /// </summary>
        private byte[] m_byRecvBuffer;

        /// <summary>
        /// 接收数据的长度
        /// </summary>
        private int m_nLength;

        /// <summary>
        /// 客户端发送到server的报文
        /// 注意:在有些情况下报文可能仅仅是报文的片断而不完整
        /// </summary>
        private string m_strDatagram;

        /// <summary>
        /// 客户端的Socket
        /// </summary>
        private Socket m_clientSock;

        #endregion

        #region 属性

        /// <summary>
        /// 接收数据缓冲区 
        /// </summary>
        public byte[] RecvDataBuffer
        {
            get
            {
                return m_byRecvBuffer;
            }
            set
            {
                m_byRecvBuffer = value;
            }
        }

        /// <summary>
        /// 接收数据的长度
        /// </summary>
        public int Length
        {
            get
            {
                return m_nLength;
            }

            set
            {
                m_nLength = value;
            }
        }

        /// <summary>
        /// 存取会话的报文
        /// </summary>
        public string Datagram
        {
            get
            {
                return m_strDatagram;
            }
            set
            {
                m_strDatagram = value;
            }
        }

        /// <summary>
        /// 获得与客户端会话关联的Socket对象
        /// </summary>
        public Socket ClientSocket
        {
            get
            {
                return m_clientSock;

            }
        }


        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cliSock">会话使用的Socket连接</param>
        public AsyncSocketState(Socket cliSock)
        {
            m_clientSock = cliSock;
        }

        /// <summary>
        /// 初始化数据缓冲区
        /// </summary>
        public void InitBuffer()
        {
            if (m_byRecvBuffer == null && m_clientSock != null)
            {
                m_byRecvBuffer = new byte[m_clientSock.ReceiveBufferSize];
            }
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        public void Close()
        {

            //关闭数据的接受和发送
            m_clientSock.Shutdown(SocketShutdown.Both);

            //清理资源
            m_clientSock.Close();
        }

        /// <summary>
        /// 获取EndPoint字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (m_clientSock != null && m_clientSock.Connected)
            {
                return m_clientSock.RemoteEndPoint.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
