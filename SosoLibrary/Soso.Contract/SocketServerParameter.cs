namespace Soso.Contract
{
    public class SocketServerParameter
    {
        public string Index { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public bool Enable { get; set; }
        public string BeginMark { get; set; }
        public string EndMark { get; set; }
        public string CommandSpliter { get; set; }
        public string ParameterSpliter { get; set; }
    }
}
