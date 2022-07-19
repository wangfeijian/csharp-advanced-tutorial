// See https://aka.ms/new-console-template for more information
using SingletonTemplateTest;

Console.WriteLine("Hello, World!");
Console.WriteLine("输入q，退出线程");
Console.WriteLine("输入z，退出进程");

TestSingleton.GetInstance().ShowName();
TestSingleton.GetInstance().StartMonitor();

// 报错不能直接new()
//TestSingleton test = new TestSingleton();
// 也不能直接通过基类来new()
//SingletonTemplate<TestSingleton> test = new SingletonTemplate<TestSingleton>();

while (true)
{
    Console.WriteLine("进程运行！！");
    Thread.Sleep(500);
    var key = Console.ReadKey();
    Console.WriteLine();
    if (key.Key == ConsoleKey.Q)
    {
        TestSingleton.GetInstance().StopMonitor();
        Console.WriteLine("线程退出");
    }
    else if (key.Key == ConsoleKey.Z)
    {
        Console.WriteLine("进程退出");
        break;
    }
}