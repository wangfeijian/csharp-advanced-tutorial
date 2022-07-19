using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingletonTemplateTest
{
    public class SingletonTemplate<T> where T : class
    {
        /// <summary>
        /// 保护的构造函数，只允许子类访问
        /// </summary>
        protected SingletonTemplate() { }

        /// <summary>
        /// 线程互斥对像
        /// </summary>
        private static readonly object syslock = new object();

        /// <summary>
        /// 线程取消对象
        /// </summary>
        protected CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// 实例对像
        /// </summary>
        private Task? _task = null;

        /// <summary>
        /// 实例引用
        /// </summary>
        private static T? _instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns>T</returns>
        public static T GetInstance()
        {
            if (_instance == null)
            {
                lock (syslock)
                {
                    if (_instance == null)
                        _instance = Activator.CreateInstance(typeof(T), true) as T;
                }
            }
            return _instance;
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        protected async virtual void ThreadMonitor()
        {
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// 开始监视线程
        /// </summary>
        public void StartMonitor()
        {
            if (_task == null)
                _task = new Task(ThreadMonitor, cts.Token);
            _task.Start();
        }

        /// <summary>
        /// 结束监视线程
        /// </summary>
        public void StopMonitor()
        {
            if (_task == null)
                return;
            cts.Cancel();
            _task = null;
        }
    }
}
