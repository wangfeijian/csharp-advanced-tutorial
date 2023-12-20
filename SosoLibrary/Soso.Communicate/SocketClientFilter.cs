using SuperSocket.ProtoBase;
using System;
using System.Text;

namespace Soso.Communicate
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