namespace Soso.Contract
{
    public class SocketClientParameter
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string ServerIPAddress { get; set; }
        public int Port { get; set; }
        public int TimeOut { get; set; }
        public string BeginMark { get; set; }
        public string EndMark { get; set; }
        public string CommandSpliter { get; set; }
        public string ParameterSpliter { get; set; }
    }
}
