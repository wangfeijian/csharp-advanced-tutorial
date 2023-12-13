using System;
using System.Threading;

namespace EventTest
{
    delegate void TestEventHandler(string mesg);
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start invoke!!");
            Test t = new Test();
            t.TestEvent += T_TestEvent;

            t.Show("invoke");
            Console.WriteLine("invoke done");

            Console.WriteLine("Start BeginInvoke!!");

            for (int i = 0; i < 10; i++)
            {
                t.ShowBegin($"begininvoke{i}");
            }

            Console.WriteLine("BeginInvoke done");
            Console.ReadLine();
        }

        private static void T_TestEvent(string mesg)
        {
            Console.WriteLine($"recevie {mesg}");
            Thread.Sleep(1000);
            Console.WriteLine($"{mesg} in event done!");
        }
    }

    class Test
    {
        public event TestEventHandler TestEvent;

        public void Show(string msg)
        {
            TestEvent?.Invoke(msg);
        }

        public void ShowBegin(string msg)
        {
            TestEvent?.BeginInvoke(msg, null, null);
        }
    }
}
