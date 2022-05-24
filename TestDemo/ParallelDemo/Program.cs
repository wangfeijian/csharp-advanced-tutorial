using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ParallelDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            object o = new object();
            for (int i = 0; i < 100; i++)
            {
                Parallel.Invoke
                (() =>

                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("我是蓝色");
                    Console.WriteLine("我是蓝色");
                    //Thread.Sleep(1000);
                },
                () =>
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("我是黄色");
                    Console.WriteLine("我是黄色");
                    // Thread.Sleep(1000);
                },
                () =>
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("我是红色");
                    Console.WriteLine("我是红色");
                    //Thread.Sleep(1000);
                }
                );
                //Console.ReadLine();
                Console.WriteLine();
                Console.WriteLine("exculate");
            }
            Console.ReadLine();
        }
    }
}
