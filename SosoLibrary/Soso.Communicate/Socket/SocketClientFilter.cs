#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Communicate.Socket
 * 唯一标识：8b354cde-8bb8-4329-926c-30bba71da774
 * 文件名：SocketClientFilter
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：12/26/2023 9:33:48 AM
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

using SuperSocket.ProtoBase;
using System;
using System.Text;

namespace Soso.Communicate.Socket
{
    public static class StringPackageInfoParser
    {
        public static StringPackageInfo Parser(string source, string commandSpliter, string parameterSpliters)
        {
            int num = source.IndexOf(commandSpliter);
            string empty = string.Empty;
            string text = string.Empty;
            if (num > 0)
            {
                empty = source.Substring(0, num);
                text = source.Substring(num + 1);
            }
            else
            {
                empty = source;
            }

            return new StringPackageInfo(empty, text, text.Split(new string[] { parameterSpliters }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
    /// <summary>
    /// 开始与结束符过滤类
    /// </summary>
    public class BeginEndMarkFilter : BeginEndMarkReceiveFilter<StringPackageInfo>
    {
        private byte[] _beginMark;
        private byte[] _endMark;
        string _commandSplit, _paramSplit;
        public BeginEndMarkFilter(string beginMark, string endMark, string commandSplit, string paramSplit) : base(Encoding.Default.GetBytes(beginMark), Encoding.Default.GetBytes(endMark))
        {
            _beginMark = Encoding.Default.GetBytes(beginMark);
            _endMark = Encoding.Default.GetBytes(endMark);
            _commandSplit = commandSplit;
            _paramSplit = paramSplit;
        }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            string recevie = bufferStream.Skip(_beginMark.Length).ReadString((int)bufferStream.Length - _beginMark.Length - _endMark.Length, Encoding.Default);

            return StringPackageInfoParser.Parser(recevie, _commandSplit, _paramSplit);
        }
    }

    public class OlnyEndMarkFilter : TerminatorReceiveFilter<StringPackageInfo>
    {
        byte[] _endMark;
        string _commandSplit, _paramSplit;
        public OlnyEndMarkFilter(string endMark, string commandSplit, string paramSplit) : base(Encoding.Default.GetBytes(endMark))
        {
            _endMark = Encoding.Default.GetBytes(endMark);
            _commandSplit = commandSplit;
            _paramSplit = paramSplit;
        }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            string recevie = bufferStream.ReadString((int)bufferStream.Length - _endMark.Length, Encoding.Default);
            return StringPackageInfoParser.Parser(recevie, _commandSplit, _paramSplit);
        }
    }
}
