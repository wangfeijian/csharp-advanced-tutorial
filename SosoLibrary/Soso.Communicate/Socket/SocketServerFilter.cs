#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Communicate.Socket
 * 唯一标识：5b41a968-6e1b-48ae-b4ab-a7e04ae7a12d
 * 文件名：SocketServerFilter
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 9:35:21 AM
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

using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System.Net;
using System.Text;

namespace Soso.Communicate.Socket
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
