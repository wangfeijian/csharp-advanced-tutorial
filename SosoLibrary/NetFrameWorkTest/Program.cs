using Autofac;
using Soso.Communicate;
using Soso.Contract;
using Soso.Log;
using Soso.Services;
using System;
using System.Linq;

namespace NetFrameWorkTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DIServices.Instance.AddPrivateCtorInstance<ILogServices, LogServices>();
            SocketClientTest();
            Console.ReadLine();
        }

        static void SocketClientTest()
        {
            ConfigServices.Instance.InitSocketParameters();

            foreach (var client in ConfigServices.Instance.SocketClientParameters)
            {
                DIServices.Instance.ContainerBuilder.Register(o => new SocketClient(client)).Named<SocketClient>(client.Index);
            }
            DIServices.Instance.ServicesBuilder();

            var client1 = DIServices.Instance.Container.ResolveNamed<SocketClient>("1");
            var client2 = DIServices.Instance.Container.ResolveNamed<SocketClient>("2");

            InitClient(client1, "client1");
            InitClient(client2, "client2");
            client1.Close();
            client2.Close();
        }

        static void InitClient(SocketClient client, string name)
        {
            client.Open();
            client.ServersClosed += Client_ServersClosed;

            client.Error += Client_Error;
            client.Write($"Hello {name}");

            string result;
            if (client.ReadString(out result) == -1)
            {
                Console.WriteLine($"{name} timeout!");
            }
            else
            {
                Console.WriteLine(result);
            }
        }

        private static void Client_ServersClosed(object sender, string msg)
        {
            Console.WriteLine(msg);
        }

        private static void Client_Error(object sender, string msg)
        {
            Console.WriteLine(msg);
        }

        static void SocketServerTest()
        {
            ConfigServices.Instance.InitSocketParameters();

            foreach (var service in ConfigServices.Instance.SocketServerParameters)
            {
                DIServices.Instance.ContainerBuilder.Register(o => new SocketServer(service)).Named<SocketServer>(service.Index);
            }
            DIServices.Instance.ServicesBuilder();

            var server = DIServices.Instance.Container.ResolveNamed<SocketServer>("1");
            var server2 = DIServices.Instance.Container.ResolveNamed<SocketServer>("2");
            InitServer(server);
            InitServer(server2);

            Console.ReadLine();
            server.Broadcast("Send to all\r\n");
            server.Stop();
            server2.Stop();
            Console.ReadLine();
        }
        static void InitServer(SocketServer server)
        {
            server.ClientConnected += (session) => { Console.WriteLine($"{session.RemoteEndPoint.Address}:{session.RemoteEndPoint.Port} connected!"); };
            server.ClientDisconnected += (session, c) => { Console.WriteLine($"{session.RemoteEndPoint.Address}:{session.RemoteEndPoint.Port} disconnected! Reason:{c}"); };
            server.DataReceived += Server_DataReceived;
            server.Start();
        }

        private static void Server_DataReceived(SuperSocket.SocketBase.AppSession session, SuperSocket.SocketBase.Protocol.StringRequestInfo requestInfo)
        {
            Console.WriteLine($"Recevie data from {session.RemoteEndPoint.Address}:{session.RemoteEndPoint.Port}: Commmand: {requestInfo.Key} Parameters: {requestInfo.Body}");
            string send;
            switch (requestInfo.Key.ToUpper())
            {
                case ("ECHO"):
                    Console.WriteLine($"Send data:{requestInfo.Body} to Client {session.RemoteEndPoint.Address}:{session.RemoteEndPoint.Port}");
                    session.Send(requestInfo.Body);
                    break;

                case ("ADD"):
                    send = requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString();
                    Console.WriteLine($"Send data:{send} to Client {session.RemoteEndPoint.Address}:{session.RemoteEndPoint.Port}");
                    session.Send(send);
                    break;

                case ("MULT"):

                    var result = 1;

                    foreach (var factor in requestInfo.Parameters.Select(p => Convert.ToInt32(p)))
                    {
                        result *= factor;
                    }
                    send = result.ToString();
                    Console.WriteLine($"Send data:{send} to Client {session.RemoteEndPoint.Address}:{session.RemoteEndPoint.Port}");
                    session.Send(send);
                    break;
            }

        }
    }
}
