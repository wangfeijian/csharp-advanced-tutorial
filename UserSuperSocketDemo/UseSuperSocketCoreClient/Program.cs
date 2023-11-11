// See https://aka.ms/new-console-template for more information
using SuperSocket.Client;
using SuperSocket.ProtoBase;
using System.Diagnostics;
using System.Net;
using System.Text;
using UseSuperSocketCoreClient;

var cancelSource = new CancellationTokenSource();
var cancellation = cancelSource.Token;
int index = 0;
EndMarkFilter.SetEndMark("$");
IEasyClient<TextPackageInfo> _client = new EasyClient<TextPackageInfo>(new EndMarkFilter());
await _client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000));
try
{

    await _client.SendAsync(new ReadOnlyMemory<byte>(Encoding.Default.GetBytes("Hello")));
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return;
}
string info = string.Empty;
Task.Run(() =>
{
    info = _client.ReceiveAsync().Result.Text;

}, cancellation);

Stopwatch stopwatch = Stopwatch.StartNew();
while (true)
{
    await Task.Delay(1);
    if (stopwatch.ElapsedMilliseconds > 9000)
    {
        cancelSource.Cancel();
        Console.WriteLine("未接收到数据！");
        stopwatch.Stop();
        break;
    }

    if (!string.IsNullOrEmpty(info))
    {
        stopwatch.Stop();
        Console.WriteLine($"正常接收数据{info}");
        break;
    }
}
//#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//Task.Run(() =>
//{
//    while (true)
//    {
//        var text = _client.ReceiveAsync().Result;
//        if (text == null)
//        {
//            break;
//        }
//        Console.WriteLine(text);
//        Console.WriteLine(index++);
//    }
//}, cancellation);
//#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed


