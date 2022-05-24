
using System.Reflection;
using System.Runtime.InteropServices;
using TestSnippets;

//使用XmlDocument操作
//XmlReadWrite.CreateXmlFile("d:/test.xml", "Test");
//XmlReadWrite.AddXmlNodeAndSetAttribute("d:/test.xml", "Test", "TestParam", "Param");
//XmlReadWrite.GetXmlNodeAttributeValue("d:/test.xml", "/Test/TestParam", "name");
//XmlReadWrite.GetXmlNodeAttributeValue("d:/test.xml", "/Test/TestParam", "version");

//使用XDocument操作
//XmlReadWriteUseXDocument.CreateXmlFile("d:/testX.xml", "TestXml");
//XmlReadWriteUseXDocument.AddXmlNodeAndSetAttribute("d:/testX.xml", "TestXml", "TestParm", "Param");
//XmlReadWriteUseXDocument.GetXmlNodeAttributeValue("d:/testX.xml", "Param", "name");
//XmlReadWriteUseXDocument.GetXmlNodeAttributeValue("d:/testX.xml", "Param", "version");

#region 监视文件
//using var watcher = new FileSystemWatcher(@"D:\TestFileChange");

//watcher.NotifyFilter = NotifyFilters.Attributes
//                     | NotifyFilters.CreationTime
//                     | NotifyFilters.DirectoryName
//                     | NotifyFilters.FileName
//                     | NotifyFilters.LastAccess
//                     | NotifyFilters.LastWrite
//                     | NotifyFilters.Security
//                     | NotifyFilters.Size;

//watcher.Changed += OnChanged;
//watcher.Created += OnCreated;
//watcher.Deleted += OnDeleted;
//watcher.Renamed += OnRenamed;
//watcher.Error += OnError;

//watcher.Filter = "*.txt";
//watcher.IncludeSubdirectories = true;
//watcher.EnableRaisingEvents = true;

//Console.WriteLine("Press enter to exit.");
//Console.ReadLine();

//static void OnChanged(object sender, FileSystemEventArgs e)
//{
//    if (e.ChangeType != WatcherChangeTypes.Changed)
//    {
//        return;
//    }
//    Console.WriteLine($"Changed: {e.FullPath}");
//}

//static void OnCreated(object sender, FileSystemEventArgs e)
//{
//    string value = $"Created: {e.FullPath}";
//    Console.WriteLine(value);
//}

//static void OnDeleted(object sender, FileSystemEventArgs e) =>
//    Console.WriteLine($"Deleted: {e.FullPath}");

//static void OnRenamed(object sender, RenamedEventArgs e)
//{
//    Console.WriteLine($"Renamed:");
//    Console.WriteLine($"    Old: {e.OldFullPath}");
//    Console.WriteLine($"    New: {e.FullPath}");
//}

//static void OnError(object sender, ErrorEventArgs e) =>
//    PrintException(e.GetException());

//static void PrintException(Exception? ex)
//{
//    if (ex != null)
//    {
//        Console.WriteLine($"Message: {ex.Message}");
//        Console.WriteLine("Stacktrace:");
//        Console.WriteLine(ex.StackTrace);
//        Console.WriteLine();
//        PrintException(ex.InnerException);
//    }
//}
#endregion

#region 遍历枚举

//foreach (var item in Enum.GetValues(typeof(EnumTest)))
//{
//    Console.WriteLine($"{(int)item}--{item}");
//}
//enum EnumTest
//{
//    First,
//    Second,
//    Third,
//    Fourth
//}
#endregion


#region 反射

//TestClass test = new TestClass();
//Type t = test.GetType();
//var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
//foreach (var method in methods)
//{
//    Console.WriteLine(method.Name);
//    method.Invoke(test, null);
//}
//class TestClass
//{
//    public void Test1()
//    {
//        Console.WriteLine("Test1 Invoke");
//    }

//    public void Test2()
//    {
//        Console.WriteLine("Test2 Invoke");
//    }
//}
#endregion

#region 精确计时器
AccurTimer accurTimer = new AccurTimer();
accurTimer.SetCount(2000);

while (!accurTimer.IsArrived())
{
    if (accurTimer.IsArrived())
    {
        Console.WriteLine("计时时间到");
    }
    else
    {
        Console.WriteLine($"当前的计时时间是：{accurTimer.GetTime(false)}ms");
    }
}
/// <summary>
/// 精确计时器类实现
/// </summary>
public class AccurTimer
{
    private long _counterFrequency;
    private long _counterResetTime;
    private long _counterSetup;

    public AccurTimer()
    {
        _counterSetup = 0L;
        QueryPerformanceFrequency(ref _counterFrequency);
        Reset();
    }

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(ref long count);

    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(ref long count);

    /// <summary>
    /// 重置计时器开始时间和清除计时标志
    /// </summary>
    public void Reset()
    {
        QueryPerformanceCounter(ref _counterResetTime);
        _counterSetup = 0L;
    }

    /// <summary>
    /// 查询计时器是否到达祋定时间
    /// </summary>
    /// <returns></returns>
    public bool IsArrived()
    {
        if (_counterSetup <= 0L)
            return true;
        long count = 0;
        QueryPerformanceCounter(ref count);
        return (double)(count - _counterResetTime) / (double)_counterFrequency * 1000.0 >= (double)_counterSetup;
    }

    /// <summary>
    /// 设置计时器计时的毫秒数
    /// </summary>
    /// <param name="dwMilliseconds"></param>
    /// <returns></returns>
    public long SetCount(long dwMilliseconds)
    {
        _counterSetup = dwMilliseconds;
        return _counterSetup;
    }

    /// <summary>
    /// 获取计时器设定的计时值
    /// </summary>
    /// <returns></returns>
    public long GetCount()
    {
        return _counterSetup;
    }

    /// <summary>
    /// 获取计时器经过的毫秒数，默认时获取时间后自动重置计时器
    /// </summary>
    /// <param name="bReset"></param>
    /// <returns></returns>
    public long GetTime(bool bReset = true)
    {
        long count = 0;
        QueryPerformanceCounter(ref count);
        long num = count - _counterResetTime;
        if (bReset)
        {
            _counterResetTime = count;
            _counterSetup = 0L;
        }
        return (long)(num * 1000.0 / _counterFrequency );
    }
}
#endregion