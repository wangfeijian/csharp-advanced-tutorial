using SuperSocket.ClientEngine;
using SuperSocket.ProtoBase;
using System;
using System.Net;
using System.Text;

namespace UserSuperSocketClient
{
    public class EndMarkFilter : TerminatorReceiveFilter<StringPackageInfo>
    {
        byte[] _endMark;
        public EndMarkFilter(byte[] end) : base(end)
        {
            _endMark = end;
        }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            string body = bufferStream.ReadString((int)bufferStream.Length - _endMark.Length, Encoding.Default);
            return new StringPackageInfo("", body, new string[] { });
        }
    }
    public class ReceiveFilter : BeginEndMarkReceiveFilter<StringPackageInfo>
    {
        private static byte[] BeginMark = new byte[] { (byte)'!' };
        private static byte[] EndMark = new byte[] { (byte)'\r', (byte)'\n' };
        /// <summary>
        /// 过滤器
        /// </summary>
        /// <param name="beginMark">开始字符串 或 开始字节数组</param>
        /// <param name="endMark">结束字符串 或 结束字节数组</param>
        public ReceiveFilter() : base(BeginMark, EndMark)
        {
            //this.BeginMark = beginMark;
            //this.EndMark = endMark;
        }

        /// <summary>
        /// 经过过滤器，收到的字符串会到这个函数
        /// </summary>
        /// <param name="bufferStream"></param>
        /// <returns></returns>
        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            //string body = bufferStream.ReadString((int)bufferStream.Length, Encoding.Default);
            //body = body.Substring(BeginMark.Length, body.Length - EndMark.Length - BeginMark.Length);

            //Skip(int count):从数据源跳过指定的字节个数。直接获取过滤后的数据
            string key = bufferStream.ReadString(BeginMark.Length, Encoding.Default);
            string body = bufferStream.ReadString((int)bufferStream.Length - BeginMark.Length - EndMark.Length, Encoding.Default);
            return new StringPackageInfo(key, body, new string[] { });
        }
    }

    internal class Program
    {
        private static int _index = 0;
        private static EasyClient<StringPackageInfo> _client;
        static void Main(string[] args)
        {

            _client = new EasyClient<StringPackageInfo>();
            _client.Initialize(new ReceiveFilter());
            _client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000));
            _client.NewPackageReceived += _client_NewPackageReceived;
            _client.Closed += _client_Closed;
            _client.Connected += _client_Connected;
            _client.Error += _client_Error;
            //_client.DataReceived += _client_DataReceived;
            Console.ReadLine();
        }

        private static void _client_Error(object sender, ErrorEventArgs e)
        {

        }

        private static void _client_Connected(object sender, EventArgs e)
        {

        }

        private static void _client_Closed(object sender, EventArgs e)
        {

        }

        private static void _client_NewPackageReceived(object sender, PackageEventArgs<StringPackageInfo> e)
        {
            _index++;
            Console.WriteLine(_index);
            Console.WriteLine(e.Package.Key + e.Package.Body);
        }

        private static void _client_DataReceived(object sender, DataEventArgs e)
        {
            //string msg = Encoding.Default.GetString(e.Data);
            //_index++;
            //Console.WriteLine(_index);
            //Console.WriteLine(msg);
        }
    }
}
