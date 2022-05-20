
using System.Reflection;
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

TestClass test = new TestClass();
Type t = test.GetType();
var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
foreach (var method in methods)
{
    Console.WriteLine(method.Name);
    method.Invoke(test, null);
}
class TestClass
{
    public void Test1()
    {
        Console.WriteLine("Test1 Invoke");
    }

    public void Test2()
    {
        Console.WriteLine("Test2 Invoke");
    }
}
#endregion