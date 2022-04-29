using System.Diagnostics;


//long l1, l2, l3, l4, l5, l6, l7;
//long number;
//long b1, b2, b3, b4, b5, b6, b7;

//Stopwatch stopwatch = new Stopwatch();

//stopwatch.Start();
//for (long i = 0; i < 1000000000; i++)
//{
//    number = i;
//}
//for (long i = 0; i < 1000000000; i++)
//{
//    number = i;
//}
//stopwatch.Stop();

//Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);

// 堆上对象地址的获取 
string str1 = "hello";
string str2 = "hello2";
unsafe
{

    fixed (char* str1p = str1)
    fixed (char* str2p = str2)
    {

        Console.WriteLine($"Address of str1:0x{(int)str1p:x}");
        Console.WriteLine($"Address of str2:0x{(int)str2p:x}");
    }
}

// 栈上对象地址的获取
int a = 15;
unsafe
{
    int* p = &a;
    Console.WriteLine(*p);
    Console.WriteLine($"Address of a:0x{(int)p:x}");
}