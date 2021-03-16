using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericDemo
{
    public class Montior
    {
        public static void ShowTime()
        {
            // 只是单纯调用方法并传参，其它什么都不操作，测试性能
            int iValue = 100;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 100000000; i++)
            {
                CommonRun(iValue);
            }
            watch.Stop();
            var commonTime = watch.Elapsed.TotalSeconds;

            watch.Reset();
            watch.Start();
            for (int i = 0; i < 100000000; i++)
            {
                GenericRun(iValue);
            }
            watch.Stop();
            var genericTime = watch.Elapsed.TotalSeconds;

            watch.Reset();
            watch.Start();
            for (int i = 0; i < 100000000; i++)
            {
                ObjectRun(iValue);
            }
            watch.Stop();
            var objectTime = watch.Elapsed.TotalSeconds;

            Console.WriteLine($"普通方法运行时间：{commonTime}");
            Console.WriteLine($"泛型方法运行时间：{genericTime}");
            Console.WriteLine($"object方法运行时间：{objectTime}");
        }

        static void CommonRun(int iParam)
        {

        }

        static void GenericRun<T>(T tParam)
        {

        }

        static void ObjectRun(object oParam)
        {

        }
    }
}
