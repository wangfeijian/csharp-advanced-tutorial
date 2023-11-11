#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：UseSuperSocketCoreServer
 * 唯一标识：66c4f057-b3a7-4c65-9df5-cddc76047caa
 * 文件名：SocketServerServices
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：11/11/2023 9:32:07 AM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using Microsoft.Extensions.Hosting;
using SuperSocket;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using System.Collections.Concurrent;
using System.Text;

namespace UseSuperSocketCoreServer
{
    public class SocketServerServices
    {
        private static readonly object _lock = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static SocketServerServices _instance;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private IHost _host;
        /// <summary>
        /// 单例实体
        /// </summary>
        /// <returns></returns>
        public static SocketServerServices Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SocketServerServices();
                        }
                    }
                }
                return _instance;
            }
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private SocketServerServices()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        /// <summary>
        /// 会话集合
        /// </summary>
        private readonly ConcurrentDictionary<string, IAppSession> SessionsDict = new ConcurrentDictionary<string, IAppSession>();
        /// <summary>
        /// 启动Socket服务(只须调用此方法即可启动)
        /// </summary>
        /// <returns></returns>
        public void RunServer(string endMark)
        {
            EndMarkFilter.SetEndMark(endMark);
            _host = SuperSocketHostBuilder.Create<TextPackageInfo, EndMarkFilter>()
                .UsePackageHandler(OnPackageAsync)
                .ConfigureSuperSocket(options =>
                {
                    options.Name = "PxIotServer";
                    options.Listeners = new[] {
                    new ListenOptions
                    {
                        Ip = "Any",
                        Port = 9000
                    } }.ToList();
                })
                .UseSessionHandler(OnConnectedAsync, OnClosedAsync)
                .Build();

            _host.RunAsync();
        }




        private async ValueTask OnPackageAsync(IAppSession session, TextPackageInfo info)
        {
            await Task.Run(() =>
             {
                 //发送收到的数据
                 session.SendAsync(new ReadOnlyMemory<byte>(Encoding.Default.GetBytes(info.Text)));
             });
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public bool StopServer()
        {
            bool isSuccess = false;
            foreach (var v in SessionsDict)
            {
                v.Value.CloseAsync(CloseReason.ServerShutdown);
            }
            SessionsDict.Clear();
            return isSuccess;
        }
        /// <summary>
        /// 会话的连接事件
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private async ValueTask OnConnectedAsync(IAppSession session)
        {
            await Task.Run(() =>
             {
                 while (!SessionsDict.ContainsKey(session.SessionID))
                 {
                     //添加不成功则重复添加
                     if (!SessionsDict.TryAdd(session.SessionID, session))
                         Thread.Sleep(1);
                 }
             });
        }
        /// <summary>
        /// 会话的断开事件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async ValueTask OnClosedAsync(IAppSession session, CloseEventArgs e)
        {
            await Task.Run(() =>
             {
                 while (SessionsDict.ContainsKey(session.SessionID))
                 {
                     //移除不成功则重复移除
                     if (!SessionsDict.TryRemove(session.SessionID, out _))
                         Thread.Sleep(1);
                 }
             });
        }

    }
}