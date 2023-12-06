#region << 版 本 注 释 >>
/*----------------------------------------------------------------
 * 版权所有 (c) 2023 wangfeijian 保留所有权利。
 * CLR版本：4.0.30319.42000
 * 机器名称：WANGFEIJIAN
 * 公司名称：wangfeijian
 * 命名空间：Soso.Common.SystemHelp
 * 唯一标识：11ed81f3-875f-4781-a0ac-a6ade5c198cf
 * 文件名：PerformanceTimer
 * 当前用户域：WANGFEIJIAN
 * 
 * 创建者：王飞箭 wangfeijian
 * 电子邮箱：wangfeijianhao@163.com
 * 创建时间：7/13/2023 9:13:42 AM
 * 版本：V1.0.0
 * 描述：
 *
 * ----------------------------------------------------------------
 * 修改人：
 * 时间：
 * 修改说明：
 *
 * 版本：V1.0.1
 *----------------------------------------------------------------*/
#endregion << 版 本 注 释 >>

using System;
using System.ComponentModel;
using System.Threading;
using Windows.Win32;

namespace Soso.Common.SystemHelp
{
    /// <summary>
    /// 高性能计时器
    /// </summary>
    /// <remarks>
    /// 支持记录经过多少时间，总时间<br/>
    /// 可以通过<see cref="TimeSpan"/>查看记录的时间，也可以通过具体的单位查看记录的时间。比如：纳秒、微秒、毫秒、秒。
    /// </remarks>
    public class PerformanceTimer
    {
        private bool _isRunning = false;
        private long _startTime;
        private long _endTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <exception cref="Win32Exception">系统不支持</exception>
        public PerformanceTimer()
        {
            _startTime = 0;
            _endTime = 0;

            if (!PInvoke.QueryPerformanceFrequency(out long _))
            {
                throw new Win32Exception("System not support performance counter!");
            }
        }

        /// <summary>
        /// 计时器是否运行
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// 计时器经过的时间间隔
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                if (_isRunning)
                {
                    PInvoke.QueryPerformanceCounter(out _endTime);
                }

                return new TimeSpan(_endTime - _startTime);
            }
        }

        /// <summary>
        /// 计时器经过时间，以秒为单位
        /// </summary>
        public double ElapsedSeconds => Elapsed.TotalSeconds;

        /// <summary>
        /// 计时器经过时间，以毫秒为单位
        /// </summary>
        public double ElapsedMilliseconds => Elapsed.TotalMilliseconds;

        /// <summary>
        /// 计时器经过时间，以微秒为单位
        /// </summary>
        public double ElapsedMicroseconds => ElapsedMilliseconds * 1000;

        /// <summary>
        /// 计时器经过时间，以纳秒为单位
        /// </summary>
        public double ElapsedNanoseconds => ElapsedMicroseconds * 1000;

        /// <summary>
        /// 计时器开始计时
        /// </summary>
        public void Start()
        {
            _isRunning = true;
            Thread.Sleep(0);
            PInvoke.QueryPerformanceCounter(out _startTime);
        }

        /// <summary>
        /// 计时器重新开始计时
        /// </summary>
        public void Restart()
        {
            _startTime = 0;
            _endTime = 0;
            Start();
        }

        /// <summary>
        /// 计时器停止计时
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            PInvoke.QueryPerformanceCounter(out _endTime);
        }

        /// <summary>
        /// 获取一个新的计时器，并启动
        /// </summary>
        /// <returns>返回<see cref="PerformanceTimer"/></returns>
        public static PerformanceTimer StartNew()
        {
            var timer = new PerformanceTimer();
            timer.Start();
            return timer;
        }

        /// <summary>
        /// 传入一个任务，并计算该任务运行的总时间间隔
        /// </summary>
        /// <param name="action">任务委托</param>
        /// <returns>返回<see cref="TimeSpan"/></returns>
        public static TimeSpan Execute(Action action)
        {
            var timer = new PerformanceTimer();
            timer.Start();
            action?.Invoke();
            timer.Stop();
            return timer.Elapsed;
        }
    }
}