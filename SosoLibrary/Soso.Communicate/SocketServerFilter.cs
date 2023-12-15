using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System.Net;
using System.Text;

namespace Soso.Communicate
{
    /// <summary>
    /// 开始与结束符过滤工厂
    /// </summary>
    public class BeginEndFilterFactory : IReceiveFilterFactory<StringRequestInfo>, IReceiveFilterFactory
    {
        private byte[] _beginMark;
        private byte[] _endMark;
        private readonly IRequestInfoParser<StringRequestInfo> _requestParser;
        public BeginEndFilterFactory(string beginMark, string endMark, IRequestInfoParser<StringRequestInfo> requestParser)
        {
            _beginMark = Encoding.Default.GetBytes(beginMark);
            _endMark = Encoding.Default.GetBytes(endMark);
            _requestParser = requestParser;
        }

        public IReceiveFilter<StringRequestInfo> CreateFilter(IAppServer appServer, IAppSession appSession, IPEndPoint remoteEndPoint)
        {
            return new BeginEndFilter(_beginMark, _endMark, _requestParser);
        }
    }

    /// <summary>
    /// 开始与结束符过滤类
    /// </summary>
    public class BeginEndFilter : BeginEndMarkReceiveFilter<StringRequestInfo>
    {
        private byte[] _beginMark;
        private byte[] _endMark;
        private readonly IRequestInfoParser<StringRequestInfo> _requestParser;

        public BeginEndFilter(byte[] beginMark, byte[] endMark, IRequestInfoParser<StringRequestInfo> requestParser) : base(beginMark, endMark)
        {
            _beginMark = beginMark;
            _endMark = endMark;
            _requestParser = requestParser;
        }
        protected override StringRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            string recevie = Encoding.Default.GetString(readBuffer, _beginMark.Length, length - _beginMark.Length - _endMark.Length);

            return _requestParser.ParseRequestInfo(recevie);
        }
    }
}
