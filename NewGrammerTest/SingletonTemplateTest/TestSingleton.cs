using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingletonTemplateTest
{
    public class TestSingleton : SingletonTemplate<TestSingleton>
    {
        /// <summary>
        /// 保护的构造函数，只能通过基类的GetInstance获取，防止外部new
        /// </summary>
        private TestSingleton() { }

        public void ShowName()
        {
            Console.WriteLine(GetType());
        }

        /// <summary>
        /// 线程函数
        /// 如果使用Task.Delay来阻塞的话，需要加async
        /// 因为Task.Delay是异步阻塞方法
        /// 需要调用基类的方法来判断线程是否关闭
        /// </summary>
        /// <param name="obj"></param>
        protected override async void ThreadMonitor()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("线程运行中!!");
                    Console.WriteLine("线程阻塞1000ms前运行。");
                    await Task.Delay(2000, cts.Token);

                    Console.WriteLine("线程阻塞1000ms后运行。");
                }
                catch (OperationCanceledException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
        }
    }
}
