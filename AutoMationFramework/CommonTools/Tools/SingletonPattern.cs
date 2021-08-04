/*********************************************************************
*           Author:         wangfeijian                              *
*                                                                    *
*           CreatTime:      2021-07-26                               *
*                                                                    *
*           ModifyTime:     2021-07-28                               *
*                                                                    *
*           Email:          wangfeijianhao@163.com                   *
*                                                                    *
*           Description:    Singleton pattern class                  *
*********************************************************************/

using System;
using System.Threading;
using System.Windows.Controls;

namespace CommonTools.Tools
{
    /// <summary>
    /// 单例模板类
    /// </summary>
    /// <typeparam name="T">实际类型</typeparam>
    public class SingletonPattern<T> where T : class, new()
    {
        /// <summary>
        /// 线程互斥对像
        /// </summary>
        private static readonly object Syslock = new object();

        /// <summary>
        /// 用来显示站位运行日志的列表框
        /// </summary>
        private Control _logListBox;

        /// <summary>
        /// 实例对像
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// 实例引用
        /// </summary>
        private static T _instance;

        /// <summary>
        /// 线程是否运行中
        /// </summary>
        protected bool BRunThread;

        /// <summary>
        /// 日志信息显示事件
        /// </summary>
        public event LogHandler LogEvent;

        /// <summary>
        /// 触发日志显示事件
        /// </summary>
        /// <param name="strLog"></param>
        /// <param name="level">显示的等级,便于颜色提示</param>
        public void ShowLog(string strLog, LogLevel level = LogLevel.Info)
        {
            // ISSUE: reference to a compiler-generated field
            if (LogEvent == null || _logListBox == null)
                return;
            // ISSUE: reference to a compiler-generated field
            LogEvent(_logListBox, strLog, level);
        }

        /// <summary>
        /// 设置日志要显示在哪个列表框上
        /// </summary>
        /// <param name="logListBox"></param>
        public void SetLogListBox(Control logListBox)
        {
            _logListBox = logListBox;
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static T GetInstance()
        {
            if (_instance == null)
            {
                lock (Syslock)
                {
                    if (_instance == null)
                        _instance = Activator.CreateInstance<T>();
                }
            }

            return _instance;
        }

        /// <summary>
        /// 线程函数
        /// </summary>
        public virtual void ThreadMonitor()
        {
            if (!BRunThread)
                return;
            Thread.Sleep(100);
        }

        /// <summary>
        /// 开始监视线程
        /// </summary>
        public void StartMonitor()
        {
            if (_thread == null)
                _thread = new Thread(ThreadMonitor);
            if ((uint) _thread.ThreadState <= 0U)
                return;
            BRunThread = true;
            _thread.Start();
        }

        /// <summary>
        /// 结束监视线程
        /// </summary>
        public void StopMonitor()
        {
            if (_thread == null)
                return;
            BRunThread = false;
            if (!_thread.Join(5000))
                _thread.Abort();
            _thread = null;
        }
    }
}
