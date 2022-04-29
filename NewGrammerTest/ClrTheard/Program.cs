bool _threadTest = false;
bool _cancellationTokenTest = false;
bool _parallelTest = false;
bool _timerTest = false;
bool _volatile = true;
bool _stop = false;
object _lock = new object();
#region Thread Test
if (_threadTest)
{
    Console.WriteLine("Main thread: starting a dedicated thread to do an asynchronous operation");

    //Thread dedicatedThread = new Thread(ComputerBoundOp);
    //dedicatedThread.Start(5);
    ThreadPool.QueueUserWorkItem(ComputerBoundOp, 5);

    Console.WriteLine("Doing other work here...");
    Thread.Sleep(10000);

    //dedicatedThread.Join();
    Console.WriteLine("Hit <Enter> to end this program...");

    static void ComputerBoundOp(object state)
    {
        Console.WriteLine($"In ComputerBoundOp: state={state}");
        Thread.Sleep(1000);
    }
}

#endregion

#region CancellationToken Test/
if (_cancellationTokenTest)
{
    CancellationTokenSource cts = new CancellationTokenSource();
    //cts.Token.Register(() => Console.WriteLine("子线程结束后回调1"),false);
    //cts.Token.Register(() => Console.WriteLine("子线程结束后回调2"),false);
    //cts.Token.Register(() => Console.WriteLine("子线程结束后回调3"),false);
    ThreadPool.QueueUserWorkItem(o => { TestCancellationToken(cts.Token); });

    // 子线程在5秒后自动退出
    //cts.CancelAfter(5000);
    if (Console.ReadKey().Key == ConsoleKey.Escape)
    {
        // 手动让子线程退出
        cts.Cancel();
    }

    Console.WriteLine("子线程退出.....");

    static void TestCancellationToken(CancellationToken clt)
    {
        int i = 0;
        while (true)
        {
            i++;
            if (clt.IsCancellationRequested)
            {
                Console.WriteLine("外部使用CancellationToken设置取消，线程结束！！！");
                break;
            }
#if DEBUG
            Console.WriteLine("Debug 运行");
#endif
            Console.WriteLine($"点击 ESC 让子线程退出...");
            Console.WriteLine($"线程运行{i}次...");
            Thread.Sleep(1000);
        }
    }
}
#endregion

#region Parallel
if (_parallelTest)
{
    CancellationTokenSource cts = new CancellationTokenSource();

    Parallel.Invoke(
        () => TestMethodForParallel(cts.Token, ConsoleColor.Red, _lock),
        () => TestMethodForParallel(cts.Token, ConsoleColor.Green, _lock),
        () => TestMethodForParallel(cts.Token, ConsoleColor.Yellow, _lock),
        () =>
        {
            if (Console.ReadKey().Key == ConsoleKey.Escape)
            {
                // 手动让子线程退出
                cts.Cancel();
            }
        }
        );

    static void TestMethodForParallel(CancellationToken clt, ConsoleColor consoleColor, object _lock)
    {
        while (!clt.IsCancellationRequested)
        {
            lock (_lock)
            {
                Console.ForegroundColor = consoleColor;
                Thread.Sleep(1000);
                Console.WriteLine("Test Parallel");
            }
        }
    }
}
#endregion

#region Timer
if (_timerTest)
{
    TimerTest();
    Console.ReadKey();
    static async void TimerTest()
    {
        while (true)
        {
            Console.WriteLine("Test timer...");
            await Task.Delay(1000);
        }
    }
}
#endregion

#region volatile
if (_volatile)
{
    Console.WriteLine("Main: letting worker run for 5 seconds");
    Thread t = new Thread(Worker);
    t.Start();
    _stop = true;
    Console.WriteLine("Main: waiting for worker to stop");
    t.Join();
    int x = 10;
    Interlocked.Increment(ref x);
    Console.WriteLine($"{x}");
    void Worker()
    {
        int x = 0;
        while (!_stop) x++;
        Console.WriteLine($"Worker: stopped when x = {x}");
    }
}
#endregion