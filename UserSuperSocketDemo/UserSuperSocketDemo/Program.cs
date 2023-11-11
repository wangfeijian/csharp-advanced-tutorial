using SuperSocket.SocketBase;
using System;
using System.Linq;

namespace UserSuperSocketDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var appServer = new AppServer();
            appServer.Setup("127.0.0.1", 9000);
            appServer.NewRequestReceived += AppServer_NewRequestReceived;

            appServer.Start();

            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }

            appServer.Stop();

            Console.WriteLine("The server was stopped!");
            Console.ReadKey();
        }

        private static void AppServer_NewRequestReceived(AppSession session, SuperSocket.SocketBase.Protocol.StringRequestInfo requestInfo)
        {
            switch (requestInfo.Key.ToUpper())
            {
                case ("ECHO"):
                    session.Send(requestInfo.Body);
                    break;

                case ("ADD"):
                    session.Send(requestInfo.Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());
                    break;

                case ("MULT"):

                    var result = 1;

                    foreach (var factor in requestInfo.Parameters.Select(p => Convert.ToInt32(p)))
                    {
                        result *= factor;
                    }

                    session.Send(result.ToString());
                    break;
            }
        }
    }
}
